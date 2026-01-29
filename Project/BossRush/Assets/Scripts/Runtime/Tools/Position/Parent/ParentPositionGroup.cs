using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace TeamSuneat
{
    public class ParentPositionGroup : XBehaviour
    {
        [TextArea]
        [Title("#PositionGroup")]
        [SerializeField]
        private string _description;
        public string Description => _description;

        [Title("#자식 포지션 그룹")]
        [InfoBox("관리할 자식 포지션 그룹들을 할당합니다.")]
        [SerializeField]
        private List<PositionGroup> _childGroups = new();

        public enum RetrievalMode
        {
            None,
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
        [SerializeField]
        private PositionGroupNames _positionGroupName;
        public PositionGroupNames PositionGroupName => _positionGroupName;

        [SerializeField]
        private string _positionGroupNameString;
        public string PositionGroupNameString => _positionGroupNameString;

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

            if (_positionGroupName != PositionGroupNames.None)
            {
                _positionGroupNameString = _positionGroupName.ToString();
            }
        }

        private void OnValidate()
        {
            EnumEx.ConvertTo(ref _positionGroupName, _positionGroupNameString);

            SetRetrievalModeMessage();
        }

        private void SetRetrievalModeMessage()
        {
            switch (_retrievalMode)
            {
                case RetrievalMode.None:
                    _retrievalModeMessage = "None: 포지션 불러오기 모드가 지정되지 않았습니다.";
                    break;

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

                _positionGroupName = _childGroups[0].PositionGroupName;
                _positionGroupNameString = _childGroups[0].PositionGroupNameString;
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

        private void RegisterToManager()
        {
            if (_positionGroupName != PositionGroupNames.None)
            {
                PositionGroupManager.Instance.Register(_positionGroupName, this);
            }
        }

        private void UnregisterFromManager()
        {
            PositionGroupManager.Instance.Unregister(this);
        }

        public List<Vector3> GetPositions(Vector3 originPosition, int positionCount = -1)
        {
            if (!_isShuffled)
            {
                Log.Warning("아직 Shuffle가 실행되지 않았습니다. 자동으로 ShuffleNow()를 호출합니다.");
                ShuffleNow();
            }

            if (_retrievalMode != RetrievalMode.SingleGroup && positionCount > 0 && positionCount < _cachedPositions.Count)
            {
                List<Vector3> result = new List<Vector3>(positionCount);
                for (int i = 0; i < positionCount; i++)
                {
                    result.Add(_cachedPositions[i]);
                }
                return result;
            }
            return _cachedPositions;
        }

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

        public void ShuffleNow()
        {
            _cachedPositions = new List<Vector3>();

            switch (_retrievalMode)
            {
                case RetrievalMode.None:
                    break;

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