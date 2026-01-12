using Lean.Pool;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    /// <summary>
    /// 자동배틀에서 팝업 UI 요소를 관리하기 위한 UIPopup 클래스.
    /// </summary>
    public class UIPopup : XBehaviour, IPoolable
    {
        // 핵심 UI 컴포넌트

        [FoldoutGroup("#UIPopup")]
        public UILocalizedText TitleText;

        [FoldoutGroup("#UIPopup")]
        public Button CancelButton;

        [FoldoutGroup("#UIPopup")]
        public Button BackdropButton;

        // 핸들러들

        private IUIPopupInputHandler _inputHandler;
        private IUIPopupFeedbackHandler _feedbackHandler;
        private IUIPopupCallbackHandler _callbackHandler;
        private IUIPopupInputBlockHandler _inputBlockHandler;
        private UIPopupFloatyHandler _floatyHandler;

        // 핵심 설정

        [FoldoutGroup("#UIPopup/Settings")]
        public bool NotCheckIfAllPopupsClosed;

        [FoldoutGroup("#UIPopup/Settings")]
        public bool UseFullScreenSize;

        // 속성

        public virtual UIPopupNames Name => UIPopupNames.None;

        // Unity 이벤트 메서드

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            TitleText ??= this.FindComponent<UILocalizedText>("Rect/Title Text");
            CancelButton ??= this.FindComponent<Button>("Rect/Cancel Button");
            BackdropButton ??= this.FindComponent<Button>("Backdrop Button");
        }

        public override void AutoNaming()
        {
            SetGameObjectName(string.Format("UI{0}Popup", Name.ToString()));
        }

        protected virtual void Awake()
        {
            if (CancelButton != null)
            {
                CancelButton.onClick.AddListener(OnClickCancelButton);
            }

            if (BackdropButton != null)
            {
                BackdropButton.onClick.AddListener(CloseWithFailure);
            }

            InitializeHandlers();
        }

        // 인터페이스 구현 (IPoolable)

        public void OnSpawn()
        {
            if (!UseFullScreenSize)
            {
                return;
            }

            rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);
            rectTransform.anchoredPosition = Vector2.zero;

            CanvasOrder hudCanvas = UIManager.Instance.GetCanvas(CanvasOrderNames.HUD);
            if (hudCanvas != null)
            {
                hudCanvas.SetEnabledRaycast(false);
            }
        }

        public virtual void OnDespawn()
        {
            _inputHandler?.Cleanup();
            _feedbackHandler?.Cleanup();
            _callbackHandler?.Cleanup();
            _inputBlockHandler?.Cleanup();
            _floatyHandler?.Cleanup();

            if (!UseFullScreenSize)
            {
                return;
            }

            CanvasOrder hudCanvas = UIManager.Instance.GetCanvas(CanvasOrderNames.HUD);
            if (hudCanvas != null)
            {
                hudCanvas.SetEnabledRaycast(true);
            }
        }

        // 이벤트 핸들러

        private void OnClickCancelButton()
        {
            CoroutineNextTimer(0.3f, CloseWithFailure);
        }

        // 공개 메서드

        public virtual void LogicUpdate()
        {
        }

        public virtual void Open()
        {
            LogInfo("팝업을 엽니다.");

            ConfigurePopupSettings(true);
            Activate();
            StartBlockInput();
            TriggerOpenFeedback();

            GlobalEvent<UIPopupNames>.Send(GlobalEventType.GAME_POPUP_OPEN, Name);
        }

        public void CloseWithSuccess()
        {
            LogClosePopup(true);
            OnClose(true);
            Despawn();

            GlobalEvent<UIPopupNames>.Send(GlobalEventType.GAME_POPUP_CLOSE, Name);
        }

        public void CloseWithFailure()
        {
            LogClosePopup(false);
            OnClose(false);
            Despawn();

            GlobalEvent<UIPopupNames>.Send(GlobalEventType.GAME_POPUP_CLOSE, Name);
        }

        public virtual void OnClose(bool result)
        {
            UIManager.Instance.DetailsManager.Clear();
            ConfigurePopupSettings(false);
            InvokeCloseCallback(result);
            TriggerCloseFeedback();
        }

        public virtual void Despawn()
        {
            if (IsDestroyed)
            {
                return;
            }

            ResourcesManager.Despawn(gameObject);
        }

        public virtual void Activate()
        {
            SelectFirstSlotEvent();
        }

        protected virtual void CloseAll()
        {
            UIManager.Instance.PopupManager.CloseAllPopups();
        }

        protected virtual void RefreshTitleText()
        {
            if (TitleText == null)
            {
                return;
            }

            TitleText.SetText(Name.GetLocalizedString());
        }

        public virtual void SelectFirstSlotEvent()
        {
            LogInfo("팝업에서 첫 번째 슬롯 이벤트를 선택합니다.");
        }

        #region 핸들러 위임 메서드들

        public virtual void InputSetup(int maxIndex)
        {
            _inputHandler?.SetupInput(maxIndex);
        }

        public virtual void InputActivateTarget(int index)
        {
            _inputHandler?.ActivateTarget(index);
        }

        public virtual void InputNextTarget()
        {
            _inputHandler?.NextTarget();
        }

        public virtual void InputPrevTarget()
        {
            _inputHandler?.PrevTarget();
        }

        public virtual void InputSelectTarget(int index)
        {
            _inputHandler?.SelectTarget(index);
        }

        public void RegisterCloseCallback(UnityAction<bool> action)
        {
            _callbackHandler?.RegisterCloseCallback(action);
        }

        public void UnregisterCloseCallback(UnityAction<bool> action)
        {
            _callbackHandler?.UnregisterCloseCallback(action);
        }

        public void SpawnFloatyText(string content, UIFloatyMoveNames moveType)
        {
            _floatyHandler?.SpawnFloatyText(content, moveType);
        }

        public void SpawnFloatyGetStringText(string content, UIFloatyMoveNames moveType)
        {
            _floatyHandler?.SpawnFloatyGetStringText(content, moveType);
        }

        #endregion 핸들러 위임 메서드들

        #region 피드백

        private void TriggerOpenFeedback()
        {
            _feedbackHandler?.TriggerOpenFeedback();
        }

        private void TriggerCloseFeedback()
        {
            _feedbackHandler?.TriggerCloseFeedback();
        }

        #endregion 피드백

        #region 콜백

        private void InvokeCloseCallback(bool result)
        {
            _callbackHandler?.InvokeCloseCallback(result);
        }

        #endregion 콜백

        #region 입력 차단

        protected void StartBlockInput()
        {
            _inputBlockHandler?.StartBlockInput();
        }

        #endregion 입력 차단

        #region 내부 메서드

        private void InitializeHandlers()
        {
            _inputHandler = GetComponent<IUIPopupInputHandler>();
            _feedbackHandler = GetComponent<IUIPopupFeedbackHandler>();
            _callbackHandler = GetComponent<IUIPopupCallbackHandler>();
            _inputBlockHandler = GetComponent<IUIPopupInputBlockHandler>();
            _floatyHandler = GetComponent<UIPopupFloatyHandler>();

            _inputHandler?.Initialize();
            _feedbackHandler?.Initialize();
            _callbackHandler?.Initialize();
            _inputBlockHandler?.Initialize();
            _floatyHandler?.Initialize();
        }

        private void ConfigurePopupSettings(bool isOpening)
        {
            _inputBlockHandler?.ConfigurePopupSettings(isOpening);
        }

        #endregion 내부 메서드

        #region 로그

        protected virtual void LogInfo(string content)
        {
            if (!Log.LevelInfo)
            {
                return;
            }

            Log.Info(LogTags.UI_Popup, $"{Name.ToLogString()} {content}");
        }

        protected void LogClosePopup(bool result)
        {
            if (!Log.LevelInfo)
            {
                return;
            }

            Log.Info(LogTags.UI_Popup, $"{Name.ToLogString()} 팝업을 닫습니다. 결과: {result.ToBoolString()}");
        }

        #endregion 로그
    }
}