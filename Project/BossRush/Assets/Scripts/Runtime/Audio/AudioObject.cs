using System;
using System.Collections;
using TeamSuneat;
using TeamSuneat.Data;
using Lean.Pool;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

namespace TeamSuneat.Audio
{
    public enum AudioTimeType
    {
        Scaled,
        Unscaled
    }

    public class AudioObject : XBehaviour, IPoolable
    {
        private const float MIN_VOLUME = 0f;
        private const float MAX_VOLUME = 1f;
        private const float SOUND_EFFECT_VOLUME_SCALE_LOWER_BOUND = 0.75f;

        public AudioTypes Type { get; set; }
        private AudioSource _audioSource;

        private SoundAsset _asset;
        private UnityAction<SoundNames, AudioObject> _onPlayCallback;
        private float _startTime;
        private AudioTimeType _timeType;
        private Coroutine _despawnCoroutine;
        private Coroutine _fadeOutCoroutine;
        private WaitForSecondsRealtime _waitForCheck;
        private bool _isFading;
        private bool _isPlaying;

        public SoundNames Name => _asset != null ? _asset.Name : SoundNames.None;

        public float Volume => _audioSource?.volume ?? 0f;

        public float PlayTime => _timeType == AudioTimeType.Scaled
            ? Time.time - _startTime
            : Time.unscaledTime - _startTime;

        private void Awake()
        {
            _waitForCheck = new WaitForSecondsRealtime(Time.fixedDeltaTime);

            _audioSource = GetComponent<AudioSource>();

            if (_audioSource == null)
            {
                Log.Error(LogTags.Audio, "AudioSource 컴포넌트를 찾을 수 없습니다.");
            }
        }

        public void OnSpawn()
        {
            ResetState();
        }

        public void OnDespawn()
        {
            ResetState();
        }

        private void ResetState()
        {
            _startTime = 0f;
            _asset = null;
            _onPlayCallback = null;
            _isPlaying = false;
            _isFading = false;

            if (_audioSource != null)
            {
                _audioSource.Stop();
                _audioSource.clip = null;
                _audioSource.volume = MAX_VOLUME;
            }

            StopDespawnCoroutine();
            StopFadeOutCoroutine();
        }

        protected override void OnDisabled()
        {
            base.OnDisabled();
            CallPlayEvent();
        }

        public void Despawn()
        {
            CallPlayEvent();

            if (Type == AudioTypes.SFX)
            {
                AudioManager.Instance?.UnregisterSFX(this);
                ResourcesManager.Despawn(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

            ResetState();
        }

        private void StopDespawnCoroutine()
        {
            StopXCoroutine(ref _despawnCoroutine);
        }

        private void StopFadeOutCoroutine()
        {
            StopXCoroutine(ref _fadeOutCoroutine);
        }

        #region Pause

        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                StopDespawnCoroutine();
                Pause();
            }
            else
            {
                UnPause();
            }
        }

        private void Pause()
        {
            if (_audioSource != null && _audioSource.isPlaying)
            {
                _audioSource.Pause();
            }
        }

        private void UnPause()
        {
            if (_audioSource != null)
            {
                _audioSource.UnPause();

                if (_audioSource.isPlaying)
                {
                    _despawnCoroutine = StartXCoroutine(ProcessCheckPlaying());
                }
            }
        }

        #endregion Pause

        public void Stop()
        {
            if (_audioSource != null && _audioSource.isPlaying)
            {
                _audioSource.Stop();
            }

            StopFadeOutCoroutine();
            StopDespawnCoroutine();
            _isPlaying = false;
            Despawn();
        }

        #region Fade

        public void FadeOut(bool isStopOnCompleted)
        {
            if (_audioSource == null)
            {
                return;
            }

            _isFading = true;
            _fadeOutCoroutine ??= StartXCoroutine(ProcessFadeOut(isStopOnCompleted));
        }

        private IEnumerator ProcessFadeOut(bool isStopOnCompleted)
        {
            if (_audioSource == null)
            {
                yield break;
            }

            AudioSource checkSource = _audioSource;
            float targetVolume = MIN_VOLUME;
            float fadeSpeed = Time.deltaTime;

            while (checkSource != null && checkSource.volume > targetVolume)
            {
                yield return null;
                if (checkSource != null)
                {
                    checkSource.volume = Mathf.Max(targetVolume, checkSource.volume - fadeSpeed);
                }
            }

            if (checkSource != null)
            {
                checkSource.volume = targetVolume;
            }

            _isFading = false;

            if (isStopOnCompleted)
            {
                Stop();
            }
        }

        #endregion Fade

        #region 재생 결정 (Determine)

        public bool CompareClip(AudioClip clip)
        {
            return _audioSource != null && _audioSource.clip == clip;
        }

        public bool DetermineMinTimeToPlay()
        {
            return _asset == null || _asset.MinTimeToPlay <= 0 || PlayTime >= _asset.MinTimeToPlay;
        }

