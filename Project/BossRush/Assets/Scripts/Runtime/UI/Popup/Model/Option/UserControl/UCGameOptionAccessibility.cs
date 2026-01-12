using Sirenix.OdinInspector;
using TeamSuneat.Data;
using TeamSuneat.Setting;
using UnityEngine;

namespace TeamSuneat.UserInterface
{
    public class UCGameOptionAccessibility : UCGameOptionBase
    {
        [FoldoutGroup("#Accessibility/Buttons")]
        [SerializeField] private UISelectElement _vibrationButton;
        [FoldoutGroup("#Accessibility/Buttons")]
        [SerializeField] private UISelectElement _cameraShakeButton;
        [FoldoutGroup("#Accessibility/Buttons")]
        [SerializeField] private UISelectElement _damageTextButton;
        [FoldoutGroup("#Accessibility/Buttons")]
        [SerializeField] private UISelectElement _defaultValuesButton;
        [FoldoutGroup("#Accessibility/Buttons")]
        [SerializeField] private UISelectElement _backButton;

        [FoldoutGroup("#Accessibility")]
        [SerializeField] private UILocalizedText _descriptionText;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _indexer ??= GetComponentInChildren<UISelectElementIndexer>();

            _vibrationButton = this.FindComponent<UISelectElement>("#Content/Vibration Button");
            _cameraShakeButton = this.FindComponent<UISelectElement>("#Content/CameraShake Button");
            _damageTextButton = this.FindComponent<UISelectElement>("#Content/DamageText Button");
            _defaultValuesButton = this.FindComponent<UISelectElement>("#Content/Default Values Button");
            _backButton = this.FindComponent<UISelectElement>("#Content/Back Button");
            _descriptionText = this.FindComponent<UILocalizedText>("Description Text");
        }

        protected override void OnStart()
        {
            base.OnStart();

            _vibrationButton?.RegisterOnPointEnter(SetVibrationDescription);
            _cameraShakeButton?.RegisterOnPointEnter(SetCameraShakeDescription);
            _damageTextButton?.RegisterOnPointEnter(SetDamageTextDescription);
            _defaultValuesButton?.RegisterOnPointEnter(SetDefaultValuesDescription);
            _backButton?.RegisterOnPointEnter(ResetDescription);

            _vibrationButton?.OnPointerClickLeftEvent.AddListener(SwitchVibration);
            _cameraShakeButton?.OnPointerClickLeftEvent.AddListener(SwitchCameraShake);
            _damageTextButton?.OnPointerClickLeftEvent.AddListener(SwitchDamageText);
            _defaultValuesButton?.OnPointerClickLeftEvent.AddListener(SetDefaultValues);
            _backButton?.OnPointerClickLeftEvent.AddListener(Hide);
        }

        protected override void OnShow()
        {
            base.OnShow();

            HideUnderlineEventButton(_vibrationButton);
            HideUnderlineEventButton(_cameraShakeButton);
            HideUnderlineEventButton(_damageTextButton);

            SetActiveEventButton(_vibrationButton, GameSetting.Instance.Play.Vibration);
            SetActiveEventButton(_cameraShakeButton, GameSetting.Instance.Play.CameraShake);
            SetActiveEventButton(_damageTextButton, GameSetting.Instance.Play.UseDamageText);
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
            SetActiveEventButton(_vibrationButton, GameSetting.Instance.Play.Vibration);
        }

        private void SwitchCameraShake()
        {
            GameSetting.Instance.Play.CameraShake = !GameSetting.Instance.Play.CameraShake;
            SetActiveEventButton(_cameraShakeButton, GameSetting.Instance.Play.CameraShake);
        }

        private void SwitchDamageText()
        {
            GameSetting.Instance.Play.UseDamageText = !GameSetting.Instance.Play.UseDamageText;
            SetActiveEventButton(_damageTextButton, GameSetting.Instance.Play.UseDamageText);
        }

        #endregion Switch

        private void SetDefaultValues()
        {
            GameSetting.Instance.Play.Vibration = false;
            GameSetting.Instance.Play.CameraShake = true;
            GameSetting.Instance.Play.UseDamageText = true;
            GameSetting.Instance.Play.UseStateEffectText = true;
            GameSetting.Instance.Play.ShowMonsterLifeText = false;
            GameSetting.Instance.Play.ShowItemOptionRange = false;
            GameSetting.Instance.Play.ShowStatusCalculations = false;
            GameSetting.Instance.Play.UseGameplayTimer = false;

            SetActiveEventButton(_vibrationButton, GameSetting.Instance.Play.Vibration);
            SetActiveEventButton(_cameraShakeButton, GameSetting.Instance.Play.CameraShake);
            SetActiveEventButton(_damageTextButton, GameSetting.Instance.Play.UseDamageText);
        }
    }
}