using UnityEditor;
using UnityEngine;

namespace TeamSuneat
{
    public static class GizmoEx
    {
        private static Color backupColor;

        #region Text

        public static void DrawText(string text, Vector3 position)
        {
#if UNITY_EDITOR
            GUIStyle centeredStyle = GUI.skin.GetStyle("Label");
            centeredStyle.normal.textColor = Color.white;
            centeredStyle.alignment = TextAnchor.UpperCenter;
            centeredStyle.fontStyle = FontStyle.Bold;
            centeredStyle.fontSize = 12;
            centeredStyle.richText = true;

            HandleLabel(position, text, centeredStyle);
#endif
        }

        public static void DrawText(string text, Vector3 position, Color? color)
        {
#if UNITY_EDITOR
            GUIStyle centeredStyle = GUI.skin.GetStyle("Label");
            centeredStyle.alignment = TextAnchor.UpperCenter;
            centeredStyle.fontStyle = FontStyle.Bold;
            centeredStyle.fontSize = 12;
            centeredStyle.richText = true;

            text = string.Format("<color={0}>{1}</color>", ColorEx.ColorToHex(color.Value), text);

            HandleLabel(position, text, centeredStyle);
#endif
        }

        public static void DrawText(string text, Vector3 position, int fontSize)
        {
#if UNITY_EDITOR
            GUIStyle centeredStyle = GUI.skin.GetStyle("Label");

            centeredStyle.alignment = TextAnchor.UpperCenter;
            centeredStyle.fontStyle = FontStyle.Bold;
            centeredStyle.fontSize = fontSize;
            centeredStyle.richText = true;

            HandleLabel(position, text, centeredStyle);
#endif
        }

        private static void HandleLabel(Vector3 position, string text, GUIStyle style)
        {
#if UNITY_EDITOR
            Handles.Label(position, text, style);
#endif
        }

        #endregion Text

        #region Ray

        public static void DrawGizmoRay(Vector3 v3From, Vector3 v3Dir)
        {
#if UNITY_EDITOR
            Gizmos.DrawRay(v3From, v3Dir);
#endif
        }

        public static void DrawGizmoRay(Vector3 v3From, Vector3 v3Dir, Color color)
        {
#if UNITY_EDITOR
            backupColor = Gizmos.color;
            Gizmos.color = color;
            Gizmos.DrawRay(v3From, v3Dir);
            Gizmos.color = backupColor;
#endif
        }

        #endregion Ray

        #region Line

        public static void DrawGizmoLine(Vector3 v3From, Vector3 v3To)
        {
#if UNITY_EDITOR
            Gizmos.DrawLine(v3From, v3To);
#endif
        }

        public static void DrawGizmoLine(Vector3 v3From, Vector3 v3To, Color color)
        {
#if UNITY_EDITOR
            backupColor = Gizmos.color;
            Gizmos.color = color;
            Gizmos.DrawLine(v3From, v3To);
            Gizmos.color = backupColor;
#endif
        }

        #endregion Line

        #region Cross

        public static void DrawGizmoCross(Vector2 center, float size)
        {
#if UNITY_EDITOR
            DrawGizmoLine(center + new Vector2(-size, size), center + new Vector2(size, -size), Color.white);
            DrawGizmoLine(center + new Vector2(size, size), center + new Vector2(-size, -size), Color.white);
#endif
        }

        public static void DrawGizmoCross(Vector2 center, float size, Color color)
        {
#if UNITY_EDITOR
            DrawGizmoLine(center + new Vector2(-size, size), center + new Vector2(size, -size), color);
            DrawGizmoLine(center + new Vector2(size, size), center + new Vector2(-size, -size), color);
#endif
        }

        #endregion Cross

        #region Rectangle

        public static void DrawGizmoRectangle(Vector3 v3TopLeft, Vector3 v3TopRight, Vector3 v3BottomRight, Vector3 v3BottomLeft, Color color)
        {
#if UNITY_EDITOR
            backupColor = Gizmos.color;
            Gizmos.color = color;
            Gizmos.DrawLine(v3TopLeft, v3TopRight);
            Gizmos.DrawLine(v3TopRight, v3BottomRight);
            Gizmos.DrawLine(v3BottomRight, v3BottomLeft);
            Gizmos.DrawLine(v3BottomLeft, v3TopLeft);
            Gizmos.color = backupColor;
#endif
        }

