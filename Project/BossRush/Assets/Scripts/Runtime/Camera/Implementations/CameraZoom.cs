using Unity.Cinemachine;
using UnityEngine;

namespace TeamSuneat.CameraSystem.Implementations
{
    public class CameraZoom : XBehaviour
    {
        public bool IsSetDefaultSizeOnStart;
        public float ZoomDefaultOrthographicSize;

        private CinemachineCamera _virtualCamera;

        private void Awake()
        {
            _virtualCamera = GetComponent<CinemachineCamera>();
        }

        protected override void OnStart()
        {
            base.OnStart();

            if (IsSetDefaultSizeOnStart && _virtualCamera != null)
            {
                _virtualCamera.Lens.OrthographicSize = ZoomDefaultOrthographicSize;
            }
        }

        public void ZoomDefault(Transform target)
        {
            MoveToTarget(target, ZoomDefaultOrthographicSize);
        }

        public void Zoom(Transform target, float zoomSize)
        {
            MoveToTarget(target, zoomSize);
        }

        public void MoveToTarget(Transform target, float orthographicSize)
        {
            if (_virtualCamera != null && target != null)
            {
                _virtualCamera.transform.position = target.position + new Vector3(0, 0, -10);
                _virtualCamera.Lens.OrthographicSize = orthographicSize;
            }
        }
    }
}