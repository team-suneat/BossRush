using Sirenix.OdinInspector;
using TeamSuneat.Setting;
using UnityEngine;

namespace TeamSuneat.UserInterface
{
    public partial class UIManager : XStaticBehaviour<UIManager>
    {
        [FoldoutGroup("Manager")] public UICanvasManager CanvasManager;
        [FoldoutGroup("Manager")] public UIGaugeManager GaugeManager;
        [FoldoutGroup("Manager")] public UITextManager TextManager;
        [FoldoutGroup("Manager")] public HUDManager HUDManager;
        [FoldoutGroup("Manager")] public UIPopupManager PopupManager;
        [FoldoutGroup("Manager")] public UIDetailsManager DetailsManager;
        [FoldoutGroup("Manager")] public UINoticeManager NoticeManager;
        public UIScreenFader ScreenFader;

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
        }

        public void Clear()
        {
            PopupManager?.ResetValues();
            GaugeManager?.Clear();
            NoticeManager?.Clear();
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
            HUDManager?.LogicUpdate();
            NoticeManager?.LogicUpdate();
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