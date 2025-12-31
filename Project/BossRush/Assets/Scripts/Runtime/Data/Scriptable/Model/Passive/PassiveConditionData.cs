using Sirenix.OdinInspector;

namespace TeamSuneat.Data
{
    [System.Serializable]
    public class PassiveConditionData
    {
        [FoldoutGroup("#Condition")] public PassiveTargetTypes ConditionTarget;
        [FoldoutGroup("#Condition")] public BuffNames ConditionBuff;
        [FoldoutGroup("#Condition")] public BuffTypes ConditionBuffType;
        [FoldoutGroup("#Condition")] public int ConditionBuffStack;

        [FoldoutGroup("#String")] public string ConditionTargetString;
        [FoldoutGroup("#String")] public string ConditionSkillCooldownString;
        [FoldoutGroup("#String")] public string ConditionBuffString;
        [FoldoutGroup("#String")] public string ConditionBuffTypeString;

        public void Validate()
        {
            EnumEx.ConvertTo(ref ConditionTarget, ConditionTargetString);
            EnumEx.ConvertTo(ref ConditionBuff, ConditionBuffString);
            EnumEx.ConvertTo(ref ConditionBuffType, ConditionBuffTypeString);
        }

        public void Refresh()
        {
            ConditionTargetString = ConditionTarget.ToString();
            ConditionBuffString = ConditionBuff.ToString();
            ConditionBuffTypeString = ConditionBuffType.ToString();
        }
    }
}