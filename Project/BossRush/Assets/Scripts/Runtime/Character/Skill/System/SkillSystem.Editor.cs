using TeamSuneat.Data;

namespace TeamSuneat
{
    public partial class SkillSystem : XBehaviour
    {
        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            Owner = this.FindFirstParentComponent<Character>();
        }

        public override void AutoNaming()
        {
            base.AutoNaming();

            if (Owner != null)
            {
                SetGameObjectName(string.Format("#Skill({0})", Owner.Name.ToString()));
            }
            else
            {
                SetGameObjectName("#Skill");
            }
        }

        private void OnAddSkillEntity(SkillAssetData skillData, SkillEntity skillEntity)
        {
            skillEntity.Name = skillData.Name;

            skillEntity.AutoAddComponents();
            skillEntity.AutoSetting();
            skillEntity.AutoNaming();
        }
    }
}