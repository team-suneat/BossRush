using Lean.Pool;
using UnityEngine;

namespace TeamSuneat
{
    public partial class Projectile
    {
        protected int _anotherCount;
        protected int _anotherMaxCount;
        protected int _throughCount;
        protected int _throughMaxCount;

        public int ThroughCount => _throughCount;

        protected int AnotherMaxCount => _anotherMaxCount;

        protected int ThroughMaxCount => _throughMaxCount;

        private void SetAnotherMaxCount()
        {
            _anotherMaxCount = _projectileData.ChainCount;
        }

        private void SetThroughMaxCount()
        {
            _throughMaxCount = _projectileData.ThroughMaxCount;
        }

        public void InitializeAttack()
        {
            if (AttackSystem != null)
            {
                AttackSystem.Initialize();
                if (AttackSystem.Attack != null)
                {
                    SetTargetLayer(AttackSystem.Attack.DamageLayer);
                }
            }
        }

        public void InitializeMaxCount()
        {
            SetAnotherMaxCount();

            SetThroughMaxCount();
        }

        public void SetTargetLayer(LayerMask targetLayer)
        {
            m_targetLayer = targetLayer;
        }

        public void SetChargeRate(float chargeRate)
        {
            if (AttackSystem != null)
            {
                AttackSystem.ChargeRate = chargeRate;
            }
        }

        public void SetAttackCustomDamage(int customDamage)
        {
            if (AttackSystem != null)
            {
                AttackSystem.SetAttackCustomDamage(customDamage);
            }
        }

        public void AddAttackTarget(Vital targetVital)
        {
            AttackEntityTypes entityType = AttackSystem.GetAttackType();

            if (entityType == AttackEntityTypes.Target)
            {
                AttackSystem.AddAttackTarget(targetVital);
            }
        }

        private bool TryAttackOnHit(Vital targetVital)
        {
            if (false == _projectileData.IsAttackOnHit)
            {
                Log.Warning(LogTags.Projectile, "{0} 발사체는 충돌시 공격하지 않는다.",
                    _projectileData.Name.ToLogString());
                return false;
            }

            if (false == targetVital.CompareLayer(_projectileData.DamageLayer))
            {
                Log.Warning(LogTags.Projectile, "Target Vital의 레이어는 공격하지 않는다. {0}",
                    LayerMask.LayerToName(targetVital.gameObject.layer));
                return false;
            }

            if (false == TryDamageToTarget(targetVital))
            {
                return false;
            }

            switch (_motionType)
            {
                case ProjectileMotionTypes.Homing:
                case ProjectileMotionTypes.DetectHoming:
                case ProjectileMotionTypes.HomingLinear:
                    {
                        if (_homingTarget == null)
                        {
                            Log.Warning(LogTags.Projectile, "{0}, 유도 발사체가 설정된 타겟이 없다.",
                                _projectileData.Name.ToLogString());
                            return false;
                        }

                        if (_homingTarget != targetVital)
                        {
                            Log.Warning(LogTags.Projectile, "{0}, 유도 발사체가 설정한 타겟과 다르다. TargetVital:{1}, CollisionVital:{2}",
                                _projectileData.Name.ToLogString(),
                                _homingTarget.GetHierarchyPath(),
                                targetVital.GetHierarchyPath());
                            return false;
                        }
                    }
                    break;
            }

            return true;
        }

        private void AttackOnHit(Vital targetVital)
        {
            AttackEntityTypes entityType = AttackSystem.GetAttackType();
            switch (entityType)
            {
                case AttackEntityTypes.Target:
                    {
                        if (TryTargetAttack(targetVital))
                        {
                            SetHomingTarget(targetVital);

                            AddAttackTarget(targetVital);

                            StartAttack();

                            AttackedOnHit();
                        }
                    }
                    break;

                case AttackEntityTypes.Area:
                    {
                        StartAttack();

                        AttackedOnHit();
                    }
                    break;
            }
        }

        public bool TryTargetAttack(Vital targetVital)
        {
            if (targetVital == null)
            {
                Log.Warning(LogTags.Projectile, "타겟이 설정되지 않아 공격할 수 없습니다. {0}", Name.ToLogString());
                return false;
            }

            switch (_motionType)
            {
                case ProjectileMotionTypes.Homing:
                case ProjectileMotionTypes.HomingLinear:
                case ProjectileMotionTypes.DetectHoming:
                    {
                        if (HomingTarget != null)
                        {
                            if (HomingTarget != targetVital)
                            {
                                Log.Warning(LogTags.Projectile, "설정된 유도 타겟과 공격하려는 타겟이 달라 공격할 수 없습니다. {0}, homing target:{1} != target vital: {2}",
                                    Name.ToLogString(), HomingTarget.name, targetVital.name);

                                return false;
                            }
                        }
                    }
                    break;
            }

            if (IsThrough)
            {
                if (_throughCount >= ThroughMaxCount)
                {
                    Log.Info(LogTags.Projectile, "발사체의 관통 개수가 최대입니다. {0}, {1}/{2}", Name.ToLogString(), _throughCount, ThroughMaxCount);
                    return false;
                }
            }

            if (AttackSystem == null)
            {
                Log.Info(LogTags.Projectile, "발사체의 공격 시스템이 설정되지 않았습니다. {0}", Name.ToLogString());
                return false;
            }

            return true;
        }

