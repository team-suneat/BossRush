namespace TeamSuneat
{
    public partial class Character
    {
        private void InitializeAbilities()
        {
            LogInfo("캐릭터의 어빌리티를 초기화합니다.");

            if (Abilities.IsValid())
            {
                for (int i = 0; i < Abilities.Length; i++)
                {
                    Abilities[i].Initialization();
                }
            }
            else
            {
                LogWarning("캐릭터의 어빌리티가 배열이 설정되어있지 않습니다.");
            }
        }

        public void ResetAbilities()
        {
            if (Abilities.IsValid())
            {
                for (int i = 0; i < Abilities.Length; i++)
                {
                    Abilities[i].ResetAbility();
                }
            }
            else
            {
                Log.Error("캐릭터의 어빌리티가 배열이 설정되어있지 않습니다. {0}", this.GetHierarchyName());
            }
        }

        protected virtual void EarlyProcessAbilities()
        {
            if (Abilities.IsValid())
            {
                for (int i = 0; i < Abilities.Length; i++)
                {
                    if (Abilities[i] == null) { continue; }
                    if (Abilities[i].enabled)
                    {
                        if (Abilities[i].AbilityInitialized)
                        {
                            Abilities[i].EarlyProcessAbility();
                        }
                    }
                }
            }
        }

        protected virtual void ProcessAbilities()
        {
            if (Abilities != null)
            {
                for (int i = 0; i < Abilities.Length; i++)
                {
                    if (Abilities[i] == null)
                    {
                        continue;
                    }

                    if (!Abilities[i].enabled)
                    {
                        continue;
                    }

                    if (!Abilities[i].AbilityInitialized)
                    {
                        continue;
                    }

                    Abilities[i].ProcessAbility();
                }
            }

            if (MyVital != null)
            {
                MyVital.ProcessAbility();
            }
        }

        protected virtual void LateProcessAbilities()
        {
            if (Abilities != null)
            {
                for (int i = 0; i < Abilities.Length; i++)
                {
                    if (Abilities[i] == null)
                    {
                        continue;
                    }

                    if (!Abilities[i].enabled)
                    {
                        continue;
                    }

                    if (!Abilities[i].AbilityInitialized)
                    {
                        continue;
                    }

                    Abilities[i].LateProcessAbility();
                }
            }
        }

        protected virtual void PhysicsProcessAbilities()
        {
            if (Abilities != null)
            {
                for (int i = 0; i < Abilities.Length; i++)
                {
                    if (Abilities[i] == null)
                    {
                        continue;
                    }

                    if (!Abilities[i].enabled)
                    {
                        continue;
                    }

                    if (!Abilities[i].AbilityInitialized)
                    {
                        continue;
                    }

                    Abilities[i].PhysicsProcessAbility();
                }
            }
        }

        public T FindAbility<T>() where T : CharacterAbility
        {
            if (Abilities.IsValid())
            {
                for (int i = 0; i < Abilities.Length; i++)
                {
                    if (Abilities[i] is T characterAbility)
                    {
                        return characterAbility;
                    }
                }
            }

            return null;
        }
    }
}