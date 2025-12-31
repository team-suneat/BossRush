using Sirenix.OdinInspector;
using System.Collections.Generic;
using TeamSuneat;
using TeamSuneat.Data;
using TeamSuneat.Setting;
using UnityEngine;
using UnityEngine.Audio;

namespace TeamSuneat.Audio
{
    public partial class AudioManager : SingletonMonoBehaviour<AudioManager>
    {
        /// <summary>  효과음 최대개수 </summary>
        private const int SOUND_EFFECT_MAX_COUNT = 16;

        /// <summary>  음향 페이드 아웃 지속시간 </summary>
        private const float CROSS_FADE_DURATION = 3f;

        private const int INITIAL_AUDIO_OBJECT_CAPACITY = 32;
        private const float MIN_PLAY_INTERVAL = 0.1f;

        public AudioMixer Mixer;
        public AudioMixerGroup MusicMixerGroup;
        public AudioMixerGroup SFXMixerGroup;

        [ReadOnly]
        public float ResultSFXVolume;

        private List<AudioObject> _audioObjects = new(INITIAL_AUDIO_OBJECT_CAPACITY);
        private ListMultiMap<SoundNames, AudioObject> _audioMap = new();
        private Dictionary<SoundNames, Deck<AudioClip>> _audioClipDecks = new();
        private Dictionary<SoundNames, float> _lastPlayTime = new();

        public AudioObject BgmAudioObject => _bgmAudioObject;

        private AudioObject _bgmAudioObject;

        private AudioCrossFader _bgmFader;

        protected override void Awake()
        {
            base.Awake();

            _bgmFader = new AudioCrossFader(this);
        }

        protected override void OnStart()
        {
            base.OnStart();

            LoadVolumes();
        }

        protected override void OnRelease()
        {
            base.OnRelease();

            Clear();
        }

        private void LoadVolumes()
        {
            if (GameSetting.Instance == null)
            {
                return;
            }

            SetMixerVolume("Music", GameSetting.Instance.Audio.BGMVolume, GameSetting.Instance.Audio.MuteBGM);
            SetMixerVolume("SFX", GameSetting.Instance.Audio.SFXVolume, GameSetting.Instance.Audio.MuteSFX);
        }

        public void SetMixerVolume(string mixerParameter, float volume, bool isMuted)
        {
            if (Mixer == null)
            {
                if (Log.LevelWarning)
                {
                    Log.Warning(LogTags.Audio, "(Manager) AudioMixer가 설정되지 않았습니다.");
                }

                return;
            }

            _ = isMuted || volume.IsZero() ? Mixer.SetFloat(mixerParameter, -80f) : Mixer.SetFloat(mixerParameter, Mathf.Log10(volume) * 20);
        }

        private bool CanPlaySound(SoundNames soundName)
        {
            return !_lastPlayTime.TryGetValue(soundName, out float lastTime) || Time.unscaledTime - lastTime >= MIN_PLAY_INTERVAL;
        }

        private void UpdateLastPlayTime(SoundNames soundName)
        {
            _lastPlayTime[soundName] = Time.unscaledTime;
        }

        //

        private void Release(ref AudioObject audioObject)
        {
            if (audioObject != null)
            {
                Log.Warning(LogTags.Audio, "(Manager) 오디오 오브젝트를 정지/삭제합니다: {0}", audioObject.name);

                audioObject.Stop();
                audioObject.Despawn();
                audioObject = null;
            }
        }

        // BGM

        private bool CheckPlayableBGM(AudioClip clip)
        {
            if (!Application.isPlaying)
            {
                if (Log.LevelWarning)
                {
                    Log.Warning(LogTags.Audio, "(Manager) 게임이 실행 중이 아닙니다.");
                }

                return false;
            }

            if (clip == null)
            {
                if (Log.LevelWarning)
                {
                    Log.Warning(LogTags.Audio, "(Manager) BGM 클립이 null입니다.");
                }

                return false;
            }

            return true;
        }

        public AudioObject PlayBGM(SoundNames soundName)
        {
            SoundAsset soundAsset = ScriptableDataManager.Instance.FindSound(soundName);
            if (!soundAsset.IsValid() || !soundAsset.SoundClips.IsValid())
            {
                return null;
            }

            AudioClip clip = soundAsset.SoundClips[0];
            return PlayBGM(clip, soundAsset.VolumeScale);
        }

        public AudioObject PlayBGM(AudioClip clip, float volume = 1f)
        {
            if (!CheckPlayableBGM(clip))
            {
                return null;
            }

            AudioObject audioObject = Spawn(clip.name, transform);
            if (audioObject != null)
            {
                audioObject.Type = AudioTypes.Music;
                audioObject.PlayLoopScaled(clip);
                audioObject.SetVolume(volume);
                audioObject.SetMixerGroup(MusicMixerGroup);

                _bgmAudioObject = audioObject;

                if (Log.LevelInfo)
                {
                    Log.Info(LogTags.Audio, "(Manager) 배경음(BGM)을 재생합니다: {0}", clip.name);
                }
            }

            return audioObject;
        }

