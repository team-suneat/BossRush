namespace TeamSuneat
{
    using UnityEngine;

    public static class VectorEx
    {
        public static Vector2 ToVector(this bool isFacingRight)
        {
            if (isFacingRight)
            {
                return Vector2.right;
            }
            else
            {
                return Vector2.left;
            }
        }

        public static bool IsNan(this Vector2 velocity)
        {
            if (false == float.IsNaN(velocity.x))
            {
                return false;
            }

            if (false == float.IsNaN(velocity.y))
            {
                return false;
            }

            return true;
        }

        public static bool IsNan(this Vector3 velocity)
        {
            if (false == float.IsNaN(velocity.x))
            {
                return false;
            }

            if (false == float.IsNaN(velocity.y))
            {
                return false;
            }

            if (false == float.IsNaN(velocity.z))
            {
                return false;
            }

            return true;
        }

        public static bool IsZero(this Vector3 velocity)
        {
            if (false == Mathf.Approximately(0f, velocity.x))
            {
                return false;
            }

            if (false == Mathf.Approximately(0f, velocity.y))
            {
                return false;
            }

            if (false == Mathf.Approximately(0f, velocity.z))
            {
                return false;
            }

            return true;
        }

        public static bool IsOne(this Vector3 velocity)
        {
            if (false == Mathf.Approximately(1f, velocity.x))
            {
                return false;
            }

            if (false == Mathf.Approximately(1f, velocity.y))
            {
                return false;
            }

            return true;
        }

        public static bool IsZero(this Vector2 velocity)
        {
            if (false == Mathf.Approximately(0f, velocity.x))
            {
                return false;
            }

            if (false == Mathf.Approximately(0f, velocity.y))
            {
                return false;
            }

            return true;
        }

        public static bool IsOne(this Vector2 velocity)
        {
            if (false == Mathf.Approximately(1f, velocity.x))
            {
                return false;
            }

            if (false == Mathf.Approximately(1f, velocity.y))
            {
                return false;
            }

            return true;
        }

        public static Vector2 ResetX(this Vector2 vector)
        {
            return new Vector2(0, vector.y);
        }

        public static Vector3 ResetX(this Vector3 vector)
        {
            return new Vector3(0, vector.y);
        }

        public static Vector2 ResetY(this Vector2 vector)
        {
            return new Vector2(vector.x, 0);
        }

        public static Vector3 ResetY(this Vector3 vector)
        {
            return new Vector3(vector.x, 0);
        }

        public static Vector2 ApplyX(this Vector2 vector, float x)
        {
            return new Vector2(x, vector.y);
        }

        public static Vector2 ApplyY(this Vector2 vector, float y)
        {
            return new Vector2(vector.x, y);
        }

        public static Vector3 ApplyX(this Vector3 vector, float x)
        {
            return new Vector3(x, vector.y, vector.z);
        }

        public static Vector3 ApplyY(this Vector3 vector, float y)
        {
            return new Vector3(vector.x, y, vector.z);
        }

        public static Vector3 ApplyZ(this Vector3 vector, float z)
        {
            return new Vector3(vector.x, vector.y, z);
        }

        public static Vector2 FlipX(this Vector2 velocity)
        {
            return new Vector2(-velocity.x, velocity.y);
        }

        public static Vector3 FlipX(this Vector3 velocity)
        {
            return new Vector3(-velocity.x, velocity.y);
        }

        public static Vector2 Normalize(this Vector2 vector)
        {
            float distance = Mathf.Sqrt(vector.x * vector.x + vector.y * vector.y);

            return new Vector2(vector.x / distance, vector.y / distance);
        }

        public static Vector3 Normalize(this Vector3 vector)
        {
            float distance = Mathf.Sqrt(vector.x * vector.x + vector.y * vector.y);

            return new Vector3(vector.x / distance, vector.y / distance);
        }

        public static Vector2 RoundWithDigits(this Vector2 vector, int digits = 6)
        {
            Vector2 result;

            result.x = vector.x.RoundWithDigits(digits);
            result.y = vector.y.RoundWithDigits(digits);

            return result;
        }

        public static Vector3 RoundWithDigits(this Vector3 vector, int digits = 6)
        {
            Vector3 result;

            result.x = vector.x.RoundWithDigits(digits);
            result.y = vector.y.RoundWithDigits(digits);
            result.z = vector.z.RoundWithDigits(digits);

            return result;
        }

        public static Vector2 Rotate(this Vector2 vector, float angleInDegrees)
        {
            float sin = Mathf.Sin(angleInDegrees * Mathf.Deg2Rad);
            float cos = Mathf.Cos(angleInDegrees * Mathf.Deg2Rad);
            float tx = vector.x;
            float ty = vector.y;
            vector.x = (cos * tx) - (sin * ty);
            vector.y = (sin * tx) + (cos * ty);
            return vector;
        }

        public static Vector2 CalculateDifference(Vector2 a, Vector2 b)
        {
            float xDiff = (a.x >= b.x) ? a.x - b.x : b.x - a.x;
            float yDiff = (a.y >= b.y) ? a.y - b.y : b.y - a.y;

            return new Vector2(xDiff, yDiff);
        }

        public static Vector3 CalculateDifference(Vector3 a, Vector3 b)
        {
            float xDiff = (a.x >= b.x) ? a.x - b.x : b.x - a.x;
            float yDiff = (a.y >= b.y) ? a.y - b.y : b.y - a.y;
            float zDiff = (a.z >= b.z) ? a.z - b.z : b.z - a.z;

            return new Vector3(xDiff, yDiff, zDiff);
        }
    }
}