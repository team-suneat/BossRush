using Sirenix.OdinInspector;
using System;
using System.Collections;
using UnityEngine;

namespace TeamSuneat.Feedbacks
{
    /// <summary>
    /// Feedback을 정의하는 확장을 위한 기본 클래스. 피드백은 일반적으로 플레이어의 입력 또는 작업에 대한 반응으로 GameFeedbacks에 의해 트리거되는 작업입니다.
    /// 감정과 가독성을 모두 통합하여 게임 느낌을 개선하는 데 도움이 됩니다.
    /// 새 피드백을 생성하려면 이 클래스를 확장하고 이 클래스의 끝에 선언된 Custom 메서드를 재정의합니다. 많은 예를 참조로 볼 수 있습니다.
    /// </summary>

    [RequireComponent(typeof(GameFeedbacks))]
    [System.Serializable]
    [ExecuteAlways]
    public abstract class GameFeedback : XBehaviour
    {
        [FoldoutGroup("#Feedback")]
        [Tooltip("활성화 여부")]
        public bool Active = true;

        [FoldoutGroup("#Feedback")]
        [Tooltip("피드백 이름")]
        public string Label = "GameFeedback";

        [FoldoutGroup("#Feedback")]
        [SuffixLabel("발생 확률")]
        [Tooltip("퍼센트: 100: 항상 발생, 0: 발생하지 않음, 50: 두 번 호출할 때마다 발생 등")]
        [Range(0, 100)]
        public float Chance = 100f;

        [FoldoutGroup("#Feedback")]
        [Tooltip("타이밍 관련 값의 수(지연, 반복 등)")]
        public GameFeedbackTiming Timing;

        [FoldoutGroup("#Feedback")]
        [Tooltip(" GameFeedbacks의 위치를 사용합니다")]
        public bool UsingFeedbackPosition;

        /// <summary>
        /// Initialization 메서드를 호출할 때 정의된 피드백의 소유자
        /// </summary>
        public Character Owner { get; set; }

        /// <suTSary>
        /// 이 피드백이 디버그 모드인지 여부
        /// </suTSary>
        [HideInInspector]
        public bool DebugActive = false;

        /// <suTSary>
        /// 피드백이 피드백 시퀀스의 실행을 일시 중지해야 하는 경우 true로 설정하십시오.
        /// </suTSary>
        public virtual IEnumerator Pause => null;

        /// <suTSary>
        /// 이것이 사실이면 이 피드백은 모든 이전 피드백이 실행될 때까지 기다립니다.
        /// </suTSary>
        public virtual bool HoldingPause => false;

        /// <suTSary>
        /// 이것이 사실이면 이 피드백은 모든 이전 피드백이 실행될 때까지 기다린 다음 모든 이전 피드백을 다시 실행합니다.
        /// </suTSary>
        public virtual bool LooperPause => false;

        /// <suTSary>
        /// 이것이 사실이면 이 피드백은 일시 중지되고 실행을 재개하기 위해 부모 GameFeedbacks에서 Resume()이 호출될 때까지 기다립니다.
        /// </suTSary>
        public virtual bool ScriptDrivenPause { get; set; }

        /// <suTSary>
        /// 이 값이 양수이면 피드백이 스크립트를 통해 이미 재개되지 않은 경우 해당 기간 후에 자동으로 재개됩니다.
        /// </suTSary>
        public virtual float ScriptDrivenPauseAutoResume { get; set; }

        /// <suTSary>
        /// 이것이 사실이면 이 피드백은 모든 이전 피드백이 실행될 때까지 기다린 다음 모든 이전 피드백을 다시 실행합니다.
        /// </suTSary>
        public virtual bool LooperStart => false;

        /// <suTSary>
        /// 피드백에 대해 재정의할 수 있는 무시 가능한 색상입니다.
        /// 흰색은 유일하게 예약된 색상이며 흰색으로 두면 피드백이 정상(밝거나 어두운 피부)으로 돌아갑니다.
        /// </suTSary>
#if UNITY_EDITOR

        public virtual Color FeedbackColor => GameColors.CreamIvory;

#endif

        /// <suTSary>
        /// 이 피드백이 현재 쿨다운 상태이면(따라서 재생할 수 없는 경우) true를 반환하고, 그렇지 않으면 false를 반환합니다.
        /// </suTSary>
        public virtual bool InCooldown => (Timing.CooldownDuration > 0f) && (FeedbackTime - _lastPlayTimestamp < Timing.CooldownDuration);

        /// <suTSary>
        /// 이것이 사실이라면 이 피드백은 현재 재생 중입니다.
        /// </suTSary>
        public virtual bool IsPlaying { get; set; }

