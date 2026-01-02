

namespace TeamSuneat.Passive
{
    public class PassiveTriggerStopSkillCooldown : PassiveTriggerReceiver
    {
        private SkillNames _skillName;

        protected override PassiveTriggers Trigger => PassiveTriggers.StopSkillCooldown;

        protected override void Register()
        {
            base.Register();

            GlobalEvent<SkillNames>.Register(GlobalEventType.PLAYER_CHARACTER_STOP_SKILL_COOLDOWN, OnGlobalEvent);
        }

        protected override void Unregister()
        {
            base.Unregister();

            GlobalEvent<SkillNames>.Unregister(GlobalEventType.PLAYER_CHARACTER_STOP_SKILL_COOLDOWN, OnGlobalEvent);
        }

        private void OnGlobalEvent(SkillNames skillName)
        {
            _skillName = skillName;

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

            if (!Checker.CheckTriggerSkill(_skillName))
            {
                return false;
            }
            else if (!Checker.CheckTriggerSkillCategory(_skillName))
            {
                return false;
            }
            else if (!Checker.CheckTriggerSkillElement(_skillName))
            {
                return false;
            }
            else if (!Checker.CheckTriggerMonsterRange())
            {
                return false;
            }

            return CheckConditions();
        }

        public override void Execute()
        {
            if (Entity != null)
            {
                TriggerInfo.SkillName = _skillName;
            }

            base.Execute();
        }
    }
}