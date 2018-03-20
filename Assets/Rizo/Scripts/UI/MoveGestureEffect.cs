using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveGestureEffect : MonoBehaviour
{
    public float _tressHold = 0.0f;
    [HideInInspector]
    public Camera _camera;

    private ParticleSystem[] _particles;
    private Vector3          _startPosition;
    private bool _turnOn = false;

    private void Awake()
    {
        _particles = GetComponentsInChildren<ParticleSystem>();
        foreach(var ps in _particles)
        {
            var main = ps.main;
            main.playOnAwake = false;
            ps.Stop(true);
        }
    }

    private void Start()
    {
        _startPosition = Input.mousePosition;
    }

    private void Update()
    {
        if(!Input.GetMouseButton(0))
        {
            Destroy(gameObject);
            return;
        }

        var vDelta = Input.mousePosition - _startPosition;

        RaycastHit hitInfo;
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        bool raycasted = Physics.Raycast(ray, out hitInfo, 100.0f, 1 << LayerMask.NameToLayer("Player"));
        
        if(!_turnOn && raycasted && vDelta.magnitude >= _tressHold)
        {
            _turnOn = true;
            foreach (var ps in _particles)
            {
                ps.Play(true);
            }
        }

        if(raycasted)
        {
            transform.position = ray.origin + (hitInfo.point - ray.origin) * 0.8f;
        }
    }
}