        public void StopBGM()
        {
            if (_bgmAudioObject != null)
            {
                Log.Info(LogTags.Audio, "(Manager) 배경음(BGM) 페이드를 정지합니다. {0}", _bgmAudioObject.name);
            }

            _bgmFader.StopFade();
            Release(ref _bgmAudioObject);
        }

        public bool TryStartChangeBGM(SoundNames soundName)
        {
            SoundAsset asset = ScriptableDataManager.Instance.FindSound(soundName);
            if (!asset.IsValid() || !asset.SoundClips.IsValid())
            {
                return false;
            }

            AudioClip audioClip = asset.SoundClips[0];
            if (audioClip == null)
            {
                return false;
            }
            else if (_bgmAudioObject == null)
            {
                return PlayBGM(soundName);
            }
            else if (_bgmAudioObject.CompareClip(audioClip))
            {
                if (Log.LevelWarning)
                {
                    Log.Warning(LogTags.Audio, "(Manager) 같은 이름의 배경음(BGM)을 재생하지 않습니다: {0}", soundName);
                }

                return false;
            }

            StartChangeBGM(audioClip, asset.VolumeScale);
            return true;
        }

        public void StartChangeBGM(AudioClip audioClip, float volume = 1f)
        {
            AudioObject newAudioObject = Spawn(audioClip.name, transform);
            if (newAudioObject != null)
            {
                newAudioObject.PlayLoopScaled(audioClip);
                newAudioObject.SetVolume(volume);
                newAudioObject.SetMixerGroup(MusicMixerGroup);

                if (Log.LevelInfo)
                {
                    Log.Info(LogTags.Audio, "(Manager) 배경음(BGM)을 페이드 변경합니다: {0} >> {1}", _bgmAudioObject.name, audioClip.name);
                }

                _bgmFader.StartFade(_bgmAudioObject, newAudioObject, CROSS_FADE_DURATION, result =>
                {
                    _bgmAudioObject = result;
                });
            }
        }

        // SFX

        private bool CheckPlayableSFX(SoundAsset asset)
        {
            if (!Application.isPlaying)
            {
                if (Log.LevelWarning)
                {
                    Log.Warning(LogTags.Audio, "(Manager) 게임이 실행 중이 아닙니다.");
                }

                return false;
            }

            if (asset == null)
            {
                if (Log.LevelWarning)
                {
                    Log.Warning(LogTags.Audio, "(Manager) 사운드 에셋이 null입니다.");
                }

                return false;
            }

            if (!asset.SoundClips.IsValid())
            {
                if (Log.LevelWarning)
                {
                    Log.Warning(LogTags.Audio, $"(Manager) 설정된 사운드 클립이 없습니다. {asset.Name}");
                }

                return false;
            }

            if (GameSetting.Instance.Audio.MuteSFX)
            {
                return false;
            }

            if (GameSetting.Instance.Audio.SFXVolume <= 0f)
            {
                return false;
            }

            if (!CanPlaySound(asset.Name))
            {
                return false;
            }

            if (_audioMap.ContainsKey(asset.Name))
            {
                if (_audioMap.TryGetValue(asset.Name, out List<AudioObject> audioObjects))
                {
                    if (audioObjects.IsValid(asset.MaxPlayCount))
                    {
                        CleanupInvalidAudioObjects(audioObjects);
                        AudioObject firstAudioObject = GetFirst(audioObjects);
                        if (firstAudioObject == null) { return true; }
                        if (firstAudioObject.DetermineMinProgress())
                        {
                            if (Log.LevelInfo)
                            {
                                Log.Info(LogTags.Audio, $"(Manager) SFX 정지: {firstAudioObject.name}");
                            }
                            StopSFX(firstAudioObject);
                            return true;
                        }
                        if (Log.LevelWarning)
                        {
                            Log.Warning(LogTags.Audio, $"(Manager) 최대 재생 개수 초과: {asset.Name}, Count: {audioObjects.Count}");
                        }

                        return false;
                    }
                }
            }

            if (_audioObjects.Count > SOUND_EFFECT_MAX_COUNT)
            {
                if (Log.LevelWarning)
                {
                    Log.Warning(LogTags.Audio, $"(Manager) 전체 사운드 객체 최대 개수 초과: {asset.Name}");
                }

                StopSFX(_audioObjects[0]);
                return false;
            }

            return true;
        }

