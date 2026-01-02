using Lean.Pool;


namespace TeamSuneat
{
    public partial class SkillEntity : XBehaviour, IPoolable
    {
        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            Owner = this.FindFirstParentComponent<Character>();

            AutoGetFeedbackComponents();
        }

        public override void AutoSetting()
        {
            base.AutoSetting();

            if (Name == SkillNames.None)
            {
                return;
            }

            NameString = Name.ToString();
            NameContent = Name.GetLocalizedString();
        }

        private void OnValidate()
        {
            TSEnumEx.ConvertTo(ref Name, NameString);
        }

        public override void AutoNaming()
        {
            SetGameObjectName(Name.ToString());
        }
    }
}