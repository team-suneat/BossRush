using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using TeamSuneat.Setting;
using UnityEngine;

namespace TeamSuneat.UserInterface
{
    public class UCGameOptionVideo : UCGameOptionBase
    {
        [FoldoutGroup("#Video")]
        [SerializeField] private UISelectButton _fullScreenButton;
        [FoldoutGroup("#Video")]
        [SerializeField] private UISelectButton _borderlessButton;
        [FoldoutGroup("#Video")]
        [SerializeField] private UISelectButton _resolutionButton;
        [FoldoutGroup("#Video")]
        [SerializeField] private UISelectButton _vSyncButton;
        [FoldoutGroup("#Video")]
        [SerializeField] private UISelectButton _defaultValuesButton;
        [FoldoutGroup("#Video")]
        [SerializeField] private UISelectButton _backButton;

        [FoldoutGroup("#Video/Buttons")]
        [SerializeField] private GameObject _borderlessButton2Object;
        [FoldoutGroup("#Video/Buttons")]
        [SerializeField] private UISelectButton _prevResolutionButton;
        [FoldoutGroup("#Video/Buttons")]
        [SerializeField] private UISelectButton _nextResolutionButton;
        [FoldoutGroup("#Video/Buttons")]
        [SerializeField] private UISelectButton _applyResolutionButton;
        [FoldoutGroup("#Video/Buttons")]
        [SerializeField] private UISelectButton _prevDisplayButton;
        [FoldoutGroup("#Video/Buttons")]
        [SerializeField] private UISelectButton _nextDisplayButton;

        [FoldoutGroup("#Video/Text")]
        [SerializeField] private UILocalizedText _resolutionText;
        [FoldoutGroup("#Video/Text")]
        [SerializeField] private UILocalizedText _displayText;
        [FoldoutGroup("#Video/Text")]
        [SerializeField] private UILocalizedText _applyText;

        private int _resolutionSelectedIndex;
        private int _resolutionCurrentIndex;
        private float _currentInputWaitTime;
        private Coroutine _moveDisplayCoroutine;
        private Tweener _tweenerText;
        private Tweener _tweener;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _indexer ??= GetComponentInChildren<UISelectElementIndexer>();

            _fullScreenButton = this.FindComponent<UISelectButton>("#Content/FullScreen Button");
            _borderlessButton = this.FindComponent<UISelectButton>("#Content/Borderless Button");
            _resolutionButton = this.FindComponent<UISelectButton>("#Content/Resolution Button");
            _vSyncButton = this.FindComponent<UISelectButton>("#Content/VSync Button");
            _defaultValuesButton = this.FindComponent<UISelectButton>("#Content/Default Values Button");
            _backButton = this.FindComponent<UISelectButton>("#Content/Back Button");

            _borderlessButton2Object = this.FindGameObject("#Content2/#Borderless");
            _prevResolutionButton = this.FindComponent<UISelectButton>("#Content2/#Resolution/Prev Resolution Button");
            _nextResolutionButton = this.FindComponent<UISelectButton>("#Content2/#Resolution/Next Resolution Button");
            _applyResolutionButton = this.FindComponent<UISelectButton>("#Content2/#Resolution/Apply Resolution Button");
            _prevDisplayButton = this.FindComponent<UISelectButton>("#Content2/#Default Display/Prev Display Button");
            _nextDisplayButton = this.FindComponent<UISelectButton>("#Content2/#Default Display/Next Display Button");
        }

