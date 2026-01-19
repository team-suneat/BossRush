using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using TeamSuneat;

namespace TeamSuneat.Data
{
    [Serializable]
    public class CharacterStatEntry
    {
        [SuffixLabel("능력치 이름")]
        public StatNames Name;

        [SuffixLabel("능력치 값")]
        public float Value;

        [FoldoutGroup("#String")] public string NameAsString;

        public string GetStatEntryLabel()
        {
            if (Name == StatNames.None)
            {
                return "None";
            }
            return $"{Name}: {Value}";
        }
    }

    [Serializable]
    public class CharacterAssetData : ScriptableData<int>
    {
        public bool IsChangingAsset;

        [GUIColor("GetCharacterNameColor")]
        public CharacterNames Name;

        public bool SuperArmor;
        public bool IsFlying;

        [FoldoutGroup("#능력치")]
        [ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "GetStatEntryLabel")]
        public List<CharacterStatEntry> Stats = new();

        public override int GetKey()
        {
            return BitConvert.Enum32ToInt(Name);
        }

        public override void Refresh()
        {
            base.Refresh();

            if (Stats != null)
            {
                for (int i = 0; i < Stats.Count; i++)
                {
                    Stats[i].NameAsString = Stats[i].Name.ToString();
                }
            }

            IsChangingAsset = false;
        }

        public override void OnLoadData()
        {
            base.OnLoadData();

            CustomLog();
        }

        public void Validate()
        {
            if (!IsChangingAsset)
            {
                if (Stats != null)
                {
                    for (int i = 0; i < Stats.Count; i++)
                    {
                        if (!EnumEx.ConvertTo(ref Stats[i].Name, Stats[i].NameAsString))
                        {
                            Log.Error("CharacterAssetData의 Stats[{0}] NameAsString 변수를 변환할 수 없습니다. {1} ({2}), {3}", i, Stats[i].Name, Stats[i].Name.ToLogString(), Stats[i].NameAsString);
                        }
                    }
                }
            }
        }

        public CharacterAssetData Clone()
        {
            CharacterAssetData assetData = new()
            {
                Name = Name,
                IsChangingAsset = IsChangingAsset,
                SuperArmor = SuperArmor,
                IsFlying = IsFlying,
                Stats = new List<CharacterStatEntry>()
            };

            if (Stats != null)
            {
                for (int i = 0; i < Stats.Count; i++)
                {
                    assetData.Stats.Add(new CharacterStatEntry
                    {
                        Name = Stats[i].Name,
                        Value = Stats[i].Value,
                        NameAsString = Stats[i].NameAsString
                    });
                }
            }

            return assetData;
        }

        private void CustomLog()
        {
#if UNITY_EDITOR
            if (Name == CharacterNames.None)
            {
                Log.Error("캐릭터의 이름이 설정되지 않았습니다: {0}", Name);
            }
#endif
        }

#if UNITY_EDITOR

        public bool RefreshWithoutSave()
        {
            _hasChangedWhiteRefreshAll = false;

            if (Stats != null)
            {
                for (int i = 0; i < Stats.Count; i++)
                {
                    UpdateIfChanged(ref Stats[i].NameAsString, Stats[i].Name);
                }
            }

            IsChangingAsset = false;

            return _hasChangedWhiteRefreshAll;
        }

        private bool _hasChangedWhiteRefreshAll = false;

        private void UpdateIfChanged<TEnum>(ref string target, TEnum newValue) where TEnum : System.Enum
        {
            string newString = newValue?.ToString();
            if (target != newString)
            {
                target = newString;
                _hasChangedWhiteRefreshAll = true;
            }
        }

        #region Inspector Color Methods

        // 필요시 추가 색상 메서드

        #endregion Inspector Color Methods

#endif
    }
}