        public bool DetermineMinProgress()
        {
            if (_audioSource == null || _audioSource.clip == null || _asset == null)
            {
                return true;
            }

            float minProgressTime = CalculateMinProgressTime();
            return minProgressTime <= 0 || PlayTime > minProgressTime;
        }

        private float CalculateMinProgressTime()
        {
            if (_asset.MinProgressType == SoundAsset.MinimumProgressType.Rate)
            {
                return _audioSource.clip.length * _asset.MinProgressRateToPlayNext;
            }

            return _asset.MinProgressType == SoundAsset.MinimumProgressType.Seconds ? _asset.MinProgressSecondsToPlayNext : 0f;
        }

        #endregion 재생 결정 (Determine)

        #region 재생 (Play)

        public void PlayOneShotScaled(AudioClip clip, UnityAction<SoundNames, AudioObject> OnPlay = null)
        {
            Play(clip, AudioTimeType.Scaled, false, OnPlay);
        }

        public void PlayOneShotUnscaled(AudioClip clip, UnityAction<SoundNames, AudioObject> OnPlay = null)
        {
            Play(clip, AudioTimeType.Unscaled, false, OnPlay);
        }

        public void PlayLoopScaled(AudioClip clip)
        {
            Play(clip, AudioTimeType.Scaled, true);
        }

        public void PlayLoopUnscaled(AudioClip clip)
        {
            Play(clip, AudioTimeType.Unscaled, true);
        }

        private void Play(AudioClip clip, AudioTimeType timeType, bool loop, UnityAction<SoundNames, AudioObject> OnPlay = null)
        {
            if (clip == null)
            {
                Log.Warning(LogTags.Audio, "재생할 오디오 클립이 null입니다.");
                return;
            }

            if (!ActiveInHierarchy)
            {
                Log.Warning(LogTags.Audio, "게임 오브젝트가 비활성화되어 사운드를 재생하지 못합니다. clipName: {0}", clip.name);
                return;
            }

            if (_audioSource == null)
            {
                Log.Error(LogTags.Audio, "AudioSource가 없어 사운드를 재생할 수 없습니다.");
                return;
            }

            // 이전 재생 중지
            if (_isPlaying)
            {
                Stop();
            }

            _isPlaying = true;
            _audioSource.Stop();
            _audioSource.clip = clip;
            _audioSource.loop = loop;
            _audioSource.time = 0;
            _audioSource.Play();

            _timeType = timeType;
            _startTime = _timeType == AudioTimeType.Scaled ? Time.time : Time.unscaledTime;

            if (!loop)
            {
                StopDespawnCoroutine();
                _onPlayCallback = OnPlay;
                _despawnCoroutine = StartXCoroutine(ProcessCheckPlaying());
            }
        }

        #endregion 재생 (Play)

        private IEnumerator ProcessCheckPlaying()
        {
            if (_audioSource == null)
            {
                yield break;
            }

            AudioSource checkSource = _audioSource;
            while (checkSource != null && checkSource.isPlaying)
            {
                yield return _waitForCheck;
            }

            _isPlaying = false;
            Despawn();
            _despawnCoroutine = null;
        }

        private void CallPlayEvent()
        {
            if (_onPlayCallback != null)
            {
                _onPlayCallback(Name, this);
                _onPlayCallback = null;
            }
        }

        #region 설정 (Set)

        public void SetSoundAsset(SoundAsset asset)
        {
            if (!asset.IsValid())
            {
                Log.Warning(LogTags.Audio, "유효하지 않은 사운드 에셋입니다.");
                return;
            }

            // 현재 재생 중인 경우 중지
            if (_isPlaying)
            {
                Stop();
            }

            _asset = asset;

            if (_asset.VolumeScale > 0)
            {
                SetVolume(_asset.VolumeScale);
            }
        }

        public void SetMixerGroup(AudioMixerGroup mixerGroup)
        {
            if (_audioSource == null)
            {
                Log.Warning(LogTags.Audio, "AudioSource가 없어 믹서 그룹을 설정할 수 없습니다.");
                return;
            }

            _audioSource.outputAudioMixerGroup = mixerGroup;
        }

        public void MultiplyVolumeScale(int current, int max)
        {
            if (current < 1 || max < 1 || current > max)
            {
                Log.Warning(LogTags.Audio, "잘못된 볼륨 스케일 파라미터입니다. current: {0}, max: {1}", current, max);
                return;
            }

            float value = (float)(current - 1) / (max - 1);
            float volumeRange = 1f - SOUND_EFFECT_VOLUME_SCALE_LOWER_BOUND;
            value = Mathf.Clamp(1f - (value * volumeRange), SOUND_EFFECT_VOLUME_SCALE_LOWER_BOUND, 1f);
            SetVolume(_asset?.VolumeScale * value ?? 1f);
        }

        public void SetVolume(float volume)
        {
            if (_audioSource == null)
            {
                throw new InvalidOperationException("AudioSource가 초기화되지 않았습니다.");
            }

            volume = Mathf.Clamp(volume, MIN_VOLUME, MAX_VOLUME);

            if (!_isFading)
            {
                _audioSource.volume = volume;
            }
        }

        #endregion 설정 (Set)
    }
}