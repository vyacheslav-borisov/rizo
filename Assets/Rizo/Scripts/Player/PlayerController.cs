using UnityEngine;
using UnityEngine.Networking;

namespace Pegas.Rizo
{
    public class PlayerController : NetworkBehaviour
    {
        //private PlayerPawn _pawn =  null;
        private NetworkIdentity _netID;

        /*
        public void SetPawn(PlayerPawn pawn)
        {
            _pawn = pawn;
        }*/

        public override void OnStartLocalPlayer()
        {
            GameManager.Instance.OnLocalPlayerAction += LocalPlayerActionHandler;

            _netID = GetComponent<NetworkIdentity>();
            CmdAssignPawn(_netID.netId);            
        }

        private void LocalPlayerActionHandler(GameManager.PlayerActionType playerActionType, GameManager.PlayerActionArgs args)
        {
            if(!isLocalPlayer)
            {
                return;
            }

            switch(playerActionType)
            {
                case GameManager.PlayerActionType.HIT_TYPE_CHOOSED:
                    CmdChooseHit(_netID.netId, args._hitType);
                    break;
                case GameManager.PlayerActionType.SPEAR_TYPE_CHOOSED:
                    CmdChooseSpear(_netID.netId, args._spearType);
                    break;
                case GameManager.PlayerActionType.ATTACKED_BODY_PART_CHOOSED:
                    CmdChooseAttakedBodyPart(_netID.netId, args._bodyPart);
                    break;
                case GameManager.PlayerActionType.DEFENDED_BODY_PART_CHOOSED:
                    CmdChooseDefendedBodyPart(_netID.netId, args._bodyPart);
                    break;
                case GameManager.PlayerActionType.ARMOR_PART_DROPPED:
                    CmdChooseDroppedArmorPart(_netID.netId, args._bodyPart);
                    break;
                case GameManager.PlayerActionType.PLAYER_IS_READY:
                    CmdSayImReady(_netID.netId);
                    break;
                default:
                    break;
            }
        }

        [Command]
        private void CmdSayImReady(NetworkInstanceId playerID)
        {
            GameManager.Instance.Server_OnPlayerReady(playerID);
        }

        [Command]
        private void CmdChooseHit(NetworkInstanceId playerID, HitType hitType)
        {
            GameManager.Instance.Server_OnPlayerChoosedHit(playerID, hitType);
        }

        [Command]
        private void CmdChooseSpear(NetworkInstanceId playerID, SpearType spearType)
        {
            GameManager.Instance.Server_OnPlayerChoosedSpear(playerID, spearType);
        }

        [Command]
        private void CmdChooseAttakedBodyPart(NetworkInstanceId playerID, BodyPartType bodyPart)
        {
            GameManager.Instance.Server_OnPlayerChoosedAttakedBodyPart(playerID, bodyPart);
        }

        [Command]
        private void CmdChooseDefendedBodyPart(NetworkInstanceId playerID, BodyPartType bodyPart)
        {
            GameManager.Instance.Server_OnPlayerChoosedDefenedBodyPart(playerID, bodyPart);
        }

        [Command]
        private void CmdChooseDroppedArmorPart(NetworkInstanceId playerID, BodyPartType bodyPart)
        {
            GameManager.Instance.Server_OnPlayerDroppedArmorBodyPart(playerID, bodyPart);
        }

        [Command]
        private void CmdAssignPawn(NetworkInstanceId playerID)
        {
            GameManager.Instance.Server_CreatePawn(playerID);
        }
    }
}
