using Colossal.Logging;
using Colossal.UI;
using Game;
using Game.Modding;
using Game.Prefabs;
using Game.SceneFlow;
using System.IO;

namespace SpecializedIndustryZones
{
    public class Mod : IMod
    {
        public const string HostName = "speciz";
        public static ILog log = LogManager.GetLogger($"{nameof(SpecializedIndustryZones)}.{nameof(Mod)}").SetShowsErrorsInUI(false);
        //private Setting m_Setting;

        public void OnLoad(UpdateSystem updateSystem)
        {
            log.Info(nameof(OnLoad));

            if (GameManager.instance.modManager.TryGetExecutableAsset(this, out var asset))
                log.Info($"Current mod asset at {asset.path}");

            var path = Path.GetDirectoryName(asset.GetMeta().path);
            UIManager.defaultUISystem.AddHostLocation(HostName, Path.Combine(path, "Assets/"));

            updateSystem.UpdateBefore<SpecializedZoningSystem, PrefabSystem>(SystemUpdatePhase.MainLoop);

            //m_Setting = new Setting(this);
            //m_Setting.RegisterInOptionsUI();
            //GameManager.instance.localizationManager.AddSource("en-US", new LocaleEN(m_Setting));
            
            //AssetDatabase.global.LoadSettings(nameof(SpecializedIndustryZones), m_Setting, new Setting(this));
        }

        public void OnDispose()
        {
            log.Info(nameof(OnDispose));
            //if (m_Setting != null)
            //{
            //    //m_Setting.UnregisterInOptionsUI();
            //    m_Setting = null;
            //}
        }
    }
}
