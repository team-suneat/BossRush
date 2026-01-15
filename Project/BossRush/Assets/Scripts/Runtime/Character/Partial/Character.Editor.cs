using UnityEngine;

namespace TeamSuneat
{
    public partial class Character
    {

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            HeadPoint = this.FindTransform("Point/Point-Head");
            BodyPoint = this.FindTransform("Point/Point-Body");
            FootPoint = this.FindTransform("Point/Point-Foot");
        }

        public override void AutoSetting()
        {
            base.AutoSetting();
            if (Name != 0)
            {
                NameString = Name.ToString();
            }
        }

        private void OnValidate()
        {
            EnumEx.ConvertTo(ref Name, NameString);
        }

        public override void AutoNaming()
        {
            if (Application.isPlaying)
            {
                SetGameObjectName($"{NameString}({SID.ToString()})");
            }
            else
            {
                SetGameObjectName(NameString);
            }
        }
    }
}