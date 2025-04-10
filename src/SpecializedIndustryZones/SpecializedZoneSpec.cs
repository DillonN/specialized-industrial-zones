using System.Collections.Generic;
using System.Linq;
using Game.Economy;
using UnityEngine;

namespace SpecializedIndustryZones;

public class SpecializedZoneSpec
{
    public string Name { get; set; }
    
    public Color Color { get; set; }
    
    public HashSet<ResourceInEditor> Filter { get; set; }
    
    public ResourceInEditor[] ApplyFilter(ResourceInEditor[] source) =>
        source.Where(m => Filter.Contains(m)).ToArray();

    public static readonly IReadOnlyList<SpecializedZoneSpec> AllZones =
    [
        new()
        {
            Name = "Forestry",
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
            Name = "Agriculture",
            Color = Color.red,
            Filter =
            [
                ResourceInEditor.Grain,
                ResourceInEditor.Livestock,
                ResourceInEditor.Vegetables,
                ResourceInEditor.Cotton,
                ResourceInEditor.Beverages,
                ResourceInEditor.ConvenienceFood,
                ResourceInEditor.Food,
                ResourceInEditor.Textiles
            ]
        },
        new()
        {
            Name = "Ore",
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
            Name = "Oil",
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
