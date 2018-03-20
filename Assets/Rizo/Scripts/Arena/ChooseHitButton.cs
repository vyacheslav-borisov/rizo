using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pegas.Rizo
{
    [RequireComponent(typeof(DropDownButton))]
    public class ChooseHitButton : MonoBehaviour
    {
        private void Awake()
        {
            var dropDownButton = GetComponent<DropDownButton>();
            dropDownButton.OnOptionChossed += Event_OnOptionChoosed;        
        }

        private void Event_OnOptionChoosed(int option)
        {
            switch(option)
            {
                case 0:
                    GameManager.Instance.Action_ChooseHit(HitType.Heavy);
                    break;
                case 1:
                    GameManager.Instance.Action_ChooseHit(HitType.Breakable);
                    break;
                case 2:
                    GameManager.Instance.Action_ChooseHit(HitType.Simple);
                    break;
            }
        }
    }
}