        private void CleanupInvalidAudioObjects(List<AudioObject> audioObjects)
        {
            int before = audioObjects.Count;
            audioObjects.RemoveAll(obj => obj == null);
            int after = audioObjects.Count;

            if (Log.LevelInfo && before != after)
            {
                Log.Info(LogTags.Audio, $"(Manager) 삭제된 오디오 오브젝트 정리됨: {before - after}개");
            }
        }

        private AudioObject GetFirst(List<AudioObject> audioObjects)
        {
            for (int i = 0; i < audioObjects.Count; i++)
            {
                if (audioObjects[i] != null)
                {
                    return audioObjects[i];
                }
            }

            // 모두 삭제된 경우 리스트 비움 (성능/일관성 유지)
            audioObjects.Clear();
            return null;
        }

        public AudioObject PlayDonDestroyedSFXOneShotUnscaled(SoundAsset asset, Vector3 position)
        {
            AudioObject audioObject = PlaySFXOneShot(asset, position, AudioTimeType.Unscaled);
            if (audioObject != null)
            {
                audioObject.transform.SetParent(transform);
            }

            return audioObject;
        }

        public AudioObject PlayDonDestroyedSFXOneShotScaled(SoundAsset asset, Vector3 position)
        {
            AudioObject audioObject = PlaySFXOneShot(asset, position, AudioTimeType.Scaled);
            if (audioObject != null)
            {
                audioObject.transform.SetParent(transform);
            }

            return audioObject;
        }

        //

        public AudioObject PlaySFXOneShotScaled(SoundNames soundName)
        {
            SoundAsset asset = ScriptableDataManager.Instance.FindSound(soundName);
            return PlaySFXOneShot(asset, Vector3.zero, AudioTimeType.Scaled);
        }

        public AudioObject PlaySFXOneShotUnscaled(SoundNames soundName)
        {
            SoundAsset asset = ScriptableDataManager.Instance.FindSound(soundName);
            return PlaySFXOneShot(asset, Vector3.zero, AudioTimeType.Unscaled);
        }

        public AudioObject PlaySFXOneShotScaled(SoundNames soundName, Vector3 spawnPosition)
        {
            SoundAsset asset = ScriptableDataManager.Instance.FindSound(soundName);
            return PlaySFXOneShot(asset, spawnPosition, AudioTimeType.Scaled);
        }

        public AudioObject PlaySFXOneShotScaled(SoundAsset soundAsset, Vector3 spawnPosition)
        {
            return PlaySFXOneShot(soundAsset, spawnPosition, AudioTimeType.Scaled);
        }

        public AudioObject PlaySFXOneShotUnscaled(SoundNames soundName, Vector3 spawnPosition)
        {
            SoundAsset asset = ScriptableDataManager.Instance.FindSound(soundName);
            return PlaySFXOneShot(asset, spawnPosition, AudioTimeType.Unscaled);
        }

        public AudioObject PlaySFXOneShotUnscaled(SoundAsset soundAsset, Vector3 spawnPosition)
        {
            return PlaySFXOneShot(soundAsset, spawnPosition, AudioTimeType.Unscaled);
        }

        private AudioObject PlaySFXOneShot(SoundAsset soundAsset, Vector3 position, AudioTimeType timeType)
        {
            if (!CheckPlayableSFX(soundAsset))
            {
                return null;
            }

            AudioClip clip = GetAudioClip(soundAsset);
            if (clip != null)
            {
                int index = _audioMap.GetValueCount(soundAsset.Name);
                AudioObject audioObject = Spawn($"{clip.name} {index}", position);
                if (audioObject != null)
                {
                    if (Log.LevelInfo)
                    {
                        Log.Info(LogTags.Audio, $"(Manager) 효과음을 재생합니다: {clip.name}");
                    }

                    RegisterSFX(soundAsset.Name, audioObject);
                    UpdateLastPlayTime(soundAsset.Name);

                    audioObject.Type = AudioTypes.SFX;
                    audioObject.SetMixerGroup(SFXMixerGroup);
                    audioObject.SetSoundAsset(soundAsset);
                    audioObject.MultiplyVolumeScale(_audioObjects.Count, SOUND_EFFECT_MAX_COUNT);

                    if (timeType == AudioTimeType.Scaled)
                    {
                        audioObject.PlayOneShotScaled(clip);
                    }
                    else
                    {
                        audioObject.PlayOneShotUnscaled(clip);
                    }
                }

                return audioObject;
            }

            return null;
        }

        //

        private AudioClip GetAudioClip(SoundAsset asset)
        {
            if (asset.SoundClips.Count == 1)
            {
                return asset.SoundClips[0];
            }
            else if (_audioClipDecks.ContainsKey(asset.Name))
            {
                if (_audioClipDecks[asset.Name].Count == 0)
                {
                    _audioClipDecks[asset.Name].Set(asset.SoundClips);
                }

                return _audioClipDecks[asset.Name].DrawTop();
            }
            else
            {
                Deck<AudioClip> audioClipDeck = new();

                audioClipDeck.Set(asset.SoundClips);

                _audioClipDecks.Add(asset.Name, audioClipDeck);

                return _audioClipDecks[asset.Name].DrawTop();
            }
        }

