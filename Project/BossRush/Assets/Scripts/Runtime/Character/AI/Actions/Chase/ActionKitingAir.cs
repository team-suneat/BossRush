using NodeCanvas.Framework;

using ParadoxNotion.Design;

namespace TeamSuneat
{
    [Category("@TeamSuneat/Chase")]
    public class ActionKitingAir : ActionTask<Character>
    {
        protected override void OnExecute()
        {
            // 더 이상 사용되지 않음 - 아무것도 하지 않음
            EndAction();

            // if (agent.chaseSystem != null)
            // {
            //     agent.chaseSystem.KitingInAir();
            // }
            //
            // EndAction();
        }

        protected override string info
        {
            get
            {
                return "공중 카이팅 (사용 안 함)";
            }
        }
    }
}