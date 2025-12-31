using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using TeamSuneat;
using TeamSuneat.Data;
using UnityEngine;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    // 스킬 페이지 - 모든 스킬 카드 목록 표시
    public class UISkillPage : UIPage
    {
        [Title("#UISkillPage")]
        [SerializeField] private UISkillSlotItem[] _items;
        private readonly Dictionary<SkillNames, UISkillSlotItem> _skillItemMap = new();

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _items = GetComponentsInChildren<UISkillSlotItem>(true);
        }

        public override void Initialize()
        {
            base.Initialize();

            if (_items == null || _items.Length == 0)
            {
                Log.Warning(LogTags.UI_Page, "UISkillSlotItem을 찾을 수 없습니다.");
                return;
            }

            SetupSkillItems();
            RefreshAllItems();
        }

        protected override void OnShow()
        {
            base.OnShow();

            Refresh();
        }

        private void SetupSkillItems()
        {
            _skillItemMap.Clear();

            SkillNames[] skillNames = EnumEx.GetValues<SkillNames>(true);
            List<SkillNames> validSkillNames = new List<SkillNames>();

            // None 제외한 유효한 스킬 이름 수집
            for (int i = 0; i < skillNames.Length; i++)
            {
                if (skillNames[i] != SkillNames.None)
                {
                    validSkillNames.Add(skillNames[i]);
                }
            }

            int itemIndex = 0;

            for (int i = 0; i < validSkillNames.Count; i++)
            {
                SkillNames skillName = validSkillNames[i];

                if (itemIndex >= _items.Length)
                {
                    Log.Warning(LogTags.UI_Page, "스킬 아이템 개수가 부족합니다. 필요한 개수: {0}, 현재 개수: {1}", validSkillNames.Count, _items.Length);
                    break;
                }

                if (_items[itemIndex] != null)
                {
                    _items[itemIndex].Setup(skillName);
                    _skillItemMap.Add(skillName, _items[itemIndex]);
                    itemIndex++;
                }
            }

            Log.Info(LogTags.UI_Page, "스킬 아이템 {0}개 설정 완료", itemIndex);
        }

        public void Refresh()
        {
            RefreshAllItems();
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

        public UISkillSlotItem FindItem(SkillNames skillName)
        {
            return _skillItemMap.TryGetValue(skillName, out UISkillSlotItem item) ? item : null;
        }
    }
}