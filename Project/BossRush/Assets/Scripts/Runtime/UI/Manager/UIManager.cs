using Sirenix.OdinInspector;
using TeamSuneat.Setting;
using UnityEngine;

namespace TeamSuneat.UserInterface
{
    public partial class UIManager : XStaticBehaviour<UIManager>
    {
        [FoldoutGroup("Manager-Canvas")] public UICanvasManager CanvasManager;
        [FoldoutGroup("Manager-Canvas")] public HUDManager HUDManager;
        [FoldoutGroup("Manager-Canvas")] public UIPopupManager PopupManager;
        [FoldoutGroup("Manager-Canvas")] public UIDetailsManager DetailsManager;
        [FoldoutGroup("Manager-Canvas")] public UINoticeManager NoticeManager;

        //

        [FoldoutGroup("Manager-Runtime")] public UIGaugeManager GaugeManager;
        [FoldoutGroup("Manager-Runtime")] public UITextManager TextManager;

        //

        [FoldoutGroup("Manager-Controller")] public UISelectController SelectController;

        //

        [FoldoutGroup("Manager-Effect")] public UIScreenFader ScreenFader;
        [FoldoutGroup("Manager-Effect")] public UICinematicBar CinematicBar;

        public Vector3 WorldPositionMin { get; set; }
        public Vector3 WorldPositionMax { get; set; }

        //──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────

        public const float WAIT_INPUT_TIME = 0.2f;

        //──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            CanvasManager = GetComponent<UICanvasManager>();
            PopupManager = GetComponentInChildren<UIPopupManager>();
            HUDManager = GetComponentInChildren<HUDManager>();
            GaugeManager = GetComponentInChildren<UIGaugeManager>();
            DetailsManager = GetComponentInChildren<UIDetailsManager>();
            NoticeManager = GetComponentInChildren<UINoticeManager>();
            TextManager = GetComponentInChildren<UITextManager>();
            ScreenFader = GetComponentInChildren<UIScreenFader>();
            CinematicBar = GetComponentInChildren<UICinematicBar>();
            SelectController = GetComponentInChildren<UISelectController>();
        }

        public void Clear()
        {
            PopupManager?.ResetValues();
            GaugeManager?.Clear();
            SelectController?.Clear();
        }

        public CanvasOrder GetCanvas(CanvasOrderNames canvasOrderName)
        {
            return CanvasManager.Get(canvasOrderName);
        }

        public void LogicUpdate()
        {
            if (GameSetting.Instance.Input.IsBlockUIInput)
            {
                return;
            }

            PopupManager?.LogicUpdate();
            NoticeManager?.LogicUpdate();
            SelectController?.LogicUpdate();
        }

        internal void SpawnSoliloquyNotice(SoliloquyTypes content)
        {
        }

        internal void SpawnSoliloquyNotice(string content)
        {
        }

        internal void SpawnSoliloquyIngame(SoliloquyTypes canNotUsedYet)
        {
        }

        internal void SpawnSoliloquyIngame(SoliloquyTypes unstackEffect, string content)
        {
        }

        internal void SpawnNoticeMessage(string nameContent, string descContent)
        {
        }
    }
}