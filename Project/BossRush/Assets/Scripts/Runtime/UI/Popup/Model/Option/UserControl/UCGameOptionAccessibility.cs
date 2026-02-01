using Sirenix.OdinInspector;
using TeamSuneat.Data;
using TeamSuneat.Setting;
using UnityEngine;

namespace TeamSuneat.UserInterface
{
    public class UCGameOptionAccessibility : UCGameOptionBase
    {
        [FoldoutGroup("#Accessibility/Buttons")]
        [SerializeField] private UIToggleButton _vibrationButton;

        [FoldoutGroup("#Accessibility/Buttons")]
        [SerializeField] private UIToggleButton _cameraShakeButton;

        [FoldoutGroup("#Accessibility/Buttons")]
        [SerializeField] private UIToggleButton _damageTextButton;

        [FoldoutGroup("#Accessibility/Buttons")]
        [SerializeField] private UIToggleButton _stateEffectTextButton;

        [FoldoutGroup("#Accessibility/Buttons")]
        [SerializeField] private UIImmediateButton _defaultValuesButton;

        [FoldoutGroup("#Accessibility/Buttons")]
        [SerializeField] private UIImmediateButton _backButton;

        [FoldoutGroup("#Accessibility")]
        [SerializeField] private UILocalizedText _descriptionText;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _indexer ??= GetComponentInChildren<UISelectElementIndexer>();

            _vibrationButton = this.FindComponent<UIToggleButton>("#Content/Vibration Button");
            _cameraShakeButton = this.FindComponent<UIToggleButton>("#Content/CameraShake Button");
            _damageTextButton = this.FindComponent<UIToggleButton>("#Content/DamageText Button");
            _stateEffectTextButton = this.FindComponent<UIToggleButton>("#Content/StateEffectText Button");
            _defaultValuesButton = this.FindComponent<UIImmediateButton>("#Content/Default Values Button");
            _backButton = this.FindComponent<UIImmediateButton>("#Content/Back Button");
            _descriptionText = this.FindComponent<UILocalizedText>("Description Text");
        }

        protected override void OnStart()
        {
            base.OnStart();

            _vibrationButton?.RegisterOnPointEnter(SetVibrationDescription);
            _cameraShakeButton?.RegisterOnPointEnter(SetCameraShakeDescription);
            _damageTextButton?.RegisterOnPointEnter(SetDamageTextDescription);
            _stateEffectTextButton?.RegisterOnPointEnter(SetStateEffectTextDescription);
            _defaultValuesButton?.RegisterOnPointEnter(SetDefaultValuesDescription);
            _backButton?.RegisterOnPointEnter(ResetDescription);

            _vibrationButton?.OnPointerClickLeftEvent.AddListener(SwitchVibration);
            _cameraShakeButton?.OnPointerClickLeftEvent.AddListener(SwitchCameraShake);
            _damageTextButton?.OnPointerClickLeftEvent.AddListener(SwitchDamageText);
            _stateEffectTextButton?.OnPointerClickLeftEvent.AddListener(SwitchStateEffectText);
            _defaultValuesButton?.OnPointerClickLeftEvent.AddListener(SetDefaultValues);
            _backButton?.OnPointerClickLeftEvent.AddListener(Hide);
        }

        protected override void OnShow()
        {
            base.OnShow();

            SetButtonSelected(_vibrationButton, GameSetting.Instance.Play.Vibration);
            SetButtonSelected(_cameraShakeButton, GameSetting.Instance.Play.CameraShake);
            SetButtonSelected(_damageTextButton, GameSetting.Instance.Play.UseDamageText);
            SetButtonSelected(_stateEffectTextButton, GameSetting.Instance.Play.UseStateEffectText);
        }

        #region Set Description

        private void SetVibrationDescription()
        {
            _descriptionText?.SetText(JsonDataManager.FindStringClone("Option_Desc_Vibration"));
        }

        private void SetCameraShakeDescription()
        {
            _descriptionText?.SetText(JsonDataManager.FindStringClone("Option_Desc_CameraShake"));
        }

        private void SetDamageTextDescription()
        {
            _descriptionText?.SetText(JsonDataManager.FindStringClone("Option_Desc_DamageText"));
        }

        private void SetStateEffectTextDescription()
        {
            _descriptionText?.SetText(JsonDataManager.FindStringClone("Option_Desc_StateEffectText"));
        }

        private void SetDefaultValuesDescription()
        {
            _descriptionText?.SetText(JsonDataManager.FindStringClone("Option_Desc_DefaultValues"));
        }

        private void ResetDescription()
        {
            _descriptionText?.ResetText();
        }

        #endregion Set Description

        #region Switch

        private void SwitchVibration()
        {
            GameSetting.Instance.Play.Vibration = !GameSetting.Instance.Play.Vibration;
            SetButtonSelected(_vibrationButton, GameSetting.Instance.Play.Vibration);
        }

        private void SwitchCameraShake()
        {
            GameSetting.Instance.Play.CameraShake = !GameSetting.Instance.Play.CameraShake;
            SetButtonSelected(_cameraShakeButton, GameSetting.Instance.Play.CameraShake);
        }

        private void SwitchDamageText()
        {
            GameSetting.Instance.Play.UseDamageText = !GameSetting.Instance.Play.UseDamageText;
            SetButtonSelected(_damageTextButton, GameSetting.Instance.Play.UseDamageText);
        }
        private void SwitchStateEffectText()
        {
            GameSetting.Instance.Play.UseStateEffectText = !GameSetting.Instance.Play.UseStateEffectText;
            SetButtonSelected(_damageTextButton, GameSetting.Instance.Play.UseStateEffectText);
        }


        #endregion Switch

        private void SetDefaultValues()
        {
            GameSetting.Instance.Play.Vibration = false;
            GameSetting.Instance.Play.CameraShake = true;
            GameSetting.Instance.Play.UseDamageText = true;
            GameSetting.Instance.Play.UseStateEffectText = true;

            SetButtonSelected(_vibrationButton, GameSetting.Instance.Play.Vibration);
            SetButtonSelected(_cameraShakeButton, GameSetting.Instance.Play.CameraShake);
            SetButtonSelected(_damageTextButton, GameSetting.Instance.Play.UseDamageText);
            SetButtonSelected(_damageTextButton, GameSetting.Instance.Play.UseStateEffectText);
        }
    }
}