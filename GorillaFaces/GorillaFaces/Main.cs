﻿/*
    Fix incompatbility with holdablepad *done?*
    Redo networking code for joining and player joining **done**
*/
using BepInEx;

namespace GorillaFaces
{
    [BepInPlugin("crafterbot.gorillafaces", "GorillaFaces", "1.0.2"), BepInDependency("tonimacaroni.computerinterface"), BepInDependency("dev.auros.bepinex.bepinject")]
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
            new HarmonyLib.Harmony(Info.Metadata.GUID).PatchAll(typeof(Patches));
        }

        internal static void Log(object message, BepInEx.Logging.LogLevel logLevel = BepInEx.Logging.LogLevel.Info)
        {
#if DEBUG
            _instance.Logger.Log(logLevel, message);
#endif
        }
    }
}