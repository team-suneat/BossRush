using System;
using System.Collections.Generic;

namespace TeamSuneat.Data.Game
{
    [System.Serializable]
    public class VCharacterSkill
    {
        public Dictionary<string, VSkill> Skills = new(); // 기술 목록
        public List<VSkillSlot> Slots = new(); // 기술 슬롯 목록
        public List<string> UnlockedCards = new(); // 해금된 기술 카드 목록
        public List<string> ObtainedCards = new(); // 획득한 기술 카드 목록
        public bool IsAutoUse; // 자동 기술 사용

        [NonSerialized]
        private readonly Dictionary<SkillNames, VSkill> _skillMap = new();

        [NonSerialized]
        private readonly List<SkillNames> _slotSkillNames = new();

        public IReadOnlyList<SkillNames> SlotSkillNames => _slotSkillNames;

        public void OnLoadGameData()
        {
            LoadSkillMap();
            LoadSlotSkillNames();
        }

        private void LoadSkillMap()
        {
            _skillMap.Clear();

            if (!Skills.IsValid())
            {
                return;
            }

            SkillNames skillName = SkillNames.None;
            var keys = new List<string>(Skills.Keys);
            for (int i = 0; i < keys.Count; i++)
            {
                string key = keys[i];
                VSkill skill = Skills[key];
                skill.OnLoadGameData();

                if (!EnumEx.ConvertTo(ref skillName, key))
                {
                    Log.Error(LogTags.GameData_Skill, "기술 키를 SkillNames로 변환하지 못했습니다: {0}", key);
                    continue;
                }

                skill.Name = skillName;
                _skillMap[skillName] = skill;
            }
        }

        // 기술 카드 관련 메서드

        public bool CheckUnlockedCard(SkillNames skillName)
        {
            return UnlockedCards.Contains(skillName.ToString());
        }

        public void UnlockCard(SkillNames skillName)
        {
            string key = skillName.ToString();
            if (!UnlockedCards.Contains(key))
            {
                UnlockedCards.Add(key);
                Log.Info(LogTags.GameData_Skill, "기술 카드를 해금합니다: {0}", skillName);
            }
        }

        public bool CheckObtainedCard(SkillNames skillName)
        {
            return ObtainedCards.Contains(skillName.ToString());
        }

        public void ObtainCard(SkillNames skillName)
        {
            string key = skillName.ToString();
            if (!ObtainedCards.Contains(key))
            {
                ObtainedCards.Add(key);
                Log.Info(LogTags.GameData_Skill, "기술 카드를 획득합니다: {0}", skillName);
            }
        }

        // 기술 학습 관련 메서드

        public bool HasSkill(SkillNames skillName)
        {
            return _skillMap.ContainsKey(skillName);
        }

        public VSkill FindSkill(SkillNames skillName)
        {
            if (_skillMap.TryGetValue(skillName, out VSkill skill))
            {
                return skill;
            }

            Log.Warning(LogTags.GameData_Skill, "기술을 찾을 수 없습니다: {0}", skillName.ToLogString());
            return null;
        }

        public void LearnSkill(SkillNames skillName)
        {
            string key = skillName.ToString();
            if (!_skillMap.ContainsKey(skillName))
            {
                VSkill newSkill = new VSkill(skillName);
                Skills[key] = newSkill;
                _skillMap[skillName] = newSkill;

                Log.Info(LogTags.GameData_Skill, "기술을 학습합니다: {0}", skillName.ToLogString());
            }
        }

        // 기술 슬롯 관련 메서드

        public void UnlockSlot(int slotIndex)
        {
            if (Slots.IsValid(slotIndex))
            {
                Slots[slotIndex].IsUnlocked = true;
                Log.Info(LogTags.GameData_Skill, "기술 슬롯을 해금합니다: {0}", slotIndex);
            }
        }

        public bool CheckUnlockedSlot(int slotIndex)
        {
            if (Slots.IsValid(slotIndex))
            {
                return Slots[slotIndex].IsUnlocked;
            }
            return false;
        }

        public void EquipSkill(int slotIndex, SkillNames skillName)
        {
            if (!TryGetSlot(slotIndex, out VSkillSlot slot))
            {
                Log.Warning(LogTags.GameData_Skill, "유효하지 않은 기술 슬롯 인덱스입니다: {0}", slotIndex);
                return;
            }

            if (!_skillMap.ContainsKey(skillName))
            {
                Log.Warning(LogTags.GameData_Skill, "학습하지 않은 기술입니다: {0}", skillName.ToLogString());
                return;
            }

            if (!slot.IsUnlocked)
            {
                Log.Warning(LogTags.GameData_Skill, "해금되지 않은 기술 슬롯입니다: {0}", slotIndex);
                return;
            }

            UnequipSkillFromOtherSlots(slotIndex, skillName);

            string skillNameString = skillName.ToString();
            slot.SkillNameString = skillNameString;
            SyncSlotSkillNameStrings();
            Log.Info(LogTags.GameData_Skill, "기술을 슬롯에 장착합니다: 슬롯 {0}, 기술 {1}", slotIndex, skillName.ToLogString());
        }

