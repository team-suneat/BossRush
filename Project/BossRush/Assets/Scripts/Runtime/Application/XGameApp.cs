using TeamSuneat.Scenes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TeamSuneat
{
    public abstract class XGameApp : MonoBehaviour
    {
        public static string ActiveSceneName
        {
            get
            {
                Scene scene = SceneManager.GetActiveScene();
                return scene.name;
            }
        }

        public static XScene FindGameScene()
        {
            return FindFirstObjectByType<XScene>();
        }

        protected virtual void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
        }

        protected virtual void OnSceneUnloaded(Scene scene)
        {
        }

        /// 어플리케이션이 초기화 된 후 콜 된다.
        protected abstract void OnApplicationStart();

        /// 최초 씬의 로딩이 끝난 후 콜 된다.
        protected abstract void OnApplicationStarted();

        protected virtual void OnApplicationPaused()
        {
        }

        protected virtual void OnApplicationResume()
        {
        }

        private void ApplicationStart()
        {
            // EntryPoint Message에 의해 Call 된다.
            OnApplicationStart();

            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        private void ApplicationStarted()
        {
            // EntryPoint Message에 의해 Call 된다.
            OnApplicationStarted();
        }

        private void OnApplicationPause(bool status)
        {
            if (status)
            {
                OnApplicationPaused();
            }
            else
            {
                OnApplicationResume();
            }
        }

        private void OnApplicationFocus(bool status)
        {
            if (status)
            {
                Log.Progress("Application Focused");
            }
            else
            {
                Log.Progress("Application Lost Focus");
            }
        }
    }
}