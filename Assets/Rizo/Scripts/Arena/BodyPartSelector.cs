using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        public Color _attackBPColor;
        public Color _deffenceBPColor;
        public float _longHoldTressHold = 2.0f;
        public float _longMoveTressHold = 20.0f;
        public float _simpleClickMaxTime = 1.0f;
        public float _simpleClickMaxRadius = 10.0f;

        public GameObject _longHoldEffect;
        public MoveGestureEffect _moveGestureEffect;
        public FireEdgeEffect _fireEdgeEffect;

        private ArenaCamera _arenaCamera;
        private Camera      _camera;
        private BodyPart _prevBodyPart = null;
        private State _currenState = State.None;

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

        private void Awake()
        {
            _arenaCamera = GetComponent<ArenaCamera>();
            _camera = GetComponent<Camera>();
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

                                _arenaCamera.ZoomInLocalPlayer();
                                yield break;
                            }
                            else
                            {
                                Debug.Log("Remote Player clicked");

                                _arenaCamera.ZoomInRemotePlayer();
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
            /*recognizer.OnRecognized = (GestureRecognizer.Gesture gesture) =>
            {
                Debug.Log("Gesture " + gesture.ID + " is recognized");
            };*/

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
                    var position = transform.position + (hitInfo.point - transform.position) * 0.8f;
                    Instantiate(_longHoldEffect, position, Quaternion.identity);
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

            if (gesture.ID != "LongMove")
            {
                return;
            }

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

                GameObject go = Instantiate(_fireEdgeEffect.gameObject, position, Quaternion.identity);
                FireEdgeEffect effect = go.GetComponent<FireEdgeEffect>();
                effect.Radius = radius; 
            }
        }
    }
}
