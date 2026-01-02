using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat.Data
{
    public enum JumpTypes
    {
        None,
        PlayerInput,
        Attack,
        Track,
        TargetPoint,
    }

    [System.Serializable]
    public class CharacterJumpData
    {
        [Tooltip("점프를 방식을 구분 짓기 위한 enum")]
        public JumpTypes Type;

        [Tooltip("점프 중 플랫폼의 충돌을 무시할지 여부 검사")]
        public bool IgonorePlatform = false;

        [ShowIf("IgonorePlatform", true)]
        [Tooltip("점프 중 플랫폼의 충돌을 무시하는 시간")]
        public float IgonrePlatformTime = 0;

        #region Point

        [ShowIf("Type", JumpTypes.TargetPoint)]
        [Tooltip("지점 점프까지의 수평힘의 배율 정의합니다.")]
        public float HorizontalPointForceRate = 1;

        #endregion Point

        [Space]
        [Tooltip("허용되는 최대 점프 수(0: 점프 없음, 1: 일반 점프, 2: 이중 점프 등...)")]
        public int NumberOfJumps;

        [Tooltip("캐릭터가 점프할 수 있는 높이를 정의합니다.")]
        public float JumpHeight;

        #region Horizontal

        [Header("HorizontalForce")]
        [Tooltip("점프 중 수평이동 여부를 정의합니다.")]
        public bool IsHorizontalForce = false;

        [Tooltip("True일 경우 앞으로, false 경우 뒤로 힘을 가합니다.")]
        [ShowIfGroup("IsHorizontalForce", true)]
        public bool IsForwardForce = true;

        [Tooltip("캐릭터가 점프할 수 있는 수평이동 힘을 정의합니다.")]
        [ShowIfGroup("IsHorizontalForce", true)]
        public float HorizontalForce;

        [Tooltip("캐릭터가 수평이동하는 시간을 정의합니다.")]
        [ShowIfGroup("IsHorizontalForce", true)]
        public float HorizontalForceTime;

        [Tooltip("캐릭터가 수평이동 중 플랫폼에 닿으면 강제로 힘을 중지 시킬지 여부를 정의합니다.")]
        [ShowIfGroup("IsHorizontalForce", true)]
        public bool StopHorizontalGrounded = true;

        #endregion Horizontal

        #region PlayerInput

        [Header("PlayerInput")]
        [ShowIf("Type", JumpTypes.PlayerInput)]
        [Tooltip("이것이 사실이라면 점프 시 카메라 오프셋이 재설정됩니다.")]
        public bool ResetCameraOffsetOnJump = false;

        [ShowIf("Type", JumpTypes.PlayerInput)]
        [Tooltip("이것이 사실이라면 이 캐릭터는 아래로 + 점프를 수행하여 플랫폼 아래로 단방향으로 점프할 수 있습니다.")]
        public bool CanJumpDownOneWayPlatforms = true;

        [ShowIf("Type", JumpTypes.PlayerInput)]
        [Tooltip("이것이 사실이라면 점프 지속 시간/높이는 버튼을 누른 시간에 비례합니다.")]
        public bool JumpIsProportionalToThePressTime = true;

        [ShowIf("Type", JumpTypes.PlayerInput)]
        [Tooltip("점프할 때 허용되는 최소 공중 시간 - 압력 제어 점프에 사용됩니다.")]
        public float JumpMinimumAirTime;

        [ShowIf("Type", JumpTypes.PlayerInput)]
        [Tooltip("점프 버튼을 놓을 때 현재 속도를 수정할 양")]
        public float JumpReleaseForceFactor;

        [ShowIf("Type", JumpTypes.PlayerInput)]
        [Tooltip("지면을 떠난 후에도 캐릭터가 여전히 점프를 트리거할 수 있는 기간")]
        public float CoyoteTime;

        [ShowIf("Type", JumpTypes.PlayerInput)]
        [Tooltip("캐릭터가 착지하고 해당 InputBufferDuration 동안 점프 버튼을 누르면 새로운 점프가 트리거됩니다.")]
        public float InputBufferDuration;

        #endregion PlayerInput

        [Tooltip("지속 시간(초) 단방향 플랫폼에서 점프할 때 충돌을 비활성화해야 합니다.")]
        public float OneWayPlatformsJumpCollisionOffDuration;

        [Tooltip("지속 시간(초) 움직이는 플랫폼에서 뛰어내릴 때 충돌을 비활성화해야 합니다.")]
        public float MovingPlatformsJumpCollisionOffDuration;

        public CharacterJumpData Clone()
        {
            return new CharacterJumpData()
            {
                HorizontalPointForceRate = HorizontalPointForceRate,
                NumberOfJumps = NumberOfJumps,
                JumpHeight = JumpHeight,
                IsHorizontalForce = IsHorizontalForce,
                IsForwardForce = IsForwardForce,
                HorizontalForce = HorizontalForce,
                HorizontalForceTime = HorizontalForceTime,
                ResetCameraOffsetOnJump = ResetCameraOffsetOnJump,
                CanJumpDownOneWayPlatforms = CanJumpDownOneWayPlatforms,
                JumpIsProportionalToThePressTime = JumpIsProportionalToThePressTime,
                JumpMinimumAirTime = JumpMinimumAirTime,
                JumpReleaseForceFactor = JumpReleaseForceFactor,
                CoyoteTime = CoyoteTime,
                InputBufferDuration = InputBufferDuration,
                OneWayPlatformsJumpCollisionOffDuration = OneWayPlatformsJumpCollisionOffDuration,
                MovingPlatformsJumpCollisionOffDuration = MovingPlatformsJumpCollisionOffDuration,
            };
        }

        public void Paste(CharacterJumpData data)
        {
            data.HorizontalPointForceRate = HorizontalPointForceRate;
            data.NumberOfJumps = NumberOfJumps;
            data.JumpHeight = JumpHeight;
            data.IsHorizontalForce = IsHorizontalForce;
            data.IsForwardForce = IsForwardForce;
            data.HorizontalForceTime = HorizontalForceTime;
            data.ResetCameraOffsetOnJump = ResetCameraOffsetOnJump;
            data.CanJumpDownOneWayPlatforms = CanJumpDownOneWayPlatforms;
            data.JumpIsProportionalToThePressTime = JumpIsProportionalToThePressTime;
            data.JumpMinimumAirTime = JumpMinimumAirTime;
            data.JumpReleaseForceFactor = JumpReleaseForceFactor;
            data.CoyoteTime = CoyoteTime;
            data.InputBufferDuration = InputBufferDuration;
            data.OneWayPlatformsJumpCollisionOffDuration = OneWayPlatformsJumpCollisionOffDuration;
            data.MovingPlatformsJumpCollisionOffDuration = MovingPlatformsJumpCollisionOffDuration;
        }
    }
}