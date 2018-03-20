using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;


namespace Pegas.Rizo
{
    public enum BodyPartType
    {
        Unknown,
        Torso,
        Head,
        Neck,
        Chest,
        LeftHand,
        RightHand
    }

    public enum HitType
    {
        Unknown,
        Simple,
        Heavy,
        Breakable
    }

    public enum SpearType
    {
        Unknown,
        Light,
        Heavy
    }

    public class TurnMaker
    {
        private class PlayerSessionData
        {
            public PlayerSessionData()
            {
                Reset();
                _isAssigned = false;
            }

            public void Reset()
            {
                _droppedArmor.Clear();

                _attackedBodyPart = BodyPartType.Torso;
                _defencedBodyPart = BodyPartType.Torso;
                _hitType = HitType.Simple;
                _spearType = SpearType.Light;
                _isReady = false;
            }

            public NetworkInstanceId _playerID;
            public BodyPartType _attackedBodyPart = BodyPartType.Torso;
            public BodyPartType _defencedBodyPart = BodyPartType.Torso;
            public HitType      _hitType = HitType.Simple;
            public SpearType    _spearType = SpearType.Light; 
            public bool _isReady = false;
            public bool _isAssigned = false;

            public List<BodyPartType> _droppedArmor = new List<BodyPartType>();
        };

        PlayerSessionData _player1 = new PlayerSessionData();
        PlayerSessionData _player2 = new PlayerSessionData();

        public void Reset()
        {
            _player1.Reset();
            _player2.Reset();
        }

        public bool AssignPlayer(NetworkInstanceId playerID)
        {
            var slot = GetUnassignedSlot();
            if (slot != null)
            {
                slot.Reset();
                slot._playerID = playerID;
                slot._isAssigned = true;

                return true;
            }

            return false;
        }

        public void SetPlayerReady(NetworkInstanceId playerID)
        {
            var player = GetPlayer(playerID);

            if (player != null)
            {
                player._isReady = true;
            }            
        }

        public void ChooseSpear(NetworkInstanceId playerID, SpearType spearType)
        {
            var player = GetPlayer(playerID);

            if (player != null)
            {
                player._spearType = spearType;                
            }
        }

        public void ChooseHitType(NetworkInstanceId playerID, HitType hitType)
        {
            var player = GetPlayer(playerID);

            if (player != null)
            {
                player._hitType = hitType;
            }
        }

        public void ChooseAttackedBodyPart(NetworkInstanceId playerID, BodyPartType bodyPart)
        {
            var player = GetPlayer(playerID);

            if (player != null)
            {
                player._attackedBodyPart = bodyPart;
            }
        }

        public void ChooseDefendedBodyPart(NetworkInstanceId playerID, BodyPartType bodyPart)
        {
            var player = GetPlayer(playerID);

            if (player != null)
            {
                player._defencedBodyPart = bodyPart;
            }
        }

        public void DropArmorPart(NetworkInstanceId playerID, BodyPartType bodyPart)
        {
            var player = GetPlayer(playerID);

            if (player != null)
            {
                player._droppedArmor.Add(bodyPart);
            }
        }

        public bool IsBothPlayersReady()
        {
            if(_player1 == null || _player2 == null)
            {
                return false;
            }

            bool first = _player1._isAssigned && _player1._isReady;
            bool second = _player2._isAssigned && _player2._isReady;

            return first && second;
        }        

        private PlayerSessionData GetPlayer(NetworkInstanceId playerID)
        {
            if(_player1._playerID.Value == playerID.Value)
            {
                return _player1;
            }

            if (_player2._playerID.Value == playerID.Value)
            {
                return _player2;
            }

            return null;
        }
        
        private PlayerSessionData GetUnassignedSlot()
        {
            if(!_player1._isAssigned)
            {
                return _player1;
            }

            if(!_player2._isAssigned)
            {
                return _player2;
            }

            return null;
        }     
    }
}
