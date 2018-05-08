using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonChooseOption : EventTrigger
{
    private Image           _icon;
    private GameObject      _tintMessage;
    private DropDownButton  _master;
    private int             _index;
    
    public Image Icon
    {
        get { return _icon; }
    }

    /*private void Awake()
    {
        _icon = transform.Find("icon").GetComponent<Image>();
        _tintMessage = transform.Find("tint").gameObject;       
    }*/

    public void Init(DropDownButton master, int index)
    {
        _icon = transform.Find("icon").GetComponent<Image>();
        _tintMessage = transform.Find("tint").gameObject;
        _master = master;
        _index = index;
    }
    
    public void ShowTint()
    {
        if(_tintMessage)
        {
            _tintMessage.SetActive(true);
        }
    }
    
    public void HideTint()
    {
        if (_tintMessage)
        {
            _tintMessage.SetActive(false);
        }
    }

    private void OnEnable()
    {
        HideTint();
    }

    private void OnDisable()
    {
        HideTint();
    }

    public override void OnPointerEnter(PointerEventData data)
    {
        Debug.Log("Option " + gameObject.name + ": OnPointerEnter");

        ShowTint();
    }

    public override void OnPointerExit(PointerEventData data)
    {
        Debug.Log("Option " + gameObject.name + ": OnPointerExit");

        HideTint();
    }

    public override void OnDrop(PointerEventData data)
    {
        Debug.Log("Option " + gameObject.name + ": OnDrop");

        _master.Event_OnOptionChoosed(_index, _icon);
    }
}
