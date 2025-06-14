using Colossal.Logging;
using Game;
using Game.Prefabs;
using Game.SceneFlow;
using Game.UI.InGame;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpecializedIndustryZones;

internal partial class SpecializedZoningSystem : GameSystemBase
{
    private readonly ILog _log = Mod.log;

    private readonly Dictionary<ZonePrefab, BuildingPrefab[]> _spawnableBuildingsByZone = [];
    private readonly Dictionary<string, ZonePrefab> _provisionedZones = [];

    private PrefabSystem _prefabSystem = default!;
    private PrefabUISystem _prefabUISystem = default!;

    private List<PrefabBase> _allPrefabs = default!;
    private ZonePrefab[] _initialZones = default!;

    protected override void OnCreate()
    {
        base.OnCreate();

        _prefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();
        _prefabUISystem = World.GetOrCreateSystemManaged<PrefabUISystem>();

        _allPrefabs = typeof(PrefabSystem)
            .GetField("m_Prefabs", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.GetValue(_prefabSystem) as List<PrefabBase>
            ?? throw new Exception("Could not access m_Prefabs field in PrefabSystem, likely broken by game update");

        _initialZones = [.._allPrefabs.OfType<ZonePrefab>()];
        foreach (var zone in _initialZones)
        {
            var spawnableBuildings = _allPrefabs
                .OfType<BuildingPrefab>()
                .Where(b => b.components.OfType<SpawnableBuilding>().Any(c => c.m_ZoneType == zone))
                .ToArray();
            _spawnableBuildingsByZone[zone] = spawnableBuildings;
        }

        var specs = GetZoneSpecs();
        foreach (var spec in specs)
        {
            if (_provisionedZones.ContainsKey(spec.ID))
            {
                continue;
            }

            try
            {
                var zone = ProvisionZone(spec);
                if (zone != null)
                {
                    _provisionedZones[spec.ID] = zone;
                    _log.Debug($"Provisioned zone: {spec.Name}");
                }
            }
            catch (Exception e)
            {
                _log.Error(e, $"Failed to provision zone {spec.Name}");
                continue;
            }
        }
    }

    protected override void OnUpdate()
    {
        // TODO dynamic updates
    }

    private IEnumerable<SpecializedZoneSpec> GetZoneSpecs()
    {
        foreach (var zone in SpecializedZoneSpecSource.AllZones)
        {
            var prefix = zone.Type switch
            {
                IndustryType.General => "Manufacturing and warehouses",
                IndustryType.Manufacturing => "Manufacturing only",
                IndustryType.Warehouses => "Warehouses only",
                _ => "Unknown"
            };

            var includedResources = string.Join(", ", zone.Filter);
            var description = $"{prefix} - {includedResources}";

            var iconBase = zone.Type switch
            {
                IndustryType.General or IndustryType.Manufacturing => "ZoneIndustrialManufacturing",
                IndustryType.Warehouses => "ZoneIndustrialWarehouses",
                _ => throw new ArgumentOutOfRangeException(nameof(zone.Type), zone.Type, "Invalid type")
            };

            var iconUri = $"coui://{Mod.HostName}/{iconBase}_{zone.Specialization}.svg";

            var spec = new SpecializedZoneSpec(zone.ID, zone.Name, "Industrial Manufacturing", description)
            {
                Color = zone.Color,
                IconUri = iconUri,
                Filters =
                [
                    new SpecializedZoneFilterSpec
                    {
                        IndustryType = zone.Type,
                        ManufacturedResources = zone.Filter.ToHashSet(),
                        StoredResources = zone.Filter.ToHashSet(),
                        SoldResources = zone.Filter.ToHashSet()
                    }
                ]
            };

            yield return spec;
        }
    }

    private ZonePrefab? ProvisionZone(SpecializedZoneSpec spec)
    {
        var baseZone = _initialZones.FirstOrDefault(z => z.name == spec.BaseZoneName)
            ?? throw new Exception($"Base zone '{spec.BaseZoneName}' not found for specialized zone '{spec.Name}'");

        var combinedFilter = spec.CombinedFilter;
        var zone = CreateSpecializedZonePrefab(spec, baseZone, combinedFilter);
        if (zone == null)
        {
            return null;
        }

        CopyBuildings(spec, zone, baseZone, combinedFilter);
        ApplyUILabels(spec, baseZone, zone);

        return zone;
    }

    private ZonePrefab? CreateSpecializedZonePrefab(
        SpecializedZoneSpec spec,
        ZonePrefab sourceZone,
        SpecializedZoneFilterSpec combinedFilter)
    {
        var zone = Clone(sourceZone, spec);
        zone.m_Edge = spec.Color;

        if (zone.name == sourceZone.name)
        {
            _log.Warn($"Zone {sourceZone.name} was not renamed for specialization {spec.ID} and will be ignored.");
            return null;
        }

        var zoneProps = zone.GetComponent<ZoneProperties>();

        if (combinedFilter.ManufacturedResources != null)
            zoneProps.m_AllowedManufactured = [.. combinedFilter.ManufacturedResources.Intersect(zoneProps.m_AllowedManufactured)];
        if (combinedFilter.StoredResources != null)
            zoneProps.m_AllowedStored = [.. combinedFilter.StoredResources.Intersect(zoneProps.m_AllowedStored)];
        if (combinedFilter.SoldResources != null)
            zoneProps.m_AllowedSold = [.. combinedFilter.SoldResources.Intersect(zoneProps.m_AllowedSold)];

        if (spec.IconUri != null)
        {
            var uiObj = zone.GetComponent<UIObject>();
            uiObj.m_Icon = spec.IconUri;
        }

        HandleObsoleteIdentifiers(sourceZone, zone, spec);

        _prefabSystem.AddPrefab(zone);
        return zone;
    }

    private void CopyBuildings(SpecializedZoneSpec spec, ZonePrefab zone, ZonePrefab sourceZone, SpecializedZoneFilterSpec combinedFilter)
    {
        if (!_spawnableBuildingsByZone.TryGetValue(sourceZone, out var buildings))
        {
            _log.Error($"No spawnable buildings found for zone {sourceZone.name}");
            return;
        }

        foreach (var sourceBuilding in buildings)
        {
            if (sourceBuilding.TryGet<BuildingProperties>(out var sourceBuildingProperties))
            {
                var hasRelevantManufactured = sourceBuildingProperties.m_AllowedManufactured.Intersect(combinedFilter.ManufacturedResources).Any();
                var hasRelevantSold = sourceBuildingProperties.m_AllowedSold.Intersect(combinedFilter.SoldResources).Any();
                var hasRelevantStored = sourceBuildingProperties.m_AllowedStored.Intersect(combinedFilter.StoredResources).Any();

                if (!hasRelevantManufactured && !hasRelevantSold && !hasRelevantStored)
                {
                    // Not applicable to this specialization
                    continue;
                }
            }

            var building = Clone(sourceBuilding, spec);

            if (building.name == sourceBuilding.name)
            {
                _log.Warn($"Building {sourceBuilding.name} was not renamed for specialization {spec.ID} and will be ignored.");
                continue;
            }

            var spawnableBuilding = building.GetComponent<SpawnableBuilding>();
            spawnableBuilding.m_ZoneType = zone;

            if (building.TryGet<BuildingProperties>(out var buildingProperties))
            {
                if (combinedFilter.ManufacturedResources != null)
                    buildingProperties.m_AllowedManufactured = [..combinedFilter.ManufacturedResources.Intersect(buildingProperties.m_AllowedManufactured)];
                if (combinedFilter.StoredResources != null)
                    buildingProperties.m_AllowedStored = [..combinedFilter.StoredResources.Intersect(buildingProperties.m_AllowedStored)];
                if (combinedFilter.SoldResources != null)
                    buildingProperties.m_AllowedSold = [.. combinedFilter.SoldResources.Intersect(buildingProperties.m_AllowedSold)];
            }

            HandleObsoleteIdentifiers(sourceBuilding, building, spec);

            try
            {
                _prefabSystem.AddPrefab(building);
            }
            catch (Exception e)
            {
                _log.Error($"Failed to add building prefab {building.name}: {e}");
            }
        }
    }

    private void ApplyUILabels(SpecializedZoneSpec spec, ZonePrefab industrialZone, ZonePrefab zone)
    {
        if (!_prefabSystem.TryGetEntity(zone, out var zoneEntity))
        {
            _log.Error($"Could not find entity for zone prefab {zone.name}");
            return;
        }

        if (!_prefabSystem.TryGetEntity(industrialZone, out var industrialZoneEntity))
        {
            _log.Error($"Could not find entity for industrial zone prefab {industrialZone.name}");
            return;
        }

        var dictionary = new SpecializedIndustrialZoneLocaleDictionary(
            spec,
            industrialZoneEntity,
            zoneEntity,
            _prefabUISystem,
            _log);

        foreach (var localeId in GameManager.instance.localizationManager.GetSupportedLocales())
        {
            GameManager.instance.localizationManager.AddSource(localeId, dictionary);
        }
    }

    private static T Clone<T>(T source, SpecializedZoneSpec spec)
        where T : PrefabBase
    {
        var newName = GetUpdatedName(source.name, spec);
        var clone = (T)source.Clone(newName);
        return clone;
    }

    private static string GetUpdatedName(string originalName, SpecializedZoneSpec spec)
    {
        if (originalName.Contains("IndustrialManufacturing"))
        {
            return originalName.Replace("IndustrialManufacturing", $"SpecializedIndustrialManufacturing{spec.ID}");
        }

        if (originalName.Contains("Industrial Manufacturing"))
        {
            return originalName.Replace("Industrial Manufacturing", $"SpecializedIndustrialManufacturing{spec.ID}");
        }

        if (originalName.Contains("IndustrialStorage"))
        {
            return originalName.Replace("IndustrialStorage", $"SpecializedIndustrialStorage{spec.ID}");
        }

        if (originalName.Contains("Warehouses"))
        {
            return originalName.Replace("Warehouses", $"SpecializedIndustrialStorage{spec.ID}");
        }

        if (originalName.Contains("Industrial"))
        {
            return originalName.Replace("Industrial", $"SpecializedIndustrial{spec.ID}");
        }

        throw new Exception($"Could not determine how to rename prefab {originalName} for specialization {spec.ID}.");
    }

    private void HandleObsoleteIdentifiers(PrefabBase sourcePrefab, PrefabBase newPrefab, SpecializedZoneSpec spec)
    {
        var obsoleteIdentifiers = newPrefab.AddOrGetComponent<ObsoleteIdentifiers>();

        // This is required due to the rename in 0.2.1, may be removed in future versions
        var legacyName = sourcePrefab.name.Replace("Industrial", $"Specialized Industrial {spec.Name}");
        var legacyPrefabInfo = new PrefabIdentifierInfo
        {
            m_Name = legacyName,
            m_Type = newPrefab.GetType().Name
        };

        if (obsoleteIdentifiers.m_PrefabIdentifiers == null)
        {
            obsoleteIdentifiers.m_PrefabIdentifiers = [legacyPrefabInfo];
        }
        else
        {
            var extraLegacyInfos = new List<PrefabIdentifierInfo>();
            foreach (var identifier in obsoleteIdentifiers.m_PrefabIdentifiers)
            {
                var newName = GetUpdatedName(identifier.m_Name, spec);
                if (newName == identifier.m_Name)
                    _log.Warn($"Identifier {identifier.m_Name} for prefab {sourcePrefab.name} was not updated for specialization {spec.ID}.");

                identifier.m_Name = newName;
                extraLegacyInfos.Add(new PrefabIdentifierInfo
                {
                    m_Name = identifier.m_Name.Replace("Industrial", $"Specialized Industrial {spec.Name}"),
                    m_Type = identifier.m_Type
                });
            }

            obsoleteIdentifiers.m_PrefabIdentifiers =
            [
                ..obsoleteIdentifiers.m_PrefabIdentifiers,
                legacyPrefabInfo,
                ..extraLegacyInfos
            ];
        }
    }
}
