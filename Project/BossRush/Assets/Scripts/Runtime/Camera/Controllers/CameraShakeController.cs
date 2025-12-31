using TeamSuneat.CameraSystem.Implementations;
using TeamSuneat.Data;
using TeamSuneat.Setting;
using Sirenix.OdinInspector;
using Unity.Cinemachine;
using UnityEngine;

namespace TeamSuneat.CameraSystem.Controllers
{
    /// <summary>
    /// 카메라 쉐이크를 조율하는 컨트롤러
    /// 쉐이크 실행, 설정 조정, 상태 관리를 담당합니다.
    /// </summary>
    public class CameraShakeController : XBehaviour
    {
        [Title("쉐이크 설정")]
        [InfoBox("카메라 쉐이크를 조율합니다.")]
        [SerializeField] private float _globalShakeForce = 1f;

        [SerializeField] private float _maxShakeIntensity = 5f;

        // 쉐이크 관련 컴포넌트들 (AutoGetComponents에서 사용)
        [SerializeField] private CinemachineImpulseListener _impulseListener;

        [SerializeField] private CinemachineImpulseDefinition _impulseDefinition;
        [SerializeField] private bool _isShaking = false;

        [SerializeField] private CameraShake _cameraShake;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _cameraShake = GetComponentInChildren<CameraShake>();
            _impulseListener = GetComponentInChildren<CinemachineImpulseListener>();
        }

        /// <summary>
        /// 쉐이크를 실행합니다.
        /// </summary>
        public void Shake(CinemachineImpulseSource impulseSource)
        {
            if (!CanShake())
            {
                Log.Warning(LogTags.Camera, "(Shake) 현재 쉐이크를 실행할 수 없습니다.");
                return;
            }

            if (impulseSource == null)
            {
                Log.Warning(LogTags.Camera, "(Shake) ImpulseSource가 null입니다.");
                return;
            }

            // 쉐이크 실행 (구현 클래스에 위임)
            ExecuteShake(impulseSource);

            Log.Info(LogTags.Camera, "(Shake) 쉐이크가 실행되었습니다. 강도: {0}", _globalShakeForce);
        }

        /// <summary>
        /// 프리셋을 사용하여 쉐이크를 실행합니다.
        /// </summary>
        public void Shake(CinemachineImpulseSource impulseSource, ImpulsePreset preset)
        {
            if (impulseSource == null || preset == null)
            {
                Log.Warning(LogTags.Camera, "(Shake) ImpulseSource 또는 ImpulsePreset이 null입니다.");
                return;
            }

            if (!CanShake())
            {
                Log.Warning(LogTags.Camera, "(Shake) 현재 쉐이크를 실행할 수 없습니다: {0}", preset.NameString);
                return;
            }

            // 프리셋 설정 적용
            ApplyPresetSettings(impulseSource, preset);

            // 쉐이크 실행
            impulseSource.GenerateImpulseWithForce(preset.ImpactForce);
            _isShaking = true;

            CoroutineNextTimer(preset.ListenerDuration, StopShake);

            Log.Info(LogTags.Camera, "(Shake) 프리셋 쉐이크가 실행되었습니다. 강도: {0}", preset.ImpactForce);
        }

        /// <summary>
        /// 쉐이크를 중지합니다.
        /// </summary>
        public void StopShake()
        {
            if (_impulseListener != null)
            {
                _impulseListener.ReactionSettings.AmplitudeGain = 0f;
                _impulseListener.ReactionSettings.FrequencyGain = 0f;
            }

            _isShaking = false;

            Log.Info(LogTags.Camera, "(Shake) 쉐이크가 중지되었습니다.");
        }

        /// <summary>
        /// 쉐이크를 실행할 수 있는지 확인합니다.
        /// </summary>
        private bool CanShake()
        {
            return GameSetting.Instance.Play.CameraShake && !_isShaking;
        }

        /// <summary>
        /// 프리셋 설정을 적용합니다.
        /// </summary>
        private void ApplyPresetSettings(CinemachineImpulseSource impulseSource, ImpulsePreset preset)
        {
            if (impulseSource.ImpulseDefinition != null)
            {
                _impulseDefinition = impulseSource.ImpulseDefinition;

                // Impulse Source 설정
                _impulseDefinition.ImpulseDuration = preset.ImpactTime;
                _impulseDefinition.ImpulseShape = preset.ImpulseShape;
                _impulseDefinition.CustomImpulseShape = preset.ImpurseCurve;
                impulseSource.DefaultVelocity = preset.DefaultVelocity;
            }

            // Impulse Listener 설정
            if (_impulseListener != null)
            {
                _impulseListener.ReactionSettings.AmplitudeGain = preset.ListenerAmplitude;
                _impulseListener.ReactionSettings.FrequencyGain = preset.ListenerFrequency;
                _impulseListener.ReactionSettings.Duration = preset.ListenerDuration;
            }
        }

        /// <summary>
        /// 쉐이크 설정을 초기화합니다.
        /// </summary>
        public void ResetShakeSettings()
        {
            _globalShakeForce = 1f;
            _isShaking = false;

            Log.Info(LogTags.Camera, "(Shake) 쉐이크 설정이 초기화되었습니다.");
        }

        /// <summary>
        /// 실제 쉐이크를 실행합니다. (구현 클래스에 위임)
        /// </summary>
        private void ExecuteShake(CinemachineImpulseSource impulseSource)
        {
            if (_cameraShake != null) // CameraShakeNames.Normal을 기본값으로 사용
            {
                _cameraShake.StartShake(CameraShakeNames.Normal);
                _isShaking = true;
            }
            else
            {
                Log.Warning(LogTags.Camera, "(Shake) CameraShake 구현 클래스를 찾을 수 없습니다.");
            }
        }
    }
}