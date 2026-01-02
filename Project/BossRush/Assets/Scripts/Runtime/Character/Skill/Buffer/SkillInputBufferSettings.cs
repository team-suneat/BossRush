using Sirenix.OdinInspector;

using UnityEngine;

namespace TeamSuneat
{
    /// <summary>
    /// 기술 입력 버퍼의 설정을 관리하는 클래스입니다.
    /// </summary>
    [System.Serializable]
    public class SkillInputBufferSettings
    {
        #region Default Values Constants

        // 버퍼 설정 기본값
        private const bool DEFAULT_ENABLE_INPUT_BUFFER = true;
        private const float DEFAULT_BUFFER_DURATION = 0.35f;
        private const int DEFAULT_MAX_BUFFER_COUNT = 1;
        private const bool DEFAULT_SHOW_VISUAL_FEEDBACK = true;
        private const bool DEFAULT_ENABLE_DEBUG_LOG = false;

        // 입력 쿨타임 설정 기본값
        private const bool DEFAULT_ENABLE_INPUT_COOLDOWN = true;
        private const float DEFAULT_INPUT_COOLDOWN_DURATION = 0.1f;

        // 공중/지면 상태 관련 기본값
        private const bool DEFAULT_ENABLE_GROUND_STATE_CHECK = true;
        private const bool DEFAULT_CLEAR_AIR_BUFFER_ON_LANDING = true;
        private const bool DEFAULT_IGNORE_STATE_MISMATCH = true;

        #endregion Default Values Constants

        [FoldoutGroup("#Buffer Settings")]
        [LabelText("입력 버퍼 활성화")]
        [Tooltip("기술 입력 버퍼 시스템을 활성화합니다.")]
        public bool EnableInputBuffer;

        [FoldoutGroup("#Buffer Settings")]
        [LabelText("기본 버퍼 지속 시간")]
        [Tooltip("기술 입력이 버퍼에 저장되는 기본 지속 시간입니다.")]
        [Range(0.1f, 2.0f)]
        public float DefaultBufferDuration;

        [FoldoutGroup("#Buffer Settings")]
        [LabelText("최대 버퍼 개수")]
        [Tooltip("동시에 저장할 수 있는 최대 버퍼 개수입니다.")]
        [Range(1, 10)]
        public int MaxBufferCount;

        [FoldoutGroup("#Buffer Settings")]
        [LabelText("버퍼 시각적 피드백")]
        [Tooltip("버퍼된 입력을 UI에 시각적으로 표시합니다.")]
        public bool ShowVisualFeedback;

        [FoldoutGroup("#Buffer Settings")]
        [LabelText("버퍼 디버그 로그")]
        [Tooltip("버퍼 관련 디버그 로그를 출력합니다.")]
        public bool EnableDebugLog;

        [FoldoutGroup("#Input Cooldown Settings")]
        [LabelText("입력 중복 방지 쿨타임")]
        [Tooltip("동일한 입력이 버퍼에 중복 저장되는 것을 방지하는 쿨타임입니다.")]
        [Range(0.0f, 1.0f)]
        public float InputCooldownDuration;

        [FoldoutGroup("#Input Cooldown Settings")]
        [LabelText("입력 중복 방지 활성화")]
        [Tooltip("입력 중복 방지 쿨타임을 활성화합니다.")]
        public bool EnableInputCooldown;

        [FoldoutGroup("#Ground State Settings")]
        [LabelText("공중/지면 상태 체크 활성화")]
        [Tooltip("공중/지면 상태에 따른 버퍼 처리를 활성화합니다.")]
        public bool EnableGroundStateCheck;

        [FoldoutGroup("#Ground State Settings")]
        [LabelText("착지 시 공중 버퍼 자동 삭제")]
        [Tooltip("캐릭터가 지면에 착지할 때 공중에서 입력된 버퍼를 자동으로 삭제합니다.")]
        public bool ClearAirBufferOnLanding;

        [FoldoutGroup("#Ground State Settings")]
        [LabelText("상태 불일치 시 버퍼 무시")]
        [Tooltip("버퍼 처리 시 현재 상태와 저장된 상태가 다르면 버퍼를 무시합니다.")]
        public bool IgnoreStateMismatch;

        /// <summary>
        /// 기본 생성자 - 기본값으로 초기화합니다.
        /// </summary>
        public SkillInputBufferSettings()
        {
            SetDefaultValues();
        }

        /// <summary>
        /// 기술 범주에 따른 버퍼 우선순위를 반환합니다.
        /// </summary>
        /// <param name="actionName">액션 이름</param>
        /// <returns>우선순위 값</returns>
        public int GetPriorityForCategory(SkillCategories skillCategory)
        {
            switch (skillCategory)
            {
                case SkillCategories.Basic:
                case SkillCategories.Core:
                case SkillCategories.Assistant:
                case SkillCategories.Power:
                case SkillCategories.Ultimate:
                case SkillCategories.Dash:                
                    return skillCategory.ToInt();                    

                default:
                    return 0;
            }
        }

        /// <summary>
        /// 설정의 유효성을 검사합니다.
        /// </summary>
        /// <returns>유효하면 true</returns>
        public bool IsValid()
        {
            return DefaultBufferDuration > 0f &&
                   MaxBufferCount > 0 &&
                   MaxBufferCount <= 10 &&
                   InputCooldownDuration >= 0f;
        }

        /// <summary>
        /// 기본값으로 설정을 초기화합니다.
        /// </summary>
        [Button]
        public void SetDefaultValues()
        {
            EnableInputBuffer = DEFAULT_ENABLE_INPUT_BUFFER;
            DefaultBufferDuration = DEFAULT_BUFFER_DURATION;
            MaxBufferCount = DEFAULT_MAX_BUFFER_COUNT;
            ShowVisualFeedback = DEFAULT_SHOW_VISUAL_FEEDBACK;
            EnableDebugLog = DEFAULT_ENABLE_DEBUG_LOG;

            // 입력 중복 방지 설정
            EnableInputCooldown = DEFAULT_ENABLE_INPUT_COOLDOWN;
            InputCooldownDuration = DEFAULT_INPUT_COOLDOWN_DURATION;

            // 공중/지면 상태 관련 설정
            EnableGroundStateCheck = DEFAULT_ENABLE_GROUND_STATE_CHECK;
            ClearAirBufferOnLanding = DEFAULT_CLEAR_AIR_BUFFER_ON_LANDING;
            IgnoreStateMismatch = DEFAULT_IGNORE_STATE_MISMATCH;
        }
    }
}