        protected override void OnStart()
        {
            base.OnStart();

            _fullScreenButton?.OnPointerClickLeftEvent.AddListener(SwitchFullScreen);
            _borderlessButton?.OnPointerClickLeftEvent.AddListener(SwitchBorderless);
            _resolutionButton?.OnPointerClickLeftEvent.AddListener(ApplyResolution);
            _prevResolutionButton?.OnPointerClickLeftEvent.AddListener(PrevResolution);
            _nextResolutionButton?.OnPointerClickLeftEvent.AddListener(NextResolution);
            _applyResolutionButton?.OnPointerClickLeftEvent.AddListener(ApplyResolution);

            if (GameSetting.Instance.Video.DisplayCount > 1)
            {
                _prevDisplayButton?.OnPointerClickLeftEvent.AddListener(PrevDisplay);
                _nextDisplayButton?.OnPointerClickLeftEvent.AddListener(NextDisplay);
            }
            else
            {
                _prevDisplayButton?.SetActive(false);
                _nextDisplayButton?.SetActive(false);
            }

            _vSyncButton?.OnPointerClickLeftEvent.AddListener(SwitchVSync);
            _defaultValuesButton?.OnPointerClickLeftEvent.AddListener(SetDefaultValues);
            _backButton?.OnPointerClickLeftEvent.AddListener(Hide);
        }

        protected override void OnDisabled()
        {
            base.OnDisabled();
            StopTweeners();
        }

        //

        public void LogicUpdate()
        {
            if (!ActiveSelf) return;

            if (_currentInputWaitTime > GameDefine.INPUT_WAIT_TIME)
            {
                TSInputManager.Instance.GetInputState();

                if (TSInputManager.Instance.CheckButtonState(ActionNames.UIMoveLeft, ButtonStates.ButtonDown))
                {
                    if (UIManager.Instance.SelectController.CurrentIndex == _resolutionButton.SelectIndex)
                    {
                        PrevResolution();
                    }
                    else if (UIManager.Instance.SelectController.CurrentIndex == _prevDisplayButton.SelectIndex)
                    {
                        PrevDisplay();
                    }
                }
                else if (TSInputManager.Instance.CurrentControllerType == Rewired.ControllerType.Joystick && TSInputManager.Instance.PrimaryMovement.x < -0.5f)
                {
                    if (UIManager.Instance.SelectController.CurrentIndex == _resolutionButton.SelectIndex)
                    {
                        PrevResolution();
                    }
                    else if (UIManager.Instance.SelectController.CurrentIndex == _prevDisplayButton.SelectIndex)
                    {
                        PrevDisplay();
                    }

                    _currentInputWaitTime = 0f;
                }
                else if (TSInputManager.Instance.CurrentControllerType == Rewired.ControllerType.Joystick && TSInputManager.Instance.RightPadMovement.x < -0.5f)
                {
                    if (UIManager.Instance.SelectController.CurrentIndex == _resolutionButton.SelectIndex)
                    {
                        PrevResolution();
                    }
                    else if (UIManager.Instance.SelectController.CurrentIndex == _prevDisplayButton.SelectIndex)
                    {
                        PrevDisplay();
                    }

                    _currentInputWaitTime = 0f;
                }

                if (TSInputManager.Instance.CheckButtonState(ActionNames.UIMoveRight, ButtonStates.ButtonDown))
                {
                    if (UIManager.Instance.SelectController.CurrentIndex == _resolutionButton.SelectIndex)
                    {
                        NextResolution();
                    }
                    else if (UIManager.Instance.SelectController.CurrentIndex == _nextDisplayButton.SelectIndex)
                    {
                        NextDisplay();
                    }
                }
                else if (TSInputManager.Instance.CurrentControllerType == Rewired.ControllerType.Joystick && TSInputManager.Instance.PrimaryMovement.x > 0.5f)
                {
                    if (UIManager.Instance.SelectController.CurrentIndex == _resolutionButton.SelectIndex)
                    {
                        NextResolution();
                    }
                    else if (UIManager.Instance.SelectController.CurrentIndex == _nextDisplayButton.SelectIndex)
                    {
                        NextDisplay();
                    }

                    _currentInputWaitTime = 0f;
                }
                else if (TSInputManager.Instance.CurrentControllerType == Rewired.ControllerType.Joystick && TSInputManager.Instance.RightPadMovement.x > 0.5f)
                {
                    if (UIManager.Instance.SelectController.CurrentIndex == _resolutionButton.SelectIndex)
                    {
                        NextResolution();
                    }
                    else if (UIManager.Instance.SelectController.CurrentIndex == _nextDisplayButton.SelectIndex)
                    {
                        NextDisplay();
                    }

                    _currentInputWaitTime = 0f;
                }
            }

            _currentInputWaitTime += Time.unscaledDeltaTime;
        }

