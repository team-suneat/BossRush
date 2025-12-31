using Sirenix.OdinInspector;
using TeamSuneat.CameraSystem.Controllers;
using Unity.Cinemachine;
using UnityEngine;

namespace TeamSuneat
{
    /// <summary>
    /// 게임용 카메라 컨트롤러
    /// 고정된 카메라 위치와 간단한 전환 효과를 제공합니다.
    /// </summary>
    public class TurnBasedCameraController : XBehaviour
    {
        [Title("카메라 설정")]
        [InfoBox("게임용 고정 카메라 설정입니다.")]
        [SerializeField] private CinemachineCamera _mainVirtualCamera;
        [SerializeField] private Transform _defaultCameraPosition;

        [Title("전환 설정")]
        [InfoBox("카메라 전환 관련 설정입니다.")]
        [SerializeField] private float _transitionDuration = 1.0f;
        [SerializeField] private AnimationCurve _transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Title("웨이브/챕터 전환")]
        [InfoBox("웨이브나 챕터 전환 시 카메라 효과 설정입니다.")]
        [SerializeField] private Transform _waveStartPosition;
        [SerializeField] private Transform _bossFightPosition;
        [SerializeField] private Transform _rewardPhasePosition;

        private Vector3 _currentTargetPosition;
        private bool _isTransitioning = false;
        private float _transitionTimer = 0f;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();
        }

        protected override void OnStart()
        {
            base.OnStart();

            // 기본 카메라 위치 설정
            if (_defaultCameraPosition != null)
            {
                SetCameraPosition(_defaultCameraPosition.position);
            }
        }

        public void LogicUpdate()
        {
            // 카메라 전환 처리
            if (_isTransitioning)
            {
                UpdateCameraTransition();
            }
        }

        /// <summary>
        /// 카메라를 지정된 위치로 설정합니다.
        /// </summary>
        public void SetCameraPosition(Vector3 position)
        {
            if (_mainVirtualCamera == null)
            {
                Log.Warning(LogTags.Camera, "메인 가상 카메라가 설정되지 않았습니다.");
                return;
            }

            _currentTargetPosition = position;
            _mainVirtualCamera.transform.position = position;

            Log.Info(LogTags.Camera, "카메라 위치를 설정했습니다: {0}", position);
        }

        /// <summary>
        /// 카메라를 부드럽게 전환합니다.
        /// </summary>
        public void TransitionToPosition(Vector3 targetPosition, float duration = -1f)
        {
            if (duration < 0)
            {
                duration = _transitionDuration;
            }

            _currentTargetPosition = targetPosition;
            _isTransitioning = true;
            _transitionTimer = 0f;

            Log.Info(LogTags.Camera, "카메라 전환을 시작합니다: {0}", targetPosition);
        }

        /// <summary>
        /// 웨이브 시작 시 카메라 효과
        /// </summary>
        public void OnWaveStart()
        {
            if (_waveStartPosition != null)
            {
                TransitionToPosition(_waveStartPosition.position);
            }
        }

        /// <summary>
        /// 보스 전투 시 카메라 효과
        /// </summary>
        public void OnBossFight()
        {
            if (_bossFightPosition != null)
            {
                TransitionToPosition(_bossFightPosition.position);
            }
        }

        /// <summary>
        /// 보상 단계 시 카메라 효과
        /// </summary>
        public void OnRewardPhase()
        {
            if (_rewardPhasePosition != null)
            {
                TransitionToPosition(_rewardPhasePosition.position);
            }
        }

        /// <summary>
        /// 기본 카메라 위치로 복귀
        /// </summary>
        public void ReturnToDefault()
        {
            if (_defaultCameraPosition != null)
            {
                TransitionToPosition(_defaultCameraPosition.position);
            }
        }

        /// <summary>
        /// 카메라 전환 업데이트
        /// </summary>
        private void UpdateCameraTransition()
        {
            if (_mainVirtualCamera == null) return;

            _transitionTimer += Time.deltaTime;
            float progress = _transitionTimer / _transitionDuration;

            if (progress >= 1f)
            {
                // 전환 완료
                _mainVirtualCamera.transform.position = _currentTargetPosition;
                _isTransitioning = false;
                _transitionTimer = 0f;

                Log.Info(LogTags.Camera, "카메라 전환이 완료되었습니다.");
            }
            else
            {
                // 전환 중
                float curveValue = _transitionCurve.Evaluate(progress);
                Vector3 startPosition = _mainVirtualCamera.transform.position;
                Vector3 newPosition = Vector3.Lerp(startPosition, _currentTargetPosition, curveValue);
                _mainVirtualCamera.transform.position = newPosition;
            }
        }
    }
}