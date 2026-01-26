using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TeamSuneat
{
    public class AnalyticsInitializer : MonoBehaviour
    {
        private async void Start()
        {
#if DISABLE_ANALYTICS
            return;
#endif
            if (GameDefine.IS_EDITOR_OR_DEVELOPMENT_BUILD) return;
            if (UnityServices.State != ServicesInitializationState.Initialized)
            {
                try
                {
                    await UnityServices.InitializeAsync();
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"[Analytics] 초기화 실패: {ex.Message}");
                    return;
                }
            }

            // 이 부분은 초기화 여부와 상관없이 체크하는 게 안전
            if (AnalyticsService.Instance != null)
            {
                AnalyticsService.Instance.StartDataCollection();
            }
            else
            {
                Debug.LogWarning("[Analytics] AnalyticsService.Instance가 null입니다. 데이터 수집을 시작하지 못했습니다.");
            }
        }

        private void OnEnable()
        {
#if DISABLE_ANALYTICS
            return;
#endif
            Application.logMessageReceived += HandleLog;
        }

        private void OnDisable()
        {
#if DISABLE_ANALYTICS
            return;
#endif
            Application.logMessageReceived -= HandleLog;
        }

        private void HandleLog(string logString, string stackTrace, LogType type)
        {
#if DISABLE_ANALYTICS
            return;
#endif
            if (GameDefine.IS_EDITOR_OR_DEVELOPMENT_BUILD) return;
            if (type is LogType.Exception or LogType.Error)
            {
                // 스택 트레이스 잘라내기 (최대 1000자)
                string trimmedStackTrace = !string.IsNullOrEmpty(stackTrace)
                    ? stackTrace.Substring(0, Mathf.Min(stackTrace.Length, 1000))
                    : "";

                CustomEvent customEvent = new("CriticalClientError")
                {
                    { "message", logString },
                    { "stackTrace", trimmedStackTrace },
                    { "scene", SceneManager.GetActiveScene().name },
                    { "clientTimestamp", System.DateTime.UtcNow.ToString("o") },
                    { "appVersion", Application.version },
                    { "platform", GetPlatformForAnalytics() }
                };

                AnalyticsService.Instance.RecordEvent(customEvent);
            }
        }

        private string GetPlatformForAnalytics()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsPlayer:
                    return "PC_CLIENT";

                case RuntimePlatform.OSXPlayer:
                    return "MAC";

                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.OSXEditor:
                    return "EDITOR";

                case RuntimePlatform.IPhonePlayer:
                    return "IOS";

                case RuntimePlatform.Android:
                    return "ANDROID";

                case RuntimePlatform.PS4:
                case RuntimePlatform.PS5:
                    return "PLAYSTATION";

                case RuntimePlatform.XboxOne:
                    return "XBOX";

                case RuntimePlatform.Switch:
                    return "SWITCH";

                case RuntimePlatform.WebGLPlayer:
                    return "WEB";

                default:
                    return "UNKNOWN";
            }
        }
    }
}