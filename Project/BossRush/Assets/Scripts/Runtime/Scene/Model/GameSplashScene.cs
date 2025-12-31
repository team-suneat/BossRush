using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace TeamSuneat.Scenes
{
    public class GameSplashScene : XScene
    {
        public Image CIImage;
        public Sprite[] CISprites;

        protected override void OnCreateScene()
        {
        }

        protected override void OnEnterScene()
        {
            StartUIAnimation();
        }

        protected override void OnExitScene()
        {
        }

        protected override void OnDestroyScene()
        {
        }

        //───────────────────────────────────────────────────────────────────────────

        private void StartUIAnimation()
        {
            if (CIImage != null && CISprites.IsValid())
            {
                StartCoroutine(ProcessUIAnimation());
            }
        }

        private IEnumerator ProcessUIAnimation()
        {
            float fadeoutDuration = 2;
            float fadeWaitTime = 1;

            CIImage.FadeIn(1, fadeoutDuration, 0);
            yield return new WaitForSeconds(fadeoutDuration);

            CIImage.FadeOut(0, fadeoutDuration, fadeWaitTime);
            yield return new WaitForSeconds(fadeoutDuration + fadeWaitTime);

            CIImage.color = GameColors.CreamIvory;
            CIImage.SetSprite(CISprites[0], true);

            for (int i = 1; i < CISprites.Length; i++)
            {
                yield return new WaitForSeconds(0.03f);
                CIImage.sprite = CISprites[i];
            }

            CIImage.FadeOut(0, fadeoutDuration, fadeWaitTime);
            yield return new WaitForSeconds(fadeoutDuration + fadeWaitTime);

            CIImage.gameObject.SetActive(false);
            yield return new WaitForSeconds(1);

            ChangeGameTitleScene();
        }

        private void ChangeGameTitleScene()
        {
            if (DetermineChangeScene("GameTitle"))
            {
                ChangeScene("GameTitle");
            }
        }
    }
}