using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace TeamSuneat
{
	public class XIgnoreTargets
	{
		[ReadOnly] public Vital IgnoreTarget;

		[ReadOnly] public List<Vital> IgnoreTargets = new List<Vital>();

		public void ClearIgnoreTargets()
		{
			IgnoreTarget = null;
			IgnoreTargets.Clear();
		}

		public bool CheckIgnoreTarget(Vital target)
		{
			if (IgnoreTarget == target)
				return true;

			if (IgnoreTargets.Contains(target))
				return true;

			return false;
		}

		public void AddIgnoreTarget(Vital newIgnoreTarget)
		{
			if (false == IgnoreTargets.Contains(newIgnoreTarget))
			{
				IgnoreTargets.Add(newIgnoreTarget);
			}
		}

		public void SetIgnoreTarget(Vital newIgnoreTarget)
		{
			IgnoreTarget = newIgnoreTarget;
		}
	}
}