using System.Collections;
using TeamSuneat.Stage;
using TeamSuneat.UserInterface;
using UnityEngine;

namespace TeamSuneat
{
    public class GameManager : MonoBehaviour
    {
        public bool IsBattleActive { get; internal set; }
        public StageSystem CurrentStageSystem { get; set; }

        private void Awake()
        {
            CharacterManager.Instance.Reset();
        }

        private IEnumerator Start()
        {
            yield return new WaitUntil(() => { return GameApp.Instance != null; });
        }

        private void OnEnable()
        {
        }

        private void OnDisable()
        {
        }

        private void OnDestroy()
        {
        }

        private void Update()
        {
            CharacterManager.Instance.LogicUpdate();
            UIManager.Instance?.LogicUpdate();
        }

        private void LateUpdate()
        {
            // PassiveManager.Instance.LateLogicUpdate();
        }

        private void FixedUpdate()
        {
            CharacterManager.Instance.PhysicsUpdate();
        }

        private void OnApplicationFocus(bool focus)
        {
            if (!focus)
            {
            }
        }

        private void OnApplicationPause(bool pause)
        {
            // 방치형 게임에서는 앱이 백그라운드로 가도 게임이 계속 진행되어야 함
            // 오프라인 시간은 OfflineTimeManager에서 처리
        }

        internal void ResetStage()
        {
        }
    }
}