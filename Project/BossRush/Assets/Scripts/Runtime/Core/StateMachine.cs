using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace TeamSuneat
{
    public struct StateChangeEvent<T> where T : struct, IComparable, IConvertible, IFormattable
    {
        public StateMachine<T> TargetStateMachine;
        public T NewState;
        public T PreviousState;

        public StateChangeEvent(StateMachine<T> stateMachine)
        {
            TargetStateMachine = stateMachine;
            NewState = stateMachine.CurrentState;
            PreviousState = stateMachine.PreviousState;
        }
    }

    public interface IStateMachine
    {
        bool TriggerEvents { get; set; }
    }

    [System.Serializable]
    public class StateMachine<T> : IStateMachine where T : struct, IComparable, IConvertible, IFormattable
    {
        public bool TriggerEvents { get; set; }

        [ReadOnly] public string PreviousStateString;
        [ReadOnly] public string CurrentStateString;

        public T CurrentState { get; protected set; }

        public T PreviousState { get; protected set; }

        public delegate void OnStateChangeDelegate();

        public OnStateChangeDelegate OnStateChange;

        public StateMachine(bool triggerEvents)
        {
            this.TriggerEvents = triggerEvents;
        }

        public bool Compare(T state)
        {
            if (EqualityComparer<T>.Default.Equals(state, CurrentState))
            {
                return true;
            }

            return false;
        }

        public virtual void ChangeState(T newState)
        {
            // "새 상태"가 현재 상태이면 아무 것도 하지 않고 종료합니다.
            if (Compare(newState))
            {
                return;
            }

            // 이전 캐릭터의 움직임 상태를 저장합니다
            PreviousState = CurrentState;
            CurrentState = newState;

            CurrentStateString = CurrentState.ToString();
            PreviousStateString = PreviousState.ToString();

            if (OnStateChange != null)
            {
                OnStateChange.Invoke();
            }
        }

        public virtual bool And(params T[] args)
        {
            if (!args.Equals(default))
            {
                for (int i = 0; i < args.Length; i++)
                {
                    if (!CurrentState.Equals(args[i]))
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        public virtual bool Or(params T[] args)
        {
            if (!args.Equals(default))
            {
                for (int i = 0; i < args.Length; i++)
                {
                    if (CurrentState.Equals(args[i]))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public virtual void RestorePreviousState()
        {
            // 이전 상태를 복원합니다
            CurrentState = PreviousState;

            OnStateChange?.Invoke();
        }
    }
}