using Sirenix.OdinInspector;
using System.Collections;
using TeamSuneat.Setting;
using TeamSuneat.UserInterface;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TeamSuneat.Scenes
{
    /// <summary>
    /// 게임 타이틀 씬을 관리하는 클래스
    /// </summary>
    public class GameTitleScene : XScene
    {
        [Title("#Settings")]
        public float DelayTimeForChangeScene;

        [Title("#Component")]
        public Button GameStartButton;
        public UIGauge LoadingGauge;

        private bool _isChangingScene;

        protected override void OnCreateScene()
        {
            RegisterButtonEvent();
            SetInteractableButtons(false);
        }

        protected override void OnEnterScene()
        {
            StartCoroutine(WaitForInitialize());
        }

        protected override void OnExitScene()
        {
        }

        protected override void OnDestroyScene()
        {
        }

        //───────────────────────────────────────────────────────────────────────────

        private IEnumerator WaitForInitialize()
        {
            yield return new WaitUntil(() => { return GameApp.Instance.IsInitialized; });
            SetInteractableButtons(true);
        }

        private void RegisterButtonEvent()
        {
            GameStartButton.onClick.AddListener(OnGameStart);
        }

        private void OnGameStart()
        {
            SetInteractableButtons(false);
            StartChangeMainScene();
        }

        private void SetInteractableButtons(bool value)
        {
            GameStartButton.interactable = value;
        }

        //───────────────────────────────────────────────────────────────────────────

        public void StartChangeMainScene()
        {
            StartChangeScene(ChangeMainScene);
        }

        // 씬 전환 메서드들

        private void StartChangeScene(UnityAction changeSceneAction)
        {
            if (_isChangingScene) { return; }

            _isChangingScene = true;
            GameSetting.Instance.Input.BlockUIInput();
            if (DelayTimeForChangeScene > 0)
            {
                StartCoroutine(ProcessChangeScene(changeSceneAction));
            }
            else
            {
                changeSceneAction.Invoke();
            }
        }

        private IEnumerator ProcessChangeScene(UnityAction changeSceneAction)
        {
            yield return new WaitForSeconds(DelayTimeForChangeScene);
            changeSceneAction.Invoke();
        }

        private void ChangeMainScene()
        {
            ChangeToScene("GameMain");
        }

        private void ChangeToScene(string sceneName)
        {
            GameSetting.Instance.Input.UnblockUIInput();

            if (DetermineChangeScene(sceneName))
            {
                ChangeScene(sceneName);
            }
        }
    }
}