        public static void DrawGizmoRectangle(Vector2 center, Vector2 size)
        {
#if UNITY_EDITOR
            Vector3 v3TopLeft = new Vector3(center.x - size.x * 0.5f, center.y + size.y * 0.5f, 0);
            Vector3 v3TopRight = new Vector3(center.x + size.x * 0.5f, center.y + size.y * 0.5f, 0);
            Vector3 v3BottomRight = new Vector3(center.x + size.x * 0.5f, center.y - size.y * 0.5f, 0);
            Vector3 v3BottomLeft = new Vector3(center.x - size.x * 0.5f, center.y - size.y * 0.5f, 0);

            backupColor = Gizmos.color;
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(v3TopLeft, v3TopRight);
            Gizmos.DrawLine(v3TopRight, v3BottomRight);
            Gizmos.DrawLine(v3BottomRight, v3BottomLeft);
            Gizmos.DrawLine(v3BottomLeft, v3TopLeft);
            Gizmos.color = backupColor;
#endif
        }

        public static void DrawGizmoRectangle(Vector2 center, Vector2 size, Color color)
        {
#if UNITY_EDITOR
            Vector3 v3TopLeft = new Vector3(center.x - size.x * 0.5f, center.y + size.y * 0.5f, 0);
            Vector3 v3TopRight = new Vector3(center.x + size.x * 0.5f, center.y + size.y * 0.5f, 0);
            Vector3 v3BottomRight = new Vector3(center.x + size.x * 0.5f, center.y - size.y * 0.5f, 0);
            Vector3 v3BottomLeft = new Vector3(center.x - size.x * 0.5f, center.y - size.y * 0.5f, 0);

            backupColor = Gizmos.color;
            Gizmos.color = color;
            Gizmos.DrawLine(v3TopLeft, v3TopRight);
            Gizmos.DrawLine(v3TopRight, v3BottomRight);
            Gizmos.DrawLine(v3BottomRight, v3BottomLeft);
            Gizmos.DrawLine(v3BottomLeft, v3TopLeft);
            Gizmos.color = backupColor;
#endif
        }

        #endregion Rectangle

        #region Cube

        public static void DrawGizmoCube(Vector2 center, Vector2 size)
        {
#if UNITY_EDITOR
            backupColor = Gizmos.color;
            Color gizmoColor = Color.magenta;
            gizmoColor.a = 0.5f;
            Gizmos.color = gizmoColor;
            Gizmos.DrawCube(center, size);
            Gizmos.color = backupColor;
#endif
        }

        public static void DrawGizmoCube(Vector2 center, Vector2 size, Color color)
        {
#if UNITY_EDITOR
            backupColor = Gizmos.color;
            Color gizmoColor = color;
            gizmoColor.a = 0.5f;
            Gizmos.color = gizmoColor;
            Gizmos.DrawCube(center, size);
            Gizmos.color = backupColor;
#endif
        }

        #endregion Cube

        #region Circle

        public static void DrawWireSphere(Vector2 center, float radius)
        {
#if UNITY_EDITOR
            Gizmos.DrawWireSphere(center, radius);
#endif
        }

        public static void DrawWireSphere(Vector2 center, float radius, Color color)
        {
#if UNITY_EDITOR
            backupColor = Gizmos.color;
            Gizmos.color = color;
            Gizmos.DrawWireSphere(center, radius);
            Gizmos.color = backupColor;
#endif
        }

        #endregion Circle

        #region Arc

        public static void DrawWireArc(Vector3 position, Vector3 direction, float angleRange, float radius, Color gizmoColor)
        {
#if UNITY_EDITOR
            backupColor = Gizmos.color;
            Handles.color = gizmoColor;
            Handles.DrawWireArc(position, Vector3.forward, direction, angleRange, radius);
            Handles.color = backupColor;
#endif
        }

        public static void DrawSolidArc(Vector3 position, Vector3 direction, float angleRange, float radius, Color gizmoColor)
        {
#if UNITY_EDITOR
            backupColor = Gizmos.color;
            gizmoColor.a = 0.25f;
            Handles.color = gizmoColor;
            Handles.DrawSolidArc(position, Vector3.forward, direction, angleRange, radius);

            Handles.color = backupColor;
#endif
        }

        #endregion Arc
    }
}