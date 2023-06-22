using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using UnityEngine;

namespace GorillaFaces
{
    [HarmonyPatch]
    internal class Patches
    {
        [HarmonyPatch(typeof(VRRig), "Start"), HarmonyPostfix, HarmonyWrapSafe]
        private static void Start(VRRig __instance)
        {
            // new WaitForEndOfFrame();

            // Load on startup
            if (!__instance.TryGetComponent(out PhotonView component))
            {
                new GameObject("Callbacks").AddComponent<Behaviours.Callbacks>();
                Main.Instance.Faces.ElementAt(0).Value.face = __instance.transform.Find("rig/body/head/gorillaface").GetComponent<MeshRenderer>().material.mainTexture as Texture2D;
                AttemptEquip();

                // enable mirror
                if (Main.Instance.EnableMirrorOnStartup.Value)
                {
                    GameObject mirror = GameObject.Find("/Level/lower level/mirror (1)");
                    mirror.gameObject.SetActive(true);
                    foreach (Collider collider in mirror.GetComponentsInChildren<Collider>())
                        GameObject.Destroy(collider);
                    mirror.GetComponentInChildren<Camera>().cullingMask = 1574134839;
                }
            }
            else if (component.IsMine)
                AttemptEquip();

            // This is for equiping the face for a player that joins
            else
            {
                Player player = component.Owner;
                if (player.CustomProperties.TryGetValue(Main.PROPERTIES_KEY, out object property))
                    Main.Instance.EquipFace(player, property as string);
            }

            void AttemptEquip()
            {
                if (Main.Instance.Faces.ContainsKey(Main.Instance.SelectedFaceId.Value))
                    Main.Instance.EquipFace(Main.Instance.SelectedFaceId.Value);
            }
        }
    }
}
