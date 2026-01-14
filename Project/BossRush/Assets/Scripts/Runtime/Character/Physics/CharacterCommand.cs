using UnityEngine;

namespace TeamSuneat
{
    public class CharacterCommand
    {
        // 이동 입력
        public float HorizontalInput;
        public float VerticalInput;
        public bool IsDownInputPressed;

        // 한 프레임만 true인 입력
        public bool IsJumpPressed;
        public bool IsJumpReleased;
        public bool IsDashPressed;
        public bool IsAttackPressed;
        public bool IsParryPressed;

        // 이전 프레임 상태 (한 프레임만 true 처리용)
        private bool _wasJumpPressed;
        private bool _wasDashPressed;
        private bool _wasAttackPressed;
        private bool _wasParryPressed;

        // 공격 입력 버퍼
        public bool IsAttackBuffered { get; private set; }

        private float _attackBufferEndTime;
        private const float ATTACK_BUFFER_DURATION = 0.15f;

        public void SetHorizontalInput(float value)
        {
            HorizontalInput = value;
        }

        public void SetVerticalInput(float value)
        {
            VerticalInput = value;
        }

        public void SetDownInputPressed(bool value)
        {
            IsDownInputPressed = value;
        }

        public void SetJumpPressed(bool value)
        {
            if (value && !_wasJumpPressed)
            {
                IsJumpPressed = true;
            }
            else if (!value)
            {
                IsJumpPressed = false;
            }
            _wasJumpPressed = value;
        }

        public void SetJumpReleased(bool value)
        {
            IsJumpReleased = value;
        }

        public void SetDashPressed(bool value)
        {
            if (value && !_wasDashPressed)
            {
                IsDashPressed = true;
            }
            else if (!value)
            {
                IsDashPressed = false;
            }
            _wasDashPressed = value;
        }

        public void SetAttackPressed(bool value)
        {
            if (value && !_wasAttackPressed)
            {
                IsAttackPressed = true;

                // 공격 입력 버퍼 시작
                IsAttackBuffered = true;
                _attackBufferEndTime = Time.time + ATTACK_BUFFER_DURATION;
                Log.Info(LogTags.Input_Command, "[공격 입력 버퍼] 버퍼 시작. 만료 시간: {0:F3}초 후", ATTACK_BUFFER_DURATION);
            }
            else if (!value)
            {
                IsAttackPressed = false;
            }
            _wasAttackPressed = value;
        }

        public void SetParryPressed(bool value)
        {
            if (value && !_wasParryPressed)
            {
                IsParryPressed = true;
            }
            else if (!value)
            {
                IsParryPressed = false;
            }
            _wasParryPressed = value;
        }

        // 버퍼 업데이트 (매 프레임 호출)
        public void UpdateBuffer()
        {
            // 버퍼 만료 체크 (만료만 담당)
            if (IsAttackBuffered && Time.time > _attackBufferEndTime)
            {
                IsAttackBuffered = false;
                Log.Info(LogTags.Input_Command, "[공격 입력 버퍼] 버퍼 만료. 시간 초과로 무시됨");
            }
        }

        // 버퍼 소비
        public void ConsumeAttackBuffer()
        {
            if (IsAttackBuffered)
            {
                IsAttackBuffered = false;
                Log.Info(LogTags.Input_Command, "[공격 입력 버퍼] 버퍼 소비됨. 공격 실행");
            }
        }

        public void ResetFrame()
        {
            // 한 프레임만 true인 값들 초기화
            IsJumpPressed = false;
            IsJumpReleased = false;
            IsDashPressed = false;
            IsAttackPressed = false;
            IsParryPressed = false;

            // 이전 프레임 상태도 리셋 (다음 프레임에서 새로운 입력 감지 가능하도록)
            _wasJumpPressed = false;
            _wasDashPressed = false;
            _wasAttackPressed = false;
            _wasParryPressed = false;

            // 공격 버퍼(IsAttackBuffered)는 여기서 건드리지 않음
            // UpdateBuffer()와 ConsumeAttackBuffer()로만 관리
        }

        public void Reset()
        {
            HorizontalInput = 0f;
            VerticalInput = 0f;
            IsDownInputPressed = false;
            IsJumpPressed = false;
            IsJumpReleased = false;
            IsDashPressed = false;
            IsAttackPressed = false;
            IsParryPressed = false;
            _wasJumpPressed = false;
            _wasDashPressed = false;
            _wasAttackPressed = false;
            _wasParryPressed = false;

            IsAttackBuffered = false;
            _attackBufferEndTime = 0f;
        }
    }
}
