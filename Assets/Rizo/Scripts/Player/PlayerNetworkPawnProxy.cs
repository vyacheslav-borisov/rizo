using UnityEngine;
using UnityEngine.Networking;

namespace Pegas.Rizo
{
    public class PlayerNetworkPawnProxy : PlayerPawnProxy
    {
        private PlayerPawn _pawn;

        private void Awake()
        {
            _pawn = GetComponent<PlayerPawn>();            
        }

        public override bool IsLocalPlayer()
        {
            if(_pawn)
            {
                return _pawn.IsMyPlayer();
            }

            return false;
        }

        public override NetworkInstanceId GetPlayerID()
        {
            if(_pawn)
            {
                return _pawn.GetPlayerID();
            }

            return new NetworkInstanceId();
        }

        public override void SetPosition(Vector3 position)
        {
            transform.position = position;
        }

        public override void SetRotation(Quaternion rotation)
        {
            transform.rotation = rotation;
        }
    }
}
