using UnityEngine;

namespace TeamSuneat.UserInterface
{
    public class UIDetailsOffset : XBehaviour
    {
        public Vector2 DetailsOffset { get; set; }

        private Vector3 _targetPosition;

        protected override void OnEnabled()
        {
            base.OnEnabled();

            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        }

        public void SetTargetPositionFromSource(RectTransform sourceRect)
        {
            Canvas sourceCanvas = sourceRect.GetComponentInParent<Canvas>();
            Canvas targetCanvas = rectTransform.GetComponentInParent<Canvas>();
            if (sourceCanvas != targetCanvas)
            {
                // 1. 소스의 월드 위치를 구한다 (중앙 기준)
                Vector3 worldPosition = sourceRect.TransformPoint(sourceRect.rect.center);

                // 2. 캔버스의 렌더 모드에 따라 카메라를 null로 설정
                Camera sourceCamera = sourceCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : sourceCanvas.worldCamera;
                Camera targetCamera = targetCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : targetCanvas.worldCamera;

                // 3. 월드 좌표 ▶ 스크린 좌표 변환
                Vector2 screenPosition = RectTransformUtility.WorldToScreenPoint(sourceCamera, worldPosition);

                // 4. 스크린 좌표 ▶ 타겟 캔버스의 로컬 좌표 변환
                RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPosition, targetCamera, out Vector2 localPosition);

                // 5. 타겟 위치 저장
                _targetPosition = new Vector3(localPosition.x, localPosition.y, 0f);
                Log.Info(LogTags.UI_Details, "디테일({0})의 타겟 위치가 설정되었습니다: {1}", this.GetHierarchyName(), _targetPosition);
            }
            else
            {
                _targetPosition = sourceRect.anchoredPosition3D;
                Log.Info(LogTags.UI_Details, "디테일({0})의 타겟 위치가 설정되었습니다: {1}({2})", this.GetHierarchyName(), sourceRect.GetHierarchyName(), _targetPosition);
            }
        }

        public void SetupPosition(Vector2 backgroundSize)
        {
            anchoredPosition3D = _targetPosition;

            // 1. 현재 위치 ▶ 스크린 좌표 변환
            Vector2 screenPosition = GetScreenPosition(rectTransform);

            // 2. 스크린 좌표 ▶ 부모의 로컬 좌표 변환
            RectTransform parentRect = rectTransform.parent as RectTransform;
            Vector2 localPosition = ConvertScreenToLocalPosition(parentRect, screenPosition);

            if (!DetailsOffset.IsZero())
            {
                localPosition += DetailsOffset;
                Log.Info(LogTags.UI_Details, "디테일({0})의 위치에 오프셋이 적용됩니다: {1}", this.GetHierarchyName(), DetailsOffset);
            }

            // 3. 화면 경계 체크 및 위치 보정
            Vector2 adjustedPosition = AdjustPositionToScreenBounds(rectTransform, localPosition, backgroundSize);

            // 4. 최종 위치 적용
            anchoredPosition3D = new Vector3(adjustedPosition.x, adjustedPosition.y, 0f);
        }

        // 1. 타겟의 월드 좌표를 스크린 좌표로 변환
        private Vector2 GetScreenPosition(RectTransform targetRect)
        {
            Canvas canvas = targetRect.GetComponentInParent<Canvas>();
            Camera cam = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;

            // 중앙 기준의 정확한 위치 변환
            Vector3 worldPosition = targetRect.TransformPoint(targetRect.rect.center);
            return RectTransformUtility.WorldToScreenPoint(cam, worldPosition);
        }

        // 2. 스크린 좌표를 부모의 로컬 좌표로 변환
        private Vector2 ConvertScreenToLocalPosition(RectTransform parentRect, Vector2 screenPosition)
        {
            Canvas canvas = parentRect.GetComponentInParent<Canvas>();
            Camera cam = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, screenPosition, cam, out Vector2 localPosition);
            return localPosition;
        }

        // 3. 화면 경계를 벗어나지 않도록 위치를 보정 (backgroundSize 기준)
        private Vector2 AdjustPositionToScreenBounds(RectTransform targetRect, Vector2 localPosition, Vector2 backgroundSize)
        {
            RectTransform parentRect = targetRect.parent as RectTransform;
            float parentWidth = parentRect.rect.width;
            float parentHeight = parentRect.rect.height;

            // 팝업(디테일) 창의 크기
            Vector2 popupSize = backgroundSize;

            float leftEdge = localPosition.x - (popupSize.x * targetRect.pivot.x);
            float rightEdge = localPosition.x + (popupSize.x * (1 - targetRect.pivot.x));
            float bottomEdge = localPosition.y - (popupSize.y * targetRect.pivot.y);
            float topEdge = localPosition.y + (popupSize.y * (1 - targetRect.pivot.y));

            Vector2 offset = new Vector2(10f, 10f); // 화면 여백 오프셋

            // 왼쪽 경계 체크
            if (leftEdge < -parentWidth * 0.5f)
            {
                float offsetX = (-parentWidth * 0.5f - leftEdge) + offset.x;
                localPosition.x += offsetX;
                Log.Info(LogTags.UI_Details, "디테일({0})의 위치가 왼쪽 경계를 벗어나 추가 오프셋이 적용됩니다: {1}", this.GetHierarchyName(), offsetX);
            }

            // 오른쪽 경계 체크
            if (rightEdge > parentWidth * 0.5f)
            {
                float offsetX = (rightEdge - parentWidth * 0.5f) + offset.x;
                localPosition.x -= offsetX;
                Log.Info(LogTags.UI_Details, "디테일({0})의 위치가 오른쪽 경계를 벗어나 추가 오프셋이 적용됩니다: {1}", this.GetHierarchyName(), offsetX);
            }

            // 아래쪽 경계 체크
            if (bottomEdge < -parentHeight * 0.5f)
            {
                float offsetY = (-parentHeight * 0.5f - bottomEdge) + offset.y;
                localPosition.y += offsetY;
                Log.Info(LogTags.UI_Details, "디테일({0})의 위치가 아래쪽 경계를 벗어나 추가 오프셋이 적용됩니다: {1}", this.GetHierarchyName(), offsetY);
            }

            // 위쪽 경계 체크
            if (topEdge > parentHeight * 0.5f)
            {
                float offsetY = (topEdge - parentHeight * 0.5f) + offset.y;
                localPosition.y -= offsetY;
                Log.Info(LogTags.UI_Details, "디테일({0})의 위치가 위쪽 경계를 벗어나 추가 오프셋이 적용됩니다: {1}", this.GetHierarchyName(), offsetY);
            }

            return localPosition;
        }
    }
}