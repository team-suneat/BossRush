using UnityEngine;

namespace TeamSuneat
{
    public partial class Character
    {
        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            CharacterAnimator = GetComponentInChildren<CharacterAnimator>();
            CharacterRenderer = GetComponentInChildren<CharacterRenderer>();
            Animator = this.FindComponent<Animator>("Model");

            Attack = GetComponentInChildren<AttackSystem>();
            Buff = GetComponentInChildren<BuffSystem>();
            Passive = GetComponentInChildren<PassiveSystem>();
            Stat = GetComponentInChildren<StatSystem>();
            MyVital = GetComponentInChildren<Vital>();

            Abilities = GetComponents<CharacterAbility>();
            CharacterModel = this.FindGameObject("Model");
        }

        public override void AutoSetting()
        {
            base.AutoSetting();
            RefreshNameString();
        }

        public override void AutoNaming()
        {
            if (Application.isPlaying)
            {
                SetGameObjectName($"{NameString}({SID.ToString()})");
            }
            else
            {
                SetGameObjectName(NameString);
            }
        }

        private void OnValidate()
        {
            EnumEx.ConvertTo(ref Name, NameString);
        }

        private void RefreshNameString()
        {
            if (Name != 0)
            {
                NameString = Name.ToString();
            }
        }
    }
}