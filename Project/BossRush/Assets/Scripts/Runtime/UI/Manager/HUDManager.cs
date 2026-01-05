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

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _hudCanvasGroupFader ??= GetComponentInChildren<UICanvasGroupFader>();
            _normalStageGroup ??= this.FindGameObject("2. Center Group/Normal Stage Group");
        }

        private void Awake()
        {
        }

        public void OnBossDied()
        {
        }
    }
}