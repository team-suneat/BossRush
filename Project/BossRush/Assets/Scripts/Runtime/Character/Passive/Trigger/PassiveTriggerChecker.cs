using TeamSuneat.Data;
using TeamSuneat.Data.Game;

namespace TeamSuneat
{
    public partial class PassiveTriggerChecker
    {
        private Character _owner;

        private PassiveTriggerSettings _triggerSettings;

        private PassiveConditionSettings _conditionSettings;

        private PassiveNames PassiveName => _triggerSettings.Name;

        private VProfile ProfileInfo => GameApp.GetSelectedProfile();

        public void Setup(Character owner, PassiveTriggerSettings triggerSettings, PassiveConditionSettings conditionSettings)
        {
            _owner = owner;
            _triggerSettings = triggerSettings;
            _conditionSettings = conditionSettings;
        }
    }
}