        /// <suTSary>
        /// 선택한 타이밍 설정을 기반으로 하는 시간(또는 스케일되지 않은 시간)
        /// </suTSary>
        public float FeedbackTime => Timing.TimescaleMode == TimescaleModes.Scaled ? Time.time : Time.unscaledTime;

        /// the delta time (or unscaled delta time) based on the selected Timing settings
        public float FeedbackDeltaTime => Timing.TimescaleMode == TimescaleModes.Scaled ? Time.deltaTime : Time.unscaledDeltaTime;

        /// <suTSary>
        /// 이 피드백의 총 지속 시간:
        /// total = 초기 지연 + 지속 시간 * (반복 횟수 + 반복 간 지연 시간)
        /// </suTSary>
        public float TotalDuration
        {
            get
            {
                if ((Timing != null) && (!Timing.ContributeToTotalDuration))
                {
                    return 0f;
                }

                float totalTime = 0f;

                if (Timing == null)
                {
                    return 0f;
                }

                if (Timing.InitialDelay != 0)
                {
                    totalTime += ApplyTimeMultiplier(Timing.InitialDelay);
                }

                totalTime += FeedbackDuration;

                if (Timing.NumberOfRepeats > 0)
                {
                    float delayBetweenRepeats = ApplyTimeMultiplier(Timing.DelayBetweenRepeats);

                    totalTime += (Timing.NumberOfRepeats * FeedbackDuration) + (Timing.NumberOfRepeats * delayBetweenRepeats);
                }

                return totalTime;
            }
        }

        /// <suTSary>
        /// 이 피드백이 마지막으로 재생된 타임스탬프
        /// </suTSary>
        public virtual float FeedbackStartedAt => _lastPlayTimestamp;

        /// <suTSary>
        /// 피드백의 인지된 기간, 진행률 표시줄을 표시하는 데 사용, 각 피드백에 의해 의미 있는 데이터로 재정의됨
        /// </suTSary>
        public virtual float FeedbackDuration
        { get => 0f; set { } }

        /// <suTSary>
        /// 이 피드백이 지금 재생 중인지 여부
        /// </suTSary>
        public virtual bool FeedbackPlaying => (FeedbackStartedAt > 0f) && (Time.time - FeedbackStartedAt < FeedbackDuration);

        protected float _lastPlayTimestamp = -1f;
        protected int _playsLeft;
        protected bool _initialized = false;
        protected Coroutine _playCoroutine;
        protected Coroutine _infinitePlayCoroutine;
        protected Coroutine _sequenceCoroutine;
        protected Coroutine _repeatedPlayCoroutine;
        protected int _sequenceTrackID = 0;
        protected GameFeedbacks _hostGameFeedbacks;

        protected float _beatInterval;
        protected bool BeatThisFrame = false;
        protected int LastBeatIndex = 0;
        protected int CurrentSequenceIndex = 0;
        protected float LastBeatTimestamp = 0f;
        protected bool _isHostGameFeedbacksNotNull;

        protected override void OnEnabled()
        {
            base.OnEnabled();

            _hostGameFeedbacks = gameObject.GetComponent<GameFeedbacks>();
            _isHostGameFeedbacksNotNull = _hostGameFeedbacks != null;
        }

        /// <suTSary>
        ///피드백 및 타이밍 관련 변수를 초기화합니다.
        /// </suTSary>
        /// <param name="owner"></param>
        public virtual void Initialization(GameObject owner)
        {
            _initialized = true;

            Owner = null;

            _playsLeft = Timing.NumberOfRepeats + 1;
            _hostGameFeedbacks = gameObject.GetComponent<GameFeedbacks>();

            SetInitialDelay(Timing.InitialDelay);
            SetDelayBetweenRepeats(Timing.DelayBetweenRepeats);
            SetSequence(Timing.Sequence);

            CustomInitialization(owner);
        }

        /// <suTSary>
        ///피드백 및 타이밍 관련 변수를 초기화합니다.
        /// </suTSary>
        /// <param name="owner"></param>
        public virtual void Initialization(Character owner)
        {
            _initialized = true;

            Owner = owner;

            _playsLeft = Timing.NumberOfRepeats + 1;
            _hostGameFeedbacks = gameObject.GetComponent<GameFeedbacks>();

            SetInitialDelay(Timing.InitialDelay);
            SetDelayBetweenRepeats(Timing.DelayBetweenRepeats);
            SetSequence(Timing.Sequence);

            if (owner != null)
            {
                CustomInitialization(owner.gameObject);
            }
        }

