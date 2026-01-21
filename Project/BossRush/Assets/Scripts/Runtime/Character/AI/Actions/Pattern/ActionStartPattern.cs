using NodeCanvas.Framework;

using ParadoxNotion.Design;

namespace TeamSuneat
{
    [Category("@TeamSuneat/Pattern")]
    public class ActionStartPattern : ActionTask<BossCharacter>
    {
        protected override void OnExecute()
        {
             if (agent.Pattern != null)
             {
                 agent.Pattern.StartPattern();
             }
        }

        protected override string info
        {
            get
            {
                return "패턴 시작";
            }
        }
    }
}