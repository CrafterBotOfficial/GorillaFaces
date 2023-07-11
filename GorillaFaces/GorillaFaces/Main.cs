using BepInEx;
using System;

namespace GorillaFaces
{
    [BepInPlugin("crafterbot.gorillafaces", "GorillaFaces", "1.0.0"), BepInDependency("tonimacaroni.computerinterface"), BepInDependency("dev.auros.bepinex.bepinject")]
    internal class Main : BaseUnityPlugin
    {
        internal const string PropertyKey = "GorillaFaces";

        private static Main _instance;
        internal Main()
        {
            _instance = this;
            Configuration.Init(Config);

            Bepinject.Zenjector.Install<Interface.MainInstaller>().OnProject();

            // Harmony

            Type RigCache = typeof(GorillaTagger).Assembly.GetType("VRRigCache");
            System.Reflection.MethodInfo methodInfo = RigCache.GetMethod("SpawnRig", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            var harmony = new HarmonyLib.Harmony(Info.Metadata.GUID);
            harmony.Patch(methodInfo, postfix: new HarmonyLib.HarmonyMethod(typeof(Patches).GetMethod("VRRigCache_SpawnRig_Postfix")));
            harmony.PatchAll(typeof(Patches));
        }

        internal static void Log(object message, BepInEx.Logging.LogLevel logLevel = BepInEx.Logging.LogLevel.Info)
        {
            _instance.Logger.Log(logLevel, message);
        }
    }
}
