using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    //Shake amount - distance to offset in any direction
    public float ShakeAmount = 3.0f;
    //Speed of camera moving to shake points
    public float ShakeSpeed = 2.0f;

    public float JitterAmount = 0.1f;
    public bool InterpolateJitter = true;

    public Transform TrackPoint;

    public bool EnableJitter = true;
    public bool EnableTracking = true;

    private Vector3 _originalPosition;

    private void OnEnable()
    {
        _originalPosition = transform.localPosition;
        _coroutineShake = StartCoroutine(Coroutine_Shake());
    }

    private void OnDisable()
    {
        if(_coroutineShake != null)
        {
            StopCoroutine(_coroutineShake);
            _coroutineShake = null;
            transform.localPosition = _originalPosition;
        }
    }

    private Coroutine _coroutineShake;
    private IEnumerator Coroutine_Shake()
    {
        float ellapsedTime = 0.0f;
        float deviation = 0.0f;
        //Repeat for total shake time
        while (true)
        {
            ellapsedTime += Time.deltaTime * ShakeSpeed;
            ellapsedTime = ellapsedTime % Mathf.PI;
            deviation = Mathf.Sin(ellapsedTime) * ShakeAmount;

            Vector3 randomPoint = _originalPosition;
            randomPoint.y += deviation;

            if (EnableJitter)
            {
                if (JitterAmount > 0.0f)
                {
                    randomPoint += Random.insideUnitSphere * JitterAmount;
                }

                if (InterpolateJitter)
                {
                    transform.localPosition = Vector3.Lerp(transform.localPosition, randomPoint, Time.deltaTime * ShakeSpeed);
                }
                else
                {
                    transform.localPosition = randomPoint;
                }
            }

            if(EnableTracking && TrackPoint != null)
            {
                var direction = TrackPoint.position - transform.position;
                transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
            }

            yield return null;
        }
    }
}
