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

        protected Dictionary<CharacterState, ICharacterState> _states;
        protected Character _character;

        // 상태 우선순위 (높을수록 우선)
        private static readonly Dictionary<CharacterState, int> StatePriority = new()
        {
            { CharacterState.Dead, 100 },
            { CharacterState.Stunned, 80 },
            { CharacterState.ControlledMovement, 40 },
            { CharacterState.Dash, 30 },
            { CharacterState.Jumping, 20 },
            { CharacterState.Falling, 20 },
            { CharacterState.Walk, 10 },
            { CharacterState.Idle, 0 },
        };

        protected virtual void Awake()
        {
            _character = GetComponent<Character>();
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
            // 현재 상태 FixedUpdate
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
            CurrentState = newState;
            if (_states.TryGetValue(CurrentState, out ICharacterState nextState))
            {
                nextState.OnEnter();
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

        public void RequestJump()
        {
            // 점프 요청 처리 (상태별로 다르게 처리)
            if (_states.TryGetValue(CurrentState, out ICharacterState currentState))
            {
                currentState.OnJumpRequested();
            }
        }

        public virtual void RequestDash(Vector2 direction)
        {
            // 대시 요청 처리
            if (_states.TryGetValue(CurrentState, out ICharacterState currentState))
            {
                // 현재 상태가 Dash가 아니면 Dash 상태로 전환
                if (CurrentState != CharacterState.Dash)
                {
                    ChangeState(CharacterState.Dash);
                    // Dash 상태로 전환 후 실제 대시 실행은 하위 클래스에서 처리
                    ExecuteDash(direction);
                }
                else
                {
                    // 이미 Dash 상태이면 무시
                    currentState.OnDashRequested(direction);
                }
            }
        }

        protected virtual void ExecuteDash(Vector2 direction)
        {
            // 기본 구현은 비어있음
            // 하위 클래스에서 오버라이드하여 실제 물리 시스템에 대시 요청
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