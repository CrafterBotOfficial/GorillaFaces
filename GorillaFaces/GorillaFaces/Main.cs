using BepInEx;
using System;

namespace GorillaFaces
{
    [BepInPlugin("crafterbot.gorillafaces", "GorillaFaces", "1.0.0"), BepInDependency("tonimacaroni.computerinterface"), BepInDependency("dev.auros.bepinex.bepinject")]
    [BepInIncompatibility("com.dev9998.gorillatag.devblinkmod")]
    internal class Main : BaseUnityPlugin
    {
        internal const string PropertyKey = "GorillaFaces";

        private static Main _instance;
        internal Main()
        {
            _instance = this;
            Configuration.Init(Config);

            Bepinject.Zenjector.Install<Interface.MainInstaller>().OnProject();

            // Harmony - Since most of these types are internal, we need to use reflection to get them.

            Type RigCache = typeof(GorillaTagger).Assembly.GetType("VRRigCache");
            System.Reflection.MethodInfo SpawnRigInfo = RigCache.GetMethod("SpawnRig", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            System.Reflection.MethodInfo AddRigToGorillaParent = RigCache.GetMethod("AddRigToGorillaParent", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            var harmony = new HarmonyLib.Harmony(Info.Metadata.GUID);
            harmony.Patch(SpawnRigInfo, postfix: new HarmonyLib.HarmonyMethod(typeof(Patches).GetMethod("VRRigCache_SpawnRig_Postfix")));
            harmony.Patch(AddRigToGorillaParent, postfix: new HarmonyLib.HarmonyMethod(typeof(Patches).GetMethod("VRRigCache_AddRigToGorillaParent_Postfix")));
            harmony.PatchAll(typeof(Patches));
        }

        internal static void Log(object message, BepInEx.Logging.LogLevel logLevel = BepInEx.Logging.LogLevel.Info)
        {
            _instance.Logger.Log(logLevel, message);
        }
    }
}
