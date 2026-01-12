using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace TeamSuneat
{
    public class CharacterStateMachine : MonoBehaviour
    {
        public CharacterState CurrentState { get; protected set; }

        protected Dictionary<CharacterState, ICharacterState> _states = new();
        protected Character _character;

        public Character Character => _character;
        private static readonly Dictionary<CharacterState, int> StatePriority = new();

        protected virtual void Awake()
        {
            _character = GetComponent<Character>();
        }

        private void Start()
        {
            InitializeStates();
        }

        protected virtual void InitializeStates()
        {
            // 기본 구현은 비어있음
            // 하위 클래스에서 오버라이드하여 상태 초기화
        }

        public virtual void LogicUpdate()
        {
            // 조건 상태일 때는 입력 처리 스킵
            if (IsConditionState(CurrentState))
            {
                if (_states.TryGetValue(CurrentState, out ICharacterState currentState))
                {
                    currentState.OnUpdate();
                }
                return;
            }

            // 이동 상태면 입력 처리
            HandleInput();

            // 현재 상태 업데이트
            if (_states.TryGetValue(CurrentState, out ICharacterState movementState))
            {
                movementState.OnUpdate();
            }
        }

        protected virtual void HandleInput()
        {
            // 기본 구현은 비어있음
            // 하위 클래스에서 오버라이드하여 입력 처리
        }

        public virtual void PhysisUpdate()
        {
            if (_states.TryGetValue(CurrentState, out ICharacterState currentState))
            {
                currentState.OnFixedUpdate();
            }
        }

        public virtual void ChangeState(CharacterState newState)
        {
            if (CurrentState == newState) return;

            // 우선순위 체크
            if (!CanChangeState(newState))
            {
                return;
            }

            // 이전 상태 Exit
            if (_states.TryGetValue(CurrentState, out ICharacterState previousState))
            {
                previousState.OnExit();
            }

            // 새 상태 Enter
            CharacterState previousStateValue = CurrentState;
            CurrentState = newState;
            if (_states.TryGetValue(CurrentState, out ICharacterState nextState))
            {
                nextState.OnEnter();
            }

            // 상태 전환 로그
            if (_character != null)
            {
                Log.Info(LogTags.CharacterState, "{0}, 상태 전환: {1} -> {2}", _character.Name.ToLogString(), previousStateValue, newState);
            }
        }

        // 상태가 자발적으로 전환을 요청할 때 사용 (OnUpdate/OnFixedUpdate 내부에서 호출)
        public virtual void TransitionToState(CharacterState newState)
        {
            if (CurrentState == newState) return;

            // 자발적 전환 요청 시 CanTransitionTo 확인
            if (!_states.TryGetValue(CurrentState, out ICharacterState currentState))
            {
                return;
            }

            // 현재 상태가 전환을 허용하는지 확인
            if (!currentState.CanTransitionTo(newState))
            {
                return;
            }

            // 자발적 전환은 우선순위 체크 건너뛰기
            // 이전 상태 Exit
            currentState.OnExit();

            // 새 상태 Enter
            CharacterState previousStateValue = CurrentState;
            CurrentState = newState;
            if (_states.TryGetValue(CurrentState, out ICharacterState nextState))
            {
                nextState.OnEnter();
            }

            // 상태 전환 로그
            if (_character != null)
            {
                Log.Info(LogTags.CharacterState, "{0}, 상태 전환: {1} -> {2}", _character.Name.ToLogString(), previousStateValue, newState);
            }
        }

        protected virtual bool CanChangeState(CharacterState newState)
        {
            // 우선순위가 높은 상태가 있으면 낮은 상태로 전환 불가
            int currentPriority = StatePriority.GetValueOrDefault(CurrentState, 0);
            int newPriority = StatePriority.GetValueOrDefault(newState, 0);

            return newPriority >= currentPriority;
        }

        protected bool IsConditionState(CharacterState state)
        {
            return state == CharacterState.Dead ||
                   state == CharacterState.Stunned ||
                   state == CharacterState.ControlledMovement;
        }

        protected void RequestJump()
        {
            // 점프 요청 처리 (상태별로 다르게 처리)
            if (_states.TryGetValue(CurrentState, out ICharacterState currentState))
            {
                currentState.OnJumpRequested();
            }
        }

        // 방향 없이 대시 요청 (캐릭터가 바라보는 방향으로 대시)
        protected virtual void RequestDash()
        {
            Vector2 direction = new Vector2(_character != null && _character.Physics != null ? _character.Physics.FacingDirection : 1f, 0f);
            RequestDash(direction);
        }

        private void RequestDash(Vector2 direction)
        {
            // 대시 요청 처리
            if (_states.TryGetValue(CurrentState, out ICharacterState currentState))
            {
                // 현재 상태가 Dash가 아니면 Dash 상태로 전환
                if (CurrentState != CharacterState.Dash)
                {
                    ChangeState(CharacterState.Dash);
                    // 실제 대시 실행은 애니메이션 이벤트에서 처리
                }
                else
                {
                    // 이미 Dash 상태이면 무시
                    currentState.OnDashRequested(direction);
                }
            }
        }

        private void OnDrawGizmos()
        {
#if UNITY_EDITOR
            // 캐릭터 위치 위에 현재 상태 표시
            Vector3 position = transform.position + Vector3.up;
            string stateText = $"State: {CurrentState}";

            // 라벨 스타일 설정
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.white;
            style.fontSize = 14;
            style.fontStyle = FontStyle.Bold;
            style.alignment = TextAnchor.MiddleCenter;

            // 라벨 표시
            Handles.Label(position, stateText, style);
#endif
        }
    }
}