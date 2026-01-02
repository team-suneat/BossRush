using System.Collections;
using System.Text;
using TeamSuneat.Data;

using UnityEngine;

namespace TeamSuneat
{
    public partial class SkillSystem : XBehaviour
    {
        #region Buffer Fields

        /// <summary> 기술 입력 버퍼 매니저 </summary>
        private SkillInputBufferManager _bufferManager = new();

        [SerializeField]
        private SkillInputBufferSettings _bufferSettings = new SkillInputBufferSettings();

        public SkillInputBufferSettings BufferSettings => _bufferSettings;

        /// <summary> 마지막 버퍼 정리 시간 </summary>
        private float _lastCleanupTime = 0f;

        /// <summary> 버퍼 정리 간격 (초) </summary>
        private const float CLEANUP_INTERVAL = 1.0f;

        /// <summary> 버퍼 처리 중복 방지 플래그 </summary>
        private bool _isProcessingBuffer = false;

        #endregion Buffer Fields

        #region Buffer Initialization

        /// <summary>
        /// 버퍼 시스템을 초기화합니다.
        /// </summary>
        private void InitializeBufferSystem()
        {
            // 설정이 유효하지 않으면 기본값 사용
            if (!BufferSettings.IsValid())
            {
                BufferSettings.SetDefaultValues();
                Log.Warning(LogTags.Skill_Buffer, "버퍼 설정이 유효하지 않아 기본값을 사용합니다.");
            }

            // 버퍼 매니저에 설정 전달
            _bufferManager.UpdateSettings(BufferSettings.DefaultBufferDuration, BufferSettings.MaxBufferCount);

            // 입력 쿨타임 설정 전달
            _bufferManager.UpdateCooldownSettings(
                BufferSettings.EnableInputCooldown,
                BufferSettings.InputCooldownDuration
            );

            Log.Progress(LogTags.Skill_Buffer, "기술 입력 버퍼 시스템을 초기화했습니다. 활성화: {0}, 기본 지속시간: {1:F2}s, 입력 쿨타임: {2:F2}s",
                BufferSettings.EnableInputBuffer, BufferSettings.DefaultBufferDuration, BufferSettings.InputCooldownDuration);
        }

        #endregion Buffer Initialization

        #region Buffer Accessors

        /// <summary>
        /// 버퍼 매니저를 반환합니다.
        /// </summary>
        /// <returns>버퍼 매니저</returns>
        public SkillInputBufferManager GetBufferManager()
        {
            return _bufferManager;
        }

        /// <summary>
        /// 버퍼 시스템이 활성화되어 있는지 확인합니다.
        /// </summary>
        /// <returns>활성화되어 있으면 true</returns>
        public bool IsBufferSystemEnabled()
        {
            return BufferSettings?.EnableInputBuffer == true && _bufferManager != null;
        }

        #endregion Buffer Accessors

        #region Buffer Processing

