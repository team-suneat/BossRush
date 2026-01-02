using TeamSuneat.Data;
using UnityEngine;

namespace TeamSuneat
{
    public class ProjectileAttackSystem : XBehaviour
    {
        public Projectile Projectile;
        public AttackEntity Attack;
        public AttackEntity Another;
        public AttackEntity Return;
        public float ChargeRate;

        #region Editor

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            Projectile = this.FindFirstParentComponent<Projectile>();

            if (Projectile != null)
            {
                Caching();
            }
        }

        public override void AutoAddComponents()
        {
            base.AutoAddComponents();

            if (Projectile != null)
            {
                if (Attack == null)
                {
                    if (Projectile.AttackHitmark != HitmarkNames.None)
                    {
                        Attack = AddAttackEntity(Projectile.AttackHitmark);
                    }
                }

                if (Another == null)
                {
                    if (Projectile.AnotherHitmark != HitmarkNames.None)
                    {
                        Another = AddAttackEntity(Projectile.AnotherHitmark);
                    }
                }

                if (Return == null)
                {
                    if (Projectile.ReturnHitmark != HitmarkNames.None)
                    {
                        Return = AddAttackEntity(Projectile.ReturnHitmark);
                    }
                }
            }
        }

        public override void AutoSetting()
        {
            base.AutoSetting();

            if (Projectile != null)
            {
                Projectile.LoadProjectileData();

                if (Attack != null)
                {
                    if (Projectile.Data.Hitmark != Attack.Name)
                    {
                        Debug.LogError(this.GetHierarchyPath());
                    }
                }

                if (Another != null)
                {
                    if (Projectile.Data.AnotherHitmark != Another.Name)
                    {
                        Debug.LogError(this.GetHierarchyPath());
                    }
                }

                if (Return != null)
                {
                    if (Projectile.Data.ReturnHitmark != Return.Name)
                    {
                        Debug.LogError(this.GetHierarchyPath());
                    }
                }
            }
        }

        private AttackEntity AddAttackEntity(HitmarkNames hitmarkName)
        {
            if (hitmarkName == HitmarkNames.None)
            {
                return null;
            }

            HitmarkAssetData hitmarkData = ScriptableDataManager.Instance.FindHitmarkClone(hitmarkName);
            if (hitmarkData == null)
            {
                return null;
            }

            GameObject newGO = GameObjectEx.CreateGameObject(hitmarkName.ToString(), transform);
            switch (hitmarkData.EntityType)
            {
                case AttackEntityTypes.Target:
                    {
                        AttackTargetEntity entity = newGO.AddComponent<AttackTargetEntity>();
                        if (entity != null)
                        {
                            Log.Info(LogTags.Projectile, "Add Component - AttackTargetEntity, {0}", hitmarkName.ToLogString());
                            entity.Name = hitmarkName;
                        }
                        else
                        {
                            Log.Error("Failed to add component - AttackTargetEntity, {0}", hitmarkName.ToLogString());
                        }

                        return entity;
                    }
                case AttackEntityTypes.Area:
                    {
                        AttackAreaEntity entity = newGO.AddComponent<AttackAreaEntity>();
                        if (entity != null)
                        {
                            Log.Info(LogTags.Projectile, "Add Component - AttackAreaEntity, {0}", hitmarkName.ToLogString());
                            entity.HitmarkName = hitmarkName;
                        }
                        else
                        {
                            Log.Error("Failed to add component - AttackAreaEntity, {0}", hitmarkName.ToLogString());
                        }

                        return entity;
                    }
                case AttackEntityTypes.Projectile:
                    {
                        AttackProjectileEntity entity = newGO.AddComponent<AttackProjectileEntity>();
                        if (entity != null)
                        {
                            Log.Info(LogTags.Projectile, "Add Component - AttackProjectileEntity, {0}", hitmarkName.ToLogString());
                            entity.HitmarkName = hitmarkName;
                        }
                        else
                        {
                            Log.Error("Failed to add component - AttackProjectileEntity, {0}", hitmarkName.ToLogString());
                        }

                        return entity;
                    }

                default:
                    {
                        return null;
                    }
            }
        }

        #endregion Editor

        public void Caching()
        {
            if (Projectile != null)
            {
                CachingAttack(Projectile.AttackHitmark);

                CachingAnotherAttack(Projectile.AnotherHitmark);

                CachingReturnAttack(Projectile.ReturnHitmark);
            }
        }

        public void SetOwnerCharacter(Character ownerCharacter)
        {
            if (Attack != null)
            {
                Attack.SetOwner(ownerCharacter);
            }

            if (Another != null)
            {
                Another.SetOwner(ownerCharacter);
            }

            if (Return != null)
            {
                Return.SetOwner(ownerCharacter);
            }
        }

        public void CachingAttack(HitmarkNames hitmarkName)
        {
            if (hitmarkName == HitmarkNames.None)
            {
                return;
            }

            AttackEntity[] entities = Projectile.GetComponentsInChildren<AttackEntity>();

            if (entities != null)
            {
                for (int i = 0; i < entities.Length; i++)
                {
                    if (entities[i].Name == hitmarkName)
                    {
                        Attack = entities[i];
                    }
                }
            }
        }

        public void CachingAnotherAttack(HitmarkNames hitmarkName)
        {
            if (hitmarkName == HitmarkNames.None)
            {
                return;
            }

            AttackEntity[] entities = Projectile.GetComponentsInChildren<AttackEntity>();

            if (entities != null)
            {
                for (int i = 0; i < entities.Length; i++)
                {
                    if (entities[i].Name == hitmarkName)
                    {
                        Another = entities[i];
                    }
                }
            }
        }

