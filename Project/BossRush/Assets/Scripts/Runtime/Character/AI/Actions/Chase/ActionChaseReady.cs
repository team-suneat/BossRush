using NodeCanvas.Framework;

using ParadoxNotion.Design;

namespace TeamSuneat
{
    [Category("@TeamSuneat/Chase")]
    public class ActionChaseReady : ActionTask<Character>
    {
        // private CommandInfo currentCommand = new CommandInfo();

        public float ChaseSpeedMultiplier = 1f;

        protected override void OnExecute()
        {
            // 더 이상 사용되지 않음 - 아무것도 하지 않음
            EndAction();

            // agent.characterAnimator.AnimationChase();
            //
            // EndAction();
        }

        protected override string info
        {
            get
            {
                return $"추적 준비 (사용 안 함): 속도 배율 {ChaseSpeedMultiplier:F1}";
            }
        }
    }
}