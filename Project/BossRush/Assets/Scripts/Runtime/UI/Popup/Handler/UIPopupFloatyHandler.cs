using UnityEngine;

namespace TeamSuneat.UserInterface
{
    /// <summary>
    /// UIPopup의 Floaty 텍스트 처리를 담당하는 핸들러입니다.
    /// </summary>
    public class UIPopupFloatyHandler : MonoBehaviour
    {
        private UIFloatyText _floatyText;

        public void Initialize()
        {
            _floatyText = null;
        }

        public void Cleanup()
        {
            DespawnFloatyText();
        }

        public void SpawnFloatyText(string content, UIFloatyMoveNames moveType)
        {
            DespawnFloatyText();

            string value = Data.JsonDataManager.FindStringClone(content);
            _floatyText = ResourcesManager.SpawnUIFloatyText(value, moveType);

            if (_floatyText != null)
            {
                _floatyText.StartMove();
                Log.Info(LogTags.UI_Popup, $"Floaty 텍스트를 생성했습니다: {content}");
            }
            else
            {
                Log.Warning(LogTags.UI_Popup, $"Floaty 텍스트 생성에 실패했습니다: {content}");
            }
        }

        public void SpawnFloatyGetStringText(string content, UIFloatyMoveNames moveType)
        {
            DespawnFloatyText();

            _floatyText = ResourcesManager.SpawnUIFloatyText(content, moveType);

            if (_floatyText != null)
            {
                _floatyText.StartMove();
                Log.Info(LogTags.UI_Popup, $"Floaty 텍스트를 생성했습니다 (직접 문자열): {content}");
            }
            else
            {
                Log.Warning(LogTags.UI_Popup, $"Floaty 텍스트 생성에 실패했습니다 (직접 문자열): {content}");
            }
        }

        public void DespawnFloatyText()
        {
            if (_floatyText != null)
            {
                _floatyText.Despawn();
                _floatyText = null;
                Log.Info(LogTags.UI_Popup, "Floaty 텍스트를 제거했습니다.");
            }
        }
    }
}