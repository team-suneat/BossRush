using UnityEngine;

namespace TeamSuneat
{
    public class EntryPoint
    {
        public static XGameApp AppBase
        {
            get; private set;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void RuntimeInitializeBeforeSceneLoad()
        {
            Scenes.XScene.ResetChangeSceneCoroutine();

            AppBase = Object.FindFirstObjectByType<XGameApp>();
            if (AppBase == null)
            {
                // [EntryPoint] 전용 GameApp 오브젝트를 발견할 수 없습니다. GameApp이 자동 생성 되었습니다.
                AppBase = GameApp.Create();
            }

            Object.DontDestroyOnLoad(AppBase.gameObject);

            AppBase.SendMessage("ApplicationStart");
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void RuntimeInitializeAfterSceneLoad()
        {
            AppBase.SendMessage("ApplicationStarted");
        }
    }
}