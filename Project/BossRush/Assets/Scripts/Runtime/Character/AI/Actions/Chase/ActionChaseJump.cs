using NodeCanvas.Framework;

using ParadoxNotion.Design;

namespace TeamSuneat
{
    [Category("@TeamSuneat/Chase")]
    public class ActionChaseJump : ActionTask<Character>
    {
        protected override void OnExecute()
        {
            // 더 이상 사용되지 않음 - 아무것도 하지 않음
            EndAction();
        }
    }
}