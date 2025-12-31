using Sirenix.OdinInspector;
using TeamSuneat.Data;
using TeamSuneat.Setting;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    [DisallowMultipleComponent]
    public class UITextFontLocalizer : XBehaviour, IUITextLocalizable
    {
        [SerializeField] private TextMeshProUGUI _textPro;
        public GameFontTypes FontType;
        public string FontTypeString;

        [Title("#UI Text Size")]
        [SerializeField]
        private bool _sizeToTextLengthX;

        [SerializeField]
        private bool _sizeToTextLengthY;

        [SerializeField]
        private LayoutElement _layoutElement;

        private float _defaultFontSize;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();
            _textPro ??= GetComponent<TextMeshProUGUI>();
            _layoutElement ??= GetComponent<LayoutElement>();
        }

        public override void AutoSetting()
        {
            base.AutoSetting();

            if (FontType != GameFontTypes.None)
            {
                FontTypeString = FontType.ToString();
            }

            LoadData();

            RefreshFont(GameSetting.Instance.Language.Name);
        }

        private void LoadData()
        {
            if (!PathManager.CheckLoaded())
            {
                PathManager.LoadAllSync();
            }

            if (!ScriptableDataManager.Instance.CheckLoadedSync())
            {
                ScriptableDataManager.Instance.LoadScriptableAssetsSyncByLabel(AddressableLabels.ScriptableSync);
            }
        }

        private void OnValidate()
        {
            if (!EnumEx.ConvertTo(ref FontType, FontTypeString))
            {
                Log.Error(LogTags.Font, "{0} 폰트 타입이 변환되지 않습니다. {1}", FontType, FontTypeString);
            }

            ValidateLayoutComponents();
        }

        private void ValidateLayoutComponents()
        {
            if (!_sizeToTextLengthX && !_sizeToTextLengthY)
            {
                return;
            }

            bool hasLayoutElement = _layoutElement != null || TryGetComponent(out LayoutElement assignedLayoutElement);
            if (!hasLayoutElement)
            {
                Log.Warning($"[UITextFontLocalizer] LayoutElement가 없습니다. ({this.GetHierarchyName()})");
            }
        }

        private void Awake()
        {
            RegisterToManager();

            if (_textPro != null)
            {
                _defaultFontSize = _textPro.fontSize;
            }
        }

        protected override void OnRelease()
        {
            base.OnRelease();

            UnregisterFromManager();
        }

        private void RegisterToManager()
        {
            if (UIManager.Instance != null && UIManager.Instance.TextManager != null)
            {
                UIManager.Instance.TextManager.Register(this);
            }
        }

        private void UnregisterFromManager()
        {
            if (UIManager.Instance != null && UIManager.Instance.TextManager != null)
            {
                UIManager.Instance.TextManager.Unregister(this);
            }
        }

        public void RefreshLanguage(LanguageNames languageName)
        {
            RefreshFont(languageName);
        }

        private void RefreshFont(LanguageNames languageName)
        {
            if (_textPro == null)
            {
                return;
            }

            if (FontType == GameFontTypes.None)
            {
                return;
            }

            FontAsset fontData = ScriptableDataManager.Instance.FindFont(languageName);
            if (fontData == null)
            {
                return;
            }

            TMP_FontAsset fontAsset = fontData.FindFont(FontType);
            if (fontAsset == null)
            {
                return;
            }

            _textPro.font = fontAsset;
            RefreshFontSize(languageName, fontData);
            RefreshTextRectSize();
        }

        //

        public void InitializeDefaultFontSize(float fontSize)
        {
            _defaultFontSize = fontSize;
        }

        public void SetDefaultFontSize(float fontSize)
        {
            _defaultFontSize = fontSize;
            RefreshFontSize();
        }

        //

        public void RefreshFontSize()
        {
            LanguageNames languageName = GameSetting.Instance.Language.Name;
            FontAsset fontData = ScriptableDataManager.Instance.FindFont(languageName);
            RefreshFontSize(languageName, fontData);
        }

        private void RefreshFontSize(LanguageNames languageName, FontAsset fontData)
        {
            if (_textPro == null)
            {
                return;
            }

            if (fontData != null)
            {
                FontAssetData? fontAssetData = fontData.GetFontAssetData(FontType);
                if (fontAssetData != null)
                {
                    float fontSize = fontAssetData.Value.FontSize;

                    if (!Mathf.Approximately(_textPro.fontSize, fontSize))
                    {
                        _textPro.enableAutoSizing = false;
                        _textPro.fontSize = fontSize;

                        Log.Info(LogTags.Font, "폰트 타입({0})에 맞는 폰트 크기를 적용합니다: {1}, {2}",
                            FontType, fontSize, this.GetHierarchyPath());
                    }
                    return;
                }
            }

            if (_defaultFontSize > 0)
            {
                _textPro.fontSize = _defaultFontSize;
            }

            RefreshTextRectSize();
        }

        public void RefreshTextRectSize()
        {
            if (!_sizeToTextLengthX && !_sizeToTextLengthY)
            {
                return;
            }

            if (_layoutElement == null)
            {
                return;
            }

            if (_textPro == null || _textPro.font == null)
            {
                return;
            }

            if (_sizeToTextLengthX)
            {
                _layoutElement.preferredWidth = _textPro.preferredWidth;
            }

            if (_sizeToTextLengthY)
            {
                _layoutElement.preferredHeight = _textPro.preferredHeight;
            }
        }

        public void SetClosestFontTypeByCurrentFontSize(LanguageNames languageName)
        {
            if (_textPro == null)
            {
                return;
            }

            float currentFontSize = _textPro.fontSize;
            FontAsset fontAsset = ScriptableDataManager.Instance.FindFont(languageName);
            if (fontAsset == null)
            {
                return;
            }

            GameFontTypes closestType = GameFontTypes.None;
            float minDiff = float.MaxValue;

            GameFontTypes[] matchTypes = new GameFontTypes[]
            {
                GameFontTypes.Title,
                GameFontTypes.Content,
                GameFontTypes.Button,
                GameFontTypes.Toggle
            };

            for (int i = 0; i < matchTypes.Length; i++)
            {
                GameFontTypes type = matchTypes[i];
                FontAssetData? dataNullable = fontAsset.GetFontAssetData(type);
                if (dataNullable == null)
                {
                    continue;
                }

                FontAssetData data = dataNullable.Value;
                float compareSize = data.FontSize;
                float diff = Mathf.Abs(currentFontSize - compareSize);
                if (diff < minDiff)
                {
                    minDiff = diff;
                    closestType = type;
                }
            }

            if (closestType != GameFontTypes.None)
            {
                GameFontTypes prevType = FontType;

                FontType = closestType;
                FontTypeString = closestType.ToString();
                if (prevType != closestType)
                {
                    Log.Info(LogTags.Font, $"[UITextFontLocalizer] FontType이 자동으로 변경: {prevType} → {closestType} (현재 폰트 크기: {currentFontSize})");
                }
            }
        }
    }
}