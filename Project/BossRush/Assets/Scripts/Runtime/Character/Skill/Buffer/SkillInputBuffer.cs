
using UnityEngine;

namespace TeamSuneat
{
    /// <summary>
    /// 기술 입력 버퍼의 데이터 구조를 정의합니다.
    /// 애니메이션 재생 중 입력된 기술을 저장하고 관리합니다.
    /// </summary>
    public class SkillInputBuffer
    {
        /// <summary> 버퍼된 액션 이름 </summary>
        public ActionNames ActionName { get; set; }
        
        /// <summary> 입력된 시간 </summary>
        public float InputTime { get; set; }
        
        /// <summary> 버퍼 지속 시간 </summary>
        public float BufferDuration { get; set; }
        
        /// <summary> 버퍼가 유효한지 확인 </summary>
        public bool IsValid => (Time.time - InputTime) <= BufferDuration;
        
        /// <summary> 남은 버퍼 시간 </summary>
        public float RemainingTime => Mathf.Max(0f, BufferDuration - (Time.time - InputTime));
        
        /// <summary> 버퍼 우선순위 (높을수록 우선) </summary>
        public int Priority { get; set; }
        
        /// <summary> 입력 시점의 캐릭터 지면 상태 (true: 지면, false: 공중) </summary>
        public bool IsGrounded { get; set; }
        
        /// <summary>
        /// 기술 입력 버퍼를 생성합니다.
        /// </summary>
        /// <param name="actionName">액션 이름</param>
        /// <param name="bufferDuration">버퍼 지속 시간</param>
        /// <param name="priority">우선순위</param>
        /// <param name="isGrounded">입력 시점의 지면 상태</param>
        public SkillInputBuffer(ActionNames actionName, float bufferDuration = 0.5f, int priority = 0, bool isGrounded = true)
        {
            ActionName = actionName;
            InputTime = Time.time;
            BufferDuration = bufferDuration;
            Priority = priority;
            IsGrounded = isGrounded;
        }
        
        /// <summary>
        /// 버퍼 정보를 문자열로 반환합니다.
        /// </summary>
        public override string ToString()
        {
            string groundState = IsGrounded ? "지면" : "공중";
            return $"SkillInputBuffer[{ActionName}, Remaining: {RemainingTime:F2}s, Priority: {Priority}, State: {groundState}]";
        }
    }
} 