using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TeamSuneat;
using UnityEngine;

namespace TeamSuneat
{
    public class AttackTargetEntity : AttackEntity
    {
        [FoldoutGroup("#AttackTargetEntity-Times")]
        public float AttackDelayTime;

        [FoldoutGroup("#AttackTargetEntity-Times")]
        public float AttackRestTime;

        [FoldoutGroup("#AttackTargetEntity-Decrescence Damage")]
        public int AdditionalHitCount;

        private bool _isRest;

        //

        public override void Activate()
        {
            if (DetermineActivate())
            {
                LogInfo("Attack Target Entity를 활성화합니다.");

                base.Activate();

                Apply();

                base.OnActivate();
            }
        }

        public override void Deactivate()
        {
            LogInfo("Attack Target Entity를 비활성화합니다.");

            _isRest = false;

            base.Deactivate();
        }

        public override void Apply()
        {
            LogInfo("목표 공격 독립체를 적용합니다. 레벨:{0} ", Level);

            base.Apply();

            if (_isRest)
            {
                return;
            }

            RefreshTarget();

            if (AttackDelayTime > 0 || AttackRestTime > 0)
            {
                StartXCoroutine(ProcessAttack());
            }
            else
            {
                ApplyAttack();
            }
        }

        private void ApplyAttack()
        {
            bool isAttackSuccessed = AttackToTarget();
            OnAttack(isAttackSuccessed);
        }

        public override void SetTarget(Vital targetVital)
        {
            base.SetTarget(targetVital);

            _damageInfo.SetTargetVital(targetVital);
        }

        private void RefreshTarget()
        {
            if (!AssetData.IsValid())
            {
                return;
            }

            if (_damageInfo.TargetVital != null)
            {
                return;
            }

            Vital targetVital = null;
            switch (AssetData.AttackTargetType)
            {
                case AttackTargetTypes.Owner:
                    {
                        if (Owner != null)
                        {
                            targetVital = Owner.MyVital;
                        }
                    }
                    break;

                case AttackTargetTypes.TargetOfOwner:
                    {
                        if (Owner != null && Owner.TargetCharacter != null)
                        {
                            targetVital = Owner.TargetCharacter.MyVital;
                        }
                    }
                    break;

                case AttackTargetTypes.TargetOfProjectile:
                    {
                        if (OwnerProjectile != null)
                        {
                            targetVital = OwnerProjectile.TargetVital;
                        }
                    }
                    break;
            }

            if (CheckDamageableVital(targetVital))
            {
                _damageInfo.SetTargetVital(targetVital);
            }
        }

        private bool CheckDamageableVital(Vital targetVital)
        {
            if (targetVital == null)
            {
                return false;
            }
            else if (!targetVital.IsAlive)
            {
                return false;
            }
            else if (targetVital.Health.CheckInvulnerable())
            {
                return false;
            }

            return true;
        }

        private IEnumerator ProcessAttack()
        {
            if (AttackDelayTime > 0)
            {
                yield return new WaitForSeconds(AttackDelayTime);
            }

            ApplyAttack();

            if (AttackRestTime > 0)
            {
                _isRest = true;

                yield return new WaitForSeconds(AttackRestTime);

                _isRest = false;
            }
        }

