using System.Collections.Generic;

namespace TeamSuneat
{
	[System.Serializable]
	public class XChainProjectile
	{
		public static int IssuedValue = 0;

		public static void Generate()
		{
			++IssuedValue;
		}

		public int _sid = 0;

		public int Index;

		public Character Owner;

		public AttackEntity AttackEntity;

		public List<Projectile> Projectiles = new List<Projectile>();

		public List<Vital> Targets = new List<Vital>();

		public RegisterTargetTypes RegisterTargetType;

		public int ChainCount;

		public void SetIndex(int index)
		{
			Index = index;
		}

		public void SetOwner(Character character)
		{
			Owner = character;
		}

		public void SetAttackEntity(AttackEntity entity)
		{
			AttackEntity = entity;
		}

		public void Initialize(RegisterTargetTypes registerType, int chainCount)
		{
			Generate();

			_sid = IssuedValue;

			RegisterTargetType = registerType;

			ChainCount = chainCount;
		}

		public void Reset(RegisterTargetTypes newRegisterTargetType)
		{
			_sid = 0;

			Owner = null;

			AttackEntity = null;

			Projectiles.Clear();

			Targets.Clear();

			RegisterTargetType = newRegisterTargetType;

			ChainCount = 0;
		}

		public void Replace(XChainProjectile other)
		{
			_sid = other._sid;

			Owner = other.Owner;

			AttackEntity = other.AttackEntity;

			Projectiles = other.Projectiles;

			Targets = other.Targets;

			RegisterTargetType = other.RegisterTargetType;

			ChainCount = other.ChainCount;
		}

		public void AddProjectile(Projectile projectile)
		{
			if (false == Projectiles.Contains(projectile))
			{
				Projectiles.Add(projectile);
			}
		}

		public void ClearProjectiles()
		{
			Projectiles.Clear();
		}

		public bool ContainsTarget(Vital targetVital)
		{
			return Targets.Contains(targetVital);
		}

		public Vital GetFirstTarget()
		{
			if (Targets != null && Targets.Count > 0)
			{
				return Targets[0];
			}
			else
			{
				return null;
			}
		}

		public Vital GetTarget(int index)
		{
			if (Targets != null && Targets.Count > index)
			{
				return Targets[index];
			}
			else
			{
				return null;
			}
		}

		public Vital GetLastTarget()
		{
			if (Targets != null && Targets.Count > 1)
			{
				return Targets[Targets.Count - 1];
			}
			else
			{
				return null;
			}
		}

		public void AddTargetWithRegister(Vital[] targetVitals)
		{
			for (int i = 0; i < targetVitals.Length; i++)
			{
				AddTargetWithRegister(targetVitals[i]);
			}
		}

		public void AddTargetWithRegister(Vital targetVital)
		{
			switch (RegisterTargetType)
			{
				case RegisterTargetTypes.Priority:
					{
						if (Targets.Count >= ChainCount)
						{
							ClearTargets();
						}

						AddTarget(targetVital);
					}
					break;

				case RegisterTargetTypes.Infinity:
					{
						AddTarget(targetVital);
					}
					break;
			}
		}

		public void AddTarget(Vital targetVital)
		{
			if (false == Targets.Contains(targetVital))
			{
				Targets.Add(targetVital);

				Log.Spare("Add Target {0}", targetVital.GetHierarchyPath());

				ShowTargets();
			}
		}

		public void RemoveTarget(Vital targetVital)
		{
			if (Targets.Contains(targetVital))
			{
				Targets.Remove(targetVital);

				Log.Spare("Remove Target {0}", targetVital.GetHierarchyPath());

				ShowTargets();
			}
		}

		public void ClearTargets()
		{
			Targets.Clear();
		}

		private void ShowTargets()
		{
			for (int i = 0; i < Targets.Count; i++)
			{
				Log.Spare("Show Target {0}", Targets[i].GetHierarchyPath());
			}
		}
	}

	/*
	 * AttackEntity와 Projectile이 가지는 '연쇄발사체정보'이다.

	 * Attack Projectile Entity의 Apply 함수가 호출될 때 '최초발사체'(FirstProjectile)와 '타겟등록타입'(RegisterTargetType)을 설정한다.
		: PlayerCharacter.AxeSkill.XChainProjectile

	 * Attack Projectile Entity가 생성한 Projectile에 설정한 XChainProjectile을 대체해준다.
		: PlayerCharacter.AxeSkill.XChainProjectile -> AxeSkillProjectile.XChainProjectile

	 * Projectile이 적에게 피해를 주면 피해를 입은 적을 타겟에 등록한다.
		: AxeSkillProjectile.XChainProjectile -> AxeSkillProjectile.AttackEntity.XChainProjectile

	 * Projectile의 Attack Entity가 설정한 XChainProjecile을 Projectile에게 넘겨준다.
		: AxeSkillProjectile.AttackEntity.XChainProjectile -> AxeSkillProjectile.XChainProjectile

	 * Projectile은 가지고 있는 XChainProjectile의 정보를 Another Attack Projectile Entity에게 넘겨준다.
	    : AxeSkillProjectile.XChainProjectile -> AxeSkillProjectile.AnotherAttackProjectileEntity.XChainProjectile

	 * 모든 AttackEntity와 Projectile은 XChainProjectile 정보를 디스폰될때 초기화해준다.
	 */
}