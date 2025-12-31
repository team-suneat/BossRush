namespace TeamSuneat
{
    public enum RendererFlickerNames
    {
        None,

        Damage,
        Bleeding,
        Burning,
        Poisoning,
        Chilled,
        Fire,
        Cold,
        Lightning,
        Invulnerable,
        Dash,

        White,
        Yellow,
    }

    public static class FlickerConverter
    {
        public static RendererFlickerNames ConvertToFlicker(this DamageTypes damageType)
        {
            if (damageType == DamageTypes.DamageOverTime)
            {
                return RendererFlickerNames.Bleeding;
            }

            return RendererFlickerNames.Damage;
        }
    }
}