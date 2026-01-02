using System;
using Sirenix.OdinInspector;
using UnityEngine;
using TeamSuneat;

namespace TeamSuneat.Data
{
    [Serializable]
    public partial class CharacterAssetData : ScriptableData<int>
    {
        public bool IsChangingAsset;

        [GUIColor("GetCharacterNameColor")]
        public CharacterNames Name;

        public bool SuperArmor;
        public bool IsFlying;

        [FoldoutGroup("#String")] public string NameAsString;
    }
}

