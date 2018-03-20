using UnityEngine;
using UnityEngine.UI;
using System.Collections;


namespace Pegas.Rizo
{
    public class MainMenu : MonoBehaviour
    {
        public Text _statusControl;
        public Text _connecting;

        private Animator _animator;

        private int _hashHideButtons;
        private int _hashShowButtons;

        private void Awake()
        {
            _animator = GetComponent<Animator>();

            _hashShowButtons = Animator.StringToHash("ShowButtons");
            _hashHideButtons = Animator.StringToHash("HideButtons");

            SetStatus(Status.Offline);
        }

        public void ShowMessage(string message)
        {
            _statusControl.text += message + System.Environment.NewLine;
            Debug.Log(message);
        }

        public enum Status
        {
            Offline,
            Connecting,
            SearchPlayers,
            JoinToGame,
            Failed,
            Stopped
        };
        private string _statusMessage;
        private bool _showDots;
        public void SetStatus(Status status)
        {
            switch(status)
            {
                case Status.Offline:
                    _statusMessage = "offline";
                    _showDots = false;
                    break;
                case Status.Connecting:
                    _statusMessage = "connecting";
                    _showDots = true;
                    break;
                case Status.SearchPlayers:
                    _statusMessage = "search players";
                    _showDots = true;
                    break;
                case Status.JoinToGame:
                    _statusMessage = "joining to game";
                    _showDots = true;
                    break;
                case Status.Failed:
                    _statusMessage = "error";
                    _showDots = false;
                    break;
            }
        }

        public void HidePlayButton()
        {
            Debug.Log("HidePlayButoon");

            _animator.SetTrigger(_hashHideButtons);
            _loopConnecting = StartCoroutine(Coroutine_Connecting());
        }


        public void ShowPlayButton()
        {
            Debug.Log("ShowPlayButoon");

            _animator.SetTrigger(_hashShowButtons);

            if (_loopConnecting != null)
            {
                StopCoroutine(_loopConnecting);
            }
        }
        

        private Coroutine _loopConnecting = null;
        private IEnumerator Coroutine_Connecting()
        {
            while(true)
            {
                _ShowStatusMessage("");
                yield return new WaitForSeconds(0.3f);
                _ShowStatusMessage(".");
                yield return new WaitForSeconds(0.3f);
                _ShowStatusMessage("..");
                yield return new WaitForSeconds(0.3f);
                _ShowStatusMessage("...");
                yield return new WaitForSeconds(0.3f);
                _ShowStatusMessage("....");
                yield return new WaitForSeconds(0.3f);
            }
        }

        private void _ShowStatusMessage(string progressDots)
        {
            _connecting.text = _statusMessage;
            if(_showDots)
            {
                _connecting.text += progressDots;
            }
        }

        
    }
}
