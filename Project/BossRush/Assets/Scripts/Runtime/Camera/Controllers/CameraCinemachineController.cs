using Sirenix.OdinInspector;
using Unity.Cinemachine;
using UnityEngine;

namespace TeamSuneat.CameraSystem.Controllers
{
    public class CameraCinemachineController : XBehaviour
    {
        [Title("Cinemachine 설정")]
        [InfoBox("Cinemachine 파라미터를 제어합니다.")]
        [SerializeField] private float _defaultLookaheadTime = 0.5f;
        [SerializeField] private float _defaultSoftZoneWidth = 1f;
        [SerializeField] private CinemachinePositionComposer _framingTransposer;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _framingTransposer = GetComponentInChildren<CinemachinePositionComposer>();
        }

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

        public void ResetLookaheadTime()
        {
            SetLookaheadTime(_defaultLookaheadTime);
        }

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

        public void ResetSoftZoneWidth()
        {
            SetSoftZoneWidth(_defaultSoftZoneWidth);
        }

        public void ResetAllParameters()
        {
            ResetLookaheadTime();
            ResetSoftZoneWidth();

            Log.Info(LogTags.Camera, "(Cinemachine) 모든 Cinemachine 파라미터가 기본값으로 복원되었습니다.");
        }

        public CinemachineSettings GetCurrentSettings()
        {
            if (_framingTransposer == null)
            {
                return new CinemachineSettings();
            }

            return new CinemachineSettings
            {
                LookaheadTime = _framingTransposer.Lookahead.Time,
                // SoftZoneWidth = _framingTransposer.m_SoftZoneWidth
            };
        }
    }

    [System.Serializable]
    public struct CinemachineSettings
    {
        public float LookaheadTime;
        public float SoftZoneWidth;
    }
}