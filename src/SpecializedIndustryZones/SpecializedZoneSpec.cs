using Game.Economy;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SpecializedIndustryZones;

public class SpecializedZoneSpec(string name, string baseZoneName, string description)
{
    public string Name => name;

    public string BaseZoneName => baseZoneName;

    public string Description => description;

    public Color Color { get; set; }

    public string? IconUri { get; set; }

    public IList<SpecializedZoneFilterSpec> Filters { get; set; } = [];

    [JsonIgnore]
    public SpecializedZoneFilterSpec CombinedFilter => Filters.Count == 0 ? new() : Filters.Aggregate((agg, curr) => new()
    {
        ManufacturedResources = IntersectOptionalSets(agg.ManufacturedResources, curr.ManufacturedResources),
        StoredResources = IntersectOptionalSets(agg.StoredResources, curr.StoredResources),
        SoldResources = IntersectOptionalSets(agg.SoldResources, curr.SoldResources),
        RequireManufactured = agg.RequireManufactured ?? curr.RequireManufactured,
        RequireStored = agg.RequireStored ?? curr.RequireStored,
        RequireSold = agg.RequireSold ?? curr.RequireSold
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
    public ISet<ResourceInEditor>? ManufacturedResources { get; set; }

    public ISet<ResourceInEditor>? StoredResources { get; set; }

    public ISet<ResourceInEditor>? SoldResources { get; set; }

    public bool? RequireManufactured { get; set; }

    public bool? RequireStored { get; set; }

    public bool? RequireSold { get; set; }
}
