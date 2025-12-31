using TeamSuneat.Data;
using Unity.Cinemachine;

namespace TeamSuneat
{
    public class VirtualCamera : XBehaviour
    {
        public CinemachineCamera CineCamera;

        public CinemachineImpulseListener ImpulseListener { get; private set; }

        public CinemachinePositionComposer Transposer { get; private set; }

        public CinemachineConfiner2D Confiner { get; private set; }

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            CineCamera = GetComponent<CinemachineCamera>();
        }

        private void Awake()
        {
            if (CineCamera != null)
            {
                Transposer = CineCamera.GetComponent<CinemachinePositionComposer>();
                Confiner = CineCamera.GetComponent<CinemachineConfiner2D>();
            }

            ImpulseListener = GetComponent<CinemachineImpulseListener>();
        }

        public void Setup(CameraAsset asset)
        {
            if (CineCamera != null)
            {
                CineCamera.Lens.OrthographicSize = asset.OrthographicSize;
            }

            if (Transposer != null)
            {
                Transposer.Lookahead.Time = asset.LookaheadTime;
                Transposer.Lookahead.Smoothing = asset.LookaheadSmooting;
                Transposer.Damping = new UnityEngine.Vector3(asset.XDamping, asset.YDamping);
            }
        }
    }
}