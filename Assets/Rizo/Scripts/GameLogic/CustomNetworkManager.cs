using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

namespace Pegas.Rizo
{
    public class CustomNetworkManager : NetworkLobbyManager
    {
        public MainMenu mainMenu;

        private NetworkMessages _messageManager = null;
        private MatchInfo _myOwnRoom = null;
        private DateTime _myOwnRoomCreationDate;
        private MatchInfoSnapshot _matchToJoin = null;

        private static string GenerateRoomName(DateTime creationDate)
        {
            var timestamp = Utils.ConvertToUnixTimestamp(creationDate);
            var randomPostfix = UnityEngine.Random.Range(0, 10000);

            string name = "";
            name += timestamp.ToString();
            name += '_'; 
            name += randomPostfix.ToString();

            return name;
        }

        private static bool IsRoomCreatedEarlier(DateTime myRoomCreationDate, string foundedRoomName)
        {
            if(string.IsNullOrEmpty(foundedRoomName))
            {
                return false;
            }

            double timestamp;
            var strTimestamp = foundedRoomName.Substring(0, foundedRoomName.IndexOf("["));
            if (double.TryParse(strTimestamp, out timestamp))
            {
                var foundedRoomCreationDate = Utils.ConvertFromUnixTimestamp(timestamp);
                var comparsion = (int)myRoomCreationDate.CompareTo(foundedRoomCreationDate);

                return (comparsion == 1);
            }else
            {
                Debug.LogError("could not parse timestamp: " + strTimestamp);
            }

            return false;
        }

        public CustomNetworkManager()
        {
            if(NetworkMessages.Instance == null)
            {
                _messageManager = new NetworkMessages();
            }
        }           


        private string _savedLobbyScene;
        public void StartAutomaticMatchmaking()
        {
            if(string.IsNullOrEmpty(lobbyScene))
            {
                lobbyScene = _savedLobbyScene;
            }else
            {
                _savedLobbyScene = lobbyScene;
            }           

            if (matchMaker == null)
            {
                StartMatchMaker();
                mainMenu.SetStatus(MainMenu.Status.Connecting);
            }

            CreateMyOwnRoom();
            
            mainMenu.HidePlayButton();           
        }
        
        public void StopAutomaticMatchmaking()
        {
            lobbyScene = string.Empty;
            _matchToJoin = null;
            DestroyMyOwnRoom();
            if(_roomSearchLoop != null)
            {
                StopCoroutine(_roomSearchLoop);
                _roomSearchLoop = null;
            }
        }
        
        private void CreateMyOwnRoom()
        {
            if (_myOwnRoom == null)
            {
                _myOwnRoomCreationDate = DateTime.Now;
                var roomName = GenerateRoomName(_myOwnRoomCreationDate);
                matchMaker.CreateMatch(roomName, matchSize, true, "", "", "", 0, 0, OnMatchCreate);

            }else
            {
                StartRoomSearchLoop(1.0f);
            }
        }

        private void DestroyMyOwnRoom()
        {
            if(_myOwnRoom != null)
            {
                mainMenu.ShowMessage("before closing my room...");
                matchMaker.DestroyMatch(_myOwnRoom.networkId, 0, OnDestroyMatch);                
            }
        }

        private void StartRoomSearchLoop(float delay)
        {
            _roomSearchLoop = StartCoroutine(Coroutine_AutomaticMatchmaking(delay));
            mainMenu.SetStatus(MainMenu.Status.SearchPlayers);                        
        }

        private void _ObtainRoomList()
        {
            matchMaker.ListMatches(0, 10, "", true, 0, 0, OnMatchList);
        }

        private Coroutine _roomSearchLoop = null;
        private bool _doNextRequest = false;
        private bool _roomFound = false;

        private IEnumerator Coroutine_AutomaticMatchmaking(float delay)
        {
            _doNextRequest = true;
            while (!_roomFound)
            {
                if (_doNextRequest)
                {
                    _doNextRequest = false;
                    _ObtainRoomList();
                }

                if (!_roomFound)
                {
                    yield return new WaitForSeconds(delay);
                }
            }
                        
            DestroyMyOwnRoom();            
        }

        public override void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
        {
            base.OnMatchCreate(success, extendedInfo, matchInfo);

            if(success)
            {
                mainMenu.ShowMessage("local player has created empty room");
                _myOwnRoom = matchInfo;               
            }
            else
            {
                mainMenu.ShowMessage("creating local room is failed");
            }

            StartRoomSearchLoop(1.0f);

            if (!string.IsNullOrEmpty(extendedInfo))
            {
                mainMenu.ShowMessage(extendedInfo);
            }            
        }

