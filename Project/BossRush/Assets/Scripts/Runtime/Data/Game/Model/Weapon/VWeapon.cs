using System;
using System.Collections.Generic;

namespace TeamSuneat.Data.Game
{
    [System.Serializable]
    public class VWeapon
    {
        [NonSerialized]
        public ItemNames Name;
        public string NameString;
        public int Level;

        [NonSerialized]
        public List<GradeNames> Grades = new();
        public List<string> GradeStrings = new();

        [NonSerialized]
        public List<StatNames> Stats = new();
        public List<string> StatStrings = new();

        private VWeapon()
        {
            NameString = string.Empty;
            Level = 0;
        }

        public VWeapon(ItemNames weaponName)
        {
            Name = weaponName;
            NameString = weaponName.ToString();
            Level = 1;
        }

        public void OnLoadGameData()
        {
            EnumEx.ConvertTo(ref Name, NameString);
            EnumEx.ConvertTo(ref Grades, GradeStrings);
            EnumEx.ConvertTo(ref Stats, StatStrings);
        }

        public void AddGrade(GradeNames gradeName, StatNames statName)
        {
            Grades.Add(gradeName);
            GradeStrings.Add(gradeName.ToString());

            Stats.Add(statName);
            StatStrings.Add(statName.ToString());

            Level = Grades.Count + 1;

            Log.Info(LogTags.GameData_Weapon, "인게임 무기의 추가 옵션 등급과 능력치를 등록합니다: {0}(Lv.{1}, {2}), {3} ",
                Name.ToLogString(), Level, gradeName.ToLogString(), statName.ToLogString());
        }
    }
}