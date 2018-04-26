using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pegas.Rizo
{
    public class CutscenePlayer : MonoBehaviour
    {
        public const string CUTSCENE_LONG_INTRO = "LongIntro";
        public const string CUTSCENE_SHORT_INTRO = "ShortIntro";
        public const string CUTSCENE_LOCAL_PLAYER_ZOOM_IN = "ZoomIn_LocalPlayer";
        public const string CUTSCENE_LOCAL_PLAYER_ZOOM_OUT = "ZoomOut_LocalPlayer";
        public const string CUTSCENE_REMOTE_PLAYER_ZOOM_IN = "ZoomIn_RemotePlayer";
        public const string CUTSCENE_REMOTE_PLAYER_ZOOM_OUT = "ZoomOut_RemotePlayer";
        public const string CUTSCENE_RAPID_1 = "Rapid_1";
        public const string CUTSCENE_RAPID_2 = "Rapid_2";

        private Dictionary<string, CPC_CameraPath> _cutscenes = new Dictionary<string, CPC_CameraPath>();

        public delegate void CutSceneEvent(string sceneName);
        public delegate void CutSceneEvent2(string sceneName, float normalizedTime);

        public CutSceneEvent    OnCutSceneStart;
        public CutSceneEvent    OnCutSceneEnd;
        public CutSceneEvent2   OnCutSceneUpdate;

        public static CutscenePlayer Instance
        {
            get; private set;
        }

        public CutscenePlayer()
        {
            Instance = this;
        }

        private void Awake()
        {
            var cutscenes = GetComponentsInChildren<CPC_CameraPath>();

            foreach (var cutscene in cutscenes)
            {
                var key = cutscene.gameObject.name;
                _cutscenes.Add(key, cutscene);
            }
        }

        public void PlayScene(string cutSceneName, Camera camera)
        {
            if (!_cutscenes.ContainsKey(cutSceneName))
            {
                Debug.LogWarning("Unknown cutscene: " + cutSceneName);
                return;
            }

            if (_observerLoop != null)
            {
                Debug.LogWarning("Over cutscene is playing");
                return;
            }

            var cutScene = _cutscenes[cutSceneName];

            cutScene.useMainCamera = false;
            cutScene.selectedCamera = camera;
            cutScene.PlayPath(cutScene.playOnAwakeTime);

            _observerLoop = StartCoroutine(Coroutine_PathObserver(cutScene));
        }

        private Coroutine _observerLoop;
        private IEnumerator Coroutine_PathObserver(CPC_CameraPath cutScene)
        {
            if (OnCutSceneStart != null)
            {
                OnCutSceneStart(cutScene.gameObject.name);
            }

            float duration = cutScene.playOnAwakeTime;
            float elapsedTime = 0.0f;
            float normalizedTime = 0.0f;

            while (cutScene.IsPlaying())
            {
                if (OnCutSceneUpdate != null)
                {
                    elapsedTime += Time.deltaTime;
                    normalizedTime = elapsedTime / duration;
                    normalizedTime = Mathf.Clamp01(normalizedTime);

                    OnCutSceneUpdate(cutScene.gameObject.name, normalizedTime);
                }
                
                yield return null;
            }

            if (OnCutSceneEnd != null)
            {
                OnCutSceneEnd(cutScene.gameObject.name);
            }

            _observerLoop = null;
        }
    }
}
