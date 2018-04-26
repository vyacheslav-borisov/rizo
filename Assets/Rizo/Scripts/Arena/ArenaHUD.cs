using UnityEngine;

namespace Pegas.Rizo
{
    public class ArenaHUD : MonoBehaviour
    {
        public delegate void HUDEvent();

        public HUDEvent OnFadeInComplete;
        public HUDEvent OnFadeOutComplete;

        private Animator _animator;

        private int _hashResetCommonHUD;
        private int _hashShowCommonHUD;
        private int _hashShowZoomedHUD;
        private int _hashHideZoomedHUD;
        private int _hashShowLocalPlayerHUD;

        private int _hashShowReadyButton;
        private int _hashOnReadyButtonPressed;
        private int _hashOnLocalPlayerReady;
        private int _hashOnRemotePlayerReady;

        private int _hashFaderOn;
        private int _hashFitilOn;

        private Fitil _fitil;

        private void Awake()
        {
            _animator = GetComponent<Animator>();

            _hashResetCommonHUD = Animator.StringToHash("ResetCommonHUD");
            _hashShowCommonHUD = Animator.StringToHash("ShowCommonHUD");
            _hashShowZoomedHUD = Animator.StringToHash("ShowZoomedHUD");
            _hashHideZoomedHUD = Animator.StringToHash("HideZoomedHUD");
            _hashShowLocalPlayerHUD = Animator.StringToHash("ShowLocalPlayerHUD");

            _hashShowReadyButton = Animator.StringToHash("ShowReadyButton");
            _hashOnReadyButtonPressed = Animator.StringToHash("PressReadyButton");
            _hashOnLocalPlayerReady = Animator.StringToHash("LocalPlayerReady");
            _hashOnRemotePlayerReady = Animator.StringToHash("RemotePlayerReady");

            _hashFaderOn = Animator.StringToHash("FaderOn");
            _hashFitilOn = Animator.StringToHash("FitilOn");

            var fitilGO = transform.Find("FitilWrapper/Fitil");
            if(fitilGO != null)
            {
                _fitil = fitilGO.GetComponent<Fitil>();
            }

            
        }

        public void ResetHUD()
        {
            _animator.SetBool(_hashOnLocalPlayerReady, false);
            _animator.SetBool(_hashOnRemotePlayerReady, false);
            _animator.SetBool(_hashShowReadyButton, false);
            _animator.SetTrigger(_hashResetCommonHUD);
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

        public void HidePlayerReadyStatuses()
        {
            _animator.SetBool(_hashOnLocalPlayerReady, false);
            _animator.SetBool(_hashOnRemotePlayerReady, false);
        }

        public void FadeIn()
        {
            _animator.SetBool(_hashFaderOn, true);
        }

        public void FadeOut()
        {
            _animator.SetBool(_hashFaderOn, false);
        }

        public void ShowFitil()
        {
            _animator.SetBool(_hashFitilOn, true);
        }

        public void HideFitil()
        {
            _animator.SetBool(_hashFitilOn, false);
        }

        public void SetFitilAnimPos(float normalizedTime)
        {
            /*
            if(_fitil != null)
            {
                _fitil.SetPosition(normalizedTime);
            }else
            {
                Debug.LogWarning("Fitil object not found!!!");
            }*/
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
