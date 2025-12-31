using TeamSuneat.Audio;

namespace TeamSuneat.Setting
{
    public class GameAudio
    {
        private float _bgmVolume;
        private float _sfxVolume;
        private bool _muteBGM;
        private bool _muteSFX;

        private const float DEFAULT_BGM_VOLUME = 0.5f;
        private const float DEFAULT_SFX_VOLUME = 1f;
        private const bool DEFAULT_MUTE_BGM = false;
        private const bool DEFAULT_MUTE_SFX = false;

        public float BGMVolume
        {
            get => _bgmVolume;
            set
            {
                if (!_bgmVolume.Compare(value))
                {
                    _bgmVolume = value;
                    AudioManager.Instance.SetMixerVolume("Music", BGMVolume, MuteBGM);
                    GamePrefs.SetFloat(GamePrefTypes.OPTION_BGM_VOLUME, value);
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

        public bool MuteBGM
        {
            get => _muteBGM;
            set
            {
                if (_muteBGM != value)
                {
                    _muteBGM = value;
                    AudioManager.Instance.SetMixerVolume("Music", BGMVolume, MuteBGM);
                    GamePrefs.SetBool(GamePrefTypes.OPTION_MUTE_BGM, value);
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
            // BGM 볼륨 로드 (기존 Music 볼륨과 호환)
            if (GamePrefs.HasKey(GamePrefTypes.OPTION_BGM_VOLUME))
            {
                _bgmVolume = GamePrefs.GetFloat(GamePrefTypes.OPTION_BGM_VOLUME);
            }
            else
            {
                _bgmVolume = DEFAULT_BGM_VOLUME;
            }

            if (GamePrefs.HasKey(GamePrefTypes.OPTION_SFX_VOLUME))
            {
                _sfxVolume = GamePrefs.GetFloat(GamePrefTypes.OPTION_SFX_VOLUME);
            }
            else
            {
                _sfxVolume = DEFAULT_SFX_VOLUME;
            }

            // BGM 음소거 로드 (기존 Music 음소거와 호환)
            if (GamePrefs.HasKey(GamePrefTypes.OPTION_MUTE_BGM))
            {
                _muteBGM = GamePrefs.GetBool(GamePrefTypes.OPTION_MUTE_BGM);
            }
            else
            {
                _muteBGM = DEFAULT_MUTE_BGM;
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
            AudioManager.Instance.SetMixerVolume("Music", BGMVolume, MuteBGM);
            AudioManager.Instance.SetMixerVolume("SFX", SFXVolume, MuteSFX);

            Log.Info(LogTags.Setting, "사운드를 설정합니다. BGM 볼륨: {0}, 효과음 볼륨: {1}, BGM 음소거: {2}, 효과음 음소거: {3}",
                _bgmVolume.ToString(), _sfxVolume.ToString(), _muteBGM.ToBoolString(), _muteSFX.ToBoolString());
        }

        public void SetDefaultValues()
        {
            BGMVolume = DEFAULT_BGM_VOLUME;
            SFXVolume = DEFAULT_SFX_VOLUME;
            MuteBGM = DEFAULT_MUTE_BGM;
            MuteSFX = DEFAULT_MUTE_SFX;

            Log.Info(LogTags.Setting, "사운드를 초기화합니다. BGM 볼륨: {0}, 효과음 볼륨: {1}, BGM 음소거: {2}, 효과음 음소거: {3}",
                _bgmVolume.ToString(), _sfxVolume.ToString(), _muteBGM.ToBoolString(), _muteSFX.ToBoolString());
        }
    }
}