using NodeCanvas.Framework;

using ParadoxNotion.Design;

namespace TeamSuneat
{
    [Category("@TeamSuneat")]
    public class CheckOwnerBlockMovable : ConditionTask<Character>
    {
        public string result;

        protected override bool OnCheck()
        {
            if (agent == null)
            {
                result = "Character를 찾을 수 없습니다.";
                return false;
            }
            switch (agent.StateMachine.CurrentState)
            {
                case CharacterState.Stunned:
                case CharacterState.Dead:
                case CharacterState.ControlledMovement:
                    result = "FV가 적용 중입니다.";
                    return true;
            }

            result = null;
            return false;
        }
    }
}