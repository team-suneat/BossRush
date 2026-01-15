using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace TeamSuneat.Data
{
    public partial class ScriptableDataManager
    {
        public bool CheckLoadedSync()
        {
            if (_logSetting == default) { return false; }
            else if (_gameDefine == default) { return false; }
            else if (!_fontAssets.IsValid()) { return false; }

            return true;
        }

        public bool CheckLoaded()
        {
            if (_logSetting == default) { return false; }
            else if (_gameDefine == default) { return false; }
            else if (!_characterAssets.IsValid()) { return false; }
            else if (!_hitmarkAssets.IsValid()) { return false; }
            else if (!_fontAssets.IsValid()) { return false; }
            else if (!_floatyAssets.IsValid()) { return false; }
            else if (!_flickerAssets.IsValid()) { return false; }
            else if (!_soundAssets.IsValid()) { return false; }
            // else if (!_stageAssets.IsValid()) { return false; }
            // else if (!_forceVelocityAssets.IsValid()) { return false; }
            else if (_playerCharacterStatAsset == null) { return false; }
            else if (_cameraImpulseAssets.IsValid()) { return false; }

            return true;
        }

        protected void OnLoadData()
        {
            // Core.cs의 OnLoadData() 메서드 호출
            _logSetting?.OnLoadData();

            // 스테이지 에셋 OnLoadData() 메서드 호출
            foreach (var stageAsset in _stageAssets.Values)
            {
                stageAsset?.OnLoadData();
            }

            // 캐릭터 에셋 OnLoadData() 메서드 호출
            foreach (var characterAsset in _characterAssets.Values)
            {
                characterAsset?.OnLoadData();
            }

            // ForceVelocity 에셋 OnLoadData() 메서드 호출
            foreach (var forceVelocityAsset in _forceVelocityAssets.Values)
            {
                forceVelocityAsset?.OnLoadData();
            }
        }

        public void LoadScriptableAssets()
        {
            int count = 0;
            string[] pathArray = PathManager.FindAllAssetPath();
            Clear();

            Log.Info("스크립터블 파일을 읽기 시작합니다.");

            for (int i = 0; i < pathArray.Length; i++)
            {
                string path = pathArray[i];

                if (LoadLogSettingSync(path))
                {
                    count += 1;
                }
                else if (LoadGameDefineSync(path))
                {
                    count += 1;
                }
                else if (LoadCharacterSync(path))
                {
                    count += 1;
                }
                else if (LoadHitmarkSync(path))
                {
                    count += 1;
                }
                else if (LoadFontSync(path))
                {
                    count += 1;
                }
                else if (LoadFloatySync(path))
                {
                    count += 1;
                }
                else if (LoadFlickerSync(path))
                {
                    count += 1;
                }
                else if (LoadSoundSync(path))
                {
                    count += 1;
                }
                else if (LoadStageSync(path))
                {
                    count += 1;
                }
                else if (LoadForceVelocitySync(path))
                {
                    count += 1;
                }
                else if (LoadPlayerCharacterStatSync(path))
                {
                    count += 1;
                }
            }

            Log.Info("파일을 읽어왔습니다. Count: {0}", count.ToString());

            OnLoadData();
        }

        //

        private bool LoadPlayerCharacterStatSync(string filePath)
        {
            if (!filePath.Contains("PlayerCharacterStat"))
            {
                return false;
            }

            PlayerCharacterStatConfigAsset asset = ResourcesManager.LoadResource<PlayerCharacterStatConfigAsset>(filePath);
            if (asset != null)
            {
                if (_playerCharacterStatAsset != null)
                {
                    Log.Warning(LogTags.ScriptableData, "플레이어 캐릭터 능력치 에셋이 중복으로 로드 되고 있습니다. 기존: {0}, 새로운: {1}",
                        _playerCharacterStatAsset.name, asset.name);
                }
                else
                {
                    Log.Progress("스크립터블 데이터를 읽어왔습니다. Path: {0}", filePath);
                    _playerCharacterStatAsset = asset;
                }

                return true;
            }
            else
            {
                Log.Warning("스크립터블 데이터를 읽을 수 없습니다. Path: {0}", filePath);
            }

            return false;
        }

        private bool LoadForceVelocitySync(string filePath)
        {
            if (!filePath.Contains("ForceVelocity"))
            {
                return false;
            }

            ForceVelocityAsset asset = ResourcesManager.LoadResource<ForceVelocityAsset>(filePath);
            if (asset != null)
            {
                if (asset.TID == 0)
                {
                    Log.Warning(LogTags.ScriptableData, "{0}, ForceVelocity 아이디가 설정되어있지 않습니다. {1}", asset.name, filePath);
                }
                else if (_forceVelocityAssets.ContainsKey(asset.TID))
                {
                    Log.Warning(LogTags.ScriptableData, "같은 TID로 중복 ForceVelocity가 로드 되고 있습니다. TID: {0}, 기존: {1}, 새로운 이름: {2}",
                         asset.TID, _forceVelocityAssets[asset.TID].name, asset.name);
                }
                else
                {
                    Log.Progress("스크립터블 데이터를 읽어왔습니다. Path: {0}", filePath);
                    _forceVelocityAssets[asset.TID] = asset;
                }

                return true;
            }
            else
            {
                Log.Warning("스크립터블 데이터를 읽을 수 없습니다. Path: {0}", filePath);
            }

            return false;
        }

        public void LoadScriptableAssetsSyncByLabel(string label)
        {
            IList<ScriptableObject> assets = ResourcesManager.LoadResourcesByLabelSync<UnityEngine.ScriptableObject>(label);
            int count = 0;

            for (int i = 0; i < assets.Count; i++)
            {
                ScriptableObject asset = assets[i];
                if (asset == null)
                {
                    continue;
                }

                switch (asset)
                {
                    case LogSettingAsset logSetting:
                        if (_logSetting == null)
                        {
                            _logSetting = logSetting;
#if !UNITY_EDITOR
                            _logSetting.ExternSwitchOffAll();
#endif
                            count++;
                        }
                        else
                        {
                            Log.Warning(LogTags.ScriptableData, "LogSettingAsset이 중복으로 로드 되고 있습니다. 기존: {0}, 새로운: {1}",
                                _logSetting.name, logSetting.name);
                        }
                        break;

                    case GameDefineAsset gameDefine:
                        if (_gameDefine == null)
                        {
                            _gameDefine = gameDefine;
                            count++;
                        }
                        else
                        {
                            Log.Warning(LogTags.ScriptableData, "GameDefineAsset이 중복으로 로드 되고 있습니다. 기존: {0}, 새로운: {1}",
                                _gameDefine.name, gameDefine.name);
                        }
                        break;

                    case FontAsset font:
                        if (!_fontAssets.ContainsKey(font.TID))
                        {
                            _fontAssets[font.TID] = font;
                            count++;
                        }
                        else
                        {
                            Log.Warning(LogTags.ScriptableData, "FontAsset이 중복으로 로드 되고 있습니다. TID: {0}, 기존: {1}, 새로운: {2}",
                                font.TID, _fontAssets[font.TID].name, font.name);
                        }
                        break;
                }
            }

            if (count > 0)
            {
                Log.Info(LogTags.ScriptableData, "Addressable ScriptableSync 라벨로 {0}개 파일을 동기적으로 읽어왔습니다.", count.ToString());
            }
        }

        //

        public async Task LoadScriptableAssetsAsync()
        {
            Clear();

            // 동기 로드: GameDefineAsset, LogSettingAsset, FontAsset
            LoadScriptableAssetsSyncByLabel(AddressableLabels.Default);

            await LoadScriptableAssetsAsyncByLabel(AddressableLabels.Scriptable);
        }

        public async Task LoadScriptableAssetsAsyncByLabel(string label)
        {
            // 비동기 로드: 나머지 에셋들
            int count = 0;
            IList<ScriptableObject> assets = await ResourcesManager.LoadResourcesByLabelAsync<UnityEngine.ScriptableObject>(label);
            for (int i = 0; i < assets.Count; i++)
            {
                ScriptableObject asset = assets[i];
                if (asset == null)
                {
                    continue;
                }

                switch (asset)
                {
                    case LogSettingAsset logSetting:
                        // 이미 동기 로드되었으면 건너뛰기
                        if (_logSetting == null)
                        {
                            _logSetting = logSetting;
                            count++;
                        }
                        break;

                    case GameDefineAsset gameDefine:
                        // 이미 동기 로드되었으면 건너뛰기
                        if (_gameDefine == null)
                        {
                            _gameDefine = gameDefine;
                            count++;
                        }
                        break;

                    case CharacterAsset character:
                        if (!_characterAssets.ContainsKey(character.TID))
                        {
                            _characterAssets[character.TID] = character;
                            count++;
                        }
                        break;

                    case HitmarkAsset hitmark:
                        if (!_hitmarkAssets.ContainsKey(hitmark.TID))
                        {
                            _hitmarkAssets[hitmark.TID] = hitmark;
                            count++;
                        }
                        break;

                    case FontAsset font:
                        // 이미 동기 로드되었으면 건너뛰기
                        if (!_fontAssets.ContainsKey(font.TID))
                        {
                            _fontAssets[font.TID] = font;
                            count++;
                        }
                        break;

                    case FloatyAsset floaty:
                        if (!_floatyAssets.ContainsKey(floaty.TID))
                        {
                            _floatyAssets[floaty.TID] = floaty;
                            count++;
                        }
                        break;

                    case FlickerAsset flicker:
                        if (!_flickerAssets.ContainsKey(flicker.TID))
                        {
                            _flickerAssets[flicker.TID] = flicker;
                            count++;
                        }
                        break;

                    case SoundAsset sound:
                        if (!_soundAssets.ContainsKey(sound.TID))
                        {
                            _soundAssets[sound.TID] = sound;
                            count++;
                        }
                        break;

                    case StageAsset stage:
                        int stageTid = BitConvert.Enum32ToInt(stage.Name);
                        if (!_stageAssets.ContainsKey(stageTid))
                        {
                            _stageAssets[stageTid] = stage;
                            count++;
                        }
                        break;

                    case ForceVelocityAsset forceVelocity:
                        if (!_forceVelocityAssets.ContainsKey(forceVelocity.TID))
                        {
                            _forceVelocityAssets[forceVelocity.TID] = forceVelocity;
                            count++;
                        }
                        break;

                    case CameraImpulseAsset cameraImpulse:
                        if (!_cameraImpulseAssets.ContainsKey(cameraImpulse.TID))
                        {
                            _cameraImpulseAssets[cameraImpulse.TID] = cameraImpulse;
                            count++;
                        }
                        break;

                    case PlayerCharacterStatConfigAsset playerCharacterStat:
                        if (_playerCharacterStatAsset == null)
                        {
                            _playerCharacterStatAsset = playerCharacterStat;
                            count++;
                        }
                        else
                        {
                            Log.Warning(LogTags.ScriptableData, "플레이어 캐릭터 능력치 에셋이 중복으로 로드 되고 있습니다. 기존: {0}, 새로운: {1}",
                                _playerCharacterStatAsset.name, playerCharacterStat.name);
                        }
                        break;
                }
            }

            Log.Info(LogTags.ScriptableData, "Addressable Scriptable 라벨로 파일을 읽어왔습니다. Count: {0}", count.ToString());

            OnLoadData();
        }
    }
}