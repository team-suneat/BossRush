using NodeCanvas.Framework;

using ParadoxNotion.Design;

namespace TeamSuneat
{
    [Category("@TeamSuneat/Chase")]
    public class ActionChaseGround : ActionTask<BossCharacter>
    {
        protected override void OnExecute()
        {
            if (agent.Chase != null)
            {
                agent.Chase.ChaseInGround();
            }

            EndAction();
        }

        protected override string info
        {
            get
            {
                return "지상 추적";
            }
        }
    }
}