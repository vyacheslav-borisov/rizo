using UnityEngine;
using UnityEngine.Networking;

namespace Pegas.Rizo
{
    public abstract class PlayerPawnProxy : MonoBehaviour
    {
        public abstract bool IsLocalPlayer();
        public abstract NetworkInstanceId GetPlayerID();
        public abstract void SetPosition(Vector3 position);
        public abstract void SetRotation(Quaternion rotation);
    }
}
