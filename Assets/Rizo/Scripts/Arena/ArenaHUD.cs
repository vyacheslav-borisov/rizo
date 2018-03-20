using UnityEngine;

namespace Pegas.Rizo
{
    public class ArenaHUD : MonoBehaviour
    {
        public delegate void HUDEvent();

        public HUDEvent OnFadeInComplete;
        public HUDEvent OnFadeOutComplete;

        private Animator _animator;

        private int _hashShowCommonHUD;
        private int _hashShowZoomedHUD;
        private int _hashHideZoomedHUD;
        private int _hashShowLocalPlayerHUD;

        private int _hashShowReadyButton;
        private int _hashOnReadyButtonPressed;
        private int _hashOnLocalPlayerReady;
        private int _hashOnRemotePlayerReady;

        //private int _hashFadeIn;
        //private int _hashFadeOut;
        private int _hashFaderOn;

        private void Awake()
        {
            _animator = GetComponent<Animator>();

            _hashShowCommonHUD = Animator.StringToHash("ShowCommonHUD");
            _hashShowZoomedHUD = Animator.StringToHash("ShowZoomedHUD");
            _hashHideZoomedHUD = Animator.StringToHash("HideZoomedHUD");
            _hashShowLocalPlayerHUD = Animator.StringToHash("ShowLocalPlayerHUD");

            _hashShowReadyButton = Animator.StringToHash("ShowReadyButton");
            _hashOnReadyButtonPressed = Animator.StringToHash("PressReadyButton");
            _hashOnLocalPlayerReady = Animator.StringToHash("LocalPlayerReady");
            _hashOnRemotePlayerReady = Animator.StringToHash("RemotePlayerReady");

            //_hashFadeIn = Animator.StringToHash("FadeIn");
            //_hashFadeOut = Animator.StringToHash("FadeOut");
            _hashFaderOn = Animator.StringToHash("FaderOn");
        }

        public void ShowCommonHUD()
        {
            _animator.SetTrigger(_hashShowCommonHUD);
        }

        public void BackToCommonHUD()
        {
            _animator.SetTrigger(_hashHideZoomedHUD);
        }

        public void ShowLocalPlayerHUD()
        {
            _animator.SetTrigger(_hashShowLocalPlayerHUD);
        }

        public void ShowRemotePlayerHUD()
        {
            _animator.SetTrigger(_hashShowZoomedHUD);
        }

        public void ShowReadyButton()
        {
            _animator.SetBool(_hashShowReadyButton, true);
        }

        public void HideButtons()
        {
            _animator.SetTrigger(_hashOnReadyButtonPressed);
        }

        public void NotifyLocalPlayerReady()
        {
            _animator.SetBool(_hashOnLocalPlayerReady, true);
        }

        public void NotifyRemotePlayerReady()
        {
            _animator.SetBool(_hashOnRemotePlayerReady, true);
        }

        public void FadeIn()
        {
            _animator.SetBool(_hashFaderOn, true);
        }

        public void FadeOut()
        {
            _animator.SetBool(_hashFaderOn, false);
        }

        public void EventHandler_OnFadeInComplete()
        {
            if(OnFadeInComplete != null)
            {
                OnFadeInComplete();
            }
        }

        public void EventHandler_OnFadeOutComplete()
        {
            if(OnFadeOutComplete != null)
            {
                OnFadeOutComplete();
            }
        }
    }
}
