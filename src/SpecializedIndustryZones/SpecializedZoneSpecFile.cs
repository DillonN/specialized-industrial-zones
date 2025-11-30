using Game.Economy;
using System.Collections.Generic;
using System.Linq;

namespace SpecializedIndustryZones;

public class SpecializedZoneSpecFile
{
    public static readonly string CurrentVersion = "v1alpha2";

    public string Version { get; set; } = CurrentVersion;

    public IDictionary<string, SpecializedZoneSpec> Zones { get; set; } = new Dictionary<string, SpecializedZoneSpec>();

    public bool UpgradeIfNeeded()
    {
        if (Version == CurrentVersion) return false;

        if (Version == "v1alpha1")
        {
            AddFishToAgricultureZones();
        }

        Version = CurrentVersion;
        return true;
    }

    private void AddFishToAgricultureZones()
    {
        foreach (var zone in Zones.Where(kvp => kvp.Key.StartsWith("Agriculture")))
        {
            zone.Value.Description = zone.Value.Description.Replace(
                "Cotton,",
                "Cotton, Fish,");

            foreach (var filter in zone.Value.Filters)
            {
                TryInsertFish(filter.ManufacturedResources);
                TryInsertFish(filter.StoredResources);
            }
        }
    }

    private static void TryInsertFish(ISet<ResourceInEditor>? resourceSet)
    {
        if (resourceSet == null) return;

        if (!resourceSet.Contains(ResourceInEditor.Fish) && resourceSet.Count > 0)
        {
            resourceSet.Add(ResourceInEditor.Fish);
        }
    }
}
