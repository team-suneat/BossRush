using UnityEngine;

namespace TeamSuneat
{
    public static class AngleEx
    {
        public static float Abs(this float angle)
        {
            float result = angle;

            if (result < 0)
            {
                result += 360;
            }

            if (angle >= 360)
            {
                result %= 360;
            }

            return result;
        }

        public static float ToAngle(this Vector2 from, Vector2 to, bool useAbs = true)
        {
            if (from != null && to != null)
            {
                Vector2 vectorTo = VectorEx.Normalize(to - from);

                return ToAngle(vectorTo, useAbs);
            }

            return 0f;
        }

        public static float ToAngle(this Vector2 vectorTo, bool useAbs = true)
        {
            float angle = Mathf.Atan2(vectorTo.y, vectorTo.x) * Mathf.Rad2Deg;
            if (false == angle.IsNaN())
            {
                if (useAbs)
                {
                    return Abs(angle);
                }
                else
                {
                    return angle;
                }
            }

            return 0f;
        }

        public static float ToAngle90(this Vector2 direction)
        {
            // 각도를 구할 기준 벡터 (1, 0)
            Vector2 referenceVector = Vector2.right;

            // 내적 계산
            float dotProduct = Vector2.Dot(referenceVector, direction.normalized);

            // 정밀도 오류를 방지하기 위해 내적 값을 클램프
            dotProduct = Mathf.Clamp(dotProduct, -1.0f, 1.0f);

            // 라디안 단위로 각도 계산
            float angleInRadians = Mathf.Acos(dotProduct);

            // 각도를 도 단위로 변환
            float angleInDegrees = angleInRadians * Mathf.Rad2Deg;

            // 각도를 0에서 90 사이로 맞춤
            if (angleInDegrees > 90)
            {
                angleInDegrees = 180 - angleInDegrees;
            }

            return angleInDegrees;
        }

        public static Vector3 ToVector3(this float angle)
        {
            return Quaternion.AngleAxis(angle, Vector3.forward) * Vector3.right;
        }

        public static Vector3 ToVector3(this Quaternion quaternion)
        {
            return quaternion * Vector3.right;
        }

        public static Quaternion ToQuaternion(this Vector2 direction)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            return Quaternion.Euler(new Vector3(0, 0, angle));
        }
    }
}