using Lean.Pool;
using Sirenix.OdinInspector;
using TeamSuneat.Feedbacks;

using UnityEngine;

namespace TeamSuneat
{
    public partial class SkillEntity : XBehaviour, IPoolable
    {
        [FoldoutGroup("#SkillEntity-Feedback", 5)]
        public GameFeedbacks CastFeedbacks;

        [FoldoutGroup("#SkillEntity-Feedback", 5)]
        public GameFeedbacks PlayAnimationFeedbacks;

        [FoldoutGroup("#SkillEntity-Feedback", 5)]
        [LabelText("지면에 닿았을 때만 Cast 피드백 호출")]
        public bool OnlyCastFeedbacksOnGrounded;

        [FoldoutGroup("#SkillEntity-Feedback", 5)]
        [LabelText("애니메이션이 대체되었을 때만 Cast 피드백 호출")]
        public bool OnlyCastFeedbackOnReplaced;

        protected void AutoGetFeedbackComponents()
        {
            CastFeedbacks = this.FindComponent<GameFeedbacks>("#Feedbacks/Cast");

            PlayAnimationFeedbacks = this.FindComponent<GameFeedbacks>("#Feedbacks/PlayAnimation");
        }

        protected void InitializeFeedbacks()
        {
            if (CastFeedbacks != null)
            {
                CastFeedbacks.Initialization(Owner);
            }

            if (PlayAnimationFeedbacks != null)
            {
                PlayAnimationFeedbacks.Initialization(Owner);
            }
        }

        protected void TriggerCastFeedback()
        {
            if (CastFeedbacks != null)
            {
                if (OnlyCastFeedbacksOnGrounded)
                {
                    if (!Owner.Controller.State.IsGrounded)
                    {
                        LogWarningCastFeedbackOnGround();
                        return;
                    }
                }

                if (OnlyCastFeedbackOnReplaced)
                {
                    LogWarningCastFeedbackOnReplaced();
                    return;
                }

                Order order = GetCurrentOrder();
                if (order != null)
                {
                    int index = Mathf.Max(0, order.Current - 1);
                    CastFeedbacks.PlayFeedbacks(position, index);
                }
                else
                {
                    LogWarningOrderNotSet();
                }
            }
        }

        private void TriggerPlayAnimationFeedback()
        {
            if (PlayAnimationFeedbacks != null)
            {
                PlayAnimationFeedbacks.PlayFeedbacks();
            }
        }

        private void StopCastFeedback()
        {
            if (CastFeedbacks != null)
            {
                LogInfoStopCastFeedback();
                CastFeedbacks.StopFeedbacks();
            }
        }

        #region Log

        private void LogWarningCastFeedbackOnGround()
        {
            if (Log.LevelWarning)
            {
                LogWarning("기술의 Cast 피드백을 호출할 수 없습니다. 지면에 닿았을 때만 호출할 수 있습니다.");
            }
        }

        private void LogWarningCastFeedbackOnReplaced()
        {
            if (Log.LevelWarning)
            {
                LogWarning("기술의 Cast 피드백을 호출할 수 없습니다. 애니메이션이 대체되었을 때만 호출할 수 있습니다.");
            }
        }

        private void LogWarningOrderNotSet()
        {
            if (Log.LevelWarning)
            {
                LogWarning("기술의 시전 피드백을 호출할 수 없습니다. 기술의 순서가 설정되지 않았습니다.");
            }
        }

        private void LogInfoStopCastFeedback()
        {
            if (Log.LevelInfo)
            {
                LogInfo("기술의 시전 피드백을 정지합니다.");
            }
        }

        #endregion Log
    }
}