        /// <summary>
        /// 버퍼에 저장된 입력을 처리합니다.
        /// </summary>
        public void ProcessBufferedInput()
        {
            if (!IsBufferSystemEnabled())
            {
                Log.Progress(LogTags.Skill_Buffer, "버퍼 시스템이 비활성화되어 있습니다.");
                return;
            }

            // 중복 처리 방지
            if (_isProcessingBuffer)
            {
                Log.Warning(LogTags.Skill_Buffer, "버퍼 처리가 이미 진행 중입니다.");
                return;
            }

            _isProcessingBuffer = true;

            try
            {
                // 애니메이션 상태 확인 및 로깅
                bool isPlayingAnimation = Owner.CharacterAnimator.CheckPlayingSkillAnimation();
                string currentAnimationName = Owner.CharacterAnimator.PlayingSkillAnimationName;

                Log.Progress(LogTags.Skill_Buffer, "버퍼 처리 시작 - 애니메이션 재생 중: {0}, 현재 애니메이션: {1}",
                    isPlayingAnimation, string.IsNullOrEmpty(currentAnimationName) ? "None" : currentAnimationName);

                // 안전 장치: 애니메이션이 재생 중이면 버퍼 처리를 건너뜀
                if (isPlayingAnimation)
                {
                    Log.Warning(LogTags.Skill_Buffer, "애니메이션이 아직 재생 중이므로 버퍼 처리를 건너뜁니다. 애니메이션: {0}",
                        currentAnimationName);
                    return;
                }

                // 만료된 버퍼 정리
                _bufferManager.CleanupExpiredBuffers();

                // 버퍼 상태 로깅
                Log.Progress(LogTags.Skill_Buffer, "현재 버퍼 개수: {0}", _bufferManager.BufferCount);

                // 버퍼에서 액션 가져오기
                ActionNames? bufferedAction = null;

                // 공중/지면 상태 체크가 활성화되어 있으면 상태를 고려한 버퍼 처리
                if (BufferSettings.EnableGroundStateCheck)
                {
                    bool currentIsGrounded = Owner.Controller.State.IsGrounded;

                    if (BufferSettings.IgnoreStateMismatch)
                    {
                        // 상태 불일치 시 무시 모드: 현재 상태와 일치하는 버퍼만 처리
                        bufferedAction = _bufferManager.GetBufferedActionWithStateCheck(currentIsGrounded);

                        if (bufferedAction.HasValue)
                        {
                            string groundState = currentIsGrounded ? "지면" : "공중";
                            Log.Progress(LogTags.Skill_Buffer, "상태 일치 버퍼 처리: {0} (현재 상태: {1})",
                                bufferedAction.Value.ToLogString(), groundState);
                        }
                    }
                    else
                    {
                        // 기존 방식: 모든 버퍼 처리
                        bufferedAction = _bufferManager.GetBufferedAction();
                    }
                }
                else
                {
                    // 공중/지면 상태 체크 비활성화: 기존 방식
                    bufferedAction = _bufferManager.GetBufferedAction();
                }

                // 우선순위 캔슬 체크: 현재 재생 중인 기술보다 높은 우선순위의 버퍼가 있는지 확인
                if (!bufferedAction.HasValue)
                {
                    int currentPriority = GetCurrentPlayingSkillPriority();
                    if (currentPriority > 0 && _bufferManager.HasCancellableBuffer(currentPriority))
                    {
                        bufferedAction = _bufferManager.GetCancellableBuffer(currentPriority);
                        if (bufferedAction.HasValue)
                        {
                            Log.Progress(LogTags.Skill_Buffer, "우선순위 캔슬로 버퍼 처리: {0}", bufferedAction.Value.ToLogString());

                            // 현재 애니메이션 중단 (캔슬)
                            Owner.CharacterAnimator.StopPlayingSkillAnimation(false);
                        }
                    }
                }

                if (bufferedAction.HasValue)
                {
                    // 액션에 해당하는 기술 찾기
                    SkillNames skillName = CharacterSkillInfo.FindName(bufferedAction.Value);
                    if (skillName != SkillNames.None)
                    {
                        Log.Progress(LogTags.Skill_Buffer, "버퍼된 입력 처리: {0} ▶ {1}", bufferedAction.Value.ToLogString(), skillName.ToLogString());

                        // 재시전 취소 로직을 고려한 기술 시전 시도
                        if (CheckAnimationConditionsForCast(skillName) && TryCast(skillName))
                        {
                            Log.Progress(LogTags.Skill_Buffer, "버퍼된 기술 시전 성공: {0}", skillName.ToLogString());
                            DoCast(skillName);
                        }
                        else
                        {
                            Log.Warning(LogTags.Skill_Buffer, "버퍼된 입력으로 기술 시전 실패: {0}", skillName.ToLogString());
                        }
                    }
                    else
                    {
                        Log.Warning(LogTags.Skill_Buffer, "버퍼된 액션에 해당하는 기술을 찾을 수 없습니다: {0}", bufferedAction.Value.ToLogString());
                    }
                }
                else
                {
                    Log.Progress(LogTags.Skill_Buffer, "처리할 버퍼가 없습니다.");
                }
            }
            finally
            {
                _isProcessingBuffer = false;
                Log.Progress(LogTags.Skill_Buffer, "버퍼 처리 완료");
            }
        }

