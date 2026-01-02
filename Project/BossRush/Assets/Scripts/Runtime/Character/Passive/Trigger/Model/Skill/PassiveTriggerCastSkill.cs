using TeamSuneat.Data;


namespace TeamSuneat.Passive
{
    public class PassiveTriggerCastSkill : PassiveTriggerReceiver
    {
        private SkillNames _skillName;

        protected override PassiveTriggers Trigger => PassiveTriggers.CastSkill;

        protected override void Register()
        {
            base.Register();

            GlobalEvent<SkillNames>.Register(GlobalEventType.PLAYER_CHARACTER_CAST_SKILL, OnGlobalEvent);
        }

        protected override void Unregister()
        {
            base.Unregister();

            GlobalEvent<SkillNames>.Unregister(GlobalEventType.PLAYER_CHARACTER_CAST_SKILL, OnGlobalEvent);
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
            else if (!Checker.CheckTriggerSkill(_skillName))
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
            else if (!CheckConditions())
            {
                return false;
            }

            return true;
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