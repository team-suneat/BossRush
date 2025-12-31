using Sirenix.OdinInspector;
using TeamSuneat.Data;
using TeamSuneat.Data.Game;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    // 강화 레벨업 버튼 컴포넌트
    public class UIEnhancementButton : UIPurchaseButton
    {
        private const CurrencyNames ENHANCEMENT_COST_CURRENCY = CurrencyNames.Gold;

        [FoldoutGroup("#UIButton-Enhancement"), SerializeField]
        public UnityEvent OnLevelUpSuccess;

        private EnhancementConfigData _data;
        private UIEnhancementItem _parentItem;

        public void Setup(EnhancementConfigData data, UIEnhancementItem parentItem)
        {
            _data = data;
            _parentItem = parentItem;
            base.Setup(ENHANCEMENT_COST_CURRENCY, 0);
            Refresh();
        }

        public override void Refresh()
        {
            if (_data == null)
            {
                return;
            }

            VProfile profile = GameApp.GetSelectedProfile();
            if (profile == null)
            {
                return;
            }

            int currentLevel = profile.Enhancement.GetLevel(_data.StatName);

            base.Refresh();

            RefreshLevelUpButton(profile, currentLevel);
        }

        protected override bool CheckClickable()
        {
            if (!base.CheckClickable())
            {
                return false;
            }

            if (_data == null)
            {
                return false;
            }

            VProfile profile = GameApp.GetSelectedProfile();
            if (profile == null)
            {
                return false;
            }

            int currentLevel = profile.Enhancement.GetLevel(_data.StatName);
            return CanLevelUp(profile, currentLevel);
        }

        protected override void OnClickSucceeded()
        {
            base.OnClickSucceeded();

            if (_data == null)
            {
                return;
            }

            VProfile profile = GameApp.GetSelectedProfile();
            if (profile == null)
            {
                return;
            }

            int currentLevel = profile.Enhancement.GetLevel(_data.StatName);
            profile.Enhancement.AddLevel(_data.StatName);

            int newLevel = profile.Enhancement.GetLevel(_data.StatName);
            int cost = CalculateCost(currentLevel);

            Log.Info(LogTags.UI_Page, "{0} 강화 레벨업! Lv.{1} → Lv.{2}, 비용: {3}",
                _data.StatName, currentLevel, newLevel, cost);

            Refresh();

            if (_parentItem != null)
            {
                _parentItem.Refresh();
                _parentItem.StartPunchScale();
            }

            OnLevelUpSuccess?.Invoke();
        }

        protected override void OnHoldSucceeded()
        {
            OnClickSucceeded();
        }

        private void RefreshLevelUpButton(VProfile profile, int currentLevel)
        {
            bool canLevelUp = CanLevelUp(profile, currentLevel);
            if (canLevelUp)
            {
                canLevelUp = CanPurchase(profile);
            }

            Color buttonColor = canLevelUp ? GameColors.MediumAquamarine : GameColors.IndianRed;
            SetFrameImageColor(buttonColor);
        }

        private int CalculateCost(int level)
        {
            if (level <= 0)
            {
                return _data.InitialCost;
            }

            // 비용 = 초기 비용 × 비용 성장률^(레벨-1)
            return Mathf.RoundToInt(_data.InitialCost * Mathf.Pow(_data.CostGrowthRate, level - 1));
        }

        private bool CanLevelUp(VProfile profile, int currentLevel)
        {
            if (_data == null)
            {
                return false;
            }

            if (currentLevel >= _data.MaxLevel)
            {
                return false;
            }

            if (_data.HasRequirement)
            {
                int requiredLevel = profile.Enhancement.GetLevel(_data.RequiredStatName);
                if (requiredLevel < _data.RequiredStatLevel)
                {
                    return false;
                }
            }

            return true;
        }
    }
}