using TeamSuneat.Setting;
using TeamSuneat.UserInterface;
using UnityEngine;

namespace TeamSuneat
{
    public class GameManager : MonoBehaviour
    {
        private void Awake()
        {
            CharacterManager.Instance.Reset();
            TSInputManager.Instance.Initialize();
        }

        private void OnEnable()
        {
            if (TSInputManager.Instance.IsInitialized)
            {
                TSInputManager.Instance.SubscribeEvents();
            }
        }

        private void OnDisable()
        {
            TSInputManager.Instance.UnsubscribeEvents();
        }

        private void OnDestroy()
        {
            // 안전장치
            TSInputManager.Instance.UnsubscribeEvents();
            TSInputManager.Instance.IsInitialized = false;
        }

        private void Update()
        {
            TSInputManager.Instance.GetInputState();
            CharacterManager.Instance.LogicUpdate();
            UIManager.Instance?.LogicUpdate();
            GameTimeManager.Instance.LogicUpdate();
        }

        private void LateUpdate()
        {
            CharacterManager.Instance.LateLogicUpdate();
            TSInputManager.Instance.ProcessButtonStates();
            GameSetting.Instance.Video.RefreshResolutionRate();
        }

        private void FixedUpdate()
        {
            CharacterManager.Instance.PhysicsUpdate();
        }

        private void OnApplicationFocus(bool focus)
        {
            if (!focus)
            {
                TSInputManager.Instance.ResetButtonStates();
            }
        }

        private void OnApplicationPause(bool pause)
        {
        }

        internal void ResetStage()
        {
        }
    }
}