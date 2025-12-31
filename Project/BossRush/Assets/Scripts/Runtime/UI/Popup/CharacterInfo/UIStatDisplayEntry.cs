using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat.UserInterface
{
    // 개별 능력치 항목 표시 컴포넌트
    public class UIStatDisplayEntry : XBehaviour
    {
        [Title("#UIStatDisplayEntry")]
        [SerializeField] private UILocalizedText _nameText;
        [SerializeField] private UILocalizedText _valueText;

        private StatNames _statName;

        public StatNames StatName => _statName;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _nameText ??= this.FindComponent<UILocalizedText>("Name Text");
            _valueText ??= this.FindComponent<UILocalizedText>("Value Text");
        }

        public void SetStatName(StatNames statName)
        {
            _statName = statName;

            if (_nameText != null)
            {
                string localizedName = statName.GetLocalizedString();
                _nameText.SetText(localizedName);
            }
        }

        public void SetValue(float value)
        {
            if (_valueText != null)
            {
                string formattedValue = _statName.GetStatValueString(value);
                _valueText.SetText(formattedValue);
            }
        }

        public void SetData(StatNames statName, float value)
        {
            SetStatName(statName);
            SetValue(value);
        }

        public void Clear()
        {
            _statName = StatNames.None;

            if (_nameText != null)
            {
                _nameText.SetText(string.Empty);
            }

            if (_valueText != null)
            {
                _valueText.SetText(string.Empty);
            }
        }
    }
}