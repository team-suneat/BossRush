using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    public partial class UIGauge : XBehaviour
    {
        [FoldoutGroup("#UI Gauge-Component")]
        public Slider FrontSlider;

        [FoldoutGroup("#UI Gauge-Component")]
        public Slider BackSlider;

        [FoldoutGroup("#UI Gauge-Component")]
        public TextMeshProUGUI ValueText;

        [FoldoutGroup("#UI Gauge-Toggle")]
        public bool UseFrontValueText;

        [ReadOnly] public float FrontValue;
        [ReadOnly] public float BackValue;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            Transform rect = this.FindTransform("Rect");
            if (rect == null)
            {
                rect = transform;
            }

            FrontSlider = rect.FindComponent<Slider>("Slider (Front)");
            BackSlider = rect.FindComponent<Slider>("Slider (Back)");

            if (ValueText == null)
            {
                ValueText = rect.FindComponent<TextMeshProUGUI>("Value Text");
            }
        }

        protected override void OnStart()
        {
            base.OnStart();

            ResetBackValue();
        }

        public void LogicUpdate()
        {
            DecreaseBackValue();
        }

        private void DecreaseBackValue()
        {
            if (BackSlider == null)
            {
                return;
            }

            if (FrontValue < BackValue)
            {
                float backValue = BackValue - (Time.deltaTime * 0.3f);
                SetBackValue(backValue);
            }
        }

        // 생명력 텍스트 (Health Text)
        public void ResetValueText()
        {
            if (ValueText != null)
            {
                ValueText.SetText(string.Empty);
            }
        }

        public virtual void SetValueText(string content)
        {
            if (ValueText != null)
            {
                if (UseFrontValueText)
                {
                    ValueText.SetText(content);
                }
                else
                {
                    ValueText.SetText(string.Empty);
                }
            }
        }

        public virtual void SetValueText(int current, int max)
        {
            if (ValueText != null)
            {
                if (UseFrontValueText)
                {
                    if (max > 0)
                    {
                        string content = ValueStringEx.GetNoDigitString(current, max);
                        ValueText.SetText(content);
                        LogProgress("게이지의 값 텍스트를 설정합니다. {0}", content);
                        return;
                    }
                }

                ValueText.SetText(string.Empty);
            }
        }

        // 생명력의 앞 레이어 게이지 설정 (Health Front Value)

        public void SetFrontValue(float currentValue)
        {
            float previousFrontValue = FrontValue;
            float newFrontValue = Mathf.Clamp01(currentValue);

            // FrontValue가 감소하면 이전 값을 BackValue로 설정
            if (BackSlider != null && newFrontValue < previousFrontValue)
            {
                SetBackValue(previousFrontValue);
            }

            FrontValue = newFrontValue;

            if (FrontSlider != null)
            {
                FrontSlider.value = FrontValue;
            }
        }

        public void ResetFrontValue()
        {
            SetFrontValue(0f);
        }

        public void SetFrontColor(Color color)
        {
            FrontSlider.targetGraphic.color = color;
        }

        // Back 게이지

        public void SetBackValue(float backFillAmount)
        {
            BackValue = Mathf.Clamp01(backFillAmount);

            if (BackSlider != null)
            {
                BackSlider.value = BackValue;
            }
        }

        public void ResetBackValue()
        {
            SetBackValue(0f);
        }
    }
}