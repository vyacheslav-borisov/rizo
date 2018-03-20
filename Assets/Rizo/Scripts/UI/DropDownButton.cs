using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class DropDownButton : EventTrigger
{
    public delegate void OptionChossedEvent(int option);
    public OptionChossedEvent OnOptionChossed;

    private Image _icon;
    private GameObject _optionLayer;
    private ButtonChooseOption[] _options;

    private void Awake()
    {
        _icon = transform.Find("icon").GetComponent<Image>();

        _options = gameObject.GetComponentsInChildren<ButtonChooseOption>();
        if(_options != null && _options.Length > 0)
        {
            _optionLayer = _options[0].transform.parent.gameObject;
        }

        int index = 0;
        foreach(var option in _options)
        {
            option.Init(this, index++);            
        }

        _optionLayer.SetActive(false);
    }

    public void Event_OnOptionChoosed(int index, Image icon)
    {
        _icon.sprite = icon.sprite;

        if(OnOptionChossed != null)
        {
            OnOptionChossed.Invoke(index);
        }
    }

    public override void OnBeginDrag(PointerEventData data)
    {
        _optionLayer.SetActive(true);
    }

    public override void OnEndDrag(PointerEventData data)
    {
        foreach (var option in _options)
        {
            option.HideTint();
        }

        _optionLayer.SetActive(false);
    }
}
