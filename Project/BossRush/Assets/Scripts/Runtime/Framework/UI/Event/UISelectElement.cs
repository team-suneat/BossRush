using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace TeamSuneat.UserInterface
{
    public class UISelectElement : XBehaviour
    {
        public enum PadClickTypes
        {
            Right,
            Left,
        }

        [FoldoutGroup("#UISelectElement")]
        public PadClickTypes PadClickType;

        [FoldoutGroup("#UISelectElement")]
        public UIClickable Clickable;

        [FoldoutGroup("#UISelectElement")]
        public UISelectable Selectable;

        [FoldoutGroup("#UISelectElement-Select")]
        public bool LockTrigger;

        [FoldoutGroup("#UISelectElement-Select")]
        [DisableIf("LockTrigger", true)]
        public int SelectIndex;

        [FoldoutGroup("#UISelectElement-Select")]
        [DisableIf("LockTrigger", true)]
        public int SelectOrderIndex;

        [FoldoutGroup("#UISelectElement-Select")]
        [DisableIf("LockTrigger", true)]
        public int SelectLeftIndex;

        [FoldoutGroup("#UISelectElement-Select")]
        [DisableIf("LockTrigger", true)]
        public int SelectRightIndex;

        [FoldoutGroup("#UISelectElement-Select")]
        [DisableIf("LockTrigger", true)]
        public int SelectUpIndex;

        [FoldoutGroup("#UISelectElement-Select")]
        [DisableIf("LockTrigger", true)]
        public int SelectDownIndex;

        [FoldoutGroup("#UISelectElement-SelectFrame")]
        public Vector2 SelectFrameSizeDelta;

        [FoldoutGroup("#UISelectElement-SelectFrame")]
        public Vector3 SelectFrameOffset;

        [FoldoutGroup("#UISelectElement-MoveSpeed")]
        [Tooltip("커스텀 이동 대기 시간 (초). -1이면 기본값 사용, 양수면 해당 값 사용")]
        public float CustomMoveWaitTime = -1f;

        private UnityAction _enterEventAction;
        private UnityAction _exitEventAction;

        [FoldoutGroup("#UISelectElement-Event")]
        public UnityEvent OnPointerPressLeftEvent;

        [FoldoutGroup("#UISelectElement-Event")]
        public UnityEvent OnPointerClickLeftEvent;

        [FoldoutGroup("#UISelectElement-Event")]
        public UnityEvent OnPointerClickRightEvent;

        [FoldoutGroup("#UISelectElement-Event")]
        public UnityEvent OnPointerUpLeftEvent;

        private const int DEFAULT_SELECT_INDEX = 0;
        protected bool IsEnterPointer { get; private set; }

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            Clickable ??= GetComponentInChildren<UIClickable>();
            Selectable ??= GetComponentInChildren<UISelectable>();
        }

        public override void AutoSetting()
        {
            base.AutoSetting();

            if (Selectable == null && Clickable == null)
            {
                ClearSelectIndex();
            }
        }

        protected virtual void Awake()
        {
            RegisterClickableEvents();
            RegisterSelectableEvents();
        }

        private void RegisterClickableEvents()
        {
            if (Clickable == null) return;

            Clickable.RegisterPointerClickLeftEvent(_ => OnPointerClickLeft());
            Clickable.RegisterPointerClickRightEvent(_ => OnPointerClickRight());
            Clickable.RegisterPointerPressLeftEvent(_ => OnPointerPressLeft());
            Clickable.RegisterPointerUpLeftEvent(_ => OnPointerUpLeft());
        }

        private void RegisterSelectableEvents()
        {
            if (Selectable == null) return;

            Selectable.RegisterPointerEnterEvent(_ => OnPointerEnter());
            Selectable.RegisterPointerExitEvent(_ => OnPointerExit());
        }

        public void OnPointerClick()
        {
            if (PadClickType == PadClickTypes.Left)
                OnPointerClickLeft();
            else if (PadClickType == PadClickTypes.Right)
                OnPointerClickRight();
        }

        public virtual void OnPointerPressLeft()
        {
            OnPointerPressLeftEvent?.Invoke();
        }

        public virtual void OnPointerClickLeft()
        {
            OnPointerClickLeftEvent?.Invoke();
        }

        public virtual void OnPointerClickRight()
        {
            OnPointerClickRightEvent?.Invoke();
        }

        public virtual void OnPointerUpLeft()
        {
            OnPointerUpLeftEvent?.Invoke();
        }

        public virtual void OnPointerEnter()
        {
            _enterEventAction?.Invoke();
            IsEnterPointer = true;

            UIManager.Instance.SelectController
                .ShowSelectFrame(UISelectFrameTypes.Normal,
                                 SelectFrameSizeDelta,
                                 SelectFrameOffset,
                                 transform,
                                 transform);
        }

        public virtual void OnPointerExit()
        {
            _exitEventAction?.Invoke();
            IsEnterPointer = false;

            UIManager.Instance.SelectController.HideSelectFrame();
        }

        public virtual void RegisterOnPointEnter(UnityAction unityAction)
        {
            _enterEventAction += unityAction;
        }

        public virtual void RegisterOnPointExit(UnityAction unityAction)
        {
            _exitEventAction += unityAction;
        }

        public void ClearSelectIndex()
        {
            SelectIndex = DEFAULT_SELECT_INDEX;
            SelectLeftIndex = DEFAULT_SELECT_INDEX;
            SelectRightIndex = DEFAULT_SELECT_INDEX;
            SelectUpIndex = DEFAULT_SELECT_INDEX;
            SelectDownIndex = DEFAULT_SELECT_INDEX;
        }

        public void SetSelectIndex(int selectIndex)
        {
            SelectIndex = selectIndex;
        }

        private void OnDrawGizmos()
        {
            float offset = 24;
            int fontSize = 10;

            if (SelectLeftIndex > 0)
            {
                GizmoEx.DrawText(SelectLeftIndex.ToColorString(GameColors.WhiteSmoke), position + (Vector3.left * offset), fontSize);
            }
            if (SelectRightIndex > 0)
            {
                GizmoEx.DrawText(SelectRightIndex.ToColorString(GameColors.AntiqueWhite), position + (Vector3.right * offset), fontSize);
            }
            if (SelectUpIndex > 0)
            {
                GizmoEx.DrawText(SelectUpIndex.ToColorString(GameColors.FloralWhite), position + (Vector3.up * offset), fontSize);
            }
            if (SelectDownIndex > 0)
            {
                GizmoEx.DrawText(SelectDownIndex.ToColorString(GameColors.GhostWhite), position + (Vector3.down * offset), fontSize);
            }
            if (SelectIndex > 0)
            {
                GizmoEx.DrawText(SelectIndex.ToColorString(GameColors.LimeGreen), position);
            }
        }
    }
}