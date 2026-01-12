using Sirenix.OdinInspector;
using TeamSuneat.Setting;
using UnityEngine;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    public class UCGameOptionAudio : UCGameOptionBase
    {
        [FoldoutGroup("#Audio/Buttons")]
        [SerializeField] private UISelectElement _muteMusicButton;
        [FoldoutGroup("#Audio/Buttons")]
        [SerializeField] private UISelectElement _muteSFXButton;
        [FoldoutGroup("#Audio/Buttons")]
        [SerializeField] private UISelectElement _masterVolumeButton;
        [FoldoutGroup("#Audio/Buttons")]
        [SerializeField] private UISelectElement _musicVolumeButton;
        [FoldoutGroup("#Audio/Buttons")]
        [SerializeField] private UISelectElement _sfxVolumeButton;
        [FoldoutGroup("#Audio/Buttons")]
        [SerializeField] private UISelectElement _defaultValuesButton;
        [FoldoutGroup("#Audio/Buttons")]
        [SerializeField] private UISelectElement _backButton;

        [FoldoutGroup("#Audio")]
        [SerializeField] private UILocalizedText _masterVolumeText;
        [FoldoutGroup("#Audio")]
        [SerializeField] private UILocalizedText _musicVolumeText;
        [FoldoutGroup("#Audio")]
        [SerializeField] private UILocalizedText _sfxVolumeText;

        [FoldoutGroup("#Audio/Slider")]
        [SerializeField] private Slider _masterVolumeSlider;
        [FoldoutGroup("#Audio/Slider")]
        [SerializeField] private Slider _musicVolumeSlider;
        [FoldoutGroup("#Audio/Slider")]
        [SerializeField] private Slider _sfxVolumeSlider;

        private float _currentInputWaitTime;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _indexer ??= this.GetComponentInChildren<UISelectElementIndexer>();
            _muteMusicButton = this.FindComponent<UISelectElement>("#Content/MuteMusic Button");
            _muteSFXButton = this.FindComponent<UISelectElement>("#Content/MuteSFX Button");
            _masterVolumeButton = this.FindComponent<UISelectElement>("#Content/MasterVolume Button");
            _musicVolumeButton = this.FindComponent<UISelectElement>("#Content/MusicVolume Button");
            _sfxVolumeButton = this.FindComponent<UISelectElement>("#Content/SFXVolume Button");
            _defaultValuesButton = this.FindComponent<UISelectElement>("#Content/Default Values Button");
            _backButton = this.FindComponent<UISelectElement>("#Content/Back Button");

            _masterVolumeText = this.FindComponent<UILocalizedText>("#Content/MasterVolume Button/Volume Text");
            _musicVolumeText = this.FindComponent<UILocalizedText>("#Content/MusicVolume Button/Volume Text");
            _sfxVolumeText = this.FindComponent<UILocalizedText>("#Content/SFXVolume Button/Volume Text");

            _masterVolumeSlider = this.FindComponent<Slider>("#Content2/MasterVolume Slider");
            _musicVolumeSlider = this.FindComponent<Slider>("#Content2/MusicVolume Slider");
            _sfxVolumeSlider = this.FindComponent<Slider>("#Content2/SFXVolume Slider");
        }

        protected override void OnStart()
        {
            base.OnStart();

            _muteMusicButton?.OnPointerClickLeftEvent.AddListener(SwitchMuteMusic);
            _muteSFXButton?.OnPointerClickLeftEvent.AddListener(SwitchMuteSFX);
            _defaultValuesButton?.OnPointerClickLeftEvent.AddListener(SetDefaultValues);
            _backButton?.OnPointerClickLeftEvent.AddListener(Hide);

            _masterVolumeText?.SetText(ValueStringEx.GetPercentString(GameSetting.Instance.Audio.MasterVolume));
            _musicVolumeText?.SetText(ValueStringEx.GetPercentString(GameSetting.Instance.Audio.MusicVolume));
            _sfxVolumeText?.SetText(ValueStringEx.GetPercentString(GameSetting.Instance.Audio.SFXVolume));

            if (_masterVolumeSlider != null)
            {
                _masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
                _masterVolumeSlider.value = GameSetting.Instance.Audio.MasterVolume;
            }

            if (_musicVolumeSlider != null)
            {
                _musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
                _musicVolumeSlider.value = GameSetting.Instance.Audio.MusicVolume;
            }

            if (_sfxVolumeSlider != null)
            {
                _sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
                _sfxVolumeSlider.value = GameSetting.Instance.Audio.SFXVolume;
            }
        }

        public void LogicUpdate()
        {
            if (!ActiveSelf) return;

            if (_currentInputWaitTime > 0.1f)
            {
                TSInputManager.Instance.GetInputState();

                if (TSInputManager.Instance.CheckButtonState(ActionNames.UIMoveLeft, ButtonStates.ButtonPressed))
                {
                    if (UIManager.Instance.SelectController.CurrentIndex == _masterVolumeButton.SelectIndex) { SubtractMasterVolume(); }
                    else if (UIManager.Instance.SelectController.CurrentIndex == _musicVolumeButton.SelectIndex) { SubtractMusicVolume(); }
                    else if (UIManager.Instance.SelectController.CurrentIndex == _sfxVolumeButton.SelectIndex) { SubtractSFXVolume(); }

                    _currentInputWaitTime = 0f;
                }
                if (TSInputManager.Instance.PrimaryMovement.x < -TSInputManager.ThresholdUI.x)
                {
                    if (UIManager.Instance.SelectController.CurrentIndex == _masterVolumeButton.SelectIndex) { SubtractMasterVolume(); }
                    else if (UIManager.Instance.SelectController.CurrentIndex == _musicVolumeButton.SelectIndex) { SubtractMusicVolume(); }
                    else if (UIManager.Instance.SelectController.CurrentIndex == _sfxVolumeButton.SelectIndex) { SubtractSFXVolume(); }

                    _currentInputWaitTime = 0f;
                }
                else if (TSInputManager.Instance.RightPadMovement.x < -TSInputManager.ThresholdUI.x)
                {
                    if (UIManager.Instance.SelectController.CurrentIndex == _masterVolumeButton.SelectIndex) { SubtractMasterVolume(); }
                    else if (UIManager.Instance.SelectController.CurrentIndex == _musicVolumeButton.SelectIndex) { SubtractMusicVolume(); }
                    else if (UIManager.Instance.SelectController.CurrentIndex == _sfxVolumeButton.SelectIndex) { SubtractSFXVolume(); }

                    _currentInputWaitTime = 0f;
                }

                if (TSInputManager.Instance.CheckButtonState(ActionNames.UIMoveRight, ButtonStates.ButtonPressed))
                {
                    if (UIManager.Instance.SelectController.CurrentIndex == _masterVolumeButton.SelectIndex) { AddMasterVolume(); }
                    else if (UIManager.Instance.SelectController.CurrentIndex == _musicVolumeButton.SelectIndex) { AddMusicVolume(); }
                    else if (UIManager.Instance.SelectController.CurrentIndex == _sfxVolumeButton.SelectIndex) { AddSFXVolume(); }

                    _currentInputWaitTime = 0f;
                }
                else if (TSInputManager.Instance.PrimaryMovement.x > TSInputManager.ThresholdUI.x)
                {
                    if (UIManager.Instance.SelectController.CurrentIndex == _masterVolumeButton.SelectIndex) { AddMasterVolume(); }
                    else if (UIManager.Instance.SelectController.CurrentIndex == _musicVolumeButton.SelectIndex) { AddMusicVolume(); }
                    else if (UIManager.Instance.SelectController.CurrentIndex == _sfxVolumeButton.SelectIndex) { AddSFXVolume(); }

                    _currentInputWaitTime = 0f;
                }
                else if (TSInputManager.Instance.RightPadMovement.x > TSInputManager.ThresholdUI.x)
                {
                    if (UIManager.Instance.SelectController.CurrentIndex == _masterVolumeButton.SelectIndex) { AddMasterVolume(); }
                    else if (UIManager.Instance.SelectController.CurrentIndex == _musicVolumeButton.SelectIndex) { AddMusicVolume(); }
                    else if (UIManager.Instance.SelectController.CurrentIndex == _sfxVolumeButton.SelectIndex) { AddSFXVolume(); }

                    _currentInputWaitTime = 0f;
                }
            }

            _currentInputWaitTime += Time.unscaledDeltaTime;
        }

        protected override void OnShow()
        {
            base.OnShow();

            HideUnderlineEventButton(_muteMusicButton);
            HideUnderlineEventButton(_muteSFXButton);

            SetActiveEventButton(_muteMusicButton, GameSetting.Instance.Audio.MuteMusic);
            SetActiveEventButton(_muteSFXButton, GameSetting.Instance.Audio.MuteSFX);
        }

        private void SwitchMuteMusic()
        {
            GameSetting.Instance.Audio.MuteMusic = !GameSetting.Instance.Audio.MuteMusic;
            SetActiveEventButton(_muteMusicButton, GameSetting.Instance.Audio.MuteMusic);
        }

        private void SwitchMuteSFX()
        {
            GameSetting.Instance.Audio.MuteSFX = !GameSetting.Instance.Audio.MuteSFX;
            SetActiveEventButton(_muteSFXButton, GameSetting.Instance.Audio.MuteSFX);
        }

        private void AddMasterVolume()
        {
            float targetVolume = GameSetting.Instance.Audio.MasterVolume + 0.01f;
            GameSetting.Instance.Audio.MasterVolume = Mathf.Min(targetVolume.RoundWithDigits(2), 1);

            _masterVolumeText.SetText(ValueStringEx.GetPercentString(GameSetting.Instance.Audio.MasterVolume));
            _masterVolumeSlider.value = GameSetting.Instance.Audio.MasterVolume;
        }

        private void AddMusicVolume()
        {
            float targetVolume = GameSetting.Instance.Audio.MusicVolume + 0.01f;
            GameSetting.Instance.Audio.MusicVolume = Mathf.Min(targetVolume.RoundWithDigits(2), 1);

            _musicVolumeText.SetText(ValueStringEx.GetPercentString(GameSetting.Instance.Audio.MusicVolume));
            _musicVolumeSlider.value = GameSetting.Instance.Audio.MusicVolume;
        }

        private void AddSFXVolume()
        {
            float targetVolume = GameSetting.Instance.Audio.SFXVolume + 0.01f;
            GameSetting.Instance.Audio.SFXVolume = Mathf.Min(targetVolume.RoundWithDigits(2), 1);

            _sfxVolumeText.SetText(ValueStringEx.GetPercentString(GameSetting.Instance.Audio.SFXVolume));
            _sfxVolumeSlider.value = GameSetting.Instance.Audio.SFXVolume;
        }

        private void SubtractMasterVolume()
        {
            float targetVolume = GameSetting.Instance.Audio.MasterVolume - 0.01f;
            GameSetting.Instance.Audio.MasterVolume = Mathf.Max(targetVolume.RoundWithDigits(2), 0);

            _masterVolumeText.SetText(ValueStringEx.GetPercentString(GameSetting.Instance.Audio.MasterVolume));
            _masterVolumeSlider.value = GameSetting.Instance.Audio.MasterVolume;
        }

        private void SubtractMusicVolume()
        {
            float targetVolume = GameSetting.Instance.Audio.MusicVolume - 0.01f;
            GameSetting.Instance.Audio.MusicVolume = Mathf.Max(targetVolume.RoundWithDigits(2), 0);

            _musicVolumeText.SetText(ValueStringEx.GetPercentString(GameSetting.Instance.Audio.MusicVolume));
            _musicVolumeSlider.value = GameSetting.Instance.Audio.MusicVolume;
        }

        private void SubtractSFXVolume()
        {
            float targetVolume = GameSetting.Instance.Audio.SFXVolume - 0.01f;
            GameSetting.Instance.Audio.SFXVolume = Mathf.Max(targetVolume.RoundWithDigits(2), 0);

            _sfxVolumeText.SetText(ValueStringEx.GetPercentString(GameSetting.Instance.Audio.SFXVolume));
            _sfxVolumeSlider.value = GameSetting.Instance.Audio.SFXVolume;
        }

        private void OnMasterVolumeChanged(float targetVolume)
        {
            GameSetting.Instance.Audio.MasterVolume = Mathf.Clamp01(targetVolume.RoundWithDigits(2));
            _masterVolumeText.SetText(ValueStringEx.GetPercentString(GameSetting.Instance.Audio.MasterVolume));
            UIManager.Instance.SelectController.Select(_masterVolumeButton.SelectIndex);
        }

        private void OnMusicVolumeChanged(float targetVolume)
        {
            GameSetting.Instance.Audio.MusicVolume = Mathf.Clamp01(targetVolume.RoundWithDigits(2));
            _musicVolumeText.SetText(ValueStringEx.GetPercentString(GameSetting.Instance.Audio.MusicVolume));
            UIManager.Instance.SelectController.Select(_musicVolumeButton.SelectIndex);
        }

        private void OnSFXVolumeChanged(float targetVolume)
        {
            GameSetting.Instance.Audio.SFXVolume = Mathf.Clamp01(targetVolume.RoundWithDigits(2));
            _sfxVolumeText.SetText(ValueStringEx.GetPercentString(GameSetting.Instance.Audio.SFXVolume));
            UIManager.Instance.SelectController.Select(_sfxVolumeButton.SelectIndex);
        }

        private void SetDefaultValues()
        {
            GameSetting.Instance.Audio.SetDefaultValues();

            SetActiveEventButton(_muteMusicButton, GameSetting.Instance.Audio.MuteMusic);
            SetActiveEventButton(_muteSFXButton, GameSetting.Instance.Audio.MuteSFX);

            _masterVolumeText.SetText(ValueStringEx.GetPercentString(GameSetting.Instance.Audio.MasterVolume));
            _musicVolumeText.SetText(ValueStringEx.GetPercentString(GameSetting.Instance.Audio.MusicVolume));
            _sfxVolumeText.SetText(ValueStringEx.GetPercentString(GameSetting.Instance.Audio.SFXVolume));

            _masterVolumeSlider.value = GameSetting.Instance.Audio.MasterVolume;
            _musicVolumeSlider.value = GameSetting.Instance.Audio.MusicVolume;
            _sfxVolumeSlider.value = GameSetting.Instance.Audio.SFXVolume;
        }
    }
}