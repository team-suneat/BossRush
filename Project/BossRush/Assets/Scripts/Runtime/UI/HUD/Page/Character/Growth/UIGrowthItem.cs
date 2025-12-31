using DG.Tweening;
using Sirenix.OdinInspector;
using TeamSuneat.Data;
using TeamSuneat.Data.Game;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    // 성장 아이템 - 스크롤 뷰 내 개별 성장 항목
    public class UIGrowthItem : XBehaviour
    {
        [Title("#UIGrowthItem")]
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private Image _frameImage;
        [SerializeField] private Image _statIconImage;
        [SerializeField] private UILocalizedText _typeText;
        [SerializeField] private UILocalizedText _maxLevelText;
        [SerializeField] private UILocalizedText _levelText;
        [SerializeField] private UILocalizedText _statText;

        [Title("#LevelUp Button")]
        [SerializeField] private UIGrowthButton _levelUpButton;

        private GrowthConfigData _growthData;

        public UnityEvent OnLevelUpSuccess;

        private void Awake()
        {
            AutoGetComponents();

            if (_statText != null)
            {
                _originalScale = _statText.transform.localScale;
            }
        }

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _backgroundImage ??= this.FindComponent<Image>("Background Image");
            _frameImage ??= this.FindComponent<Image>("Top/Frame Image");
            _statIconImage ??= this.FindComponent<Image>("Top/Growth Icon Image");

            _typeText ??= this.FindComponent<UILocalizedText>("Top/Growth Type Text");
            _maxLevelText ??= this.FindComponent<UILocalizedText>("Top/Growth MaxLevel Text");

            _levelText ??= this.FindComponent<UILocalizedText>("Growth Level Text");
            _statText ??= this.FindComponent<UILocalizedText>("Growth Stat Text");

            Transform levelUpButtonTransform = this.FindTransform("LevelUp Button");
            if (levelUpButtonTransform != null)
            {
                _levelUpButton ??= levelUpButtonTransform.GetComponent<UIGrowthButton>();
            }
        }

        public void Setup(GrowthConfigData data)
        {
            _growthData = data;

            if (_levelUpButton != null)
            {
                _levelUpButton.Setup(data, this);
                _levelUpButton.OnLevelUpSuccess.AddListener(() => OnLevelUpSuccess?.Invoke());
            }

            Refresh();
        }

        public void Refresh()
        {
            if (_growthData == null)
            {
                return;
            }

            VProfile profile = GameApp.GetSelectedProfile();
            if (profile == null)
            {
                return;
            }

            int currentLevel = profile.Growth.GetLevel(_growthData.GrowthType);

            RefreshStatName();
            RefreshLevel(currentLevel);
            RefreshMaxLevel();
            RefreshStatValue(currentLevel);

            if (_levelUpButton != null)
            {
                _levelUpButton.Refresh();
            }
        }

        private void RefreshStatName()
        {
            if (_typeText == null)
            {
                return;
            }

            string stringKey = _growthData.GrowthType.GetStringKey();
            _typeText.SetStringKey(stringKey);
        }

        private void RefreshLevel(int currentLevel)
        {
            if (_levelText == null)
            {
                return;
            }

            _levelText.SetText(currentLevel.GetLevelString());
        }

        private void RefreshMaxLevel()
        {
            if (_maxLevelText == null)
            {
                return;
            }

            _maxLevelText.SetText(_growthData.MaxLevel.GetMaxLevelString());
        }

        private void RefreshStatValue(int currentLevel)
        {
            if (_statText == null)
            {
                return;
            }

            float currentValue = _growthData.CalculateStatValue(currentLevel);
            float nextValue = _growthData.CalculateStatValue(currentLevel + 1);

            string nameContent = _growthData.StatName.GetLocalizedString();
            string currentContent = _growthData.StatName.GetStatValueString(currentValue, true);
            string nextContent = _growthData.StatName.GetStatValueString(nextValue, true);
            _statText.SetText($"{nameContent} {currentContent} → {nextContent}");
        }

        #region Punch Scale

        public void StartPunchScale()
        {
            if (_statText == null)
            {
                return;
            }

            _scaleTween?.Kill();
            _scaleTween = null;

            _scaleTween = _statText.transform.DOPunchScale(_punchScale,
                DEFAULT_PUNCH_SCALE_DURATION, PUNCH_SCALE_VIBRATO, PUNCH_SCALE_ELASTICITY)

                .SetEase(Ease.OutQuad).OnComplete(OnCompletedPunchScale);
        }

        private void OnCompletedPunchScale()
        {
            _statText.transform.localScale = _originalScale;
            _scaleTween = null;
        }

        protected Tween _scaleTween;
        private const float DEFAULT_PUNCH_SCALE_DURATION = 0.1f;
        private const int PUNCH_SCALE_VIBRATO = 1;
        private const float PUNCH_SCALE_ELASTICITY = 0.5f;
        private const float DEFAULT_PUNCH_SCALE_VALUE = -0.1f;

        protected Vector3 _punchScale =
            new(DEFAULT_PUNCH_SCALE_VALUE, DEFAULT_PUNCH_SCALE_VALUE, DEFAULT_PUNCH_SCALE_VALUE);

        private Vector3 _originalScale;

        #endregion Punch Scale
    }
}