        /// <suTSary>
        /// 피드백을 재생합니다
        /// </suTSary>
        public virtual void Play(Vector3 feedbackPosition, int index, float feedbacksIntensity = 1.0f)
        {
            if (!Active)
            {
                return;
            }
            if (!_initialized)
            {
                Debug.LogWarningFormat("[{0}] Feedback is playing without being initialized. Call the Initialization() function first.", this.GetHierarchyPath());
            }

            if (InCooldown)
            {
                return;
            }

            if (Timing.InitialDelay > 0f)
            {
                _playCoroutine = StartCoroutine(PlayCoroutine(feedbackPosition, index, feedbacksIntensity));
            }
            else
            {
                _lastPlayTimestamp = FeedbackTime;
                RegularPlay(feedbackPosition, index, feedbacksIntensity);
            }
        }

        /// <suTSary>
        /// 피드백의 초기 재생을 지연시키는 내부 코루틴
        /// </suTSary>
        protected virtual IEnumerator PlayCoroutine(Vector3 feedbackPosition, int index, float feedbacksIntensity = 1.0f)
        {
            yield return Timing.TimescaleMode == TimescaleModes.Scaled
            ? new WaitForSeconds(Timing.InitialDelay)
            : new WaitForSecondsRealtime(Timing.InitialDelay);

            _lastPlayTimestamp = FeedbackTime;
            RegularPlay(feedbackPosition, index, feedbacksIntensity);
        }

        /// <suTSary>
        /// 필요한 경우 지연 코루틴을 트리거합니다.
        /// </suTSary>
        protected virtual void RegularPlay(Vector3 feedbackPosition, int index, float feedbacksIntensity = 1.0f)
        {
            if (Chance == 0f)
            {
                return;
            }

            if (Chance != 100f)
            {
                float random = RandomEx.Range(0f, 100f);
                if (random > Chance)
                {
                    return;
                }
            }

            if (Timing.UseIntensityInterval)
            {
                if ((feedbacksIntensity < Timing.IntensityIntervalMin) || (feedbacksIntensity >= Timing.IntensityIntervalMax))
                {
                    return;
                }
            }

            if (Timing.RepeatForever)
            {
                _infinitePlayCoroutine = StartCoroutine(InfinitePlay(feedbackPosition, index, feedbacksIntensity));
                return;
            }
            if (Timing.NumberOfRepeats > 0)
            {
                _repeatedPlayCoroutine = StartCoroutine(RepeatedPlay(feedbackPosition, index, feedbacksIntensity));
                return;
            }
            if (Timing.Sequence == null)
            {
                CustomPlayFeedback(feedbackPosition, index, feedbacksIntensity);
            }
            else
            {
                _sequenceCoroutine = StartCoroutine(SequenceCoroutine(feedbackPosition, index, feedbacksIntensity));
            }
        }

        /// <suTSary>
        /// 끝이 없는 반복 재생에 사용되는 내부 코루틴
        /// </suTSary>
        protected virtual IEnumerator InfinitePlay(Vector3 position, int index, float feedbacksIntensity = 1.0f)
        {
            while (true)
            {
                _lastPlayTimestamp = FeedbackTime;
                if (Timing.Sequence == null)
                {
                    CustomPlayFeedback(position, index, feedbacksIntensity);
                    yield return Timing.TimescaleMode == TimescaleModes.Scaled
                    ? new WaitForSeconds(Timing.DelayBetweenRepeats)
                    : new WaitForSecondsRealtime(Timing.DelayBetweenRepeats);
                }
                else
                {
                    _sequenceCoroutine = StartCoroutine(SequenceCoroutine(position, index, feedbacksIntensity));

                    float delay = ApplyTimeMultiplier(Timing.DelayBetweenRepeats) + Timing.Sequence.Length;
                    yield return Timing.TimescaleMode == TimescaleModes.Scaled
                    ? new WaitForSeconds(delay)
                    : new WaitForSecondsRealtime(delay);
                }
            }
        }

        /// <suTSary>
        /// 반복 재생에 사용되는 내부 코루틴
        /// </suTSary>
        /// <param name="position"></param>
        /// <param name="feedbacksIntensity"></param>
        /// <returns></returns>
        protected virtual IEnumerator RepeatedPlay(Vector3 position, int index, float feedbacksIntensity = 1.0f)
        {
            while (_playsLeft > 0)
            {
                _lastPlayTimestamp = FeedbackTime;
                _playsLeft--;
                if (Timing.Sequence == null)
                {
                    CustomPlayFeedback(position, index, feedbacksIntensity);

                    yield return Timing.TimescaleMode == TimescaleModes.Scaled
                    ? new WaitForSeconds(Timing.DelayBetweenRepeats)
                    : new WaitForSecondsRealtime(Timing.DelayBetweenRepeats);
                }
                else
                {
                    _sequenceCoroutine = StartCoroutine(SequenceCoroutine(position, index, feedbacksIntensity));

                    float delay = ApplyTimeMultiplier(Timing.DelayBetweenRepeats) + Timing.Sequence.Length;
                    yield return Timing.TimescaleMode == TimescaleModes.Scaled
                    ? new WaitForSeconds(delay)
                    : new WaitForSecondsRealtime(delay);
                }
            }
            _playsLeft = Timing.NumberOfRepeats + 1;
        }

