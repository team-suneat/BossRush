using System.Collections;
using TeamSuneat.Setting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TeamSuneat
{
    public class FadeSceneLoader : ISceneLoader
    {
        public UICanvasGroupFader Fader;
        public float FadeInDuration;
        public float FadeOutDuration;
        public float FadeDelayTime;

        public class WaitForFadeIn : CustomYieldInstruction
        {
            private bool m_keepWaiting = true;

            public override bool keepWaiting => m_keepWaiting;

            public WaitForFadeIn(float duration, UICanvasGroupFader fader)
            {
                if (fader != null)
                {
                    fader.FadeInDuration = duration;
                    fader.SetCompletedCallback(StopKeepWaiting);
                    fader.FadeIn();
                }
            }

            private void StopKeepWaiting()
            {
                m_keepWaiting = false;
            }
        }

        public class WaitForFadeOut : CustomYieldInstruction
        {
            private bool m_keepWaiting = true;

            public override bool keepWaiting => m_keepWaiting;

            public WaitForFadeOut(float duration, UICanvasGroupFader fader)
            {
                if (fader != null)
                {
                    fader.FadeOutDuration = duration;
                    fader.SetCompletedCallback(StopKeepWaiting);
                    fader.FadeOut();
                }
            }

            private void StopKeepWaiting()
            {
                m_keepWaiting = false;
            }
        }

        protected override IEnumerator OnAsyncLoad(string prevSceneName, string targetSceneName)
        {
            GameSetting.Instance.Input.BlockInput();

            yield return FadeIn(prevSceneName, FadeInDuration);

            ExitScene();

            AsyncOperation unloadAsync = UnloadSceneAsync(prevSceneName);
            while (!unloadAsync.isDone)
            {
                yield return null;
            }

            unloadAsync = null;
            yield return Resources.UnloadUnusedAssets();

            System.GC.Collect();

            AsyncOperation loadAsync = LoadSceneAsync(targetSceneName);
            while (!loadAsync.isDone)
            {
                yield return null;
            }

            ActiveTargetScene(SceneManager.GetActiveScene().name);

            yield return Delay(FadeDelayTime);
            yield return FadeOut(targetSceneName, FadeOutDuration);

            string loadingSceneName = SceneManager.GetActiveScene().name;
            unloadAsync = SceneManager.UnloadSceneAsync(loadingSceneName);
            while (!loadAsync.isDone)
            {
                yield return null;
            }

            GameSetting.Instance.Input.ResetInput();
            Scenes.XScene.ResetChangeSceneCoroutine();

            GlobalEvent.Send(GlobalEventType.CHANGE_GAME_MAIN_SCENE_COMPLETE);
        }

        private void ExitScene()
        {
            // Unload Prev
            Scenes.XScene gameScene = GameApp.FindGameScene();

            if (gameScene)
            {
                Log.Progress(LogTags.Scene, "OnAsyncSceneLoad::Exit Scene.");
                gameScene.Invoke("ExitScene", 0);
            }
        }

        //────────────────────────────────────────────────────────────────────────────────────────────────

        private CustomYieldInstruction FadeIn(string sceneName, float duration)
        {
            if (!Mathf.Approximately(0f, duration))
            {
                Log.Progress(LogTags.Scene, "OnAsyncSceneLoad::FadeIn Scene: {0}, Duration: {1}", sceneName, duration);
                return new WaitForFadeIn(duration, Fader);
            }

            return null;
        }

        private CustomYieldInstruction FadeOut(string sceneName, float duration)
        {
            if (!Mathf.Approximately(0f, duration))
            {
                Log.Progress(LogTags.Scene, "OnAsyncSceneLoad::FadeOut Scene: {0}, Duration: {1}", sceneName, duration);
                return new WaitForFadeOut(duration, Fader);
            }

            return null;
        }

        private YieldInstruction Delay(float delayTime)
        {
            if (delayTime > 0f)
            {
                Log.Progress(LogTags.Scene, "OnAsyncSceneLoad::Loading Delay Time: {0}", delayTime);
                return new WaitForSeconds(delayTime);
            }

            return null;
        }

        private AsyncOperation LoadSceneAsync(string sceneName)
        {
            Log.Progress(LogTags.Scene, "OnAsyncSceneLoad::LoadSceneAsync. SceneName: {0}", sceneName);
            return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        }

        private AsyncOperation UnloadSceneAsync(string sceneName)
        {
            Log.Progress(LogTags.Scene, "OnAsyncSceneLoad::UnloadSceneAsync. SceneName: {0}", sceneName);
            return SceneManager.UnloadSceneAsync(sceneName);
        }

        private void ActiveTargetScene(string sceneName)
        {
            Log.Progress(LogTags.Scene, "OnAsyncSceneLoad::Active TargetScene. SceneName: {0}", sceneName);
            Scene targetScene = SceneManager.GetSceneByName(sceneName);

            _ = SceneManager.SetActiveScene(targetScene);
        }
    }
}