        /// <summary>
        /// 애니메이션 종료 시 버퍼 처리를 트리거합니다.
        /// </summary>
        public void TriggerBufferedInputProcessing()
        {
            if (IsBufferSystemEnabled())
            {
                // 다음 프레임에 버퍼 처리
                _ = StartCoroutine(ProcessBufferedInputNextFrame());
            }
        }

        /// <summary>
        /// 다음 프레임에 버퍼 처리를 수행합니다.
        /// </summary>
        private IEnumerator ProcessBufferedInputNextFrame()
        {
            yield return null; // 다음 프레임까지 대기

            // 버퍼 처리
            ProcessBufferedInput();
        }

        #endregion Buffer Processing

        #region Buffer Maintenance

        /// <summary>
        /// 주기적으로 버퍼를 정리합니다.
        /// </summary>
        private void UpdateBufferCleanup()
        {
            if (!IsBufferSystemEnabled())
            {
                return;
            }

            // 주기적으로 만료된 버퍼 정리
            if (Time.time - _lastCleanupTime > CLEANUP_INTERVAL)
            {
                _bufferManager.CleanupExpiredBuffers();
                _lastCleanupTime = Time.time;
            }
        }

        /// <summary>
        /// Update에서 호출되는 버퍼 정리 메서드
        /// </summary>
        private void Update()
        {
            UpdateBufferCleanup();
        }

        /// <summary>
        /// 모든 버퍼를 제거합니다.
        /// </summary>
        public void ClearAllBuffers()
        {
            if (_bufferManager != null)
            {
                _bufferManager.ClearAllBuffers();
                Log.Progress(LogTags.Skill_Buffer, "모든 버퍼를 제거했습니다.");
            }
        }

        /// <summary>
        /// 특정 액션의 버퍼를 제거합니다.
        /// </summary>
        /// <param name="actionName">제거할 액션 이름</param>
        public void RemoveBufferedAction(ActionNames actionName)
        {
            _bufferManager?.RemoveBufferedAction(actionName);
        }

        /// <summary>
        /// 캐릭터 사망 시 모든 버퍼를 정리합니다.
        /// </summary>
        public void ClearBuffersOnDeath()
        {
            if (_bufferManager != null && !_bufferManager.IsEmpty)
            {
                _bufferManager.ClearAllBuffers();
                Log.Progress(LogTags.Skill_Buffer, "캐릭터 사망으로 인한 버퍼 정리");
            }
        }

        /// <summary>
        /// 캐릭터 착지 시 공중 버퍼를 정리합니다.
        /// </summary>
        public void ClearAirBuffersOnLanding()
        {
            if (BufferSettings?.EnableGroundStateCheck == true &&
                BufferSettings?.ClearAirBufferOnLanding == true &&
                _bufferManager != null)
            {
                _bufferManager.ClearAirBuffers();
            }
        }

        /// <summary>
        /// 현재 재생 중인 기술의 우선순위를 가져옵니다.
        /// </summary>
        /// <returns>현재 기술의 우선순위 (기술이 재생 중이 아니면 0)</returns>
        private int GetCurrentPlayingSkillPriority()
        {
            // 현재 재생 중인 애니메이션 이름으로 기술 찾기
            SkillAnimationAsset currentAnimation = Owner.CharacterAnimator.GetPlayingAnimation();
            if (currentAnimation == null)
            {
                return 0; // 애니메이션 재생 중이 아님
            }

            // 애니메이션 이름으로 기술 찾기
            SkillNames currentSkill = currentAnimation.SkillName;
            if (currentSkill == SkillNames.None)
            {
                Log.Warning(LogTags.Skill_Buffer, "현재 애니메이션에 해당하는 기술을 찾을 수 없음: {0}", currentAnimation.AnimationName);
                return 0;
            }

            // 기술의 우선순위 반환
            SkillAssetData skillData = ScriptableDataManager.Instance.FindSkillClone(currentSkill);
            int priority = BufferSettings.GetPriorityForCategory(skillData.Category);

            Log.Progress(LogTags.Skill_Buffer, "현재 재생 중인 기술 우선순위: {0} (우선순위: {1})",
                currentSkill.ToLogString(), priority);

            return priority;
        }

