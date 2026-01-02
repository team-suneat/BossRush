using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace TeamSuneat
{
    /// <summary>
    /// 기술 입력 버퍼를 관리하는 매니저 클래스입니다.
    /// 애니메이션 재생 중 입력된 기술들을 저장하고 처리합니다.
    /// </summary>
    public class SkillInputBufferManager
    {
        private readonly List<SkillInputBuffer> _inputBuffers = new();
        private float _defaultBufferDuration = 0.35f;
        private int _maxBufferCount = 1;

        // 입력 쿨타임 관리
        private bool _enableInputCooldown = true;
        private float _defaultCooldownDuration = 0.1f;

        /// <summary> 현재 버퍼된 입력 개수 </summary>
        public int BufferCount => _inputBuffers.Count;

        /// <summary> 버퍼가 비어있는지 확인 </summary>
        public bool IsEmpty => _inputBuffers.Count == 0;

        /// <summary>
        /// 버퍼 설정을 업데이트합니다.
        /// </summary>
        /// <param name="defaultDuration">기본 버퍼 지속 시간</param>
        /// <param name="maxCount">최대 버퍼 개수</param>
        public void UpdateSettings(float defaultDuration, int maxCount)
        {
            _defaultBufferDuration = defaultDuration;
            _maxBufferCount = maxCount;

            // 설정 변경 시 기존 버퍼 정리
            CleanupExpiredBuffers();

            Log.Progress(LogTags.Skill_Buffer, "버퍼 설정 업데이트: 지속시간={0:F2}s, 최대개수={1}",
                _defaultBufferDuration, _maxBufferCount);
        }

        /// <summary>
        /// 입력 쿨타임 설정을 업데이트합니다.
        /// </summary>
        /// <param name="enableCooldown">쿨타임 활성화 여부</param>
        /// <param name="defaultCooldown">기본 쿨타임 시간</param>
        public void UpdateCooldownSettings(bool enableCooldown, float defaultCooldown)
        {
            _enableInputCooldown = enableCooldown;
            _defaultCooldownDuration = defaultCooldown;
        }

        /// <summary>
        /// 기술 입력을 버퍼에 추가합니다.
        /// </summary>
        /// <param name="actionName">액션 이름</param>
        /// <param name="bufferDuration">버퍼 지속 시간</param>
        /// <param name="priority">우선순위</param>
        /// <param name="isGrounded">입력 시점의 지면 상태</param>
        /// <returns>버퍼 추가 성공 여부</returns>
        public bool AddInputBuffer(ActionNames actionName, float bufferDuration = -1f, int priority = 0, bool isGrounded = true)
        {
            // 입력 쿨타임 체크 (프레임 단위 중복 방지)
            if (IsActionOnCooldown(actionName))
            {
                Log.Progress(LogTags.Skill_Buffer, "액션 {0}이 쿨타임 중이므로 버퍼에 추가하지 않습니다.", actionName);
                return false;
            }

            // 기본 지속 시간 사용
            if (bufferDuration < 0f)
            {
                bufferDuration = _defaultBufferDuration;
            }

            string groundState = isGrounded ? "지면" : "공중";

            // 중복 입력 허용: 같은 액션이라도 새로 추가
            // 최대 버퍼 개수 체크
            if (_inputBuffers.Count >= _maxBufferCount)
            {
                // 가장 오래된 버퍼 제거
                _inputBuffers.RemoveAt(0);
            }

            // 새 버퍼 추가
            SkillInputBuffer newBuffer = new(actionName, bufferDuration, priority, isGrounded);
            _inputBuffers.Add(newBuffer);

            Log.Progress(LogTags.Skill_Buffer, "기술 입력 버퍼에 추가: {0} (지속시간: {1:F2}s, 우선순위: {2}, 상태: {3})",
                actionName, bufferDuration, priority, groundState);

            return true;
        }

        /// <summary>
        /// 버퍼에서 가장 우선순위가 높은 액션을 가져옵니다.
        /// </summary>
        /// <returns>버퍼된 액션, 없으면 null</returns>
        public ActionNames? GetBufferedAction()
        {
            CleanupExpiredBuffers();

            if (_inputBuffers.Count == 0)
            {
                return null;
            }

            // 우선순위가 높은 순서로 정렬하여 가장 높은 우선순위의 버퍼 반환
            SkillInputBuffer highestPriorityBuffer = _inputBuffers
                .OrderByDescending(buffer => buffer.Priority)
                .ThenByDescending(buffer => buffer.InputTime) // 우선순위가 같으면 최신 입력 우선
                .First();

            _inputBuffers.Remove(highestPriorityBuffer);

            Log.Progress(LogTags.Skill_Buffer, "버퍼에서 기술 시전: {0} (우선순위: {1})",
                highestPriorityBuffer.ActionName, highestPriorityBuffer.Priority);

            return highestPriorityBuffer.ActionName;
        }

        /// <summary>
        /// 특정 액션의 버퍼를 제거합니다.
        /// </summary>
        /// <param name="actionName">제거할 액션 이름</param>
        public void RemoveBufferedAction(ActionNames actionName)
        {
            int removedCount = _inputBuffers.RemoveAll(buffer => buffer.ActionName == actionName);

            if (removedCount > 0)
            {
                Log.Progress(LogTags.Skill_Buffer, "버퍼에서 액션 제거: {0} ({1}개)", actionName, removedCount);
            }
        }

        /// <summary>
        /// 모든 버퍼를 제거합니다.
        /// </summary>
        public void ClearAllBuffers()
        {
            _inputBuffers.Clear();
            Log.Progress(LogTags.Skill_Buffer, "모든 버퍼를 제거했습니다.");
        }

        /// <summary>
        /// 만료된 버퍼들을 정리합니다.
        /// </summary>
        public void CleanupExpiredBuffers()
        {
            int expiredCount = _inputBuffers.RemoveAll(buffer => !buffer.IsValid);

            if (expiredCount > 0)
            {
                Log.Progress(LogTags.Skill_Buffer, "만료된 버퍼 정리: {0}개", expiredCount);
            }
        }

        /// <summary>
        /// 특정 액션이 버퍼에 있는지 확인합니다.
        /// </summary>
        /// <param name="actionName">확인할 액션 이름</param>
        /// <returns>버퍼에 있으면 true</returns>
        public bool HasBufferedAction(ActionNames actionName)
        {
            CleanupExpiredBuffers();
            return _inputBuffers.Any(buffer => buffer.ActionName == actionName);
        }

        /// <summary>
        /// 특정 액션이 쿨타임 중인지 확인합니다.
        /// </summary>
        /// <param name="actionName">확인할 액션 이름</param>
        /// <returns>쿨타임 중이면 true</returns>
        public bool IsActionOnCooldown(ActionNames actionName)
        {
            if (!_enableInputCooldown)
            {
                return false;
            }

            // 해당 액션의 마지막 입력 시간 확인
            SkillInputBuffer lastInput = _inputBuffers
                .Where(buffer => buffer.ActionName == actionName)
                .OrderByDescending(buffer => buffer.InputTime)
                .FirstOrDefault();

            if (lastInput != null)
            {
                float timeSinceLastInput = Time.time - lastInput.InputTime;
                bool isOnCooldown = timeSinceLastInput < _defaultCooldownDuration;

                if (isOnCooldown)
                {
                    Log.Progress(LogTags.Skill_Buffer, "액션 {0}이 쿨타임 중입니다. 남은시간: {1:F2}s",
                        actionName, _defaultCooldownDuration - timeSinceLastInput);
                }

                return isOnCooldown;
            }

            return false;
        }

        /// <summary>
        /// 마지막 입력 시간을 반환합니다.
        /// </summary>
        /// <returns>마지막 입력 시간 (입력이 없으면 0)</returns>
        public float GetLastInputTime()
        {
            if (_inputBuffers.Count == 0)
            {
                return 0f;
            }

            return _inputBuffers.Max(buffer => buffer.InputTime);
        }

        /// <summary>
        /// 공중에서 입력된 버퍼들을 모두 제거합니다.
        /// </summary>
        public void ClearAirBuffers()
        {
            int removedCount = _inputBuffers.RemoveAll(buffer => !buffer.IsGrounded);

            if (removedCount > 0)
            {
                Log.Progress(LogTags.Skill_Buffer, "공중에서 입력된 버퍼 {0}개를 제거했습니다.", removedCount);
            }
        }

        /// <summary>
        /// 특정 상태의 버퍼가 있는지 확인합니다.
        /// </summary>
        /// <param name="isGrounded">확인할 지면 상태</param>
        /// <returns>해당 상태의 버퍼가 있으면 true</returns>
        public bool HasBufferedActionWithState(bool isGrounded)
        {
            CleanupExpiredBuffers();
            return _inputBuffers.Any(buffer => buffer.IsGrounded == isGrounded);
        }

        /// <summary>
        /// 현재 상태와 일치하는 버퍼만 가져옵니다.
        /// </summary>
        /// <param name="currentIsGrounded">현재 지면 상태</param>
        /// <returns>상태가 일치하는 버퍼된 액션, 없으면 null</returns>
        public ActionNames? GetBufferedActionWithStateCheck(bool currentIsGrounded)
        {
            CleanupExpiredBuffers();

            if (_inputBuffers.Count == 0)
            {
                return null;
            }

            // 현재 상태와 일치하는 버퍼만 필터링
            var matchingBuffers = _inputBuffers.Where(buffer => buffer.IsGrounded == currentIsGrounded).ToList();

            if (matchingBuffers.Count == 0)
            {
                return null;
            }

            // 우선순위가 높은 순서로 정렬하여 가장 높은 우선순위의 버퍼 반환
            SkillInputBuffer highestPriorityBuffer = matchingBuffers
                .OrderByDescending(buffer => buffer.Priority)
                .ThenByDescending(buffer => buffer.InputTime) // 우선순위가 같으면 최신 입력 우선
                .First();

            _inputBuffers.Remove(highestPriorityBuffer);

            string groundState = currentIsGrounded ? "지면" : "공중";
            Log.Progress(LogTags.Skill_Buffer, "상태 일치 버퍼에서 기술 시전: {0} (우선순위: {1}, 상태: {2})",
                highestPriorityBuffer.ActionName, highestPriorityBuffer.Priority, groundState);

            return highestPriorityBuffer.ActionName;
        }

        /// <summary>
        /// 우선순위에 따른 캔슬 여부를 확인합니다.
        /// </summary>
        /// <param name="newPriority">새로 입력된 기술의 우선순위</param>
        /// <param name="currentPriority">현재 재생 중인 기술의 우선순위</param>
        /// <returns>캔슬 가능하면 true</returns>
        public bool CanCancelByPriority(int newPriority, int currentPriority)
        {
            bool canCancel = newPriority > currentPriority;

            if (canCancel)
            {
                Log.Progress(LogTags.Skill_Buffer, "우선순위 캔슬 가능: 새 기술({0}) > 현재 기술({1})", newPriority, currentPriority);
            }
            else
            {
                Log.Progress(LogTags.Skill_Buffer, "우선순위 캔슬 불가: 새 기술({0}) <= 현재 기술({1})", newPriority, currentPriority);
            }

            return canCancel;
        }

        /// <summary>
        /// 우선순위에 따른 캔슬이 가능한 버퍼가 있는지 확인합니다.
        /// </summary>
        /// <param name="currentPriority">현재 재생 중인 기술의 우선순위</param>
        /// <returns>캔슬 가능한 버퍼가 있으면 true</returns>
        public bool HasCancellableBuffer(int currentPriority)
        {
            CleanupExpiredBuffers();
            return _inputBuffers.Any(buffer => buffer.Priority > currentPriority);
        }

        /// <summary>
        /// 현재 재생 중인 기술보다 높은 우선순위의 버퍼를 가져옵니다.
        /// </summary>
        /// <param name="currentPriority">현재 재생 중인 기술의 우선순위</param>
        /// <returns>캔슬할 버퍼된 액션, 없으면 null</returns>
        public ActionNames? GetCancellableBuffer(int currentPriority)
        {
            CleanupExpiredBuffers();

            if (_inputBuffers.Count == 0)
            {
                return null;
            }

            // 현재 우선순위보다 높은 버퍼만 필터링
            var cancellableBuffers = _inputBuffers.Where(buffer => buffer.Priority > currentPriority).ToList();

            if (cancellableBuffers.Count == 0)
            {
                return null;
            }

            // 우선순위가 높은 순서로 정렬하여 가장 높은 우선순위의 버퍼 반환
            SkillInputBuffer highestPriorityBuffer = cancellableBuffers
                .OrderByDescending(buffer => buffer.Priority)
                .ThenByDescending(buffer => buffer.InputTime) // 우선순위가 같으면 최신 입력 우선
                .First();

            _inputBuffers.Remove(highestPriorityBuffer);

            Log.Progress(LogTags.Skill_Buffer, "우선순위 캔슬 버퍼에서 기술 시전: {0} (우선순위: {1}, 현재: {2})",
                highestPriorityBuffer.ActionName, highestPriorityBuffer.Priority, currentPriority);

            return highestPriorityBuffer.ActionName;
        }

        /// <summary>
        /// 버퍼 상태를 로그로 출력합니다.
        /// </summary>
        public void LogBufferStatus()
        {
            if (Log.LevelProgress)
            {
                StringBuilder sb = new();
                sb.AppendLine("=== 버퍼 상태 ===");
                sb.AppendLine($"버퍼 개수: {_inputBuffers.Count}");

                for (int i = 0; i < _inputBuffers.Count; i++)
                {
                    SkillInputBuffer buffer = _inputBuffers[i];
                    sb.AppendLine($"[{i}] {buffer.ActionName.ToLogString()} - 우선순위: {buffer.Priority}, 남은시간: {buffer.RemainingTime:F2}s");
                }

                sb.AppendLine("=================");

                Log.Progress(LogTags.Skill_Buffer, sb.ToString());
            }
        }

        /// <summary>
        /// 디버그 정보를 문자열로 반환합니다.
        /// </summary>
        /// <returns>디버그 정보</returns>
        public string GetDebugInfo()
        {
            if (_inputBuffers.Count == 0)
            {
                return "Buffer: Empty";
            }

            StringBuilder sb = new();
            sb.AppendLine($"Buffer Count: {_inputBuffers.Count}");

            for (int i = 0; i < _inputBuffers.Count; i++)
            {
                SkillInputBuffer buffer = _inputBuffers[i];
                sb.AppendLine($"[{i}] {buffer.ActionName} - Priority: {buffer.Priority}, Time: {buffer.RemainingTime:F2}s");
            }

            return sb.ToString();
        }
    }
}