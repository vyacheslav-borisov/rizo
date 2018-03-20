using UnityEngine;
using UnityEngine.Networking;

namespace Pegas.Rizo
{
    public class PlayerPawn : NetworkBehaviour
    {
        [SyncVar]
        private NetworkInstanceId _playerID;
        [SyncVar]
        private string _assignedMaterial;

        private void Awake()
        {
            Transform camera = transform.Find("Camera");
            if (camera)
            {
                camera.gameObject.SetActive(false);
            }
        }

        private NetworkIdentity _playerNetworkIdentity;

        public bool IsMyPlayer()
        {
            if(_playerNetworkIdentity)
            {
                return _playerNetworkIdentity.isLocalPlayer;
            }

            return false;
        }

        public NetworkInstanceId GetPlayerID()
        {
            return _playerID;
        }

        public void Server_Init(NetworkInstanceId playerID, string assignedMaterial)
        {
            _playerID = playerID;
            _assignedMaterial = assignedMaterial;
        }

        public override void OnStartClient()
        {
            string playerTag = "LocalPlayer";

            GameObject player = ClientScene.FindLocalObject(_playerID);
            if (player)
            {
                //player.GetComponent<PlayerController>().SetPawn(this);

                _playerNetworkIdentity = player.GetComponent<NetworkIdentity>();
                if (_playerNetworkIdentity.isLocalPlayer)
                {
                    GameManager.Instance.PlaceMeToLocalSP(transform);
                    playerTag = "LocalPlayer";
                }
                else
                {
                    GameManager.Instance.PlaceMeToRemoteSP(transform);
                    playerTag = "RemotePlayer";
                }                
            }

            Material material = Resources.Load("Materials/" + _assignedMaterial) as Material;
            if (material)
            {
                MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
                foreach (var renderer in renderers)
                {
                    renderer.material = material;
                    renderer.gameObject.tag = playerTag;
                }
            }
        }
    }
}
