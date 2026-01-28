using Sirenix.OdinInspector;
using TeamSuneat.Feedbacks;
using UnityEngine;

namespace TeamSuneat
{
    public partial class SkillEntity : XBehaviour
    {
        [FoldoutGroup("#SkillEntity-Feedbacks")]
        [SuffixLabel("시작")]
        public GameFeedbacks SkillStartTSFeedback;

        [FoldoutGroup("#SkillEntity-Feedbacks")]
        [SuffixLabel("사용")]
        public GameFeedbacks SkillUsedTSFeedback;

        [FoldoutGroup("#SkillEntity-Feedbacks")]
        [SuffixLabel("종료")]
        public GameFeedbacks SkillStopTSFeedback;

        [FoldoutGroup("#SkillEntity-Feedbacks")]
        [SuffixLabel("빗나감")]
        public GameFeedbacks SkillOnMissFeedback;

        [FoldoutGroup("#SkillEntity-Feedbacks")]
        [SuffixLabel("스킬 성공(상대를 죽이지 못함)")]
        public GameFeedbacks SkillOnHitDamageableFeedback;

        [FoldoutGroup("#SkillEntity-Feedbacks")]
        [SuffixLabel("스킬 실패")]
        public GameFeedbacks SkillOnHitNonDamageableFeedback;

        [FoldoutGroup("#SkillEntity-Feedbacks")]
        [SuffixLabel("스킬 성공(상대를 죽임)")]
        public GameFeedbacks SkillOnKillFeedback;

        //

        protected void AutoGetFeedbackComponents()
        {
            SkillStartTSFeedback = this.FindComponent<GameFeedbacks>("#Feedbacks/SkillStart");
            SkillUsedTSFeedback = this.FindComponent<GameFeedbacks>("#Feedbacks/SkillUse");
            SkillStopTSFeedback = this.FindComponent<GameFeedbacks>("#Feedbacks/SkillStop");
            SkillOnMissFeedback = this.FindComponent<GameFeedbacks>("#Feedbacks/SkillMiss");
            SkillOnHitDamageableFeedback = this.FindComponent<GameFeedbacks>("#Feedbacks/OnHitDamageable");
            SkillOnHitNonDamageableFeedback = this.FindComponent<GameFeedbacks>("#Feedbacks/OnHitNonDamageable");
            SkillOnKillFeedback = this.FindComponent<GameFeedbacks>("#Feedbacks/OnKill");
        }

        protected void InitializeFeedbacks()
        {
            SkillStartTSFeedback?.Initialization(Owner);
            SkillUsedTSFeedback?.Initialization(Owner);
            SkillStopTSFeedback?.Initialization(Owner);
            SkillOnMissFeedback?.Initialization(Owner);
            SkillOnHitDamageableFeedback?.Initialization(Owner);
            SkillOnHitNonDamageableFeedback?.Initialization(Owner);
            SkillOnKillFeedback?.Initialization(Owner);
        }

        //

        protected void TriggerSkillStartFeedback()
        {
            if (SkillStartTSFeedback != null)
            {
                SkillStartTSFeedback.PlayFeedbacks(position, 0);
            }
        }

        protected void TriggerSkillUsedFeedback()
        {
            if (SkillUsedTSFeedback != null)
            {
                SkillUsedTSFeedback.PlayFeedbacks(position, 0);
            }
        }

        protected void TriggerSkillStopFeedback()
        {
            if (SkillStopTSFeedback != null)
            {
                SkillStopTSFeedback.PlayFeedbacks(position, 0);
            }
        }

        protected void TriggerSkillOnMissFeedback()
        {
            if (SkillOnMissFeedback != null)
            {
                SkillOnMissFeedback.PlayFeedbacks(position, 0);
            }
        }

        protected void TriggerSkillOnHitDamageableFeedback(Vector3 feedbackPosition)
        {
            if (SkillOnHitDamageableFeedback != null)
            {
                SkillOnHitDamageableFeedback.PlayFeedbacks(feedbackPosition, 0);
            }
        }

        protected void TriggerSkillOnHitNonDamageableFeedback(Vector3 feedbackPosition)
        {
            if (SkillOnHitNonDamageableFeedback != null)
            {
                SkillOnHitNonDamageableFeedback.PlayFeedbacks(feedbackPosition, 0);
            }
        }

        protected void TriggerSkillOnKillFeedback(Vector3 feedbackPosition)
        {
            if (SkillOnKillFeedback != null)
            {
                SkillOnKillFeedback.PlayFeedbacks(feedbackPosition, 0);
            }
        }

        protected void StopSkillStartFeedback()
        {
            if (SkillStartTSFeedback != null)
            {
                SkillStartTSFeedback.StopFeedbacks(position);
            }
        }
    }
}
