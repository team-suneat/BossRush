
using UnityEngine;

namespace TeamSuneat
{
    /// <summary>
    /// 기술 입력 버퍼 관련 확장 메서드들을 정의합니다.
    /// </summary>
    public static class SkillInputBufferExtensions
    {
        /// <summary>
        /// ActionNames가 기술 입력으로 사용되는지 확인합니다.
        /// </summary>
        /// <param name="actionName">확인할 액션 이름</param>
        /// <returns>기술 입력이면 true</returns>
        public static bool IsSkillInput(this ActionNames actionName)
        {
            switch (actionName)
            {
                case ActionNames.Attack:
                case ActionNames.SubAttack:
                case ActionNames.Cast1:
                case ActionNames.Cast2:
                case ActionNames.Cast3:
                case ActionNames.Cast4:
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// ActionNames가 기본 공격인지 확인합니다.
        /// </summary>
        /// <param name="actionName">확인할 액션 이름</param>
        /// <returns>기본 공격이면 true</returns>
        public static bool IsBasicAttack(this ActionNames actionName)
        {
            return actionName == ActionNames.Attack || actionName == ActionNames.SubAttack;
        }

        /// <summary>
        /// ActionNames가 스킬인지 확인합니다.
        /// </summary>
        /// <param name="actionName">확인할 액션 이름</param>
        /// <returns>스킬이면 true</returns>
        public static bool IsSkill(this ActionNames actionName)
        {
            return actionName == ActionNames.Cast1 ||
                   actionName == ActionNames.Cast2 ||
                   actionName == ActionNames.Cast3 ||
                   actionName == ActionNames.Cast4;
        }

        /// <summary>
        /// ActionNames가 콤보 가능한 기술인지 확인합니다.
        /// </summary>
        /// <param name="actionName">확인할 액션 이름</param>
        /// <returns>콤보 가능하면 true</returns>
        public static bool IsComboSkill(this ActionNames actionName)
        {
            // 기본 공격과 일부 스킬은 콤보 가능
            return actionName.IsBasicAttack() ||
                   actionName == ActionNames.Cast1 ||
                   actionName == ActionNames.Cast2;
        }

        /// <summary>
        /// ActionNames가 취소 가능한 기술인지 확인합니다.
        /// </summary>
        /// <param name="actionName">확인할 액션 이름</param>
        /// <returns>취소 가능하면 true</returns>
        public static bool IsCancelableSkill(this ActionNames actionName)
        {
            // 기본 공격과 일부 스킬은 취소 가능
            return actionName.IsBasicAttack() ||
                   actionName == ActionNames.Cast1;
        }

        /// <summary>
        /// ActionNames를 로그용 문자열로 변환합니다.
        /// </summary>
        /// <param name="actionName">변환할 액션 이름</param>
        /// <returns>로그용 문자열</returns>
        public static string ToLogString(this ActionNames actionName)
        {
            return actionName.ToString();
        }

        /// <summary>
        /// ActionNames를 UI 표시용 문자열로 변환합니다.
        /// </summary>
        /// <param name="actionName">변환할 액션 이름</param>
        /// <returns>UI 표시용 문자열</returns>
        public static string ToDisplayString(this ActionNames actionName)
        {
            switch (actionName)
            {
                case ActionNames.Attack:
                    return "기본 공격";

                case ActionNames.SubAttack:
                    return "보조 공격";

                case ActionNames.Cast1:
                    return "기술 1";

                case ActionNames.Cast2:
                    return "기술 2";

                case ActionNames.Cast3:
                    return "기술 3";

                case ActionNames.Cast4:
                    return "기술 4";

                default:
                    return actionName.ToString();
            }
        }

        /// <summary>
        /// 버퍼 우선순위를 문자열로 변환합니다.
        /// </summary>
        /// <param name="priority">우선순위 값</param>
        /// <returns>우선순위 문자열</returns>
        public static string ToPriorityString(this int priority)
        {
            switch (priority)
            {
                case 0:
                    return "낮음";

                case 1:
                    return "보통";

                case 2:
                    return "높음";

                case 3:
                    return "매우 높음";

                default:
                    return $"우선순위 {priority}";
            }
        }
    }
}