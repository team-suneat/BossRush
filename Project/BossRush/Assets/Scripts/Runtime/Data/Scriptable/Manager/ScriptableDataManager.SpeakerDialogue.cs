using System.Collections.Generic;
using TeamSuneat;

namespace TeamSuneat.Data
{
    /// <summary>
    /// ScriptableDataManager의 화자 대화 관련 기능
    /// </summary>
    public partial class ScriptableDataManager
    {
        #region SpeakerDialogue Get Methods

        /// <summary>
        /// 화자 대화 에셋을 가져옵니다.
        /// </summary>
        public SpeakerDialogueAsset GetSpeakerDialogueAsset(CharacterNames speakerName)
        {
            int key = BitConvert.Enum32ToInt(speakerName);
            return _speakerDialogueAssets.TryGetValue(key, out var asset) ? asset : null;
        }

        #endregion SpeakerDialogue Get Methods

        #region SpeakerDialogue Find Methods

        /// <summary>
        /// 화자 대화 에셋을 찾습니다.
        /// </summary>
        public SpeakerDialogueAsset FindSpeakerDialogue(CharacterNames speakerName)
        {
            return FindSpeakerDialogue(BitConvert.Enum32ToInt(speakerName));
        }

        private SpeakerDialogueAsset FindSpeakerDialogue(int TID)
        {
            if (_speakerDialogueAssets.ContainsKey(TID))
            {
                return _speakerDialogueAssets[TID];
            }

            return null;
        }

        #endregion SpeakerDialogue Find Methods

        #region SpeakerDialogue Load Methods

        /// <summary>
        /// 화자 대화 에셋을 동기적으로 로드합니다.
        /// </summary>
        private bool LoadSpeakerDialogueSync(string filePath)
        {
            if (!filePath.Contains("SpeakerDialogue_"))
            {
                return false;
            }

            SpeakerDialogueAsset asset = ResourcesManager.LoadResource<SpeakerDialogueAsset>(filePath);

            if (asset != null)
            {
                if (asset.TID == 0)
                {
                    Log.Warning(LogTags.ScriptableData, "{0}, 화자 대화 아이디가 설정되어있지 않습니다. {1}", asset.name, filePath);
                }
                else if (_speakerDialogueAssets.ContainsKey(asset.TID))
                {
                    Log.Warning(LogTags.ScriptableData, "같은 TID로 중복 SpeakerDialogue가 로드 되고 있습니다. TID: {0}, 기존: {1}, 새로운 이름: {2}",
                         asset.TID, _speakerDialogueAssets[asset.TID].name, asset.name);
                }
                else
                {
                    Log.Progress("스크립터블 데이터를 읽어왔습니다. Path: {0}", filePath);
                    _speakerDialogueAssets[asset.TID] = asset;
                }

                return true;
            }
            else
            {
                Log.Warning("스크립터블 데이터를 읽을 수 없습니다. Path: {0}", filePath);
            }

            return false;
        }

        #endregion SpeakerDialogue Load Methods

        #region SpeakerDialogue Refresh Methods

        /// <summary>
        /// 모든 화자 대화 에셋을 리프레시합니다.
        /// </summary>
        public void RefreshAllSpeakerDialogue()
        {
            foreach (KeyValuePair<int, SpeakerDialogueAsset> item in _speakerDialogueAssets) { Refresh(item.Value); }
        }

        private void Refresh(SpeakerDialogueAsset speakerDialogueAsset)
        {
            speakerDialogueAsset?.Refresh();
        }

        #endregion SpeakerDialogue Refresh Methods
    }
}
