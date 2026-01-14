using System.Collections.Generic;

namespace TeamSuneat.Data
{
    /// <summary>
    /// ScriptableDataManager의 캐릭터 관련 기능
    /// </summary>
    public partial class ScriptableDataManager
    {
        #region Character Get Methods

        /// <summary>
        /// 캐릭터 에셋을 가져옵니다.
        /// </summary>
        public CharacterAsset GetCharacterAsset(CharacterNames characterName)
        {
            int key = BitConvert.Enum32ToInt(characterName);
            return _characterAssets.TryGetValue(key, out var asset) ? asset : null;
        }

        #endregion Character Get Methods

        #region Character Find Methods

        /// <summary>
        /// 캐릭터 에셋을 찾습니다.
        /// </summary>
        public CharacterAsset FindCharacter(CharacterNames key)
        {
            return FindCharacter(BitConvert.Enum32ToInt(key));
        }

        private CharacterAsset FindCharacter(int tid)
        {
            if (_characterAssets.ContainsKey(tid))
            {
                return _characterAssets[tid];
            }

            return null;
        }

        #endregion Character Find Methods

        #region Character FindClone Methods

        /// <summary>
        /// 캐릭터 데이터 클론을 찾습니다.
        /// </summary>
        public CharacterAssetData FindCharacterClone(CharacterNames characterName)
        {
            if (characterName != CharacterNames.None)
            {
                CharacterAssetData assetData = FindCharacterClone(BitConvert.Enum32ToInt(characterName));
                if (!assetData.IsValid())
                {
                    Log.Warning(LogTags.ScriptableData, "캐릭터 데이터를 찾을 수 없습니다. {0}", characterName.ToLogString());
                }

                return assetData;
            }

            return new CharacterAssetData();
        }

        public CharacterAssetData FindCharacterClone(int characterTID)
        {
            if (_characterAssets.ContainsKey(characterTID))
            {
                return _characterAssets[characterTID].Clone();
            }
            else
            {
                return new CharacterAssetData();
            }
        }

        #endregion Character FindClone Methods

        #region Character Load Methods

        /// <summary>
        /// 캐릭터 에셋을 동기적으로 로드합니다.
        /// </summary>
        private bool LoadCharacterSync(string filePath)
        {
            if (!filePath.Contains("Character_"))
            {
                return false;
            }

            CharacterAsset asset = ResourcesManager.LoadResource<CharacterAsset>(filePath);
            if (asset != null)
            {
                if (asset.TID == 0)
                {
                    Log.Warning(LogTags.ScriptableData, "{0}, 캐릭터 아이디가 설정되어있지 않습니다. {1}", asset.name, filePath);
                }
                else if (_characterAssets.ContainsKey(asset.TID))
                {
                    Log.Warning(LogTags.ScriptableData, "같은 TID로 중복 캐릭터가 로드 되고 있습니다. TID: {0}, 기존: {1}, 새로운 이름: {2}",
                         asset.TID, _characterAssets[asset.TID].name, asset.name);
                }
                else
                {
                    Log.Progress("스크립터블 데이터를 읽어왔습니다. Path: {0}", filePath);
                    _characterAssets[asset.TID] = asset;
                }

                return true;
            }
            else
            {
                Log.Warning("스크립터블 데이터를 읽을 수 없습니다. Path: {0}", filePath);
            }

            return false;
        }

        #endregion Character Load Methods

        #region Character Refresh Methods

        /// <summary>
        /// 모든 캐릭터 에셋을 리프레시합니다.
        /// </summary>
        public void RefreshAllCharacter()
        {
            foreach (KeyValuePair<int, CharacterAsset> item in _characterAssets) { Refresh(item.Value); }
        }

        private void Refresh(CharacterAsset characterAsset)
        {
            characterAsset?.Refresh();
        }

        #endregion Character Refresh Methods

        #region Character Validation Methods

        /// <summary>
        /// 캐릭터 에셋 유효성을 검사합니다.
        /// </summary>
        private void CheckValidCharactersOnLoadAssets()
        {
#if UNITY_EDITOR
            CharacterNames[] keys = EnumEx.GetValues<CharacterNames>();
            int tid = 0;
            for (int i = 1; i < keys.Length; i++)
            {
                tid = BitConvert.Enum32ToInt(keys[i]);
                if (!_characterAssets.ContainsKey(tid))
                {
                    Log.Warning(LogTags.ScriptableData, "캐릭터 에셋이 설정되지 않았습니다. {0}({1})", keys[i], keys[i].ToLogString());
                }
            }
#endif
        }

        #endregion Character Validation Methods
    }
}