        public override void OnDestroyMatch(bool success, string extendedInfo)
        {
            base.OnDestroyMatch(success, extendedInfo);

            if (success)
            {
                StopMatchMaker();
                StopHost();

                _myOwnRoom = null;
                mainMenu.ShowMessage("my room has closed...");

                if (_matchToJoin != null)
                {
                    mainMenu.ShowMessage("try to join founded room...");

                    StartMatchMaker();
                    matchMaker.JoinMatch(_matchToJoin.networkId, "", "", "", 0, 0, OnMatchJoined);
                    mainMenu.SetStatus(MainMenu.Status.JoinToGame);
                }else
                {
                    mainMenu.ShowPlayButton();
                }
            }

            if (!string.IsNullOrEmpty(extendedInfo))
            {
                mainMenu.ShowMessage(extendedInfo);
            }
        }

        public override void OnMatchJoined(bool success, string extendedInfo, MatchInfo matchInfo)
        {
            if (success)
            {
                base.OnMatchJoined(success, extendedInfo, matchInfo);
            }else
            {
                mainMenu.SetStatus(MainMenu.Status.Failed);
                mainMenu.ShowMessage("joining to game failed, try to find other matches...");

                if (!string.IsNullOrEmpty(extendedInfo))
                {
                    mainMenu.ShowMessage(extendedInfo);
                }

                StartAutomaticMatchmaking();
            }
        }


        public override void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches)
        {
            if (success)
            {
                if (matches.Count != 0)
                {
                    mainMenu.ShowMessage("list of matches obtained: " + matches.Count + " matches");
                }

                foreach(var matchInfo in matches)
                {
                    mainMenu.ShowMessage("processing: " + matchInfo.name);

                    if (_myOwnRoom != null)
                    {
                        if(_myOwnRoom.networkId == matchInfo.networkId)
                        {
                            mainMenu.ShowMessage("this match is my own - skip");
                            continue;
                        }
                    }

                    if(matchInfo.currentSize >= matchInfo.maxSize)
                    {
                        mainMenu.ShowMessage("this match is full - skip");
                        continue;
                    }

                    if(IsRoomCreatedEarlier(_myOwnRoomCreationDate, matchInfo.name))
                    {
                        _matchToJoin = matchInfo;
                        _roomFound = true;

                        mainMenu.ShowMessage("founded suitable match: " + matchInfo.name);
                        break;                        
                    }else
                    {
                        mainMenu.ShowMessage("this match is created later than my own");
                    }
                }

                if (!_roomFound)
                {
                    _doNextRequest = true;
                }
            }
            else
            {
                mainMenu.ShowMessage("could not to connect to match maker!");
            }

            if (!string.IsNullOrEmpty(extendedInfo))
            {
                mainMenu.ShowMessage(extendedInfo);
            }
        }

        public override void OnStartClient(NetworkClient lobbyClient)
        {
            base.OnStartClient(lobbyClient);

            NetworkMessages.Instance.SubscribeServerMessages(lobbyClient);
        }

        public override void OnStartServer()
        {
            base.OnStartServer();

            NetworkMessages.Instance.SubscribeClientMessages();
        }

        public override void OnLobbyServerPlayersReady()
        {
            bool allready = true;
            for (int i = 0; i < lobbySlots.Length; ++i)
            {
                if (lobbySlots[i] != null)
                {
                    allready &= lobbySlots[i].readyToBegin;
                }
            }

            if (allready)
            {
                ServerChangeScene(playScene);
            }
        }

        public override void OnLobbyStartServer()
        {
            base.OnLobbyStartServer();
            mainMenu.ShowMessage("lobby server started...");
            mainMenu.ShowMessage("waiting for players...");
        }

        public override GameObject OnLobbyServerCreateLobbyPlayer(NetworkConnection conn, short playerControllerId)
        {
            mainMenu.ShowMessage("player " +  playerControllerId + " joined game");

            return base.OnLobbyServerCreateLobbyPlayer(conn, playerControllerId);
        }

        /*public override void OnLobbyStopHost()
        {
            ServerChangeScene(lobbyScene);
        }

        public override void OnLobbyServerDisconnect(NetworkConnection conn)
        {
            ServerChangeScene(lobbyScene);
        }*/
    }
}
