using UnityEngine;

public static class DebugEx
{
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DrawRay(Vector2 start, Vector2 dir)
    {
        Debug.DrawRay(start, dir);
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DrawRay(Vector2 start, Vector2 dir, Color color)
    {
        Debug.DrawRay(start, dir, color);
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DrawRay(Vector2 start, Vector2 dir, Color color, float duration)
    {
        Debug.DrawRay(start, dir, color, duration);
    }

    //

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DrawLine(Vector2 start, Vector2 end, Color color)
    {
        Debug.DrawLine(start, end, color);
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DrawLine(Vector2 start, Vector2 end, Color color, float duration)
    {
        Debug.DrawLine(start, end, color, duration);
    }

    //

    public static void DebugDrawArrow(Vector3 origin, Vector3 direction, Color color, float duration, float arrowHeadLength = 0.2f, float arrowHeadAngle = 35f)
    {
        Debug.DrawRay(origin, direction, color, duration);

        DrawArrowEnd(false, origin, direction, color, duration, arrowHeadLength, arrowHeadAngle);
    }

    private static void DrawArrowEnd(bool drawGizmos, Vector3 arrowEndPosition, Vector3 direction, Color color, float duration, float arrowHeadLength = 0.25f, float arrowHeadAngle = 40.0f)
    {
        if (direction == Vector3.zero)
        {
            return;
        }

        Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(arrowHeadAngle, 0, 0) * Vector3.back;
        Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(-arrowHeadAngle, 0, 0) * Vector3.back;
        Vector3 up = Quaternion.LookRotation(direction) * Quaternion.Euler(0, arrowHeadAngle, 0) * Vector3.back;
        Vector3 down = Quaternion.LookRotation(direction) * Quaternion.Euler(0, -arrowHeadAngle, 0) * Vector3.back;

        if (drawGizmos)
        {
            Gizmos.color = color;
            Gizmos.DrawRay(arrowEndPosition + direction, right * arrowHeadLength);
            Gizmos.DrawRay(arrowEndPosition + direction, left * arrowHeadLength);
            Gizmos.DrawRay(arrowEndPosition + direction, up * arrowHeadLength);
            Gizmos.DrawRay(arrowEndPosition + direction, down * arrowHeadLength);
        }
        else
        {
            Debug.DrawRay(arrowEndPosition + direction, right * arrowHeadLength, color, duration);
            Debug.DrawRay(arrowEndPosition + direction, left * arrowHeadLength, color, duration);
            Debug.DrawRay(arrowEndPosition + direction, up * arrowHeadLength, color, duration);
            Debug.DrawRay(arrowEndPosition + direction, down * arrowHeadLength, color, duration);
        }
    }

    //

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DrawRectangle(Rect rect, Color color, float duration = 0f)
    {
        Vector3 v3TopLeft = new Vector3(rect.xMin, rect.yMax, 0);
        Vector3 v3TopRight = new Vector3(rect.xMax, rect.yMax, 0);
        Vector3 v3BottomRight = new Vector3(rect.xMax, rect.yMin, 0);
        Vector3 v3BottomLeft = new Vector3(rect.xMin, rect.yMin, 0);

        if (Mathf.Approximately(0, duration))
        {
            Debug.DrawLine(v3TopLeft, v3TopRight, color);
            Debug.DrawLine(v3TopRight, v3BottomRight, color);
            Debug.DrawLine(v3BottomRight, v3BottomLeft, color);
            Debug.DrawLine(v3BottomLeft, v3TopLeft, color);
        }
        else
        {
            Debug.DrawLine(v3TopLeft, v3TopRight, color, duration);
            Debug.DrawLine(v3TopRight, v3BottomRight, color, duration);
            Debug.DrawLine(v3BottomRight, v3BottomLeft, color, duration);
            Debug.DrawLine(v3BottomLeft, v3TopLeft, color, duration);
        }
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DrawRectangle(Vector2 center, Vector2 size, float duration = 0f)
    {
        size *= 0.5f;

        Vector3 v3TopLeft = new Vector3(center.x - size.x, center.y + size.y, 0);
        Vector3 v3TopRight = new Vector3(center.x + size.x, center.y + size.y, 0);
        Vector3 v3BottomRight = new Vector3(center.x + size.x, center.y - size.y, 0);
        Vector3 v3BottomLeft = new Vector3(center.x - size.x, center.y - size.y, 0);

        if (Mathf.Approximately(0, duration))
        {
            Debug.DrawLine(v3TopLeft, v3TopRight);
            Debug.DrawLine(v3TopRight, v3BottomRight);
            Debug.DrawLine(v3BottomRight, v3BottomLeft);
            Debug.DrawLine(v3BottomLeft, v3TopLeft);
        }
        else
        {
            Debug.DrawLine(v3TopLeft, v3TopRight, Color.white, duration);
            Debug.DrawLine(v3TopRight, v3BottomRight, Color.white, duration);
            Debug.DrawLine(v3BottomRight, v3BottomLeft, Color.white, duration);
            Debug.DrawLine(v3BottomLeft, v3TopLeft, Color.white, duration);
        }
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DrawRectangle(Vector2 center, Vector2 size, Color color, float duration = 0f)
    {
        size *= 0.5f;

        Vector3 v3TopLeft = new Vector3(center.x - size.x, center.y + size.y, 0);
        Vector3 v3TopRight = new Vector3(center.x + size.x, center.y + size.y, 0);
        Vector3 v3BottomRight = new Vector3(center.x + size.x, center.y - size.y, 0);
        Vector3 v3BottomLeft = new Vector3(center.x - size.x, center.y - size.y, 0);

        if (Mathf.Approximately(0, duration))
        {
            Debug.DrawLine(v3TopLeft, v3TopRight, color);
            Debug.DrawLine(v3TopRight, v3BottomRight, color);
            Debug.DrawLine(v3BottomRight, v3BottomLeft, color);
            Debug.DrawLine(v3BottomLeft, v3TopLeft, color);
        }
        else
        {
            Debug.DrawLine(v3TopLeft, v3TopRight, color, duration);
            Debug.DrawLine(v3TopRight, v3BottomRight, color, duration);
            Debug.DrawLine(v3BottomRight, v3BottomLeft, color, duration);
            Debug.DrawLine(v3BottomLeft, v3TopLeft, color, duration);
        }
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DrawRectangle(float minX, float maxX, float minY, float maxY, Color color, float duration = 0f)
    {
        Vector3 v3TopLeft = new Vector3(minX, maxY, 0);
        Vector3 v3TopRight = new Vector3(maxX, maxY, 0);
        Vector3 v3BottomRight = new Vector3(maxX, minY, 0);
        Vector3 v3BottomLeft = new Vector3(minX, minY, 0);

        if (Mathf.Approximately(0, duration))
        {
            Debug.DrawLine(v3TopLeft, v3TopRight, color);
            Debug.DrawLine(v3TopRight, v3BottomRight, color);
            Debug.DrawLine(v3BottomRight, v3BottomLeft, color);
            Debug.DrawLine(v3BottomLeft, v3TopLeft, color);
        }
        else
        {
            Debug.DrawLine(v3TopLeft, v3TopRight, color, duration);
            Debug.DrawLine(v3TopRight, v3BottomRight, color, duration);
            Debug.DrawLine(v3BottomRight, v3BottomLeft, color, duration);
            Debug.DrawLine(v3BottomLeft, v3TopLeft, color, duration);
        }
    }

    //

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DrawCircle(Vector3 center, float radius, Color color, float duration = 0f)
    {
        int segments = 32;
        float angleStep = 360f / segments;
        Vector3 prevPoint = center + new Vector3(Mathf.Cos(0), Mathf.Sin(0)) * radius;

        for (int i = 1; i <= segments; i++)
        {
            float rad = Mathf.Deg2Rad * (i * angleStep);
            Vector3 nextPoint = center + new Vector3(Mathf.Cos(rad), Mathf.Sin(rad)) * radius;

            Debug.DrawLine(prevPoint, nextPoint, color, duration);
            prevPoint = nextPoint;
        }
    }

    //

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DrawCross(Vector2 center, float size, float duration = 0f)
    {
        if (duration > 0)
        {
            DrawLine(center + new Vector2(-size, size), center + new Vector2(size, -size), Color.white, duration);
            DrawLine(center + new Vector2(size, size), center + new Vector2(-size, -size), Color.white, duration);
        }
        else
        {
            DrawLine(center + new Vector2(-size, size), center + new Vector2(size, -size), Color.white);
            DrawLine(center + new Vector2(size, size), center + new Vector2(-size, -size), Color.white);
        }
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DrawCross(Vector2 center, float size, Color color, float duration = 0f)
    {
        if (duration > 0)
        {
            DrawLine(center + new Vector2(-size, size), center + new Vector2(size, -size), color, duration);
            DrawLine(center + new Vector2(size, size), center + new Vector2(-size, -size), color, duration);
        }
        else
        {
            DrawLine(center + new Vector2(-size, size), center + new Vector2(size, -size), color);
            DrawLine(center + new Vector2(size, size), center + new Vector2(-size, -size), color);
        }
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DrawPlus(Vector2 center, float size, float duration = 0f)
    {
        if (duration > 0)
        {
            DrawLine(center + new Vector2(-size, 0), center + new Vector2(size, 0), Color.white, duration);
            DrawLine(center + new Vector2(0, -size), center + new Vector2(0, size), Color.white, duration);
        }
        else
        {
            DrawLine(center + new Vector2(-size, 0), center + new Vector2(size, 0), Color.white);
            DrawLine(center + new Vector2(0, -size), center + new Vector2(0, size), Color.white);
        }
    }
}