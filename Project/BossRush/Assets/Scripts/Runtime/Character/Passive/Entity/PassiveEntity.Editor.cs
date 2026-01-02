using Lean.Pool;

namespace TeamSuneat.Passive
{
    public partial class PassiveEntity : XBehaviour, IPoolable
    {
        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            Owner = this.FindFirstParentComponent<Character>();
        }

        public override void AutoSetting()
        {
            base.AutoSetting();

            if (Name != 0)
            {
                NameString = Name.ToString();
            }
        }

        public override void AutoNaming()
        {
            SetGameObjectName(Name.ToString());
        }

        private void OnValidate()
        {
            EnumEx.ConvertTo(ref Name, NameString);
        }
    }
}