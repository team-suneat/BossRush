namespace TeamSuneat
{
    public enum CharacterGrowthTypes
    {
        None,
        Strength,
        HealthPoint,
        Vitality,
        Critical,
        Luck,
        Accuracy,
        Dodge,
    }

    public static class GrowthTypeConverter
    {
        public static StatNames ConvertToStatName(this CharacterGrowthTypes growthType)
        {
            return growthType switch
            {
                CharacterGrowthTypes.Strength => StatNames.Attack,
                CharacterGrowthTypes.HealthPoint => StatNames.Health,
                CharacterGrowthTypes.Vitality => StatNames.HealthRegen,
                CharacterGrowthTypes.Critical => StatNames.CriticalDamage,
                CharacterGrowthTypes.Luck => StatNames.GoldGain,
                CharacterGrowthTypes.Accuracy => StatNames.Accuracy,
                CharacterGrowthTypes.Dodge => StatNames.Dodge,
                _ => StatNames.None
            };
        }
    }
}