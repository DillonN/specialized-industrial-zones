using System.Collections.Generic;
using System.Linq;
using Game.Economy;
using UnityEngine;

namespace SpecializedIndustryZones;

public class SpecializedZoneSpecSource
{
    public string ID => $"{Specialization}{Type}";

    public string Name => Type == IndustryType.General ? Specialization.ToString() : $"{Specialization} - {Type} Only";

    public IndustrySpecialization Specialization { get; set; }

    public IndustryType Type { get; set; }
    
    public Color Color { get; set; }
    
    public HashSet<ResourceInEditor> Filter { get; set; }
    
    public ResourceInEditor[] ApplyFilter(ResourceInEditor[] source) => [.. source.Where(Filter.Contains)];

    public ResourceInEditor[] ApplyFilter(ResourceInEditor[] source, IndustryType forType) =>
        (Type == IndustryType.General || Type == forType) ?
        ApplyFilter(source) :
        [];

    public static readonly IReadOnlyList<SpecializedZoneSpecSource> AllZones =
    [
        new()
        {
            Specialization = IndustrySpecialization.Forestry,
            Type = IndustryType.General,
            Color = Color.green,
            Filter =
            [
                ResourceInEditor.Wood,
                ResourceInEditor.Timber,
                ResourceInEditor.Paper,
                ResourceInEditor.Furniture
            ]
        },
        new()
        {
            Specialization = IndustrySpecialization.Agriculture,
            Type = IndustryType.General,
            Color = Color.red,
            Filter =
            [
                ResourceInEditor.Grain,
                ResourceInEditor.Livestock,
                ResourceInEditor.Vegetables,
                ResourceInEditor.Cotton,
                ResourceInEditor.Fish,
                ResourceInEditor.Beverages,
                ResourceInEditor.ConvenienceFood,
                ResourceInEditor.Food,
                ResourceInEditor.Textiles
            ]
        },
        new()
        {
            Specialization = IndustrySpecialization.Ore,
            Type = IndustryType.General,
            Color = Color.blue,
            Filter =
            [
                ResourceInEditor.Ore,
                ResourceInEditor.Coal,
                ResourceInEditor.Stone,
                ResourceInEditor.Minerals,
                ResourceInEditor.Concrete,
                ResourceInEditor.Steel,
                ResourceInEditor.Metals,
                ResourceInEditor.Machinery,
                ResourceInEditor.Vehicles
            ]
        },
        new()
        {
            Specialization = IndustrySpecialization.Oil,
            Type = IndustryType.General,
            Color = Color.black,
            Filter =
            [
                ResourceInEditor.Oil,
                ResourceInEditor.Petrochemicals,
                ResourceInEditor.Chemicals,
                ResourceInEditor.Pharmaceuticals,
                ResourceInEditor.Plastics,
                ResourceInEditor.Electronics
            ]
        },
        new()
        {
            Specialization = IndustrySpecialization.Forestry,
            Type = IndustryType.Manufacturing,
            Color = Color.green,
            Filter =
            [
                ResourceInEditor.Wood,
                ResourceInEditor.Timber,
                ResourceInEditor.Paper,
                ResourceInEditor.Furniture
            ]
        },
        new()
        {
            Specialization = IndustrySpecialization.Agriculture,
            Type = IndustryType.Manufacturing,
            Color = Color.red,
            Filter =
            [
                ResourceInEditor.Grain,
                ResourceInEditor.Livestock,
                ResourceInEditor.Vegetables,
                ResourceInEditor.Cotton,
                ResourceInEditor.Fish,
                ResourceInEditor.Beverages,
                ResourceInEditor.ConvenienceFood,
                ResourceInEditor.Food,
                ResourceInEditor.Textiles
            ]
        },
        new()
        {
            Specialization = IndustrySpecialization.Ore,
            Type = IndustryType.Manufacturing,
            Color = Color.blue,
            Filter =
            [
                ResourceInEditor.Ore,
                ResourceInEditor.Coal,
                ResourceInEditor.Stone,
                ResourceInEditor.Minerals,
                ResourceInEditor.Concrete,
                ResourceInEditor.Steel,
                ResourceInEditor.Metals,
                ResourceInEditor.Machinery,
                ResourceInEditor.Vehicles
            ]
        },
        new()
        {
            Specialization = IndustrySpecialization.Oil,
            Type = IndustryType.Manufacturing,
            Color = Color.black,
            Filter =
            [
                ResourceInEditor.Oil,
                ResourceInEditor.Petrochemicals,
                ResourceInEditor.Chemicals,
                ResourceInEditor.Pharmaceuticals,
                ResourceInEditor.Plastics,
                ResourceInEditor.Electronics
            ]
        },
        new()
        {
            Specialization = IndustrySpecialization.Forestry,
            Type = IndustryType.Warehouses,
            Color = Color.green,
            Filter =
            [
                ResourceInEditor.Wood,
                ResourceInEditor.Timber,
                ResourceInEditor.Paper,
                ResourceInEditor.Furniture
            ]
        },
        new()
        {
            Specialization = IndustrySpecialization.Agriculture,
            Type = IndustryType.Warehouses,
            Color = Color.red,
            Filter =
            [
                ResourceInEditor.Grain,
                ResourceInEditor.Livestock,
                ResourceInEditor.Vegetables,
                ResourceInEditor.Cotton,
                ResourceInEditor.Fish,
                ResourceInEditor.Beverages,
                ResourceInEditor.ConvenienceFood,
                ResourceInEditor.Food,
                ResourceInEditor.Textiles
            ]
        },
        new()
        {
            Specialization = IndustrySpecialization.Ore,
            Type = IndustryType.Warehouses,
            Color = Color.blue,
            Filter =
            [
                ResourceInEditor.Ore,
                ResourceInEditor.Coal,
                ResourceInEditor.Stone,
                ResourceInEditor.Minerals,
                ResourceInEditor.Concrete,
                ResourceInEditor.Steel,
                ResourceInEditor.Metals,
                ResourceInEditor.Machinery,
                ResourceInEditor.Vehicles
            ]
        },
        new()
        {
            Specialization = IndustrySpecialization.Oil,
            Type = IndustryType.Warehouses,
            Color = Color.black,
            Filter =
            [
                ResourceInEditor.Oil,
                ResourceInEditor.Petrochemicals,
                ResourceInEditor.Chemicals,
                ResourceInEditor.Pharmaceuticals,
                ResourceInEditor.Plastics,
                ResourceInEditor.Electronics
            ]
        },
    ];
}
