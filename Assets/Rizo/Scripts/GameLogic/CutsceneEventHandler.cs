using UnityEngine;
using UnityEngine.Events;

namespace Pegas.Rizo
{
    public class CutsceneEventHandler : MonoBehaviour
    {
        public ArenaHUD hud;
        public BodyPartSelector bodyPartSelector;

        private void Awake()
        {
            var csp = GetComponent<CutscenePlayer>();
            csp.OnCutSceneStart += OnCutSceneStart;
            csp.OnCutSceneEnd += OnCutSceneFinish;
        }

        private void OnCutSceneStart(string name)
        {
            if (name == CutscenePlayer.CUTSCENE_LONG_INTRO
                || name == CutscenePlayer.CUTSCENE_SHORT_INTRO)
            {
                hud.ResetHUD();
                hud.FadeOut();
                return;
            }

            if (name == CutscenePlayer.CUTSCENE_LOCAL_PLAYER_ZOOM_IN)
            {
                hud.ShowLocalPlayerHUD();
                bodyPartSelector.ResetState();
                return;
            }

            if (name == CutscenePlayer.CUTSCENE_REMOTE_PLAYER_ZOOM_IN)
            {
                hud.ShowRemotePlayerHUD();
                bodyPartSelector.ResetState();
                return;
            }

            if (name == CutscenePlayer.CUTSCENE_REMOTE_PLAYER_ZOOM_OUT
                || name == CutscenePlayer.CUTSCENE_LOCAL_PLAYER_ZOOM_OUT)
            {
                hud.BackToCommonHUD();
                bodyPartSelector.ResetState();
                return;
            }
        }

        private void OnCutSceneFinish(string name)
        {
            if (name == CutscenePlayer.CUTSCENE_LONG_INTRO
                || name == CutscenePlayer.CUTSCENE_SHORT_INTRO)
            {
                hud.ShowCommonHUD();
                bodyPartSelector.SetCommonViewState();
                return;
            }

            if (name == CutscenePlayer.CUTSCENE_LOCAL_PLAYER_ZOOM_IN)
            {
                bodyPartSelector.SetLocalPlayerViewState();
                return;
            }

            if (name == CutscenePlayer.CUTSCENE_REMOTE_PLAYER_ZOOM_IN)
            {
                bodyPartSelector.SetRemotePlayerViewState();
                return;
            }

            if (name == CutscenePlayer.CUTSCENE_REMOTE_PLAYER_ZOOM_OUT
                || name == CutscenePlayer.CUTSCENE_LOCAL_PLAYER_ZOOM_OUT)
            {
                bodyPartSelector.SetCommonViewState();
                return;
            }
        }
    }
}
