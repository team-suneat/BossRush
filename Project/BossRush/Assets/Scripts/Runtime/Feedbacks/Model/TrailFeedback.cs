using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat.Feedbacks
{
    public class TrailFeedback : GameFeedback
    {
        [FoldoutGroup("#Trail")]
        [SerializeField]
        private SpriteTrail.SpriteTrail[] _trails;

        [FoldoutGroup("#Trail")]
        [SerializeField]
        private float _duration;

        [FoldoutGroup("#Trail")]
        [SerializeField]
        private bool _deactivateOnStop;

        protected override void OnEnabled()
        {
            base.OnEnabled();

            DeactivateTrails();
        }

        protected override void CustomPlayFeedback(Vector3 position, int index, float feedbacksIntensity = 1f)
        {
            ActivateTrails();

            if (_duration > 0)
            {
                CoroutineNextTimer(_duration, DeactivateTrails);
            }
        }

        protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1f)
        {
            base.CustomStopFeedback(position, feedbacksIntensity);

            if (_deactivateOnStop)
            {
                DeactivateTrails();
            }
        }

        private void ActivateTrails()
        {
            if (_trails != null)
            {
                for (int i = 0; i < _trails.Length; i++)
                {
                    _trails[i].EnableTrail();
                }
            }
        }

        private void DeactivateTrails()
        {
            if (_trails != null)
            {
                for (int i = 0; i < _trails.Length; i++)
                {
                    _trails[i].DisableTrailEffect();
                }
            }
        }
    }
}