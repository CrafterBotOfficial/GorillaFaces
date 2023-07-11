using HarmonyLib;
using UnityEngine;

namespace GorillaFaces
{
    internal static class Patches
    {
        // Just going to use callbacks for this, should be more reliable though ugly and repetitive

        /*[HarmonyWrapSafe] 
        public static void VRRigCache_AddRigToGorillaParent_Postfix(Player player, VRRig vrrig)
        {
            Main.Log("Player rig added to parent: " + player.NickName);
            if (player.CustomProperties.TryGetValue(Main.PropertyKey, out object value))
            {
                Main.Log("Player has a face: " + player.NickName);
                FaceController.EquipFace(player, (string)value);
            }
        }*/

        [HarmonyPatch(typeof(GorillaTagger), "Start"), HarmonyPostfix]
        private static void GorillaTagger_Start_Postfix()
        {
            new GameObject("Callbacks").AddComponent<Behaviours.Callbacks>();
            FaceController.LoadFaces(true);

            if (Configuration.EnableMirrorOnStartup.Value)
            {
                GameObject mirror = GameObject.Find("/Level/lower level/mirror (1)");
                mirror.gameObject.SetActive(true);
                foreach (Collider collider in mirror.GetComponentsInChildren<Collider>())
                    GameObject.Destroy(collider);
                mirror.GetComponentInChildren<Camera>().cullingMask = 1788280631;
            }
        }
    }
}
