using UnityEngine;

namespace TeamSuneat.XProjectile
{
	public class ProjectileMoverOnTheSpot : IProjectileMover
	{
		private Projectile projectile;

		private float elapsedTime;

		public void SetProjectile(Projectile projectile)
		{
			this.projectile = projectile;
		}

		public void Setup()
		{
			if (projectile != null)
			{
				projectile.SetEnabledCollider(true);
				projectile.SetTriggerCollider(true);
				projectile.ResetRigidBody();
				projectile.RefreshRendererFlip();
			}
		}

		public void Move()
		{
			elapsedTime += Time.fixedDeltaTime;
		}
	}
}