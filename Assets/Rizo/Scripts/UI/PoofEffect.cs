using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoofEffect : MonoBehaviour
{
    public Ray   _focusRay;
    public float _focusDistance = 20.0f;

    void Start ()
    {
        transform.position = _focusRay.origin + _focusRay.direction * _focusDistance;
        transform.rotation = Quaternion.LookRotation(_focusRay.direction, Vector3.up);

        var ps = GetComponentInChildren<ParticleSystem>();
        Destroy(gameObject, ps.main.duration);                		
	}
}
