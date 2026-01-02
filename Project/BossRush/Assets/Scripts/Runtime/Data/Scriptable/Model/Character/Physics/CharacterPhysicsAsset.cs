using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat.Data
{
    [CreateAssetMenu(fileName = "Character Physics", menuName = "TeamSuneat/Scriptable/Character Physics")]
    public class CharacterPhysicsAsset : XScriptableObject
    {
        public bool IsChangingAsset;

        [Title("Character")]
        [EnableIf("IsChangingAsset")]
        public CharacterNames Name;

        public CharacterPhysicsAssetData Data;

        public int TID => BitConvert.Enum32ToInt(Name);

        public CharacterPhysicsAssetData Clone()
        {
            return Data.Clone();
        }

        public void Paste(CharacterPhysicsAssetData physicsAssetData)
        {
            Data.Paste(physicsAssetData);
        }

        #region Editor

#if UNITY_EDITOR

        public override void Validate()
        {
            base.Validate();

            if (!IsChangingAsset)
            {
                if (!TSEnumEx.ConvertTo(ref Name, NameString))
                {
                    Log.Error("CharacterPhysics 에셋의 이름을 전환하지 못합니다. {0}", Name.ToLogString());
                }
            }
        }

        public override void Refresh()
        {
            if (Name != 0)
            {
                NameString = Name.ToString();
            }

            IsChangingAsset = false;
            base.Refresh();
        }

        public override void Rename()
        {
            Rename("Physics");
        }

#endif

        #endregion Editor
    }
}