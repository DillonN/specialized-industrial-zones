using System.IO;
using Colossal.PSI.Environment;

namespace SpecializedIndustryZones;

readonly struct Filepaths
{
    public static readonly string ZoneFilePath = Path.Combine(EnvPath.kUserDataPath, "ModsData", "SpecializedZones", "SpecializedZones.json");
}