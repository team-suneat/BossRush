using Sirenix.OdinInspector;
using TeamSuneat.Feedbacks;
using UnityEngine;

namespace TeamSuneat
{
    public partial class Health : VitalResource
    {
        [GUIColor("GetFeedbackColor")]
        [FoldoutGroup("#Feedback")] public GameFeedbacks HealFeedbacks;

        [GUIColor("GetFeedbackColor")]
        [FoldoutGroup("#Feedback")] public GameFeedbacks DamageFeedbacks;

        [GUIColor("GetFeedbackColor")]
        [FoldoutGroup("#Feedback")] public GameFeedbacks DamageZeroFeedbacks;

        [GUIColor("GetFeedbackColor")]
        [FoldoutGroup("#Feedback")] public GameFeedbacks BlockDamageFeedbacks;

        [GUIColor("GetFeedbackColor")]
        [FoldoutGroup("#Feedback")] public GameFeedbacks DeathFeedbacks;

        [GUIColor("GetFeedbackColor")]
        [FoldoutGroup("#Feedback")] public GameFeedbacks KilledFeedbacks;

        [GUIColor("GetFeedbackColor")]
        [FoldoutGroup("#Feedback")] public GameFeedbacks SuicideFeedbacks;

        #region Feedback

        private void GetFeedbackComponents()
        {
            HealFeedbacks = this.FindComponent<GameFeedbacks>("#Feedbacks/Heal");
            DamageFeedbacks = this.FindComponent<GameFeedbacks>("#Feedbacks/Damage");
            DamageZeroFeedbacks = this.FindComponent<GameFeedbacks>("#Feedbacks/Damage(Zero)");
            BlockDamageFeedbacks = this.FindComponent<GameFeedbacks>("#Feedbacks/BlockDamage");
            DeathFeedbacks = this.FindComponent<GameFeedbacks>("#Feedbacks/Death");
            KilledFeedbacks = this.FindComponent<GameFeedbacks>("#Feedbacks/Killed");
            SuicideFeedbacks = this.FindComponent<GameFeedbacks>("#Feedbacks/Suicide");
        }

        private void PlayHealFeedbacks(float damageValue)
        {
            HealFeedbacks?.PlayFeedbacks(position, 0);
        }

        private void PlayDamageFeedbacks(float damageValue)
        {
            DamageFeedbacks?.PlayFeedbacks(position, 0);
        }

        private void PlayDeathFeedback()
        {
            DeathFeedbacks?.PlayFeedbacks();
        }

        private void PlayKilledFeedbacks()
        {
            KilledFeedbacks?.PlayFeedbacks();
        }

        private void PlaySuicideFeedbacks()
        {
            SuicideFeedbacks?.PlayFeedbacks();
        }

        #endregion Feedback

        private Color GetFeedbackColor(GameFeedbacks feedbacks)
        {
            if (feedbacks == null)
                return GameColors.Gray;

            return GameColors.White;
        }
    }
}