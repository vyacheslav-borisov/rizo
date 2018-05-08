using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DnDShield : EventTrigger
{
    public delegate bool ShieldDroppedEvent();
    public ShieldDroppedEvent OnShieldDropped;

    private Vector3 prevPosition;
    private bool dragStarted;

    private Animator hudAnimator;
    private Button backButton;
    private Image image; 

    private void Awake()
    {
        hudAnimator = GetComponentInParent<Animator>();
        backButton = transform.parent.Find("BackButton").GetComponent<Button>();
        image = GetComponent<Image>();
    }

    public override void OnBeginDrag(PointerEventData data)
    {
        if (coroutine == null)
        {
            hudAnimator.enabled = false;
            prevPosition = transform.position;
            dragStarted = true;
        }
    }

    public override void OnEndDrag(PointerEventData data)
    {
        dragStarted = false;

        if (OnShieldDropped != null)
        {
            if (OnShieldDropped())
            {
                coroutine = StartCoroutine(Coroutine_Disappear());
            }
            else
            {
                coroutine = StartCoroutine(Coroutine_BackToHome());
            }

        }
        else
        {
            coroutine = StartCoroutine(Coroutine_BackToHome());
        }
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if(dragStarted)
        {
            var position = transform.position;
            position.x = eventData.position.x;
            position.y = eventData.position.y;
            transform.position = position;
        }                              
    }

    private Coroutine coroutine;
    IEnumerator Coroutine_BackToHome()
    {
        backButton.enabled = false;

        var delta = prevPosition - transform.position;
        var startPosition = transform.position;
        var endPosition = prevPosition;

        const float speed = 1000.0f;
        var totalTime = delta.magnitude / speed;
        var ellapsedTime = 0.0f;

        while (ellapsedTime < totalTime)
        {
            ellapsedTime += Time.deltaTime;
            var k = ellapsedTime / totalTime;
            transform.position = Vector3.Lerp(startPosition, endPosition, k);

            yield return null;
        }

        backButton.enabled = true;
        hudAnimator.enabled = true;
        coroutine = null;
    }

    IEnumerator Coroutine_Disappear()
    {
        backButton.enabled = false;

        float elapsedTime = 0.0f;
        while(elapsedTime < 1.0f)
        {
            elapsedTime += Time.deltaTime;

            var color = image.color;
            color.a = 1.0f - elapsedTime;
            image.color = color;

            yield return null;
        }

        transform.position = prevPosition;

        {
            var color = image.color;
            color.a = 1.0f;
            image.color = color;
        }

        backButton.enabled = true;
        hudAnimator.enabled = true;
        coroutine = null;
    }
}
