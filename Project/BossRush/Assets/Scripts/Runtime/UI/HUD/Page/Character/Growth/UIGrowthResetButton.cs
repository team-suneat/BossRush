using Sirenix.OdinInspector;
using TeamSuneat.Data;
using TeamSuneat.Data.Game;
using UnityEngine;
using UnityEngine.Events;

namespace TeamSuneat.UserInterface
{
    public class UIGrowthResetButton : UIButton
    {
        [FoldoutGroup("#UIButton-GrowthReset"), SerializeField]
        private UIGrowthPage _parentPage;

        [FoldoutGroup("#UIButton-GrowthReset"), SerializeField]
        public UnityEvent OnResetSuccess;

        private const int RESET_COST_DIAMOND = 3000;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();
            _parentPage = this.FindFirstParentComponent<UIGrowthPage>();
        }

        public void Refresh()
        {
            VProfile profile = GameApp.GetSelectedProfile();
            if (profile == null)
            {
                SetFrameImageColor(GameColors.IndianRed);
                return;
            }

            if (CanReset(profile))
            {
                SetFrameImageColor(GameColors.SteelBlue);
            }
            else
            {
                SetFrameImageColor(GameColors.IndianRed);
            }
        }

        private bool CanReset(VProfile profile)
        {
            if (profile == null)
            {
                return false;
            }

            // 다이아몬드 보유 여부 확인
            if (!profile.Currency.CanUse(CurrencyNames.Diamond, RESET_COST_DIAMOND))
            {
                return false;
            }

            // 리셋할 레벨이 있는지 확인
            int totalConsumed = profile.Growth.GetTotalConsumedStatPoints();
            if (totalConsumed <= 0)
            {
                return false;
            }

            return true;
        }

        protected override void OnButtonClick()
        {
            SpawnPurchasePopup();
        }

        private void SpawnPurchasePopup()
        {
            var popup = UIManager.Instance.PopupManager.SpawnCenterPopup(UIPopupNames.Purchase, OnDespawnPurchasePopup);
            if (popup != null)
            {
                string content = JsonDataManager.FindStringClone("Popup_Content_Growth_Reset");
                _purchasePopup = popup as UIPurchasePopup;
                _purchasePopup.Setup(content, CurrencyNames.Diamond, RESET_COST_DIAMOND);
            }
        }

        private UIPurchasePopup _purchasePopup = null;

        private void OnDespawnPurchasePopup(bool result)
        {
            if (result)
            {
                VProfile profile = GameApp.GetSelectedProfile();
                if (profile != null)
                {
                    // 소비한 능력치 포인트 계산
                    int totalConsumed = profile.Growth.GetTotalConsumedStatPoints();
                    if (totalConsumed <= 0)
                    {
                        return;
                    }

                    // 모든 성장 능력치 레벨 초기화 및 소비한 포인트 반환
                    int returnedPoints = profile.Growth.ResetGrowthLevels();

                    profile.Growth.AddStatPoint(returnedPoints);
                }
                Refresh();
                _parentPage?.Refresh();

                // 리셋 성공 이벤트 발생
                OnResetSuccess?.Invoke();
            }
        }
    }
}