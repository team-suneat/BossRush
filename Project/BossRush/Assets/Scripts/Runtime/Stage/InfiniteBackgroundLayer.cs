using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat.Stage
{
    [System.Serializable]
    public class InfiniteBackgroundLayer
    {
        [FoldoutGroup("배경 조각")]
        [SerializeField]
        [SuffixLabel("무한 스크롤을 위한 배경 조각 Transform 배열", Overlay = false)]
        private Transform[] _backgroundPieces;

        [FoldoutGroup("배경 조각")]
        [SerializeField]
        [SuffixLabel("각 배경 조각의 너비 (자동 계산 가능)", Overlay = false)]
        private float _backgroundWidth = 10f;

        [FoldoutGroup("스크롤 설정")]
        [SerializeField]
        [SuffixLabel("패럴랙스 효과 속도 배율 (1.0 = 기본 속도)", Overlay = false)]
        private float _parallaxSpeed = 1f;

        [FoldoutGroup("스크롤 설정")]
        [SerializeField]
        [SuffixLabel("배경 스크롤 방향 벡터", Overlay = false)]
        private Vector3 _scrollDirection = Vector3.left;

        private bool _isInitialized = false;
        private Camera _cachedCamera;
        private float _cachedCameraWidth;

        public Transform[] BackgroundPieces => _backgroundPieces;

        public float ParallaxSpeed
        {
            get => _parallaxSpeed;
            set => _parallaxSpeed = value;
        }

        private Vector3 _firstPiecePosition;

        public void Initialize()
        {
            if (_backgroundPieces == null || _backgroundPieces.Length < 2)
            {
                Log.Warning(LogTags.Stage, "무한 배경 레이어 초기화 실패: 배경 조각이 2개 미만입니다.");
                return;
            }

            // 배경 너비 자동 계산 (첫 번째 배경의 SpriteRenderer 또는 RectTransform 사용)
            if (_backgroundWidth <= 0.1f)
            {
                _backgroundWidth = CalculateBackgroundWidth(_backgroundPieces[0]);
            }

            if (_backgroundWidth <= 0.1f)
            {
                Log.Warning(LogTags.Stage, "배경 너비를 계산할 수 없습니다. 수동으로 설정해주세요.");
                _backgroundWidth = 10f;
            }

            // 첫 번째 배경 조각의 위치를 기준으로 설정
            _firstPiecePosition = _backgroundPieces[0].localPosition;

            // 배경 위치 초기화
            ResetPosition();

            _isInitialized = true;
        }

        public void ResetPosition()
        {
            if (_backgroundPieces == null || _backgroundPieces.Length < 2)
            {
                return;
            }

            // 배경들을 나란히 배치
            for (int i = 0; i < _backgroundPieces.Length; i++)
            {
                if (_backgroundPieces[i] != null)
                {
                    _backgroundPieces[i].localPosition = new Vector3(
                        _firstPiecePosition.x + (i * _backgroundWidth),
                        _firstPiecePosition.y,
                        _firstPiecePosition.z
                    );
                }
            }
        }

        private float CalculateBackgroundWidth(Transform backgroundTransform)
        {
            if (backgroundTransform == null)
            {
                return 0f;
            }

            // SpriteRenderer 확인
            SpriteRenderer spriteRenderer = backgroundTransform.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null && spriteRenderer.sprite != null)
            {
                return spriteRenderer.bounds.size.x;
            }

            // RectTransform 확인 (UI Image)
            RectTransform rectTransform = backgroundTransform.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                return rectTransform.rect.width;
            }

            // Renderer의 bounds 확인
            Renderer renderer = backgroundTransform.GetComponent<Renderer>();
            if (renderer != null)
            {
                return renderer.bounds.size.x;
            }

            return 0f;
        }

        public void CheckAndReposition()
        {
            if (_backgroundPieces == null || _backgroundPieces.Length < 2)
            {
                return;
            }

            UpdateCameraCache();
            if (_cachedCamera == null)
            {
                return;
            }

            float cameraX = _cachedCamera.transform.position.x;
            float cameraWidth = _cachedCameraWidth;
            bool isScrollingLeft = _scrollDirection.x < 0;

            if (isScrollingLeft)
            {
                float threshold = cameraX - (cameraWidth * 0.5f + _backgroundWidth);
                for (int i = 0; i < _backgroundPieces.Length; i++)
                {
                    if (_backgroundPieces[i] == null)
                    {
                        continue;
                    }

                    float positionX = _backgroundPieces[i].position.x;
                    if (positionX < threshold)
                    {
                        float rightmostX = FindExtremePiecePosition(i, true);
                        if (rightmostX != float.MinValue)
                        {
                            RepositionPiece(i, rightmostX, true);
                        }
                    }
                }
            }
            else if (_scrollDirection.x > 0)
            {
                float rightThreshold = cameraX + (cameraWidth * 0.5f + _backgroundWidth);
                for (int i = 0; i < _backgroundPieces.Length; i++)
                {
                    if (_backgroundPieces[i] == null)
                    {
                        continue;
                    }

                    float positionX = _backgroundPieces[i].position.x;
                    if (positionX > rightThreshold)
                    {
                        float leftmostX = FindExtremePiecePosition(i, false);
                        if (leftmostX != float.MaxValue)
                        {
                            RepositionPiece(i, leftmostX, false);
                        }
                    }
                }
            }
        }

        private float FindExtremePiecePosition(int excludeIndex, bool findRightmost)
        {
            float extremeX = findRightmost ? float.MinValue : float.MaxValue;

            for (int j = 0; j < _backgroundPieces.Length; j++)
            {
                if (_backgroundPieces[j] != null && _backgroundPieces[j] != _backgroundPieces[excludeIndex])
                {
                    float positionX = _backgroundPieces[j].position.x;
                    if (findRightmost)
                    {
                        extremeX = Mathf.Max(extremeX, positionX);
                    }
                    else
                    {
                        extremeX = Mathf.Min(extremeX, positionX);
                    }
                }
            }

            return extremeX;
        }

        private void RepositionPiece(int pieceIndex, float extremePosition, bool isRightmost)
        {
            if (pieceIndex < 0 || pieceIndex >= _backgroundPieces.Length || _backgroundPieces[pieceIndex] == null)
            {
                return;
            }

            float newX = isRightmost ? extremePosition + _backgroundWidth : extremePosition - _backgroundWidth;
            _backgroundPieces[pieceIndex].position = new Vector3(
                newX,
                _backgroundPieces[pieceIndex].position.y,
                _backgroundPieces[pieceIndex].position.z
            );
        }

        private void UpdateCameraCache()
        {
            Camera mainCamera = Camera.main;
            if (mainCamera != _cachedCamera)
            {
                _cachedCamera = mainCamera;
                if (_cachedCamera != null)
                {
                    _cachedCameraWidth = GetCameraWidth(_cachedCamera);
                }
            }
        }

        private float GetCameraWidth(Camera camera)
        {
            if (camera == null)
            {
                return 20f;
            }

            float height = 2f * camera.orthographicSize;
            float width = height * camera.aspect;
            return width;
        }

        public void SetScrollDirection(Vector3 direction)
        {
            _scrollDirection = direction;
        }
    }
}