using TeamSuneat.UserInterface;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat.CameraSystem.Implementations
{
    [RequireComponent(typeof(PolygonCollider2D))]
    [InfoBox("카메라 이동 범위를 제한하고 영역 밖 몬스터를 자동으로 제거합니다.")]
    public partial class CameraBoundingCollider : XBehaviour
    {
        // 상수 정의
        private const float ENEMY_CHECK_INTERVAL = 5f;

        [Title("바운딩 설정")]
        [InfoBox("카메라가 이동할 수 있는 영역을 정의하는 폴리곤 콜라이더입니다.")]
        [SuffixLabel("바운딩 영역 콜라이더")]
        public PolygonCollider2D BoundingShape;

        [Title("설정 무시")]
        [InfoBox("자동 설정을 무시하고 수동으로 설정할지 결정합니다.")]
        [SuffixLabel("자동 설정 무시")]
        public bool IgnoreSetting;

        private Coroutine _checkOusideEnemiesCoroutine;

       
        private void Awake()
        {
            SetupWorldPositionRange();
        }

        private void SetupWorldPositionRange()
        {
            if (BoundingShape != null)
            {
                float minX = float.MaxValue, minY = float.MaxValue;
                float maxX = float.MinValue, maxY = float.MinValue;

                for (int i = 0; i < BoundingShape.points.Length; i++)
                {
                    if (minX > BoundingShape.points[i].x)
                    {
                        minX = BoundingShape.points[i].x;
                    }
                    if (maxX < BoundingShape.points[i].x)
                    {
                        maxX = BoundingShape.points[i].x;
                    }

                    if (minY > BoundingShape.points[i].y)
                    {
                        minY = BoundingShape.points[i].y;
                    }
                    if (maxY < BoundingShape.points[i].y)
                    {
                        maxY = BoundingShape.points[i].y;
                    }
                }

                UIManager.Instance.WorldPositionMin = new Vector2(minX, minY);
                UIManager.Instance.WorldPositionMax = new Vector2(maxX, maxY);
            }
        }

        protected override void OnDisabled()
        {
            base.OnDisabled();

            // 스테이지가 비활성화될 때 자동으로 중단
            if (_checkOusideEnemiesCoroutine != null)
            {
                StopXCoroutine(ref _checkOusideEnemiesCoroutine);
                Log.Info(LogTags.Camera, "스테이지 비활성화: 영역 밖 적 제거 기능 중단");
            }
        }
    }
}