using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartMark : MonoBehaviour
{
    public Camera _camera;
    public Transform _bodyPart;

    public float _nearFocusDistance = 90.0f;
    public float _farFocusDistance = 150.0f;
    [Range(0, 1)]
    public float _kParam = 1.0f;

    void Update ()
    {
        var screenCoords = _camera.WorldToScreenPoint(_bodyPart.position);
        var ray = _camera.ScreenPointToRay(screenCoords);

        transform.position = ray.origin + ray.direction * Mathf.Lerp(_nearFocusDistance, _farFocusDistance, _kParam);
        transform.rotation = Quaternion.LookRotation(ray.direction, Vector3.up);
    }
}
