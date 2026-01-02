using UnityEditor;
using UnityEngine;
using TeamSuneat;

namespace TeamSuneat.Data
{
    [CreateAssetMenu(fileName = "Character", menuName = "TeamSuneat/Scriptable/Character")]
    public class CharacterAsset : XScriptableObject
    {
        public int TID => BitConvert.Enum32ToInt(Data.Name);

        public CharacterNames Name => Data.Name;

        public CharacterAssetData Data;

        public override void OnLoadData()
        {
            base.OnLoadData();

            LogError();
            EnumLog();

            Data.OnLoadData();
        }

        private void LogError()
        {
#if UNITY_EDITOR

            if (Data.IsChangingAsset)
            {
                Log.Error("Asset의 IsChangingAsset 변수가 활성화되어있습니다. {0}", name);
            }
            if (Data.Name == CharacterNames.None)
            {
                Log.Warning(LogTags.ScriptableData, "[Character] Character Asset의 Name 변수가 설정되지 않았습니다. {0}", name);
            }

#endif
        }

        private void EnumLog()
        {
#if UNITY_EDITOR
            string type = "CharacterAsset".ToSelectString();
            // 필요시 EnumExplorer 로그 추가
#endif
        }

#if UNITY_EDITOR

        public override void Validate()
        {
            base.Validate();

            if (!Data.IsChangingAsset)
            {
                _ = EnumEx.ConvertTo(ref Data.Name, NameString);
            }

            Data.Validate();
        }

        public override void Refresh()
        {
            NameString = Data.Name.ToString();
            Data.Refresh();

            base.Refresh();
        }

        public override bool RefreshWithoutSave()
        {
            _hasChangedWhiteRefreshAll = false;

            UpdateIfChanged(ref NameString, Data.Name);
            if (Data.RefreshWithoutSave())
            {
                _hasChangedWhiteRefreshAll = true;
            }

            _ = base.RefreshWithoutSave();

            return _hasChangedWhiteRefreshAll;
        }

        public override void Rename()
        {
            Rename("Character");
        }

        protected override void RefreshAll()
        {
#if UNITY_EDITOR
            if (Selection.objects.Length > 1)
            {
                Debug.LogWarning("여러 개의 스크립터블 오브젝트가 선택되었습니다. 하나만 선택한 상태에서 실행하세요.");
                return;
            }
#endif
            CharacterNames[] characterNames = EnumEx.GetValues<CharacterNames>();
            int characterCount = 0;

            Log.Info("모든 캐릭터 에셋의 갱신을 시작합니다: {0}", characterNames.Length);

            base.RefreshAll();

            for (int i = 1; i < characterNames.Length; i++)
            {
                if (characterNames[i] != CharacterNames.None)
                {
                    CharacterAsset asset = ScriptableDataManager.Instance.FindCharacter(characterNames[i]);
                    if (asset.IsValid())
                    {
                        if (asset.RefreshWithoutSave())
                        {
                            characterCount += 1;
                        }
                    }
                }

                float progressRate = (i + 1).SafeDivide(characterNames.Length);
                EditorUtility.DisplayProgressBar("모든 캐릭터 에셋의 갱신", characterNames[i].ToString(), progressRate);
            }

            EditorUtility.ClearProgressBar();
            OnRefreshAll();

            Log.Info("모든 캐릭터 에셋의 갱신을 종료합니다: {0}/{1}", characterCount.ToSelectString(characterNames.Length), characterNames.Length);
        }

        protected override void CreateAll()
        {
            base.CreateAll();

            CharacterNames[] characterNames = EnumEx.GetValues<CharacterNames>();
            for (int i = 1; i < characterNames.Length; i++)
            {
                if (characterNames[i] is CharacterNames.None)
                {
                    continue;
                }

                CharacterAsset asset = ScriptableDataManager.Instance.FindCharacter(characterNames[i]);
                if (asset == null)
                {
                    asset = CreateAsset<CharacterAsset>("Character", characterNames[i].ToString(), true);
                    if (asset != null)
                    {
                        asset.Data = new CharacterAssetData
                        {
                            Name = characterNames[i]
                        };
                        asset.NameString = characterNames[i].ToString();
                    }
                }
            }

            PathManager.UpdatePathMetaData();
        }

#endif

        public CharacterAssetData Clone()
        {
            return Data.Clone();
        }
    }
}

