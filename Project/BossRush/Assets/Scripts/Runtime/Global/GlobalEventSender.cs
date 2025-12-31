using TeamSuneat;

namespace TeamSuneat
{
    public class GlobalEventSender : XBehaviour
    {
        public GlobalEventType Type;

        public string TypeString;

        private void OnValidate()
        {
            EnumEx.ConvertTo(ref Type, TypeString);
        }

        public override void AutoSetting()
        {
            base.AutoSetting();

            if (Type != GlobalEventType.NONE)
            {
                TypeString = Type.ToString();
            }
        }

        public void Send()
        {
            GlobalEvent.Send(Type);
        }
    }
}