        /// <suTSary>
        /// 시퀀스에서 이 피드백을 재생하는 데 사용되는 코루틴
        /// </suTSary>
        /// <param name="position"></param>
        /// <param name="feedbacksIntensity"></param>
        /// <returns></returns>
        protected virtual IEnumerator SequenceCoroutine(Vector3 position, int index, float feedbacksIntensity = 1.0f)
        {
            yield return null;
            float timeStartedAt = FeedbackTime;
            float lastFrame = FeedbackTime;

            BeatThisFrame = false;
            LastBeatIndex = 0;
            CurrentSequenceIndex = 0;
            LastBeatTimestamp = 0f;

            if (Timing.Quantized)
            {
                while (CurrentSequenceIndex < Timing.Sequence.QuantizedSequence[0].Line.Count)
                {
                    _beatInterval = 60f / Timing.TargetBPM;

                    if ((FeedbackTime - LastBeatTimestamp >= _beatInterval) || (LastBeatTimestamp == 0f))
                    {
                        BeatThisFrame = true;
                        LastBeatIndex = CurrentSequenceIndex;
                        LastBeatTimestamp = FeedbackTime;

                        for (int i = 0; i < Timing.Sequence.SequenceTracks.Count; i++)
                        {
                            if (Timing.Sequence.QuantizedSequence[i].Line[CurrentSequenceIndex].ID == Timing.TrackID)
                            {
                                CustomPlayFeedback(position, index, feedbacksIntensity);
                            }
                        }
                        CurrentSequenceIndex += 1;
                    }
                    yield return null;
                }
            }
            else
            {
                while (FeedbackTime - timeStartedAt < Timing.Sequence.Length)
                {
                    foreach (GameFeedbackSequenceNote item in Timing.Sequence.OriginalSequence.Line)
                    {
                        if ((item.ID == Timing.TrackID) && (item.Timestamp >= lastFrame) && (item.Timestamp <= FeedbackTime - timeStartedAt))
                        {
                            CustomPlayFeedback(position, index, feedbacksIntensity);
                        }
                    }
                    lastFrame = FeedbackTime - timeStartedAt;
                    yield return null;
                }
            }
        }

        /// <suTSary>
        /// 모든 피드백 재생을 중지합니다. 피드백 반복을 중지하고 맞춤형 중지 구현을 호출합니다.
        /// </suTSary>
        /// <param name="position"></param>
        /// <param name="feedbacksIntensity"></param>
        public virtual void Stop(Vector3 position, float feedbacksIntensity = 1.0f)
        {
            if (_playCoroutine != null) { StopCoroutine(_playCoroutine); }
            if (_infinitePlayCoroutine != null) { StopCoroutine(_infinitePlayCoroutine); }
            if (_repeatedPlayCoroutine != null) { StopCoroutine(_repeatedPlayCoroutine); }
            if (_sequenceCoroutine != null) { StopCoroutine(_sequenceCoroutine); }

            _lastPlayTimestamp = 0f;
            _playsLeft = Timing.NumberOfRepeats + 1;
            if (Timing.InterruptsOnStop)
            {
                CustomStopFeedback(position, feedbacksIntensity);
            }
        }

        /// <suTSary>
        /// 이 피드백의 사용자 지정 재설정이라고 합니다.
        /// </suTSary>
        public virtual void ResetFeedback()
        {
            _playsLeft = Timing.NumberOfRepeats + 1;
            CustomReset();
        }

        /// <suTSary>
        /// 런타임에 이 피드백의 순서를 변경하려면 이 방법을 사용하십시오.
        /// </suTSary>
        /// <param name="newSequence"></param>
        public virtual void SetSequence(GameFeedbackSequence newSequence)
        {
            Timing.Sequence = newSequence;
            if (Timing.Sequence != null)
            {
                for (int i = 0; i < Timing.Sequence.SequenceTracks.Count; i++)
                {
                    if (Timing.Sequence.SequenceTracks[i].ID == Timing.TrackID)
                    {
                        _sequenceTrackID = i;
                    }
                }
            }
        }

