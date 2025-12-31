using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace TeamSuneat
{
    /// <summary>
    /// 포지션 그룹에서 위치 정렬 우선순위를 지정하는 열거형
    /// </summary>
    public enum SortPriorities
    {
        None,           // 정렬 우선순위가 지정되지 않음
        Nearest,        // 기준 위치에서 가장 가까운 순으로 정렬
        Farthest,       // 기준 위치에서 가장 먼 순으로 정렬
        Randomly,       // 무작위로 정렬
        Deck,           // 덱 방식(순서대로 사용 후 다시 재구성)
        NotShuffleDeck, // 섞지 않는 덱 방식(무작위 정렬 옵션 있음)
    }

    /// <summary>
    /// PositionGroup 클래스는 게임 오브젝트의 자식 위치(Transform)를 관리하고,
    /// 위치 정렬 전략에 따라 적절한 위치를 계산하여 반환하는 역할을 합니다.
    /// XBehaviour는 MonoBehaviour를 상속받은 커스텀 베이스 클래스라고 가정합니다.
    /// </summary>
    public partial class PositionGroup : XBehaviour
    {
        // 포지션 그룹의 타입을 지정 (예: 대상(Target) 그룹 등)
        public enum Types
        {
            None,
            Target,
        }

        #region Field

        // 포지션 그룹의 타입 (예: 대상 그룹)
        public Types Type;

        // 포지션 그룹에 대한 설명을 에디터에서 입력할 수 있도록 TextArea로 표시
        [TextArea]
        [Title("#PositionGroup")]
        public string Description;

        // 이 옵션이 true이면, 매니저에 포지션 그룹을 등록하지 않고 캐싱 방식으로 사용함
        [InfoBox("매니저에 해당 포지션 그룹을 등록하지 않습니다. 캐싱을 통해 포지션 그룹을 사용합니다.")]
        public bool IgnoreRegister;

        // 자식 포지션(Transform) 리스트 : 기본적으로 에디터에서 지정하거나, 자식 오브젝트를 자동으로 추가함
        [FoldoutGroup("#PositionGroup-Point")]
        public List<Transform> Children = new();

        // 포지션 그룹을 식별하기 위한 키 값들 (각각 다른 용도의 이름)
        [FoldoutGroup("#PositionGroup-Keys")] public HitmarkNames HitmarkName;
        [FoldoutGroup("#PositionGroup-Keys")] public string HitmarkNameString;

        // 위치 정렬 우선순위를 지정하며, 기본은 무작위 정렬(Randomly)로 설정됨
        [FoldoutGroup("#PositionGroup-Sort")]
        public SortPriorities SortPriority = SortPriorities.Randomly;

        // 덱 방식 정렬 시 마지막 위치를 무시할지 여부
        [FoldoutGroup("#PositionGroup-Sort")]
        public bool IgnoreLastDeckPosition;

        // NotShuffleDeck 정렬 방식을 사용할 때, 무작위 정렬 순서를 사용할지 여부
        [FoldoutGroup("#PositionGroup-Sort/#Not Shuffle Deck")]
        [EnableIf("SortPriority", SortPriorities.NotShuffleDeck)]
        [SuffixLabel("무작위 정렬 순서 사용")]
        public bool UseRandomSortOrder;

        // 에디터에서 자식 위치를 기즈모(Gizmo)로 표시할지 여부 및 다른 포지션 그룹과 연결할 수 있는 옵션
        [FoldoutGroup("#PositionGroup-Editor")]
        public bool ShowChildrenPositionInGizmo;

        [FoldoutGroup("#PositionGroup-Editor")]
        public PositionGroup LinkedPositionGroup;

        // 내부적으로 자식 Transform을 캐싱하는 리스트
        private List<Transform> _childrenPoints;

        // 현재 설정된 정렬 전략(인터페이스를 통해 다양한 정렬 전략 적용 가능)
        private IPositionGroupSortStrategy _sortStrategy;

        #endregion Field

        protected void Awake()
        {
            // 자식 Transform을 초기화하고 캐싱하며, null 요소 제거
            InitializeChildren();

            // 지정된 SortPriority에 따라 정렬 전략을 설정
            SetPositionGroupSortStrategy();
        }

        /// <summary>
        /// 자식 Transform을 초기화하는 메서드
        /// </summary>
        private void InitializeChildren()
        {
            // 자식 오브젝트들을 Children 리스트에 추가
            SetupChildren();

            // 리스트 내에 null 값 제거 (런타임 오류 방지)
            Children.RemoveNull();

            // 캐싱을 위해 _childrenPoints 리스트를 생성하고 Children의 값을 복사
            _childrenPoints = new List<Transform>();
            _childrenPoints.AddRange(Children);
        }

        protected override void OnStart()
        {
            base.OnStart();

            // 자식 오브젝트들의 SpriteRenderer를 비활성화하여, 씬에서 보이지 않도록 함
            DisableRenderers();
        }

        protected override void OnEnabled()
        {
            base.OnEnabled();

            // IgnoreRegister가 false이면, 포지션 그룹을 매니저에 등록
            if (!IgnoreRegister)
            {
                RegisterToManager();
            }
        }

        protected override void OnDisabled()
        {
            base.OnDisabled();

            // 매니저에서 포지션 그룹 등록 해제
            UnregisterFromManager();
        }

        /// <summary>
        /// 매니저에 포지션 그룹을 등록하는 메서드
        /// </summary>
        private void RegisterToManager()
        {
            // 각 키(Hitmark, FV, Blink) 중 설정된 값이 있으면 해당 키를 사용해 등록
            if (HitmarkName != HitmarkNames.None)
            {
                PositionGroupManager.Instance.Register(HitmarkName, this);
            }
        }

        /// <summary>
        /// 매니저에서 포지션 그룹 등록을 해제하는 메서드
        /// </summary>
        private void UnregisterFromManager()
        {
            PositionGroupManager.Instance.Unregister(this);
        }

        /// <summary>
        /// SortPriority 값에 따라 적절한 정렬 전략 객체를 생성 및 설정하는 메서드
        /// </summary>
        private void SetPositionGroupSortStrategy()
        {
            switch (SortPriority)
            {
                case SortPriorities.Nearest:
                    _sortStrategy = new NearestSortStrategy();
                    break;

                case SortPriorities.Farthest:
                    _sortStrategy = new FarthestSortStrategy();
                    break;

                case SortPriorities.Randomly:
                    _sortStrategy = new RandomSortStrategy();
                    break;

                case SortPriorities.Deck:
                    _sortStrategy = new DeckSortStrategy(Children, IgnoreLastDeckPosition);
                    break;

                case SortPriorities.NotShuffleDeck:
                    _sortStrategy = new NotShuffleDeckSortStrategy(Children, UseRandomSortOrder, IgnoreLastDeckPosition);
                    break;

                default:
                    Log.Error("정렬 우선순위가 설정되지 않았습니다.");
                    return;
            }
        }

        /// <summary>
        /// 자식 오브젝트들을 Children 리스트에 추가하는 메서드
        /// </summary>
        public void SetupChildren()
        {
            // 리스트 내 null 제거
            Children.RemoveNull();

            // 현재 Transform의 모든 자식에 대해
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);

                // 이미 리스트에 포함되지 않았다면 추가
                if (!Children.Contains(child))
                {
                    Children.Add(child);
                }
            }
        }

        /// <summary>
        /// 덱 정렬 전략의 내부 상태를 초기화하는 메서드
        /// </summary>
        public void ClearPositionDeck()
        {
            _sortStrategy?.Clear();
        }

        /// <summary>
        /// 인덱스에 해당하는 자식의 위치(Vector3)를 반환 (인덱스 범위 체크 포함)
        /// </summary>
        public Vector3 GetChildPosition(int index)
        {
            if (Children.IsValid())
            {
                if (index < 0)
                {
                    return Children[0].position;
                }
                if (index <= Children.Count - 1)
                {
                    return Children[index].position;
                }
            }
            return Vector3.zero;
        }

        /// <summary>
        /// 덱 방식 정렬일 경우, 덱을 구성하는 메서드 (Deck 또는 NotShuffleDeck 전용)
        /// </summary>
        public void SetupDeck()
        {
            switch (SortPriority)
            {
                case SortPriorities.Deck:
                    {
                        // DeckSortStrategy로 캐스팅 후, 덱 설정
                        DeckSortStrategy deckSortStrategy = _sortStrategy as DeckSortStrategy;
                        deckSortStrategy.SetupDeck(Children);
                    }
                    break;

                case SortPriorities.NotShuffleDeck:
                    {
                        // NotShuffleDeckSortStrategy로 캐스팅 후, 덱 설정
                        NotShuffleDeckSortStrategy notShuffleDeckStrategy = _sortStrategy as NotShuffleDeckSortStrategy;
                        notShuffleDeckStrategy.SetupDeck(Children);
                    }
                    break;
            }
        }

        /// <summary>
        /// 주어진 기준 위치(originPosition)를 기준으로 정렬 전략에 따라 위치를 반환
        /// </summary>
        public Vector3 GetPosition(Vector3 originPosition)
        {
            return _sortStrategy.GetPosition(originPosition, Children);
        }

        public Vector3 GetPosition(Vector3 originPosition, int positionIndex)
        {
            List<Vector3> positions = GetPositions(originPosition, Children.Count);
            if (positions.IsValid(positionIndex))
            {
                return positions[positionIndex];
            }

            return Vector3.zero;
        }

        /// <summary>
        /// 주어진 기준 위치와 개수(positionCount)를 기반으로 위치 리스트를 반환
        /// </summary>
        public List<Vector3> GetPositions(Vector3 originPosition, int positionCount)
        {
            return _sortStrategy.GetPositions(originPosition, Children, positionCount);
        }

        /// <summary>
        /// 주어진 기준 위치와 개수(positionCount)를 기반으로 무작위 섞인 위치 리스트를 반환
        /// </summary>
        public List<Vector3> GetShufflePositions(Vector3 originPosition, int positionCount)
        {
            return _sortStrategy.GetShufflePositions(originPosition, Children, positionCount);
        }

        /// <summary>
        /// 모든 자식의 위치를 특정 point로 갱신하는 메서드 (예: 위치 초기화)
        /// </summary>
        public void RefreshChildPoint(Vector3 point)
        {
            if (Children.IsValid())
            {
                for (int i = 0; i < Children.Count; i++)
                {
                    Children[i].position = point;
                }
            }
        }

        /// <summary>
        /// 자식 오브젝트들의 SpriteRenderer를 비활성화하여, 게임 실행 시 보이지 않게 설정하는 메서드
        /// </summary>
        private void DisableRenderers()
        {
            // 자식 오브젝트에서 SpriteRenderer 컴포넌트를 모두 가져옴
            SpriteRenderer[] _childrenRenderers = GetComponentsInChildren<SpriteRenderer>();
            if (_childrenRenderers.IsValid())
            {
                for (int i = 0; i < _childrenRenderers.Length; i++)
                {
                    _childrenRenderers[i].enabled = false;
                }
            }
        }
    }
}