using DG.Tweening;
using Sirenix.OdinInspector;
using TeamSuneat.Data;
using TeamSuneat.Data.Game;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    // 강화 아이템 - 스크롤 뷰 내 개별 강화 항목
    public class UIEnhancementItem : XBehaviour
    {
        [Title("#UIEnhancementItem")]
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private Image _frameImage;
        [SerializeField] private Image _statIconImage;
        [SerializeField] private UILocalizedText _statNameText;
        [SerializeField] private UILocalizedText _levelText;
        [SerializeField] private TextMeshProUGUI _statValueText;

        [Title("#LevelUp Button")]
        [SerializeField] private UIEnhancementButton _levelUpButton;

        private EnhancementConfigData _enhancementData;

        public UnityEvent OnLevelUpSuccess;

        private void Awake()
        {
            AutoGetComponents();
            if (_statValueText != null)
            {
                _originalScale = _statValueText.transform.localScale;
            }
        }

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _backgroundImage ??= this.FindComponent<Image>("Background Image");
            _frameImage ??= this.FindComponent<Image>("Frame Image");
            _statIconImage ??= this.FindComponent<Image>("Stat Icon Image");
            _statNameText ??= this.FindComponent<UILocalizedText>("Stat Name Text");
            _levelText ??= this.FindComponent<UILocalizedText>("Level Text");
            _statValueText ??= this.FindComponent<TextMeshProUGUI>("StatValue Text");

            Transform levelUpButtonTransform = this.FindTransform("LevelUp Button");
            if (levelUpButtonTransform != null)
            {
                _levelUpButton ??= levelUpButtonTransform.GetComponent<UIEnhancementButton>();
            }
        }

        public void Setup(EnhancementConfigData data)
        {
            _enhancementData = data;

            if (_levelUpButton != null)
            {
                _levelUpButton.Setup(data, this);
                _levelUpButton.OnLevelUpSuccess.AddListener(() => OnLevelUpSuccess?.Invoke());
            }

            Refresh();
        }

        public void Refresh()
        {
            if (_enhancementData == null)
            {
                return;
            }

            VProfile profile = GameApp.GetSelectedProfile();
            if (profile == null)
            {
                return;
            }

            int currentLevel = profile.Enhancement.GetLevel(_enhancementData.StatName);
            int cost = CalculateCost(currentLevel);

            RefreshStatName();
            RefreshLevel(currentLevel);
            RefreshStatValue(currentLevel);

            if (_levelUpButton != null)
            {
                _levelUpButton.Setup(CurrencyNames.Gold, cost);
            }
        }

        private void RefreshStatName()
        {
            if (_statNameText == null)
            {
                return;
            }

            string stringKey = _enhancementData.StatName.GetStringKey();
            _statNameText.SetStringKey(stringKey);
        }

        private void RefreshLevel(int currentLevel)
        {
            if (_levelText == null)
            {
                return;
            }

            _levelText.SetText(currentLevel.GetLevelString());
        }

        private void RefreshStatValue(int currentLevel)
        {
            if (_statValueText == null)
            {
                return;
            }

            float currentValue = _enhancementData.CalculateStatValue(currentLevel);
            float nextValue = _enhancementData.CalculateStatValue(currentLevel + 1);

            string currentContent = _enhancementData.StatName.GetStatValueString(currentValue);
            string nextContent = _enhancementData.StatName.GetStatValueString(nextValue);
            _statValueText.SetText($"{currentContent} → {nextContent}");

        }

        private int CalculateCost(int level)
        {
            if (level <= 0)
            {
                return _enhancementData.InitialCost;
            }

            // 비용 = 초기 비용 × 비용 성장률^(레벨-1)
            return Mathf.RoundToInt(_enhancementData.InitialCost * Mathf.Pow(_enhancementData.CostGrowthRate, level - 1));
        }

        #region Punch Scale

        public void StartPunchScale()
        {
            if (_statValueText == null)
            {
                return;
            }

            if (_scaleTween != null)
            {
                _scaleTween.Kill();
                _scaleTween = null;
            }

            _scaleTween = _statValueText.transform.DOPunchScale(_punchScale,
                DEFAULT_PUNCH_SCALE_DURATION, PUNCH_SCALE_VIBRATO, PUNCH_SCALE_ELASTICITY)
                .SetEase(Ease.OutQuad).OnComplete(OnCompletedPunchScale);
        }

        private void OnCompletedPunchScale()
        {
            _statValueText.transform.localScale = _originalScale;
            _scaleTween = null;
        }

        protected Tween _scaleTween;
        private const float DEFAULT_PUNCH_SCALE_DURATION = 0.1f;
        private const int PUNCH_SCALE_VIBRATO = 1;
        private const float PUNCH_SCALE_ELASTICITY = 0.5f;
        private const float DEFAULT_PUNCH_SCALE_VALUE = -0.1f;

        protected Vector3 _punchScale =
            new Vector3(DEFAULT_PUNCH_SCALE_VALUE, DEFAULT_PUNCH_SCALE_VALUE, DEFAULT_PUNCH_SCALE_VALUE);

        private Vector3 _originalScale;

        #endregion Punch Scale
    }
}