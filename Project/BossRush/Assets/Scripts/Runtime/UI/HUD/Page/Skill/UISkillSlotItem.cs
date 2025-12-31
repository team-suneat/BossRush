using Sirenix.OdinInspector;
using TeamSuneat.Data;
using TeamSuneat.Data.Game;
using UnityEngine;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    // 스킬 슬롯 아이템 - 스킬 페이지 내 개별 스킬 카드 표시
    public class UISkillSlotItem : XBehaviour
    {
        [FoldoutGroup("#UISkillSlotItem-Image")]
        [SerializeField] private Image _skillIconImage;

        [FoldoutGroup("#UISkillSlotItem-Image")]
        [SerializeField] private Image _skillFrameImage;

        [FoldoutGroup("#UISkillSlotItem-Image")]
        [SerializeField] private GameObject _unknownImage;

        [FoldoutGroup("#UISkillSlotItem-Text")]
        [SerializeField] private UILocalizedText _skillNameText;

        [FoldoutGroup("#UISkillSlotItem-Text")]
        [SerializeField] private UILocalizedText _equippedText;

        [FoldoutGroup("#UISkillSlotItem-Gauge")]
        [SerializeField] private UIGauge _skillCountGauge;

        private SkillNames _skillName = SkillNames.None;
        private SkillAsset _skillAsset;

        private enum SkillSlotState
        {
            Unknown,        // 배우지 않음
            Learned,        // 배우고 할당 안 함
            Equipped        // 배우고 할당함
        }

        public SkillNames SkillName => _skillName;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _skillIconImage ??= this.FindComponent<Image>("Skill Icon Image");
            _skillFrameImage ??= this.FindComponent<Image>("Skill Frame Image");
            _skillNameText ??= this.FindComponent<UILocalizedText>("Skill Name Text");
            _skillCountGauge ??= this.FindComponent<UIGauge>("Skill Count Gauge");
            _equippedText ??= this.FindComponent<UILocalizedText>("Skill Equipped Text");
            _unknownImage ??= this.FindGameObject("Unknown Image");
        }

        public override void AutoNaming()
        {
            if (_skillName != SkillNames.None)
            {
                SetGameObjectName($"UISkillSlotItem({_skillName})");
            }
            else
            {
                SetGameObjectName("UISkillSlotItem");
            }
        }

        public void Setup(SkillNames skillName)
        {
            _skillName = skillName;
            _skillAsset = ScriptableDataManager.Instance?.FindSkill(skillName);

            Refresh();
        }

        public void Refresh()
        {
            SkillSlotState state = GetSkillSlotState();
            RefreshSkillState(state);
            RefreshSkillIcon(state);
            RefreshSkillFrame();
            RefreshSkillName(state);

            if (state != SkillSlotState.Unknown)
            {
                RefreshSkillCount();
            }
            else
            {
                ClearSkillCount();
            }
        }

        private SkillSlotState GetSkillSlotState()
        {
            VProfile profile = GameApp.GetSelectedProfile();
            if (profile == null)
            {
                return SkillSlotState.Unknown;
            }

            VCharacterSkill characterSkill = profile.Skill;
            if (characterSkill == null)
            {
                return SkillSlotState.Unknown;
            }

            bool hasSkill = characterSkill.HasSkill(_skillName);
            if (!hasSkill)
            {
                return SkillSlotState.Unknown;
            }

            bool isEquipped = characterSkill.ContainsSkillSlot(_skillName);
            return isEquipped ? SkillSlotState.Equipped : SkillSlotState.Learned;
        }

        private void RefreshSkillState(SkillSlotState state)
        {
            if (_skillIconImage != null)
            {
                _skillIconImage.SetGreyScale(state == SkillSlotState.Equipped);
            }

            if (_equippedText != null)
            {
                if (state == SkillSlotState.Equipped)
                {
                    _equippedText.SetText("E");
                    _equippedText.gameObject.SetActive(true);
                }
                else
                {
                    _equippedText.gameObject.SetActive(false);
                }
            }

            if (_unknownImage != null)
            {
                _unknownImage.SetActive(state == SkillSlotState.Unknown);
            }
        }

        private void RefreshSkillIcon(SkillSlotState state)
        {
            if (_skillIconImage == null)
            {
                return;
            }

            if (state == SkillSlotState.Unknown)
            {
                _skillIconImage.enabled = false;
                return;
            }

            Sprite icon = _skillAsset?.Data?.Icon;
            if (icon != null)
            {
                _skillIconImage.sprite = icon;
                _skillIconImage.enabled = true;
            }
            else
            {
                _skillIconImage.enabled = false;
            }
        }

        private void RefreshSkillFrame()
        {
            if (_skillFrameImage == null)
            {
                return;
            }

            if (_skillAsset?.Data?.Grade != null)
            {
                Color color = _skillAsset.Data.Grade.GetGradeColor();
                _skillFrameImage.SetColor(color);
            }
        }

        private void RefreshSkillName(SkillSlotState state)
        {
            if (_skillNameText == null)
            {
                return;
            }

            if (state == SkillSlotState.Unknown)
            {
                _skillNameText.ResetText();
                return;
            }

            if (_skillName != SkillNames.None)
            {
                _skillNameText.SetText(_skillName.GetLocalizedString());
            }
            else
            {
                _skillNameText.ResetText();
            }
        }

        private void RefreshSkillCount()
        {
            VProfile profile = GameApp.GetSelectedProfile();
            if (profile == null)
            {
                ClearSkillCount();
                return;
            }

            VCharacterSkill characterSkill = profile.Skill;
            if (characterSkill == null)
            {
                ClearSkillCount();
                return;
            }

            int currentCount = GetCurrentSkillCardCount(characterSkill);
            int requiredCount = GetRequiredSkillCardCount();
            RefreshSkillCountGauge(currentCount, requiredCount);
        }

        private int GetCurrentSkillCardCount(VCharacterSkill characterSkill)
        {
            if (characterSkill.ObtainedCards == null)
            {
                return 0;
            }

            string skillNameString = _skillName.ToString();
            int count = 0;

            for (int i = 0; i < characterSkill.ObtainedCards.Count; i++)
            {
                if (characterSkill.ObtainedCards[i] == skillNameString)
                {
                    count++;
                }
            }

            return count;
        }

        private int GetRequiredSkillCardCount()
        {
            VProfile profile = GameApp.GetSelectedProfile();
            if (profile == null)
            {
                return 0;
            }

            VCharacterSkill characterSkill = profile.Skill;
            if (characterSkill == null)
            {
                return 0;
            }

            VSkill skill = characterSkill.FindSkill(_skillName);
            if (skill == null)
            {
                return 0;
            }

            int currentLevel = skill.Level;
            if (_skillAsset?.Data?.LevelUpCosts == null)
            {
                return 0;
            }

            if (currentLevel >= _skillAsset.Data.MaxLevel)
            {
                return 0;
            }

            int nextLevel = currentLevel + 1;
            if (nextLevel > 0 && nextLevel <= _skillAsset.Data.LevelUpCosts.Length)
            {
                return _skillAsset.Data.LevelUpCosts[nextLevel - 1];
            }

            return 0;
        }

        private void RefreshSkillCountGauge(int currentCount, int requiredCount)
        {
            if (_skillCountGauge == null)
            {
                return;
            }

            if (requiredCount <= 0)
            {
                _skillCountGauge.SetFrontValue(0);
                _skillCountGauge.SetValueText($"0 / {requiredCount}");
            }
            else
            {
                float fillAmount = Mathf.Clamp01((float)currentCount / requiredCount);
                _skillCountGauge.SetFrontValue(fillAmount);
                _skillCountGauge.SetValueText($"{currentCount} / {requiredCount}");
            }
        }

        private void ClearSkillCount()
        {
            if (_skillCountGauge != null)
            {
                _skillCountGauge.ResetFrontValue();
                _skillCountGauge.ResetValueText();
            }
        }
    }
}