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
            bool isAttackSucceeded = AttackToTarget();
            OnAttack(isAttackSucceeded );
        }

        public override void SetTarget(Vital targetVital)
        {
            base.SetTarget(targetVital);

            _damageCalculator.SetTargetVital(targetVital);
        }

        private void RefreshTarget()
        {
            if (!AssetData.IsValid())
            {
                return;
            }

            if (_damageCalculator.TargetVital != null)
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

            }

            if (CheckDamageableVital(targetVital))
            {
                _damageCalculator.SetTargetVital(targetVital);
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
            else if (targetVital.Life.CheckInvulnerable())
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
            if (_damageCalculator.TargetVital == null)
            {
                LogWarning("공격 독립체의 목표 바이탈이 설정되지 않았습니다. Hitmark: {0}, Entity: {1}", _damageCalculator.HitmarkAssetData.Name.ToLogString(), this.GetHierarchyPath());

                return false;
            }

            if (!_damageCalculator.HitmarkAssetData.IsValid())
            {
                LogError("피해량 정보의 히트마크 에셋이 올바르지 않습니다. Hitmark:{0}, Entity: {1}", Name.ToLogString(), this.GetHierarchyPath());
                return false;
            }

            _damageCalculator.Execute();
            if (!_damageCalculator.DamageResults.IsValid())
            {
                LogWarning("공격 독립체의 피해 결과가 설정되지 않았습니다. Hitmark: {0}, Entity: {1}", _damageCalculator.HitmarkAssetData.Name.ToLogString(), this.GetHierarchyPath());

                return false;
            }

            bool isAttackSucceeded = false;
            DamageResult damageResult;
            for (int i = 0; i < _damageCalculator.DamageResults.Count; i++)
            {
                damageResult = _damageCalculator.DamageResults[i];

                switch (damageResult.DamageType)
                {
                    case DamageTypes.Heal:
                    case DamageTypes.HealOverTime:
                        {
                            _damageCalculator.TargetVital.Heal(damageResult.DamageValueToInt);
                            isAttackSucceeded = true;
                        }
                        break;

                    default:
                        {
                            if (!_damageCalculator.TargetVital.CheckDamageImmunity(damageResult))
                            {
                                if (_damageCalculator.TargetVital.TakeDamage(damageResult))
                                {
                                    if (_damageCalculator.TargetVital.IsAlive)
                                    {
                                        TriggerAttackOnHitDamageableFeedback(_damageCalculator.TargetVital.position);
                                    }
                                    else
                                    {
                                        TriggerAttackOnKillFeedback(_damageCalculator.TargetVital.position);
                                    }
                                }

                                isAttackSucceeded = true;
                            }
                        }
                        break;
                }
            }

            if (isAttackSucceeded && Owner != null && Owner.IsPlayer)
            {
                OnPlayerAttackSuccess();
            }

            return isAttackSucceeded;
        }

        /// <summary> 플레이어 공격 성공 시 마나 게이지 증가. </summary>
        private void OnPlayerAttackSuccess()
        {
            if (Owner == null || Owner.MyVital == null)
            {
                return;
            }

            Mana mana = Owner.MyVital.Mana;
            if (mana != null)
            {
                mana.OnAttackSuccess();
            }
        }

        #region Log

        protected override void LogProgress(string content)
        {
            if (Log.LevelProgress)
            {
                if (Owner != null)
                {
                    Log.Progress(LogTags.Attack, StringGetter.ConcatStringWithComma(Owner.Name.ToLogString(), Name.ToLogString(), content));
                }
            }
        }

        protected override void LogInfo(string content)
        {
            if (Log.LevelInfo)
            {
                if (Owner != null)
                {
                    Log.Info(LogTags.Attack, StringGetter.ConcatStringWithComma(Owner.Name.ToLogString(), Name.ToLogString(), content));
                }
            }
        }

        protected override void LogWarning(string content)
        {
            if (Log.LevelWarning)
            {
                if (Owner != null)
                {
                    Log.Warning(LogTags.Attack, StringGetter.ConcatStringWithComma(Owner.Name.ToLogString(), Name.ToLogString(), content));
                }
                
            }
        }

        #endregion Log
    }
}