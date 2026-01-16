using DG.Tweening;
using Sirenix.OdinInspector;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;

namespace TeamSuneat.UserInterface
{
    public class UISelectElement : XBehaviour
    {
        public enum PadClickTypes
        {
            Right,
            Left,
        }

        private const int DEFAULT_SELECT_INDEX = 0;
        private const float DEFAULT_CLICK_DELAY = 0.3f;

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

        [FoldoutGroup("#Event")]
        public UnityEvent OnPointerPressLeftEvent;

        [FoldoutGroup("#Event")]
        public UnityEvent OnPointerClickLeftEvent;

        [FoldoutGroup("#Event")]
        public UnityEvent OnPointerClickRightEvent;

        [FoldoutGroup("#Event")]
        public UnityEvent OnPointerUpLeftEvent;

        protected bool IsEnterPointer { get; private set; }

        private UnityAction _enterEventAction;
        private UnityAction _exitEventAction;
        private Tween _clickDelayTween;

        #region Editor

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

        [FoldoutGroup("#Buttons", 999)]
        [Button("Resize Frame", ButtonSizes.Medium)]
        [Conditional("UNITY_EDITOR")]
        private void ResizeFrame()
        {
            if (SelectFrameSizeDelta.IsZero())
            {
                if (Selectable != null)
                {
                    SelectFrameSizeDelta = Selectable.sizeDelta;
                }
                else if (Clickable != null)
                {
                    SelectFrameSizeDelta = Clickable.sizeDelta;
                }
            }
        }

        #endregion Editor

        protected virtual void Awake()
        {
            RegisterClickableEvents();
            RegisterSelectableEvents();
        }

        protected override void OnDisabled()
        {
            base.OnDisabled();
            KillClickDelayTween();
        }

        protected override void OnRelease()
        {
            base.OnRelease();
            KillClickDelayTween();
        }

        public void OnPointerClick()
        {
            if (PadClickType == PadClickTypes.Left)
            {
                OnPointerClickLeft();
            }
            else if (PadClickType == PadClickTypes.Right)
            {
                OnPointerClickRight();
            }
        }

        public virtual void OnPointerPressLeft()
        {
            OnPointerPressLeftEvent?.Invoke();
        }

        public virtual void OnPointerClickLeft()
        {
            // 딜레이 중이면 클릭 무시
            if (_clickDelayTween != null)
            {
                return;
            }

            StartClickDelay(() => OnPointerClickLeftEvent?.Invoke());
        }

        public virtual void OnPointerClickRight()
        {
            // 딜레이 중이면 클릭 무시
            if (_clickDelayTween != null)
            {
                return;
            }

            StartClickDelay(() => OnPointerClickRightEvent?.Invoke());
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

        private void RegisterClickableEvents()
        {
            if (Clickable == null)
            {
                return;
            }

            Clickable.RegisterPointerClickLeftEvent(_ => OnPointerClickLeft());
            Clickable.RegisterPointerClickRightEvent(_ => OnPointerClickRight());
            Clickable.RegisterPointerPressLeftEvent(_ => OnPointerPressLeft());
            Clickable.RegisterPointerUpLeftEvent(_ => OnPointerUpLeft());
        }

        private void RegisterSelectableEvents()
        {
            if (Selectable == null)
            {
                return;
            }

            Selectable.RegisterPointerEnterEvent(_ => OnPointerEnter());
            Selectable.RegisterPointerExitEvent(_ => OnPointerExit());
        }

        private void StartClickDelay(UnityAction action)
        {
            KillClickDelayTween();
            _clickDelayTween = DOVirtual.DelayedCall(DEFAULT_CLICK_DELAY, () =>
            {
                _clickDelayTween = null;
                action?.Invoke();
            });
        }

        private void KillClickDelayTween()
        {
            _clickDelayTween?.Kill();
            _clickDelayTween = null;
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