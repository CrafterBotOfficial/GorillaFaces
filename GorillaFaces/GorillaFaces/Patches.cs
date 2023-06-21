using HarmonyLib;
using System.Linq;
using UnityEngine;

namespace GorillaFaces
{
    [HarmonyPatch]
    internal class Patches
    {
        [HarmonyPatch(typeof(VRRig), "Start"), HarmonyPostfix]
        private static async void Start(VRRig __instance)
        {
            new WaitForEndOfFrame();

            // Load on startup
            if (__instance == GorillaTagger.Instance.offlineVRRig)
            {
                new GameObject("Callbacks").AddComponent<Behaviours.Callbacks>();
                Main.Instance.Faces.ElementAt(0).Value.face = __instance.transform.Find("rig/body/head/gorillaface").GetComponent<MeshRenderer>().material.mainTexture as Texture2D;
                AttemptEquip();
            }
            else if (__instance.photonView.IsMine)
                AttemptEquip();

            void AttemptEquip()
            {
                if (Main.Instance.Faces.ContainsKey(Main.Instance.SelectedFaceId.Value))
                    Main.Instance.EquipFace(Main.Instance.SelectedFaceId.Value);
            }
        }
    }
}
