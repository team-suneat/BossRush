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

                case LinkedDamageTypes.CurrentLifeOfAttacker:
                    {
                        ReferenceValue = Attacker.MyVital.CurrentLife;
                        LogProgressReferenceValue("공격자의 현재 생명력", ReferenceValue);
                    }
                    break;

                case LinkedDamageTypes.MaxLifeOfAttacker:
                    {
                        ReferenceValue = Attacker.MyVital.MaxLife;
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

                case LinkedDamageTypes.CurrentLifeOfTarget:
                    {
                        ReferenceValue = TargetVital.CurrentLife;
                        LogProgressReferenceValue("피격자의 현재 생명력", ReferenceValue);
                    }
                    break;

                case LinkedDamageTypes.MaxLifeOfTarget:
                    {
                        ReferenceValue = TargetVital.MaxLife;
                        LogProgressReferenceValue("피격자의 최대 생명력", ReferenceValue);
                    }
                    break;
            }

            if (ReferenceValue == 0)
            {
                LogErrorReferenceValue(damageAsset.LinkedDamageType, damageAsset.LinkedStateEffect, ReferenceValue);
            }
        }
    }
}