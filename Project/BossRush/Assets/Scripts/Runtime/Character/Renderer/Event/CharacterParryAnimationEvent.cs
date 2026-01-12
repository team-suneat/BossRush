using UnityEngine;

namespace TeamSuneat
{
    public class CharacterParryAnimationEvent : MonoBehaviour
    {
        private Character _character;

        private void Awake()
        {
            _character = this.FindFirstParentComponent<Character>();
        }

        private void CallParryAnimationEvent()
        {
            // 펄스 소모는 ParryState.OnEnter()에서 처리하므로 여기서는 제거
            // 필요시 다른 로직 추가 가능
        }
    }
}