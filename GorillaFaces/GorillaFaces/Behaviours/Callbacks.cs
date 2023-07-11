using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Threading.Tasks;

namespace GorillaFaces.Behaviours
{
    internal class Callbacks : MonoBehaviourPunCallbacks
    {
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Main.Log("Player left room: " + otherPlayer.NickName);
            if (otherPlayer.CustomProperties.TryGetValue(Main.PropertyKey, out object value))
            {
                Main.Log("Cleaning up face for " + otherPlayer.NickName);
                FaceController.UnEquipFace(otherPlayer);
            }
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            Main.Log("Player properties updated: " + targetPlayer.NickName);
            if (changedProps.TryGetValue(Main.PropertyKey, out object value))
            {
                FaceController.EquipFace(targetPlayer, (string)value);
            }
        }

        public override async void OnJoinedRoom()
        {
            await Task.Yield(); // Waiting for the VRRig to be pulled from the pool
            // We need to set the face for the online rig
            Main.Log("Joined room", BepInEx.Logging.LogLevel.Message);
            FaceController.EquipFace(PhotonNetwork.LocalPlayer, Configuration.SelectedFace.Value);
        }
    }
}
