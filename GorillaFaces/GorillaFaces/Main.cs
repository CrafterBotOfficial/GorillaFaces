using BepInEx;

namespace GorillaFaces;

[BepInPlugin("crafterbot.gorillafaces", "GorillaFaces", "1.0.4")]
[BepInIncompatibility("com.dev9998.gorillatag.devblinkmod")]
public class Main : BaseUnityPlugin
{
    private static Main instance;

    public void Awake()
    {
        instance = this;
        Configuration.Init(Config);

        new HarmonyLib.Harmony(Info.Metadata.GUID).PatchAll();
    }

    public static void Log(object message, BepInEx.Logging.LogLevel level = BepInEx.Logging.LogLevel.Info)
    {
        instance?.Logger.Log(level, message);
    }
}