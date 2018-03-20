using UnityEngine;

namespace Pegas.Rizo
{
    public class ArenaHUD_SMB : StateMachineBehaviour
    {
        private int _hashFadeIn;
        private int _hashFadeOut;
        private bool _doOnce;

        private void Awake()
        {
            _hashFadeIn = Animator.StringToHash("FadeIn");
            _hashFadeOut = Animator.StringToHash("FadeOut");
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            _doOnce = true;
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            if (animatorStateInfo.shortNameHash == _hashFadeOut)
            {
                var hud = animator.GetComponent<ArenaHUD>();
                if(hud != null)
                {
                    hud.EventHandler_OnFadeOutComplete();
                }
            }
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            if (animatorStateInfo.shortNameHash == _hashFadeIn && animatorStateInfo.normalizedTime >= 1.0f)
            {
                if (_doOnce)
                {
                    _doOnce = false;
                    var hud = animator.GetComponent<ArenaHUD>();
                    if(hud != null)
                    {
                        hud.EventHandler_OnFadeInComplete();
                    }
                }
            }//if (animatorStateInfo.shortNameHash == _hashFadeIn && animatorStateInfo.normalizedTime >= 1.0f)
        }
    }
}
