using Game.Economy;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SpecializedIndustryZones;

public class SpecializedZoneSpec(string id, string name, string baseZoneName, string description)
{
    public string ID => id;

    public string Name => name;

    public string BaseZoneName => baseZoneName;

    public string Description => description;

    public Color Color { get; set; }

    public string? IconUri { get; set; }

    public IList<SpecializedZoneFilterSpec> Filters { get; set; } = [];

    public SpecializedZoneFilterSpec CombinedFilter => Filters.Count == 0 ? new() : Filters.Aggregate((agg, curr) => new()
    {
        IndustryType = curr.IndustryType ?? agg.IndustryType,
        ManufacturedResources = IntersectOptionalSets(agg.ManufacturedResources, curr.ManufacturedResources),
        StoredResources = IntersectOptionalSets(agg.StoredResources, curr.StoredResources),
        SoldResources = IntersectOptionalSets(agg.SoldResources, curr.SoldResources)
    });

    private static ISet<T>? IntersectOptionalSets<T>(ISet<T>? set1, ISet<T>? set2)
    {
        if (set1 == null && set2 == null) return null;
        if (set1 == null) return set2;
        if (set2 == null) return set1;
        return set1.Intersect(set2).ToHashSet();
    }
}

public class SpecializedZoneFilterSpec
{
    public IndustryType? IndustryType { get; set; }

    public ISet<ResourceInEditor>? ManufacturedResources { get; set; }

    public ISet<ResourceInEditor>? StoredResources { get; set; }

    public ISet<ResourceInEditor>? SoldResources { get; set; }
}
