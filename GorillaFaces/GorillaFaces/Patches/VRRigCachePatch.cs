using HarmonyLib;
using Photon.Realtime;

namespace GorillaFaces.Patches;

[HarmonyPatch(typeof(VRRigCache))]
public static class VRRigCachePatch
{
    [HarmonyPatch("AddRigToGorillaParent"), HarmonyPostfix, HarmonyWrapSafe]
    private static void AddRigToGorillaParent(Player player)
    {
        Main.Log($"{player.NickName} has joined, attempting to equip face.");
        FaceController.AttemptEquip(player);
    }
}