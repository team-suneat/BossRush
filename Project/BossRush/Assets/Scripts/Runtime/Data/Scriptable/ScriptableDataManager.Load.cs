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
            else if (!_buffAssets.IsValid()) { return false; }
            else if (!_stateEffectAssets.IsValid()) { return false; }
            else if (!_passiveAssets.IsValid()) { return false; }
            else if (!_skillAssets.IsValid()) { return false; }
            else if (!_hitmarkAssets.IsValid()) { return false; }
            else if (!_fontAssets.IsValid()) { return false; }
            else if (!_floatyAssets.IsValid()) { return false; }
            else if (!_flickerAssets.IsValid()) { return false; }
            else if (!_soundAssets.IsValid()) { return false; }
            else if (!_stageAssets.IsValid()) { return false; }
            else if (!_areaAssets.IsValid()) { return false; }
            else if (!_forceVelocityAssets.IsValid()) { return false; }
            else if (_enhancementDataAsset == null) { return false; }
            else if (_growthDataAsset == null) { return false; }
            else if (_experienceConfigAsset == null) { return false; }
            else if (_monsterStatConfigAsset == null) { return false; }
            else if (_monsterDropConfigAsset == null) { return false; }
            else if (_playerCharacterStatAsset == null) { return false; }
            else if (_skillCardUnlockAsset == null) { return false; }
            else if (_skillSlotUnlockAsset == null) { return false; }

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

            // 지역 에셋 OnLoadData() 메서드 호출
            foreach (var areaAsset in _areaAssets.Values)
            {
                areaAsset?.OnLoadData();
            }

            // 강화 시스템 데이터 OnLoadData() 메서드 호출
            _enhancementDataAsset?.OnLoadData();

            // 성장 시스템 데이터 OnLoadData() 메서드 호출
            _growthDataAsset?.OnLoadData();

            // 경험치 설정 데이터 OnLoadData() 메서드 호출
            _experienceConfigAsset?.OnLoadData();

            // 몬스터 능력치 설정 데이터 OnLoadData() 메서드 호출
            _monsterStatConfigAsset?.OnLoadData();

            // 몬스터 경험치 드랍 설정 데이터 OnLoadData() 메서드 호출
            _monsterDropConfigAsset?.OnLoadData();

            // 플레이어 캐릭터 능력치 데이터 OnLoadData() 메서드 호출
            _playerCharacterStatAsset?.OnLoadData();

            // 스킬 카드 해금 데이터 OnLoadData() 메서드 호출
            _skillCardUnlockAsset?.OnLoadData();

            // 스킬 슬롯 해금 데이터 OnLoadData() 메서드 호출
            _skillSlotUnlockAsset?.OnLoadData();

            // 스킬 에셋 OnLoadData() 메서드 호출
            foreach (var skillAsset in _skillAssets.Values)
            {
                skillAsset?.OnLoadData();
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
                else if (LoadBuffSync(path))
                {
                    count += 1;
                }
                else if (LoadBuffStateEffectSync(path))
                {
                    count += 1;
                }
                else if (LoadPassiveSync(path))
                {
                    count += 1;
                }
                else if (LoadSkillSync(path))
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
                else if (LoadAreaSync(path))
                {
                    count += 1;
                }
                else if (LoadForceVelocitySync(path))
                {
                    count += 1;
                }
                else if (LoadEnhancementDataSync(path))
                {
                    count += 1;
                }
                else if (LoadGrowthDataSync(path))
                {
                    count += 1;
                }
                else if (LoadExperienceConfigSync(path))
                {
                    count += 1;
                }
                else if (LoadMonsterStatConfigSync(path))
                {
                    count += 1;
                }
                else if (LoadMonsterExperienceDropConfigSync(path))
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

        private bool LoadEnhancementDataSync(string filePath)
        {
            if (!filePath.Contains("EnhancementData"))
            {
                return false;
            }

            EnhancementConfigAsset asset = ResourcesManager.LoadResource<EnhancementConfigAsset>(filePath);
            if (asset != null)
            {
                if (_enhancementDataAsset != null)
                {
                    Log.Warning(LogTags.ScriptableData, "강화 시스템 데이터 에셋이 중복으로 로드 되고 있습니다. 기존: {0}, 새로운: {1}",
                        _enhancementDataAsset.name, asset.name);
                }
                else
                {
                    Log.Progress("스크립터블 데이터를 읽어왔습니다. Path: {0}", filePath);
                    _enhancementDataAsset = asset;
                }

                return true;
            }
            else
            {
                Log.Warning("스크립터블 데이터를 읽을 수 없습니다. Path: {0}", filePath);
            }

            return false;
        }

        private bool LoadGrowthDataSync(string filePath)
        {
            if (!filePath.Contains("GrowthData"))
            {
                return false;
            }

            GrowthConfigAsset asset = ResourcesManager.LoadResource<GrowthConfigAsset>(filePath);
            if (asset != null)
            {
                if (_growthDataAsset != null)
                {
                    Log.Warning(LogTags.ScriptableData, "성장 시스템 데이터 에셋이 중복으로 로드 되고 있습니다. 기존: {0}, 새로운: {1}",
                        _growthDataAsset.name, asset.name);
                }
                else
                {
                    Log.Progress("스크립터블 데이터를 읽어왔습니다. Path: {0}", filePath);
                    _growthDataAsset = asset;
                }

                return true;
            }
            else
            {
                Log.Warning("스크립터블 데이터를 읽을 수 없습니다. Path: {0}", filePath);
            }

            return false;
        }

        private bool LoadExperienceConfigSync(string filePath)
        {
            if (!filePath.Contains("ExperienceConfig"))
            {
                return false;
            }

            ExperienceConfigAsset asset = ResourcesManager.LoadResource<ExperienceConfigAsset>(filePath);
            if (asset != null)
            {
                if (_experienceConfigAsset != null)
                {
                    Log.Warning(LogTags.ScriptableData, "경험치 설정 에셋이 중복으로 로드 되고 있습니다. 기존: {0}, 새로운: {1}",
                        _experienceConfigAsset.name, asset.name);
                }
                else
                {
                    Log.Progress("스크립터블 데이터를 읽어왔습니다. Path: {0}", filePath);
                    _experienceConfigAsset = asset;
                }

                return true;
            }
            else
            {
                Log.Warning("스크립터블 데이터를 읽을 수 없습니다. Path: {0}", filePath);
            }

            return false;
        }

        private bool LoadMonsterStatConfigSync(string filePath)
        {
            if (!filePath.Contains("MonsterStatConfig"))
            {
                return false;
            }

            MonsterStatConfigAsset asset = ResourcesManager.LoadResource<MonsterStatConfigAsset>(filePath);
            if (asset != null)
            {
                if (_monsterStatConfigAsset != null)
                {
                    Log.Warning(LogTags.ScriptableData, "몬스터 능력치 설정 에셋이 중복으로 로드 되고 있습니다. 기존: {0}, 새로운: {1}",
                        _monsterStatConfigAsset.name, asset.name);
                }
                else
                {
                    Log.Progress("스크립터블 데이터를 읽어왔습니다. Path: {0}", filePath);
                    _monsterStatConfigAsset = asset;
                }

                return true;
            }
            else
            {
                Log.Warning("스크립터블 데이터를 읽을 수 없습니다. Path: {0}", filePath);
            }

            return false;
        }

        private bool LoadMonsterExperienceDropConfigSync(string filePath)
        {
            if (!filePath.Contains("MonsterExperienceDropConfig"))
            {
                return false;
            }

            MonsterDropConfigAsset asset = ResourcesManager.LoadResource<MonsterDropConfigAsset>(filePath);
            if (asset != null)
            {
                if (_monsterDropConfigAsset != null)
                {
                    Log.Warning(LogTags.ScriptableData, "몬스터 경험치 드랍 설정 에셋이 중복으로 로드 되고 있습니다. 기존: {0}, 새로운: {1}",
                        _monsterDropConfigAsset.name, asset.name);
                }
                else
                {
                    Log.Progress("스크립터블 데이터를 읽어왔습니다. Path: {0}", filePath);
                    _monsterDropConfigAsset = asset;
                }

                return true;
            }
            else
            {
                Log.Warning("스크립터블 데이터를 읽을 수 없습니다. Path: {0}", filePath);
            }

            return false;
        }

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
            LoadScriptableAssetsSyncByLabel(AddressableLabels.ScriptableSync);

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

                    case BuffAsset buff:
                        if (!_buffAssets.ContainsKey(buff.TID))
                        {
                            _buffAssets[buff.TID] = buff;
                            count++;
                        }
                        break;

                    case BuffStateEffectAsset buffStateEffect:
                        if (!_stateEffectAssets.ContainsKey(buffStateEffect.TID))
                        {
                            _stateEffectAssets[buffStateEffect.TID] = buffStateEffect;
                            count++;
                        }
                        break;

                    case PassiveAsset passive:
                        if (!_passiveAssets.ContainsKey(passive.TID))
                        {
                            _passiveAssets[passive.TID] = passive;
                            count++;
                        }
                        break;

                    case SkillAsset skill:
                        if (!_skillAssets.ContainsKey(skill.TID))
                        {
                            _skillAssets[skill.TID] = skill;
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

                    case EnhancementConfigAsset enhancementData:
                        if (_enhancementDataAsset == null)
                        {
                            _enhancementDataAsset = enhancementData;
                            count++;
                        }
                        break;

                    case GrowthConfigAsset growthData:
                        if (_growthDataAsset == null)
                        {
                            _growthDataAsset = growthData;
                            count++;
                        }
                        break;

                    case ExperienceConfigAsset experienceConfig:
                        if (_experienceConfigAsset == null)
                        {
                            _experienceConfigAsset = experienceConfig;
                            count++;
                        }
                        break;

                    case MonsterStatConfigAsset monsterStatConfig:
                        if (_monsterStatConfigAsset == null)
                        {
                            _monsterStatConfigAsset = monsterStatConfig;
                            count++;
                        }
                        break;

                    case MonsterDropConfigAsset monsterExperienceDropConfig:
                        if (_monsterDropConfigAsset == null)
                        {
                            _monsterDropConfigAsset = monsterExperienceDropConfig;
                            count++;
                        }
                        break;

                    case PlayerCharacterStatConfigAsset playerCharacterStat:
                        if (_playerCharacterStatAsset == null)
                        {
                            _playerCharacterStatAsset = playerCharacterStat;
                            count++;
                        }
                        break;

                    case SkillCardUnlockAsset skillCardUnlock:
                        if (_skillCardUnlockAsset == null)
                        {
                            _skillCardUnlockAsset = skillCardUnlock;
                            count++;
                        }
                        break;

                    case SkillSlotUnlockAsset skillSlotUnlock:
                        if (_skillSlotUnlockAsset == null)
                        {
                            _skillSlotUnlockAsset = skillSlotUnlock;
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

                    case AreaAsset area:
                        int areaTid = BitConvert.Enum32ToInt(area.AreaName);
                        if (!_areaAssets.ContainsKey(areaTid))
                        {
                            _areaAssets[areaTid] = area;
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
                }
            }

            Log.Info(LogTags.ScriptableData, "Addressable Scriptable 라벨로 파일을 읽어왔습니다. Count: {0}", count.ToString());

            OnLoadData();
        }
    }
}