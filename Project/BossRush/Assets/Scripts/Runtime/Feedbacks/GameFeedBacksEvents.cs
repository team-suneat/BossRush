using System;
using UnityEngine;
using UnityEngine.Events;

namespace TeamSuneat.Feedbacks
{
    /// <summary>
    /// 일련의 피드백을 재생할 때 GameFeedbacks에 의해 트리거되는 이벤트
    /// - play : GameFeedbacks가 재생을 시작할 때
    /// - pause : 홀딩 멈춤이 충족되었을 때
    /// - resume : 보류 일시 중지가 다시 시작된 후
    /// - revert : GameFeedbacks가 재생 방향을 되돌릴 때
    /// - complete : GameFeedbacks가 마지막 피드백을 재생했을 때
    ///
    /// 이러한 이벤트를 수신하려면 다음을 수행합니다.
    ///
    /// public virtual void OnGameFeedbacksEvent(GameFeedbacks source, EventTypes type)
    /// {
    ///     // do something
    /// }
    ///
    /// protected virtual void OnEnable()
    /// {
    ///     GameFeedbacksEvent.Register(OnGameFeedbacksEvent);
    /// }
    ///
    /// protected virtual void OnDisable()
    /// {
    ///     GameFeedbacksEvent.Unregister(OnGameFeedbacksEvent);
    /// }
    ///
    /// </summary>
    public struct GameFeedbacksEvent
    {
        public enum EventTypes
        { Play, Pause, Resume, Revert, Complete, Skip }

        public delegate void Delegate(GameFeedbacks source, EventTypes type);

        private static event Delegate OnEvent;

        public static void Register(Delegate callback)
        {
            OnEvent += callback;
        }

        public static void Unregister(Delegate callback)
        {
            OnEvent -= callback;
        }

        public static void Trigger(GameFeedbacks source, EventTypes type)
        {
            OnEvent?.Invoke(source, type);
        }
    }

    /// <summary>
    ///GameFeedbacks의 하위 클래스에는 재생할 수 있는 UnityEvents가 포함되어 있습니다.
    /// </summary>
    [Serializable]
    public class GameFeedbacksEvents
    {
        [Tooltip("이 GameFeedbacks가 GameFeedbacksEvents를 발생시켜야 하는지 여부")]
        public bool TriggerGameFeedbacksEvents = false;

        [Tooltip("이 GameFeedback이 Unity 이벤트를 발생시켜야 하는지 여부")]
        public bool TriggerUnityEvents = true;

        [Tooltip("이 이벤트는 이 GameFeedbacks가 재생될 때마다 실행됩니다.")]
        public UnityEvent OnPlay;

        [Tooltip("이 이벤트는 이 GameFeedbacks가 보류 일시 중지를 시작할 때마다 실행됩니다.")]
        public UnityEvent OnPause;

        [Tooltip("이 이벤트는 일시 중지 후 이 GameFeedbacks가 재개될 때마다 시작됩니다.")]
        public UnityEvent OnResume;

        [Tooltip("이 이벤트는 이 GameFeedbacks가 재생 방향을 되돌릴 때마다 시작됩니다.")]
        public UnityEvent OnRevert;

        [Tooltip("이 이벤트는 이 GameFeedbacks가 마지막 GameFeedback을 재생할 때마다 실행됩니다.")]
        public UnityEvent OnComplete;

        public bool OnPlayIsNull { get; protected set; }

        public bool OnPauseIsNull { get; protected set; }

        public bool OnResumeIsNull { get; protected set; }

        public bool OnRevertIsNull { get; protected set; }

        public bool OnCompleteIsNull { get; protected set; }

        /// <summary>
        /// init에서 호출할 이벤트가 있는지 여부에 관계없이 각 이벤트에 대해 저장합니다.
        /// </summary>
        public virtual void Initialization()
        {
            OnPlayIsNull = OnPlay == null;
            OnPauseIsNull = OnPause == null;
            OnResumeIsNull = OnResume == null;
            OnRevertIsNull = OnRevert == null;
            OnCompleteIsNull = OnComplete == null;
        }

        /// <summary>
        /// 필요한 경우 Play 이벤트를 발생시킵니다.
        /// </summary>
        /// <param name="source"></param>
        public virtual void TriggerOnPlay(GameFeedbacks source)
        {
            if (!OnPlayIsNull && TriggerUnityEvents)
            {
                OnPlay.Invoke();
            }

            if (TriggerGameFeedbacksEvents)
            {
                GameFeedbacksEvent.Trigger(source, GameFeedbacksEvent.EventTypes.Play);
            }
        }

        /// <summary>
        /// 필요한 경우 일시 중지 이벤트 발생
        /// </summary>
        /// <param name="source"></param>
        public virtual void TriggerOnPause(GameFeedbacks source)
        {
            if (!OnPauseIsNull && TriggerUnityEvents)
            {
                OnPause.Invoke();
            }

            if (TriggerGameFeedbacksEvents)
            {
                GameFeedbacksEvent.Trigger(source, GameFeedbacksEvent.EventTypes.Pause);
            }
        }

        /// <summary>
        /// 필요한 경우 건너뛰기 이벤트 발생
        /// </summary>
        /// <param name="source"></param>
        public virtual void TriggerOnSkip(GameFeedbacks source)
        {
            if (!OnPauseIsNull && TriggerUnityEvents)
            {
                OnPause.Invoke();
            }

            if (TriggerGameFeedbacksEvents)
            {
                GameFeedbacksEvent.Trigger(source, GameFeedbacksEvent.EventTypes.Skip);
            }
        }

        /// <summary>
        /// 필요한 경우 재개 이벤트 발생시킵니다.
        /// </summary>
        /// <param name="source"></param>
        public virtual void TriggerOnResume(GameFeedbacks source)
        {
            if (!OnResumeIsNull && TriggerUnityEvents)
            {
                OnResume.Invoke();
            }

            if (TriggerGameFeedbacksEvents)
            {
                GameFeedbacksEvent.Trigger(source, GameFeedbacksEvent.EventTypes.Resume);
            }
        }

        /// <summary>
        ///필요한 경우 되돌리기 이벤트를 발생시킵니다.
        /// </summary>
        /// <param name="source"></param>
        public virtual void TriggerOnRevert(GameFeedbacks source)
        {
            if (!OnRevertIsNull && TriggerUnityEvents)
            {
                OnRevert.Invoke();
            }

            if (TriggerGameFeedbacksEvents)
            {
                GameFeedbacksEvent.Trigger(source, GameFeedbacksEvent.EventTypes.Revert);
            }
        }

        /// <summary>
        /// 필요한 경우 완료 이벤트를 발생시킵니다.
        /// </summary>
        /// <param name="source"></param>
        public virtual void TriggerOnComplete(GameFeedbacks source)
        {
            if (!OnCompleteIsNull && TriggerUnityEvents)
            {
                OnComplete.Invoke();
            }

            if (TriggerGameFeedbacksEvents)
            {
                GameFeedbacksEvent.Trigger(source, GameFeedbacksEvent.EventTypes.Complete);
            }
        }
    }
}