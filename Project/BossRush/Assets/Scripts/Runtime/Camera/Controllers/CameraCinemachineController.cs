using Sirenix.OdinInspector;
using Unity.Cinemachine;
using UnityEngine;

namespace TeamSuneat.CameraSystem.Controllers
{
    /// <summary>
    /// Cinemachine 파라미터를 제어하는 컨트롤러
    /// 댐핑, 룩어헤드, 소프트존 등의 Cinemachine 설정을 관리합니다.
    /// </summary>
    public class CameraCinemachineController : XBehaviour
    {
        [Title("Cinemachine 설정")]
        [InfoBox("Cinemachine 파라미터를 제어합니다.")]
        [SerializeField] private float _defaultXDamping = 1f;

        [SerializeField] private float _defaultLookaheadTime = 0.5f;
        [SerializeField] private float _defaultSoftZoneWidth = 1f;
        [SerializeField] private CinemachinePositionComposer _framingTransposer;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _framingTransposer = GetComponentInChildren<CinemachinePositionComposer>();
        }

        /// <summary>
        /// X축 댐핑을 설정합니다.
        /// </summary>
        public void SetXDamping(float xDamping)
        {
            if (_framingTransposer == null)
            {
                Log.Warning(LogTags.Camera, "(Cinemachine) FramingTransposer가 null입니다.");
                return;
            }

            _framingTransposer.Damping = _framingTransposer.Damping.ApplyX(xDamping);

            Log.Info(LogTags.Camera, "(Cinemachine) X축 댐핑이 설정되었습니다: {0}", xDamping);
        }

        /// <summary>
        /// X축 댐핑을 기본값으로 복원합니다.
        /// </summary>
        public void ResetXDamping()
        {
            SetXDamping(_defaultXDamping);
        }

        /// <summary>
        /// 룩어헤드 시간을 설정합니다.
        /// </summary>
        public void SetLookaheadTime(float lookaheadTime)
        {
            if (_framingTransposer == null)
            {
                Log.Warning(LogTags.Camera, "(Cinemachine) FramingTransposer가 null입니다.");
                return;
            }

            _framingTransposer.Lookahead.Time = lookaheadTime;

            Log.Info(LogTags.Camera, "(Cinemachine) 룩어헤드 시간이 설정되었습니다: {0}", lookaheadTime);
        }

        /// <summary>
        /// 룩어헤드 시간을 기본값으로 복원합니다.
        /// </summary>
        public void ResetLookaheadTime()
        {
            SetLookaheadTime(_defaultLookaheadTime);
        }

        /// <summary>
        /// 소프트존 너비를 설정합니다.
        /// </summary>
        public void SetSoftZoneWidth(float width)
        {
            if (_framingTransposer == null)
            {
                Log.Warning(LogTags.Camera, "(Cinemachine) FramingTransposer가 null입니다.");
                return;
            }

            // _framingTransposer.m_SoftZoneWidth = width;

            Log.Info(LogTags.Camera, "(Cinemachine) 소프트존 너비가 설정되었습니다: {0}", width);
        }

        /// <summary>
        /// 소프트존 너비를 기본값으로 복원합니다.
        /// </summary>
        public void ResetSoftZoneWidth()
        {
            SetSoftZoneWidth(_defaultSoftZoneWidth);
        }

        /// <summary>
        /// 모든 Cinemachine 파라미터를 기본값으로 복원합니다.
        /// </summary>
        public void ResetAllParameters()
        {
            ResetXDamping();
            ResetLookaheadTime();
            ResetSoftZoneWidth();

            Log.Info(LogTags.Camera, "(Cinemachine) 모든 Cinemachine 파라미터가 기본값으로 복원되었습니다.");
        }

        /// <summary>
        /// 현재 Cinemachine 설정을 반환합니다.
        /// </summary>
        public CinemachineSettings GetCurrentSettings()
        {
            if (_framingTransposer == null)
            {
                return new CinemachineSettings();
            }

            return new CinemachineSettings
            {
                XDamping = _framingTransposer.Damping.x,
                LookaheadTime = _framingTransposer.Lookahead.Time,
                // SoftZoneWidth = _framingTransposer.m_SoftZoneWidth
            };
        }
    }

    /// <summary>
    /// Cinemachine 설정 정보를 담는 구조체
    /// </summary>
    [System.Serializable]
    public struct CinemachineSettings
    {
        public float XDamping;
        public float LookaheadTime;
        public float SoftZoneWidth;
    }
}