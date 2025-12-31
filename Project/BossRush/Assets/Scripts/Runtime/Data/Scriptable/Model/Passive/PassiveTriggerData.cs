using System;
using TeamSuneat.Passive;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat.Data
{
    [System.Serializable]
    public class PassiveTriggerData
    {
        [FoldoutGroup("#Trigger")] public PassiveTriggers Trigger;
        [FoldoutGroup("#Trigger")] public DamageTypes TriggerDamageType;
        [FoldoutGroup("#Trigger")] public HitmarkNames[] TriggerHitmarks;
        [FoldoutGroup("#Trigger")] public HitmarkNames[] TriggerIgnoreHitmarks;
        [FoldoutGroup("#Trigger")] public PassiveNames TriggerPassive;
        [FoldoutGroup("#Trigger")] public BuffNames TriggerBuff;
        [FoldoutGroup("#Trigger")] public BuffTypes TriggerBuffType;

        [GUIColor("GetStatNameColor")]
        [FoldoutGroup("#Trigger")] public StatNames TriggerStat;

        [FoldoutGroup("#Trigger")] public int TriggerCount;

        [FoldoutGroup("#Trigger")][Range(0f, 1f)] public float TriggerPercent;
        [FoldoutGroup("#Trigger")] public PassiveOperator TriggerOperator;

        [FoldoutGroup("#String")] public string TriggerString;
        [FoldoutGroup("#String")] public string TriggerDamageTypeString;
        [FoldoutGroup("#String")] public string[] TriggerHitmarksString;
        [FoldoutGroup("#String")] public string[] TriggerIgnoreHitmarksString;
        [FoldoutGroup("#String")] public string TriggerPassiveString;
        [FoldoutGroup("#String")] public string TriggerBuffString;
        [FoldoutGroup("#String")] public string TriggerBuffTypeString;
        [FoldoutGroup("#String")] public string TriggerStatString;
        [FoldoutGroup("#String")] public string TriggerOperatorString;

        public void Validate()
        {
            EnumEx.ConvertTo(ref Trigger, TriggerString);
            EnumEx.ConvertTo(ref TriggerDamageType, TriggerDamageTypeString);
            EnumEx.ConvertTo(ref TriggerHitmarks, TriggerHitmarksString);

            EnumEx.ConvertTo(ref TriggerIgnoreHitmarks, TriggerIgnoreHitmarksString);
            EnumEx.ConvertTo(ref TriggerPassive, TriggerPassiveString);
            EnumEx.ConvertTo(ref TriggerBuff, TriggerBuffString);
            EnumEx.ConvertTo(ref TriggerBuffType, TriggerBuffTypeString);
            EnumEx.ConvertTo(ref TriggerStat, TriggerStatString);
            EnumEx.ConvertTo(ref TriggerOperator, TriggerOperatorString);
        }

        public void Refresh()
        {
            TriggerString = Trigger.ToString();
            TriggerDamageTypeString = TriggerDamageType.ToString();
            TriggerHitmarksString = TriggerHitmarks.ToStringArray();
            TriggerIgnoreHitmarksString = TriggerIgnoreHitmarks.ToStringArray();
            TriggerPassiveString = TriggerPassive.ToString();
            TriggerBuffString = TriggerBuff.ToString();
            TriggerBuffTypeString = TriggerBuffType.ToString();
            TriggerStatString = TriggerStat.ToString();
            TriggerOperatorString = TriggerOperator.ToString();
        }
    }
}