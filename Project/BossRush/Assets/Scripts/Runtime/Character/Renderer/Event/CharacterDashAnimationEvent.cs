using UnityEngine;

namespace TeamSuneat
{
    public class CharacterDashAnimationEvent : MonoBehaviour
    {
        private Character _character;

        private void Awake()
        {
            _character = this.FindFirstParentComponent<Character>();
        }

        // 애니메이션 이벤트로 호출됩니다.
        private void CallDashAnimationEvent()
        {
            if (_character == null || _character.Physics == null) return;
            if (_character.StateMachine == null) return;
            if (_character.StateMachine.CurrentState != CharacterState.Dash) return;

            // 캐릭터가 바라보는 방향으로 대시 요청 (대시 가능 여부 체크 포함)
            _character.Physics.RequestDash();
        }
    }
}
