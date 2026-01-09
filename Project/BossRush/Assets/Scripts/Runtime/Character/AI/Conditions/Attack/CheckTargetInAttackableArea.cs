using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace TeamSuneat
{
	[Category("@TeamSuneat/Attack")]
	[Description("타겟이 공격가능한 영역 안에 있는지 확인합니다.")]
	public class CheckTargetInAttackableArea : ConditionTask<Character>
	{
        public string hitmarkNameString;

		protected override bool OnCheck()
		{
			if (agent.Attack != null)
			{
                HitmarkNames hitmarkName = DataConverter.ToEnum<HitmarkNames>(hitmarkNameString);
                if (hitmarkName != HitmarkNames.None)
                {
                    if (agent.Attack.CheckTargetInAttackableArea(hitmarkName))
					{
						return true;
					}
                }
			}

			return false;
		}
	}
}