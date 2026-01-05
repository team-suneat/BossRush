using System.Collections.Generic;

namespace TeamSuneat.Data
{
    /// <summary> 프로젝트에서 생성한 Scriptable Object를 관리합니다. </summary>
    public partial class ScriptableDataManager : Singleton<ScriptableDataManager>
    {
        private GameDefineAsset _gameDefine;
        private LogSettingAsset _logSetting;

        private PlayerCharacterStatConfigAsset _playerCharacterStatAsset; // 플레이어 캐릭터 능력치
        private readonly Dictionary<int, HitmarkAsset> _hitmarkAssets = new();
        private readonly Dictionary<int, CharacterAsset> _characterAssets = new();
        private readonly Dictionary<int, FontAsset> _fontAssets = new();
        private readonly Dictionary<int, FloatyAsset> _floatyAssets = new();
        private readonly Dictionary<int, FlickerAsset> _flickerAssets = new();
        private readonly Dictionary<int, SoundAsset> _soundAssets = new();
        private readonly Dictionary<int, StageAsset> _stageAssets = new();
        private readonly Dictionary<int, ForceVelocityAsset> _forceVelocityAssets = new();

        public void Clear()
        {
            _logSetting = null;
            _gameDefine = null;

            _soundAssets.Clear();
            _hitmarkAssets.Clear();
            _characterAssets.Clear();
            _fontAssets.Clear();
            _floatyAssets.Clear();
            _flickerAssets.Clear();
            _stageAssets.Clear();
            _forceVelocityAssets.Clear();
            _playerCharacterStatAsset = null;
        }

        public void RefreshAll()
        {
            RefreshAllCharacter();
            RefreshAllHitmark();
            RefreshAllFonts();
            RefreshAllFlicker();
            RefreshAllFloaty();
            RefreshAllSounds();
            RefreshAllStage();
            RefreshAllForceVelocity();
        }
    }
}