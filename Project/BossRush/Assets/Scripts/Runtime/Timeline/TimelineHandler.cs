using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TeamSuneat.Timeline
{
    public static class TimelineHandler
    {
        private static bool BindTrackInternal<T>(PlayableDirector director, string trackName, T component, string methodName) where T : UnityEngine.Object
        {
            if (director == null)
            {
                Log.Warning(LogTags.Timeline, "{0}: PlayableDirector가 null입니다.", methodName);
                return false;
            }

            if (component == null)
            {
                Log.Warning(LogTags.Timeline, "{0}: 컴포넌트가 null입니다. trackName: {1}", methodName, trackName);
                return false;
            }

            TimelineAsset timeline = director.playableAsset as TimelineAsset;
            if (timeline == null)
            {
                Log.Warning(LogTags.Timeline, "{0}: TimelineAsset이 null입니다. trackName: {1}", methodName, trackName);
                return false;
            }

            foreach (TrackAsset track in timeline.GetOutputTracks())
            {
                if (track.name == trackName)
                {
                    director.SetGenericBinding(track, component);
                    return true;
                }
            }

            Log.Warning(LogTags.Timeline, "{0}: 트랙을 찾을 수 없습니다. trackName: {1}", methodName, trackName);
            return false;
        }

        public static bool BindAnimator(PlayableDirector director, string trackName, Animator animator)
        {
            return BindTrackInternal(director, trackName, animator, "BindAnimator");
        }

        public static bool BindActivationTrack(PlayableDirector director, string trackName, GameObject gameObject)
        {
            return BindTrackInternal(director, trackName, gameObject, "BindActivationTrack");
        }

        public static bool BindAudioSource(PlayableDirector director, string trackName, AudioSource audioSource)
        {
            return BindTrackInternal(director, trackName, audioSource, "BindAudioSource");
        }

        public static bool BindCinemachine(PlayableDirector director, string trackName, CinemachineBrain brainCamera)
        {
            return BindTrackInternal(director, trackName, brainCamera, "BindCinemachine");
        }
    }
}
