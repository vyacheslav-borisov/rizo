using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pegas.Rizo
{
    [RequireComponent(typeof(DropDownButton))]
    public class ChooseHitButton : MonoBehaviour
    {
        public  BodyPartSelector bpSelector;
        private DropDownButton dropDownButton;

        private void Awake()
        {
            dropDownButton = GetComponent<DropDownButton>();
            dropDownButton.OnOptionChossed += Event_OnOptionChoosed;

            if (bpSelector != null)
            {
                bpSelector.OnChoosedAttackedBodyPart += Event_OnBPChossed;
                bpSelector.OnChoosedDefencedBodyPart += Event_OnShieldDropped;
            }
        }

        private void Event_OnBPChossed(BodyPartType bodyPartType, HitType hitType)
        {
            GameManager.Instance.Action_ChooseHit(hitType);
            GameManager.Instance.Action_ChooseAttackedBodyPart(bodyPartType);

            switch(hitType)
            {
                case HitType.Simple:
                    dropDownButton.SetOptionChoosed(2);
                    break;
                case HitType.Heavy:
                    dropDownButton.SetOptionChoosed(0);
                    break;
                case HitType.Breakable:
                    dropDownButton.SetOptionChoosed(1);
                    break;
                default:
                    break;
            }
        }

        private void Event_OnShieldDropped(BodyPartType bodyPartType)
        {
            GameManager.Instance.Action_ChooseDefendedBodyPart(bodyPartType);
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
