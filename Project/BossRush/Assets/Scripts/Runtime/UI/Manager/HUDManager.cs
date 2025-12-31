using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat.UserInterface
{
    public class HUDManager : XBehaviour
    {
        [FoldoutGroup("HUD-Normal")]
        [SerializeField] private GameObject _normalStageGroup;

        [FoldoutGroup("HUD-Normal")]
        [SerializeField] private UICanvasGroupFader _hudCanvasGroupFader;

        [FoldoutGroup("HUD-Normal")]
        [SerializeField] private HUDStageProgressGauge _stageProgressGauge;

        [FoldoutGroup("HUD-Normal")]
        [SerializeField]
        private HUDStage _normalStageGroupStage;

        //

        [FoldoutGroup("HUD-Boss")]
        [SerializeField] private GameObject _bossStageGroup;

        [FoldoutGroup("HUD-Boss")]
        [SerializeField] private HUDBossTimerGauge _bossTimerGauge;

        [FoldoutGroup("HUD-Boss")]
        [SerializeField] private HUDBossHealthGauge _bossHealthGauge;

        [FoldoutGroup("HUD-Boss")]
        [SerializeField]
        private HUDStage _bossStageGroupStage;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _hudCanvasGroupFader ??= GetComponentInChildren<UICanvasGroupFader>();
            _stageProgressGauge ??= GetComponentInChildren<HUDStageProgressGauge>();
            _bossTimerGauge ??= GetComponentInChildren<HUDBossTimerGauge>();
            _bossHealthGauge ??= GetComponentInChildren<HUDBossHealthGauge>();

            _normalStageGroup ??= this.FindGameObject("2. Center Group/Normal Stage Group");
            _bossStageGroup ??= this.FindGameObject("2. Center Group/Boss Stage Group");

            _normalStageGroupStage ??= _normalStageGroup?.GetComponentInChildren<HUDStage>();
            _bossStageGroupStage ??= _bossStageGroup?.GetComponentInChildren<HUDStage>();
        }

        private void Awake()
        {
            // 초기 상태: 일반 스테이지 그룹 활성화, 보스 스테이지 그룹 비활성화
            SwitchStageGroup(false);
        }

        protected override void RegisterGlobalEvent()
        {
            base.RegisterGlobalEvent();
            GlobalEvent.Register(GlobalEventType.BOSS_MODE_ENTERED, OnBossModeEntered);
            GlobalEvent.Register(GlobalEventType.BOSS_MODE_EXITED, OnBossModeExited);
            GlobalEvent.Register(GlobalEventType.GAME_DATA_STAGE_SET, OnStageChanged);
        }

        protected override void UnregisterGlobalEvent()
        {
            base.UnregisterGlobalEvent();
            GlobalEvent.Unregister(GlobalEventType.BOSS_MODE_ENTERED, OnBossModeEntered);
            GlobalEvent.Unregister(GlobalEventType.BOSS_MODE_EXITED, OnBossModeExited);
            GlobalEvent.Unregister(GlobalEventType.GAME_DATA_STAGE_SET, OnStageChanged);
        }

        private void SwitchStageGroup(bool isBossMode)
        {
            if (_normalStageGroup != null)
            {
                _normalStageGroup.SetActive(!isBossMode);
            }
            if (_bossStageGroup != null)
            {
                _bossStageGroup.SetActive(isBossMode);
            }
        }

        private void OnStageChanged()
        {
            // 활성화된 그룹의 HUDStage만 갱신
            if (_normalStageGroup != null && _normalStageGroup.activeSelf)
            {
                _normalStageGroupStage?.Refresh();
            }
            else if (_bossStageGroup != null && _bossStageGroup.activeSelf)
            {
                _bossStageGroupStage?.Refresh();
            }
        }

        public void OnBossModeEntered()
        {
            // 그룹 전환
            SwitchStageGroup(true);

            // 타이머 게이지와 체력 게이지를 같은 시점에 활성화
            _bossTimerGauge?.OnBossModeEntered();
            _bossHealthGauge?.OnBossModeEntered();

            Log.Info(LogTags.UI, "[HUDManager] 보스 모드 진입. 일반 스테이지 그룹 비활성화, 보스 스테이지 그룹 활성화");
        }

        public void OnBossModeExited()
        {
            // 보스 타이머 게이지 비활성화
            _bossTimerGauge?.OnBossModeExited();

            // 보스 체력 게이지 정리
            _bossHealthGauge?.OnBossModeExited();

            // 그룹 전환
            SwitchStageGroup(false);

            Log.Info(LogTags.UI, "[HUDManager] 보스 모드 종료. 보스 스테이지 그룹 비활성화, 일반 스테이지 그룹 활성화");
        }

        public void SetStageProgress(int currentWave, int totalWave)
        {
            _stageProgressGauge?.SetProgress(currentWave, totalWave);
        }

        public void OnBossSpawned(Character bossCharacter)
        {
            _bossHealthGauge?.OnBossSpawned(bossCharacter);
        }

        public void OnBossDied()
        {
            _bossHealthGauge?.OnBossDied();
        }

        public void LogicUpdate()
        {
            _bossTimerGauge?.LogicUpdate();
            _bossHealthGauge?.LogicUpdate();
        }
    }
}