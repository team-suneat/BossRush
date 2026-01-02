using Lean.Pool;
using TeamSuneat.Data;


namespace TeamSuneat.Passive
{
    public partial class PassiveEntity : XBehaviour, IPoolable
    {
        // �нú꿡 ������ �����̵� ����
        private void LoadForceVelocity()
        {
            if (_forceVelocityAsset == null)
            {
                if (EffectSettings.ForceVelocityName != FVNames.None)
                {
                    _forceVelocityAsset = ScriptableDataManager.Instance.FindForceVelocity(EffectSettings.ForceVelocityName);

                    if (!_forceVelocityAsset.IsValid())
                    {
                        LogFailedLoadForceVelocity();
                    }
                }
            }
        }

        private void ResetForceVelocity()
        {
            _forceVelocityAsset = null;
        }

        private void ApplyForceVelocity()
        {
            if (_forceVelocityAsset.IsValid())
            {
                CharacterForceVelocity characterFV = Owner.FindAbility<CharacterForceVelocity>();
                if (characterFV != null)
                {
                    characterFV.StartForceVelocity(_forceVelocityAsset, Owner.IsFacingRight, this);
                    AppliedEffects |= AppliedEffects.ForceVelocity; // 강제 이동 적용 완료
                }
            }
        }
    }
}