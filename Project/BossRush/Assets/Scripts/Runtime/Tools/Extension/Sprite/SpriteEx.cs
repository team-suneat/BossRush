using Rewired;
using System.Text;
using UnityEngine;

namespace TeamSuneat
{
    public static partial class SpriteEx
    {
        // 아이콘 스트링 포맷
        private const string CHARACTER_ICON_FORMAT = "ui_character_icon_";
        private const string ITEM_ICON_FORMAT = "ui_item_icon_";
        private const string CURRENCY_ICON_FORMAT = "ui_currency_icon_";

        // 아틀라스 이름
        private const string CHARACTER_ATLAS_NAME = "atlas_character";
        private const string ITEM_ATLAS_NAME = "atlas_item";
        private const string INPUT_ATLAS_NAME = "atlas_input";

        // 공통 StringBuilder 인스턴스
        private static readonly StringBuilder _stringBuilder = new();

        public static string GetSpriteName(this CharacterNames key)
        {
            _ = _stringBuilder.Clear();
            _ = _stringBuilder.Append(CHARACTER_ICON_FORMAT);
            _ = _stringBuilder.Append(key.ToLowerString());

            return _stringBuilder.ToString();
        }

        public static string GetSpriteName(this ItemNames key)
        {
            _ = _stringBuilder.Clear();
            _ = _stringBuilder.Append(ITEM_ICON_FORMAT);
            _ = _stringBuilder.Append(key.ToLowerString());

            return _stringBuilder.ToString();
        }

        public static string GetSpriteName(this CurrencyNames key)
        {
            _ = _stringBuilder.Clear();
            _ = _stringBuilder.Append(CURRENCY_ICON_FORMAT);
            _ = _stringBuilder.Append(key.ToLowerString());

            return _stringBuilder.ToString();
        }

        //

        private const string MOUSE_PREFIX = "ui_input_mouse_";
        private const string KEYBOARD_PREFIX = "ui_input_keyboard_";
        private const string JOYSTICK_PREFIX = "ui_input_joystick_";
        private const string NINTENDO_PREFIX = "ui_input_nintendo_";

        public static string GetInputSpriteName(ControllerType controllerType, string keyName)
        {
            if (string.IsNullOrEmpty(keyName))
            {
                return string.Empty;
            }

            // 키 이름 전처리
            keyName = ConvertKeyName(keyName);
            keyName = CleanModifierKeyName(keyName);
            keyName = keyName.Replace(" ", "");

            // 접두사 결정
            string prefix = GetSpritePrefix(controllerType, keyName);
            if (string.IsNullOrEmpty(prefix))
            {
                return string.Empty;
            }
#if UNITY_SWITCH
            keyName = keyName.ToLower();
            if (keyName == "rightstick")
            {
                keyName = "r3";
            }
#endif
            return $"{prefix}{keyName}".ToLowerString();
        }

        private static string ConvertKeyName(string keyName)
        {
            // 키패드를 알파로 변환 (키보드 스프라이트 재사용)
            if (keyName.StartsWith("Keypad", System.StringComparison.OrdinalIgnoreCase))
            {
                string number = keyName.Substring(6); // "Keypad" 제거
                keyName = "alpha" + number;
            }

            keyName = ModifyKeyNameForConsoles(keyName);

            if (TSInputManager.Instance.CurrentJoystickType is JoystickTypes.PlayStation5)
            {
                if (keyName.Contains("Options", System.StringComparison.InvariantCultureIgnoreCase))
                {
                    keyName += "2";
                }
                if (keyName.Contains("Touch", System.StringComparison.InvariantCultureIgnoreCase))
                {
                    keyName += "2";
                }
            }

            return keyName;
        }

        private static string ModifyKeyNameForConsoles(string keyName)
        {
#if UNITY_GAMECORE
            keyName = keyName.Replace("Bumper", "shoulder");
            keyName = keyName.Replace("Menu Button", "start");
            keyName = keyName.Replace("View Button", "back");

            if (keyName.Contains("Left Stick", System.StringComparison.InvariantCultureIgnoreCase))
            {
                //NOTE: LeftStickButton + L3 are swapped in assets - change if fixed
                keyName = "L3";
            }
            else if (keyName.Contains("Right Stick", System.StringComparison.InvariantCultureIgnoreCase))
            {
                //NOTE: RightStickButton + R3 are swapped in assets - change if fixed
                keyName = "R3";
            }
#endif
#if UNITY_PS5
            if (!keyName.Contains("touch", System.StringComparison.InvariantCultureIgnoreCase))
            {
                keyName = keyName.Replace("button", string.Empty);
                keyName = keyName.Trim();
            }
            else
            {
                keyName = "touchpadbutton";
            }

            if (keyName.Contains("L3", System.StringComparison.InvariantCultureIgnoreCase))
            {
                //NOTE: LeftStickButton + L3 are swapped in assets - change if fixed
                keyName = "LeftStickButton";
            }
            else if (keyName.Contains("R3", System.StringComparison.InvariantCultureIgnoreCase))
            {
                //NOTE: RightStickButton + R3 are swapped in assets - change if fixed
                keyName = "RightStickButton";
            }

            switch (keyName)
            {
                case "up":
                case "down":
                case "left":
                case "right":
                    keyName = "d-pad" + keyName + "_ps";
                    break;
            }
#endif
#if UNITY_SWITCH
            if (!keyName.Contains("LeftStick", System.StringComparison.InvariantCultureIgnoreCase) &&
                !keyName.Contains("RightStick", System.StringComparison.InvariantCultureIgnoreCase))
            {
                keyName = keyName.Replace("Button", string.Empty);
                keyName = keyName.Replace("+Control Pad", string.Empty); //Pro controller
                keyName = keyName.Trim();
            }
            else if(keyName.Contains("LeftStick", System.StringComparison.InvariantCultureIgnoreCase))
            {
                //NOTE: LeftStickButton + L3 are swapped in assets - change if fixed
                keyName = "L3";
            }
            else if (keyName.Contains("RightStick", System.StringComparison.InvariantCultureIgnoreCase))
            {
                //NOTE: RightStickButton + R3 are swapped in assets - change if fixed
                keyName = "R3";
            }

            switch (keyName)
            {
                case "Up":
                case "Down":
                case "Left":
                case "Right":
                    keyName = "d-pad" + keyName.ToLower() + "_nx";
                    break;

                case "+":
                    keyName = "plus";
                    break;

                case "-":
                    keyName = "minus";
                    break;
            }
#endif
            return keyName;
        }