        private bool AttackToTarget()
        {
            if (_damageInfo.TargetVital == null)
            {
                LogWarning("공격 독립체의 목표 바이탈이 설정되지 않았습니다. Hitmark: {0}, Entity: {1}", _damageInfo.HitmarkAssetData.Name.ToLogString(), this.GetHierarchyPath());

                return false;
            }

            if (!_damageInfo.HitmarkAssetData.IsValid())
            {
                LogError("피해량 정보의 히트마크 에셋이 올바르지 않습니다. Hitmark:{0}, Entity: {1}", Name.ToLogString(), this.GetHierarchyPath());
                return false;
            }

            ApplyDecrescenceDamageFromProjectile();
            _damageInfo.Execute();
            if (!_damageInfo.DamageResults.IsValid())
            {
                LogWarning("공격 독립체의 피해 결과가 설정되지 않았습니다. Hitmark: {0}, Entity: {1}", _damageInfo.HitmarkAssetData.Name.ToLogString(), this.GetHierarchyPath());

                return false;
            }

            bool isAttackSuccessed = false;
            DamageResult damageResult;
            for (int i = 0; i < _damageInfo.DamageResults.Count; i++)
            {
                damageResult = _damageInfo.DamageResults[i];

                switch (damageResult.DamageType)
                {
                    case DamageTypes.Heal:
                    case DamageTypes.HealOverTime:
                        {
                            _damageInfo.TargetVital.Heal(damageResult.DamageValueToInt);
                            _damageInfo.TargetVital.DamageBuffOnHit(damageResult);
                            isAttackSuccessed = true;
                        }
                        break;

                    case DamageTypes.Restore:
                        {
                            _damageInfo.TargetVital.Restore(damageResult.DamageValueToInt);
                            _damageInfo.TargetVital.DamageBuffOnHit(damageResult);

                            isAttackSuccessed = true;
                        }
                        break;

                    case DamageTypes.Charge:
                        {
                            _damageInfo.TargetVital.Charge(damageResult.DamageValueToInt);
                            _damageInfo.TargetVital.DamageBuffOnHit(damageResult);

                            isAttackSuccessed = true;
                        }
                        break;

                    case DamageTypes.CooldownTimeReduction:
                        {
                            if (TryReduceSkillCooldownTime(damageResult))
                            {
                                _damageInfo.TargetVital.DamageBuffOnHit(damageResult);
                                isAttackSuccessed = true;
                            }
                        }
                        break;

                    default:
                        {
                            if (!_damageInfo.TargetVital.CheckDamageImmunity(damageResult))
                            {
                                if (_damageInfo.TargetVital.TakeDamage(damageResult))
                                {
                                    if (_damageInfo.TargetVital.IsAlive)
                                    {
                                        TriggerAttackOnHitDamageableFeedback(_damageInfo.TargetVital.position);
                                    }
                                    else
                                    {
                                        TriggerAttackOnKillFeedback(_damageInfo.TargetVital.position);
                                    }
                                }

                                isAttackSuccessed = true;
                            }
                        }
                        break;
                }
            }

            return isAttackSuccessed;
        }

        private void ApplyDecrescenceDamageFromProjectile()
        {
            if (OwnerProjectile != null)
            {
                if (OwnerProjectile.IsDecrescenceDamageByPenetration)
                {
                    int penetrationsCount = OwnerProjectile.PenetrationsCount - OwnerProjectile.RemainingPenetrationsCount;
                    int hitCount = penetrationsCount + AdditionalHitCount;

                    _damageInfo.SetDecrescenceRate(hitCount, ApplyCount);
                }
            }
        }

        private bool TryReduceSkillCooldownTime(DamageResult damageResult)
        {
            if (_damageInfo.TargetCharacter != null && _damageInfo.TargetCharacter.Skill != null)
            {
                bool isAttackSuccessed = false;
                List<SkillEntity> entities = _damageInfo.TargetCharacter.Skill.FindAll(damageResult.Asset.LinkedSkillCategory);
                for (int j = 0; j < entities.Count; j++)
                {
                    entities[j].ReduceCooldownTime(damageResult.DamageValue);
                    isAttackSuccessed = true;
                }

                return isAttackSuccessed;
            }

            return false;
        }

        #region Log

        protected override void LogProgress(string content)
        {
            if (Log.LevelProgress)
            {
                if (Owner != null)
                {
                    Log.Progress(LogTags.Attack_Target, TSStringGetter.ConcatStringWithComma(Owner.Name.ToLogString(), Name.ToLogString(), content));
                }
                else if (OwnerProjectile != null)
                {
                    Log.Progress(LogTags.Attack_Target, TSStringGetter.ConcatStringWithComma(OwnerProjectile.Name.ToLogString(), Name.ToLogString(), content));
                }
            }
        }

        protected override void LogInfo(string content)
        {
            if (Log.LevelInfo)
            {
                if (Owner != null)
                {
                    Log.Info(LogTags.Attack_Target, TSStringGetter.ConcatStringWithComma(Owner.Name.ToLogString(), Name.ToLogString(), content));
                }
                else if (OwnerProjectile != null)
                {
                    Log.Info(LogTags.Attack_Target, TSStringGetter.ConcatStringWithComma(OwnerProjectile.Name.ToLogString(), Name.ToLogString(), content));
                }
            }
        }

        protected override void LogWarning(string content)
        {
            if (Log.LevelWarning)
            {
                if (Owner != null)
                {
                    Log.Warning(LogTags.Attack_Target, TSStringGetter.ConcatStringWithComma(Owner.Name.ToLogString(), Name.ToLogString(), content));
                }
                else if (OwnerProjectile != null)
                {
                    Log.Warning(LogTags.Attack_Target, TSStringGetter.ConcatStringWithComma(OwnerProjectile.Name.ToLogString(), Name.ToLogString(), content));
                }
            }
        }

        #endregion Log
    }
}