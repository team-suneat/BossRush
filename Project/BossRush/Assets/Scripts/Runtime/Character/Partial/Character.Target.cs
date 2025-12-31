namespace TeamSuneat
{
    public partial class Character
    {
        public bool CheckTargetCharacterAlive()
        {
            return TargetCharacter != null && TargetCharacter.IsAlive;
        }

        public virtual void SetTarget(Vital targetVital)
        {
            if (targetVital == null)
            {
                return;
            }

            if (targetVital.Owner == null)
            {
                return;
            }

            TargetCharacter = targetVital.Owner;
            LogInfo("캐릭터의 타겟을 설정합니다: {0}", TargetCharacter.GetHierarchyName());
        }

        public virtual void SetTarget(Character targetCharacter)
        {
            if (targetCharacter != null)
            {
                TargetCharacter = targetCharacter;
                LogInfo("캐릭터의 타겟을 설정합니다: {0}", targetCharacter.GetHierarchyName());
            }
        }

        public virtual void ResetTarget()
        {
            LogInfo("캐릭터의 타겟을 초기화합니다.");
            TargetCharacter = null;
        }

        public virtual void SetTargetCamp(CharacterCamps characterCamp)
        {
        }
    }
}