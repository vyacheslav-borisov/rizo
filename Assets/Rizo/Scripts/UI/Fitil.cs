using UnityEngine;

public class Fitil : MonoBehaviour
{
    private RectTransform _leftTarget;
    private RectTransform _rightTarget;

    private Vector2 _leftTargetInitialPosition;
    private Vector2 _rightTargetInitialPosition;

    private void Awake()
    {
        _leftTarget = transform.Find("target_left") as RectTransform;
        _rightTarget = transform.Find("target_right") as RectTransform;

        if(_leftTarget != null && _rightTarget != null)
        {
            _leftTargetInitialPosition = _leftTarget.anchoredPosition;
            _rightTargetInitialPosition = _rightTarget.anchoredPosition;
        }
    }

    public void SetPosition(float normalizedTime)
    {
        normalizedTime = Mathf.Clamp01(normalizedTime);

        _leftTarget.anchoredPosition = Vector2.Lerp(_leftTargetInitialPosition, _rightTargetInitialPosition, normalizedTime);
        _rightTarget.anchoredPosition = Vector2.Lerp(_rightTargetInitialPosition, _leftTargetInitialPosition, normalizedTime);
    }
}
