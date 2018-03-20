using UnityEngine;
using System.Collections;

namespace Pegas.Rizo
{
    public class ArenaCamera_SMB : StateMachineBehaviour
    {
        private ArenaCamera _camera = null;
        private int _hashMyPlayerZoomIn;
        private int _hashRemotePlayerZoomIn;
        private int _hashMyPlayerZoomOut;
        private int _hashRemotePlayerZoomOut;
        private int _hashLongIntro;
        private int _hashShortIntro;

        private bool _waitForAnimationFinish = false;
        private bool _isLocalPlayer = false;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if(_camera == null)
            {
                _camera = animator.GetComponent<ArenaCamera>();
                _hashMyPlayerZoomIn = Animator.StringToHash("Base Layer.MyPlayerZoomIn");
                _hashRemotePlayerZoomIn = Animator.StringToHash("Base Layer.RemotePlayerZoomIn");
                _hashMyPlayerZoomOut = Animator.StringToHash("Base Layer.MyPlayerZoomOut");
                _hashRemotePlayerZoomOut = Animator.StringToHash("Base Layer.RemotePlayerZoomOut");
                _hashLongIntro = Animator.StringToHash("Base Layer.LongIntro");
                _hashShortIntro = Animator.StringToHash("Base Layer.ShortIntro");
            }

            _waitForAnimationFinish = (stateInfo.fullPathHash == _hashMyPlayerZoomIn 
                || stateInfo.fullPathHash == _hashRemotePlayerZoomIn);         
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if ((stateInfo.fullPathHash == _hashMyPlayerZoomOut || stateInfo.fullPathHash == _hashRemotePlayerZoomOut) && _camera)
            {
                _camera.Event_ZoomOut();
            }

            if (stateInfo.fullPathHash == _hashLongIntro && _camera)
            {
                _camera.Event_OnLongIntroFinished();
            }

            if (stateInfo.fullPathHash == _hashShortIntro && _camera)
            {
                _camera.Event_OnShortIntroFinished();
            }
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if(_waitForAnimationFinish && stateInfo.normalizedTime >= 1.0f)
            {
                _waitForAnimationFinish = false;

                if (stateInfo.fullPathHash == _hashMyPlayerZoomIn && _camera)
                {
                    _camera.Event_OnLocalPlayer_ZoomedIn();                    
                }

                if (stateInfo.fullPathHash == _hashRemotePlayerZoomIn && _camera)
                {
                    _camera.Event_OnRemotePlayer_ZoomedIn();
                }                
            }            
        }
    }
}
