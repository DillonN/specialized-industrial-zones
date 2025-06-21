using Colossal.Logging;
using Colossal.PSI.Environment;
using Game;
using Game.Prefabs;
using Game.SceneFlow;
using Game.UI.InGame;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

    private static readonly JsonSerializerSettings JsonSettings = new()
    {
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        Formatting = Formatting.Indented,
        Converters = [new StringEnumConverter()]
    };

    private DateTimeOffset _lastFileCheck;
    private DateTimeOffset _lastFileModifiedTimestamp;

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

        var zonesText = string.Join('\n', _initialZones.Select(z => z.name));
        _log.InfoFormat("Loaded {0} zones:\n{1}", _initialZones.Length, zonesText);
    }

    protected override void OnUpdate()
    {
        var now = DateTimeOffset.UtcNow;
        if ((now - _lastFileCheck).TotalSeconds < 5)
        {
            return;
        }

        _lastFileCheck = now;

        if (!File.Exists(ZoneFilePath))
        {
            var defaultSpecs = GenerateDefaultSpecs().ToList();
            SaveZoneFile(defaultSpecs);
        }

        var lastModified = File.GetLastWriteTimeUtc(ZoneFilePath);

        if (lastModified == _lastFileModifiedTimestamp)
        {
            _log.Trace("No changes detected in zone file, skipping update.");
            return;
        }

        _lastFileModifiedTimestamp = lastModified;
        _log.InfoFormat("Reloading specialized zones from file '{0}'.", ZoneFilePath);
        var sw = Stopwatch.StartNew();
        
        var specs = LoadZoneFile();
        if (specs == null)
        {
            _log.Warn("No specs loaded, cannot provision zones.");
            return;
        }

        var numLoaded = 0;
        foreach (var spec in specs)
        {
            try
            {
                var baseZone = _initialZones.FirstOrDefault(z => z.name == spec.BaseZoneName)
                    ?? throw new Exception($"Base zone '{spec.BaseZoneName}' not found for specialized zone '{spec.Name}'");
                var combinedFilter = spec.CombinedFilter;

                var existing = false;
                var success = false;
                if (!_provisionedZones.TryGetValue(spec.ID, out var zone))
                {
                    zone = CreateZonePrefab(spec, baseZone, combinedFilter);
                    success = _prefabSystem.AddPrefab(zone);
                }
                else
                {
                    existing = true;
                    var replacementZone = CreateZonePrefab(spec, baseZone, combinedFilter);
                    _prefabSystem.UpdatePrefab(replacementZone);
                    success = true;
                }

                if (zone == null || !success)
                {
                    _log.Error($"Failed to provision zone: {spec.ID}");
                    continue;
                }

                var buildings = CopyBuildings(spec, zone, baseZone, combinedFilter);
                foreach (var building in buildings)
                {
                    try
                    {
                        if (existing)
                        {
                            _prefabSystem.UpdatePrefab(building);
                            success = true;
                        }
                        else
                        {
                            success = _prefabSystem.AddPrefab(building);
                        }

                        if (!success)
                        {
                            _log.Error($"Failed to add specialized building prefab {building.name} to PrefabSystem. It may already exist or be invalid.");
                            continue;
                        }
                    }
                    catch (Exception e)
                    {
                        _log.Error($"Failed to add building prefab {building.name}: {e}");
                    }
                }

                ApplyUILabels(spec, baseZone, zone);

                _provisionedZones[spec.ID] = zone;
                _log.Debug($"Provisioned zone: {spec.ID} ({spec.Name})");
                numLoaded++;
            }
            catch (Exception e)
            {
                _log.Error(e, $"Failed to provision zone {spec.Name}");
                continue;
            }
        }

        sw.Stop();
        _log.InfoFormat("{0} specialized zones reloaded in {1} ms.", numLoaded, sw.ElapsedMilliseconds);
    }

    private static readonly string ZoneFilePath = Path.Combine(EnvPath.kUserDataPath, "ModsData", "SpecializedZones", "SpecializedZones.json");

    private List<SpecializedZoneSpec>? LoadZoneFile()
    {
        if (!File.Exists(ZoneFilePath))
        {
            return null;
        }

        try
        {
            var json = File.ReadAllText(ZoneFilePath);
            return JsonConvert.DeserializeObject<List<SpecializedZoneSpec>>(json, JsonSettings);
        }
        catch (Exception e)
        {
            Mod.log.Error(e, "Failed to load specialized zones from file.");
            return null;
        }
    }

    private void SaveZoneFile(List<SpecializedZoneSpec> specs)
    {
        try
        {
            var directory = Path.GetDirectoryName(ZoneFilePath) ?? throw new Exception("Zone file directory is null");
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var json = JsonConvert.SerializeObject(specs, JsonSettings);
            File.WriteAllText(ZoneFilePath, json);
            _log.Info("Specialized zones saved to file.");
        }
        catch (Exception e)
        {
            _log.Error(e, "Failed to save specialized zones to file.");
        }
    }

    private IEnumerable<SpecializedZoneSpec> GenerateDefaultSpecs()
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

    private ZonePrefab? CreateZonePrefab(SpecializedZoneSpec spec, ZonePrefab sourceZone, SpecializedZoneFilterSpec combinedFilter)
    {
        var zone = CreateSpecializedZonePrefab(spec, sourceZone, combinedFilter);

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

        return zone;
    }

    private IEnumerable<BuildingPrefab> CopyBuildings(SpecializedZoneSpec spec, ZonePrefab zone, ZonePrefab sourceZone, SpecializedZoneFilterSpec combinedFilter)
    {
        if (!_spawnableBuildingsByZone.TryGetValue(sourceZone, out var buildings))
        {
            _log.Error($"No spawnable buildings found for zone {sourceZone.name}");
            yield break;
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

                if (combinedFilter.IndustryType == IndustryType.Manufacturing && !hasRelevantManufactured)
                {
                    // Only manufacturing buildings are allowed in this specialization
                    continue;
                }

                if (combinedFilter.IndustryType == IndustryType.Warehouses && !hasRelevantStored)
                {
                    // Only warehouse buildings are allowed in this specialization
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

            yield return building;
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

        return originalName + spec.ID;
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
