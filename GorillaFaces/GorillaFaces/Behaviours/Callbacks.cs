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

            FaceController.PlayerRigs.Add(newPlayer, FaceController.FindVRRigForPlayer(newPlayer)); // Moved this up so when someone leaves who doesnt have the mod it can still unequip the face
            if (newPlayer.CustomProperties.TryGetValue(Main.PropertyKey, out object value))
                FaceController.EquipFace(newPlayer, (string)value);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            // Due to the rig caching we have to be extra sure that we don't have a face equipped
            // Its farly efficent so its not the end of the world.
            FaceController.UnEquipFace(otherPlayer);

            if (FaceController.PlayerRigs.ContainsKey(otherPlayer))
                FaceController.PlayerRigs.Remove(otherPlayer);
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            // Main.Log("Player properties updated: " + targetPlayer.NickName);
            if (changedProps.TryGetValue(Main.PropertyKey, out object value))
            {
                FaceController.EquipFace(targetPlayer, (string)value);
            }
        }

        public override async void OnJoinedRoom()
        {
            FaceController.PlayerRigs.Clear(); 

            await Task.Yield(); // Waiting for our local VRRig to be pulled from the pool
            // We need to set the face for the online rig
            // Main.Log("Joined room", BepInEx.Logging.LogLevel.Message);
            FaceController.EquipFace(PhotonNetwork.LocalPlayer, Configuration.SelectedFace.Value);

            // Now we need to equip the faces for all the other players
            Player[] players = PhotonNetwork.PlayerListOthers; // excludes the local player, and ordered by actor number
            foreach (Player player in players)
            {
                FaceController.PlayerRigs.Add(player, FaceController.FindVRRigForPlayer(player));
                if (player.CustomProperties.TryGetValue(Main.PropertyKey, out object value))
                    FaceController.EquipFace(player, (string)value);
                else
                    FaceController.UnEquipFace(player); // Occasionaly the rig will not be correctly cleaned up from the last posser, so we need to make sure its unequiped
            }
        }
    }
}