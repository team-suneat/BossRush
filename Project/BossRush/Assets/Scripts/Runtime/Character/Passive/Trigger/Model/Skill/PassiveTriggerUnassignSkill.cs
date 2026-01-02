

namespace TeamSuneat.Passive
{
    public class PassiveTriggerUnassignSkill : PassiveTriggerReceiver
    {
        private SkillNames _skillName;

        protected override PassiveTriggers Trigger => PassiveTriggers.UnassignSkill;

        protected override void Register()
        {
            base.Register();

            GlobalEvent<SkillNames>.Register(GlobalEventType.GAME_DATA_UNASSIGN_CHARACTER_SKILL, OnGlobalEvent);
        }

        protected override void Unregister()
        {
            base.Unregister();

            GlobalEvent<SkillNames>.Unregister(GlobalEventType.GAME_DATA_UNASSIGN_CHARACTER_SKILL, OnGlobalEvent);
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
            else if (!Checker.CheckDifferentSkillCount())
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