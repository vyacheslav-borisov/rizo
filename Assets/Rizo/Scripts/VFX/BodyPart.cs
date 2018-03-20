using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pegas.Rizo
{
    public class BodyPart : MonoBehaviour
    {
        public bool hideOnPlayerZoom;

        private Material _material;
        private Mesh _mesh;
        private int _colorParamID;
        private bool _isActive;

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
                if (player.IsLocalPlayer())
                {
                    ArenaCamera.Instance.OnLocalPlayer_ZoomStart.AddListener(Event_OnPlayerZoom);
                }
                else
                {
                    ArenaCamera.Instance.OnRemotePlayer_ZoomStart.AddListener(Event_OnPlayerZoom);
                }

                ArenaCamera.Instance.OnPlayer_UnZoomEnd.AddListener(Event_OnPlayerUnZoom);
            }
            else
            {
                gameObject.layer = LayerMask.NameToLayer("Player");
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
