using DG.Tweening;
using Sirenix.OdinInspector;
using TeamSuneat.Setting;
using TMPro;
using UnityEngine;

namespace TeamSuneat.UserInterface
{
    public class UILocalizedText : XBehaviour, IUITextLocalizable
    {
        [Title("#UI Localized Text")]
        [SerializeField]
        private TextMeshProUGUI _textPro;

        [SerializeField]
        private UITextLocalizer _textLocalizer;

        [SerializeField]
        private UITextFontLocalizer _fontLocalizer;

        public string StringKey
        {
            get => TextLocalizer?.StringKey ?? string.Empty;
            set
            {
                if (TextLocalizer != null)
                {
                    TextLocalizer.SetStringKey(value);
                }
            }
        }

        public GameFontTypes FontType
        {
            get => FontLocalizer?.FontType ?? GameFontTypes.None;
            set
            {
                if (FontLocalizer != null)
                {
                    FontLocalizer.FontType = value;
                    FontLocalizer.FontTypeString = value.ToString();
                }
            }
        }

        public string FontTypeString
        {
            get => FontLocalizer?.FontTypeString ?? string.Empty;
            set
            {
                if (FontLocalizer != null)
                {
                    FontLocalizer.FontTypeString = value;
                    if (!EnumEx.ConvertTo(ref FontLocalizer.FontType, value))
                    {
                        Log.Error(LogTags.Font, "{0} 폰트 타입이 변환되지 않습니다. {1}", FontLocalizer.FontType, value);
                    }
                }
            }
        }

        public Color DefaultTextColor { get; private set; }

        public int FontSize => _textPro != null ? (int)_textPro.fontSize : 0;

        public string Content => _textPro != null ? _textPro.text : string.Empty;

        #region 컴포넌트 할당

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _textPro ??= GetComponent<TextMeshProUGUI>();
            _textLocalizer ??= GetComponent<UITextLocalizer>();
            _fontLocalizer ??= GetComponent<UITextFontLocalizer>();
        }

        public override void AutoAddComponents()
        {
            base.AutoAddComponents();

            if (_textLocalizer == null)
            {
                _textLocalizer = gameObject.AddComponent<UITextLocalizer>();
            }

            if (_fontLocalizer == null)
            {
                _fontLocalizer = gameObject.AddComponent<UITextFontLocalizer>();
            }
        }

        #endregion 컴포넌트 할당

        #region 초기화 및 이벤트

        public override void AutoSetting()
        {
            base.AutoSetting();

            if (TextLocalizer != null && !string.IsNullOrEmpty(TextLocalizer.StringKey))
            {
                TextLocalizer.StringKey = TextLocalizer.StringKey.Replace(" ", "");
            }
        }

        protected void Awake()
        {
            _textPro ??= GetComponent<TextMeshProUGUI>();
            _textLocalizer ??= GetComponent<UITextLocalizer>();
            _fontLocalizer ??= GetComponent<UITextFontLocalizer>();

            Initialize();
        }

        private void Initialize()
        {
            if (_textPro != null)
            {
                DefaultTextColor = _textPro.color;
                FontLocalizer?.InitializeDefaultFontSize(_textPro.fontSize);
            }
        }

        protected override void OnEnabled()
        {
            base.OnEnabled();

            RefreshLanguage(GameSetting.Instance.Language.Name);
        }

        #endregion 초기화 및 이벤트

        #region 활성화

        public void Activate()
        {
            if (_textPro != null)
            {
                _textPro.enabled = true;
            }
        }

        public void Deactivate()
        {
            if (_textPro != null)
            {
                _textPro.enabled = false;
            }
        }

        #endregion 활성화

        #region 텍스트 처리

        public void ResetText()
        {
            if (TextLocalizer != null)
            {
                TextLocalizer.ResetText();
            }
            else if (_textPro != null)
            {
                _textPro.ResetText();
            }
        }

        public void SetText(string content)
        {
            if (TextLocalizer != null)
            {
                TextLocalizer.SetText(content);
            }
            else if (_textPro != null)
            {
                _textPro.SetText(content);
            }
            else
            {
                return;
            }

            FontLocalizer?.RefreshTextRectSize();
        }

        public void SetStringKey(string stringKey)
        {
            TextLocalizer?.SetStringKey(stringKey);
            FontLocalizer?.RefreshTextRectSize();
        }

        public void ResetStringKey()
        {
            TextLocalizer?.ResetStringKey();
        }

        public void SetSpriteText(string spriteContent)
        {
            TextLocalizer?.SetSpriteText(spriteContent);
        }

        public void ResetSpriteText()
        {
            if (TextLocalizer != null)
            {
                TextLocalizer.ResetSpriteText();
            }
        }

        #endregion 텍스트 처리

        #region 폰트 크기 및 스타일

        public void SetDefaultFontSize(float fontSize)
        {
            FontLocalizer?.SetDefaultFontSize(fontSize);
        }

        public void SetAlignment(TextAlignmentOptions alignment)
        {
            if (_textPro != null)
            {
                _textPro.alignment = alignment;
            }
        }

        public void SetFontStyle(FontStyles fontStyle)
        {
            if (_textPro != null)
            {
                _textPro.fontStyle = fontStyle;
            }
        }

        public void SetSpriteAsset(TMP_SpriteAsset spriteAsset)
        {
            if (_textPro != null)
            {
                _textPro.spriteAsset = spriteAsset;
            }
        }

        public void SetTextColor(Color fontColor)
        {
            if (_textPro != null)
            {
                _textPro.SetTextColor(fontColor);
            }
        }

        #endregion 폰트 크기 및 스타일

        #region 페이드 효과

        public Tweener FadeOut(float targetAlpha, float duration, float delayTime)
        {
            if (_textPro != null)
            {
                return _textPro.FadeOut(targetAlpha, duration, delayTime);
            }
            return null;
        }

        #endregion 페이드 효과

        #region 언어변환 및 폰트 설정

        public void RefreshLanguage(LanguageNames languageName)
        {
            TextLocalizer?.RefreshLanguage(languageName);
            FontLocalizer?.RefreshLanguage(languageName);
        }

        public void Refresh(LanguageNames languageName)
        {
            RefreshLanguage(languageName);
        }

        #endregion 언어변환 및 폰트 설정

        #region 텍스트 색상

        public void ResetTextColor()
        {
            if (_textPro != null)
            {
                _textPro.color = DefaultTextColor;
            }
        }

        #endregion 텍스트 색상

        #region 크기 조정

        private UITextLocalizer TextLocalizer => _textLocalizer ??= GetComponent<UITextLocalizer>();

        private UITextFontLocalizer FontLocalizer => _fontLocalizer ??= GetComponent<UITextFontLocalizer>();

        #endregion 크기 조정

        #region 밑줄 처리

        public void SetUnderline(bool isActive)
        {
            if (_textPro == null)
            {
                return;
            }

            if (isActive)
            {
                _textPro.fontStyle |= TMPro.FontStyles.Underline;
            }
            else
            {
                _textPro.fontStyle &= ~TMPro.FontStyles.Underline;
            }
        }

        #endregion 밑줄 처리
    }
}