        public void CachingReturnAttack(HitmarkNames hitmarkName)
        {
            if (hitmarkName == HitmarkNames.None)
            {
                return;
            }

            AttackEntity[] entities = Projectile.GetComponentsInChildren<AttackEntity>();

            if (entities != null)
            {
                for (int i = 0; i < entities.Length; i++)
                {
                    if (entities[i].Name == hitmarkName)
                    {
                        Return = entities[i];
                    }
                }
            }
        }

        public AttackMethods GetAttackMethod()
        {
            if (Attack != null)
            {
                return Attack.AttackMethod;
            }

            return AttackMethods.None;
        }

        public AttackEntityTypes GetAttackType()
        {
            if (Attack is AttackTargetEntity)
            {
                return AttackEntityTypes.Target;
            }
            else if (Attack is AttackAreaEntity)
            {
                return AttackEntityTypes.Area;
            }
            else if (Attack is AttackProjectileEntity)
            {
                return AttackEntityTypes.Projectile;
            }

            return AttackEntityTypes.None;
        }

        public bool CheckReturnProjectile()
        {
            return Projectile.ProjectileType == ProjectileTypes.Return;
        }

        public void SetAttackChainProjectileInfo(XChainProjectile ChainProjectileInfo)
        {
            if (Attack != null)
            {
                Attack.SetAttackEntityChainProjectileInfo(ChainProjectileInfo);

                Log.Info(LogTags.Projectile, "발사체의 공격엔티티의 연쇄발사체 정보를 설정한다. {0}", Attack.GetHierarchyPath());

                for (int i = 0; i < ChainProjectileInfo.Targets.Count; i++)
                {
                    Log.Info(LogTags.Projectile, "발사체의 공격엔티티의 연쇄발사체 정보가 가진 {0}번째 타겟. {1}", i + 1, ChainProjectileInfo.Targets[i].GetHierarchyPath());
                }
            }
        }

        public void SetAnotherChainProjectileInfo(XChainProjectile ChainProjectileInfo)
        {
            if (Another != null)
            {
                Another.SetAttackEntityChainProjectileInfo(ChainProjectileInfo);

                Log.Info(LogTags.Projectile, "발사체의 추가엔티티의 연쇄발사체 정보를 설정한다. {0}", Another.GetHierarchyPath());

                for (int i = 0; i < ChainProjectileInfo.Targets.Count; i++)
                {
                    Log.Info(LogTags.Projectile, "발사체의 추가엔티티의 연쇄발사체 정보가 가진 {0}번째 타겟. {1}", i + 1, ChainProjectileInfo.Targets[i].GetHierarchyPath());
                }
            }
        }

        public void SetReturnChainProjectileInfo(XChainProjectile ChainProjectileInfo)
        {
            if (Return != null)
            {
                Return.SetAttackEntityChainProjectileInfo(ChainProjectileInfo);

                Log.Info(LogTags.Projectile, "발사체의 반환엔티티의 연쇄발사체 정보를 설정한다. {0}", Return.GetHierarchyPath());

                for (int i = 0; i < ChainProjectileInfo.Targets.Count; i++)
                {
                    Log.Info(LogTags.Projectile, "발사체의 반환엔티티의 연쇄발사체 정보가 가진 {0}번째 타겟. {1}", i + 1, ChainProjectileInfo.Targets[i].GetHierarchyPath());
                }
            }
        }

        public void SetAttackCustomDamage(int customDamage)
        {
            if (Attack != null)
            {
                Attack.SetCustomDamage(customDamage);
            }
        }

        public void AddAttackTarget(Vital targetVital)
        {
            if (Attack != null)
            {
                Attack.AddTarget(targetVital);
            }
        }

        public void Initialize()
        {
            if (Attack != null)
            {
                Attack.Initialize();
            }

            if (Another != null)
            {
                Another.Initialize();
            }

            if (Return != null)
            {
                Return.Initialize();
            }
        }

        public void SetAnotherCount(int count)
        {
            if (Attack != null)
            {
                Attack.SetAnotherCount(count);
            }

            if (Another != null)
            {
                Another.SetAnotherCount(count);
            }

            if (Return != null)
            {
                Return.SetAnotherCount(count);
            }
        }

        public void SetAnotherMaxCount(int maxCount)
        {
            if (Attack != null)
            {
                Attack.SetAnotherMaxCount(maxCount);
            }

            if (Another != null)
            {
                Another.SetAnotherMaxCount(maxCount);
            }

            if (Return != null)
            {
                Return.SetAnotherMaxCount(maxCount);
            }
        }

        public void DoAttack()
        {
            if (Attack != null)
            {
                Attack.SetProjectile(Projectile);

                Attack.SetChargeRate(ChargeRate);

                Attack.Activate();

                Log.Info(LogTags.Projectile, "발사체가 공격합니다. {0}, hitmark:{1}", Projectile.Name.ToLogString(), Attack.Name.ToLogString());
            }
        }

        public bool ExecuteAnother()
        {
            if (Another != null)
            {
                return Another.Execute();
            }

            return false;
        }

        public bool ExecuteReturn()
        {
            if (Return != null)
            {
                Return.ResetDamageIndex();

                return Return.Execute();
            }

            return false;
        }

        public void StopAttack()
        {
            if (Attack != null)
            {
                Attack.Deactivate();

                Attack.ClearTargets();
            }
        }

        public void StopAnother()
        {
            if (Another != null)
            {
                Another.Deactivate();

                Another.ClearTargets();
            }
        }

        public void StopReturnAttack()
        {
            if (Return != null)
            {
                Return.Deactivate();

                Return.ClearTargets();
            }
        }
    }
}