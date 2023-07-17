using BepInEx.Configuration;

namespace GorillaFaces
{
    internal static class Configuration
    {
        internal static ConfigEntry<string> SelectedFace;
        internal static ConfigEntry<bool> EnableMirrorOnStartup;

        internal static void Init(ConfigFile config)
        {
            SelectedFace = config.Bind("General", "SelectedFace", "Default", "The face that will be loaded on startup");
            EnableMirrorOnStartup = config.Bind("General", "EnableMirrorOnStartup", true, "If true, the mirror will be enabled on startup");
        }
    }
}