        #endregion Buffer Maintenance

        #region Buffer Integration with TryCast

        /// <summary>
        /// 버퍼 시스템을 고려한 기술 시전 시도입니다.
        /// </summary>
        /// <param name="skillName">시전할 기술 이름</param>
        /// <returns>시전 가능하면 true</returns>
        private bool TryCastWithBuffer(SkillNames skillName)
        {
            // 애니메이션 상태 확인 및 로깅
            bool isPlayingAnimation = Owner.CharacterAnimator.CheckPlayingSkillAnimation();
            string currentAnimationName = Owner.CharacterAnimator.PlayingSkillAnimationName;

            Log.Progress(LogTags.Skill_Buffer, "TryCastWithBuffer - 기술: {0}, 애니메이션 재생 중: {1}, 현재 애니메이션: {2}",
                skillName.ToLogString(), isPlayingAnimation,
                string.IsNullOrEmpty(currentAnimationName) ? "None" : currentAnimationName);

            // 애니메이션 재생 중인지 확인
            if (isPlayingAnimation)
            {
                // 액션 이름 찾기
                ActionNames actionName = CharacterSkillInfo.Find(skillName)?.ActionName ?? ActionNames.None;
                if (actionName != ActionNames.None)
                {
                    // 우선순위와 지속시간 가져오기
                    SkillAssetData skillData = ScriptableDataManager.Instance.FindSkillClone(skillName);
                    int newPriority = BufferSettings.GetPriorityForCategory(skillData.Category);
                    float duration = BufferSettings.DefaultBufferDuration;

                    // 현재 재생 중인 기술의 우선순위 확인
                    int currentPriority = GetCurrentPlayingSkillPriority();

                    Log.Progress(LogTags.Skill_Buffer, "우선순위 비교 - 새 기술: {0} ({1}), 현재 기술: ({2})",
                        skillName.ToLogString(), newPriority, currentPriority);

                    // 우선순위 캔슬 가능 여부 확인
                    if (_bufferManager.CanCancelByPriority(newPriority, currentPriority))
                    {
                        // 우선순위가 높으면 즉시 시전 (현재 기술 캔슬)
                        Log.Progress(LogTags.Skill_Buffer, "우선순위 캔슬로 즉시 시전: {0} (우선순위: {1} > {2})",
                            skillName.ToLogString(), newPriority, currentPriority);

                        // 현재 애니메이션 중단 (캔슬)
                        Owner.CharacterAnimator.StopPlayingSkillAnimation(false);

                        // 재시전 취소 로직을 고려한 즉시 시전
                        // 애니메이션 조건을 다시 확인하여 재시전 취소가 필요한지 체크
                        if (CheckAnimationConditionsForCast(skillName))
                        {
                            Log.Progress(LogTags.Skill_Buffer, "재시전 취소 조건 통과 - 기술 시전 진행: {0}", skillName.ToLogString());
                            return TryCastOriginal(skillName);
                        }
                        else
                        {
                            Log.Progress(LogTags.Skill_Buffer, "재시전 취소 조건에 의해 기술 시전이 차단됨: {0}", skillName.ToLogString());
                            return false;
                        }
                    }
                    else
                    {
                        // 우선순위가 낮거나 같으면 버퍼에 저장
                        // 현재 캐릭터의 지면 상태 확인
                        bool isGrounded = Owner.Controller.State.IsGrounded;

                        // 버퍼에 저장
                        bool added = _bufferManager.AddInputBuffer(actionName, duration, newPriority, isGrounded);

                        if (added)
                        {
                            string groundState = isGrounded ? "지면" : "공중";
                            Log.Progress(LogTags.Skill_Buffer, "우선순위 낮아 버퍼에 저장: {0} (우선순위: {1} <= {2}, 지속시간: {3:F2}s, 상태: {4})",
                                skillName.ToLogString(), newPriority, currentPriority, duration, groundState);
                        }
                        else
                        {
                            Log.Warning(LogTags.Skill_Buffer, "버퍼 저장 실패: {0}", skillName.ToLogString());
                        }

                        return false; // 시전 실패 (버퍼에 저장됨)
                    }
                }
                else
                {
                    Log.Warning(LogTags.Skill_Buffer, "액션 이름을 찾을 수 없어 버퍼에 저장하지 못함: {0}", skillName.ToLogString());
                }
            }

            // 기존 TryCast 로직 실행
            return TryCastOriginal(skillName);
        }

