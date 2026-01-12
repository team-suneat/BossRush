using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    public class UIPulseBar : XBehaviour
    {
        [FoldoutGroup("#UIPulseBar")]
        [SerializeField] private Image _grayImage;

        [FoldoutGroup("#UIPulseBar")]
        [SerializeField] private Image _fillImage;

        public override void AutoGetComponents() {
            base.AutoGetComponents();

            _grayImage ??= this.FindComponent<Image>("Pulse Gray Image");
            _fillImage ??= this.FindComponent<Image>("Pulse Fill Image");
        }

        public void SetFillAmount(float grayAmount, float fillAmount) {
            if (_grayImage != null) {
                _grayImage.fillAmount = Mathf.Clamp01(grayAmount);
            }

            if (_fillImage != null) {
                _fillImage.fillAmount = Mathf.Clamp01(fillAmount);
            }
        }

        public void SetBurnoutState(bool isBurnout) {
            if (_grayImage != null) {
                _grayImage.gameObject.SetActive(isBurnout);
            }

            if (_fillImage != null) {
                _fillImage.gameObject.SetActive(!isBurnout);
            }
        }
    }
}

