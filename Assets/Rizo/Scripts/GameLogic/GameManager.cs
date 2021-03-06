﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

namespace Pegas.Rizo
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance
        {
            get; private set;
        }

        public CutscenePlayer _cutscenes;
        public Camera _camera;
        public ArenaHUD _hud;
        public BodyPartSelector _bodyPartSelector; 
        
        public GameObject _pawnPrefab;
        public Material[] _materials;

        private GameObject _localPlayerSpawnPoint;
        private GameObject _remotePlayerSpawnPoint;

        private Queue<Material> _unassignedMaterials = new Queue<Material>();
        private List<PlayerPawnProxy> _pawns = new List<PlayerPawnProxy>();
        private TurnMaker _turnMaker = new TurnMaker();

        private CameraShake _cameraShake;

        GameManager()
        {
            Instance = this;
        }

        public static bool IsNetworkedMode()
        {
            return (NetworkMessages.Instance != null);
        }

        private void Awake()
        {
            _localPlayerSpawnPoint = GameObject.FindGameObjectWithTag("SpawnPoint_LocalPlayer");
            _remotePlayerSpawnPoint = GameObject.FindGameObjectWithTag("SpawnPoint_RemotePlayer");

            foreach (var material in _materials)
            {
                _unassignedMaterials.Enqueue(material);
            }

            if (IsNetworkedMode())
            {
                NetworkMessages.Instance.OnPlayerReady += Client_OnPlayerReady;
                NetworkMessages.Instance.OnRapidEvent += Client_OnRapid;
                NetworkMessages.Instance.OnRapidEventComplete += Server_OnRapidStageComplete;
                NetworkMessages.Instance.OnStrikePointAssigned += Client_OnStrikePointAssigned;
                NetworkMessages.Instance.OnPlayerWinStrike += Client_OnPlayerWinStrike;
            }

            _hud.OnFadeInComplete += EventHandler_OnFadeInComplete;
            _hud.OnFadeOutComplete += EventHandler_OnFadeOutComplete;
            _cutscenes.OnCutSceneEnd += EventHandler_OnStrikeCutSceneComplete;

            _turnMaker.Reset();
            _pawns.Clear();

            _cameraShake = _camera.GetComponent<CameraShake>();
            _cameraShake.enabled = false;
        }

        private void Start()
        {
            _clientCurrentStage = RapidStage.Choises;
            //_cutscenes.PlayScene(CutscenePlayer.CUTSCENE_LONG_INTRO, _camera);
            _cutscenes.PlayScene(CutscenePlayer.CUTSCENE_SHORT_INTRO, _camera);
        }

        public void AddPawn(PlayerPawnProxy proxy)
        {
            _pawns.Add(proxy);

            var playerId = proxy.GetPlayerID();
            if (!_turnMaker.AssignPlayer(playerId))
            {
                Debug.LogError("can not assign session slot to player!");
            }
        }

        
        public void Server_CreatePawn(NetworkInstanceId playerId)
        {
            if (IsNetworkedMode())
            {
                string resourceName = "material_default";
                Material assignedMaterial = Server_AssignMaterial();
                if (assignedMaterial)
                {
                    resourceName = assignedMaterial.name;
                }

                GameObject newPawn = Instantiate(_pawnPrefab);
                PlayerPawn pawnScript = newPawn.GetComponent<PlayerPawn>();
                pawnScript.Server_Init(playerId, resourceName);

                PlayerPawnProxy proxy = newPawn.GetComponent<PlayerPawnProxy>();
                if(proxy)
                {
                    AddPawn(proxy);
                }

                NetworkServer.Spawn(newPawn);
            }
        }

        private Material Server_AssignMaterial()
        {
            if (_unassignedMaterials.Count > 0)
            {
                return _unassignedMaterials.Dequeue();
            }

            return null;
        }

        public void Server_OnPlayerReady(NetworkInstanceId playerId)
        {
            Debug.Log("GameManager.Server_OnPlayerReady, playerID = " + playerId);

            _turnMaker.SetPlayerReady(playerId);

            if (IsNetworkedMode())
            {
                NetworkMessages.Instance.NotifyClients_PlayerReady(playerId);
            }else
            {
                _Client_OnPlayerReady(playerId);
            }

            //check if both player are ready
            if (_turnMaker.IsBothPlayersReady())
            {
                //and if this one yes - start delayed event  - strike started
                if (_strikeRapidLoop == null)
                {
                    _strikeRapidLoop = StartCoroutine(Coroutine_Server_StrikeRapidLoop());
                }                
            }                                                
        }

        public void Server_OnPlayerChoosedHit(NetworkInstanceId playerId, HitType hitType)
        {
            Debug.Log("GameManager.Server_OnPlayerChoosedHit, playerID = " + playerId);
            Debug.Log("hitType = " + hitType);

            _turnMaker.ChooseHitType(playerId, hitType);
        }

        public void Server_OnPlayerChoosedSpear(NetworkInstanceId playerId, SpearType spearType)
        {
            Debug.Log("GameManager.Server_OnPlayerChoosedSpear, playerID = " + playerId);
            Debug.Log("spearType = " + spearType);

            _turnMaker.ChooseSpear(playerId, spearType);
        }

        public void Server_OnPlayerChoosedAttakedBodyPart(NetworkInstanceId playerId, BodyPartType bodyPart)
        {
            Debug.Log("GameManager.Server_OnPlayerChoosedAttakedBodyPart, playerID = " + playerId);
            Debug.Log("bodyPart" + bodyPart);

            _turnMaker.ChooseAttackedBodyPart(playerId, bodyPart);
        }

        public void Server_OnPlayerChoosedDefenedBodyPart(NetworkInstanceId playerId, BodyPartType bodyPart)
        {
            Debug.Log("GameManager.Server_OnPlayerChoosedDefenedBodyPart, playerID = " + playerId);
            Debug.Log("bodyPart" + bodyPart);

            _turnMaker.ChooseAttackedBodyPart(playerId, bodyPart);
        }

        public void Server_OnPlayerDroppedArmorBodyPart(NetworkInstanceId playerId, BodyPartType bodyPart)
        {
            Debug.Log("GameManager.Server_OnPlayerDroppedArmorBodyPart, playerID = " + playerId);
            Debug.Log("bodyPart" + bodyPart);

            _turnMaker.DropArmorPart(playerId, bodyPart);
        }

        public void Server_OnRapidStageComplete(NetworkMessage message)
        {
            var rapidEvent = message.ReadMessage<NetworkMessages.RapidEvent>();
            var stage = (RapidStage)rapidEvent._rapidEvent;

            _Server_OnRapidStageComplete(stage);
        }

        private void _Server_OnRapidStageComplete(RapidStage stage)
        {
            if(stage == RapidStage.PreStartFadeIn)
            {
                _playersFadeInComplete++;
            }

            if(stage == RapidStage.PreStartFadeOut)
            {
                _playersFadeOutComplete++;
            }

            if(stage == RapidStage.RapidCutSceneComplete)
            {
                _playersStrikeCutsceneComplete++;
            }
        }

        private enum RapidStage
        {
            Choises,
            PreStartFadeIn,
            PreStartFadeOut,
            StrikeBegin,
            RapidCutSceneComplete,
            PostStrikeFadeIn                        
        }

        private struct RapidPawnState
        {
            public PlayerPawnProxy _proxy;
            public Vector3 _startPosition;
            public Vector3 _endPosition;            
        }

        private Coroutine _strikeRapidLoop = null;
        private int _playersFadeInComplete = 0;
        private int _playersFadeOutComplete = 0;
        private int _playersStrikeCutsceneComplete = 0;

        public IEnumerator Coroutine_Server_StrikeRapidLoop()
        {
            Debug.Log("Strike rapid loop started");

            _playersFadeInComplete = 0;
            _playersFadeOutComplete = 0;
            _playersStrikeCutsceneComplete = 0;

            //start delaying
            yield return new WaitForSeconds(1.0f);

            if(IsNetworkedMode())
            {
                NetworkMessages.Instance.NotifyClients_OnRapidStage((int)RapidStage.PreStartFadeIn);
            }else
            {
                _Client_OnRapid(RapidStage.PreStartFadeIn);
            }

            while(_playersFadeInComplete < _pawns.Count)
            {
                yield return null;
            }

            var strikePoints = GameObject.FindGameObjectsWithTag("StrikePoint");
            RapidPawnState[] pawnStates = new RapidPawnState[_pawns.Count];

            for(int i = 0; i < _pawns.Count; i++)
            {
                pawnStates[i]._proxy = _pawns[i];
                pawnStates[i]._startPosition = strikePoints[i].transform.position;
                pawnStates[i]._endPosition = strikePoints[i].transform.GetChild(0).transform.position;

                pawnStates[i]._proxy.SetPosition(pawnStates[i]._startPosition);
                pawnStates[i]._proxy.SetRotation(strikePoints[i].transform.rotation);

                var playerID = _pawns[i].GetPlayerID();
                var pointName = strikePoints[i].gameObject.name;
                if (IsNetworkedMode())
                {

                    NetworkMessages.Instance.NotifyClients_OnPlayerAssignStrikePoint(playerID, pointName);
                }
                else
                {
                    _Client_OnStrikePointAssigned(playerID, pointName);
                }
            }

            if (IsNetworkedMode())
            {
                NetworkMessages.Instance.NotifyClients_OnRapidStage((int)RapidStage.PreStartFadeOut);
            }
            else
            {
                _Client_OnRapid(RapidStage.PreStartFadeOut);
            }

            while (_playersFadeOutComplete < _pawns.Count)
            {
                yield return null;
            }

            yield return new WaitForSeconds(2.0f);

            if (IsNetworkedMode())
            {
                NetworkMessages.Instance.NotifyClients_OnRapidStage((int)RapidStage.StrikeBegin);
            }
            else
            {
                _Client_OnRapid(RapidStage.StrikeBegin);
            }

            const float k_strikeDuration = 30.0f;
            float ellapsedTime = 0.0f;
            float speed = 1.0f;
            NetworkInstanceId? winner = null;

            while(ellapsedTime < k_strikeDuration)
            {
                ellapsedTime += Time.deltaTime * speed;
                float k = ellapsedTime / k_strikeDuration;

                if(k >= 0.45 && !winner.HasValue)
                {
                    winner = _turnMaker.GetTurnWinner();
                    speed = 3.0f;

                    if (IsNetworkedMode())
                    {
                        NetworkMessages.Instance.NotifyClients_PlayerWinStrike(winner.Value);
                    }
                    else
                    {
                        _Client_OnPlayerWinStrike(winner.Value);
                    }
                }

                for(int i = 0; i < pawnStates.Length; i++)
                {
                    var playerID = pawnStates[i]._proxy.GetPlayerID();

                    if (winner.HasValue && !playerID.Equals(winner))
                    {
                        continue;                        
                    }

                    var position = Vector3.Lerp(pawnStates[i]._startPosition, pawnStates[i]._endPosition, k);
                    pawnStates[i]._proxy.SetPosition(position);
                }                

                yield return null;
            }

            while(_playersStrikeCutsceneComplete < _pawns.Count)
            {
                yield return null;
            }

            if (IsNetworkedMode())
            {
                NetworkMessages.Instance.NotifyClients_OnRapidStage((int)RapidStage.PostStrikeFadeIn);
            }
            else
            {
                _Client_OnRapid(RapidStage.PostStrikeFadeIn);
            }

            //TODO: потом убрать
            _turnMaker.Reset();
            _strikeRapidLoop = null;
        }

        public void Client_OnRapid(NetworkMessage message)
        {
            var rapidMessage = message.ReadMessage<NetworkMessages.RapidEvent>();
            var stage = (RapidStage)rapidMessage._rapidEvent;
            
            _Client_OnRapid(stage);
        }
        
        private void _Client_OnRapid(RapidStage stage)
        {
            Debug.Log("GameManager.Client_OnPreStartRapid");

            _clientCurrentStage = stage;

            if (stage == RapidStage.PreStartFadeIn || stage == RapidStage.PostStrikeFadeIn)
            {
                if(stage == RapidStage.PostStrikeFadeIn)
                {
                    _hud.HideFitil();
                }

                _hud.FadeIn();
            }

            if(stage == RapidStage.PreStartFadeOut)
            {
                //find local player
                var pawns = GameObject.FindGameObjectsWithTag("PlayerPawn");
                foreach (var pawn in pawns)
                {
                    var proxy = pawn.GetComponent<PlayerPawnProxy>();
                    if (proxy && proxy.IsLocalPlayer())
                    {
                        //attach camera to local player
                        var camAttachPoint = pawn.transform.Find("back_cam_point");
                        _camera.transform.parent = camAttachPoint;
                        _camera.transform.localPosition = Vector3.zero;
                        _camera.transform.localRotation = Quaternion.identity;
                        break;
                    }
                }

                _hud.FadeOut();
            }

            if(stage == RapidStage.StrikeBegin)
            {
                _cameraShake.enabled = true;
            }
        }

        public void Client_OnPlayerReady(NetworkMessage message)
        {
            var playerReadyMsg = message.ReadMessage<NetworkMessages.PlayerReady>();
            _Client_OnPlayerReady(playerReadyMsg._playerID);
        }

        private void _Client_OnPlayerReady(NetworkInstanceId playerID)
        {
            Debug.Log("GameManager.Client_OnPlayerReady");

            const int k_isLocalPlayer = 1;
            const int k_isRemotePlayer = 2;
            int playerStatus = 0;

            if (IsNetworkedMode())
            {
                var player = ClientScene.FindLocalObject(playerID);
                if (player)
                {
                    var netID = player.GetComponent<NetworkIdentity>();
                    playerStatus = netID.isLocalPlayer ? k_isLocalPlayer : k_isRemotePlayer;                    
                }
            }else
            {
                var pawns = GameObject.FindGameObjectsWithTag("PlayerPawn");
                foreach(var pawn in pawns)
                {
                    var proxy = pawn.GetComponent<PlayerPawnProxy>();
                    if(proxy && proxy.GetPlayerID().Equals(playerID))
                    {
                        playerStatus = proxy.IsLocalPlayer() ? k_isLocalPlayer : k_isRemotePlayer;
                    }
                }
            }

            if (playerStatus == k_isLocalPlayer)
            {
                _hud.NotifyLocalPlayerReady();
            }

            if (playerStatus == k_isRemotePlayer)
            {
                _hud.NotifyRemotePlayerReady();                
            }            
        }

        private void Client_OnPlayerWinStrike(NetworkMessage message)
        {
            var pwsMessage = message.ReadMessage<NetworkMessages.PlayerWinStrike>();
            _Client_OnPlayerWinStrike(pwsMessage._winnerPlayerID);
        }

        private void _Client_OnPlayerWinStrike(NetworkInstanceId winnerPlayerId)
        {
            _cameraShake.enabled = false;
            _camera.transform.parent = null;
            _cutscenes.PlayScene(_strikeCutscenes[winnerPlayerId], _camera);
        }

        private void Client_OnStrikePointAssigned(NetworkMessage message)
        {
            var spaMessage = message.ReadMessage<NetworkMessages.AssignStrikePoint>();
            _Client_OnStrikePointAssigned(spaMessage._playerID, spaMessage._pointName);
        }

        private Dictionary<NetworkInstanceId, string> _strikeCutscenes = new Dictionary<NetworkInstanceId, string>();
        private void _Client_OnStrikePointAssigned(NetworkInstanceId playerID, string strikePointName)
        {
            string value = strikePointName == "StartPoint_1" ? 
                CutscenePlayer.CUTSCENE_RAPID_1 : CutscenePlayer.CUTSCENE_RAPID_2;

            if(_strikeCutscenes.ContainsKey(playerID))
            {
                _strikeCutscenes[playerID] = value;
            }else
            {
                _strikeCutscenes.Add(playerID, value);
            }
        }

        private RapidStage _clientCurrentStage;
        private void EventHandler_OnFadeInComplete()
        {
            Debug.Log("GameManager.EventHandler_OnFadeInComplete");

            if(_clientCurrentStage == RapidStage.PreStartFadeIn)
            {
                _hud.HidePlayerReadyStatuses();

                //send message to server FadeIn complete
                if(IsNetworkedMode())
                {
                    NetworkMessages.Instance.NotifyServer_RapidStageComplete((int)_clientCurrentStage);
                }else
                {
                    _Server_OnRapidStageComplete(_clientCurrentStage); //for local player
                    _Server_OnRapidStageComplete(_clientCurrentStage); //for remote player
                }
            }

            if(_clientCurrentStage == RapidStage.PostStrikeFadeIn)
            {
                _cameraShake.enabled = false;

                //dettach camera from local player
                _camera.transform.parent = null;

                //place players to default spawn points
                var pawns = GameObject.FindGameObjectsWithTag("PlayerPawn");
                foreach (var pawn in pawns)
                {
                    var proxy = pawn.GetComponent<PlayerPawnProxy>();
                    if (proxy == null) continue;

                    if (proxy.IsLocalPlayer())
                    {
                        PlaceMeToLocalSP(pawn.transform);
                        
                    }else
                    {
                        PlaceMeToRemoteSP(pawn.transform);
                    }
                }

                _cutscenes.PlayScene(CutscenePlayer.CUTSCENE_SHORT_INTRO, Camera.main);

                _clientCurrentStage = RapidStage.Choises;
            }
        }

        private void EventHandler_OnFadeOutComplete()
        {
            Debug.Log("GameManager.EventHandler_OnFadeOutComplete");

            if(_clientCurrentStage == RapidStage.PreStartFadeOut)
            {
                if(IsNetworkedMode())
                {
                    NetworkMessages.Instance.NotifyServer_RapidStageComplete((int)_clientCurrentStage);
                }
                else
                {
                    _Server_OnRapidStageComplete(_clientCurrentStage);
                    _Server_OnRapidStageComplete(_clientCurrentStage);
                }

                _hud.ShowFitil();
            }
        }

        private void EventHandler_OnStrikeCutSceneComplete(string cutSceneName)
        {
            if(cutSceneName == CutscenePlayer.CUTSCENE_RAPID_1 || cutSceneName == CutscenePlayer.CUTSCENE_RAPID_2)
            {
                if(IsNetworkedMode())
                {
                    NetworkMessages.Instance.NotifyServer_RapidStageComplete((int)RapidStage.RapidCutSceneComplete);
                }else
                {
                    _Server_OnRapidStageComplete(RapidStage.RapidCutSceneComplete);
                    _Server_OnRapidStageComplete(RapidStage.RapidCutSceneComplete);
                }
            }
        }

        public enum PlayerActionType
        {
            HIT_TYPE_CHOOSED,
            SPEAR_TYPE_CHOOSED,
            ATTACKED_BODY_PART_CHOOSED,
            DEFENDED_BODY_PART_CHOOSED,
            ARMOR_PART_DROPPED,
            PLAYER_IS_READY
        };

        public class PlayerActionArgs
        {
            public PlayerActionArgs(BodyPartType bodyPart)
            {
                _bodyPart = bodyPart;
            }

            public PlayerActionArgs(HitType hitType)
            {
                _hitType = hitType;
            }

            public PlayerActionArgs(SpearType spearType)
            {
                _spearType = spearType;
            }

            public BodyPartType _bodyPart;
            public HitType _hitType;
            public SpearType _spearType;            
        };

        public delegate void PlayerActionEvent(PlayerActionType et, PlayerActionArgs evArg);
        public PlayerActionEvent OnLocalPlayerAction;

        public void Action_ChooseHit(HitType hitType)
        {
            _hud.ShowReadyButton();

            if(OnLocalPlayerAction != null)
            {
                OnLocalPlayerAction.Invoke(PlayerActionType.HIT_TYPE_CHOOSED, new PlayerActionArgs(hitType));
            }
        }

        public void Action_ChooseSpearType(SpearType spearType)
        {
            _hud.ShowReadyButton();

            if(OnLocalPlayerAction != null)
            {
                OnLocalPlayerAction.Invoke(PlayerActionType.SPEAR_TYPE_CHOOSED, new PlayerActionArgs(spearType));
            }
        }

        public void Action_ChooseAttackedBodyPart(BodyPartType bodyPart)
        {
            _hud.ShowReadyButton();

            if(OnLocalPlayerAction != null)
            {
                OnLocalPlayerAction.Invoke(PlayerActionType.ATTACKED_BODY_PART_CHOOSED, new PlayerActionArgs(bodyPart));
            }
        }

        public void Action_ChooseDefendedBodyPart(BodyPartType bodyPart)
        {
            _hud.ShowReadyButton();

            if(OnLocalPlayerAction != null)
            {
                OnLocalPlayerAction.Invoke(PlayerActionType.DEFENDED_BODY_PART_CHOOSED, new PlayerActionArgs(bodyPart));
            }
        }

        public void Action_DropArmor(BodyPartType bodyPart)
        {
            _hud.ShowReadyButton();

            if(OnLocalPlayerAction != null)
            {
                OnLocalPlayerAction.Invoke(PlayerActionType.ARMOR_PART_DROPPED, new PlayerActionArgs(bodyPart));
            }
        }

        public void Action_SayImReady()
        {
            _hud.HideButtons();
            _bodyPartSelector.enabled = false;

            if(OnLocalPlayerAction != null)
            {
                OnLocalPlayerAction.Invoke(PlayerActionType.PLAYER_IS_READY, null);
            }
        }

        public void PlaceMeToLocalSP(Transform me)
        {
            if(_localPlayerSpawnPoint)
            {
                me.position = _localPlayerSpawnPoint.transform.position;
                me.rotation = _localPlayerSpawnPoint.transform.rotation;
            }
        }

        public void PlaceMeToRemoteSP(Transform me)
        {
            if (_remotePlayerSpawnPoint)
            {
                me.position = _remotePlayerSpawnPoint.transform.position;
                me.rotation = _remotePlayerSpawnPoint.transform.rotation;
            }
        }        
    }
}
