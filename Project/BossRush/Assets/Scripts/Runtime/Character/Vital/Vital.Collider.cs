using UnityEngine;

namespace TeamSuneat
{
    public partial class Vital : Entity
    {
        public Vector3 GetNearestColliderPosition(Vector3 originPosition)
        {
            if (Collider != null)
            {
                return Collider.transform.position;
            }

            return transform.position;
        }

        public void ResizeCollider(Vector2 colliderOffset, Vector2 colliderSize)
        {
            if (Collider != null)
            {
                localPosition = colliderOffset;
                Collider.size = colliderSize + (Vector2.one * 0.1f);
            }
        }

        public void ResizeCollider(string typeString)
        {
            VitalColliderData data = VitalColliderHandler.Find(typeString);
            if (data != null)
            {
                Collider.offset = data.Offset;
                Collider.size = data.Size;
            }
        }

        public void ActivateColliders()
        {
            if (Collider != null)
            {
                Collider.enabled = true;
            }

            LogProgress("바이탈의 충돌체를 활성화합니다.");
        }

        public void DeactivateColliders()
        {
            if (Collider != null)
            {
                Collider.enabled = false;
            }

            LogProgress("바이탈의 충돌체를 비활성화합니다.");
        }

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
            if (radius.IsZero() || collider == null)
            {
                return false;
            }

            Vector3 colliderPosition = position + (Vector3)collider.offset;

            if (IsColliderInWrongDirection(colliderPosition, areaPosition, isFacingRight))
            {
                return false;
            }

            if (!IsColliderWithinRadius(colliderPosition, areaPosition, radius, collider.bounds.size))
            {
                return false;
            }

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
            Bounds bounds = new Bounds(colliderPosition, colliderSize);

            Vector3 closestPoint = bounds.ClosestPoint(areaPosition);

            float distanceSquared = (areaPosition - closestPoint).sqrMagnitude;

            return distanceSquared <= (radius * radius);
        }

        private bool IsWithinArcAngle(Vector3 colliderPosition, Vector3 areaPosition, float arcAngle)
        {
            Vector2 direction = (colliderPosition - areaPosition).normalized;
            float angle = AngleEx.ToAngle90(direction);
            float halfArc = arcAngle * 0.5f;

            return angle <= halfArc;
        }

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

            float closestX = Mathf.Clamp(areaPosition.x, vitalRect.xMin, vitalRect.xMax);
            float closestY = Mathf.Clamp(areaPosition.y, vitalRect.yMin, vitalRect.yMax);

            float distanceX = areaPosition.x - closestX;
            float distanceY = areaPosition.y - closestY;

            float distanceSquared = (distanceX * distanceX) + (distanceY * distanceY);

            return distanceSquared < (radius * radius);
        }

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

        private Rect CreateRect(Vector3 position, Vector3 size)
        {
            return new Rect(position - (size * 0.5f), size);
        }
    }
}