        private static string CleanModifierKeyName(string keyName)
        {
            if (keyName.Contains("Control") || keyName.Contains("Alt") || keyName.Contains("Shift"))
            {
                keyName = keyName.Replace("Left", string.Empty);
                keyName = keyName.Replace("Right", string.Empty);
            }
            return keyName;
        }

        private static string GetSpritePrefix(ControllerType controllerType, string keyName)
        {
            switch (controllerType)
            {
                case ControllerType.Mouse:
                    {
                        return MOUSE_PREFIX;
                    }

                case ControllerType.Keyboard:
                    {
                        return KEYBOARD_PREFIX;
                    }

                case ControllerType.Joystick:
                    {
                        if (IsNintendoSpecialButton(keyName))
                        {
                            return NINTENDO_PREFIX;
                        }
                        return JOYSTICK_PREFIX;
                    }

                default:
                    {
                        return string.Empty;
                    }
            }
        }

        private static bool IsNintendoSpecialButton(string keyName)
        {
            return TSInputManager.Instance.CurrentJoystickType == JoystickTypes.Nintendo &&
                   (keyName == "A" || keyName == "B" || keyName == "X" || keyName == "Y");
        }

        public static string GetInputMouseSpriteName(string keyName)
        {
            if (string.IsNullOrEmpty(keyName))
            {
                return string.Empty;
            }

            // 마우스 키 매핑
            if (keyName.Contains("Equip") || keyName.Contains("Use") || keyName.Contains("ResetSkill"))
            {
                keyName = "mouse1";
            }
            else if (keyName.Contains("Discard") || keyName.Contains("Register") || keyName.Contains("UISubmitClick"))
            {
                keyName = "mouse0";
            }

            keyName = keyName.Replace(" ", "");
            return $"{MOUSE_PREFIX}{keyName}".ToLowerString();
        }

        public static string GetInputStickSpriteName(ActionNames actionName)
        {
            switch (actionName)
            {
                case ActionNames.MoveHorizontal:
                case ActionNames.MoveVertical:
                    return "ui_joystick_leftstick";

                // return "ui_joystick_rightstick";

                case ActionNames.UIMoveUp:
                case ActionNames.UIMoveDown:
                case ActionNames.UIMoveLeft:
                case ActionNames.UIMoveRight:
                    return "ui_joystick_d-pad";
            }

            return string.Empty;
        }

        //

        public static Sprite LoadSprite(this CharacterNames characterName)
        {
            if (characterName == CharacterNames.None)
            {
                return null;
            }

            string spriteName = GetSpriteName(characterName);
            if (string.IsNullOrEmpty(spriteName))
            {
                return null;
            }

            return ResourcesManager.LoadSprite(spriteName, CHARACTER_ATLAS_NAME);
        }

        public static Sprite LoadSprite(this ItemNames itemName)
        {
            if (itemName == ItemNames.None)
            {
                return null;
            }

            string spriteName = GetSpriteName(itemName);
            if (string.IsNullOrEmpty(spriteName))
            {
                return null;
            }

            return ResourcesManager.LoadSprite(spriteName, ITEM_ATLAS_NAME);
        }

        public static Sprite LoadSprite(this CurrencyNames currencyName)
        {
            if (currencyName == CurrencyNames.None)
            {
                return null;
            }

            string spriteName = GetSpriteName(currencyName);
            if (string.IsNullOrEmpty(spriteName))
            {
                return null;
            }

            return ResourcesManager.LoadSprite(spriteName, ITEM_ATLAS_NAME);
        }

        public static Sprite LoadSprite(this ControllerType controllerType, string keyName)
        {
            string spriteName = GetInputSpriteName(controllerType, keyName);
            Sprite sprite = ResourcesManager.LoadSprite(spriteName, INPUT_ATLAS_NAME);
            if (sprite != null)
            {
                return sprite;
            }

            return null;
        }

        public static Sprite LoadMouseSprite(this ActionNames actionName)
        {
            string key = actionName.ToString();
            string spriteName = GetInputMouseSpriteName(key);
            Sprite sprite = ResourcesManager.LoadSprite(spriteName, INPUT_ATLAS_NAME);
            if (sprite != null)
            {
                return sprite;
            }

            return null;
        }
        public static Sprite LoadStickSprite(this ActionNames actionName)
        {
            string spriteName = GetInputStickSpriteName(actionName);
            Sprite sprite = ResourcesManager.LoadSprite(spriteName, INPUT_ATLAS_NAME);
            if (sprite != null)
            {
                return sprite;
            }

            return null;
        }
    }
}