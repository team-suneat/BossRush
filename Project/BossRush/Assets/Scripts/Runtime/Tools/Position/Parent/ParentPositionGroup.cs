using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TeamSuneat
{
    /// <summary>
    /// 자식 PositionGroup들을 관리하며, 다양한 방식(RetrievalMode)으로 포지션들을 반환하는 역할을 합니다.
    /// </summary>
    public class ParentPositionGroup : XBehaviour
    {
        [TextArea]
        [Title("#PositionGroup")]
        public string Description;

        [Title("#자식 포지션 그룹")]
        [InfoBox("관리할 자식 포지션 그룹들을 할당합니다.")]
        [SerializeField]
        private List<PositionGroup> _childGroups = new();

        /// <summary>
        /// 포지션을 불러오는 모드를 정의한 enum입니다.
        /// </summary>
        public enum RetrievalMode
        {
            GroupShuffle,
            AllShuffle,
            SingleGroup
        }

        [Title("#포지션 불러오기 모드")]
        [InfoBox("$_retrievalModeMessage")]
        [SerializeField]
        private RetrievalMode _retrievalMode = RetrievalMode.GroupShuffle;
        private string _retrievalModeMessage;

        [Title("#Keys")]
        public HitmarkNames HitmarkName;

        [SerializeField] private string HitmarkNameString;

        private bool _isShuffled;             // 이미 섞었는지 여부
        private List<Vector3> _cachedPositions; // 섞은 결과를 캐싱할 리스트

#if UNITY_EDITOR

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();
            _childGroups.Clear();
            _childGroups.AddRange(GetComponentsInChildren<PositionGroup>());
        }

        public override void AutoSetting()
        {
            base.AutoSetting();

            if (HitmarkName != HitmarkNames.None)
            {
                HitmarkNameString = HitmarkName.ToString();
            }
        }

        private void OnValidate()
        {
            EnumEx.ConvertTo(ref HitmarkName, HitmarkNameString);

            SetRetrievalModeMessage();
        }

        /// <summary>
        /// 현재 _retrievalMode에 따라 에디터에 표시할 메시지를 설정합니다.
        /// </summary>
        private void SetRetrievalModeMessage()
        {
            switch (_retrievalMode)
            {
                case RetrievalMode.GroupShuffle:
                    _retrievalModeMessage = "GroupShuffle: 자식 그룹 순서를 무작위로 섞되, 각 그룹 내부의 순서는 그대로 유지합니다.";
                    break;

                case RetrievalMode.AllShuffle:
                    _retrievalModeMessage = "AllShuffle: 모든 포지션을 모은 후 전체를 무작위로 섞습니다.";
                    break;

                case RetrievalMode.SingleGroup:
                    _retrievalModeMessage = "SingleGroup: 자식 그룹 중 하나를 무작위로 선택하여 인덱스에 맞는 포지션을 반환합니다.";
                    break;
            }
        }

        /// <summary>
        /// 버튼 클릭 시, 첫 번째 자식 그룹의 키 값을 부모에 로드합니다.
        /// </summary>
        [FoldoutGroup("#Buttons2", 1000)]
        [Button(ButtonSizes.Medium)]
        private void LoadKeyToChildren()
        {
            if (_childGroups.IsValid())
            {
                if (_childGroups.Count == 0)
                {
                    Log.Warning("자식 포지션 그룹이 없습니다.");
                    return;
                }

                // 첫 번째 자식 그룹의 키 값을 부모로 불러옵니다.
                HitmarkName = _childGroups[0].HitmarkName;
                HitmarkNameString = _childGroups[0].HitmarkNameString;
            }
        }

#endif

        private void Awake()
        {
            if (_childGroups.IsValid())
            {
                foreach (PositionGroup group in _childGroups)
                {
                    group.IgnoreRegister = true;
                }
            }
        }

        protected override void OnEnabled()
        {
            base.OnEnabled();
            RegisterToManager();
        }

        protected override void OnDisabled()
        {
            base.OnDisabled();
            UnregisterFromManager();
        }

        /// <summary>
        /// 해당 키 값(Hitmark, FV, Blink) 중 설정된 값에 따라 PositionGroupManager에 등록합니다.
        /// </summary>
        private void RegisterToManager()
        {
            if (HitmarkName != HitmarkNames.None)
            {
                PositionGroupManager.Instance.Register(HitmarkName, this);
            }
        }

        /// <summary>
        /// PositionGroupManager에서 이 객체의 등록을 해제합니다.
        /// </summary>
        private void UnregisterFromManager()
        {
            PositionGroupManager.Instance.Unregister(this);
        }

        /// <summary>
        /// 지정된 기준 위치(originPosition)를 기준으로, 섞인 포지션 리스트를 반환합니다.
        /// </summary>
        /// <param name="originPosition">기준이 되는 위치</param>
        /// <param name="positionCount">
        /// 반환할 포지션의 개수. 기본값 -1이면 전체 리스트를 반환합니다.
        /// </param>
        /// <returns>계산된 포지션 벡터 리스트</returns>
        public List<Vector3> GetPositions(Vector3 originPosition, int positionCount = -1)
        {
            if (!_isShuffled)
            {
                Log.Warning("아직 Shuffle가 실행되지 않았습니다. 자동으로 ShuffleNow()를 호출합니다.");
                ShuffleNow();
            }

            if (_retrievalMode != RetrievalMode.SingleGroup && positionCount > 0 && positionCount < _cachedPositions.Count)
            {
                return _cachedPositions.Take(positionCount).ToList();
            }
            return _cachedPositions;
        }

        /// <summary>
        /// 자식 그룹들을 무작위로 섞은 후, 각 그룹의 포지션을 순서대로 가져와 하나의 리스트로 반환합니다.
        /// (GroupShuffle 모드 전용)
        /// </summary>
        /// <returns>섞인 포지션 벡터 리스트</returns>
        private List<Vector3> GetGroupShufflePositions()
        {
            List<Vector3> positions = new List<Vector3>();
            List<PositionGroup> shuffledGroups = new List<PositionGroup>(_childGroups);
            Shuffle(shuffledGroups);

            foreach (PositionGroup group in shuffledGroups)
            {
                for (int i = 0; i < group.Children.Count; i++)
                {
                    positions.Add(group.GetChildPosition(i));
                }
            }
            return positions;
        }

        /// <summary>
        /// 모든 자식 그룹의 포지션을 모은 후, 전체를 무작위로 섞어 리스트로 반환합니다.
        /// (AllShuffle 모드 전용)
        /// </summary>
        /// <returns>무작위로 섞인 포지션 벡터 리스트</returns>
        private List<Vector3> GetAllShufflePositions()
        {
            List<Vector3> positions = new List<Vector3>();
            foreach (PositionGroup group in _childGroups)
            {
                for (int i = 0; i < group.Children.Count; i++)
                {
                    positions.Add(group.GetChildPosition(i));
                }
            }
            Shuffle(positions);
            return positions;
        }

        /// <summary>
        /// 자식 그룹 중 하나를 무작위로 선택하여, 해당 그룹의 포지션을 반환합니다.
        /// (SingleGroup 모드 전용)
        /// </summary>
        /// <param name="positionCount">
        /// 반환할 포지션의 인덱스 혹은 전체(-1이면 전체)를 결정하는 값
        /// </param>
        /// <returns>선택된 그룹의 포지션 벡터 리스트</returns>
        private List<Vector3> GetSingleGroupPositions(int positionCount)
        {
            List<Vector3> positions = new();
            if (!_childGroups.IsValid() || _childGroups.Count == 0)
            {
                Log.Warning("자식 포지션 그룹이 없습니다.");
                return positions;
            }

            int randomGroupIndex = RandomEx.Range(0, _childGroups.Count);
            PositionGroup selectedGroup = _childGroups[randomGroupIndex];

            if (positionCount < 0)
            {
                for (int i = 0; i < selectedGroup.Children.Count; i++)
                {
                    positions.Add(selectedGroup.GetChildPosition(i));
                }
            }
            else
            {
                if (positionCount < selectedGroup.Children.Count)
                {
                    positions.Add(selectedGroup.GetChildPosition(positionCount));
                }
                else
                {
                    Log.Warning("지정한 인덱스가 선택된 그룹의 범위를 초과합니다.");
                }
            }
            return positions;
        }

        /// <summary>
        /// 현재 설정된 RetrievalMode에 따라 포지션들을 섞어 _cachedPositions에 저장하고, _isShuffled 플래그를 true로 설정합니다.
        /// </summary>
        public void ShuffleNow()
        {
            _cachedPositions = new List<Vector3>();

            switch (_retrievalMode)
            {
                case RetrievalMode.GroupShuffle:
                    _cachedPositions = GetGroupShufflePositions();
                    break;

                case RetrievalMode.AllShuffle:
                    _cachedPositions = GetAllShufflePositions();
                    break;

                case RetrievalMode.SingleGroup:
                    _cachedPositions = GetSingleGroupPositions(-1);
                    break;
            }

            _isShuffled = true;
        }

        /// <summary>
        /// 제네릭 리스트를 Fisher-Yates 알고리즘을 사용하여 무작위로 섞습니다.
        /// </summary>
        /// <typeparam name="T">리스트 항목의 타입</typeparam>
        /// <param name="list">섞을 리스트</param>
        private void Shuffle<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int randomIndex = RandomEx.Range(0, i + 1);
                (list[randomIndex], list[i]) = (list[i], list[randomIndex]);
            }
        }
    }
}