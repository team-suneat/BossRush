using TeamSuneat.Data;
using UnityEngine;

namespace TeamSuneat.XProjectile
{
    public class ProjectileMoverHomingLinear : IProjectileMover
    {
        private Character owner;
        private Vital targetVital;
        private Projectile projectile;
        private ProjectileAssetData projectileData;
        private Vector2 direction;
        private int level;
        private float elapsedTime;

        public void SetOwner(Character owner)
        {
            this.owner = owner;
        }

        public void SetTarget(Vital targetVital)
        {
            this.targetVital = targetVital;
        }

        public void SetProjectile(Projectile projectile)
        {
            this.projectile = projectile;
            projectileData = ScriptableDataManager.Instance.FindProjectileClone(projectile.Name);
        }

        public void SetLevel(int level)
        {
            this.level = level;
        }

        public void SetDirection(Vector2 direction)
        {
            this.direction = VectorEx.Normalize(direction);
        }

        public void Setup()
        {
            projectile.SetEnabledCollider(true);
            projectile.SetTriggerCollider(true);
            projectile.ResetRigidBody();

            if (targetVital != null && targetVital.IsAlive)
            {
                direction = VectorEx.Normalize(targetVital.position - projectile.position);
                projectile.SetRotation(direction);
            }
        }

        public void Move()
        {
            if (targetVital != null && targetVital.IsAlive)
            {
                direction = VectorEx.Normalize(targetVital.position - projectile.position);

                projectile.SetRotation(direction);
                projectile.Translate(Vector2.right, GetMoveSpeed());

                elapsedTime += Time.fixedDeltaTime;
            }
            else
            {
                projectile.ForceApplyReturn();
                projectile.Destroy();
            }
        }

        public float GetMoveSpeed()
        {
            float additionalSpeed = 0f;

            if (false == Mathf.Approximately(0f, projectileData.Acceleration))
            {
                additionalSpeed = projectileData.Acceleration * elapsedTime;
            }

            if (level > 1)
            {
                for (int i = 0; i < projectileData.AddSpeed.Length; i++)
                {
                    additionalSpeed += projectileData.AddSpeed[i];
                }
            }

            if (owner != null)
            {
                if (projectile.Name == ProjectileNames.Proj_ChainRake)
                {
                    additionalSpeed += owner.statSystem.FindValue(StatNames.ChainRakeSpeed);
                }
            }

            return projectileData.Speed + additionalSpeed;
        }
    }
}