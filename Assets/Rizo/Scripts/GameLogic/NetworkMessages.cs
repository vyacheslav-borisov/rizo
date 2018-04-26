using UnityEngine;
using UnityEngine.Networking;

namespace Pegas.Rizo
{
    public class NetworkMessages
    {
        public static NetworkMessages Instance { get; private set; }

        private const short MT_PlayerReady = MsgType.Highest + 1;
        private const short MT_RapidEvent = MsgType.Highest + 2;
        private const short MT_PlayerWinStrike = MsgType.Highest + 3;
        private const short MT_AssignStrikePoint = MsgType.Highest + 4;


        public class PlayerReady : MessageBase
        {
            public NetworkInstanceId _playerID;
        };

        public class PlayerWinStrike: MessageBase
        {
            public NetworkInstanceId _winnerPlayerID;
        }

        public class RapidEvent : MessageBase
        {
            public int _rapidEvent;
        };

        public class AssignStrikePoint : MessageBase
        {
            public NetworkInstanceId _playerID;
            public string _pointName;
        };

        public NetworkMessageDelegate OnPlayerReady;
        public NetworkMessageDelegate OnPlayerWinStrike;
        public NetworkMessageDelegate OnRapidEvent;
        public NetworkMessageDelegate OnRapidEventComplete;
        public NetworkMessageDelegate OnStrikePointAssigned;

        public NetworkMessages()
        {
            Instance = this;
        }

        private NetworkClient _client;

        public void SubscribeServerMessages(NetworkClient client)
        {
            Debug.Log("NetworkMessages.SubscribeServerMessages");

            _client = client;

            client.RegisterHandler(MT_PlayerReady, MessageHandler_PlayerReady);
            client.RegisterHandler(MT_RapidEvent, MessageHandler_OnRapidEvent);
            client.RegisterHandler(MT_PlayerWinStrike, MessageHandler_PlayerWinStrike);
            client.RegisterHandler(MT_AssignStrikePoint, MessageHandler_StrikePointAssigned);            
        }

        public void SubscribeClientMessages()
        {
            NetworkServer.RegisterHandler(MT_RapidEvent, MessageHandler_OnRapidEventComplete);
        }

        public void NotifyClients_PlayerReady(NetworkInstanceId playerID)
        {
            Debug.Log("NetworkMessages.NotifyClients_PlayerReady");

            var message = new PlayerReady();
            message._playerID = playerID;
            NetworkServer.SendToAll(MT_PlayerReady, message);
        }

        public void NotifyClients_PlayerWinStrike(NetworkInstanceId winnerPlayerID)
        {
            Debug.Log("NetworkMessages.NotifyClients_PlayerReady");

            var message = new PlayerWinStrike();
            message._winnerPlayerID = winnerPlayerID;
            NetworkServer.SendToAll(MT_PlayerWinStrike, message);
        }

        public void NotifyClients_OnPlayerAssignStrikePoint(NetworkInstanceId playerID, string strikePointName)
        {
            Debug.Log("NetworkMessages.NotifyClients_OnPlayerAssignStrikePoint");

            var message = new AssignStrikePoint();
            message._playerID = playerID;
            message._pointName = strikePointName;
            NetworkServer.SendToAll(MT_AssignStrikePoint, message);
        }

        public void NotifyClients_OnRapidStage(int stage)
        {
            Debug.Log("NetworkMessages.NotifyClients_OnRapidStage");

            var message = new RapidEvent();
            message._rapidEvent = stage;
            NetworkServer.SendToAll(MT_RapidEvent, message);
        }

        public void NotifyServer_RapidStageComplete(int stage)
        {
            Debug.Log("NetworkMessages.NotifyServer_RapidStageComplete");

            if(_client != null)
            {
                var message = new RapidEvent();
                message._rapidEvent = stage;

                _client.Send(MT_RapidEvent, message);
            }
        }

        private void MessageHandler_PlayerReady(NetworkMessage message)
        {
            Debug.Log("NetworkMessages.MessageHandler_PlayerReady");

            if (OnPlayerReady != null)
            {
                OnPlayerReady.Invoke(message);
            }
        }

        private void MessageHandler_PlayerWinStrike(NetworkMessage message)
        {
            Debug.Log("NetworkMessages.MessageHandler_PlayerWinStrike");

            if (OnPlayerWinStrike != null)
            {
                OnPlayerWinStrike.Invoke(message);
            }
        }

        private void MessageHandler_StrikePointAssigned(NetworkMessage message)
        {
            Debug.Log("NetworkMessages.MessageHandler_StrikePointAssigned");

            if(OnStrikePointAssigned != null)
            {
                OnStrikePointAssigned.Invoke(message);
            }
        }

        private void MessageHandler_OnRapidEvent(NetworkMessage message)
        {
            Debug.Log("NetworkMessages.MessageHandler_OnRapidEvent");

            if(OnRapidEvent != null)
            {
                OnRapidEvent.Invoke(message);
            }
        }

        private void MessageHandler_OnRapidEventComplete(NetworkMessage message)
        {
            Debug.Log("NetworkMessages.MessageHandler_OnRapidEventComplete");

            if (OnRapidEventComplete != null)
            {
                OnRapidEventComplete.Invoke(message);
            }
        }
    }
}
