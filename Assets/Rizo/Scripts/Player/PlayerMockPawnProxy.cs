using UnityEngine;
using UnityEngine.Networking;

namespace Pegas.Rizo
{
    public class PlayerMockPawnProxy : PlayerPawnProxy
    {
        private NetworkInstanceId _mockID;
        private bool _isLocalPlayer;

        private static uint _s_nextNetID = 1;
        private static NetworkInstanceId AssignMockID()
        {
            NetworkInstanceId result = new NetworkInstanceId(_s_nextNetID++);

            return result;
        }        

        public override bool IsLocalPlayer()
        {
            return _isLocalPlayer;
        }

        public override NetworkInstanceId GetPlayerID()
        {
            return _mockID;
        }

        public override void SetPosition(Vector3 position)
        {
            transform.position = position;
        }

        public override void SetRotation(Quaternion rotation)
        {
            transform.rotation = rotation;
        }

        private void Awake()
        {
            if(GameManager.IsNetworkedMode())
            {
                gameObject.SetActive(false);
                return;
            }

            string parentTag = transform.parent.gameObject.tag;
            _isLocalPlayer = (parentTag != "SpawnPoint_RemotePlayer");
            _mockID = AssignMockID();

            GameManager.Instance.OnLocalPlayerAction += LocalPlayerActionHandler;
        }

        private void Start()
        {
            GameManager.Instance.AddPawn(this);
        }

        private void LocalPlayerActionHandler(GameManager.PlayerActionType playerActionType, GameManager.PlayerActionArgs args)
        {
            switch(playerActionType)
            {
                case GameManager.PlayerActionType.HIT_TYPE_CHOOSED:
                    if(IsLocalPlayer())
                    {
                        GameManager.Instance.Server_OnPlayerChoosedHit(_mockID, args._hitType);
                    }
                    break;

                case GameManager.PlayerActionType.SPEAR_TYPE_CHOOSED:
                    if(IsLocalPlayer())
                    {
                        GameManager.Instance.Server_OnPlayerChoosedSpear(_mockID, args._spearType);
                    }
                    break;
                case GameManager.PlayerActionType.ATTACKED_BODY_PART_CHOOSED:
                    if(IsLocalPlayer())
                    {
                        GameManager.Instance.Server_OnPlayerChoosedAttakedBodyPart(_mockID, args._bodyPart);
                    }
                    break;
                case GameManager.PlayerActionType.DEFENDED_BODY_PART_CHOOSED:
                    if(IsLocalPlayer())
                    {
                        GameManager.Instance.Server_OnPlayerChoosedDefenedBodyPart(_mockID, args._bodyPart);
                    }
                    break;
                case GameManager.PlayerActionType.ARMOR_PART_DROPPED:
                    if(IsLocalPlayer())
                    {
                        GameManager.Instance.Server_OnPlayerDroppedArmorBodyPart(_mockID, args._bodyPart);
                    }
                    break;
                case GameManager.PlayerActionType.PLAYER_IS_READY:
                    if(!IsLocalPlayer())
                    {
                        AI_MakeTurnOptions();
                    }
                    GameManager.Instance.Server_OnPlayerReady(_mockID);
                    break;
            }
        }

        private void AI_MakeTurnOptions()
        {
            GameManager.PlayerActionType[] actions = new GameManager.PlayerActionType[]
            {
                GameManager.PlayerActionType.HIT_TYPE_CHOOSED,
                GameManager.PlayerActionType.SPEAR_TYPE_CHOOSED,
                GameManager.PlayerActionType.ATTACKED_BODY_PART_CHOOSED,
                GameManager.PlayerActionType.DEFENDED_BODY_PART_CHOOSED,
                GameManager.PlayerActionType.ARMOR_PART_DROPPED
            };
            Shuffle(actions);

            int n = Random.Range(0, actions.Length);
            for(int i = 0; i < n; i++)
            {
                switch(actions[i])
                {
                    case GameManager.PlayerActionType.HIT_TYPE_CHOOSED:
                        {
                            var option = (HitType)Random.Range((int)HitType.Simple, (int)(HitType.Breakable + 1));
                            GameManager.Instance.Server_OnPlayerChoosedHit(_mockID, option);
                        }
                        break;
                    case GameManager.PlayerActionType.SPEAR_TYPE_CHOOSED:
                        {
                            var option = (SpearType)Random.Range((int)SpearType.Light, (int)(SpearType.Heavy + 1));
                            GameManager.Instance.Server_OnPlayerChoosedSpear(_mockID, option);
                        }
                        break;
                    case GameManager.PlayerActionType.ATTACKED_BODY_PART_CHOOSED:
                        {
                            var option = (BodyPartType)Random.Range((int)BodyPartType.Torso, (int)(BodyPartType.RightHand + 1));
                            GameManager.Instance.Server_OnPlayerChoosedAttakedBodyPart(_mockID, option);
                        }
                        break;
                    case GameManager.PlayerActionType.DEFENDED_BODY_PART_CHOOSED:
                        {
                            var option = (BodyPartType)Random.Range((int)BodyPartType.Torso, (int)(BodyPartType.RightHand + 1));
                            GameManager.Instance.Server_OnPlayerChoosedDefenedBodyPart(_mockID, option);
                        }
                        break;
                    case GameManager.PlayerActionType.ARMOR_PART_DROPPED:
                        {
                            var option = (BodyPartType)Random.Range((int)BodyPartType.Torso, (int)(BodyPartType.RightHand + 1));
                            GameManager.Instance.Server_OnPlayerDroppedArmorBodyPart(_mockID, option);
                        }
                        break;
                }
            }                                                
        }

        public static void Shuffle<T>(T[] array)
        {
            int n = array.Length;
            while (n > 1)
            {
                int k = Random.Range(0, n--);
                T temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
        }        
    }
}
