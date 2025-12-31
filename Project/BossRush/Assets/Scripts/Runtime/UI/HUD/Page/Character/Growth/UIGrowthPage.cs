using Sirenix.OdinInspector;
using System.Collections.Generic;
using TeamSuneat.Data;
using TeamSuneat.Data.Game;
using UnityEngine;

namespace TeamSuneat.UserInterface
{
    // 성장 페이지 - 능력치 포인트로 구매하는 성장 능력치 관리
    public class UIGrowthPage : UIPage
    {
        [Title("#UIGrowthPage")]
        [SerializeField] private UIGrowthItem[] _items;
        [SerializeField] private UILocalizedText _statPointText;

        private readonly Dictionary<StatNames, UIGrowthItem> _growthItemMap = new();

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _items = GetComponentsInChildren<UIGrowthItem>(true);
        }

        protected override void RegisterGlobalEvent()
        {
            base.RegisterGlobalEvent();

            GlobalEvent<int>.Register(GlobalEventType.GAME_DATA_CHARACTER_LEVEL_CHANGED, OnPlayerCharacterLevelChange);

            GlobalEvent<CharacterGrowthTypes, int>.Register(GlobalEventType.GAME_DATA_CHARACTER_GROWTH_LEVEL_CHANGED, OnGrowthLevelChanged);
            GlobalEvent<int>.Register(GlobalEventType.GAME_DATA_CHARACTER_GROWTH_STAT_POINT_CHANGED, OnStatPointChanged);
        }

        protected override void UnregisterGlobalEvent()
        {
            base.UnregisterGlobalEvent();

            GlobalEvent<int>.Unregister(GlobalEventType.GAME_DATA_CHARACTER_LEVEL_CHANGED, OnPlayerCharacterLevelChange);
            GlobalEvent<CharacterGrowthTypes, int>.Register(GlobalEventType.GAME_DATA_CHARACTER_GROWTH_LEVEL_CHANGED, OnGrowthLevelChanged);
            GlobalEvent<int>.Register(GlobalEventType.GAME_DATA_CHARACTER_GROWTH_STAT_POINT_CHANGED, OnStatPointChanged);
        }

        private void OnPlayerCharacterLevelChange(int level)
        {
            RefreshAllItems();
        }

        private void OnGrowthLevelChanged(CharacterGrowthTypes growthType, int level)
        {
            if (growthType == CharacterGrowthTypes.None)
            {
                return;
            }

            Refresh();
        }

        private void OnStatPointChanged(int statPoint)
        {
            RefreshStatPoint();
        }

        public override void Initialize()
        {
            base.Initialize();

            if (_items == null || _items.Length == 0)
            {
                Log.Warning(LogTags.UI_Page, "UIGrowthItem을 찾을 수 없습니다.");
                return;
            }

            SetupGrowthItems();
            RefreshAllItems();
        }

        protected override void OnShow()
        {
            base.OnShow();

            Refresh();
        }

        private void SetupGrowthItems()
        {
            _growthItemMap.Clear();

            GrowthConfigAsset asset = ScriptableDataManager.Instance?.GetGrowthDataAsset();
            if (asset == null || asset.DataArray == null)
            {
                Log.Warning(LogTags.UI_Page, "성장 데이터 에셋을 찾을 수 없습니다.");
                return;
            }

            int itemIndex = 0;

            for (int i = 0; i < asset.DataArray.Length; i++)
            {
                GrowthConfigData data = asset.DataArray[i];
                if (data == null || data.StatName == StatNames.None)
                {
                    continue;
                }

                if (itemIndex >= _items.Length)
                {
                    Log.Warning(LogTags.UI_Page, "성장 아이템 개수가 부족합니다. 필요한 개수: {0}, 현재 개수: {1}", asset.DataArray.Length, _items.Length);
                    break;
                }

                if (_items[itemIndex] != null)
                {
                    _items[itemIndex].Setup(data);
                    _items[itemIndex].OnLevelUpSuccess.AddListener(OnItemLevelUpSuccess);
                    _growthItemMap.Add(data.StatName, _items[itemIndex]);
                    itemIndex++;
                }
            }

            Log.Info(LogTags.UI_Page, "성장 아이템 {0}개 설정 완료", itemIndex);
        }

        public void Refresh()
        {
            RefreshStatPoint();
            RefreshAllItems();
        }

        private void RefreshStatPoint()
        {
            if (_statPointText == null)
            {
                return;
            }

            VProfile profile = GameApp.GetSelectedProfile();
            if (profile == null)
            {
                return;
            }

            StringData stringData = JsonDataManager.FindStringData("Format_StatPoint");
            int statPoint = profile.Growth.StatPoint;
            string content = StringGetter.Format(stringData, statPoint.ToString());
            _statPointText.SetText(content);
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
            // 능력치 포인트 표시 갱신
            RefreshStatPoint();
            // 다른 아이템의 버튼 상태도 갱신
            RefreshAllItems();
        }

        public UIGrowthItem FindItem(StatNames statName)
        {
            return _growthItemMap.TryGetValue(statName, out UIGrowthItem item) ? item : null;
        }
    }
}