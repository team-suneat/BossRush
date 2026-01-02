using TeamSuneat.Data;
using UnityEngine;

namespace TeamSuneat.XProjectile
{
    public class ProjectileMoverLinear : IProjectileMover
    {
        private Character owner;
        private Projectile projectile;
        private ProjectileAssetData projectileData;
        private Vector2 direction;
        private int level;
        private float elapsedTime;
        private float moveSpeed;

        public void SetOwner(Character owner)
        {
            this.owner = owner;
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

            if (direction == Vector2.left || direction == Vector2.right)
            {
                projectile.RefreshRendererFlip();
                projectile.SetRendererRotation(direction);
            }
            else
            {
                projectile.BlockFlip();
                projectile.ResetFlip();
                projectile.SetRotation(direction);
            }
        }

        public void Move()
        {
            moveSpeed = GetMoveSpeed();

            if (direction.IsZero() || moveSpeed.IsZero())
            {
                Log.Error("이 발사체는 움직이지 않습니다. {0}", projectile.Name.ToLogString());

                projectile.ForceApplyReturn();

                projectile.Destroy();
            }
            else
            {
                if (direction == Vector2.left || direction == Vector2.right)
                {
                    projectile.Translate(direction, moveSpeed);
                }
                else
                {
                    projectile.Translate(Vector2.right, moveSpeed);
                }

                elapsedTime += Time.fixedDeltaTime;
            }
        }

        public float GetMoveSpeed()
        {
            float additionalSpeed = 0f;

            if (false == Mathf.Approximately(0f, projectileData.Acceleration))
            {
                additionalSpeed = projectileData.Acceleration * elapsedTime;
            }

            return projectileData.Speed + additionalSpeed;
        }
    }
}