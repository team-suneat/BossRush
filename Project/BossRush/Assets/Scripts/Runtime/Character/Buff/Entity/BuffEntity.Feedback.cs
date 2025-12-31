using TeamSuneat.Feedbacks;
using Lean.Pool;
using Sirenix.OdinInspector;

namespace TeamSuneat
{
    public partial class BuffEntity : XBehaviour, IPoolable
    {
        [FoldoutGroup("#Feedbacks")] public GameFeedbacks ActivateFeedbacks;
        [FoldoutGroup("#Feedbacks")] public GameFeedbacks ApplyFeedbacks;
        [FoldoutGroup("#Feedbacks")] public GameFeedbacks OverlapFeedbacks;
        [FoldoutGroup("#Feedbacks")] public GameFeedbacks DeactivateFeedbacks;

        #region Feedback

        private void InitializeFeedbacks()
        {
            ActivateFeedbacks?.Initialization(Owner);
            ApplyFeedbacks?.Initialization(Owner);
            OverlapFeedbacks?.Initialization(Owner);
            DeactivateFeedbacks?.Initialization(Owner);
        }

        //

        private void PlayActivateFeedbacks()
        {
            ActivateFeedbacks?.PlayFeedbacks();
        }

        private void PlayApplyFeedbacks()
        {
            ApplyFeedbacks?.PlayFeedbacks();
        }

        public void PlayOverlapFeedbacks()
        {
            OverlapFeedbacks?.PlayFeedbacks();
        }

        private void PlayDeactivateFeedbacks()
        {
            DeactivateFeedbacks?.PlayFeedbacks();
        }

        //

        private void StopActivateFeedbacks()
        {
            ActivateFeedbacks?.StopFeedbacks();
        }

        private void StopApplyFeedbacks()
        {
            ApplyFeedbacks?.StopFeedbacks();
        }

        private void StopOverlapFeedbacks()
        {
            OverlapFeedbacks?.StopFeedbacks();
        }

        #endregion Feedback
    }
}