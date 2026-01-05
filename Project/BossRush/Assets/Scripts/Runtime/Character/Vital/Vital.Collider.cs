using System.Collections.Generic;
using UnityEngine;

namespace TeamSuneat
{
    public partial class Vital : Entity
    {
        /// <summary> 해당 충돌체가 몇 번째 충돌체인지 반환합니다. </summary>
        public int GetColliderIndex(Collider2D collider)
        {
            if (collider != null)
            {
                if (Colliders.IsValid())
                {
                    for (int i = 0; i < Colliders.Length; i++)
                    {
                        if (Colliders[i] == collider)
                        {
                            return i;
                        }
                    }
                }
            }

            return -1;
        }

        /// <summary> 특정 위치에서 가장 가까운 충돌체를 반환합니다. </summary>
        public Collider2D GetNearestCollider(Vector3 originPosition)
        {
            Collider2D result = null;

            if (Colliders.IsValid() && UseIndividualCollider)
            {
                float distance;
                float distanceMin = float.MaxValue;
                Vector3 colliderPosition;

                for (int i = 0; i < Colliders.Length; i++)
                {
                    colliderPosition = Colliders[i].transform.position;
                    distance = Vector3.Distance(originPosition, colliderPosition);

                    if (distance < distanceMin)
                    {
                        distanceMin = distance;
                        result = Colliders[i];
                    }
                }
            }
            else if (Collider != null)
            {
                result = Collider;
            }

            return result;
        }

        public Collider2D[] GetNearestColliders(Vector3 originPosition)
        {
            List<Collider2D> result = new List<Collider2D>();

            if (Colliders.IsValid() && UseIndividualCollider)
            {
                float distance;
                float distanceMin = float.MaxValue;
                Vector3 colliderPosition;

                for (int i = 0; i < Colliders.Length; i++)
                {
                    colliderPosition = Colliders[i].transform.position;
                    distance = Vector3.Distance(originPosition, colliderPosition);

                    if (distance < distanceMin)
                    {
                        distanceMin = distance;
                        result.Add(Colliders[i]);
                    }
                }
            }
            else if (Collider != null)
            {
                result.Add(Collider);
            }

            if (result.Count > 1)
            {
                NearestCollider2DComparer comparer = new NearestCollider2DComparer();
                result.Sort(comparer);
            }

            return result.ToArray();
        }

        /// <summary> 특정 위치에서 가장 가까운 충돌체의 위치를 반환합니다. </summary>
        public Vector3 GetNearestColliderPosition(Vector3 originPosition)
        {
            Collider2D nearestCollider = GetNearestCollider(originPosition);
            if (nearestCollider != null)
            {
                return nearestCollider.transform.position;
            }

            return transform.position;
        }

        /// <summary> 충돌체의 크기와 오프셋을 설정합니다. </summary>
        public void ResizeCollider(Vector2 colliderOffset, Vector2 colliderSize)
        {
            if (Collider != null)
            {
                localPosition = colliderOffset;
                Collider.size = colliderSize + (Vector2.one * 0.1f);
            }
        }

        /// <summary> 미리 등록된 충돌체의 크기와 오프셋을 가져와 바이탈의 충돌체에 적용합니다. </summary>
        public void ResizeCollider(string typeString)
        {
            VitalColliderData colliderdata = VitalColliderHandler.Find(typeString);
            if (colliderdata != null)
            {
                Collider.offset = colliderdata.Offset;
                Collider.size = colliderdata.Size;
            }
        }

        // Collider Active

        public void ActivateColliders()
        {
            if (Collider != null)
            {
                Collider.enabled = true;
            }

            if (Colliders.IsValid())
            {
                for (int i = 0; i < Colliders.Length; i++)
                {
                    Colliders[i].enabled = true;
                }
            }

            LogProgress("바이탈의 모든 충돌체를 활성화합니다.");
        }

        public void DeactivateColliders()
        {
            if (Collider != null)
            {
                Collider.enabled = false;
            }

            if (Colliders.IsValid())
            {
                for (int i = 0; i < Colliders.Length; i++)
                {
                    Colliders[i].enabled = false;
                }
            }

            LogProgress("바이탈의 모든 충돌체를 비활성화합니다.");
        }

        // In Arc
        public bool CheckColliderInArc(Vector3 areaPosition, float radius, float arcAngle, bool isFacingRight)
        {
            if (radius.IsZero())
            {
                return false;
            }

            if (Collider != null)
            {
                if (CheckColliderInArc(Collider, areaPosition, radius, arcAngle, isFacingRight))
                {
                    return true;
                }
            }

            return false;
        }

        private bool CheckColliderInArc(Collider2D collider, Vector3 areaPosition, float radius, float arcAngle, bool isFacingRight)
        {
            // 초기 유효성 검사
            if (radius.IsZero() || collider == null)
            {
                return false;
            }

            // 콜라이더의 위치 계산
            Vector3 colliderPosition = position + (Vector3)collider.offset;

            // 콜라이더의 위치가 지정된 방향에 있지 않다면 false 반환
            if (IsColliderInWrongDirection(colliderPosition, areaPosition, isFacingRight))
            {
                return false;
            }

            // 원과 콜라이더 사이의 거리를 계산하고 검사
            if (!IsColliderWithinRadius(colliderPosition, areaPosition, radius, collider.bounds.size))
            {
                return false;
            }

            // 호각도 검사
            if (!IsWithinArcAngle(colliderPosition, areaPosition, arcAngle))
            {
                return false;
            }

            return true;
        }

