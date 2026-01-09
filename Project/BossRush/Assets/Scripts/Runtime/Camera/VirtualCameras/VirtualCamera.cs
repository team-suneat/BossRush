using TeamSuneat.Data;
using Unity.Cinemachine;
using UnityEngine;

namespace TeamSuneat.CameraSystem.VirtualCameras
{
    public class VirtualCamera : XBehaviour
    {
        private CinemachineCamera _cineCamera;

        public CinemachineImpulseListener ImpulseListener { get; private set; }

        public CinemachineConfiner2D Confiner { get; private set; }

        private void Awake()
        {
            _cineCamera = GetComponent<CinemachineCamera>();
            if (_cineCamera != null)
            {
                Confiner = _cineCamera.GetComponent<CinemachineConfiner2D>();
            }

            ImpulseListener = GetComponent<CinemachineImpulseListener>();
        }

        public void Setup(CameraAsset asset)
        {
            if (_cineCamera != null)
            {
                _cineCamera.Lens.OrthographicSize = asset.OrthographicSize;
            }
        }

        public void SetFollow(Transform target)
        {
            CameraTarget cameraTarget = _cineCamera.Target;
            cameraTarget.TrackingTarget = target;
            _cineCamera.Target = cameraTarget;
        }
    }
}