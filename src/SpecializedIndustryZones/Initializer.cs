using System;
using System.Collections.Generic;
using System.Linq;
using Colossal.Logging;
using Game.Prefabs;
using Game.SceneFlow;
using Game.UI.InGame;

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
        
        foreach (var spec in SpecializedZoneSpec.AllZones)
        {
            var zone = CreateSpecializedZonePrefab(spec, industrialZone);
            CopyBuildings(spec, zone, buildings);
            ApplyUILabels(spec, industrialZone, zone);
        }
    }

    private bool TryGetIndustrialZonePrefab(out ZonePrefab zone)
    {
        zone = _allPrefabs.OfType<ZonePrefab>().SingleOrDefault(p => p.name == "Industrial Manufacturing");
        return zone != null;
    }

    private IEnumerable<BuildingPrefab> GetBuildings(ZonePrefab zone) => _allPrefabs.OfType<BuildingPrefab>()
        .Where(b => b.components.OfType<SpawnableBuilding>().Any(c => c.m_ZoneType == zone));

    private ZonePrefab CreateSpecializedZonePrefab(SpecializedZoneSpec spec, ZonePrefab sourceZone)
    {
        var zone = Clone(sourceZone, spec.Name);
        zone.m_Edge = spec.Color;

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
            
            var building = Clone(sourceBuilding, spec.Name);

            var spawnableBuilding = building.GetComponent<SpawnableBuilding>();
            spawnableBuilding.m_ZoneType = zone;

            if (building.TryGet<BuildingProperties>(out var buildingProperties))
            {
                buildingProperties.m_AllowedManufactured = spec.ApplyFilter(buildingProperties.m_AllowedManufactured, IndustryType.Manufacturing);
                buildingProperties.m_AllowedSold = spec.ApplyFilter(buildingProperties.m_AllowedSold);
                buildingProperties.m_AllowedStored = spec.ApplyFilter(buildingProperties.m_AllowedStored, IndustryType.Warehouses);
            }
            
            _prefabSystem.AddPrefab(building);
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

    private static T Clone<T>(T source, string name)
        where T : PrefabBase
    {
        var newName = source.name.Replace("Industrial", $"Specialized Industrial {name}");
        var clone = (T)source.Clone(newName);
        return clone;
    }
}
