using UnityEngine;
using UnityEngine.Networking;

namespace Pegas.Rizo
{
    public class MockLobbyPlayer : NetworkLobbyPlayer
    {
        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
            Debug.Log("OnStartLocalPlayer, isLocalPlayer = " + isLocalPlayer);

            SendReadyToBeginMessage();
        }
    }
}
