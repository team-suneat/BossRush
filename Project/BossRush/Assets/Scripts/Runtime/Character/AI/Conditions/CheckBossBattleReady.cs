using NodeCanvas.Framework;

using ParadoxNotion.Design;

namespace TeamSuneat
{
    [Category("@TeamSuneat/Boss")]
    public class CheckBossBattleReady : ConditionTask<BossCharacter>
    {
        protected override bool OnCheck()
        {
            if (agent.IsBattleReady)
            {
                return true;
            }

            return false;
        }

        protected override string info
        {
            get
            {
                return "보스 전투 준비 상태 확인";
            }
        }
    }
}