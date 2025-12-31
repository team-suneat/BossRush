using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine.UI;

namespace TeamSuneat.Scenes
{
    public class GameLoadingScene : XScene
    {
        [Title("LoadingScene")]
        public FadeSceneLoader FadeSceneLoader;
        public Slider LoadingSlider;

        protected override void OnCreateScene()
        {
        }

        protected override void OnEnterScene()
        {
            _ = StartCoroutine(ProcessSlider());
        }

        protected override void OnExitScene()
        {
        }

        protected override void OnDestroyScene()
        {
        }

        private IEnumerator ProcessSlider()
        {
            float duration = FadeSceneLoader.FadeDelayTime + FadeSceneLoader.FadeOutDuration;
            float elapsedTime = 0;
            while (elapsedTime < duration)
            {
                LoadingSlider.value = elapsedTime.SafeDivide01(duration);
                elapsedTime += duration;
                yield return null;
            }

            LoadingSlider.value = 1;
        }
    }
}