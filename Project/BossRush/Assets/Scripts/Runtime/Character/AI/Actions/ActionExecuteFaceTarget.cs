using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine.UIElements;

namespace TeamSuneat
{
    [Category("@TeamSuneat/Command")]
    [Description("타겟을 바라보는 명령을 실행합니다.")]
    public class ActionExecuteFaceTarget : ActionTask<Character>
    {
        protected override void OnExecute()
        {
            agent.FaceToTarget();
            EndAction();
        }

        protected override string info
        {
            get
            {
                return "타겟을 바라보기";
            }
        }
    }
}