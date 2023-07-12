using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Threading.Tasks;

namespace GorillaFaces.Behaviours
{
    internal class Callbacks : MonoBehaviourPunCallbacks
    {
        public override async void OnPlayerEnteredRoom(Player newPlayer)
        {
            await Task.Yield(); // Waiting for the VRRig to be pulled from the pool
            FaceController.AttemptEquip(newPlayer);
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            if (changedProps.TryGetValue(Main.PropertyKey, out object obj))
                FaceController.EquipFace(targetPlayer, (string)obj);
        }

        public override async void OnJoinedRoom()
        {
            await Task.Yield(); // Waiting for our local VRRig to be pulled from the pool
            // We need to set the face for the online rig
            // Main.Log("Joined room", BepInEx.Logging.LogLevel.Message);
            FaceController.EquipFace(PhotonNetwork.LocalPlayer, Configuration.SelectedFace.Value);

            // Now we need to equip the faces for all the other players
            Player[] players = PhotonNetwork.PlayerListOthers; // excludes the local player, and ordered by actor number
            foreach (Player player in players)
            {
                FaceController.AttemptEquip(player);
            }
        }
    }
}