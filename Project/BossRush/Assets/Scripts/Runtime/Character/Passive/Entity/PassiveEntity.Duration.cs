using Lean.Pool;


namespace TeamSuneat.Passive
{
    public partial class PassiveEntity : XBehaviour, IPoolable
    {
        public void ApplyAddDuration(Character targetCharacter)
        {
            if (EffectSettings.DurationStateEffect == StateEffects.None)
            {
                return;
            }
            RunewordOption option = ProfileInfo.Inventory.GetEquippedRunewordOption(Name);
            if (option != null)
            {
                targetCharacter.Buff.AddDuration(EffectSettings.DurationStateEffect, option.StateEffectDuration, EffectSettings.AddMaxDuration);
                AppliedEffects |= AppliedEffects.Duration; // 지속시간 증가 완료
            }
            else if (EffectSettings.AddDuration > 0)
            {
                targetCharacter.Buff.AddDuration(EffectSettings.DurationStateEffect, EffectSettings.AddDuration, EffectSettings.AddMaxDuration);
                AppliedEffects |= AppliedEffects.Duration; // 지속시간 증가 완료
            }
        }
    }
}