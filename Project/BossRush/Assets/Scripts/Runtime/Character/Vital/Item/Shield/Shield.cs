using Sirenix.OdinInspector;
using TeamSuneat.Feedbacks;
using UnityEngine;

namespace TeamSuneat
{
    /// <summary> 캐릭터의 보호막을 관리하는 클래스입니다. </summary>
    public partial class Shield : VitalResource
    {
        [FoldoutGroup("#Feedback")] public GameFeedbacks ActivateFeedbacks;
        [FoldoutGroup("#Feedback")] public GameFeedbacks DamageFeedbacks;
        [FoldoutGroup("#Feedback")] public GameFeedbacks DestroyFeedbacks;

        public override VitalResourceTypes Type => VitalResourceTypes.Shield;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            Transform feedbackParent = this.FindTransform("#Feedbacks-Shield");
            if (feedbackParent != null)
            {
                DamageFeedbacks = feedbackParent.FindComponent<GameFeedbacks>("Damage");
                DestroyFeedbacks = feedbackParent.FindComponent<GameFeedbacks>("Destroy");
                ActivateFeedbacks = feedbackParent.FindComponent<GameFeedbacks>("Activate");
            }
        }

        public override void LoadCurrentValue()
        {
            base.LoadCurrentValue();

            Vital.RefreshShieldGauge();
            LogShieldInitialized(Current, Max);
        }

        public override bool AddCurrentValue(int value)
        {
            if (base.AddCurrentValue(value))
            {
                Vital.RefreshShieldGauge();

                if (Vital.Owner != null && Vital.Owner.IsPlayer)
                {
                    GlobalEvent<int, int>.Send(GlobalEventType.PLAYER_CHARACTER_SHIELD_CHARGE, Current, Max);
                }
                return true;
            }

            return false;
        }

        public override bool UseCurrentValue(int value, DamageResult damageResult)
        {
            if (base.UseCurrentValue(value, damageResult))
            {
                if (Current > 0)
                {
                    OnDamageShield(damageResult);
                }
                else
                {
                    OnDestroyShield();
                }

                Vital.RefreshShieldGauge();
                return true;
            }

            LogShieldUsageFailure(value);
            return false;
        }

        public void SpawnShieldFloatyText(int damageValue)
        {
            string content = damageValue.ToString();

            if (Vital != null)
            {
                _ = SpawnFloatyText(content, DamageTextPoint, UIFloatyMoveNames.ChargeShield);
            }
        }
    }
}