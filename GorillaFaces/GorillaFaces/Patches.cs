﻿using HarmonyLib;
using Photon.Pun;
using System.Linq;

namespace GorillaFaces
{
    [HarmonyPatch]
    internal class Patches
    {
        [HarmonyPatch(typeof(VRRig), "OnEnable"), HarmonyPostfix, HarmonyWrapSafe]
        private static async void VRRig_OnEnable(VRRig __instance)
        {
            await System.Threading.Tasks.Task.Delay(1000); // Wait for the photonView to be set

            if (__instance == GorillaTagger.Instance.offlineVRRig)
            {
                Main.Instance.OfflineRigInitialized(__instance);
                return;
            }
            else if (__instance.isMyPlayer)
                return;

            if (!__instance.myPlayer.CustomProperties.TryGetValue(Main.PROPERTIES_KEY, out object obj))
            {
                Main.Instance.EquipFace(__instance, Main.Instance.Faces.First().Key); // A primative way to clean up the rig, this is due to the new "object pooling" system
                return;
            }
            string FaceId = obj as string;
            Main.Instance.EquipFace(__instance, FaceId);
        }
    }
}
