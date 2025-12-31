using TeamSuneat.Data;
using TeamSuneat.Setting;
using TMPro;
using UnityEngine;

namespace TeamSuneat.UserInterface
{
    [DisallowMultipleComponent]
    public class UITextLocalizer : XBehaviour, IUITextLocalizable
    {
        [SerializeField] private TextMeshProUGUI _textPro;
        public string StringKey;

        private string _content;
        private string _spriteContent;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();
            _textPro ??= GetComponent<TextMeshProUGUI>();
        }

        public override void AutoSetting()
        {
            base.AutoSetting();

            if (!JsonDataManager.CheckLoaded())
            {
                JsonDataManager.LoadJsonSheetsSync();
            }

            RefreshContent(GameSetting.Instance.Language.Name);
        }

        private void Awake()
        {
            RegisterToManager();
        }

        protected override void OnRelease()
        {
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
            RefreshContent(languageName);
        }

        public void SetText(string content)
        {
            if (_textPro == null)
            {
                return;
            }

            if (_content == content)
            {
                return;
            }

            _content = content;
            OnSetText();
        }

        public void SetStringKey(string stringKey)
        {
            if (StringKey == stringKey)
            {
                return;
            }

            StringKey = stringKey;
            RefreshContent(GameSetting.Instance.Language.Name);
        }

        public void ResetStringKey()
        {
            if (string.IsNullOrEmpty(StringKey))
            {
                return;
            }

            StringKey = string.Empty;
            RefreshContent(GameSetting.Instance.Language.Name);
        }

        public void ResetText()
        {
            if (_textPro == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(_content))
            {
                _content = string.Empty;
            }
            OnSetText();
        }

        public void SetSpriteText(string spriteContent)
        {
            if (_textPro == null)
            {
                return;
            }

            if (_spriteContent == spriteContent)
            {
                return;
            }

            _spriteContent = spriteContent;
            OnSetText();
        }

        public void ResetSpriteText()
        {
            if (_textPro == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(_spriteContent))
            {
                return;
            }

            _spriteContent = string.Empty;
            OnSetText();
        }

        private void RefreshContent(LanguageNames languageName)
        {
            if (string.IsNullOrEmpty(StringKey))
            {
                return;
            }

            string content = JsonDataManager.FindStringClone(StringKey, languageName);
            SetText(content);
        }

        private void OnSetText()
        {
            if (_textPro == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(_spriteContent) && !string.IsNullOrEmpty(_content))
            {
                _textPro.SetText($"{_spriteContent} {_content}");
            }
            else if (!string.IsNullOrEmpty(_spriteContent))
            {
                _textPro.SetText(_spriteContent);
            }
            else if (!string.IsNullOrEmpty(_content))
            {
                _textPro.SetText(_content);
            }
            else
            {
                _textPro.ResetText();
            }

            RefreshTextRectSize();
        }

        private void RefreshTextRectSize()
        {
            UITextFontLocalizer fontLocalizer = GetComponent<UITextFontLocalizer>();
            if (fontLocalizer != null)
            {
                fontLocalizer.RefreshTextRectSize();
            }
        }
    }
}