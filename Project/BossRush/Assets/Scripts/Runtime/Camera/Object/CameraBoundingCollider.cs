using TeamSuneat.UserInterface;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat.CameraSystem.Implementations
{
    [RequireComponent(typeof(PolygonCollider2D))]
    [InfoBox("카메라 이동 범위를 제한하고 영역 밖 몬스터를 자동으로 제거합니다.")]
    public partial class CameraBoundingCollider : XBehaviour
    {
        private const float ENEMY_CHECK_INTERVAL = 5f;

        [Title("바운딩 설정")]
        [InfoBox("카메라가 이동할 수 있는 영역을 정의하는 폴리곤 콜라이더입니다.")]
        [SuffixLabel("바운딩 영역 콜라이더")]
        [SerializeField] private PolygonCollider2D _boundingShape;

        [Title("설정 무시")]
        [InfoBox("자동 설정을 무시하고 수동으로 설정할지 결정합니다.")]
        [SuffixLabel("자동 설정 무시")]
        [SerializeField] private bool _ignoreSetting;

        private Coroutine _checkOutsideEnemiesCoroutine;

        public PolygonCollider2D BoundingShape => _boundingShape;
        public bool IgnoreSetting => _ignoreSetting;

        private void Awake()
        {
            SetupWorldPositionRange();
        }

        private void SetupWorldPositionRange()
        {
            if (_boundingShape == null)
            {
                return;
            }

            float minX = float.MaxValue;
            float minY = float.MaxValue;
            float maxX = float.MinValue;
            float maxY = float.MinValue;

            for (int i = 0; i < _boundingShape.points.Length; i++)
            {
                float pointX = _boundingShape.points[i].x;
                float pointY = _boundingShape.points[i].y;

                if (minX > pointX)
                {
                    minX = pointX;
                }
                if (maxX < pointX)
                {
                    maxX = pointX;
                }
                if (minY > pointY)
                {
                    minY = pointY;
                }
                if (maxY < pointY)
                {
                    maxY = pointY;
                }
            }

            UIManager.Instance.WorldPositionMin = new Vector2(minX, minY);
            UIManager.Instance.WorldPositionMax = new Vector2(maxX, maxY);
        }

        protected override void OnDisabled()
        {
            base.OnDisabled();

            // 스테이지가 비활성화될 때 자동으로 중단
            if (_checkOutsideEnemiesCoroutine != null)
            {
                StopXCoroutine(ref _checkOutsideEnemiesCoroutine);
                Log.Info(LogTags.Camera, "스테이지 비활성화: 영역 밖 적 제거 기능 중단");
            }
        }
    }
}