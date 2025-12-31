using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

namespace TeamSuneat.Feedbacks
{
    public class VisualEffectFeedback : GameFeedback
    {
        [FoldoutGroup("#Feedback-VisualEffect")]
        [SuffixLabel("피드백이 정지하면 생성된 이펙트를 삭제")]
        public bool DespawnOnStop;

        [FoldoutGroup("#Feedback-VisualEffect")]
        [SuffixLabel("반복 횟수")]
        public int RepeatCount;

        [FoldoutGroup("#Feedback-VisualEffect")]
        [SuffixLabel("반복 중 지연 시간")]
        public float DelayTimeForRepeat;

        [FoldoutGroup("#Feedback-VisualEffect")]
        public VisualEffectSpawnData[] SpawnDataArray;

        public bool IsForceDespawnOnce { get; set; }

        //----------------------------------------------------------------------------------------------------------------

        public override void Initialization(Character owner)
        {
            base.Initialization(owner);

            if (SpawnDataArray == null) return;

            for (int i = 0; i < SpawnDataArray.Length; i++)
            {
                SpawnDataArray[i].LoadToggleValues();
                SpawnDataArray[i].SetOwner(owner);
            }
        }

        //----------------------------------------------------------------------------------------------------------------

        protected override void CustomPlayFeedback(Vector3 feedbackPosition, int index, float feedbacksIntensity = 1)
        {
            if (RepeatCount > 0)
            {
                StartXCoroutine(ProcessSpawnVisualEffect(feedbackPosition, index));
            }
            else
            {
                SpawnVisualEffect(feedbackPosition, index);
            }
        }

        protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1)
        {
            base.CustomStopFeedback(position, feedbacksIntensity);

            if (DespawnOnStop)
            {
                DespawnVisualEffect();
            }
            else if (IsForceDespawnOnce)
            {
                IsForceDespawnOnce = false;

                DespawnVisualEffect();
            }
        }

        //----------------------------------------------------------------------------------------------------------------

        private void SpawnVisualEffect(Vector3 feedbackPosition, int index)
        {
            if (SpawnDataArray == null) return;

            for (int i = 0; i < SpawnDataArray.Length; i++)
            {
                if (!SpawnDataArray[i].TrySpawnVisualEffect()) continue;

                if (SpawnDataArray[i].UseParent)
                {
                    SpawnDataArray[i].SpawnVisualEffect(transform, index);
                }
                else if (UsingFeedbackPosition)
                {
                    SpawnDataArray[i].SpawnVisualEffect(position, index);
                }
                else
                {
                    SpawnDataArray[i].SpawnVisualEffect(feedbackPosition, index);
                }
            }
        }

        private IEnumerator ProcessSpawnVisualEffect(Vector3 feedbackPosition, int index)
        {
            for (int i = 0; i < RepeatCount; i++)
            {
                SpawnVisualEffect(feedbackPosition, index);

                yield return new WaitForSeconds(DelayTimeForRepeat);
            }
        }

        private void DespawnVisualEffect()
        {
            if (SpawnDataArray != null)
            {
                for (int i = 0; i < SpawnDataArray.Length; i++)
                {
                    SpawnDataArray[i].DespawnVisualEffect();
                }
            }
        }
    }
}