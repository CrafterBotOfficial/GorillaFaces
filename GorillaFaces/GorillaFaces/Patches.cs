using HarmonyLib;
using Photon.Pun;
using System.Linq;

namespace GorillaFaces
{
    [HarmonyPatch]
    internal class Patches
    {
        [HarmonyPatch(typeof(VRRig), "Start"), HarmonyPostfix, HarmonyWrapSafe]
        private static void VRRig_Start(VRRig __instance)
        {
            // await Task.Delay(1000);

            if (__instance == GorillaTagger.Instance.offlineVRRig)
            {
                Main.Instance.OfflineRigInitialized(__instance);
                return;
            }

            PhotonView View = __instance.photonView;// Traverse.Create(__instance).Field("photonView").GetValue<PhotonView>();
            if (!View.Owner.CustomProperties.TryGetValue(Main.PROPERTIES_KEY, out object obj))
            {
                Main.Instance.EquipFace(__instance, Main.Instance.Faces.First().Key); // A primative way to clean up the rig, this is due to the new object pooling system
                return;
            }
            string FaceId = obj as string;
            Main.Instance.EquipFace(__instance, FaceId);
        }
    }
}
