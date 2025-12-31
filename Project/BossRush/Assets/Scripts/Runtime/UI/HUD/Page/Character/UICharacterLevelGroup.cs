using Sirenix.OdinInspector;
using TeamSuneat.Data;
using TeamSuneat.Data.Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    // 캐릭터 정보 영역 - 아이콘, 레벨, 경험치, 레벨업 버튼
    public class UICharacterLevelGroup : XBehaviour
    {
        [Title("#UICharacterLevelGroup")]
        [SerializeField] private Image _characterIconImage;
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private TextMeshProUGUI _expText;
        [SerializeField] private UIGauge _experienceGauge;
        [SerializeField] private UICharacterLevelUpButton _levelUpButton;

        private void Awake()
        {
            AutoGetComponents();
        }

        protected override void OnStart()
        {
            base.OnStart();
            RegisterEvents();
            Refresh();
        }

        protected override void OnRelease()
        {
            base.OnRelease();
            UnregisterEvents();
        }

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _characterIconImage ??= this.FindComponent<Image>("Character Icon Image");
            _levelText ??= this.FindComponent<TextMeshProUGUI>("Level Text");
            _expText ??= this.FindComponent<TextMeshProUGUI>("Exp Text");
            _experienceGauge ??= GetComponentInChildren<UIGauge>();
            _levelUpButton ??= GetComponentInChildren<UICharacterLevelUpButton>();
        }

        protected override void RegisterGlobalEvent()
        {
            base.RegisterGlobalEvent();
            GlobalEvent.Register(GlobalEventType.GAME_DATA_CHARACTER_ADD_EXPERIENCE, Refresh);
        }

        protected override void UnregisterGlobalEvent()
        {
            base.UnregisterGlobalEvent();
            GlobalEvent.Unregister(GlobalEventType.GAME_DATA_CHARACTER_ADD_EXPERIENCE, Refresh);
        }

        private void RegisterEvents()
        {
            if (_levelUpButton != null)
            {
                _levelUpButton.OnLevelUpSuccess.AddListener(Refresh);
                _levelUpButton.Refresh();
            }
        }

        private void UnregisterEvents()
        {
            if (_levelUpButton != null)
            {
                _levelUpButton.OnLevelUpSuccess.RemoveListener(Refresh);
            }
        }

        public void Refresh()
        {
            VProfile profileInfo = GameApp.GetSelectedProfile();

            int level = profileInfo.Level.Level;
            int currentEXP = profileInfo.Level.Experience;
            int maxEXP = profileInfo.Level.GetRequiredExperience();
            float expRatio = currentEXP.SafeDivide01(maxEXP);

            RefreshGauge(currentEXP, maxEXP, expRatio);
            RefreshLevelText(level);
            RefreshExperienceText(expRatio);
            _levelUpButton?.Refresh();
        }

        public void RefreshGauge(int current, int max, float rate)
        {
            if (_experienceGauge != null)
            {
                _experienceGauge.SetValueText(current, max);
                _experienceGauge.SetFrontValue(rate);
            }
        }

        private void RefreshLevelText(int level)
        {
            if (_levelText == null)
            {
                return;
            }

            _levelText.SetText(level.GetLevelString());
        }

        private void RefreshExperienceText(float expRatio)
        {
            if (_expText == null)
            {
                return;
            }

            _expText.SetText(ValueStringEx.GetPercentString(expRatio));
        }

        public void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }
    }
}