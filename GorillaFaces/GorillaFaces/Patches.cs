﻿using HarmonyLib;
using UnityEngine;

namespace GorillaFaces
{
    internal static class Patches
    {
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
