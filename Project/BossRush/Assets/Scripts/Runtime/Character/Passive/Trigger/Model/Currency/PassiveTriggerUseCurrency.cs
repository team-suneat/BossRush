

namespace TeamSuneat.Passive
{
    public class PassiveTriggerUseCurrency : PassiveTriggerReceiver
    {
        private CurrencyNames _currencyName;

        protected override PassiveTriggers Trigger => PassiveTriggers.UseCurrency;

        protected override void Register()
        {
            base.Register();

            GlobalEvent<CurrencyNames>.Register(GlobalEventType.CURRENCY_PAYED, OnGlobalEvent);
        }

        protected override void Unregister()
        {
            base.Unregister();

            GlobalEvent<CurrencyNames>.Unregister(GlobalEventType.CURRENCY_PAYED, OnGlobalEvent);
        }

        private void OnGlobalEvent(CurrencyNames currencyName)
        {
            _currencyName = currencyName;

            if (TryExecute())
            {
                Execute();
            }
        }

        public override bool TryExecute()
        {
            if (!base.TryExecute())
            {
                return false;
            }

            if (!Checker.CheckTriggerCount(Entity.TriggerCount))
            {
                Entity.AddTriggerCount();

                return false;
            }
            else if (!Checker.CheckTriggerCurrency(_currencyName))
            {
                return false;
            }

            return CheckConditions();
        }
    }
}