using Dalamud.Configuration;
using Dalamud.Plugin;
using System;

namespace KonjacTranslate
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; } = 0;
        public bool Enable { get; set; } = false;

        public string DeepLAuthKey { get; set; } = "";

        public int MinLength { get; set; } = 4;
        public int MaxLength { get; set;  } = 32;

        // the below exist just to make saving less cumbersome
        [NonSerialized]
        private DalamudPluginInterface? pluginInterface;

        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            this.pluginInterface = pluginInterface;
        }

        public void Save()
        {
            pluginInterface!.SavePluginConfig(this);
        }
    }
}
