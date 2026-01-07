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
            // 더 이상 사용되지 않음 - 항상 false 반환
            return false;

            // if (agent == null)
            // {
            //     result = "Character를 찾을 수 없습니다.";
            //
            //     return false;
            // }
            //
            // if (agent.buffSystem != null)
            // {
            //     if (agent.buffSystem.IsBlocked(BuffState.Move))
            //     {
            //         result = "움직임을 금지하는 버프가 설정되어있습니다.";
            //
            //         return true;
            //     }
            // }
            //
            // if (agent.CheckAppliedAnyForceVelocity())
            // {
            //     result = "FV가 적용 중입니다.";
            //
            //     return true;
            // }
            //
            // if (GameOption.IsLockCharacterInput)
            // {
            //     result = "캐릭터의 입력이 금지되어있습니다.";
            //
            //     return true;
            // }
            //
            // result = null;
            //
            // return false;
        }
    }
}