using BepInEx;

namespace GorillaFaces
{
    [BepInPlugin("crafterbot.gorillafaces", "GorillaFaces", "1.0.3"), BepInDependency("tonimacaroni.computerinterface"), BepInDependency("dev.auros.bepinex.bepinject")]
    [BepInIncompatibility("com.dev9998.gorillatag.devblinkmod")]
    public class Main : BaseUnityPlugin
    {
        public const string PropertyKey = "GorillaFaces";

        private static Main Instance;
        public Main()
        {
            Instance = this;
            Configuration.Init(Config);

            Bepinject.Zenjector.Install<Interface.MainInstaller>().OnProject();
            new HarmonyLib.Harmony(Info.Metadata.GUID).PatchAll(typeof(Patches));
        }

        public static void Log(object data, BepInEx.Logging.LogLevel logLevel = BepInEx.Logging.LogLevel.Info)
        {
            if (_instance is object)
            {
                Instance.Logger.Log(logLevel, data);
                return;
            }
            UnityEngine.Debug.Log(" [Gorilla Faces]: " + data);
        }
    }
}