        public AudioObject PlaySFXLoop(SoundNames soundName)
        {
            SoundAsset asset = ScriptableDataManager.Instance.FindSound(soundName);

            return PlaySFXLoop(asset, Vector3.zero);
        }

        public AudioObject PlaySFXLoop(SoundNames soundName, Vector3 position)
        {
            SoundAsset asset = ScriptableDataManager.Instance.FindSound(soundName);

            return PlaySFXLoop(asset, position);
        }

        public AudioObject PlaySFXLoop(SoundAsset soundAsset, Vector3 position)
        {
            if (!CheckPlayableSFX(soundAsset))
            {
                return null;
            }

            AudioClip clip = GetAudioClip(soundAsset);
            if (clip != null)
            {
                AudioObject audioObject = Spawn(clip.name, position);
                if (audioObject != null)
                {
                    if (Log.LevelInfo)
                    {
                        Log.Info(LogTags.Audio, "(Manager) 반복하는 효과음(SFX)을 재생합니다. {0}", soundAsset.Name);
                    }

                    RegisterSFX(soundAsset.Name, audioObject);

                    audioObject.Type = AudioTypes.SFX;
                    audioObject.SetMixerGroup(SFXMixerGroup);
                    audioObject.SetSoundAsset(soundAsset);
                    audioObject.PlayLoopScaled(clip);
                }

                return audioObject;
            }

            return null;
        }

        public void StopSFX(AudioObject audioObject)
        {
            if (audioObject != null)
            {
                Log.Info(LogTags.Audio, "(Manager) 오디오 오브젝트({0})를 정지하고 삭제합니다. {1}", audioObject.Type, audioObject.name);

                audioObject.Stop();
                audioObject.Despawn();
            }
        }

        // Manage

        private AudioObject Spawn(string clipName, Transform parent)
        {
            string prefabName = "AudioObject";
            AudioObject audioObject = ResourcesManager.SpawnPrefab<AudioObject>(prefabName, parent);

#if UNITY_EDITOR
            if (audioObject != null)
            {
                string objectName = GetGameObjectName(clipName);
                audioObject.SetGameObjectName(objectName);
            }
#endif

            return audioObject;
        }

        private AudioObject Spawn(string clipName, Vector3 spawnPosition)
        {
            string prefabName = "AudioObject";
            AudioObject audioObject = ResourcesManager.SpawnPrefab<AudioObject>(prefabName, spawnPosition);

#if UNITY_EDITOR
            if (audioObject != null)
            {
                string objectName = GetGameObjectName(clipName);
                audioObject.SetGameObjectName(objectName);
            }
#endif

            return audioObject;
        }

        private void RegisterSFX(SoundNames soundName, AudioObject audioObject)
        {
            if (audioObject == null)
            {
                return;
            }

            _audioObjects.Add(audioObject);
            _audioMap.Add(soundName, audioObject);
        }

        public void UnregisterSFX(AudioObject audioObject)
        {
            if (audioObject == null)
            {
                return;
            }

            if (audioObject.Name == SoundNames.None)
            {
                return;
            }

            if (_audioMap.TryGetValue(audioObject.Name, out List<AudioObject> audioObjects))
            {
                if (audioObjects.Contains(audioObject))
                {
                    _ = _audioObjects.Remove(audioObject);
                    _audioMap.Remove(audioObject.Name, audioObject);
                }
                else if (Log.LevelWarning)
                {
                    Log.Warning(LogTags.Audio, $"(Manager) 등록되지 않은 오디오 오브젝트: {audioObject.name}");
                }
            }
        }

        public void Clear()
        {
            if (Log.LevelInfo)
            {
                Log.Info(LogTags.Audio, $"(Manager) 등록된 모든 효과음 오디오 오브젝트를 중지/삭제합니다.");
            }

            List<AudioObject> audioObjectsToClear = new(_audioObjects);
            for (int i = audioObjectsToClear.Count - 1; i >= 0; i--)
            {
                AudioObject audioObject = audioObjectsToClear[i];
                Release(ref audioObject);
            }

            _audioObjects.Clear();
            _audioMap.Clear();
            _audioClipDecks.Clear();
            _lastPlayTime.Clear();
        }

        private string GetGameObjectName(string clipName, int count = 0)
        {
            return count > 0 ? string.Format("AudioObject ({0}_{1})", clipName, count) : string.Format("AudioObject ({0})", clipName);
        }
    }
}