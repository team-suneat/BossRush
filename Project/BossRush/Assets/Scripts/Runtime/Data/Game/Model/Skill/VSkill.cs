using System;

namespace TeamSuneat.Data.Game
{
    [System.Serializable]
    public class VSkill
    {
        [NonSerialized]
        public SkillNames Name;
        public string NameString; // 스킬 이름

        public int Level; // 스킬 레벨
        public bool IsFirstAwakening; // 1차 각성 여부
        public bool IsSecondAwakening; // 2차 각성 여부
        public int MasteryLevel; // 숙련도 레벨
        public int MasteryProgress; // 숙련도 진행도

        private VSkill()
        {
            NameString = string.Empty;
            Level = 0;
            IsFirstAwakening = false;
            IsSecondAwakening = false;
            MasteryLevel = 0;
            MasteryProgress = 0;
        }

        public VSkill(SkillNames skillName)
        {
            Name = skillName;
            NameString = skillName.ToString();
            Level = 0;
            IsFirstAwakening = false;
            IsSecondAwakening = false;
            MasteryLevel = 0;
            MasteryProgress = 0;
        }

        public void OnLoadGameData()
        {
            EnumEx.ConvertTo(ref Name, NameString);
        }

        public static VSkill CreateDefault()
        {
            return new VSkill();
        }
    }
}