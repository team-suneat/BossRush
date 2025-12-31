using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat.CameraSystem.Implementations
{
    public partial class CameraBoundingCollider
    {
        #region Editor

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            BoundingShape = GetComponent<PolygonCollider2D>();
        }

        public override void AutoSetting()
        {
            base.AutoSetting();

            if (IgnoreSetting)
            {
                return;
            }

            transform.parent.localPosition = Vector3.zero;
            transform.localPosition = Vector3.zero;
            transform.localScale = Vector3.one;
            transform.localRotation = Quaternion.identity;

            SetupBoundingShape();
        }

        private void SetupBoundingShape()
        {
            if (BoundingShape != null)
            {
                BoundingShape.offset = Vector3.zero;

                Vector2[] paths = BoundingShape.GetPath(0);
                for (int i = 0; i < BoundingShape.points.Length; i++)
                {
                    paths[i] = new Vector2(Mathf.RoundToInt(BoundingShape.points[i].x), Mathf.RoundToInt(BoundingShape.points[i].y));
                }

                BoundingShape.SetPath(0, paths);
            }
        }

        #endregion Editor

        #region Button For Editor

#if UNITY_EDITOR

        [FoldoutGroup("#Buttons/Custom")]
        [Button("NormalizePolygonColliderPosition", ButtonSizes.Medium)]
        [InfoBox("폴리곤 콜라이더의 위치를 정규화합니다. 게임 오브젝트의 localPosition이 (0,0,0)이 아닌 경우 콜라이더의 모든 점에 오프셋을 적용합니다.")]
        private void NormalizePolygonColliderPosition()
        {
            // 게임 오브젝트의 localPosition 가져오기
            Vector3 gameObjectLocalPosition = transform.localPosition;

            // 게임 오브젝트의 localPosition이 (0, 0, 0)이 아니라면 폴리곤 콜라이더의 모든 점 위치에 더하기
            if (gameObjectLocalPosition != Vector3.zero)
            {
                Vector2 offset = new(gameObjectLocalPosition.x, gameObjectLocalPosition.y);

                // 폴리곤 콜라이더의 각 점 위치를 업데이트
                for (int i = 0; i < BoundingShape.pathCount; i++)
                {
                    Vector2[] path = BoundingShape.GetPath(i);
                    for (int j = 0; j < path.Length; j++)
                    {
                        path[j] += offset;
                    }
                    BoundingShape.SetPath(i, path);
                }

                // 게임 오브젝트의 localPosition을 (0, 0, 0)으로 설정
                transform.localPosition = Vector3.zero;
            }
        }

        [FoldoutGroup("#Buttons/Custom")]
        [Button("ConvertBoxToPolygonCollider", ButtonSizes.Medium)]
        [InfoBox("이 오브젝트에 추가된 모든 박스 콜라이더의 크기와 위치를 고려하여 폴리곤 콜라이더에 적용합니다.")]
        private void ConvertBoxToPolygonCollider()
        {
            // 모든 박스 콜라이더 찾기
            BoxCollider2D[] boxColliders = GetComponents<BoxCollider2D>();

            if (boxColliders == null || boxColliders.Length == 0)
            {
                Log.Warning(LogTags.Camera, "BoxCollider2D를 찾을 수 없습니다. 이 오브젝트에 BoxCollider2D가 추가되어 있는지 확인하세요.");
                return;
            }

            if (BoundingShape == null)
            {
                Log.Warning(LogTags.Camera, "PolygonCollider2D를 찾을 수 없습니다.");
                return;
            }

            // 모든 박스 콜라이더의 점들을 수집
            List<Vector2> allPoints = new();

            for (int i = 0; i < boxColliders.Length; i++)
            {
                BoxCollider2D boxCollider = boxColliders[i];

                // 박스 콜라이더의 크기와 오프셋 가져오기
                Vector2 boxSize = boxCollider.size;
                Vector2 boxOffset = boxCollider.offset;

                // 박스의 절반 크기 계산
                float halfWidth = boxSize.x * 0.5f;
                float halfHeight = boxSize.y * 0.5f;

                // 박스의 네 모서리 점들을 생성 (시계방향)
                Vector2[] boxPoints = new Vector2[4]
                {
                    new(boxOffset.x - halfWidth, boxOffset.y - halfHeight), // 왼쪽 아래
                    new(boxOffset.x + halfWidth, boxOffset.y - halfHeight), // 오른쪽 아래
                    new(boxOffset.x + halfWidth, boxOffset.y + halfHeight), // 오른쪽 위
                    new(boxOffset.x - halfWidth, boxOffset.y + halfHeight)  // 왼쪽 위
                };

                allPoints.AddRange(boxPoints);

                Log.Info(LogTags.Camera, "박스 콜라이더 {0} 처리 완료. 크기: {1}, 오프셋: {2}", i + 1, boxSize, boxOffset);
            }

            // 모든 점들을 하나의 폴리곤으로 결합
            if (allPoints.Count > 0)
            {
                // Convex Hull 알고리즘을 사용하여 외곽선만 추출
                Vector2[] convexHull = CalculateConvexHull(allPoints.ToArray());
                BoundingShape.SetPath(0, convexHull);

                Log.Info(LogTags.Camera, "총 {0}개의 박스 콜라이더를 PolygonCollider2D로 성공적으로 변환했습니다. 최종 점 개수: {1}",
                    boxColliders.Length, convexHull.Length);
            }

            // 모든 박스 콜라이더 제거
            for (int i = 0; i < boxColliders.Length; i++)
            {
                DestroyImmediate(boxColliders[i]);
            }
        }

        /// <summary>
        /// 점들의 Convex Hull을 계산합니다.
        /// </summary>
        private Vector2[] CalculateConvexHull(Vector2[] points)
        {
            if (points.Length < 3)
            {
                return points;
            }

            // Graham Scan 알고리즘을 사용한 Convex Hull 계산
            List<Vector2> hull = new();

            // 가장 아래쪽 점 찾기 (y가 가장 작고, 같으면 x가 가장 작은 점)
            Vector2 pivot = points[0];
            int pivotIndex = 0;

            for (int i = 1; i < points.Length; i++)
            {
                if (points[i].y < pivot.y || (points[i].y == pivot.y && points[i].x < pivot.x))
                {
                    pivot = points[i];
                    pivotIndex = i;
                }
            }

            // pivot을 기준으로 각도 순으로 정렬
            List<Vector2> sortedPoints = new();
            for (int i = 0; i < points.Length; i++)
            {
                if (i != pivotIndex)
                {
                    sortedPoints.Add(points[i]);
                }
            }

            sortedPoints.Sort((a, b) =>
            {
                float angleA = Mathf.Atan2(a.y - pivot.y, a.x - pivot.x);
                float angleB = Mathf.Atan2(b.y - pivot.y, b.x - pivot.x);

                if (Mathf.Abs(angleA - angleB) < 0.001f)
                {
                    // 각도가 같으면 거리순으로 정렬
                    float distA = (a - pivot).sqrMagnitude;
                    float distB = (b - pivot).sqrMagnitude;
                    return distA.CompareTo(distB);
                }

                return angleA.CompareTo(angleB);
            });

            // Graham Scan
            hull.Add(pivot);
            hull.Add(sortedPoints[0]);

            for (int i = 1; i < sortedPoints.Count; i++)
            {
                while (hull.Count > 1 &&
                       CrossProduct(hull[hull.Count - 2], hull[hull.Count - 1], sortedPoints[i]) <= 0)
                {
                    hull.RemoveAt(hull.Count - 1);
                }
                hull.Add(sortedPoints[i]);
            }

            return hull.ToArray();
        }

        /// <summary>
        /// 세 점의 외적을 계산합니다.
        /// </summary>
        private float CrossProduct(Vector2 O, Vector2 A, Vector2 B)
        {
            return ((A.x - O.x) * (B.y - O.y)) - ((A.y - O.y) * (B.x - O.x));
        }

#endif

        #endregion Button For Editor
    }
}