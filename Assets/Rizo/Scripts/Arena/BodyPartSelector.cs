using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Pegas.Rizo
{
    public class BodyPartSelector : MonoBehaviour
    {
        public enum State
        {
            None,
            CommonView,
            LocalPlayerView,
            RemotePlayerView            
        }

        public Camera _camera;
        public CutscenePlayer _cutscenes;
        public Button _backButton;
        public DnDShield _shieldButton;  

        public Color _attackBPColor;
        public Color _deffenceBPColor;
        public float _longHoldTressHold = 2.0f;
        public float _longMoveTressHold = 20.0f;
        public float _simpleClickMaxTime = 1.0f;
        public float _simpleClickMaxRadius = 10.0f;

        public GameObject _longHoldEffect;
        public MoveGestureEffect _moveGestureEffect;
        public FireEdgeEffect _fireEdgeEffect;
        public PoofEffect     _simpleHitEffect;
        public PoofEffect     _hardHitEffect;
        //TODO: temporary body part effect
        public BodyPartMark _bodyPartMarkEffect;
        private BodyPartMark _attackedBodyPartMark;
        private BodyPartMark _defencedBodyPartMark;

        private BodyPart _prevBodyPart = null;
        private BodyPart _lastSelectedBodyPart = null;
        private State _currenState = State.None;

        private VFXImageEffect _transparentRenderLayer;

        public delegate void EventHandler(BodyPartType bodyPartType, HitType hitType);
        public delegate void EventHandler2(BodyPartType bodyPartType);

        public EventHandler OnChoosedAttackedBodyPart;
        public EventHandler2 OnChoosedDefencedBodyPart;

        private void Awake()
        {
            _backButton.onClick.AddListener(Event_OnBackButtonClicked);
            _shieldButton.OnShieldDropped = Event_ShieldDroppedEvent;

            var child = _camera.transform.Find("VFXCamera_Transparent");
            if(child != null)
            {
                _transparentRenderLayer = child.GetComponent<VFXImageEffect>();
            }

            _cutscenes.OnCutSceneUpdate += Event_OnZoomUpdate;
        }

        private bool Event_ShieldDroppedEvent()
        {
            if(_currenState != State.LocalPlayerView)
            {
                return false;
            }

            RaycastHit hitInfo;
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hitInfo, 100.0f, 1 << LayerMask.NameToLayer("Player")))
            {
                var bodyPart = hitInfo.collider.gameObject.GetComponent<BodyPart>();
                if(bodyPart.bpType != BodyPartType.Unknown)
                {
                    if (_defencedBodyPartMark != null)
                    {
                        Destroy(_defencedBodyPartMark.gameObject);
                        _defencedBodyPartMark = null;
                    }

                    _defencedBodyPartMark = CreateBodyPartMark(_bodyPartMarkEffect, bodyPart);
                    
                    if(OnChoosedDefencedBodyPart != null)
                    {
                        OnChoosedDefencedBodyPart(bodyPart.bpType);
                    }                   

                    return true;
                }   
            }

            return false;
        }

        private void Event_OnBackButtonClicked()
        {
            if(_currenState == State.LocalPlayerView)
            {
                _cutscenes.PlayScene(CutscenePlayer.CUTSCENE_LOCAL_PLAYER_ZOOM_OUT, _camera);                               
            }

            if (_currenState == State.RemotePlayerView)
            {
                _cutscenes.PlayScene(CutscenePlayer.CUTSCENE_REMOTE_PLAYER_ZOOM_OUT, _camera);
            }
        }

        private void Event_OnZoomUpdate(string sceneName, float normalizedTime)
        {
            if(_transparentRenderLayer == null)
            {
                return;
            }
            
            if(sceneName == CutscenePlayer.CUTSCENE_LOCAL_PLAYER_ZOOM_IN 
                || sceneName == CutscenePlayer.CUTSCENE_REMOTE_PLAYER_ZOOM_IN)
            {
                _transparentRenderLayer._shaderAlpha = 1 - normalizedTime;

                if (_attackedBodyPartMark != null)
                {
                    _attackedBodyPartMark._kParam = 1 - normalizedTime;
                }

                return;
            }

            if (sceneName == CutscenePlayer.CUTSCENE_LOCAL_PLAYER_ZOOM_OUT
                || sceneName == CutscenePlayer.CUTSCENE_REMOTE_PLAYER_ZOOM_OUT)
            {
                _transparentRenderLayer._shaderAlpha = normalizedTime;

                if (_attackedBodyPartMark != null)
                {
                    _attackedBodyPartMark._kParam = normalizedTime;
                }

                return;
            }
            
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        //shortcuts for editor
        public void SetCommonViewState()
        {
            SetCurrentState(State.CommonView);
        }

        public void SetRemotePlayerViewState()
        {
            SetCurrentState(State.RemotePlayerView);
        }

        public void SetLocalPlayerViewState()
        {
            SetCurrentState(State.LocalPlayerView);
        }

        public void ResetState()
        {
            SetCurrentState(State.None);
        }

        public void SetCurrentState(State state)
        {
            _currenState = state;
            
            switch(_currenState)
            {
                case State.CommonView:
                    {
                        StartCoroutine(Coroutine_CheckCharacterClick());
                    }
                    break;
                case State.LocalPlayerView:
                    {
                        StartCoroutine(Coroutine_BodyPartHL(true));
                    }
                    break;
                case State.RemotePlayerView:
                    {
                        StartCoroutine(Coroutine_BodyPartHL(false));
                        StartCoroutine(Coroutine_CheckGestures());
                    }                
                    break;
            }                        
        }        

        private IEnumerator Coroutine_CheckCharacterClick()
        {
            while (_currenState == State.CommonView)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Debug.Log("mouse button was pressed");

                    RaycastHit hitInfo;
                    Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out hitInfo, 100.0f, 1 << LayerMask.NameToLayer("Player")))
                    {
                        PlayerPawnProxy player = hitInfo.collider.gameObject.GetComponentInParent<PlayerPawnProxy>();
                        if (player != null)
                        {
                            if (player.IsLocalPlayer())
                            {
                                Debug.Log("Local Player clicked");

                                _cutscenes.PlayScene(CutscenePlayer.CUTSCENE_LOCAL_PLAYER_ZOOM_IN, _camera);
                                yield break;
                            }
                            else
                            {
                                Debug.Log("Remote Player clicked");

                                _cutscenes.PlayScene(CutscenePlayer.CUTSCENE_REMOTE_PLAYER_ZOOM_IN, _camera);
                                yield break;
                            }
                        }
                    }
                }//if (Input.GetMouseButtonDown(0))

                yield return null;
            }//while (true)
        }//Coroutine_CheckCharacterClick()

        private IEnumerator Coroutine_CheckGestures()
        {
            GestureRecognizer recognizer = new GestureRecognizer();

            GestureRecognizer.GestureSimpleClick simpleClick = new GestureRecognizer.GestureSimpleClick("SimpleClick");
            simpleClick.MaxTime = _simpleClickMaxTime;
            simpleClick.MaxRadius = _simpleClickMaxRadius;

            GestureRecognizer.GestureLongHold hardClick = new GestureRecognizer.GestureLongHold("HardClick");
            hardClick.TressHoldTime = _longHoldTressHold;

            GestureRecognizer.GestureLongMove longMove = new GestureRecognizer.GestureLongMove("LongMove");
            longMove.TressHoldLength = _longMoveTressHold;
            longMove.Mode = GestureRecognizer.GestureLongMove.Constraint.AlongX;

            longMove.Priority = 0;
            hardClick.Priority = 1;
            simpleClick.Priority = 2;

            recognizer.AddGesture(simpleClick);
            recognizer.AddGesture(hardClick);
            recognizer.AddGesture(longMove);

            recognizer.OnRecognized = CallBack_OnGestureRecognized;

            hardClick.OnStartRecognizing = CallBack_OnHardClickStart;
            longMove.OnStartRecognizing = CallBack_OnLongMoveStart;

            while (_currenState == State.RemotePlayerView)
            {
                recognizer.Update();

                yield return null;
            }            
        }

        private IEnumerator Coroutine_BodyPartHL(bool isLocal)
        {
            while (_currenState == State.LocalPlayerView || _currenState == State.RemotePlayerView)
            {
                if (Input.GetMouseButton(0))
                {
                    BodyPart currentBodyPart = null;

                    RaycastHit hitInfo;
                    Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out hitInfo, 100.0f, 1 << LayerMask.NameToLayer("Player")))
                    {
                        currentBodyPart = hitInfo.collider.gameObject.GetComponent<BodyPart>();
                        _lastSelectedBodyPart = currentBodyPart;
                    }

                    if (currentBodyPart != null && _prevBodyPart != null)
                    {
                        if (_prevBodyPart.GetInstanceID() != currentBodyPart.GetInstanceID())
                        {
                            _prevBodyPart.HighLightOff();
                        }
                    }
                    else if (_prevBodyPart != null)
                    {
                        _prevBodyPart.HighLightOff();
                    }

                    if (currentBodyPart != null)
                    {
                        currentBodyPart.HighLightOn(isLocal ? _deffenceBPColor : _attackBPColor);
                        _prevBodyPart = currentBodyPart;
                    }

                }
                else
                {
                    if (_prevBodyPart != null)
                    {
                        _prevBodyPart.HighLightOff();
                        _prevBodyPart = null;
                    }

                }//if (Input.GetMouseButtonDown(0))

                yield return null;
            }//while (true)

            if (_prevBodyPart != null)
            {
                _prevBodyPart.HighLightOff();
                _prevBodyPart = null;
            }
        }//Coroutine_BodyPartHL

        private void CallBack_OnHardClickStart(GestureRecognizer.Gesture gesture)
        {
            RaycastHit hitInfo;
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hitInfo, 100.0f, 1 << LayerMask.NameToLayer("Player")))
            {
                var playerProxy = hitInfo.collider.GetComponentInParent<PlayerPawnProxy>();
                if (playerProxy && !playerProxy.IsLocalPlayer())
                {
                    Instantiate(_longHoldEffect);                    
                }
            }
        }

        private Vector3 _longMove_StartPos;
        private Vector3 _longMove_EndPos;

        private void CallBack_OnLongMoveStart(GestureRecognizer.Gesture gesture)
        {
            GameObject go = Instantiate(_moveGestureEffect.gameObject);
            MoveGestureEffect mge = go.GetComponent<MoveGestureEffect>();
            mge._camera = _camera;

            _longMove_StartPos = Input.mousePosition;
        }

        private void CallBack_OnGestureRecognized(GestureRecognizer.Gesture gesture)
        {
            Debug.Log("Gesture " + gesture.ID + " is recognized");

            if (gesture.ID == "LongMove")
            {
                _longMove_EndPos = Input.mousePosition;
                var delta = _longMove_EndPos - _longMove_StartPos;
                var center = _longMove_StartPos + delta.normalized * delta.magnitude * 0.5f;

                RaycastHit hitInfo;
                Ray ray = _camera.ScreenPointToRay(center);
                if (Physics.Raycast(ray, out hitInfo, 100.0f, 1 << LayerMask.NameToLayer("Player")))
                {
                    var position = ray.origin + (hitInfo.point - ray.origin) * 0.8f;
                    //var start = _camera.ScreenToWorldPoint(_longMove_StartPos);
                    //var end = _camera.ScreenToWorldPoint(_longMove_EndPos);
                    var start = _camera.ScreenToViewportPoint(_longMove_StartPos);
                    var end = _camera.ScreenToViewportPoint(_longMove_EndPos);

                    var radius = (end - start).magnitude * 0.5f;

                    Debug.Log("screenStart = " + _longMove_StartPos);
                    Debug.Log("screenEnd = " + _longMove_EndPos);
                    Debug.Log("start = " + start);
                    Debug.Log("end = " + end);
                    Debug.Log("radius = " + radius);

                    var go = Instantiate(_fireEdgeEffect.gameObject, position, Quaternion.identity);
                    var effect = go.GetComponent<FireEdgeEffect>();
                    effect.Radius = radius;

                    if (_lastSelectedBodyPart != null)
                    {
                        if(_attackedBodyPartMark != null)
                        {
                            Destroy(_attackedBodyPartMark.gameObject);
                            _attackedBodyPartMark = null;
                        }

                        var bodyPart = hitInfo.collider.GetComponent<BodyPart>();
                        if (bodyPart != null)
                        {
                            _attackedBodyPartMark = CreateBodyPartMark(_bodyPartMarkEffect, bodyPart);
                        }

                        if(OnChoosedAttackedBodyPart != null)
                        {
                            OnChoosedAttackedBodyPart(bodyPart.bpType, HitType.Breakable);
                        }
                    }
                }
            }


            if (gesture.ID == "SimpleClick")
            {
                var ray = _camera.ScreenPointToRay(Input.mousePosition);
                var go = Instantiate(_simpleHitEffect.gameObject, Vector3.zero, Quaternion.identity);
                var effect = go.GetComponent<PoofEffect>();
                effect._focusRay = ray;

                if (_lastSelectedBodyPart != null)
                {
                    if (_attackedBodyPartMark != null)
                    {
                        Destroy(_attackedBodyPartMark.gameObject);
                        _attackedBodyPartMark = null;
                    }

                    _attackedBodyPartMark = CreateBodyPartMark(_bodyPartMarkEffect, _lastSelectedBodyPart);

                    if (OnChoosedAttackedBodyPart != null)
                    {
                        OnChoosedAttackedBodyPart(_lastSelectedBodyPart.bpType, HitType.Simple);
                    }
                }
            }

            if (gesture.ID == "HardClick")
            {
                var ray = _camera.ScreenPointToRay(Input.mousePosition);
                var go = Instantiate(_hardHitEffect.gameObject, Vector3.zero, Quaternion.identity);
                var effect = go.GetComponent<PoofEffect>();
                effect._focusRay = ray;

                if (_lastSelectedBodyPart != null)
                {
                    if (_attackedBodyPartMark != null)
                    {
                        Destroy(_attackedBodyPartMark.gameObject);
                        _attackedBodyPartMark = null;
                    }

                    _attackedBodyPartMark = CreateBodyPartMark(_bodyPartMarkEffect, _lastSelectedBodyPart);

                    if (OnChoosedAttackedBodyPart != null)
                    {
                        OnChoosedAttackedBodyPart(_lastSelectedBodyPart.bpType, HitType.Heavy);
                    }
                }
            }
        }//CallBack_OnGestureRecognized

        private BodyPartMark CreateBodyPartMark(BodyPartMark prefab, BodyPart target)
        {
            var instance = Instantiate(prefab.gameObject, Vector3.zero, Quaternion.identity);
            var effect = instance.GetComponent<BodyPartMark>();
            effect._bodyPart = target.transform;
            effect._camera = _camera;

            return effect;
        }
    }
}
