using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

namespace GorillaFaces.Behaviours
{
    internal class Callbacks : MonoBehaviourPunCallbacks
    {
        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            try
            {
                base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
                Main.Instance.EquipFace(targetPlayer, changedProps[Main.PROPERTIES_KEY] as string);
            }
            catch
            {
                /* Do nothing, this will execute every time the players properties are changing however it is not changing the face */
            }
        }
    }
}
