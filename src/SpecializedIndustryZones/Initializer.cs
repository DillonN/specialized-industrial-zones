using Colossal.Logging;
using Game.Prefabs;
using Game.SceneFlow;
using Game.UI.InGame;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SpecializedIndustryZones;

internal class Initializer(
    PrefabSystem _prefabSystem,
    PrefabUISystem _prefabUISystem,
    ILog _log)
{ 
    private readonly List<PrefabBase> _allPrefabs = typeof(PrefabSystem)
        .GetField("m_Prefabs", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
        ?.GetValue(_prefabSystem) as List<PrefabBase>
        ?? throw new Exception("Could not access m_Prefabs field in PrefabSystem");

    public void Initialize()
    {
        if (!TryGetIndustrialZonePrefab(out var industrialZone))
        {
            _log.Error("Could not find Industrial Manufacturing prefab.");
            return;
        }
        
        var buildings = GetBuildings(industrialZone).ToList();

        var sw = new Stopwatch();
        foreach (var spec in SpecializedZoneSpec.AllZones)
        {
            sw.Restart();
            _log.TraceFormat("Creating specialized zone for {0} ({1})", spec.Name, spec.ID);
            var zone = CreateSpecializedZonePrefab(spec, industrialZone);
            if (zone == null)
            {
                _log.Warn($"Skipping specialization {spec.ID} due to couldn't create zone prefab.");
                continue;
            }

            CopyBuildings(spec, zone, buildings);
            ApplyUILabels(spec, industrialZone, zone);
            sw.Stop();
            _log.TraceFormat("Created specialized zone for {0} ({1}) in {2} ms", spec.Name, spec.ID, sw.ElapsedMilliseconds);
        }
    }

    private bool TryGetIndustrialZonePrefab(out ZonePrefab zone)
    {
        zone = _allPrefabs.OfType<ZonePrefab>().SingleOrDefault(p => p.name == "Industrial Manufacturing");
        return zone != null;
    }

    private IEnumerable<BuildingPrefab> GetBuildings(ZonePrefab zone) => _allPrefabs.OfType<BuildingPrefab>()
        .Where(b => b.components.OfType<SpawnableBuilding>().Any(c => c.m_ZoneType == zone));

    private ZonePrefab? CreateSpecializedZonePrefab(SpecializedZoneSpec spec, ZonePrefab sourceZone)
    {
        var zone = Clone(sourceZone, spec);
        zone.m_Edge = spec.Color;

        if (zone.name == sourceZone.name)
        {
            _log.Warn($"Zone {sourceZone.name} was not renamed for specialization {spec.ID} and will be ignored.");
            return null;
        }

        var zoneProps = zone.GetComponent<ZoneProperties>();
        zoneProps.m_AllowedManufactured = spec.ApplyFilter(zoneProps.m_AllowedManufactured, IndustryType.Manufacturing);
        zoneProps.m_AllowedSold = spec.ApplyFilter(zoneProps.m_AllowedSold);
        zoneProps.m_AllowedStored = spec.ApplyFilter(zoneProps.m_AllowedStored, IndustryType.Warehouses);

        var iconBase = spec.Type switch
        {
            IndustryType.General or IndustryType.Manufacturing => "ZoneIndustrialManufacturing",
            IndustryType.Warehouses => "ZoneIndustrialWarehouses",
            _ => throw new ArgumentOutOfRangeException(nameof(spec.Type), spec.Type, "Invalid type")
        };

        var uiObj = zone.GetComponent<UIObject>();
        uiObj.m_Icon = $"coui://{Mod.HostName}/{iconBase}_{spec.Specialization}.svg";

        HandleObsoleteIdentifiers(sourceZone, zone, spec);

        _prefabSystem.AddPrefab(zone);
        return zone;
    }

    private void CopyBuildings(SpecializedZoneSpec spec, ZonePrefab zone, List<BuildingPrefab> buildings)
    {
        foreach (var sourceBuilding in buildings)
        {
            if (sourceBuilding.TryGet<BuildingProperties>(out var sourceBuildingProperties))
            {
                var hasRelevantManufactured = sourceBuildingProperties.m_AllowedManufactured.Intersect(spec.Filter).Any();
                var hasRelevantSold = sourceBuildingProperties.m_AllowedSold.Intersect(spec.Filter).Any();
                var hasRelevantStored = sourceBuildingProperties.m_AllowedStored.Intersect(spec.Filter).Any();

                if (!hasRelevantManufactured && !hasRelevantSold && !hasRelevantStored)
                {
                    // Not applicable to this specialization
                    continue;
                }

                if (spec.Type == IndustryType.Manufacturing && !hasRelevantManufactured)
                {
                    // No relevant manufacturing capability
                    continue;
                }

                if (spec.Type == IndustryType.Warehouses && !hasRelevantStored)
                {
                    // No relevant warehouse capability
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
                buildingProperties.m_AllowedManufactured = spec.ApplyFilter(buildingProperties.m_AllowedManufactured, IndustryType.Manufacturing);
                buildingProperties.m_AllowedSold = spec.ApplyFilter(buildingProperties.m_AllowedSold);
                buildingProperties.m_AllowedStored = spec.ApplyFilter(buildingProperties.m_AllowedStored, IndustryType.Warehouses);
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
