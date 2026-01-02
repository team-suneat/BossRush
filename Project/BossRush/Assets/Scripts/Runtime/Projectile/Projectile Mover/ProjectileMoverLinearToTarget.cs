using TeamSuneat.Data;
using UnityEngine;

namespace TeamSuneat.XProjectile
{
	public class ProjectileMoverLinearToTarget : IProjectileMover
	{
		private Character owner;

		private Projectile projectile;

		private ProjectileAssetData projectileData;

		private Vector2 direction;

		private int level;

		private float elapsedTime;

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
			this.direction = direction.normalized;
		}

		public void Setup()
		{
			projectile.SetEnabledCollider(true);

			projectile.SetTriggerCollider(true);

			projectile.ResetRigidBody();

			projectile.SetRotation(direction);
		}

		public void Move()
		{
			if (direction.IsZero())
			{
				Log.Error("이 발사체는 움직이지 않습니다. {0}", projectile.Name.ToLogString());
				return;
			}

			projectile.Translate(Vector2.right, GetMoveSpeed());
			elapsedTime += Time.fixedDeltaTime;
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