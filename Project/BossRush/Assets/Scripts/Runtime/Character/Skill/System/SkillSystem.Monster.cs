using System.Collections.Generic;

namespace TeamSuneat
{
    public partial class SkillSystem : XBehaviour
    {
        public SkillNames GetCastableSkill()
        {
            foreach (SkillEntity entity in _entities.Values)
            {
                if (entity.Level == 0)
                {
                    continue;
                }

                return entity.Name;
            }

            return SkillNames.None;
        }

        public List<SkillNames> GetCastableSkills()
        {
            List<SkillNames> result = new List<SkillNames>();

            foreach (SkillEntity entity in _entities.Values)
            {
                if (entity.Level == 0)
                {
                    continue;
                }

                result.Add(entity.Name);
            }

            return result;
        }
    }
}