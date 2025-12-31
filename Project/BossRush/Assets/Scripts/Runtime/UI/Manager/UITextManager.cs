using System.Collections.Generic;
using TeamSuneat.Setting;
using UnityEngine;

namespace TeamSuneat.UserInterface
{
    public class UITextManager : XBehaviour
    {
        private List<IUITextLocalizable> _localizableTexts = new List<IUITextLocalizable>();

        protected override void RegisterGlobalEvent()
        {
            base.RegisterGlobalEvent();
            GlobalEvent.Register(GlobalEventType.GAME_LANGUAGE_CHANGED, OnLanguageChanged);
        }

        protected override void UnregisterGlobalEvent()
        {
            base.UnregisterGlobalEvent();
            GlobalEvent.Unregister(GlobalEventType.GAME_LANGUAGE_CHANGED, OnLanguageChanged);
        }

        private void OnLanguageChanged()
        {
            LanguageNames language = GameSetting.Instance.Language.Name;
            RefreshAllTexts(language);
        }

        public void Register(IUITextLocalizable localizable)
        {
            if (localizable != null && !_localizableTexts.Contains(localizable))
            {
                _localizableTexts.Add(localizable);
            }
        }

        public void Unregister(IUITextLocalizable localizable)
        {
            if (localizable != null)
            {
                _localizableTexts.Remove(localizable);
            }
        }

        private void RefreshAllTexts(LanguageNames languageName)
        {
            for (int i = _localizableTexts.Count - 1; i >= 0; i--)
            {
                if (_localizableTexts[i] == null)
                {
                    _localizableTexts.RemoveAt(i);
                    continue;
                }

                _localizableTexts[i].RefreshLanguage(languageName);
            }
        }

        public void Clear()
        {
            _localizableTexts.Clear();
        }
    }
}

