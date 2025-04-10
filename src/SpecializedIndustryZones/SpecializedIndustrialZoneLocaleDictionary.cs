using System.Collections.Generic;
using Colossal;
using Colossal.Logging;
using Game.SceneFlow;
using Game.UI.InGame;
using Unity.Entities;

namespace SpecializedIndustryZones;

public class SpecializedIndustrialZoneLocaleDictionary(
    SpecializedZoneSpec _spec,
    Entity _industrialZoneEntity,
    Entity _specializedZoneEntity,
    PrefabUISystem _prefabUISystem,
    ILog _log) : IDictionarySource
{
    public IEnumerable<KeyValuePair<string, string>> ReadEntries(
        IList<IDictionaryEntryError> errors,
        Dictionary<string, int> indexCounts)
    {
        _prefabUISystem.GetTitleAndDescription(_industrialZoneEntity, out var baseTitleID, out var baseDescriptionID);
        _prefabUISystem.GetTitleAndDescription(_specializedZoneEntity, out var titleID, out var descriptionID);

        var activeDict = GameManager.instance.localizationManager.activeDictionary;
        if (activeDict.TryGetValue(baseTitleID, out var baseTitle))
        {
            yield return new KeyValuePair<string, string>(titleID, baseTitle + $" [{_spec.Name}]");
        }
        else
        {
            _log.Warn("Failed to get base title");
        }

        if (activeDict.TryGetValue(baseDescriptionID, out var baseDescription))
        {
            var description = string.Join(", ", _spec.Filter);
            yield return new KeyValuePair<string, string>(descriptionID, baseDescription + $" [{description}]");
        }
        else
        {
            _log.Warn("Failed to get base description");
        }
    }

    public void Unload()
    { }
}