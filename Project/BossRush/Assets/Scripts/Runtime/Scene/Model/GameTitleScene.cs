using Sirenix.OdinInspector;
using System.Collections;
using TeamSuneat.Setting;
using TeamSuneat.UserInterface;
using UnityEngine;
using UnityEngine.Events;

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
        public UISelectElement GameStartButton;
        public UISelectElement GameOptionButton;
        public UISelectElement GameExitButton;
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
            GameStartButton.OnPointerClickLeftEvent.AddListener(OnGameStart);
            GameExitButton.OnPointerClickLeftEvent.AddListener(OnGameExit);
            GameOptionButton.OnPointerClickLeftEvent.AddListener(OnGameOption);
        }

        private void OnGameStart()
        {
            SetInteractableButtons(false);
            StartChangeMainScene();
        }

        private void OnGameExit()
        {
            SetInteractableButtons(false);
            QuitGame();
        }

        private void OnGameOption()
        {
            ShowOptionPopup();
        }

        private void SetInteractableButtons(bool value)
        {
            // UISelectElement는 LockTrigger가 true이면 선택되지 않음
            if (GameStartButton != null)
            {
                GameStartButton.LockTrigger = !value;
            }

            if (GameExitButton != null)
            {
                GameExitButton.LockTrigger = !value;
            }

            if (GameOptionButton != null)
            {
                GameOptionButton.LockTrigger = !value;
            }
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

        private void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void ShowOptionPopup()
        {
            if (UIManager.Instance != null && UIManager.Instance.PopupManager != null)
            {
                UIManager.Instance.PopupManager.SpawnCenterPopup(UIPopupNames.GameOption, OnOptionPopupClosed);
            }
        }

        private void OnOptionPopupClosed(bool popupResult)
        {
            // 옵션 팝업이 닫힌 후 처리할 로직이 있다면 여기에 추가
        }
    }
}