using TeamSuneat;
using UnityEngine;
using UnityEngine.Events;

namespace TeamSuneat.Development
{
    // 개발 도구 GUI 데이터 및 렌더링 메서드를 관리하는 클래스
    public class DevelopmentToolsGUI
    {
        private Vector2 _scrollPosition = Vector2.zero;

        [field: SerializeField]
        public float WindowWidth { get; private set; }

        [field: SerializeField]
        public float WindowHeight { get; private set; }

        [field: SerializeField]
        public float ButtonWidth { get; } = 128f;

        [field: SerializeField]
        public float ButtonHeight { get; } = 32f;

        public Vector2 ScrollPosition { get => _scrollPosition; set => _scrollPosition = value; }

        public GUIStyle TitleStyle { get; private set; }
        public GUIStyle ButtonStyle { get; private set; }
        public GUIStyle ContentStyle { get; private set; }
        public GUIStyle SubContentStyle { get; private set; }
        public GUIStyle TextureStyle { get; private set; }
        public GUIStyle WindowStyle { get; private set; }

        public bool IsEditorMode { get; set; } = false;

        // 윈도우 크기를 갱신합니다
        public void RefreshSize(float width, float height)
        {
            WindowWidth = width;
            WindowHeight = height;
        }

        // GUI 스타일을 초기화합니다
        public void RefreshStyle(bool isEditor = false)
        {
            IsEditorMode = isEditor;

#if UNITY_EDITOR
            if (isEditor)
            {
                // EditorStyles가 null일 수 있으므로 체크
                if (UnityEditor.EditorStyles.boldLabel != null)
                {
                    TitleStyle = new GUIStyle(UnityEditor.EditorStyles.boldLabel)
                    {
                        fontSize = 14,
                        fontStyle = FontStyle.Bold,
                        alignment = TextAnchor.MiddleLeft,
                        richText = true,
                    };
                }
                else
                {
                    TitleStyle = new GUIStyle()
                    {
                        fontSize = 14,
                        fontStyle = FontStyle.Bold,
                        alignment = TextAnchor.MiddleLeft,
                        richText = true,
                    };
                }

                if (UnityEditor.EditorStyles.label != null)
                {
                    ContentStyle = new GUIStyle(UnityEditor.EditorStyles.label)
                    {
                        fontSize = 11,
                        fontStyle = FontStyle.Normal,
                        alignment = TextAnchor.MiddleLeft,
                        wordWrap = true,
                        richText = true,
                    };
                }
                else
                {
                    ContentStyle = new GUIStyle()
                    {
                        fontSize = 11,
                        fontStyle = FontStyle.Normal,
                        alignment = TextAnchor.MiddleLeft,
                        wordWrap = true,
                        richText = true,
                    };
                }
            }
            else
#endif
            {
                // OnGUI 외부에서도 호출될 수 있으므로 항상 기본값으로 초기화
                // GUI.skin은 OnGUI 내부에서만 접근 가능하므로 여기서는 기본값만 설정
                TitleStyle = new GUIStyle()
                {
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleLeft,
                    fontSize = 14,
                    richText = true,
                };
                TitleStyle.normal.textColor = GameColors.CreamIvory;

                ContentStyle = new GUIStyle()
                {
                    fontStyle = FontStyle.Normal,
                    alignment = TextAnchor.MiddleLeft,
                    fontSize = 12,
                    wordWrap = true,
                    richText = true,
                };
                ContentStyle.normal.textColor = GameColors.CreamIvory;

                SubContentStyle = new GUIStyle()
                {
                    fontStyle = FontStyle.Italic,
                    alignment = TextAnchor.MiddleLeft,
                    fontSize = 11,
                    wordWrap = true,
                    richText = true,
                };
                SubContentStyle.normal.textColor = GameColors.DarkGray;

                ButtonStyle = new GUIStyle()
                {
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 14,
                    richText = true,
                };
                ButtonStyle.normal.textColor = GameColors.CreamIvory;

                TextureStyle = new GUIStyle()
                {
                    alignment = TextAnchor.MiddleCenter,
                };

                WindowStyle = new GUIStyle();
            }
        }

        // GUI.skin을 기반으로 스타일을 업데이트합니다 (OnGUI 내부에서만 호출 가능)
        public void RefreshStyleFromSkin()
        {
            if (GUI.skin == null)
            {
                return;
            }

            // GUI.skin을 기반으로 스타일 업데이트
            TitleStyle = new GUIStyle(GUI.skin.label)
            {
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft,
                fontSize = 14,
                richText = true,
            };
            TitleStyle.normal.textColor = GameColors.CreamIvory;

            ContentStyle = new GUIStyle(GUI.skin.label)
            {
                fontStyle = FontStyle.Normal,
                alignment = TextAnchor.MiddleLeft,
                fontSize = 12,
                wordWrap = true,
                richText = true,
            };
            ContentStyle.normal.textColor = GameColors.CreamIvory;

            SubContentStyle = new GUIStyle(GUI.skin.label)
            {
                fontStyle = FontStyle.Italic,
                alignment = TextAnchor.MiddleLeft,
                fontSize = 11,
                wordWrap = true,
                richText = true,
            };
            SubContentStyle.normal.textColor = GameColors.DarkGray;

            ButtonStyle = new GUIStyle(GUI.skin.button)
            {
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                fontSize = 14,
                richText = true,
            };
            ButtonStyle.normal.textColor = GameColors.CreamIvory;

            WindowStyle = new GUIStyle(GUI.skin.window);
        }

