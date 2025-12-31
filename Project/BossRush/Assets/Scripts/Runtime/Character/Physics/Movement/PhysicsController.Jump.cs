using TeamSuneat.Data;
using UnityEngine;

namespace TeamSuneat
{
    [RequireComponent(typeof(CollisionController))]
    [RequireComponent(typeof(Rigidbody2D))]
    public partial class PhysicsController : XBehaviour
    {
        public void SetCustomJumpVelocity(float customJumpHeight)
        {
            // 기본 점프 높이 : 기본 점프 시간 = 특수 점프 높이 : 특수 점프 시간
            // 특수 점프 시간 = 기본 점프 시간 * 특수 점프 높이 / 기본 점프 높이

            float timeJumpApex = JsonDataManager.FindGameConstantValue(GameConstantNames.DEFAULT_TIME_TO_JUMP_APEX);
            float jumpMaxHeight = JsonDataManager.FindGameConstantValue(GameConstantNames.DEFAULT_CHARACTER_JUMP_MAX_HEIGHT);
            float customTimeToJumpApex = customJumpHeight * timeJumpApex / jumpMaxHeight;

            gravity = -(2 * customJumpHeight) / Mathf.Pow(customTimeToJumpApex, 2);
            customJumpVelocity = Mathf.Abs(gravity) * customTimeToJumpApex;
        }

        public void ResetCustomJumpVelocity()
        {
            if (Owner != null)
            {
                if (Owner.Type != CharacterTypes.Player)
                {
                    float jumpMaxHeight = JsonDataManager.FindGameConstantValue(GameConstantNames.DEFAULT_CHARACTER_JUMP_MAX_HEIGHT);

                    SetCustomJumpVelocity(jumpMaxHeight);
                }
            }
        }
    }
}