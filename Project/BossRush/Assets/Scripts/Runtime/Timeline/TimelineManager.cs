using System.Collections.Generic;
using TeamSuneat.CameraSystem.Core;
using Unity.Cinemachine;
using UnityEngine;

namespace TeamSuneat.Timeline
{
    public class TimelineManager : Singleton<TimelineManager>
    {
        private const float INPUT_LOCK_DURATION_ON_SKIP = 0.3f;
        private const string ANIMATION_TRACK_NAME = "Animation Track Player";
        private const string CINEMACHINE_TRACK_NAME = "Cinemachine Track";

        private Dictionary<TimelineName, TimelineObject> _timelineCache;
        private TimelineObject _currentPlayingTimeline;

        public List<TimelineObject> Timelines { get; private set; } = new List<TimelineObject>();

        public bool IsPlaying
        {
            get
            {
                for (int i = 0; i < Timelines.Count; i++)
                {
                    if (Timelines[i] != null && Timelines[i].IsPlaying)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public void Register(TimelineObject timelineObject)
        {
            if (timelineObject == null)
            {
                Log.Warning(LogTags.Timeline, "null 타임라인을 등록할 수 없습니다.");
                return;
            }

            if (!Timelines.Contains(timelineObject))
            {
                Timelines.Add(timelineObject);

                // 캐시에도 추가
                if (timelineObject.Name != TimelineName.None)
                {
                    if (_timelineCache == null)
                    {
                        _timelineCache = new Dictionary<TimelineName, TimelineObject>();
                    }

                    if (_timelineCache.ContainsKey(timelineObject.Name))
                    {
                        Log.Warning(LogTags.Timeline, "중복된 타임라인 이름이 발견되었습니다: {0}. {1}",
                            timelineObject.Name, timelineObject.GetHierarchyName());
                    }
                    else
                    {
                        _timelineCache[timelineObject.Name] = timelineObject;
                    }
                }

                Log.Info(LogTags.Timeline, "타임라인을 등록합니다. {0}", timelineObject.Name);
            }
        }

        public void Unregister(TimelineObject timelineObject)
        {
            if (timelineObject == null)
            {
                return;
            }

            if (Timelines.Contains(timelineObject))
            {
                Timelines.Remove(timelineObject);

                // 캐시에서도 제거
                if (_timelineCache != null && timelineObject.Name != TimelineName.None)
                {
                    _timelineCache.Remove(timelineObject.Name);
                }

                // 현재 재생 중인 타임라인이면 초기화
                if (_currentPlayingTimeline == timelineObject)
                {
                    _currentPlayingTimeline = null;
                }

                Log.Info(LogTags.Timeline, "타임라인을 등록 해제합니다. {0}", timelineObject.Name);
            }
        }

        public void Clear()
        {
            int count = Timelines.Count;
            Timelines.Clear();
            _timelineCache?.Clear();
            _currentPlayingTimeline = null;
            Log.Info(LogTags.Timeline, "모든 타임라인을 초기화합니다. 등록 해제된 타임라인 수: {0}", count);
        }

        public bool CanPlay(TimelineObject timelineObject)
        {
            if (timelineObject == null)
            {
                return false;
            }

            // 이미 등록된 타임라인이면 재생 가능 (같은 타임라인 재시작)
            if (Timelines.Contains(timelineObject))
            {
                return true;
            }

            // 다른 타임라인이 재생 중이면 불가
            return !IsPlaying;
        }

        public void PlayTimeline(TimelineName timelineName)
        {
            if (_timelineCache == null || !_timelineCache.TryGetValue(timelineName, out TimelineObject timelineObject))
            {
                Log.Error(LogTags.Timeline, "타임라인을 찾을 수 없습니다: {0}", timelineName);
                return;
            }

            PlayTimelineInternal(timelineObject);
        }

        public void PlayTimelineByObject(TimelineObject timelineObject)
        {
            if (timelineObject == null)
            {
                Log.Error(LogTags.Timeline, "타임라인 오브젝트가 null입니다.");
                return;
            }

            PlayTimelineInternal(timelineObject);
        }

        private void PlayTimelineInternal(TimelineObject timelineObject)
        {
            if (timelineObject == null)
            {
                Log.Error(LogTags.Timeline, "타임라인 오브젝트가 null입니다.");
                return;
            }

            if (!timelineObject.TryPlay())
            {
                return;
            }

            // 동시 재생 방지 체크
            if (!CanPlay(timelineObject))
            {
                Log.Warning(LogTags.Timeline, "{0} 타임라인 재생을 취소합니다. 다른 타임라인이 재생 중입니다.",
                    timelineObject.GetHierarchyName());
                return;
            }

            BindTimelineComponents(timelineObject);

            _currentPlayingTimeline = timelineObject;
            timelineObject.Play();

            Log.Info(LogTags.Timeline, "{0} 타임라인을 재생합니다.", timelineObject.GetHierarchyName());
        }

        private void BindTimelineComponents(TimelineObject timelineObject)
        {
            if (timelineObject == null || timelineObject.TimelineDirector == null)
            {
                Log.Error(LogTags.Timeline, "타임라인 디렉터가 null입니다.");
                return;
            }

            PlayerCharacter player = CharacterManager.Instance.Player;
            if (player == null)
            {
                Log.Warning(LogTags.Timeline, "플레이어 캐릭터가 null입니다. 애니메이터를 바인딩할 수 없습니다: {0}",
                    timelineObject.GetHierarchyName());
            }
            else if (player.Animator != null)
            {
                bool animatorBound = TimelineHandler.BindAnimator(timelineObject.TimelineDirector,
                    ANIMATION_TRACK_NAME, player.Animator);

                if (!animatorBound)
                {
                    Log.Warning(LogTags.Timeline, "플레이어 애니메이터 바인딩 실패: {0}",
                        timelineObject.GetHierarchyName());
                }
            }
            else
            {
                Log.Warning(LogTags.Timeline, "플레이어 애니메이터가 null입니다. 바인딩할 수 없습니다: {0}",
                    timelineObject.GetHierarchyName());
            }

            if (CameraManager.Instance != null)
            {
                CinemachineBrain brain = CameraManager.Instance.GetBrainCamera();
                if (brain != null)
                {
                    bool cinemachineBound = TimelineHandler.BindCinemachine(timelineObject.TimelineDirector,
                        CINEMACHINE_TRACK_NAME, brain);

                    if (!cinemachineBound)
                    {
                        Log.Warning(LogTags.Timeline, "시네마신 바인딩 실패: {0}",
                            timelineObject.GetHierarchyName());
                    }
                }
                else
                {
                    Log.Warning(LogTags.Timeline, "시네마신 브레인이 null입니다: {0}",
                        timelineObject.GetHierarchyName());
                }
            }
            else
            {
                Log.Warning(LogTags.Timeline, "CameraManager.Instance가 null입니다: {0}",
                    timelineObject.GetHierarchyName());
            }
        }

        public void LogicUpdate()
        {
            if (_currentPlayingTimeline != null && _currentPlayingTimeline.IsPlaying)
            {
                if (TSInputManager.Instance != null &&
                    TSInputManager.Instance.CheckButtonState(ActionNames.Skip, ButtonStates.ButtonDown))
                {
                    _currentPlayingTimeline.Skip();
                    LockPopupPauseInput();
                }
            }
        }

        private void LockPopupPauseInput()
        {
            if (TSInputManager.Instance == null)
            {
                return;
            }

            TSInputManager.Instance.ResetButtonState(ActionNames.PopupPause);
            TSInputManager.Instance.ResetButtonState(ActionNames.PopupInventory);
            TSInputManager.Instance.ResetButtonState(ActionNames.UICancel);

            TSInputManager.Instance.LockButtonForDuration(ActionNames.PopupInventory, INPUT_LOCK_DURATION_ON_SKIP);
            TSInputManager.Instance.LockButtonForDuration(ActionNames.PopupPause, INPUT_LOCK_DURATION_ON_SKIP);
            TSInputManager.Instance.LockButtonForDuration(ActionNames.UICancel, INPUT_LOCK_DURATION_ON_SKIP);
        }

        public void OnTimelineCompleted(TimelineName timelineName)
        {
            if (_currentPlayingTimeline != null && _currentPlayingTimeline.Name == timelineName)
            {
                Log.Info(LogTags.Timeline, "{0} 타임라인을 재생완료합니다. 재생 중인 타임라인을 초기화합니다.",
                    _currentPlayingTimeline.GetHierarchyName());
                _currentPlayingTimeline = null;
            }
        }
    }
}
