using TeamSuneat.Data;
using UnityEngine;

namespace TeamSuneat.XProjectile
{
    public class ProjectileMoverPhysics : IProjectileMover
    {
        private Projectile projectile;

        private ProjectileAssetData projectileData;

        private XRigidbody body;

        private bool facingRight;

        public void SetProjectile(Projectile projectile)
        {
            this.projectile = projectile;
            body = projectile.Body;
            projectileData = ScriptableDataManager.Instance.FindProjectileClone(projectile.Name);
        }

        public void SetFacingRight(bool facingRight)
        {
            this.facingRight = facingRight;
        }

        public void Setup()
        {
            projectile.SetEnabledCollider(true);

            if (body != null)
            {
                body.SetBodyType(RigidbodyType2D.Dynamic);
                body.SetGravity(GameDefine.DEFAULT_PHYSICS_DEFAULT_GRAVITY_IN_RIGIDBODY * projectileData.GravityRate);
                body.SetVelocity(GetForce() * projectileData.Speed);

                SetMaxMagnitude();
            }

            projectile.SetFlip(facingRight);
        }

        public void Move()
        {
            if (projectile.Animator != null)
            {
                projectile.Animator.SetAnimatorSpeed(body.Velocity);
            }

            if (projectile.Renderer != null)
            {
                projectile.Renderer.SetFlipByDynimic(body.Velocity.x);
            }
        }

        private void SetMaxMagnitude()
        {
            if (projectile.Animator != null)
            {
                projectile.Animator.SetMaxMagnitude(body.Velocity.magnitude);
            }
        }

        private Vector2 GetForce()
        {
            if (projectileData.RigidMaxForce.IsZero())
            {
                return VectorEx.ApplyFacingRight(projectileData.RigidForce, facingRight);
            }

            float forceX = RandomEx.Range(projectileData.RigidForce.x, projectileData.RigidMaxForce.x);
            float forceY = RandomEx.Range(projectileData.RigidForce.y, projectileData.RigidMaxForce.y);

            if (projectileData.RigidForce.x > 0)
            {
                forceX = facingRight ? Mathf.Abs(forceX) : -Mathf.Abs(forceX);
            }
            else if (projectileData.RigidForce.x < 0)
            {
                forceX = facingRight ? -Mathf.Abs(forceX) : Mathf.Abs(forceX);
            }

            return new Vector2(forceX, forceY);
        }
    }
}