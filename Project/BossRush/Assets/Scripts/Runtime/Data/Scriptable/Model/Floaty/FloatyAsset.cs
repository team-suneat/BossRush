using DG.Tweening;
using Sirenix.OdinInspector;
using TeamSuneat.Setting;
using UnityEngine;

namespace TeamSuneat.Data
{
    [CreateAssetMenu(fileName = "FloatyAsset", menuName = "TeamSuneat/Scriptable/Floaty")]
    public class FloatyAsset : XScriptableObject
    {
        public bool IsChangingAsset;

        [EnableIf("IsChangingAsset")]
        public UIFloatyMoveNames Name;

        
        [EnableIf("IsChangingAsset")]
        [FoldoutGroup("#Font")] public GameFontTypes FontType;
        [FoldoutGroup("#Font")] public Color TextColor;

        [FoldoutGroup("#Spawn")] public Vector2 SpawnArea;
        [FoldoutGroup("#Spawn")] public Vector3 SpawnOffset;

        [EnableIf("IsChangingAsset")][FoldoutGroup("#Move")] public UIFloatyMoveTypes Type;

        [FoldoutGroup("#Move")] public bool RandomFace;
        [FoldoutGroup("#Time")] public float DelayTime;
        [FoldoutGroup("#Time")] public float Duration;
        [FoldoutGroup("#Velocity")][EnableIf("Type", UIFloatyMoveTypes.Velocity)] public Vector3 Velocity;
        [FoldoutGroup("#Jump")][EnableIf("Type", UIFloatyMoveTypes.Jump)] public Vector3 JumpEndValue;
        [FoldoutGroup("#Jump")][EnableIf("Type", UIFloatyMoveTypes.Jump)] public float JumpPower;
        [FoldoutGroup("#Jump")][EnableIf("Type", UIFloatyMoveTypes.Jump)] public int NumberOfJumps;

        [FoldoutGroup("#Fade")] public bool UseFadeOut;
        [FoldoutGroup("#Fade")][EnableIf("UseFadeOut")] public float FadeDuration;
        [FoldoutGroup("#Fade")][EnableIf("UseFadeOut")] public float FadeDelayTime;
        [FoldoutGroup("#Fade")][EnableIf("UseFadeOut")][Range(0f, 1f)] public float FadeTargetAlpha;

        [FoldoutGroup("#PunchScale")] public bool UsePunchScale;
        [FoldoutGroup("#PunchScale")][EnableIf("UsePunchScale")] public Ease PunchEase = Ease.Linear;
        [FoldoutGroup("#PunchScale")][EnableIf("UsePunchScale")] public Vector2 PunchScale;
        [FoldoutGroup("#PunchScale")][EnableIf("UsePunchScale")] public float PunchScaleDuration;
        [FoldoutGroup("#PunchScale")][EnableIf("UsePunchScale")] public int PunchScaleVibrato;

        [FoldoutGroup("#String")] public string TypeString;
        [FoldoutGroup("#String")] public string FontTypeString;

        public int TID => BitConvert.Enum32ToInt(Name);

#if UNITY_EDITOR

        public override void Validate()
        {
            base.Validate();

            if (!IsChangingAsset)
            {
                EnumEx.ConvertTo(ref Name, NameString);
                EnumEx.ConvertTo(ref Type, TypeString);
                EnumEx.ConvertTo(ref FontType, FontTypeString);
            }
        }

        public override void Refresh()
        {
            if (Name != 0)
            {
                NameString = Name.ToString();
            }

            if (Type != 0)
            {
                TypeString = Type.ToString();
            }

            if (FontType != 0)
            {
                FontTypeString = FontType.ToString();
            }

            IsChangingAsset = false;
            base.Refresh();
        }

        public override void Rename()
        {
            Rename("Floaty");
        }

#endif
    }
}