        protected bool TryDamageToTarget(Vital targetVital)
        {
            if (AttackSystem == null)
            {
                return targetVital.TryDamage();
            }

            if (AttackSystem.Attack == null)
            {
                return targetVital.TryDamage();
            }

            if (targetVital.TakeDamage(AttackSystem.Attack.DamageInfo))
            {
                return true;
            }

            Log.Warning(LogTags.Projectile, "Target Vital은 피해입을 수 없는 상태입니다. {0}, target: {1}", Name.ToLogString(), targetVital.name);

            return false;
        }

        public void StartAttackOnSpawn()
        {
            if (_projectileData.IsAttackOnSpawn)
            {
                StartAttack();
            }
        }

        public void StartAttack()
        {
            if (Animator != null)
            {
                if (TryPlayAnimation())
                {
                    if (false == PlayAttackAnimation())
                    {
                        DoAttack();
                    }
                }
            }
            else
            {
                DoAttack();
            }

            SpawnAttackFX(FacingRightAtLaunch);
        }

        /// <summary>
        /// Called MapObject Trigger, Projectile Detector
        /// </summary>
        public void StartAttackByDetector()
        {
            StartAttack();
        }

        public void DoAttack()
        {
            if (AttackSystem != null)
            {
                AttackSystem.DoAttack();

                DoResult(_projectileData.AttackResult);

                if (_projectileData.IsFirstChain || _projectileData.IsAnotherChain)
                {
                    ChainProjectileInfo.AddTargetWithRegister(AttackSystem.Attack.TargetVitals.ToArray());
                }
            }
            else
            {
                Log.Warning(LogTags.Projectile, "발사체 공격을 할 수 없습니다. 발사체의 공격시스템을 찾을 수 없습니다. {0}", Name.ToLogString());
            }
        }

        public void AttackedOnHit()
        {
            if (false == IsThrough)
            {
                DoAnotherAttack();

                Destroy();
            }
            else
            {
                Log.Info(LogTags.Projectile, "m_throughCount : {0} / {1}", _throughCount + 1, ThroughMaxCount);

                if (_throughCount < ThroughMaxCount)
                {
                    _throughCount += 1;

                    DoAnotherAttack();
                }

                if (_throughCount >= ThroughMaxCount)
                {
                    Destroy();
                }
            }
        }

        public void StopAttack()
        {
            if (AttackSystem != null)
            {
                AttackSystem.StopAttack();
            }
        }

        public void StopAnotherAttack()
        {
            if (AttackSystem != null)
            {
                AttackSystem.StopAnother();
            }
        }

        public void StopReturnAttack()
        {
            if (AttackSystem != null)
            {
                AttackSystem.StopReturnAttack();
            }
        }

        public void DoAnotherAttack()
        {
            if (_isSpawnedAnother)
            {
                Log.Warning(LogTags.Projectile, "이미 추가 발사체를 생성했다. {0}", this.GetHierarchyPath());
                return;
            }

            _isSpawnedAnother = true;

            if (_anotherCount >= AnotherMaxCount)
            {
                ApplyReturn();
                return;
            }

            _anotherCount++;

            if (AttackSystem != null)
            {
                AttackSystem.SetAnotherChainProjectileInfo(ChainProjectileInfo);

                if (AttackSystem.Another != null)
                {
                    AttackSystem.Another.SetAnotherCount(_anotherCount);

                    AttackSystem.Another.SetAnotherMaxCount(AnotherMaxCount);
                }

                if (false == AttackSystem.ExecuteAnother())
                {
                    ApplyReturn();
                }
            }
        }

        public void ForceApplyReturn()
        {
            ApplyReturn();
        }

        protected void ApplyReturn()
        {
            if (_isSpawnedReturn)
            {
                Log.Warning(LogTags.Projectile, "Failed To Apply Return. path:{0}, m_isSpawnedReturn: TRUE", this.GetHierarchyPath());
                return;
            }

            _isSpawnedReturn = true;

            if (AttackSystem != null)
            {
                AttackSystem.ExecuteReturn();
            }
            else
            {
                Log.Error("Projectile Attack System is null. {0}", this.GetHierarchyPath());
            }
        }

        protected void ReleaseReturn()
        {
            if (AttackSystem == null)
            {
                return;
            }

            if (Owner == null)
            {
                return;
            }

            if (false == AttackSystem.CheckReturnProjectile())
            {
                return;
            }

            if (Owner.MyVital.IsAlive)
            {
                return;
            }

            StopReturnAttack();

            if (Owner.characterAnimator != null)
            {
                if (Owner.characterAnimator.TryReceive())
                {
                    Owner.characterAnimator.AnimationReceiveWeapon();

                    if (_projectileData.Name == ProjectileNames.Proj_KillGauntletFire)
                    {
                        Owner.characterAnimator.RemovePlayingAnimation(PlayingAnimationStates.Receive);
                    }
                }
            }
        }

        public void SetDamageIndex(int damageIndex)
        {
            DamageIndex = damageIndex;

            if (AttackSystem == null)
            {
                return;
            }

            if (AttackSystem.Attack != null)
            {
                AttackSystem.Attack.SetDamageIndex(damageIndex++);
            }

            if (AttackSystem.Another != null)
            {
                AttackSystem.Another.SetDamageIndex(damageIndex++);
            }

            if (AttackSystem.Return != null)
            {
                AttackSystem.Return.SetDamageIndex(damageIndex);
            }
        }
    }
}