using BepInEx.Configuration;

namespace GorillaFaces;

public static class Configuration
{
    public static ConfigEntry<string> SelectedFace;
    public static ConfigEntry<bool> EnableMirrorOnStartup;

    public static void Init(ConfigFile config)
    {
        SelectedFace = config.Bind("General", "SelectedFace", "Default", "The face that will be loaded on startup");
        EnableMirrorOnStartup = config.Bind("General", "EnableMirrorOnStartup", true, "If true, the mirror will be enabled on startup");
    }
}