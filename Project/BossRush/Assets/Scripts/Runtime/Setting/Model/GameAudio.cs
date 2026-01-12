using TeamSuneat.Audio;

namespace TeamSuneat.Setting
{
    public class GameAudio
    {
        private float _masterVolume;
        private float _musicVolume;
        private float _sfxVolume;
        private bool _muteMusic;
        private bool _muteSFX;

        private const float DEFAULT_MASTER_VOLUME = 1f;
        private const float DEFAULT_MUSIC_VOLUME = 0.5f;
        private const float DEFAULT_SFX_VOLUME = 1f;
        private const bool DEFAULT_MUTE_MUSIC = false;
        private const bool DEFAULT_MUTE_SFX = false;

        public float MasterVolume
        {
            get => _masterVolume;
            set
            {
                if (!_masterVolume.Compare(value))
                {
                    _masterVolume = value;
                    AudioManager.Instance.SetMixerVolume("Master", MasterVolume, false);
                    GamePrefs.SetFloat(GamePrefTypes.OPTION_MASTER_VOLUME, value);
                }
            }
        }

        public float MusicVolume
        {
            get => _musicVolume;
            set
            {
                if (!_musicVolume.Compare(value))
                {
                    _musicVolume = value;
                    AudioManager.Instance.SetMixerVolume("Music", MusicVolume, MuteMusic);
                    GamePrefs.SetFloat(GamePrefTypes.OPTION_MUSIC_VOLUME, value);
                }
            }
        }

        public float SFXVolume
        {
            get => _sfxVolume;
            set
            {
                if (!_sfxVolume.Compare(value))
                {
                    _sfxVolume = value;
                    AudioManager.Instance.SetMixerVolume("SFX", SFXVolume, MuteSFX);
                    GamePrefs.SetFloat(GamePrefTypes.OPTION_SFX_VOLUME, value);
                }
            }
        }

        public bool MuteMusic
        {
            get => _muteMusic;
            set
            {
                if (_muteMusic != value)
                {
                    _muteMusic = value;
                    AudioManager.Instance.SetMixerVolume("Music", MusicVolume, MuteMusic);
                    GamePrefs.SetBool(GamePrefTypes.OPTION_MUTE_MUSIC, value);
                }
            }
        }

        public bool MuteSFX
        {
            get => _muteSFX;
            set
            {
                if (_muteSFX != value)
                {
                    _muteSFX = value;
                    AudioManager.Instance.SetMixerVolume("SFX", SFXVolume, MuteSFX);
                    GamePrefs.SetBool(GamePrefTypes.OPTION_MUTE_SFX, value);
                }
            }
        }

        public void Load()
        {
            // Master 볼륨 로드
            if (GamePrefs.HasKey(GamePrefTypes.OPTION_MASTER_VOLUME))
            {
                _masterVolume = GamePrefs.GetFloat(GamePrefTypes.OPTION_MASTER_VOLUME);
            }
            else
            {
                _masterVolume = DEFAULT_MASTER_VOLUME;
            }

            // MUSIC 볼륨 로드 (기존 Music 볼륨과 호환)
            if (GamePrefs.HasKey(GamePrefTypes.OPTION_MUSIC_VOLUME))
            {
                _musicVolume = GamePrefs.GetFloat(GamePrefTypes.OPTION_MUSIC_VOLUME);
            }
            else
            {
                _musicVolume = DEFAULT_MUSIC_VOLUME;
            }

            if (GamePrefs.HasKey(GamePrefTypes.OPTION_SFX_VOLUME))
            {
                _sfxVolume = GamePrefs.GetFloat(GamePrefTypes.OPTION_SFX_VOLUME);
            }
            else
            {
                _sfxVolume = DEFAULT_SFX_VOLUME;
            }

            // MUSIC 음소거 로드 (기존 Music 음소거와 호환)
            if (GamePrefs.HasKey(GamePrefTypes.OPTION_MUTE_MUSIC))
            {
                _muteMusic = GamePrefs.GetBool(GamePrefTypes.OPTION_MUTE_MUSIC);
            }
            else
            {
                _muteMusic = DEFAULT_MUTE_MUSIC;
            }

            if (GamePrefs.HasKey(GamePrefTypes.OPTION_MUTE_SFX))
            {
                _muteSFX = GamePrefs.GetBool(GamePrefTypes.OPTION_MUTE_SFX);
            }
            else
            {
                _muteSFX = DEFAULT_MUTE_SFX;
            }

            // AudioManager에 적용
            AudioManager.Instance.SetMixerVolume("Master", MasterVolume, false);
            AudioManager.Instance.SetMixerVolume("Music", MusicVolume, MuteMusic);
            AudioManager.Instance.SetMixerVolume("SFX", SFXVolume, MuteSFX);

            Log.Info(LogTags.Setting, "사운드를 설정합니다. Master 볼륨: {0}, MUSIC 볼륨: {1}, 효과음 볼륨: {2}, MUSIC 음소거: {3}, 효과음 음소거: {4}",
                _masterVolume.ToString(), _musicVolume.ToString(), _sfxVolume.ToString(), _muteMusic.ToBoolString(), _muteSFX.ToBoolString());
        }

        public void SetDefaultValues()
        {
            MasterVolume = DEFAULT_MASTER_VOLUME;
            MusicVolume = DEFAULT_MUSIC_VOLUME;
            SFXVolume = DEFAULT_SFX_VOLUME;
            MuteMusic = DEFAULT_MUTE_MUSIC;
            MuteSFX = DEFAULT_MUTE_SFX;

            Log.Info(LogTags.Setting, "사운드를 초기화합니다. Master 볼륨: {0}, MUSIC 볼륨: {1}, 효과음 볼륨: {2}, MUSIC 음소거: {3}, 효과음 음소거: {4}",
                _masterVolume.ToString(), _musicVolume.ToString(), _sfxVolume.ToString(), _muteMusic.ToBoolString(), _muteSFX.ToBoolString());
        }
    }
}