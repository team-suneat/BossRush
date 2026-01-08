using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    /// <summary> 하나의 펄스 바를 관리하는 클래스입니다. </summary>
    public class UIPulseBar : XBehaviour
    {
        [FoldoutGroup("#UIPulseBar")]
        [SerializeField] private Image _grayImage;

        [FoldoutGroup("#UIPulseBar")]
        [SerializeField] private Image _fillImage;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _grayImage ??= this.FindComponent<Image>("Pulse Gray Image");
            _fillImage ??= this.FindComponent<Image>("Pulse Fill Image");
        }

        /// <summary> 그레이 이미지와 일반 이미지의 fillAmount를 설정합니다. </summary>
        /// <param name="grayAmount">그레이 이미지 fillAmount (0~1)</param>
        /// <param name="fillAmount">일반 이미지 fillAmount (0~1)</param>
        public void SetFillAmount(float grayAmount, float fillAmount)
        {
            if (_grayImage != null)
            {
                _grayImage.fillAmount = Mathf.Clamp01(grayAmount);
            }

            if (_fillImage != null)
            {
                _fillImage.fillAmount = Mathf.Clamp01(fillAmount);
            }
        }
    }
}

