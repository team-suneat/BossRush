namespace TeamSuneat
{
    public class PassiveSystem : XBehaviour
    {
        public Character Owner;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            Owner = this.FindFirstParentComponent<Character>();
        }

        public override void AutoNaming()
        {
            SetGameObjectName(string.Format("#Passive({0})", Owner.Name));
        }

        internal void LogicUpdate()
        {
        }

        internal void Clear()
        {
        }
    }
}