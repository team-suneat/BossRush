using System.Collections.Generic;
using UnityEngine;

namespace TeamSuneat
{
    public static class AnimatorEx
    {
        public static void SetAnimatorSpeed(this Animator self, float speed)
        {
            if (self != null)
                self.speed = speed;
        }

        public static void SetAnimatorSpeed(this Animator[] animators, float speed)
        {
            if (animators != null)
            {
                for (int i = 0; i < animators.Length; i++)
                {
                    animators[i].speed = speed;
                }
            }
        }

        /// <summary>
        /// 유형과 이름을 기준으로 애니메이터에 특정 파라미터가 포함되어 있는지 여부를 결정합니다.
        /// </summary>
        public static bool HasParameterOfType(this Animator self, string name, AnimatorControllerParameterType type)
        {
            if (self == null) return false;

            if (string.IsNullOrEmpty(name)) { return false; }

            AnimatorControllerParameter[] parameters = self.parameters;

            foreach (AnimatorControllerParameter currParam in parameters)
            {
                if (currParam.type == type && currParam.name == name)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 해당 매개변수가 있는 경우 매개변수 목록에 애니메이터 매개변수 이름을 추가합니다.
        /// </summary>
        public static void AddAnimatorParameterIfExists(this Animator animator, string parameterName, out int parameter, AnimatorControllerParameterType type, HashSet<int> parameterList)
        {
            if (string.IsNullOrEmpty(parameterName))
            {
                parameter = -1;
                return;
            }

            parameter = Animator.StringToHash(parameterName);

            if (animator.HasParameterOfType(parameterName, type))
            {
                parameterList.Add(parameter);
            }
        }

        /// <summary>
        /// 해당 매개변수가 있는 경우 매개변수 목록에 애니메이터 매개변수 이름을 추가합니다.
        /// </summary>
        public static void AddAnimatorParameterIfExists(this Animator animator, string parameterName, AnimatorControllerParameterType type, HashSet<string> parameterList)
        {
            if (animator.HasParameterOfType(parameterName, type))
            {
                parameterList.Add(parameterName);
            }
        }

        // INT PARAMETER METHODS ────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────

        public static bool UpdateAnimatorBool(this Animator animator, int parameter, bool value, HashSet<int> parameterList, bool performSanityCheck = true)
        {
            if (performSanityCheck)
            {
                if (false == parameterList.Contains(parameter))
                {
                    return false;
                }
            }

            animator.SetBool(parameter, value);

            return true;
        }

        public static bool UpdateAnimatorTrigger(this Animator animator, int parameter, HashSet<int> parameterList, bool performSanityCheck = true)
        {
            if (performSanityCheck)
            {
                if (false == parameterList.Contains(parameter))
                {
                    return false;
                }
            }

            animator.SetTrigger(parameter);

            return true;
        }

        public static bool SetAnimatorTrigger(this Animator animator, int parameter, HashSet<int> parameterList, bool performSanityCheck = true)
        {
            if (performSanityCheck)
            {
                if (false == parameterList.Contains(parameter))
                {
                    return false;
                }
            }

            animator.SetTrigger(parameter);

            return true;
        }

        public static bool UpdateAnimatorFloat(this Animator animator, int parameter, float value, HashSet<int> parameterList, bool performSanityCheck = true)
        {
            if (performSanityCheck)
            {
                if (false == parameterList.Contains(parameter))
                {
                    return false;
                }
            }

            animator.SetFloat(parameter, value);

            return true;
        }

        public static bool UpdateAnimatorInteger(this Animator animator, int parameter, int value, HashSet<int> parameterList, bool performSanityCheck = true)
        {
            if (performSanityCheck)
            {
                if (false == parameterList.Contains(parameter))
                {
                    return false;
                }
            }

            animator.SetInteger(parameter, value);

            return true;
        }

        // STRING PARAMETER METHODS ────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────

        public static void UpdateAnimatorBool(this Animator animator, string parameterName, bool value, HashSet<string> parameterList, bool performSanityCheck = true)
        {
            if (parameterList.Contains(parameterName))
            {
                animator.SetBool(parameterName, value);
            }
        }

        public static void UpdateAnimatorTrigger(this Animator animator, string parameterName, HashSet<string> parameterList, bool performSanityCheck = true)
        {
            if (parameterList.Contains(parameterName))
            {
                animator.SetTrigger(parameterName);
            }
        }

        public static void SetAnimatorTrigger(this Animator animator, string parameterName, HashSet<string> parameterList, bool performSanityCheck = true)
        {
            if (parameterList.Contains(parameterName))
            {
                animator.SetTrigger(parameterName);
            }
        }

        public static void UpdateAnimatorFloat(this Animator animator, string parameterName, float value, HashSet<string> parameterList, bool performSanityCheck = true)
        {
            if (parameterList.Contains(parameterName))
            {
                animator.SetFloat(parameterName, value);
            }
        }

        public static void UpdateAnimatorInteger(this Animator animator, string parameterName, int value, HashSet<string> parameterList, bool performSanityCheck = true)
        {
            if (parameterList.Contains(parameterName))
            {
                animator.SetInteger(parameterName, value);
            }
        }

        public static bool UpdateAnimatorBoolIfExists(this Animator animator, string parameterName, bool value, bool performSanityCheck = true)
        {
            if (animator.HasParameterOfType(parameterName, AnimatorControllerParameterType.Bool))
            {
                animator.SetBool(parameterName, value);

                return true;
            }

            return false;
        }

        public static bool UpdateAnimatorTriggerIfExists(this Animator animator, string parameterName, bool performSanityCheck = true)
        {
            if (animator.HasParameterOfType(parameterName, AnimatorControllerParameterType.Trigger))
            {
                animator.SetTrigger(parameterName);
                return true;
            }

            return false;
        }

        public static bool UpdateAnimatorFloatIfExists(this Animator animator, string parameterName, float value, bool performSanityCheck = true)
        {
            if (animator.HasParameterOfType(parameterName, AnimatorControllerParameterType.Float))
            {
                animator.SetFloat(parameterName, value);

                return true;
            }

            return false;
        }

        public static void UpdateAnimatorIntegerIfExists(this Animator animator, string parameterName, int value, bool performSanityCheck = true)
        {
            if (animator.HasParameterOfType(parameterName, AnimatorControllerParameterType.Int))
            {
                animator.SetInteger(parameterName, value);
            }
        }
    }
}