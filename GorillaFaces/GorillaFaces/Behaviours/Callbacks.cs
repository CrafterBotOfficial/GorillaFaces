using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

namespace GorillaFaces.Behaviours
{
    internal class Callbacks : MonoBehaviourPunCallbacks
    {
        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            if (!targetPlayer.IsLocal && changedProps.TryGetValue(Main.PROPERTIES_KEY, out object obj))
            {
                string FaceId = obj as string;
                Main.Instance.EquipFace(GorillaGameManager.instance.FindVRRigForPlayer(targetPlayer).GetComponent<VRRig>(), FaceId);
            }
        }
    }
}