        // 레이아웃 옵션 생성 헬퍼
        private GUILayoutOption[] GetLayoutOptions(bool useWidth, bool useHeight)
        {
            if (!useWidth && !useHeight)
            {
                return new GUILayoutOption[]
                {
                    GUILayout.ExpandWidth(false),
                    GUILayout.MinHeight(ButtonHeight)
                };
            }
            else if (useWidth && useHeight)
            {
                return new GUILayoutOption[]
                {
                    GUILayout.Width(ButtonWidth),
                    GUILayout.Height(ButtonHeight)
                };
            }
            else if (useWidth)
            {
                return new GUILayoutOption[]
                {
                    GUILayout.Width(ButtonWidth),
                    GUILayout.MinHeight(ButtonHeight)
                };
            }
            else
            {
                return new GUILayoutOption[]
                {
                    GUILayout.ExpandWidth(false),
                    GUILayout.Height(ButtonHeight)
                };
            }
        }

        // 버튼을 그립니다 (Runtime용)
        public void DrawButton(string text, UnityAction onClick, bool useWidth = true, bool useHeight = true)
        {
            if (IsEditorMode)
            {
                DrawButtonEditor(text, onClick, useWidth, useHeight);
                return;
            }

            // 스타일이 초기화되지 않았으면 기본 스타일 사용
            GUIStyle style = ButtonStyle;
            if (style == null)
            {
                style = new GUIStyle();
            }
            style.richText = true;

            GUILayoutOption[] options = GetLayoutOptions(useWidth, useHeight);
            if (GUILayout.Button(text, style, options))
            {
                onClick?.Invoke();
            }
        }

#if UNITY_EDITOR

        // 버튼을 그립니다 (Editor용)
        private void DrawButtonEditor(string text, UnityAction onClick, bool useWidth, bool useHeight)
        {
            GUILayoutOption[] options = GetLayoutOptions(useWidth, useHeight);
            if (GUILayout.Button(text, options))
            {
                onClick?.Invoke();
            }
        }

#endif

        // 제목 레이블을 그립니다
        public void DrawTitleLabel(string text, bool useWidth = false, bool useHeight = true)
        {
            // 스타일이 초기화되지 않았으면 초기화 시도
            if (TitleStyle == null && GUI.skin != null)
            {
                RefreshStyle(IsEditorMode);
            }

#if UNITY_EDITOR
            if (IsEditorMode)
            {
                UnityEditor.EditorGUILayout.LabelField(text, TitleStyle);
                return;
            }
#endif

            GUILayoutOption[] options = GetLayoutOptions(useWidth, useHeight);
            GUILayout.Label(text, TitleStyle, options);
        }

        // 내용 레이블을 그립니다
        public void DrawContentLabel(string text, bool useWidth = false, bool useHeight = false)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

#if UNITY_EDITOR
            if (IsEditorMode)
            {
                UnityEditor.EditorGUILayout.LabelField(text, ContentStyle);
                return;
            }
#endif

            // 스타일이 초기화되지 않았으면 초기화 시도
            if (ContentStyle == null && GUI.skin != null)
            {
                RefreshStyle(IsEditorMode);
            }

            GUILayoutOption[] options = GetLayoutOptions(useWidth, useHeight);
            GUILayout.Label(text, ContentStyle, options);
        }

        // 서브 내용 레이블을 그립니다
        public void DrawSubContentLabel(string text, bool useWidth = false, bool useHeight = false)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            GUILayoutOption[] options = GetLayoutOptions(useWidth, useHeight);
            GUILayout.Label(text, SubContentStyle, options);
        }

        // 토글 버튼을 그립니다
        public bool DrawToggleButton(bool value, bool useColor = true, bool useWidth = true, bool useHeight = true)
        {
            string buttonText = value ? "On" : "Off";

            if (useColor)
            {
                if (value)
                {
                    buttonText = buttonText.ToSelectString();
                }
                else
                {
                    buttonText = buttonText.ToDisableString();
                }
            }

            GUILayoutOption[] options = GetLayoutOptions(useWidth, useHeight);
            if (GUILayout.Button(buttonText, options))
            {
                return !value;
            }

            return value;
        }

        // SelectionGrid를 그립니다
        public int DrawSelectionGrid(int index, string[] contents, int count, bool useWidth = true, bool useHeight = true)
        {
            int contentCount = contents.Length;
            int row = Mathf.CeilToInt((float)contentCount / count);
            
            float width = ButtonWidth * count;
            float height = ButtonHeight * row;
            
            int result = index;
            GUILayout.BeginVertical("box");
            {
                GUILayoutOption[] options;
                if (!useWidth && !useHeight)
                {
                    options = new GUILayoutOption[]
                    {
                        GUILayout.ExpandWidth(false),
                        GUILayout.MinHeight(height)
                    };
                }
                else if (useWidth && useHeight)
                {
                    options = new GUILayoutOption[]
                    {
                        GUILayout.Width(width),
                        GUILayout.Height(height)
                    };
                }
                else if (useWidth)
                {
                    options = new GUILayoutOption[]
                    {
                        GUILayout.Width(width),
                        GUILayout.MinHeight(height)
                    };
                }
                else
                {
                    options = new GUILayoutOption[]
                    {
                        GUILayout.ExpandWidth(false),
                        GUILayout.Height(height)
                    };
                }
                
                result = GUILayout.SelectionGrid(index, contents, count, options);
            }
            GUILayout.EndVertical();
            
            return result;
        }

        // 박스를 그립니다
        public void DrawBox(GUIContent content, int width, int height)
        {
            GUILayout.Box(content, TextureStyle, GUILayout.Width(width), GUILayout.Height(height));
        }
    }
}