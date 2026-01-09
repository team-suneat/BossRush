using TeamSuneat.CameraSystem.VirtualCameras;
using UnityEngine;

namespace TeamSuneat.CameraSystem.Controllers
{
    public class CameraFollowController : XBehaviour
    {
        private VirtualCamera _virtualCamera;
        private Transform _currentFollowTarget;
        private bool _isFollowing;

        private void Awake()
        {
            _virtualCamera = GetComponentInChildren<VirtualCamera>();
        }

        public void SetFollowTarget(Transform target)
        {
            if (target == null)
            {
                Log.Warning(LogTags.Camera, "(Follow) 팔로우 타겟이 null입니다.");
                return;
            }

            _virtualCamera?.SetFollow(target);
            _currentFollowTarget = target;
            _isFollowing = true;

            Log.Info(LogTags.Camera, "(Follow) 팔로우 타겟이 설정되었습니다: {0}", target.name);
        }

        public void StopFollow()
        {
            _virtualCamera?.SetFollow(null);
            _currentFollowTarget = null;
            _isFollowing = false;

            Log.Info(LogTags.Camera, "(Follow) 팔로우가 중지되었습니다.");
        }

        public bool IsFollowing()
        {
            return _isFollowing;
        }

        public Transform GetCurrentFollowTarget()
        {
            return _currentFollowTarget;
        }
    }
}