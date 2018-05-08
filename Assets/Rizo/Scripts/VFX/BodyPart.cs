using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pegas.Rizo
{
    public class BodyPart : MonoBehaviour
    {
        public bool hideOnPlayerZoom;
        public BodyPartType bpType;

        private Material _material;
        private Mesh _mesh;
        private int _colorParamID;
        private bool _isActive;
        private bool _isLocalPlayer;

        private void Awake()
        {
            _mesh = GetComponent<MeshFilter>().mesh;

            _material = Resources.Load<Material>("Materials/FlatColor");
            if (!_material)
            {
                Debug.LogError("material Materials/FlatColor not found!");
                enabled = false;
                return;
            }
            _colorParamID = Shader.PropertyToID("_Color");            
        }

        private void Start()
        {
            if (hideOnPlayerZoom)
            {
                PlayerPawnProxy player = GetComponentInParent<PlayerPawnProxy>();
                _isLocalPlayer = player.IsLocalPlayer();

                CutscenePlayer.Instance.OnCutSceneStart += Event_CutSceneStart;
                CutscenePlayer.Instance.OnCutSceneEnd += Event_CutSceneEnd;
            }
            else
            {
                gameObject.layer = LayerMask.NameToLayer("Player");
            }
        }

        private void Event_CutSceneStart(string cutSceneName)
        {
            if(_isLocalPlayer)
            {
                if (cutSceneName == CutscenePlayer.CUTSCENE_LOCAL_PLAYER_ZOOM_IN)
                {
                    Event_OnPlayerZoom();
                }
            }
            else 
            {
                //remote player
                if (cutSceneName == CutscenePlayer.CUTSCENE_REMOTE_PLAYER_ZOOM_IN)
                {
                    Event_OnPlayerZoom();
                }
            }            
        }

        private void Event_CutSceneEnd(string cutSceneName)
        {
            if (_isLocalPlayer)
            {
                if (cutSceneName == CutscenePlayer.CUTSCENE_LOCAL_PLAYER_ZOOM_OUT)
                {
                    Event_OnPlayerUnZoom();
                }
            }
            else
            {
                //remote player
                if (cutSceneName == CutscenePlayer.CUTSCENE_REMOTE_PLAYER_ZOOM_OUT)
                {
                    Event_OnPlayerUnZoom();
                }
            }
        }

        private void Event_OnPlayerZoom()
        {
            gameObject.layer = LayerMask.NameToLayer("VFX_Transparent");
        }

        private void Event_OnPlayerUnZoom()
        {
            gameObject.layer = LayerMask.NameToLayer("Player");
        }        

        private void Update()
        {
            if (_isActive)
            {
                // Debug.Log("material: " + _material.shader.name);
                Graphics.DrawMesh(_mesh, transform.localToWorldMatrix, _material, LayerMask.NameToLayer("VFX_Glow"));
            }
        }

        public void HighLightOn(Color color)
        {
            _material.SetColor(_colorParamID, color);
            _isActive = true;
        }

        public void HighLightOff()
        {
            _isActive = false;
        }
    }
}