        protected override void OnShow()
        {
            base.OnShow();

            LoadResolutions();
            LoadDisplay();
            SetupResolutionIndexes();
            SetResolutionText(false);
            SetDisplayText();

            _applyText?.SetActive(false);

            HideUnderlineEventButton(_fullScreenButton);
            HideUnderlineEventButton(_borderlessButton);
            HideUnderlineEventButton(_resolutionButton);
            HideUnderlineEventButton(_vSyncButton);

            SetActiveEventButton(_fullScreenButton, GameSetting.Instance.Video.IsFullScreen);
            SetActiveEventButton(_borderlessButton, GameSetting.Instance.Video.IsBorderless);
            SetActiveEventButton(_vSyncButton, GameSetting.Instance.Video.UseVSync);

            SetActiveBorderlessButton();
        }

        private void SwitchVSync()
        {
            GameSetting.Instance.Video.SwitchVSync();
            SetActiveEventButton(_vSyncButton, GameSetting.Instance.Video.UseVSync);
        }

        private void SwitchFullScreen()
        {
            GameSetting.Instance.Video.SwitchFullScreen();
            SetActiveEventButton(_fullScreenButton, GameSetting.Instance.Video.IsFullScreen);
            SetActiveBorderlessButton();
        }

        private void SwitchBorderless()
        {
            GameSetting.Instance.Video.SwitchBorderless();
            SetActiveEventButton(_borderlessButton, GameSetting.Instance.Video.IsBorderless);
        }

        #region Resolution

        private void LoadResolutions()
        {
            GameSetting.Instance.Video.LoadResolutions();
        }

        public void PrevResolution()
        {
            if (_resolutionSelectedIndex > 0)
            {
                _resolutionSelectedIndex -= 1;
            }
            else
            {
                _resolutionSelectedIndex = GameSetting.Instance.Video.LastResolutionIndex;
            }

            DoPunchScale(_prevResolutionButton.transform);
            SetResolutionText();

            _applyText?.SetActive(_resolutionCurrentIndex != _resolutionSelectedIndex);
        }

        public void NextResolution()
        {
            if (_resolutionSelectedIndex < GameSetting.Instance.Video.LastResolutionIndex)
            {
                _resolutionSelectedIndex += 1;
            }
            else
            {
                _resolutionSelectedIndex = 0;
            }

            DoPunchScale(_nextResolutionButton.transform);
            SetResolutionText();

            _applyText?.SetActive(_resolutionCurrentIndex != _resolutionSelectedIndex);
        }

        private void ApplyResolution()
        {
            _applyText?.SetActive(false);
            _resolutionCurrentIndex = _resolutionSelectedIndex;
            GameSetting.Instance.Video.SetResolution(_resolutionSelectedIndex);
        }

        private void SetResolutionText(bool usePunchScale = true)
        {
            if (_resolutionText != null)
            {
                Vector2Int resolution;
                if (GameSetting.Instance.Video.Resolutions.IsValid(_resolutionSelectedIndex))
                {
                    resolution = GameSetting.Instance.Video.Resolutions[_resolutionSelectedIndex];
                }
                else if (GameSetting.Instance.Video.Resolutions.IsValid())
                {
                    resolution = GameSetting.Instance.Video.Resolutions[0];
                }
                else
                {
                    return;
                }

                _resolutionText.SetText($"{resolution.x} x {resolution.y}");
                if (usePunchScale)
                {
                    DoPunchScale(_resolutionText);
                }
            }
        }

        private void SetupResolutionIndexes()
        {
            _resolutionSelectedIndex = GameSetting.Instance.Video.CurrentResolutionIndex;
            _resolutionCurrentIndex = GameSetting.Instance.Video.CurrentResolutionIndex;
        }

