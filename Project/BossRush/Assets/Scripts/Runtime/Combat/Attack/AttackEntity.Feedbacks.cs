using Sirenix.OdinInspector;
using TeamSuneat.Feedbacks;
using UnityEngine;

namespace TeamSuneat
{
    public partial class AttackEntity : XBehaviour
    {
        [FoldoutGroup("#AttackEntity-Feedbacks")]
        [SuffixLabel("시작")]
        public GameFeedbacks AttackStartTSFeedback;

        [FoldoutGroup("#AttackEntity-Feedbacks")]
        [SuffixLabel("사용")]
        public GameFeedbacks AttackUsedTSFeedback;

        [FoldoutGroup("#AttackEntity-Feedbacks")]
        [SuffixLabel("종료")]
        public GameFeedbacks AttackStopTSFeedback;

        [FoldoutGroup("#AttackEntity-Feedbacks")]
        [SuffixLabel("재장전")]
        public GameFeedbacks AttackReloadTSFeedback;

        [FoldoutGroup("#AttackEntity-Feedbacks")]
        [SuffixLabel("재장전 필요")]
        public GameFeedbacks AttackReloadNeededTSFeedback;

        [FoldoutGroup("#AttackEntity-Feedbacks")]
        [SuffixLabel("빗나감*")]
        [Tooltip("빗나감의 구성은 TSAttack 하위 클래스별로 정의됩니다")]
        public GameFeedbacks AttackOnMissFeedback;

        [FoldoutGroup("#AttackEntity-Feedbacks")]
        [SuffixLabel("공격 성공(상대를 죽이지 못함)")]
        public GameFeedbacks AttackOnHitDamageableFeedback;

        [FoldoutGroup("#AttackEntity-Feedbacks")]
        [SuffixLabel("공격 실패")]
        public GameFeedbacks AttackOnHitNonDamageableFeedback;

        [FoldoutGroup("#AttackEntity-Feedbacks")]
        [SuffixLabel("공격 성공(상대를 죽임)")]
        public GameFeedbacks AttackOnKillFeedback;

        //

        protected void AutoGetFeedbackComponents()
        {
            AttackStartTSFeedback = this.FindComponent<GameFeedbacks>("#Feedbacks/AttackStart");
            AttackUsedTSFeedback = this.FindComponent<GameFeedbacks>("#Feedbacks/AttackUse");
            AttackStopTSFeedback = this.FindComponent<GameFeedbacks>("#Feedbacks/AttackStop");
            AttackOnMissFeedback = this.FindComponent<GameFeedbacks>("#Feedbacks/AttackMiss");
            AttackOnHitDamageableFeedback = this.FindComponent<GameFeedbacks>("#Feedbacks/OnHitDamageable");
            AttackOnHitNonDamageableFeedback = this.FindComponent<GameFeedbacks>("#Feedbacks/OnHitNonDamageable");
            AttackOnKillFeedback = this.FindComponent<GameFeedbacks>("#Feedbacks/OnKill");
        }

        protected void InitializeFeedbacks()
        {
            AttackStartTSFeedback?.Initialization(Owner);
            AttackUsedTSFeedback?.Initialization(Owner);
            AttackStopTSFeedback?.Initialization(Owner);
            AttackReloadNeededTSFeedback?.Initialization(Owner);
            AttackReloadTSFeedback?.Initialization(Owner);
            AttackOnMissFeedback?.Initialization(Owner);
            AttackOnHitDamageableFeedback?.Initialization(Owner);
            AttackOnHitNonDamageableFeedback?.Initialization(Owner);
            AttackOnKillFeedback?.Initialization(Owner);
        }

        //

        protected void TriggerAttackStartFeedback()
        {
            if (AttackStartTSFeedback != null)
            {
                AttackStartTSFeedback.PlayFeedbacks(position, 0);
            }
        }

        protected void TriggerAttackUsedFeedback()
        {
            if (AttackUsedTSFeedback != null)
            {
                AttackUsedTSFeedback.PlayFeedbacks(position, 0);
            }
        }

        protected void TriggerAttackStopFeedback()
        {
            if (AttackStopTSFeedback != null)
            {
                AttackStopTSFeedback.PlayFeedbacks(position, 0);
            }
        }

        protected void TriggerAttackReloadNeededFeedback()
        {
            if (AttackReloadNeededTSFeedback != null)
            {
                AttackReloadNeededTSFeedback.PlayFeedbacks(position, 0);
            }
        }

        protected void TriggerAttackReloadFeedback()
        {
            if (AttackReloadTSFeedback != null)
            {
                AttackReloadTSFeedback.PlayFeedbacks(position, 0);
            }
        }

        protected void TriggerAttackOnMissFeedback()
        {
            if (AttackOnMissFeedback != null)
            {
                AttackOnMissFeedback.PlayFeedbacks(position, 0);
            }
        }

        protected void TriggerAttackOnHitDamageableFeedback(Vector3 feedbackPosition)
        {
            if (AttackOnHitDamageableFeedback != null)
            {
                AttackOnHitDamageableFeedback.PlayFeedbacks(feedbackPosition, 0);
            }
        }

        protected void TriggerAttackOnHitNonDamageableFeedback(Vector3 feedbackPosition)
        {
            if (AttackOnHitNonDamageableFeedback != null)
            {
                AttackOnHitNonDamageableFeedback.PlayFeedbacks(feedbackPosition, 0);
            }
        }

        protected void TriggerAttackOnKillFeedback(Vector3 feedbackPosition)
        {
            if (AttackOnKillFeedback != null)
            {
                AttackOnKillFeedback.PlayFeedbacks(feedbackPosition, 0);
            }
        }

        protected void StopAttackStartFeedback()
        {
            if (AttackStartTSFeedback != null)
            {
                AttackStartTSFeedback.StopFeedbacks(position);
            }
        }
    }
}