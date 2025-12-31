using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TeamSuneat.Scenes
{
    public abstract class XScene : MonoBehaviour
    {
        public static bool IsChangeScene;
        private static Coroutine _changeSceneCoroutine;

        protected static string PrevSceneName { get; set; }

        protected static string TargetSceneName { get; set; }

        private const string LOADING_SCENE_NAME = "GameLoading";

        //─────────────────────────────────────────────────────────────────────────────────────────────────────────

        protected abstract void OnCreateScene();

        protected abstract void OnEnterScene();

        protected abstract void OnExitScene();

        protected abstract void OnDestroyScene();

        //─────────────────────────────────────────────────────────────────────────────────────────────────────────
        private void Awake()
        {
            OnCreateScene();
        }

        private void Start()
        {
            OnEnterScene();
        }

        private void OnDisable()
        {
            OnExitScene();
        }

        private void OnDestroy()
        {
            OnDestroyScene();
        }

        public bool DetermineChangeScene(string targetSceneName)
        {
            if (_changeSceneCoroutine != null)
            {
                Log.Warning(LogTags.Scene, "변경 중인 씬({0})이 존재합니다. 새로운 씬({1})으로 변경할 수 없습니다.", TargetSceneName, targetSceneName);
                return false;
            }

            return true;
        }

        protected virtual void CleanupCurrentScene()
        {
        }

        public virtual void ChangeScene(string targetSceneName)
        {
            if (_changeSceneCoroutine == null)
            {
                Log.Progress(LogTags.Scene, "{0} 씬으로 씬을 전환합니다.", targetSceneName);

                IsChangeScene = true;

                var currentScene = SceneManager.GetActiveScene();

                PrevSceneName = currentScene.name;
                TargetSceneName = targetSceneName;

                _changeSceneCoroutine = StartCoroutine(CoChangeSceneImpl());
            }
        }

        private IEnumerator CoChangeSceneImpl()
        {
            yield return null;

            Log.Progress(LogTags.Scene, "로딩 씬을 불러옵니다.");

            // 로딩 씬을 비동기적으로 로드
            AsyncOperation loadSceneOp = SceneManager.LoadSceneAsync(LOADING_SCENE_NAME, LoadSceneMode.Additive);

            // 씬 로드가 완료될 때까지 대기
            while (!loadSceneOp.isDone)
            {
                yield return null;
            }

            Log.Progress(LogTags.Scene, "로딩 씬을 활성화합니다.");

            Scene loadingScene = SceneManager.GetSceneByName(LOADING_SCENE_NAME);
            if (loadingScene.isLoaded) // 로딩된 씬이 유효한지 확인
            {
                SceneManager.SetActiveScene(loadingScene);
            }
            else
            {
                Log.Error("로딩 씬을 활성화할 수 없습니다. 씬이 제대로 로드되지 않았습니다: {0}", LOADING_SCENE_NAME);
                yield break;
            }

            CleanupCurrentScene();

            ISceneLoader sceneLoader = FindFirstObjectByType<ISceneLoader>();
            if (!sceneLoader)
            {
                Log.Error("ISceneLoader를 구현하는 객체를 찾을 수 없습니다.");
            }
            else
            {
                Log.Progress(LogTags.Scene, "{0} 씬을 SceneLoader를 통해 불러옵니다.", TargetSceneName);
                yield return sceneLoader.Load(PrevSceneName, TargetSceneName);
            }
        }

        public static void ResetChangeSceneCoroutine()
        {
            _changeSceneCoroutine = null;

            PrevSceneName = null;
            TargetSceneName = null;
            IsChangeScene = false;
        }
    }
}