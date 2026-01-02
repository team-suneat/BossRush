

namespace TeamSuneat.Passive
{
    public class PassiveTriggerAssignSkill : PassiveTriggerReceiver
    {
        private SkillNames _skillName;

        protected override PassiveTriggers Trigger => PassiveTriggers.AssignSkill;

        protected override void Register()
        {
            base.Register();

            GlobalEvent<ActionNames, SkillNames>.Register(GlobalEventType.GAME_DATA_ASSIGN_CHARACTER_SKILL, OnGlobalEvent);
        }

        protected override void Unregister()
        {
            base.Unregister();

            GlobalEvent<ActionNames, SkillNames>.Unregister(GlobalEventType.GAME_DATA_ASSIGN_CHARACTER_SKILL, OnGlobalEvent);
        }

        private void OnGlobalEvent(ActionNames actionName, SkillNames skillName)
        {
            _skillName = skillName;

            if(TryExecute())
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