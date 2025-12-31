using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TeamSuneat
{
    public class UIToggleGroup : XBehaviour
    {
        [Title("#UIToggleGroup")]
        [SerializeField]
        private ToggleGroup _toggleGroup;

        [SerializeField]
        private UIToggle[] _toggles;

        [SerializeField]
        [Tooltip("false일 경우 항상 하나의 토글이 켜져있어야 합니다.")]
        private bool _allowSwitchOff = true;

        public int ToggleCount => _toggles.Length;
        public ToggleGroup ToggleGroup => _toggleGroup;

        [Title("#UIToggleGroup-Events")]
        public UnityEvent<int> OnToggleChanged;

        private int _currentActiveIndex = -1;
        private UIToggle[] _toggleComponents => _toggles;

        private void Awake()
        {
            Initialize();
        }

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _toggleGroup = GetComponentInParent<ToggleGroup>();
            _toggles = this.GetComponentsInDirectChildren<UIToggle>();
        }

        private void Initialize()
        {
            AutoGetComponents();

            if (!ValidateToggleGroup())
            {
                return;
            }

            SetupToggleGroup();
            RegisterToggleListeners();
            SetDefaultToggle();
            Log.Info(LogTags.UI_Toggle, $"(Group) {gameObject.name} 초기화 완료. Toggle 개수: {_toggles.Length}, AllowSwitchOff: {_allowSwitchOff}");
        }

        private bool ValidateToggleGroup()
        {
            if (_toggleGroup == null)
            {
                Log.Error(LogTags.UI_Toggle, $"(Group) ToggleGroup을 찾을 수 없습니다. {gameObject.name}");
                return false;
            }

            return true;
        }

        private void SetupToggleGroup()
        {
            _toggleGroup.allowSwitchOff = _allowSwitchOff;
        }

        private void RegisterToggleListeners()
        {
            int registeredCount = 0;
            for (int i = 0; i < _toggles.Length; i++)
            {
                if (_toggles[i] == null)
                {
                    Log.Warning(LogTags.UI_Toggle, $"(Group) 인덱스 {i}의 Toggle이 null입니다.");
                    continue;
                }

                _toggles[i].SetGroup(_toggleGroup);

                int index = i;
                _toggles[i].AddListener((isOn) => OnToggleValueChanged(index, isOn));
                registeredCount++;
            }
            Log.Info(LogTags.UI_Toggle, $"(Group) {gameObject.name} 리스너 등록 완료. 등록된 Toggle: {registeredCount}/{_toggles.Length}");
        }

        private void SetDefaultToggle()
        {
            if (!_allowSwitchOff && _toggles.IsValidArray())
            {
                _toggles[0].SetIsOn(true);                
                _currentActiveIndex = 0;                
                Log.Info(LogTags.UI_Toggle, $"(Group) {gameObject.name} 기본 Toggle 설정: 인덱스 0");
            }
        }

        private void OnToggleValueChanged(int index, bool isOn)
        {
            if (_toggles[index].IsLocked || !_toggles[index].IsClickable)
            {
                return;
            }

            if (isOn)
            {
                if (_currentActiveIndex == index)
                {
                    // 같은 토글을 다시 누른 경우
                    if (_allowSwitchOff)
                    {
                        Log.Info(LogTags.UI_Toggle, $"(Group) {gameObject.name} 인덱스 {index} 토글 해제 (같은 토글 재클릭)");
                        _currentActiveIndex = -1;
                        OnToggleChanged?.Invoke(-1);
                    }
                }
                else
                {
                    // 다른 토글을 누른 경우
                    Log.Info(LogTags.UI_Toggle, $"(Group) {gameObject.name} 활성 토글 변경: {_currentActiveIndex} -> {index}");
                    _currentActiveIndex = index;
                    OnToggleChanged?.Invoke(index);
                }
            }
            else
            {
                // 토글이 꺼진 경우
                if (_currentActiveIndex == index)
                {
                    Log.Info(LogTags.UI_Toggle, $"(Group) {gameObject.name} 인덱스 {index} 토글 해제");
                    _currentActiveIndex = -1;
                    OnToggleChanged?.Invoke(-1);
                }
            }
        }

        public void SetToggle(int index, bool isOn)
        {
            if (index < 0 || index >= _toggles.Length)
            {
                Log.Warning(LogTags.UI_Toggle, $"(Group) 유효하지 않은 인덱스입니다: {index}");
                return;
            }

            if (_toggles[index] != null)
            {
                Log.Info(LogTags.UI_Toggle, $"(Group) {gameObject.name} 인덱스 {index} 토글 설정: {isOn}");
                _toggles[index].SetIsOn(isOn);
            }
        }

        public Toggle GetToggle(int index)
        {
            if (index < 0 || index >= _toggles.Length)
            {
                return null;
            }

            return _toggles[index].Toggle;
        }

        public int GetActiveToggleIndex()
        {
            return _currentActiveIndex;
        }

        public UIToggle GetUIToggle(int index)
        {
            if (index < 0 || index >= _toggleComponents.Length)
            {
                return null;
            }

            return _toggleComponents[index];
        }
    }
}