        #endregion Resolution

        #region Display

        private void LoadDisplay()
        {
            GameSetting.Instance.Video.LoadDisplay();
        }

        private void PrevDisplay()
        {
            if (_moveDisplayCoroutine != null) return;

            if (GameSetting.Instance.Video.TryPrevDisplay())
            {
                DoPunchScale(_prevDisplayButton.transform);
                SetDisplayText();
                _moveDisplayCoroutine = StartXCoroutine(ProcessMoveDisplay());
            }
        }

        private void NextDisplay()
        {
            if (_moveDisplayCoroutine != null) return;

            if (GameSetting.Instance.Video.TryNextDisplay())
            {
                DoPunchScale(_nextDisplayButton.transform);
                SetDisplayText();
                _moveDisplayCoroutine = StartXCoroutine(ProcessMoveDisplay());
            }
        }

        private IEnumerator ProcessMoveDisplay()
        {
            DisplayInfo currentDisplay = GameSetting.Instance.Video.CurrentDisplay;

            yield return Screen.MoveMainWindowTo(currentDisplay, new Vector2Int(currentDisplay.width / 2, currentDisplay.height / 2));

            GameSetting.Instance.Video.OnMoveDisplay();
            SetupResolutionIndexes();
            ApplyResolution();
            SetResolutionText();

            _moveDisplayCoroutine = null;
        }

        private void SetDisplayText()
        {
            if (_displayText != null)
            {
                _displayText.SetText(GetDisplayName());
            }
        }

        private string GetDisplayName()
        {
            return GameSetting.Instance.Video.CurrentDisplay.name;
        }

        #endregion Display

        #region Set Default

        private void SetDefaultValues()
        {
            GameSetting.Instance.Video.ResetFullScreen();
            GameSetting.Instance.Video.ResetBorderless();
            GameSetting.Instance.Video.RefreshScreenMode();
            GameSetting.Instance.Video.ResetVSync();
            GameSetting.Instance.Video.ResetResolution();

            SetActiveEventButton(_fullScreenButton, GameSetting.Instance.Video.IsFullScreen);
            SetActiveEventButton(_borderlessButton, GameSetting.Instance.Video.IsBorderless);
            SetActiveEventButton(_vSyncButton, GameSetting.Instance.Video.UseVSync);

            _resolutionSelectedIndex = GameSetting.Instance.Video.CurrentResolutionIndex;
            _resolutionCurrentIndex = GameSetting.Instance.Video.CurrentResolutionIndex;

            SetResolutionText();
            SetActiveBorderlessButton();
        }

        #endregion Set Default

        #region Event Button

        private void SetActiveBorderlessButton()
        {
            _borderlessButton.SetActive(GameSetting.Instance.Video.IsFullScreen);
            _borderlessButton2Object.SetActive(GameSetting.Instance.Video.IsFullScreen);
        }

        #endregion Event Button

        private void DoPunchScale(UILocalizedText parent)
        {
            _tweenerText?.Kill(true);

            parent.SetTextColor(GameColors.CreamIvory);

            _tweenerText = parent.transform.DOPunchScale(Vector3.one * 0.1f, 0.1f);
            _tweenerText.SetUpdate(true);
            _tweenerText.onComplete += () =>
            {
                parent.ResetTextColor();
                parent.localScale = Vector3.one;
                _tweenerText = null;
            };
        }

        private void DoPunchScale(Transform parent)
        {
            _tweener?.Kill(true);
            _tweener = parent.DOPunchScale(Vector3.one * 0.1f, 0.1f);
            _tweener.SetUpdate(true);
            _tweener.onComplete += () =>
            {
                parent.localScale = Vector3.one;
                _tweener = null;
            };
        }

        private void StopTweeners()
        {
            if (_tweener != null)
            {
                _tweener.Kill();
                _tweener = null;
            }

            if (_tweenerText != null)
            {
                _tweenerText.Kill();
                _tweenerText = null;
            }
        }
    }
}