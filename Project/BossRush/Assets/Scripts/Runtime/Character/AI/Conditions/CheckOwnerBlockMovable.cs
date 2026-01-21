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
                    result = "캐릭터가 기절 상태입니다.";
                    return true;

                case CharacterState.Dead:
                    result = "캐릭터가 사망 상태입니다.";
                    return true;

                case CharacterState.ControlledMovement:
                    result = "캐릭터가 군중 제어 상태입니다.";
                    return true;
            }

            result = null;
            return false;
        }

        protected override string info
        {
            get
            {
                return "이동 불가 상태 확인";
            }
        }
    }
}