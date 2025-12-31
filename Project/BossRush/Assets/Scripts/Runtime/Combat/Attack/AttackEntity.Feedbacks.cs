using Sirenix.OdinInspector;
using TeamSuneat.Feedbacks;
using UnityEngine;

namespace TeamSuneat
{
    public partial class AttackEntity
    {
        [FoldoutGroup("#AttackEntity-Feedbacks")]
        [SuffixLabel("시작")]
        public GameFeedbacks AttackStartFeedback;

        [FoldoutGroup("#AttackEntity-Feedbacks")]
        [SuffixLabel("사용")]
        public GameFeedbacks AttackUsedFeedback;

        [FoldoutGroup("#AttackEntity-Feedbacks")]
        [SuffixLabel("종료")]
        public GameFeedbacks AttackStopFeedback;

        [FoldoutGroup("#AttackEntity-Feedbacks")]
        [SuffixLabel("재장전")]
        public GameFeedbacks AttackReloadFeedback;

        [FoldoutGroup("#AttackEntity-Feedbacks")]
        [SuffixLabel("재장전 필요")]
        public GameFeedbacks AttackReloadNeededFeedback;

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
            AttackStartFeedback = this.FindComponent<GameFeedbacks>("#Feedbacks/AttackStart");
            AttackUsedFeedback = this.FindComponent<GameFeedbacks>("#Feedbacks/AttackUse");
            AttackStopFeedback = this.FindComponent<GameFeedbacks>("#Feedbacks/AttackStop");
            AttackOnMissFeedback = this.FindComponent<GameFeedbacks>("#Feedbacks/AttackMiss");
            AttackOnHitDamageableFeedback = this.FindComponent<GameFeedbacks>("#Feedbacks/OnHitDamageable");
            AttackOnHitNonDamageableFeedback = this.FindComponent<GameFeedbacks>("#Feedbacks/OnHitNonDamageable");
            AttackOnKillFeedback = this.FindComponent<GameFeedbacks>("#Feedbacks/OnKill");
        }

        protected void InitializeFeedbacks()
        {
            AttackStartFeedback?.Initialization(Owner);
            AttackUsedFeedback?.Initialization(Owner);
            AttackStopFeedback?.Initialization(Owner);
            AttackReloadNeededFeedback?.Initialization(Owner);
            AttackReloadFeedback?.Initialization(Owner);
            AttackOnMissFeedback?.Initialization(Owner);
            AttackOnHitDamageableFeedback?.Initialization(Owner);
            AttackOnHitNonDamageableFeedback?.Initialization(Owner);
            AttackOnKillFeedback?.Initialization(Owner);
        }

        //

        protected void TriggerAttackStartFeedback()
        {
            if (AttackStartFeedback != null)
            {
                AttackStartFeedback.PlayFeedbacks(position, 0);
            }
        }

        protected void TriggerAttackUsedFeedback()
        {
            if (AttackUsedFeedback != null)
            {
                AttackUsedFeedback.PlayFeedbacks(position, 0);
            }
        }

        protected void TriggerAttackStopFeedback()
        {
            if (AttackStopFeedback != null)
            {
                AttackStopFeedback.PlayFeedbacks(position, 0);
            }
        }

        protected void TriggerAttackReloadNeededFeedback()
        {
            if (AttackReloadNeededFeedback != null)
            {
                AttackReloadNeededFeedback.PlayFeedbacks(position, 0);
            }
        }

        protected void TriggerAttackReloadFeedback()
        {
            if (AttackReloadFeedback != null)
            {
                AttackReloadFeedback.PlayFeedbacks(position, 0);
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
            if (AttackStartFeedback != null)
            {
                AttackStartFeedback.StopFeedbacks(position);
            }
        }   
    }
}