using TeamSuneat.Data;

namespace TeamSuneat
{
    public partial class DamageCalculator
    {
        public void RefreshReferenceValue(HitmarkAssetData damageAsset)
        {
            switch (damageAsset.LinkedDamageType)
            {
                case LinkedDamageTypes.None:
                    return;

                case LinkedDamageTypes.Attack:
                    {
                        ReferenceValue = DamageReferenceValue;
                        LogProgressReferenceValue("피해량", ReferenceValue);
                    }
                    break;

                case LinkedDamageTypes.CurrentHealthOfAttacker:
                    {
                        ReferenceValue = Attacker.MyVital.CurrentHealth;
                        LogProgressReferenceValue("공격자의 현재 생명력", ReferenceValue);
                    }
                    break;

                case LinkedDamageTypes.MaxHealthOfAttacker:
                    {
                        ReferenceValue = Attacker.MyVital.MaxHealth;
                        LogProgressReferenceValue("공격자의 최대 생명력", ReferenceValue);
                    }
                    break;

                case LinkedDamageTypes.CurrentShieldOfAttacker:
                    {
                        ReferenceValue = Attacker.MyVital.CurrentShield;
                        LogProgressReferenceValue("공격자의 현재 보호막", ReferenceValue);
                    }
                    break;

                case LinkedDamageTypes.MaxShieldOfAttacker:
                    {
                        ReferenceValue = Attacker.MyVital.MaxShield;
                        LogProgressReferenceValue("공격자의 최대 보호막", ReferenceValue);
                    }
                    break;

                case LinkedDamageTypes.CurrentHealthOfTarget:
                    {
                        ReferenceValue = TargetVital.CurrentHealth;
                        LogProgressReferenceValue("피격자의 현재 생명력", ReferenceValue);
                    }
                    break;

                case LinkedDamageTypes.MaxHealthOfTarget:
                    {
                        ReferenceValue = TargetVital.MaxHealth;
                        LogProgressReferenceValue("피격자의 최대 생명력", ReferenceValue);
                    }
                    break;

                case LinkedDamageTypes.SkillCooldownTime:
                    {
                        ReferenceValue = CooldownReferenceValue;
                        LogProgressReferenceValue("기술의 재사용 대기시간", ReferenceValue);
                    }
                    break;
            }

            if (ReferenceValue == 0)
            {
                LogErrorReferenceValue(damageAsset.LinkedDamageType, damageAsset.LinkedStateEffect, ReferenceValue);
            }
            else if (damageAsset.LinkedStateEffect != StateEffects.None)
            {
                int stack = Attacker.Buff.FindStack(damageAsset.LinkedStateEffect);
                if (stack > 0)
                {
                    ReferenceValue *= stack;
                    LogProgressReferenceValue("공격자의 상태이상 스택 적용", ReferenceValue);
                }
            }
        }
    }
}