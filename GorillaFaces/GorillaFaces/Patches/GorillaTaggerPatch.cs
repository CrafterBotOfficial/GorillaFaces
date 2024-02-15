using HarmonyLib;
using UnityEngine;

namespace GorillaFaces.Patches;

[HarmonyPatch(typeof(GorillaTagger))]
public static class GorillaTaggerPatch
{
    [HarmonyPatch("Start"), HarmonyPostfix, HarmonyWrapSafe]
    private static void Start_Postfix()
    {
        new GameObject("Callbacks").AddComponent<Callbacks>();
        FaceController.LoadFaces();

        // Load the mirror if the configuration option is enabled
        if (Configuration.EnableMirrorOnStartup.Value)
        {
            GameObject mirror = GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/mirror (1)/");
            mirror.gameObject.SetActive(true);
            foreach (Collider collider in mirror.GetComponentsInChildren<Collider>())
                Object.Destroy(collider);
            mirror.GetComponentInChildren<Camera>().cullingMask = 1788280631;
        }
    }
}