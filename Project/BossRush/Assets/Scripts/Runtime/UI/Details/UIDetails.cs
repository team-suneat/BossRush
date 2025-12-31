using System.Collections;
using TeamSuneat.Data;
using Lean.Pool;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace TeamSuneat.UserInterface
{
    public class UIDetails : XBehaviour, IPoolable
    {
        public virtual UIDetailsNames Name => UIDetailsNames.None;

        [FoldoutGroup("#UIDetails")] public CanvasGroup CanvasGroup; // UI의 투명도 및 상호작용 상태를 제어하기 위한 CanvasGroup
        [FoldoutGroup("#UIDetails")] public RectTransform Rect; // UI의 위치와 크기를 제어하는 RectTransform
        [FoldoutGroup("#UIDetails")] public float WaitShowTime = 0; // UI 표시를 지연시키는 시간(초)
        [FoldoutGroup("#UIDetails")] public bool IgnoreCallDespawn; // Despawn 호출을 무시할지 여부
        [FoldoutGroup("#UIDetails")] public UIDetailsOffset DetailsOffset;

        private UnityAction<bool> _despawnCallback; // Despawn 시 호출될 콜백

        //──────────────────────────────────────────────────────────────────────────────────────────────────────

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();
            CanvasGroup = GetComponent<CanvasGroup>(); // CanvasGroup 컴포넌트 자동 할당
            Rect = this.FindComponent<RectTransform>("Rect"); // RectTransform 컴포넌트에서 Rect를 찾음
            DetailsOffset = GetComponent<UIDetailsOffset>();
        }

        public override void AutoSetting()
        {
            base.AutoSetting();

            if (rectTransform != null)
            {
                rectTransform.anchorMin = Vector2.zero; // 앵커 최소값 초기화
                rectTransform.anchorMax = Vector2.zero; // 앵커 최대값 초기화
                rectTransform.pivot = Vector2.zero;     // Pivot 초기화
            }
        }

        //

        public void SetTargetPosition(RectTransform targetRect)
        {
            if (DetailsOffset != null)
            {
                DetailsOffset.SetTargetPositionFromSource(targetRect);
            }
        }

        public void SetTargetPosition(RectTransform targetRect, Vector3 offset)
        {
            if (DetailsOffset != null)
            {
                DetailsOffset.SetTargetPositionFromSource(targetRect);
                DetailsOffset.DetailsOffset = offset;
            }
        }

        //──────────────────────────────────────────────────────────────────────────────────────────────────────

        public virtual void OnSpawn()
        {
            if (!IgnoreCallDespawn && UIManager.Instance != null)
            {
                UIManager.Instance.DetailsManager.Register(this); // DetailsManager에 등록
            }
        }

        public virtual void OnDespawn()
        {
            ClearDespawnCallback(); // Despawn 콜백 초기화
        }

        private void Despawn(bool result)
        {
            OnDespawnCallback(result);

            if (!IgnoreCallDespawn && UIManager.Instance != null)
            {
                // DetailsManager에서 등록 해제
                UIManager.Instance.DetailsManager.Unregister(this);
            }

            if (!IsDestroyed)
            {
                ResourcesManager.Despawn(gameObject); // GameObject Despawn 처리
            }
        }

        //──────────────────────────────────────────────────────────────────────────────────────────────────────

        protected virtual void Show()
        {
            if (WaitShowTime > 0)
            {
                _ = StartXCoroutine(ProcessFadeIn(WaitShowTime)); // 코루틴으로 페이드인 처리
            }
            else
            {
                CanvasGroup.alpha = 1f; // UI를 바로 표시
            }
        }

        protected virtual void Hide()
        {
            CanvasGroup.alpha = 0f; // UI를 숨김
        }

        //──────────────────────────────────────────────────────────────────────────────────────────────────────

        public void CloseWithSuccess()
        {
            Log.Info(LogTags.UI_Details, "{0}, 상세정보(Details)를 닫습니다. 결과: 성공", this.GetHierarchyName());
            Despawn(true);
        }

        public virtual void CloseWithFailure()
        {
            Log.Info(LogTags.UI_Details, "{0}, 상세정보(Details)를 닫습니다. 결과: 실패", this.GetHierarchyName());
            Despawn(false);
        }

        //──────────────────────────────────────────────────────────────────────────────────────────────────────

        public IEnumerator ProcessFadeIn(float time)
        {
            yield return new WaitForSecondsRealtime(time); // 지정한 시간만큼 대기

            if (UIManager.Instance.DetailsManager.Contains(this))
            {
                CanvasGroup.alpha = 1; // UI 표시
            }
            else
            {
                Despawn(false); // 유효하지 않으면 Despawn 처리
            }
        }

        public void RegisterDespawnCallback(UnityAction<bool> action)
        {
            _despawnCallback += action; // Despawn 콜백 추가
        }

        public void UnregisterDespawnCallback(UnityAction<bool> action)
        {
            _despawnCallback -= action; // Despawn 콜백 제거
        }

        private void ClearDespawnCallback()
        {
            _despawnCallback = null; // 콜백 초기화
        }

        private void OnDespawnCallback(bool result)
        {
            _despawnCallback?.Invoke(result); // Despawn 콜백 실행
        }

        //──────────────────────────────────────────────────────────────────────────────────────────────────────

        public void SetCanvasParent()
        {
            CanvasOrder detailCanvas = UIManager.Instance.GetCanvas(CanvasOrderNames.Details);
            if (detailCanvas != null)
            {
                transform.SetParent(detailCanvas.transform, true);
            }
        }
    }
}