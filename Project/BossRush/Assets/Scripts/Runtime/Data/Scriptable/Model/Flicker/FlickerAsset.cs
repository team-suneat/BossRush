using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat.Data
{
    [CreateAssetMenu(fileName = "FlickerAsset", menuName = "TeamSuneat/Scriptable/Flicker")]
    public class FlickerAsset : XScriptableObject
    {
        public bool IsChangingAsset;

        [EnableIf("IsChangingAsset")]
        public RendererFlickerNames Name;

        [SuffixLabel("스프라이트가 깜박일 색상")]
        public Color FlickerColor = new Color32(255, 255, 255, 255);

        [SuffixLabel("스프라이트가 깜박일 색상 블랜드")]
        public float FlickerBlend = 0.5f;

        [SuffixLabel("대상이 피해를 입은 후 깜박이는 간격 시간(초)")]
        public float FlickerSpeed = 0.1f;

        [SuffixLabel("대상이 피해를 입은 후 깜박이는 지속 시간(초)")]
        public float FlickerDuration = 0.2f;

        public int TID => BitConvert.Enum32ToInt(Name);

        public override void Validate()
        {
            base.Validate();

            if (!IsChangingAsset)
            {
                EnumEx.ConvertTo(ref Name, NameString);
            }
        }

        public override void Rename()
        {
            Rename("Flicker");
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
    }
}