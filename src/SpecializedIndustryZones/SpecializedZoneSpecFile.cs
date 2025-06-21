using System.Collections.Generic;

namespace SpecializedIndustryZones;

public class SpecializedZoneSpecFile
{
    public string Version { get; set; } = "v1alpha1";

    public IDictionary<string, SpecializedZoneSpec> Zones { get; set; } = new Dictionary<string, SpecializedZoneSpec>();
}
