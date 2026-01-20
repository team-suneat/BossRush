using Sirenix.OdinInspector;
using System.Collections;
using TeamSuneat.Setting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

namespace TeamSuneat.Timeline
{
    [RequireComponent(typeof(PlayableDirector))]
    public class TimelineObject : XBehaviour
    {
        [Title("#Timeline Object")]
        public int Index;

        public TimelineName Name;
        public string NameString;

        [Title("#Timeline Object", "Component")]
        public PlayableDirector TimelineDirector;

        [Title("#Timeline Object", "Toggles")]
        [SuffixLabel("오브젝트가 시작되면 자동 타임라인 재생")]
        public bool AutoPlayOnStart;

        [EnableIf("AutoPlayOnStart")]
        [SuffixLabel("자동 타임라인 재생에 필요한 지연 시간")]
        public float AutoPlayDelayTime;

        [SuffixLabel("타임라인 재생 중 캐릭터 Logical Update 비활성화")]
        public bool PauseCharactersLogic;

        [Title("#Timeline Object", "Events")]
        public UnityEvent OnPlayStart;
        public UnityEvent OnPlayCompleted;

        private Coroutine _timelineCoroutine;
        private bool _isSkipping;

        public bool IsPlaying => TimelineDirector != null && TimelineDirector.state == PlayState.Playing;

        public float Duration => TimelineDirector != null ? (float)TimelineDirector.duration : 0f;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            TimelineDirector = GetComponent<PlayableDirector>();
        }

        private void OnValidate()
        {
            if (!EnumEx.ConvertTo(ref Name, NameString))
            {
                Log.Error(LogTags.Timeline, "Timeline conversion failed. {0}, {1}", NameString, this.GetHierarchyPath());
            }
        }

        public override void AutoSetting()
        {
            base.AutoSetting();

            if (Name != 0)
            {
                NameString = Name.ToString();
            }
        }

        public override void AutoNaming()
        {
            if (Index > 0)
            {
                SetGameObjectName($"{Index}. TimelineObject({Name.ToString()})");
            }
            else
            {
                SetGameObjectName($"TimelineObject({Name.ToString()})");
            }
        }

        protected override void OnStart()
        {
            base.OnStart();

            // 매니저에 자동 등록
            TimelineManager.Instance.Register(this);

            if (AutoPlayOnStart)
            {
                CoroutineNextTimer(AutoPlayDelayTime, Play);
            }
        }

        protected override void OnRelease()
        {
            base.OnRelease();

            // 매니저에서 자동 해제
            TimelineManager.Instance.Unregister(this);

            StopTimeline();

            if (IsPlaying && TimelineDirector != null)
            {
                TimelineDirector.Stop();
                OnCompleted();
            }
        }

        protected override void OnDisabled()
        {
            base.OnDisabled();

            StopTimeline();

            if (IsPlaying && TimelineDirector != null)
            {
                TimelineDirector.Stop();
                OnCompleted();
            }
        }

        public bool TryPlay()
        {
            if (TimelineDirector == null)
            {
                Log.Error(LogTags.Timeline, "{0} 타임라인 디렉터가 없습니다.", this.GetHierarchyName());
                return false;
            }

            if (TimelineDirector.playableAsset == null)
            {
                Log.Error(LogTags.Timeline, "{0} 타임라인 에셋이 null입니다: {1}", this.GetHierarchyName(), Name);
                return false;
            }

            return true;
        }

        public void Play()
        {
            if (!TryPlay())
            {
                return;
            }

            if (!TimelineManager.Instance.CanPlay(this))
            {
                Log.Warning(LogTags.Timeline, "{0} 타임라인 재생을 취소합니다. 다른 타임라인이 재생 중입니다.",
                    this.GetHierarchyName());
                return;
            }

            if (!TimelineDirector.playableGraph.IsValid())
            {
                Log.Info(LogTags.Timeline, "{0} 타임라인 그래프를 재바인딩합니다.", this.GetHierarchyName());
                TimelineDirector.RebindPlayableGraphOutputs();
            }

            TimelineDirector.Play();
            OnPlay();
            StartTimeline();

            Log.Info(LogTags.Timeline, "{0} 타임라인을 활성화합니다.", this.GetHierarchyName());
        }

        public void Skip()
        {
            if (TimelineDirector == null || !IsPlaying || _isSkipping)
            {
                return;
            }

            _isSkipping = true;

            Log.Info(LogTags.Timeline, "{0} 타임라인을 스킵합니다.", this.GetHierarchyName());

            StopTimeline();
            TimelineDirector.Pause();
            FastForwardToEnd();
            TimelineDirector.Stop();
            OnCompleted();

            _isSkipping = false;
        }

        private void OnPlay()
        {
            GameSetting.Instance.Input.BlockCharacterInput();
            // Register는 OnStart에서 이미 수행됨
            OnPlayStart?.Invoke();

            if (PauseCharactersLogic)
            {
                CharacterManager.Instance.Pause();
            }
        }

        private void StartTimeline()
        {
            _timelineCoroutine ??= StartXCoroutine(ProcessTimeline());
        }

        private void StopTimeline()
        {
            StopXCoroutine(ref _timelineCoroutine);
        }

        private IEnumerator ProcessTimeline()
        {
            while (TimelineDirector.state == PlayState.Playing)
            {
                yield return null;
            }

            if (TimelineDirector.state == PlayState.Paused && !_isSkipping)
            {
                Log.Warning(LogTags.Timeline, "{0} 타임라인이 일시정지되었습니다.", this.GetHierarchyName());
                _timelineCoroutine = null;
                yield break;
            }

            if (!_isSkipping)
            {
                OnCompleted();
            }

            Log.Info(LogTags.Timeline, "타임라인 중지. {0}", Name);
            _timelineCoroutine = null;
        }

        private void OnCompleted()
        {
            Log.Info(LogTags.Timeline, "타임라인 완료. {0}", Name);

            GameSetting.Instance.Input.UnblockCharacterInput();
            // Unregister는 OnRelease에서 처리됨

            OnPlayCompleted?.Invoke();

            TimelineManager.Instance.OnTimelineCompleted(Name);

            if (PauseCharactersLogic)
            {
                CharacterManager.Instance.Resume();
            }
        }

        private void FastForwardToEnd()
        {
            if (TimelineDirector == null || TimelineDirector.playableAsset == null)
            {
                return;
            }

            const float STEP_SIZE = 0.03f;
            const int MAX_STEPS = 2000;

            double currentTime = TimelineDirector.time;
            double duration = TimelineDirector.duration;
            double targetTime = duration;

            if (currentTime >= targetTime)
            {
                TimelineDirector.time = targetTime;
                TimelineDirector.Evaluate();
                return;
            }

            int stepCount = 0;
            double time = currentTime;

            while (time < targetTime && stepCount < MAX_STEPS)
            {
                time += STEP_SIZE;
                if (time > targetTime)
                {
                    time = targetTime;
                }

                TimelineDirector.time = time;
                TimelineDirector.Evaluate();
                stepCount++;
            }

            if (stepCount >= MAX_STEPS && time < targetTime)
            {
                Log.Warning(LogTags.Timeline, "{0} 타임라인 스킵이 최대 스텝 제한에 도달했습니다. 일부 시그널/마커가 실행되지 않았을 수 있습니다.",
                    this.GetHierarchyName());
            }

            TimelineDirector.time = targetTime;
            TimelineDirector.Evaluate();

            Log.Progress(LogTags.Timeline, "{0} 타임라인 스킵 완료. {1} 스텝 실행.", this.GetHierarchyName(), stepCount);
        }
    }
}