        #endregion Buffer Integration with TryCast

        #region Buffer Debugging

        /// <summary>
        /// 버퍼 상태를 로그로 출력합니다.
        /// </summary>
        public void LogBufferStatus()
        {
            _bufferManager?.LogBufferStatus();
        }

        /// <summary>
        /// 버퍼 정보를 디버그용 문자열로 반환합니다.
        /// </summary>
        /// <returns>디버그 정보</returns>
        public string GetBufferDebugInfo()
        {
            if (_bufferManager != null)
            {
                return _bufferManager.GetDebugInfo();
            }

            return "Buffer: Not Initialized";
        }

        /// <summary>
        /// 버퍼 시스템의 전체 상태를 로그로 출력합니다.
        /// </summary>
        public void LogBufferSystemStatus()
        {
            if (Log.LevelProgress)
            {
                StringBuilder sb = new();

                _ = sb.AppendLine("=== 버퍼 시스템 상태 ===");
                _ = sb.AppendLine($"활성화: {IsBufferSystemEnabled()}");

                if (BufferSettings != null)
                {
                    _ = sb.AppendLine($"기본 지속시간: {BufferSettings.DefaultBufferDuration:F2}s");
                    _ = sb.AppendLine($"최대 버퍼 개수: {BufferSettings.MaxBufferCount}");
                }

                if (_bufferManager != null)
                {
                    _ = sb.AppendLine($"현재 버퍼 개수: {_bufferManager.BufferCount}");
                    _ = sb.AppendLine($"버퍼 비어있음: {_bufferManager.IsEmpty}");
                }

                _ = sb.AppendLine("========================");

                Log.Progress(LogTags.Skill_Buffer, sb.ToString());
            }
        }

        #endregion Buffer Debugging

        #region Buffer Override Methods

        /// <summary>
        /// 기존 TryCast 메서드를 버퍼 시스템과 통합하여 오버라이드합니다.
        /// </summary>
        /// <param name="skillName">시전할 기술 이름</param>
        /// <returns>시전 가능하면 true</returns>
        public bool TryCastWithBufferSystem(SkillNames skillName)
        {
            // 기존 검증 로직
            if (Owner.IsCrowdControl)
            {
                Log.Warning(LogTags.Skill_Buffer, "{0}, 군중 제어 상태인 캐릭터는 기술을 시전할 수 없습니다.", skillName.ToLogString());
                return false;
            }

            VSkill skillInfo = CharacterSkillInfo.Find(skillName);
            if (skillInfo == null)
            {
                Log.Warning(LogTags.Skill_Buffer, "{0}, 할당되지 않은 기술을 시전할 수 없습니다.", skillName.ToLogString());
                return false;
            }

            if (!_entities.ContainsKey(skillInfo.Name))
            {
                Log.Warning(LogTags.Skill_Buffer, "{0}, 기술 독립체가 생성되어있지 않습니다.", skillInfo.Name.ToLogString());
                return false;
            }

            if (!CharacterSkillInfo.Contains(skillName))
            {
                Log.Warning(LogTags.Skill_Buffer, "{0}, 습득하지 않은 기술을 시전할 수 없습니다.", skillName.ToLogString());
                return false;
            }

            if (_allowedSkills.Count > 0 && !_allowedSkills.Contains(skillName))
            {
                Log.Warning(LogTags.Skill_Buffer, "{0}, 허용하지 않은 기술을 시전할 수 없습니다.", skillName.ToLogString());
                return false;
            }

            // 애니메이션 조건 확인 (재시전 취소 로직 포함)
            if (!CheckAnimationConditionsForCast(skillName))
            {
                return false;
            }

            // 버퍼 시스템과 통합된 시전 시도
            return TryCastWithBuffer(skillName);
        }

        #endregion Buffer Override Methods
    }
}