        public void UnequipSkill(int slotIndex)
        {
            if (!TryGetSlot(slotIndex, out VSkillSlot slot))
            {
                Log.Warning(LogTags.GameData_Skill, "유효하지 않은 기술 슬롯 인덱스입니다: {0}", slotIndex);
                return;
            }

            if (string.IsNullOrEmpty(slot.SkillNameString))
            {
                return;
            }

            SkillNames skillName = EnumEx.ConvertTo<SkillNames>(slot.SkillNameString);
            if (skillName != SkillNames.None)
            {
                _slotSkillNames.Remove(skillName);
            }

            slot.SkillNameString = string.Empty;
            SyncSlotSkillNameStrings();
            Log.Info(LogTags.GameData_Skill, "기술을 슬롯에서 해제합니다: 슬롯 {0}", slotIndex);
        }

        public SkillNames GetEquippedSkill(int slotIndex)
        {
            if (!TryGetSlot(slotIndex, out VSkillSlot slot))
            {
                return SkillNames.None;
            }

            if (string.IsNullOrEmpty(slot.SkillNameString))
            {
                return SkillNames.None;
            }

            SkillNames skillName = EnumEx.ConvertTo<SkillNames>(slot.SkillNameString);
            return skillName;
        }

        private bool TryGetSlot(int slotIndex, out VSkillSlot slot)
        {
            slot = null;
            if (!Slots.IsValid(slotIndex))
            {
                return false;
            }

            slot = Slots[slotIndex];
            return true;
        }

        private bool TryConvertSkillNameString(string skillNameString, out SkillNames skillName)
        {
            skillName = SkillNames.None;
            return EnumEx.ConvertTo(ref skillName, skillNameString);
        }

        private void UnequipSkillFromOtherSlots(int excludeSlotIndex, SkillNames skillName)
        {
            string skillNameString = skillName.ToString();
            for (int i = 0; i < Slots.Count; i++)
            {
                if (i == excludeSlotIndex)
                {
                    continue;
                }

                VSkillSlot slot = Slots[i];
                if (slot.SkillNameString == skillNameString)
                {
                    slot.SkillNameString = string.Empty;
                    _slotSkillNames.Remove(skillName);
                }
            }
        }

        #region Skill Slots

        public List<SkillNames> GetSkillNames()
        {
            return new List<SkillNames>(_slotSkillNames);
        }

        public List<VSkill> GetSkills()
        {
            List<VSkill> skills = new(_slotSkillNames.Count);
            for (int i = 0; i < _slotSkillNames.Count; i++)
            {
                SkillNames skillName = _slotSkillNames[i];
                if (_skillMap.TryGetValue(skillName, out VSkill skill))
                {
                    skills.Add(skill);
                }
            }

            return skills;
        }

        public bool ContainsSkillSlot(SkillNames skillName)
        {
            if (_slotSkillNames.IsValid())
            {
                return _slotSkillNames.Contains(skillName);
            }

            return false;
        }

        private void LoadSlotSkillNames()
        {
            _slotSkillNames.Clear();

            if (!Slots.IsValid())
            {
                return;
            }

            SkillNames skillName = SkillNames.None;
            for (int i = 0; i < Slots.Count; i++)
            {
                VSkillSlot slot = Slots[i];
                if (!slot.IsUnlocked || string.IsNullOrEmpty(slot.SkillNameString))
                {
                    continue;
                }

                if (!TryConvertSkillNameString(slot.SkillNameString, out skillName))
                {
                    Log.Error(LogTags.GameData_Skill, "기술 슬롯 이름을 SkillNames로 변환하지 못했습니다: {0}", slot.SkillNameString);
                    continue;
                }

                if (_skillMap.ContainsKey(skillName))
                {
                    _slotSkillNames.Add(skillName);
                }
            }
        }

        private void SyncSlotSkillNameStrings()
        {
            _slotSkillNames.Clear();

            if (!Slots.IsValid())
            {
                return;
            }

            SkillNames skillName = SkillNames.None;
            for (int i = 0; i < Slots.Count; i++)
            {
                VSkillSlot slot = Slots[i];
                if (!slot.IsUnlocked || string.IsNullOrEmpty(slot.SkillNameString))
                {
                    continue;
                }

                if (!TryConvertSkillNameString(slot.SkillNameString, out skillName))
                {
                    continue;
                }

                if (!_slotSkillNames.Contains(skillName))
                {
                    _slotSkillNames.Add(skillName);
                }
            }
        }

        #endregion Skill Slots

        public static VCharacterSkill CreateDefault()
        {
            VCharacterSkill defaultSkill = new VCharacterSkill();

            // 초기 슬롯 10개 생성 (0번만 해금)
            for (int i = 0; i < 10; i++)
            {
                defaultSkill.Slots.Add(new VSkillSlot
                {
                    SlotID = i,
                    // IsUnlocked = i == 0,
                    IsUnlocked = true,
                    SkillNameString = string.Empty
                });
            }

            SkillNames[] skills = EnumEx.GetValues<SkillNames>(true);
            for (int i = 0; i < skills.Length; i++)
            {
                defaultSkill.LearnSkill(skills[i]);
            }

            return defaultSkill;
        }
    }
}