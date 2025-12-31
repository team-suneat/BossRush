using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat.Feedbacks
{
    public partial class GameFeedbacks : XBehaviour
    {
        /// <summery>
        /// GameFeedbacks를 재생할 수 있는 가능한 방향
        /// </summery>
        public enum Directions
        {
            TopToBottom, BottomToTop
        }

        /// <summary>
        /// 가능한 SafeModes(직렬화 오류로 인해 손상되었는지 확인하기 위해 검사를 수행함)
        /// - 아니요 : 안전하지 않음
        /// - 편집기 전용: 활성화 시 검사를 수행합니다.
        /// - 런타임 전용: Awake에서 검사를 수행합니다.
        /// - full : 편집기와 런타임 검사를 모두 수행합니다. 권장 설정입니다.
        /// /// </summary>
        public enum SafeModes
        {
            Nope, EditorOnly, RuntimeOnly, Full
        }

        /// <summary>
        /// 가능한 초기화 모드. Script를 사용하는 경우 Initialization 메서드를 호출하고 소유자를 전달하여 수동으로 초기화해야 합니다.
        /// 그렇지 않으면 이 구성 요소가 Awake 또는 Start에서 초기화되도록 할 수 있으며 이 경우 소유자는 GameFeedbacks 자체가 됩니다.
        /// </summary>
        public enum InitializationModes
        {
            Script, Awake, Start
        }

        [Tooltip("선택한 초기화 모드. 스크립트를 사용하는 경우 " +
         "초기화 방법 및 소유자 전달. 그렇지 않으면 이 구성 요소를 초기화할 수 있습니다." +
         "Awake 또는 Start에 있으며 이 경우 소유자는 GameFeedbacks 자체가 됩니다.")]
        [SuffixLabel("초기화 모드")]
        public InitializationModes InitializationMode = InitializationModes.Start;

        [SuffixLabel("선택한 안전 모드")]
        public SafeModes SafeMode = SafeModes.Full;

        [Tooltip("모든 피드백을 전역적으로 켜거나 끄는 데 사용되는 전역 스위치")]
        [SuffixLabel("전역 스위치")]
        public static bool GlobalGameFeedbacksActive = true;

        [Tooltip("이러한 피드백이 재생되어야 하는 선택된 방향")]
        public Directions Direction = Directions.TopToBottom;

        [Tooltip("모든 피드백이 재생되었을 때 이 GameFeedbacks의 방향을 반전해야 하는지 여부")]
        public bool AutoChangeDirectionOnEnd = false;

        [Tooltip("시작 시 이 피드백을 자동으로 재생할지 여부")]
        public bool AutoPlayOnStart = false;

        [Tooltip("활성화 시 이 피드백을 자동으로 재생할지 여부")]
        public bool AutoPlayOnEnable = false;

        [Tooltip("트리거할 GameFeedback 목록")]
        private GameFeedback[] _feedbacks;

        [Tooltip("이 피드백을 재생하는 강도입니다. 이 값은 대부분의 피드백에서 진폭(amplitude)을 조정하는 데 사용됩니다. 1은 정상, 0.5는 절반 전력, 0은 효과가 없습니다." +
        "이 값이 제어하는 것은 피드백에서 피드백에 따라 다르므로 주저하지 말고 코드를 확인하여 정확히 무엇을 하는지 확인하십시오.")]
        public float FeedbacksIntensity = 1f;

        [Tooltip("모든 피드백 지속 시간(초기 지연, 지속 시간, 반복 사이의 지연...)에 적용될 시간 승수")]
        public float DurationMultiplier = 1f;

        [Tooltip("이것이 사실이면 지속 시간 슬롯의 피드백별로 더 많은 편집자 전용 세부 정보가 표시됩니다.")]
        public bool DisplayFullDurationDetails = false;

        [Tooltip("한 번 재생된 후 이 GameFeedbacks의 새 재생을 트리거하는 것이 불가능한 기간(초)")]
        public float CooldownDuration = 0f;

        [Tooltip("이 GameFeedbacks 콘텐츠 재생의 시작을 지연하는 지속 시간(초)")]
        public float InitialDelay = 0f;

        [Tooltip("이것이 사실이면 이 피드백이 이미 재생되는 동안 새 재생을 트리거할 수 있습니다")]
        public bool CanPlayWhileAlreadyPlaying = true;

        [Tooltip("이 시퀀스가 발생할 확률(백분율: 100: 항상 발생, 0: 발생하지 않음, 50: 두 번 호출할 때마다 발생 등)")]
        [Range(0, 100)]
        public float ChanceToPlay = 100f;

        [Tooltip("이 GameFeedbacks의 다양한 단계에서 트리거될 수 있는 여러 UnityEvents")]
        public GameFeedbacksEvents Events;

        /// <summery>
        /// 이 GameFeedbacks가 지금 재생 중인지 여부 - 아직 중지되지 않았음을 의미합니다.
        /// GameFeedbacks를 중지하지 않으면 물론 true로 유지됩니다.
        /// </summery>
        public bool IsPlaying { get; protected set; }

        /// <summery>
        /// 이 GameFeedbacks가 재생된 횟수
        /// </summery>
        public int TimesPlayed { get; protected set; }

        /// <summery>
        /// 이 GameFeedbacks 시퀀스의 실행이 방지되고 Resume() 호출을 기다리는지 여부
        /// </summery>
        public bool InScriptDrivenPause { get; set; }

        /// <summary>
        /// 이 피드백이 다음에 재생될 때 재생 방향을 변경해야 하는 경우 true
        /// </summary>
        public bool ShouldRevertOnNextPlay { get; set; }

        /// <summery>
        /// 이 GameFeedbacks에 있는 모든 활성 피드백의 총 지속 시간(초)
        /// </summery>
        public virtual float TotalDuration
        {
            get
            {
                float total = 0f;
                for (int i = 0; i < _feedbacks.Length; i++)
                {
                    if (_feedbacks[i] != null && _feedbacks[i].Active)
                    {
                        if (total < _feedbacks[i].TotalDuration)
                        {
                            total = _feedbacks[i].TotalDuration;
                        }
                    }
                }
                return InitialDelay + total;
            }
        }

        protected float _startTime = 0f;
        protected float _holdingMax = 0f;
        protected float _lastStartAt = -float.MaxValue;
        protected bool _pauseFound = false;
        protected float _totalDuration = 0f;
        protected bool _shouldStop = false;

        protected void Awake()
        {
            _feedbacks = GetComponentsInChildren<GameFeedback>();

            if ((InitializationMode == InitializationModes.Awake) && (Application.isPlaying))
            {
                Initialization(this.FindFirstParentComponent<Character>());
            }
        }

        protected override void OnStart()
        {
            base.OnStart();

            if ((InitializationMode == InitializationModes.Start) && (Application.isPlaying))
            {
                Initialization(this.FindFirstParentComponent<Character>());
            }

            if (AutoPlayOnStart && Application.isPlaying)
            {
                PlayFeedbacks();
            }
        }

        public virtual void Initialization()
        {
            Initialization(this.FindFirstParentComponent<Character>());
        }

        public virtual void Initialization(GameObject owner)
        {
            IsPlaying = false;
            TimesPlayed = 0;
            _lastStartAt = -float.MaxValue;

            for (int i = 0; i < _feedbacks.Length; i++)
            {
                if (_feedbacks[i] != null)
                {
                    _feedbacks[i].Initialization(owner);
                }
            }
        }

        /// <summary>
        /// 피드백에 의해 위치 및 계층에 대한 참조로 사용될 소유자를 지정하여 피드백을 초기화하는 공개 메소드
        /// </summary>

        public virtual void Initialization(Character owner)
        {
            IsPlaying = false;
            TimesPlayed = 0;
            _lastStartAt = -float.MaxValue;

            if (_feedbacks.IsValid())
            {
                for (int i = 0; i < _feedbacks.Length; i++)
                {
                    if (_feedbacks[i] != null)
                    {
                        _feedbacks[i].Initialization(owner);
                    }
                }
            }
        }

        /// <summery>
        /// GameFeedbacks의 위치를 참조로 사용하여 모든 피드백을 재생하고 감쇠는 없습니다.
        /// </summery>
        public virtual void PlayFeedbacks()
        {
            PlayFeedbacksInternal(transform.position, 0, FeedbacksIntensity);
        }

        /// <summery>
        /// 위치와 강도를 지정하여 모든 피드백을 재생합니다. 위치는 각 피드백에서 사용할 수 있으며 예를 들어 입자를 스파크하거나 사운드를 재생하는 데 고려할 수 있습니다.
        /// 피드백 강도는 강도를 낮추기 위해 각 피드백에서 사용할 수 있는 요소입니다. 일반적으로 시간이나 거리에 따라 감쇠를 정의하려고 할 것입니다(더 낮은
        /// 플레이어에서 더 멀리 떨어진 곳에서 발생하는 피드백에 대한 강도 값).
        /// 또한 현재 상태를 무시하고 피드백이 반대로 재생되도록 할 수 있습니다.
        /// </summery>
        /// <param name="feedbackPosition"></param>
        /// <param name="feedbacksOwner"></param>
        /// <param name="feedbacksIntensity"></param>
        public virtual void PlayFeedbacks(Vector3 feedbackPosition, int index, float feedbacksIntensity = 1.0f, bool forceRevert = false)
        {
            PlayFeedbacksInternal(feedbackPosition, index, feedbacksIntensity, forceRevert);
        }

        /// <summery>
        /// 피드백을 재생하는 데 사용되는 내부 메서드, 외부에서 호출하면 안 됩니다.
        /// </summery>
        /// <param name="feedbackPosition"></param>
        /// <param name="feedbacksIntensity"></param>
        protected virtual void PlayFeedbacksInternal(Vector3 feedbackPosition, int index, float feedbacksIntensity, bool forceRevert = false)
        {
            if (IsPlaying && !CanPlayWhileAlreadyPlaying)
            {
                return;
            }

            if (!EvaluateChance())
            {
                return;
            }

            // 재사용 대기시간이 있는 경우 필요한 경우 실행을 방지합니다.
            if (CooldownDuration > 0f)
            {
                if (Time.unscaledTime - _lastStartAt < CooldownDuration)
                {
                    return;
                }
            }

            // 모든 GameFeedbacks가 전역적으로 비활성화되면 중지하고 재생하지 않습니다.
            if (!GlobalGameFeedbacksActive)
            {
                return;
            }

            if (!ActiveInHierarchy)
            {
                return;
            }

            ResetFeedbacks();
            enabled = true;
            TimesPlayed += 1;
            IsPlaying = true;

            _startTime = Time.unscaledTime;
            _lastStartAt = _startTime;
            _totalDuration = TotalDuration;

            if (InitialDelay > 0f)
            {
                _ = StartCoroutine(HandleInitialDelayCo(feedbackPosition, index, feedbacksIntensity, forceRevert));
            }
            else
            {
                PreparePlay(feedbackPosition, index, feedbacksIntensity, forceRevert);
            }
        }

        protected virtual IEnumerator HandleInitialDelayCo(Vector3 position, int index, float feedbacksIntensity, bool forceRevert = false)
        {
            IsPlaying = true;

            yield return new WaitForSeconds(InitialDelay);

            PreparePlay(position, index, feedbacksIntensity, forceRevert);
        }

        /// <summery>
        /// 이 피드백이 재생될 가능성을 평가하고 이 피드백이 재생될 수 있으면 true를 반환하고 그렇지 않으면 false를 반환합니다.
        /// </summery>
        /// <returns></returns>
        protected virtual bool EvaluateChance()
        {
            if (ChanceToPlay == 0f)
            {
                return false;
            }

            if (ChanceToPlay >= 100f)
            {
                if (RandomEx.Range(0f, 100f) > ChanceToPlay)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summery>
        ///각 피드백이 정의한 경우 각 피드백의 Reset 메서드를 호출합니다. 깜박거리는 렌더러의 초기 색상을 재설정하는 것이 그 예입니다.
        /// </summery>
        public virtual void ResetFeedbacks()
        {
            for (int i = 0; i < _feedbacks.Length; i++)
            {
                if ((_feedbacks[i] != null) && _feedbacks[i].Active)
                {
                    _feedbacks[i].ResetFeedback();
                }
            }
            IsPlaying = false;
        }

        /// <summery>
        /// 이 GameFeedbacks의 방향을 변경합니다.
        /// </summery>
        public virtual void Revert()
        {
            Events.TriggerOnRevert(this);
            Direction = (Direction == Directions.BottomToTop) ? Directions.TopToBottom : Directions.BottomToTop;
        }

        /// <summery>
        /// 시퀀스 실행을 일시 중지한 다음 ResumeFeedbacks()를 호출하여 다시 시작할 수 있습니다.
        /// </summery>
        public virtual void PauseFeedbacks()
        {
            Events.TriggerOnPause(this);
            InScriptDrivenPause = true;
        }

        /// <summery>
        /// 스크립트 기반 일시 중지가 진행 중인 경우 시퀀스 실행을 재개합니다.
        /// </summery>
        public virtual void ResumeFeedbacks()
        {
            Events.TriggerOnResume(this);
            InScriptDrivenPause = false;
        }

        protected virtual void PreparePlay(Vector3 feedbackPosition, int index, float feedbacksIntensity, bool forceRevert = false)
        {
            Events.TriggerOnPlay(this);

            _holdingMax = 0f;

            // 일시 중지 또는 보류 일시 중지가 있는지 테스트
            _pauseFound = false;
            for (int i = 0; i < _feedbacks.Length; i++)
            {
                if (_feedbacks[i] != null)
                {
                    if ((_feedbacks[i].Pause != null) && _feedbacks[i].Active && _feedbacks[i].ShouldPlayInThisSequenceDirection)
                    {
                        _pauseFound = true;
                    }
                    if ((_feedbacks[i].HoldingPause == true) && _feedbacks[i].Active && _feedbacks[i].ShouldPlayInThisSequenceDirection)
                    {
                        _pauseFound = true;
                    }
                }
            }

            if (!_pauseFound)
            {
                PlayAllFeedbacks(feedbackPosition, index, feedbacksIntensity, forceRevert);
            }
            else
            {
                // 하나 이상의 일시 중지가 발견된 경우
                _ = StartCoroutine(PausedFeedbacksCo(feedbackPosition, index, feedbacksIntensity));
            }
        }

        protected virtual void PlayAllFeedbacks(Vector3 feedbackPosition, int index, float feedbacksIntensity, bool forceRevert = false)
        {
            // 일시 중지가 발견되지 않으면 모든 피드백을 한 번에 재생합니다.
            for (int i = 0; i < _feedbacks.Length; i++)
            {
                if (FeedbackCanPlay(_feedbacks[i]))
                {
                    _feedbacks[i].Play(feedbackPosition, index, feedbacksIntensity);
                }
            }
        }

        ///<summery>
        /// 지정된 피드백의 타이밍 섹션에 정의된 조건이 이 GameFeedbacks의 현재 재생 방향에서 재생할 수 있는 경우 true를 반환합니다.
        ///</summery>
        protected bool FeedbackCanPlay(GameFeedback feedback)
        {
            if (feedback.Timing.GameFeedbacksDirectionCondition == GameFeedbackTiming.GameFeedbacksDirectionConditions.Always)
            {
                return true;
            }
            else if ((Direction == Directions.TopToBottom) && (feedback.Timing.GameFeedbacksDirectionCondition == GameFeedbackTiming.GameFeedbacksDirectionConditions.OnlyWhenForwards))
            {
                return true;
            }
            else if ((Direction == Directions.BottomToTop) && (feedback.Timing.GameFeedbacksDirectionCondition == GameFeedbackTiming.GameFeedbacksDirectionConditions.OnlyWhenBackwards))
            {
                return true;
            }
            return false;
        }

        protected virtual IEnumerator PausedFeedbacksCo(Vector3 feedbackPosition, int index, float feedbacksIntensity)
        {
            IsPlaying = true;

            int i = (Direction == Directions.TopToBottom) ? 0 : _feedbacks.Length - 1;

            while ((i >= 0) && (i < _feedbacks.Length))
            {
                if (!IsPlaying)
                {
                    yield break;
                }

                if (_feedbacks[i] == null)
                {
                    yield break;
                }

                if ((_feedbacks[i].Active && _feedbacks[i].ScriptDrivenPause) || InScriptDrivenPause)
                {
                    InScriptDrivenPause = true;

                    bool inAutoResume = _feedbacks[i].ScriptDrivenPauseAutoResume > 0f;
                    float scriptDrivenPauseStartedAt = Time.unscaledTime;
                    float autoResumeDuration = _feedbacks[i].ScriptDrivenPauseAutoResume;

                    while (InScriptDrivenPause)
                    {
                        if (inAutoResume && (Time.unscaledTime - scriptDrivenPauseStartedAt > autoResumeDuration))
                        {
                            ResumeFeedbacks();
                        }
                        yield return null;
                    }
                }

                // handles holding pauses
                if (_feedbacks[i].Active
                 && ((_feedbacks[i].HoldingPause == true) || (_feedbacks[i].LooperPause == true))
                 && _feedbacks[i].ShouldPlayInThisSequenceDirection)
                {
                    Events.TriggerOnPause(this);

                    // we stay here until all previous feedbacks have finished
                    while (Time.unscaledTime - _lastStartAt < _holdingMax)
                    {
                        yield return null;
                    }
                    _holdingMax = 0f;
                    _lastStartAt = Time.unscaledTime;
                }

                // plays the feedback
                if (FeedbackCanPlay(_feedbacks[i]))
                {
                    _feedbacks[i].Play(feedbackPosition, index, feedbacksIntensity);
                }

                // Handles pause
                if ((_feedbacks[i].Pause != null) && _feedbacks[i].Active && _feedbacks[i].ShouldPlayInThisSequenceDirection)
                {
                    bool shouldPause = true;
                    if (_feedbacks[i].Chance < 100)
                    {
                        float random = Random.Range(0f, 100f);
                        if (random > _feedbacks[i].Chance)
                        {
                            shouldPause = false;
                        }
                    }

                    if (shouldPause)
                    {
                        yield return _feedbacks[i].Pause;
                        Events.TriggerOnResume(this);
                        _lastStartAt = Time.unscaledTime;
                        _holdingMax = 0f;
                    }
                }

                // updates holding max
                if (_feedbacks[i].Active)
                {
                    if ((_feedbacks[i].Pause == null) && _feedbacks[i].ShouldPlayInThisSequenceDirection && (!_feedbacks[i].Timing.ExcludeFromHoldingPauses))
                    {
                        float feedbackDuration = _feedbacks[i].TotalDuration;
                        _holdingMax = Mathf.Max(feedbackDuration, _holdingMax);
                    }
                }

                i += (Direction == Directions.TopToBottom) ? 1 : -1;
            }
            float unscaledTimeAtEnd = Time.unscaledTime;
            while (Time.unscaledTime - unscaledTimeAtEnd < _holdingMax)
            {
                yield return null;
            }

            while (HasFeedbackStillPlaying())
            {
                yield return null;
            }

            IsPlaying = false;
            Events.TriggerOnComplete(this);
            ApplyAutoRevert();
        }

        /// <summery>
        /// 피드백이 계속 재생 중이면 true를 반환합니다.
        /// </summery>
        public virtual bool HasFeedbackStillPlaying()
        {
            int count = _feedbacks.Length;
            for (int i = 0; i < count; i++)
            {
                if (_feedbacks[i].IsPlaying)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summery>
        /// 다음 플레이에서 방향을 되돌리기 위해 GameFeedback을 준비합니다.
        /// </summery>
        protected virtual void ApplyAutoRevert()
        {
            if (AutoChangeDirectionOnEnd)
            {
                ShouldRevertOnNextPlay = true;
            }
        }

        /// <summery>
        /// 개별 피드백을 중지하지 않고 모든 추가 피드백 재생 중지
        /// </summery>
        public virtual void StopFeedbacks()
        {
            StopFeedbacks(true);
        }

        /// <summery>
        /// 개별 피드백도 중지할 수 있는 옵션과 함께 모든 피드백 재생 중지
        /// </summery>
        public virtual void StopFeedbacks(bool stopAllFeedbacks = true)
        {
            StopFeedbacks(transform.position, 1.0f, stopAllFeedbacks);
        }

        /// <summery>
        /// 피드백에서 사용할 수 있는 위치와 강도를 지정하여 모든 피드백 재생을 중지합니다.
        /// </summery>
        public virtual void StopFeedbacks(Vector3 position, float feedbacksIntensity = 1.0f, bool stopAllFeedbacks = true)
        {
            if (stopAllFeedbacks)
            {
                for (int i = 0; i < _feedbacks.Length; i++)
                {
                    _feedbacks[i].Stop(position, feedbacksIntensity);
                }
            }
            IsPlaying = false;
            StopAllCoroutines();
        }

        /// <summary>
        /// 이 피드백의 시간 배율을 지속 시간(초)에 적용합니다.
        /// </summary>
        public virtual float ApplyTimeMultiplier(float duration)
        {
            return duration * DurationMultiplier;
        }
    }
}