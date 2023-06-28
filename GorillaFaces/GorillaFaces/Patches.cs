using HarmonyLib;
using Photon.Pun;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace GorillaFaces
{
    [HarmonyPatch]
    internal class Patches
    {
        [HarmonyPatch(typeof(VRRig), "OnEnable"), HarmonyPostfix, HarmonyWrapSafe]
        private static async void VRRigEnabled(VRRig __instance)
        {
            await Task.Delay(1000);

            if (__instance.isOfflineVRRig)
            {
                new GameObject("Callbacks").AddComponent<Behaviours.Callbacks>();
                Main.Instance.Faces.ElementAt(0).Value.face = __instance.transform.Find("rig/body/head/gorillaface").GetComponent<MeshRenderer>().material.mainTexture as Texture2D;
                
                if (Main.Instance.Faces.ContainsKey(Main.Instance.SelectedFaceId.Value))
                    Main.Instance.EquipFace(Main.Instance.SelectedFaceId.Value);
                if (Main.Instance.EnableMirrorOnStartup.Value)
                {
                    GameObject mirror = GameObject.Find("/Level/lower level/mirror (1)");
                    mirror.gameObject.SetActive(true);
                    foreach (Collider collider in mirror.GetComponentsInChildren<Collider>())
                        GameObject.Destroy(collider);
                    mirror.GetComponentInChildren<Camera>().cullingMask = 1574134839;
                }
                return;
            }

            PhotonView View = Traverse.Create(__instance).Field("photonView").GetValue<PhotonView>();
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
