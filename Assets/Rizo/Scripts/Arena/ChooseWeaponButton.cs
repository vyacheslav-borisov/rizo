using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pegas.Rizo
{
    [RequireComponent(typeof(DropDownButton))]
    public class ChooseWeaponButton : MonoBehaviour
    {
        private void Awake()
        {
            var dropDownButton = GetComponent<DropDownButton>();
            dropDownButton.OnOptionChossed += Event_OnOptionChoosed;
        }

        private void Event_OnOptionChoosed(int option)
        {
            switch (option)
            {
                case 0:
                    GameManager.Instance.Action_ChooseSpearType(SpearType.Light);
                    break;
                case 1:
                    GameManager.Instance.Action_ChooseSpearType(SpearType.Heavy);
                    break;
            }
        }
    }
}
