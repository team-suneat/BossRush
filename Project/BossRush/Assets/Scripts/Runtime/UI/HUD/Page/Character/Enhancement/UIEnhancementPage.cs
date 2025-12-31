using Sirenix.OdinInspector;
using System.Collections.Generic;
using TeamSuneat.Data;
using UnityEngine;

namespace TeamSuneat.UserInterface
{
    // 강화 페이지 - 강화 아이템 스크롤 뷰 관리
    public class UIEnhancementPage : UIPage
    {
        [Title("#UIEnhancementPage")]
        [SerializeField] private UIEnhancementItem[] _items;

        private readonly Dictionary<StatNames, UIEnhancementItem> _enhancementItemMap = new();

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _items = GetComponentsInChildren<UIEnhancementItem>(true);
        }

        protected override void RegisterGlobalEvent()
        {
            base.RegisterGlobalEvent();
            GlobalEvent<CurrencyNames, int>.Register(GlobalEventType.CURRENCY_EARNED, OnCurrencyChanged);
            GlobalEvent<CurrencyNames, int>.Register(GlobalEventType.CURRENCY_PAYED, OnCurrencyChanged);
        }

        protected override void UnregisterGlobalEvent()
        {
            base.UnregisterGlobalEvent();
            GlobalEvent<CurrencyNames, int>.Unregister(GlobalEventType.CURRENCY_EARNED, OnCurrencyChanged);
            GlobalEvent<CurrencyNames, int>.Unregister(GlobalEventType.CURRENCY_PAYED, OnCurrencyChanged);
        }

        private void OnCurrencyChanged(CurrencyNames currencyName, int addAmount)
        {
            if (currencyName == CurrencyNames.Gold)
            {
                RefreshAllItems();
            }
        }

        public override void Initialize()
        {
            base.Initialize();

            if (_items == null || _items.Length == 0)
            {
                Log.Warning(LogTags.UI_Page, "UIEnhancementItem을 찾을 수 없습니다.");
                return;
            }

            SetupEnhancementItems();
            RefreshAllItems();
        }

        private void SetupEnhancementItems()
        {
            _enhancementItemMap.Clear();

            EnhancementConfigAsset asset = ScriptableDataManager.Instance?.GetEnhancementDataAsset();
            if (asset == null || asset.DataArray == null)
            {
                Log.Warning(LogTags.UI_Page, "강화 데이터 에셋을 찾을 수 없습니다.");
                return;
            }

            int itemIndex = 0;

            for (int i = 0; i < asset.DataArray.Length; i++)
            {
                EnhancementConfigData data = asset.DataArray[i];
                if (data == null || data.StatName == StatNames.None)
                {
                    continue;
                }

                if (itemIndex >= _items.Length)
                {
                    Log.Warning(LogTags.UI_Page, "강화 아이템 개수가 부족합니다. 필요한 개수: {0}, 현재 개수: {1}", asset.DataArray.Length, _items.Length);
                    break;
                }

                if (_items[itemIndex] != null)
                {
                    _items[itemIndex].Setup(data);
                    _items[itemIndex].OnLevelUpSuccess.AddListener(OnItemLevelUpSuccess);
                    _enhancementItemMap.Add(data.StatName, _items[itemIndex]);
                    itemIndex++;
                }
            }

            Log.Info(LogTags.UI_Page, "강화 아이템 {0}개 설정 완료", itemIndex);
        }

        public void RefreshAllItems()
        {
            if (_items == null)
            {
                return;
            }

            for (int i = 0; i < _items.Length; i++)
            {
                if (_items[i] != null)
                {
                    _items[i].Refresh();
                }
            }
        }

        private void OnItemLevelUpSuccess()
        {
            // 다른 아이템의 버튼 상태도 갱신 (요구 능력치 조건이 변경될 수 있음)
            RefreshAllItems();
        }

        public UIEnhancementItem FindItem(StatNames statName)
        {
            return _enhancementItemMap.TryGetValue(statName, out UIEnhancementItem item) ? item : null;
        }
    }
}