using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace TeamSuneat.Stage
{
    public class StageScrollController : XBehaviour
    {
        [FoldoutGroup("스크롤 설정")]
        [SerializeField]
        [SuffixLabel("초당 이동 거리", Overlay = false)]
        private float _scrollSpeed = 2f;

        [FoldoutGroup("스크롤 설정")]
        [SerializeField]
        [SuffixLabel("스크롤 방향 벡터", Overlay = false)]
        private Vector3 _scrollDirection = Vector3.left;

        [FoldoutGroup("컨테이너")]
        [SerializeField]
        [SuffixLabel("스크롤되는 오브젝트들의 부모 Transform", Overlay = false)]
        private Transform _scrollContainer;

        [FoldoutGroup("배경 레이어")]
        [SerializeField]
        [SuffixLabel("무한 배경 레이어 배열", Overlay = false)]
        private InfiniteBackgroundLayer[] _infiniteBackgroundLayers;

        [FoldoutGroup("포인트 이동")]
        [SerializeField]
        [SuffixLabel("첫 스폰 위치", Overlay = false)]
        private Vector3 _firstSpawnPosition;

        [FoldoutGroup("포인트 이동")]
        [SerializeField]
        [SuffixLabel("스폰 위치 간격", Overlay = false)]
        private float _spawnPositionInterval = GameDefine.MONSTER_SPAWN_POSITION_PADDING;

        [FoldoutGroup("포인트 이동")]
        [SerializeField]
        [SuffixLabel("최대 포인트 인덱스", Overlay = false)]
        private int _maxPointIndex = 10;

        [FoldoutGroup("포인트 이동")]
        [SerializeField]
        [SuffixLabel("포인트 이동 애니메이션 시간 (초)", Overlay = false)]
        private float _moveToPointDuration = 0.5f;

        private int _currentTargetPointIndex = 0;
        private Vector3 _initialScrollContainerPosition;
        private Tween _moveTween;
        private List<Tween> _backgroundTweens;

        public Transform ScrollContainer => _scrollContainer;

        public Vector3 FirstSpawnPosition
        {
            get
            {
                return _firstSpawnPosition != Vector3.zero
                    ? _firstSpawnPosition
                    : _initialScrollContainerPosition;
            }
        }

        public float SpawnPositionInterval => _spawnPositionInterval;

        public Vector3 GetSpawnPosition(int index)
        {
            Vector3 basePosition = FirstSpawnPosition;
            return new Vector3(
                basePosition.x + (index * SpawnPositionInterval),
                basePosition.y,
                basePosition.z
            );
        }

        private void Awake()
        {
            if (_scrollContainer == null)
            {
                _scrollContainer = transform;
            }

            _backgroundTweens = new List<Tween>();
            _initialScrollContainerPosition = _scrollContainer.position;

            // 무한 배경 레이어 초기화
            if (_infiniteBackgroundLayers != null)
            {
                for (int i = 0; i < _infiniteBackgroundLayers.Length; i++)
                {
                    if (_infiniteBackgroundLayers[i] != null)
                    {
                        _infiniteBackgroundLayers[i].SetScrollDirection(_scrollDirection);
                        _infiniteBackgroundLayers[i].Initialize();
                    }
                }
            }
        }

        protected override void OnRelease()
        {
            base.OnRelease();

            // 모든 Tween 정리
            KillAllMoveTweens();
        }

        public void StopScrolling()
        {
            // 이동 중인 모든 Tween 중지
            KillAllMoveTweens();

            Log.Info(LogTags.Stage, "스크롤 중지");
        }

        public void SetMaxPointIndex(int maxIndex)
        {
            _maxPointIndex = Mathf.Max(0, maxIndex);
        }

        public void MoveToNextPoint()
        {
            _currentTargetPointIndex++;

            if (_currentTargetPointIndex > _maxPointIndex)
            {
                _currentTargetPointIndex = _maxPointIndex;
                Log.Warning(LogTags.Stage, "마지막 포인트에 도달했습니다.");
                return;
            }

            MoveToPoint(_currentTargetPointIndex);
            Log.Info(LogTags.Stage, "스크롤 이동: 포인트 {0}", _currentTargetPointIndex);
        }

        private void MoveToPoint(int index)
        {
            if (_scrollContainer == null)
            {
                return;
            }

            // 기존 Tween들이 있으면 중지
            KillAllMoveTweens();

            Vector3 basePosition = _firstSpawnPosition != Vector3.zero
                ? _firstSpawnPosition
                : _initialScrollContainerPosition;
            Vector3 targetPosition = basePosition + (Vector3.left * index * _spawnPositionInterval);
            float targetX = targetPosition.x;
            float currentX = _scrollContainer.position.x;
            float distanceX = targetX - currentX;

            // ScrollContainer 이동
            _moveTween = _scrollContainer.DOMoveX(targetX, _moveToPointDuration)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    _moveTween = null;
                });

            // InfiniteBackgroundLayers 이동
            // MoveBackgroundLayersToPoint(distanceX, _moveToPointDuration);
            RepositionBackgroundLayer();
        }

        private void MoveBackgroundLayersToPoint(float distanceX, float duration)
        {
            if (!_infiniteBackgroundLayers.IsValid())
            {
                return;
            }

            for (int i = 0; i < _infiniteBackgroundLayers.Length; i++)
            {
                if (_infiniteBackgroundLayers[i] == null)
                {
                    continue;
                }

                Transform[] pieces = _infiniteBackgroundLayers[i].BackgroundPieces;
                if (pieces == null)
                {
                    continue;
                }

                float parallaxSpeed = _infiniteBackgroundLayers[i].ParallaxSpeed;
                float layerDistanceX = distanceX * parallaxSpeed;
                InfiniteBackgroundLayer layer = _infiniteBackgroundLayers[i];

                // 각 배경 조각 이동
                for (int j = 0; j < pieces.Length; j++)
                {
                    if (pieces[j] == null)
                    {
                        continue;
                    }

                    float pieceTargetX = pieces[j].position.x + layerDistanceX;
                    Tween pieceTween = pieces[j].DOMoveX(pieceTargetX, duration)
                        .SetEase(Ease.OutQuad);

                    // 마지막 조각의 Tween 완료 시에만 재배치 확인 (레이어당 한 번만)
                    if (j == pieces.Length - 1)
                    {
                        pieceTween.OnComplete(() =>
                        {
                        });
                    }

                    _backgroundTweens.Add(pieceTween);
                }
            }
        }

        private void RepositionBackgroundLayer()
        {
            if (!_infiniteBackgroundLayers.IsValid())
            {
                return;
            }

            for (int i = 0; i < _infiniteBackgroundLayers.Length; i++)
            {
                if (_infiniteBackgroundLayers[i] == null)
                {
                    continue;
                }

                Transform[] pieces = _infiniteBackgroundLayers[i].BackgroundPieces;
                if (pieces == null)
                {
                    continue;
                }

                InfiniteBackgroundLayer layer = _infiniteBackgroundLayers[i];
                layer.CheckAndReposition();
            }
        }

        private void KillAllMoveTweens()
        {
            if (_moveTween != null && _moveTween.IsActive())
            {
                _moveTween.Kill();
                _moveTween = null;
            }

            if (_backgroundTweens != null)
            {
                for (int i = 0; i < _backgroundTweens.Count; i++)
                {
                    if (_backgroundTweens[i] != null && _backgroundTweens[i].IsActive())
                    {
                        _backgroundTweens[i].Kill();
                    }
                }
                _backgroundTweens.Clear();
            }
        }

        public void ResetToFirstPoint()
        {
            _currentTargetPointIndex = 0;
            MoveToPoint(0);
            ResetInfiniteBackgroundLayerPosition();

            Log.Info(LogTags.Stage, "스크롤 리셋: 첫 번째 포인트로 이동");
        }

        private void ResetInfiniteBackgroundLayerPosition()
        {
            if (!_infiniteBackgroundLayers.IsValid())
            {
                return;
            }
            for (int i = 0; i < _infiniteBackgroundLayers.Length; i++)
            {
                InfiniteBackgroundLayer item = _infiniteBackgroundLayers[i];
                item.ResetPosition();
            }
        }
    }
}