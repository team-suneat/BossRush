using UnityEngine;

namespace TeamSuneat
{
    public static class EnumColorEx
    {
        public static Color GetGradeColor(this GradeNames gradeName, float alpha)
        {
            Color result = GetGradeColor(gradeName);
            return new Color(result.r, result.g, result.b, alpha);
        }

        public static Color GetGradeColor(this GradeNames gradeName)
        {
            switch (gradeName)
            {
                case GradeNames.Common: return GameColors.Common;
                case GradeNames.Grand: return GameColors.Grand;
                case GradeNames.Rare: return GameColors.Rare;
                case GradeNames.Epic: return GameColors.Epic;
                case GradeNames.Legendary: return GameColors.Legendary;
                case GradeNames.Mythic: return GameColors.Mythic;
                case GradeNames.Immortal: return GameColors.Immortal;
                case GradeNames.Ancient: return GameColors.Ancient;
            }

            return Color.white;
        }

        public static string GetGradeColorHex(this GradeNames gradeName)
        {
            switch (gradeName)
            {
                case GradeNames.Common: return "#2F4F4F"; // DarkSlateGray
                case GradeNames.Grand: return "#6B8E23"; // OliveDrab
                case GradeNames.Rare: return "#D2691E"; // Chocolate
                case GradeNames.Epic: return "#4B0082"; // Indigo
                case GradeNames.Legendary: return "#8B0000"; // DarkRed
                case GradeNames.Mythic: return "#4169E1"; // RoyalBlue
                case GradeNames.Immortal: return "#808000"; // Olive
                case GradeNames.Ancient: return "#00CED1"; // DarkTurquoise
            }

            return "#FFFFFF";
        }

        //

        public static Color GetColor(DamageTypes damageType, float alpha = 1f)
        {
            switch (damageType)
            {
                case DamageTypes.Normal:
                    return GameColors.Physical;

                case DamageTypes.Thorns:
                    return GameColors.Thorns;

                case DamageTypes.DamageOverTime:
                    return GameColors.Bleed;

                default:
                    return GameColors.CreamIvory;
            }
        }

        public static Color GetColor(GameElements element, float alpha = 1f)
        {
            switch (element)
            {
                case GameElements.Fire:
                    return GameColors.Fire;

                case GameElements.Cold:
                    return GameColors.Cold;

                case GameElements.Lightning:
                    return GameColors.Lightning;

                case GameElements.Poison:
                    return GameColors.Poison;

                case GameElements.Holy:
                    return GameColors.Holy;

                case GameElements.Darkness:
                    return GameColors.Darkness;

                case GameElements.Blood:
                    return GameColors.Bleed;

                default:
                    return GameColors.CreamIvory;
            }
        }

        public static Color GetColor(this StateEffects stateEffect)
        {
            switch (stateEffect)
            {
                case StateEffects.Burning:
                    return GameColors.Fire;

                case StateEffects.Chilled:
                case StateEffects.Freeze:
                    return GameColors.Cold;

                case StateEffects.Jolted:
                case StateEffects.ElectricShock:
                    return GameColors.Lightning;

                case StateEffects.Poisoning:
                    return GameColors.Poison;

                case StateEffects.Bleeding:
                    return GameColors.Bleed;

                case StateEffects.Vulnerable:
                case StateEffects.EnhancedVulnerable:
                    return GameColors.Vulnerable;

                case StateEffects.Dazed:
                    return GameColors.Dazed;

                case StateEffects.Paralysis:
                    return GameColors.Paralysis;

                default:
                    return GameColors.CreamIvory;
            }
        }
    }
}