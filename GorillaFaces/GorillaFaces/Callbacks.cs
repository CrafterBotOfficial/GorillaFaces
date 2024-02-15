using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

namespace GorillaFaces;

public class Callbacks : MonoBehaviourPunCallbacks
{
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        try
        {
            if (changedProps.TryGetValue(FaceController.PropertyKey, out object obj))
            {
                Main.Log("Got face update event for " + targetPlayer.NickName + " with value " + (string)obj, BepInEx.Logging.LogLevel.Message);
                FaceController.EquipFace(targetPlayer, (string)obj);
            }
        }
        catch (System.Exception ex) { Main.Log("Error while checking for face change. " + ex, BepInEx.Logging.LogLevel.Error); }
    }
}