        private bool IsColliderInWrongDirection(Vector3 colliderPosition, Vector3 areaPosition, bool isFacingRight)
        {
            if (isFacingRight && colliderPosition.x < areaPosition.x)
            {
                return true;
            }
            else if (!isFacingRight && colliderPosition.x > areaPosition.x)
            {
                return true;
            }
            return false;
        }

        private bool IsColliderWithinRadius(Vector3 colliderPosition, Vector3 areaPosition, float radius, Vector3 colliderSize)
        {
            // 콜라이더의 경계 상자
            Bounds bounds = new Bounds(colliderPosition, colliderSize);

            // 경계 상자에서 원의 중심과 가장 가까운 점을 찾음
            Vector3 closestPoint = bounds.ClosestPoint(areaPosition);

            // 가장 가까운 점과 원의 중심 간의 거리 계산
            float distanceSquared = (areaPosition - closestPoint).sqrMagnitude;

            // 거리 검사
            return distanceSquared <= (radius * radius);
        }

        private bool IsWithinArcAngle(Vector3 colliderPosition, Vector3 areaPosition, float arcAngle)
        {
            Vector2 direction = (colliderPosition - areaPosition).normalized;
            float angle = AngleEx.ToAngle90(direction);
            float halfArc = arcAngle * 0.5f;

            return angle <= halfArc;
        }

        // In Circle

        public bool CheckColliderInCircle(Vector3 areaPosition, float radius)
        {
            if (radius.IsZero())
            {
                return false;
            }
            if (Collider != null)
            {
                if (CheckColliderInCirce(Collider, areaPosition, radius))
                {
                    return true;
                }
            }
            else if (Colliders.IsValid())
            {
                for (int i = 0; i < Colliders.Length; i++)
                {
                    if (CheckColliderInCirce(Colliders[i], areaPosition, radius))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool CheckColliderInCircle(Vector3 areaPosition, float radius, out Collider2D vitalCollider)
        {
            if (!radius.IsZero())
            {
                if (Collider != null)
                {
                    if (CheckColliderInCirce(Collider, areaPosition, radius))
                    {
                        vitalCollider = Collider;
                        return true;
                    }
                }
                else if (Colliders.IsValid())
                {
                    for (int i = 0; i < Colliders.Length; i++)
                    {
                        if (CheckColliderInCirce(Colliders[i], areaPosition, radius))
                        {
                            vitalCollider = Colliders[i];
                            return true;
                        }
                    }
                }
            }

            vitalCollider = null;
            return false;
        }

        private bool CheckColliderInCirce(Collider2D collider, Vector3 areaPosition, float radius)
        {
            if (radius.IsZero())
            {
                return false;
            }
            if (collider == null)
            {
                return false;
            }

            Vector3 offset = collider.offset;
            Rect vitalRect = CreateRect(position + offset, collider.bounds.size);

            // 사각형 내에서 원에 가장 가까운 점 찾기
            float closestX = Mathf.Clamp(areaPosition.x, vitalRect.xMin, vitalRect.xMax);
            float closestY = Mathf.Clamp(areaPosition.y, vitalRect.yMin, vitalRect.yMax);

            // 원의 중심과 이 가장 가까운 점 사이의 거리 계산
            float distanceX = areaPosition.x - closestX;
            float distanceY = areaPosition.y - closestY;

            // 거리가 원의 반지름보다 작으면 교차가 발생합니다.
            float distanceSquared = (distanceX * distanceX) + (distanceY * distanceY);

            return distanceSquared < (radius * radius);
        }

        // In Box

        public bool CheckColliderInBox(Vector3 areaPosition, Vector3 areaSize)
        {
            if (!areaSize.IsZero())
            {
                if (Collider != null)
                {
                    if (CheckColliderInBox(Collider, areaPosition, areaSize))
                    {
                        return true;
                    }
                }
                else if (Colliders.IsValid())
                {
                    for (int i = 0; i < Colliders.Length; i++)
                    {
                        if (CheckColliderInBox(Colliders[i], areaPosition, areaSize))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public bool CheckColliderInBox(Vector3 areaPosition, Vector3 areaSize, out Collider2D vitalCollider)
        {
            if (!areaSize.IsZero())
            {
                if (Collider != null)
                {
                    if (CheckColliderInBox(Collider, areaPosition, areaSize))
                    {
                        vitalCollider = Collider;
                        return true;
                    }
                }
                else if (Colliders.IsValid())
                {
                    for (int i = 0; i < Colliders.Length; i++)
                    {
                        if (CheckColliderInBox(Colliders[i], areaPosition, areaSize))
                        {
                            vitalCollider = Colliders[i];
                            return true;
                        }
                    }
                }
            }

            vitalCollider = null;
            return false;
        }

        private bool CheckColliderInBox(Collider2D collider, Vector3 areaPosition, Vector3 areaSize)
        {
            if (collider == null) { return false; }
            if (areaSize.IsZero()) { return false; }

            Vector3 offset;
            Rect areaRect;
            Rect vitalRect;

            offset = collider.offset;
            areaRect = CreateRect(areaPosition, areaSize);
            vitalRect = CreateRect(position + offset, collider.bounds.size);
            return areaRect.Overlaps(vitalRect);
        }

        // Rect

        private Rect CreateRect(Vector3 position, Vector3 size)
        {
            return new Rect(position - (size * 0.5f), size);
        }
    }
}