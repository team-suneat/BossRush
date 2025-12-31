using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace TeamSuneat.Data
{
    [CreateAssetMenu(fileName = "Font", menuName = "TeamSuneat/Scriptable/Font")]
    public class FontAsset : XScriptableObject
    {
        [Title("Font")]
        public LanguageNames Language;

        [Title("Font-Data")]
        public FontAssetData Title;
        public FontAssetData Content;
        public FontAssetData Button;
        public FontAssetData Toggle;

        public int TID => BitConvert.Enum32ToInt(Language);

        public override void Refresh()
        {
            NameString = Language.ToString();
            base.Refresh();
        }

        public override void Validate()
        {
            base.Validate();
            EnumEx.ConvertTo(ref Language, NameString);
        }

        public TMP_FontAsset FindFont(GameFontTypes fontType)
        {
            switch (fontType)
            {
                case GameFontTypes.Title:
                    {
                        return Title.Font;
                    }
                case GameFontTypes.Content:
                    {
                        return Content.Font;
                    }
                case GameFontTypes.Button:
                    {
                        return Button.Font;
                    }
                case GameFontTypes.Toggle:
                    {
                        return Toggle.Font;
                    }
            }

            return null;
        }

        public FontAssetData? GetFontAssetData(GameFontTypes type)
        {
            return type switch
            {
                GameFontTypes.Title => (FontAssetData?)Title,
                GameFontTypes.Content => (FontAssetData?)Content,
                GameFontTypes.Button => (FontAssetData?)Button,
                GameFontTypes.Toggle => (FontAssetData?)Toggle,
                _ => null,
            };
        }

#if UNITY_EDITOR

        public override void Rename()
        {
            Rename("Font");
        }

#endif
    }
}