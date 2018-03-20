using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Pegas.Rizo
{
    public class ArenaCamera : MonoBehaviour
    {
        public UnityEvent OnLongIntroFinished;
        public UnityEvent OnShortIntroFinished;
        public UnityEvent OnLocalPlayer_ZoomStart;
        public UnityEvent OnLocalPlayer_ZoomEnd;
        public UnityEvent OnRemotePlayer_ZoomStart;
        public UnityEvent OnRemotePlayer_ZoomEnd;
        public UnityEvent OnPlayer_UnZoomStart;
        public UnityEvent OnPlayer_UnZoomEnd;

        public static ArenaCamera Instance
        {
            get; private set;
        }

        public ArenaCamera()
        {
            Instance = this;
        }

        private Animator _animator;
        private int _hashLongIntro;
        private int _hashShortIntro;
        private int _hashZoomInLocalPlayer;
        private int _hashZoomInRemotePlayer;
        private int _hashZoomOut;

        private AudioSource _musicPlayer;
        
        private void Awake()
        {
            _animator = GetComponent<Animator>();

            _hashLongIntro = Animator.StringToHash("LongIntro");
            _hashShortIntro = Animator.StringToHash("ShortIntro");
            _hashZoomInLocalPlayer = Animator.StringToHash("MyPlayer_ZoomIn");
            _hashZoomInRemotePlayer = Animator.StringToHash("RemotePlayer_ZoomIn");
            _hashZoomOut = Animator.StringToHash("ZoomOut");

            _musicPlayer = GetComponent<AudioSource>();
            _musicPlayer.playOnAwake = false;
        }

        public void LongIntro(bool bPlayMusic = false)
        {
            _animator.SetTrigger(_hashLongIntro);

            if (bPlayMusic)
            {
                _musicPlayer.loop = true;
                _musicPlayer.Play();
            }
        }

        public void ShortIntro(bool bPlayMusic = false)
        {
            _animator.SetTrigger(_hashShortIntro);

            if (bPlayMusic)
            {
                _musicPlayer.loop = true;
                _musicPlayer.Play();
            }
        }

        public void ZoomInLocalPlayer()
        {
            _animator.SetTrigger(_hashZoomInLocalPlayer);
            
            if (OnLocalPlayer_ZoomStart != null)
            {
                OnLocalPlayer_ZoomStart.Invoke();
            }
        }

        public void ZoomInRemotePlayer()
        {
            _animator.SetTrigger(_hashZoomInRemotePlayer);
            
            if(OnRemotePlayer_ZoomStart != null)
            {
                OnRemotePlayer_ZoomStart.Invoke();
            }
        }

        public void ZoomOut()
        {
            _animator.SetTrigger(_hashZoomOut);
            
            if(OnPlayer_UnZoomStart != null)
            {
                OnPlayer_UnZoomStart.Invoke();
            }            
        }


        public void Event_OnLongIntroFinished()
        {
            if (OnLongIntroFinished != null)
            {
                OnLongIntroFinished.Invoke();
            }
        }

        public void Event_OnShortIntroFinished()
        {
            if (OnShortIntroFinished != null)
            {
                OnShortIntroFinished.Invoke();
            }
        }

        public void Event_OnLocalPlayer_ZoomedIn()
        {
            if(OnLocalPlayer_ZoomEnd != null)
            {
                OnLocalPlayer_ZoomEnd.Invoke();
            }
        }

        public void Event_OnRemotePlayer_ZoomedIn()
        {
            if(OnRemotePlayer_ZoomEnd != null)
            {
                OnRemotePlayer_ZoomEnd.Invoke();
            }
        }

        public void Event_ZoomOut()
        {
            if(OnPlayer_UnZoomEnd != null)
            {
                OnPlayer_UnZoomEnd.Invoke();
            }
        }        
    }  


}