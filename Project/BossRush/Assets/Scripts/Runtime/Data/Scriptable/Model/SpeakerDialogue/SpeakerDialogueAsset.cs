using Sirenix.OdinInspector;
using TeamSuneat;
using UnityEngine;

namespace TeamSuneat.Data
{
    [CreateAssetMenu(fileName = "SpeakerDialogueAsset", menuName = "TeamSuneat/Scriptable/SpeakerDialogue")]
    public class SpeakerDialogueAsset : XScriptableObject
    {
        public bool IsChangingAsset;

        [EnableIf("IsChangingAsset")]
        public CharacterNames SpeakerName;

        [FoldoutGroup("#Visual")] public Color TextColor = Color.white;
        [FoldoutGroup("#Visual")] public string PortraitSpriteName;
        [FoldoutGroup("#Visual")] public Vector3 SpeechBubbleOffset;

        [FoldoutGroup("#String")] public string SpeakerNameString;

        public int TID => BitConvert.Enum32ToInt(SpeakerName);

#if UNITY_EDITOR

        public override void Validate()
        {
            base.Validate();

            if (!IsChangingAsset)
            {
                EnumEx.ConvertTo(ref SpeakerName, SpeakerNameString);
            }
        }

        public override void Refresh()
        {
            if (SpeakerName != CharacterNames.None)
            {
                SpeakerNameString = SpeakerName.ToString();
            }

            IsChangingAsset = false;
            base.Refresh();
        }

        public override void Rename()
        {
            Rename("SpeakerDialogue");
        }

#endif
    }
}
