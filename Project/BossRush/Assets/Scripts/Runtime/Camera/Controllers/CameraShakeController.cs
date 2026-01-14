using TeamSuneat.Data;
using TeamSuneat.Setting;
using Sirenix.OdinInspector;
using Unity.Cinemachine;
using UnityEngine;

namespace TeamSuneat.CameraSystem.Controllers
{
    // Impulse(이벤트 기반 순간 충격) 시스템을 관리
    public class CameraShakeController : XBehaviour
    {
        [Title("Impulse Shake Setting")]
        [InfoBox("이벤트 기반 순간적인 충격에 사용됩니다. (예: 공격 히트, 폭발, 피격 등)")]
        [SerializeField] private CinemachineImpulseSource _impulseSource;
        [SerializeField] private CinemachineImpulseListener _impulseListener;

        [Title("Trauma System")]
        [Tooltip("풀 트라우마(1.0)가 몇 초에 0이 되는지 결정. decay = 1f / 목표초")]
        [SerializeField] private float _traumaDecay = 2.0f;

        [Range(2.0f, 3.0f)]
        [Tooltip("trauma^exponent로 변환. 값이 클수록 낮은 trauma에서 더 약하게 반응")]
        [SerializeField] private float _traumaExponent = 2.0f;

        [Tooltip("최대 임펄스 강도. Cinemachine 프로파일과 대략 매칭되게 설정")]
        [SerializeField] private float _maxForce = 1.0f;

        private float _trauma = 0f;
        private bool _hasImpulseThisFrame = false;
        private Coroutine _impulseCoroutine;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _impulseSource = GetComponentInChildren<CinemachineImpulseSource>();
            _impulseListener = GetComponentInChildren<CinemachineImpulseListener>();
        }

        private void LateUpdate()
        {
            // 프레임 시작 시 플래그 리셋
            _hasImpulseThisFrame = false;

            // 1. 감쇠 적용
            _trauma = Mathf.Max(_trauma - _traumaDecay * Time.deltaTime, 0f);

            // 2. Early return
            if (_trauma <= 0f)
            {
                return;
            }

            // 3. 비선형 변환
            float effectiveForce = Mathf.Pow(_trauma, _traumaExponent) * _maxForce;

            // 4. 임펄스 생성 (프레임당 1회)
            if (_impulseSource != null)
            {
                _impulseSource.GenerateImpulseWithForce(effectiveForce);
                _hasImpulseThisFrame = true;
            }
        }

        #region Impulse 쉐이크 (이벤트 기반 순간 충격)

        // Trauma 추가 (외부에서 직접 호출 가능)
        public void AddTrauma(float amount)
        {
            if (!IsShakeEnabled())
            {
                return;
            }

            _trauma = Mathf.Clamp01(_trauma + amount);
        }

        // Impulse 기반 이벤트 쉐이크 트리거 (위치 지정, 내부 Source 사용)
        public void TriggerImpulseAtPosition(Vector3 position, CameraImpulseAsset asset)
        {
            if (_impulseSource == null)
            {
                Log.Warning(LogTags.Camera, "(Impulse) ImpulseSource가 설정되지 않았습니다.");
                return;
            }

            if (asset == null)
            {
                Log.Warning(LogTags.Camera, "(Impulse) CameraImpulseAsset이 null입니다.");
                return;
            }

            _impulseSource.transform.position = position;
            TriggerImpulse(_impulseSource, asset);
        }

        // Impulse 기반 이벤트 쉐이크 트리거 (공격 히트, 폭발, 피격 등 순간적인 충격)
        public void TriggerImpulse(CinemachineImpulseSource impulseSource, CameraImpulseAsset asset)
        {
            if (impulseSource == null || asset == null)
            {
                Log.Warning(LogTags.Camera, "(Impulse) ImpulseSource 또는 CameraImpulseAsset이 null입니다.");
                return;
            }

            if (!IsShakeEnabled())
            {
                Log.Warning(LogTags.Camera, "(Impulse) 쉐이크가 비활성화되어 있습니다: {0}", asset.NameString);
                return;
            }

            ApplyPresetSettings(impulseSource, asset);

            // Trauma 추가 (명시적 폴백)
            if (asset.TraumaContribution > 0f)
            {
                AddTrauma(asset.TraumaContribution);
            }
            else
            {
                // 기존 ImpactForce를 정규화하여 사용 (호환성)
                float normalizedForce = ValueEx.SafeDivide01(asset.ImpactForce, _maxForce);
                AddTrauma(normalizedForce);
            }

            if (_impulseCoroutine != null)
            {
                StopXCoroutine(ref _impulseCoroutine);
            }

            if (asset.ListenerDuration > 0f)
            {
                _impulseCoroutine = CoroutineNextTimer(asset.ListenerDuration, OnImpulseFinished);
            }

            Log.Info(LogTags.Camera, "(Impulse) {0} 쉐이크를 트리거합니다. Trauma: {1}", asset.Type, _trauma);
        }

        // Impulse 쉐이크 수동 중지
        public void StopImpulse()
        {
            if (_impulseListener != null)
            {
                _impulseListener.ReactionSettings.AmplitudeGain = 0f;
                _impulseListener.ReactionSettings.FrequencyGain = 0f;
            }

            if (_impulseCoroutine != null)
            {
                StopXCoroutine(ref _impulseCoroutine);
            }

            Log.Info(LogTags.Camera, "(Impulse) 쉐이크를 중지합니다.");
        }

        private void OnImpulseFinished()
        {
            _impulseCoroutine = null;
        }

        #endregion Impulse 쉐이크 (이벤트 기반 순간 충격)

        #region 호환성 메서드 (기존 API 유지)

        // [호환성] Impulse 쉐이크 트리거 (외부 Source 사용)
        public void Shake(CinemachineImpulseSource impulseSource, CameraImpulseAsset asset)
        {
            TriggerImpulse(impulseSource, asset);
        }

        // [새 API] Impulse 쉐이크 트리거 (위치 지정, 내부 Source 사용)
        public void ShakeAtPosition(Vector3 position, CameraImpulseAsset asset)
        {
            TriggerImpulseAtPosition(position, asset);
        }

        #endregion 호환성 메서드 (기존 API 유지)

        #region 내부 메서드

        private bool IsShakeEnabled()
        {
            return GameSetting.Instance.Play.CameraShake;
        }

        private void ApplyPresetSettings(CinemachineImpulseSource impulseSource, CameraImpulseAsset asset)
        {
            if (impulseSource.ImpulseDefinition != null)
            {
                var impulseDefinition = impulseSource.ImpulseDefinition;

                impulseDefinition.ImpulseDuration = asset.ImpactTime;
                impulseDefinition.ImpulseShape = asset.ImpulseShape;
                impulseDefinition.CustomImpulseShape = asset.ImpulseCurve;
                impulseSource.DefaultVelocity = asset.DefaultVelocity;
            }

            if (_impulseListener != null)
            {
                _impulseListener.ReactionSettings.AmplitudeGain = asset.ListenerAmplitude;
                _impulseListener.ReactionSettings.FrequencyGain = asset.ListenerFrequency;
                _impulseListener.ReactionSettings.Duration = asset.ListenerDuration;
            }
        }

        public void ResetShakeSettings()
        {
            _trauma = 0f;
            _hasImpulseThisFrame = false;
            StopImpulse();
            Log.Info(LogTags.Camera, "(Shake) 쉐이크 설정이 초기화되었습니다.");
        }

        #endregion 내부 메서드
    }
}