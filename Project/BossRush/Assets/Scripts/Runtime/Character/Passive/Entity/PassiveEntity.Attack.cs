using Lean.Pool;
using TeamSuneat.Data;


namespace TeamSuneat.Passive
{
    public partial class PassiveEntity : XBehaviour, IPoolable
    {
        // 패시브의 히트마크 관리

        private void LoadHitmarkAsset()
        {
            if (_hitmarkAssetDatas != null)
                return;

            if (!EffectSettings.Hitmarks.IsValid())
                return;

            _hitmarkAssetDatas = new HitmarkAssetData[EffectSettings.Hitmarks.Length];
            for (int i = 0; i < EffectSettings.Hitmarks.Length; i++)
            {
                HitmarkNames hitmark = EffectSettings.Hitmarks[i];
                _hitmarkAssetDatas[i] = ScriptableDataManager.Instance.FindHitmarkClone(hitmark);
                if (_hitmarkAssetDatas.IsValid()) { continue; }

                LogFailedLoadHitmark();
            }
        }

        private void ResetHitmarkAsset()
        {
            _hitmarkAssetDatas = null;
        }

        // 패시브의 공격 엔티티 관리

        private void ExecuteAttacks(PassiveEffectSettings assetData, PassiveTrigger triggerInfo)
        {
            if (!_hitmarkAssetDatas.IsValid())
            {
                return;
            }

            if (EffectSettings.UseRandomHitmark)
            {
                int randomIndex = TSRandomEx.Range(_hitmarkAssetDatas.Length);
                HitmarkAssetData hitmarkAssetData = _hitmarkAssetDatas[randomIndex];
                ExecuteAttack(assetData, triggerInfo, hitmarkAssetData);
            }
            else
            {
                for (int i = 0; i < _hitmarkAssetDatas.Length; i++)
                {
                    HitmarkAssetData hitmarkAssetData = _hitmarkAssetDatas[i];
                    ExecuteAttack(assetData, triggerInfo, hitmarkAssetData);
                }
            }
        }

        private void ExecuteAttack(PassiveEffectSettings assetData, PassiveTrigger triggerInfo, HitmarkAssetData hitmarkAssetData)
        {
            if (!hitmarkAssetData.IsValid())
            {
                return;
            }

            AttackEntity attackEntity = Owner.Attack.FindEntity(hitmarkAssetData.Name);
            if (attackEntity == null)
            {
                attackEntity = Owner.Attack.SpawnAndRegisterEntity(hitmarkAssetData.Name);
                if (attackEntity == null)
                {
                    attackEntity = Owner.Attack.CreateAndRegisterEntity(hitmarkAssetData);
                }
            }

            if (assetData != null && attackEntity != null)
            {
                LogInfo("패시브의 공격 엔티티를 활성화합니다. Passive의 Hitmark: {0}", hitmarkAssetData.Name.ToLogString());

                Vital targetVital = GetTargetVital(hitmarkAssetData.Name, hitmarkAssetData.AttackTargetType, triggerInfo);

                attackEntity.SetTarget(targetVital);
                attackEntity.SetReferenceValues(triggerInfo);
                attackEntity.SetLevel(Level);

                if (assetData.SetPositionToAttackPosition)
                {
                    if (!triggerInfo.AttackPosition.IsZero())
                    {
                        attackEntity.position = triggerInfo.AttackPosition;
                        LogInfo($"패시브 공격({hitmarkAssetData.Name.ToLogString()}) 엔티티 위치를 트리거의 공격 위치로 설정합니다.");
                    }
                    else
                    {
                        LogInfo($"패시브 공격({hitmarkAssetData.Name.ToLogString()}) 엔티티 위치를 트리거의 공격 위치로 설정할 수 없습니다. 트리거의 공격 위치가 지정되지 않았습니다.");
                    }
                }
                else if (assetData.SetPositionToTargetPosition)
                {
                    if (triggerInfo.TargetVital != null)
                    {
                        attackEntity.position = triggerInfo.TargetVital.position;
                        LogInfo($"패시브 공격({hitmarkAssetData.Name.ToLogString()}) 엔티티 위치를 타겟 위치로 설정합니다.");
                    }
                    else
                    {
                        LogInfo($"패시브 공격({hitmarkAssetData.Name.ToLogString()}) 엔티티 위치를 타겟 위치로 설정할 수 없습니다. 타겟을 찾을 수 없습니다.");
                    }
                }

                attackEntity.Activate();
                AppliedEffects |= AppliedEffects.Attack; // 공격 실행 완료
            }
        }

        private Vital GetTargetVital(HitmarkNames hitmarkName, AttackTargetTypes targetType, PassiveTrigger triggerInfo)
        {
            switch (targetType)
            {
                case AttackTargetTypes.None:
                    {
                        LogInfo("타겟 정보를 찾을 수 없습니다. {0}, AttackTarget:{1}", hitmarkName, targetType);
                    }
                    break;

                case AttackTargetTypes.Owner:
                    {
                        if (Owner != null)
                        {
                            return Owner.MyVital;
                        }
                        else
                        {
                            LogError("타겟 정보를 찾을 수 없습니다. {0}, AttackTarget:{1}", hitmarkName, targetType);
                        }
                    }
                    break;

                case AttackTargetTypes.TargetOfOwner:
                    {
                        if (Owner.TargetCharacter != null)
                        {
                            return Owner.TargetCharacter.MyVital;
                        }
                        else
                        {
                            LogWarning("타겟 정보를 찾을 수 없습니다. {0}, AttackTarget:{1}", hitmarkName, targetType);
                        }
                    }
                    break;

                case AttackTargetTypes.TargetOfPassive:
                    {
                        Character targetCharacter = GetTargetCharacterForExecute(triggerInfo);
                        if (targetCharacter != null)
                        {
                            return targetCharacter.MyVital;
                        }
                        else
                        {
                            LogWarning("타겟 정보를 찾을 수 없습니다. {0}, AttackTarget:{1}", hitmarkName, targetType);
                        }
                    }
                    break;

                default:
                    {
                        LogWarning("타겟 정보를 찾을 수 없습니다. {0}, AttackTarget:{1}", hitmarkName, targetType);
                    }
                    break;
            }

            return null;
        }

        private Character GetTargetCharacterForExecute(PassiveTrigger triggerInfo)
        {
            if (EffectSettings.ApplyTarget == PassiveTargetTypes.Owner)
            {
                return Owner;
            }
            else if (EffectSettings.ApplyTarget == PassiveTargetTypes.Target)
            {
                return triggerInfo.TargetCharacter;
            }
            else if (EffectSettings.ApplyTarget == PassiveTargetTypes.Attacker)
            {
                return triggerInfo.Attacker;
            }

            return null;
        }
    }
}