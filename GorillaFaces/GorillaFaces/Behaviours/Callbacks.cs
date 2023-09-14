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
            try { FaceController.AttemptEquip(newPlayer); } 
            catch { Main.Log("Error occured while attempting to equip face onto user.", BepInEx.Logging.LogLevel.Error); }
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            try 
            {
                if (changedProps.TryGetValue(Main.PropertyKey, out object obj))
                {
                    Main.Log("Got face update event for " + targetPlayer.NickName + " with value " + (string)obj, BepInEx.Logging.LogLevel.Message);
                    FaceController.EquipFace(targetPlayer, (string)obj);
                }
            }
            catch (System.Exeption ex) { Main.Log("Error while checking for face change. " + ex, BepInEx.Logging.LogLevel.Error); }
        }

        public override async void OnJoinedRoom()
        {
            try
            {
                await Task.Yield(); // Waiting for our local VRRig to be pulled from the pool
                // We need to set the face for the online rig
                FaceController.EquipFace(PhotonNetwork.LocalPlayer, Configuration.SelectedFace.Value);

                // Equip the faces for all the other players
                Player[] players = PhotonNetwork.PlayerListOthers; 
                foreach (Player player in players) FaceController.AttemptEquip(player);
            }
            catch (System.Exeption ex) { Main.Log("Error while updating face for local player & others. " + ex, BepInEx.Logging.LogLevel.Error); }
        }
    }
}
