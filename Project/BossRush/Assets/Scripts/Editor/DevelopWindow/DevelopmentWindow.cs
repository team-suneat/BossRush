#if UNITY_EDITOR
using TeamSuneat.Data;
using UnityEditor;
using UnityEngine;

namespace TeamSuneat.Development
{
    public class DevelopmentWindow : EditorWindow
    {
        private Vector2 _scrollPosition;
        private GUIStyle _titleStyle;
        private GUIStyle _contentStyle;

        [MenuItem("Tools/개발 도구")]
        private static void ShowWindow()
        {
            DevelopmentWindow window = GetWindow<DevelopmentWindow>("Development Tools");
            window.Show();
        }

        private void OnEnable()
        {
            InitializeStyles();
        }

        private void InitializeStyles()
        {
            _titleStyle ??= new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold
            };

            _contentStyle ??= new GUIStyle(EditorStyles.label)
            {
                fontSize = 11,
                fontStyle = FontStyle.Normal,
            };
        }

        private void DrawTitleLabel(string title)
        {
            InitializeStyles();
            EditorGUILayout.LabelField(title, _titleStyle);
        }

        private void DrawContentLabel(string content)
        {
            InitializeStyles();
            EditorGUILayout.LabelField(content, _contentStyle);
        }

        private void OnGUI()
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            EditorGUILayout.LabelField("[개발용 에디터 윈도우]", EditorStyles.boldLabel);
            EditorGUILayout.Space(10);

            DrawPathManagerSection();
            EditorGUILayout.Space(10);

            DrawJsonDataManagerSection();
            EditorGUILayout.Space(10);

            DrawExcelSection();
            EditorGUILayout.Space(10);

            DrawGoogleSheetsSection();
            EditorGUILayout.Space(10);

            DrawLogLevelSection();

            EditorGUILayout.EndScrollView();
        }

        private void DrawPathManagerSection()
        {
            _ = EditorGUILayout.BeginVertical("box");
            DrawTitleLabel("Path Manager");

            if (GUILayout.Button("파일 경로 저장", GUILayout.Width(250)))
            {
                PathManager.UpdatePathMetaData();
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawJsonDataManagerSection()
        {
            _ = EditorGUILayout.BeginVertical("box");
            DrawTitleLabel("Json Data Manager");

            if (GUILayout.Button("JSON 시트 불러오기", GUILayout.Width(250)))
            {
                JsonDataManager.LoadJsonSheetsSync();
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawExcelSection()
        {
            _ = EditorGUILayout.BeginVertical("box");
            DrawTitleLabel("Excel");

            if (GUILayout.Button("모든 엑셀 파일 불러오기", GUILayout.Width(250)))
            {
                Excel4Unity.ConvertAllExcelToJSON();
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawGoogleSheetsSection()
        {
            _ = EditorGUILayout.BeginVertical("box");
            DrawTitleLabel("Google Sheets");

            if (GUILayout.Button("모든 시트 불러오기 (GID 목록)", GUILayout.Width(250)))
            {
                GoogleSheetsMenu.LoadMultipleSheetsByGIDs();
            }

            if (GUILayout.Button("JSON 변환 - 전체", GUILayout.Width(250)))
            {
                GoogleSheetsMenu.ConvertAllToJson();
            }

            if (GUILayout.Button("JSON 변환 - Stat", GUILayout.Width(250)))
            {
                GoogleSheetsMenu.ConvertStatToJson();
            }

            if (GUILayout.Button("JSON 변환 - String", GUILayout.Width(250)))
            {
                GoogleSheetsMenu.ConvertStringToJson();
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawLogLevelSection()
        {
            _ = EditorGUILayout.BeginVertical("box");
            DrawTitleLabel("Log Level");

            bool progress = Log.LevelProgress;
            bool info = Log.LevelInfo;
            bool warning = Log.LevelWarning;
            bool error = Log.LevelError;
            bool except = Log.LevelExcept;

            bool newProgress = EditorGUILayout.Toggle("Progress", progress);
            if (newProgress != progress)
            {
                Log.SwitchLogLevelProgress();
            }

            bool newInfo = EditorGUILayout.Toggle("Info", info);
            if (newInfo != info)
            {
                Log.SwitchLogLevelInfo();
            }

            bool newWarning = EditorGUILayout.Toggle("Warning", warning);
            if (newWarning != warning)
            {
                Log.SwitchLogLevelWarning();
            }

            bool newError = EditorGUILayout.Toggle("Error", error);
            if (newError != error)
            {
                Log.SwitchLogLevelError();
            }

            bool newExcept = EditorGUILayout.Toggle("Except", except);
            if (newExcept != except)
            {
                Log.SwitchLogLevelExcept();
            }

            EditorGUILayout.Space(5);

            _ = EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("All On", GUILayout.Width(120)))
            {
                Log.SetLogLevelAll();
            }

            if (GUILayout.Button("All Off", GUILayout.Width(120)))
            {
                Log.SetLogLevelOff();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }
    }
}
#endif