        /// <suTSary>
        /// 런타임 시 반복 사이에 새로운 지연을 지정하려면 이 방법을 사용하십시오.
        /// </suTSary>
        /// <param name="delay"></param>
        public virtual void SetDelayBetweenRepeats(float delay)
        {
            Timing.DelayBetweenRepeats = delay;
        }

        /// <suTSary>
        /// 런타임 시 새로운 초기 지연을 지정하려면 이 방법을 사용하십시오.
        /// </suTSary>
        /// <param name="delay"></param>
        public virtual void SetInitialDelay(float delay)
        {
            Timing.InitialDelay = delay;
        }

        /// <suTSary>
        /// 이 피드백의 현재 재생 방향을 기반으로 정규화된 시간의 새 값을 반환합니다.
        /// </suTSary>
        /// <param name="normalizedTime"></param>
        /// <returns></returns>
        protected virtual float ApplyDirection(float normalizedTime)
        {
            return NormalPlayDirection ? normalizedTime : 1 - normalizedTime;
        }

        /// <suTSary>
        /// 이 피드백이 정상적으로 재생되어야 하는 경우 true를 반환하고 되감기에서 재생되어야 하는 경우 false를 반환합니다.
        /// </suTSary>
        public virtual bool NormalPlayDirection => Timing.PlayDirection switch
        {
            GameFeedbackTiming.PlayDirections.FollowGameFeedbacksDirection => _hostGameFeedbacks.Direction == GameFeedbacks.Directions.TopToBottom,
            GameFeedbackTiming.PlayDirections.AlwaysNormal => true,
            GameFeedbackTiming.PlayDirections.AlwaysRewind => false,
            GameFeedbackTiming.PlayDirections.OppositeGameFeedbacksDirection => !(_hostGameFeedbacks.Direction == GameFeedbacks.Directions.TopToBottom),
            _ => true,
        };

        /// <suTSary>
        /// GameFeedbacksDirectionCondition 설정에 따라 이 피드백이 현재 상위 GameFeedbacks 방향에서 재생되어야 하는 경우 true를 반환합니다.
        /// </suTSary>
        public virtual bool ShouldPlayInThisSequenceDirection => Timing.GameFeedbacksDirectionCondition switch
        {
            GameFeedbackTiming.GameFeedbacksDirectionConditions.Always => true,
            GameFeedbackTiming.GameFeedbacksDirectionConditions.OnlyWhenForwards => _hostGameFeedbacks.Direction == GameFeedbacks.Directions.TopToBottom,
            GameFeedbackTiming.GameFeedbacksDirectionConditions.OnlyWhenBackwards => _hostGameFeedbacks.Direction == GameFeedbacks.Directions.BottomToTop,
            _ => true,
        };

        /// <suTSary>
        /// 이 피드백의 재생 시간이 끝날 때 곡선을 평가할 t 값을 반환합니다.
        /// </suTSary>
        protected virtual float FinalNormalizedTime => NormalPlayDirection ? 1f : 0f;

        /// <suTSary>
        /// 이 피드백에 호스트 GameFeedbacks의 시간 배율을 적용합니다.
        /// </suTSary>
        /// <param name="duration"></param>
        /// <returns></returns>
        protected virtual float ApplyTimeMultiplier(float duration)
        {
            return _isHostGameFeedbacksNotNull ? _hostGameFeedbacks.ApplyTimeMultiplier(duration) : duration;
        }

        /// <suTSary>
        /// 이 방법은 기본 초기화 방법 외에도 피드백에 필요한 모든 사용자 정의 초기화 프로세스를 설명합니다.
        /// </suTSary>
        /// <param name="owner"></param>
        protected virtual void CustomInitialization(GameObject owner)
        { }

        /// <suTSary>
        /// 이 방법은 피드백이 재생될 때 발생하는 일을 설명합니다.
        /// </suTSary>
        /// <param name="position"></param>
        /// <param name="feedbacksIntensity"></param>
        protected abstract void CustomPlayFeedback(Vector3 position, int index, float feedbacksIntensity = 1.0f);

        /// <suTSary>
        /// 이 메서드는 피드백이 중지되면 어떻게 되는지 설명합니다.
        /// </suTSary>
        /// <param name="position"></param>
        /// <param name="feedbacksIntensity"></param>
        protected virtual void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1.0f)
        { }

        /// <suTSary>
        /// 이 메서드는 피드백이 재설정되면 어떻게 되는지 설명합니다.
        /// </suTSary>
        protected virtual void CustomReset()
        { }
    }
}