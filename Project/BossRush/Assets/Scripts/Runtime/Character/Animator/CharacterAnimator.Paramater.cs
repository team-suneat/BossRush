using System.Collections.Generic;
using UnityEngine;

namespace TeamSuneat
{
    public partial class CharacterAnimator : XBehaviour, IAnimatorStateMachine
    {
        protected int _damageTriggerIndex;
        protected int _damageTypeIndex;

        protected HashSet<int> AnimatorParameters { get; set; } = new HashSet<int>();

        // bool parameter

        private const string ANIMATOR_IS_SPAWNING_PARAMETER_NAME = "IsSpawning";
        private const string ANIMATOR_IS_SPAWNED_PARAMETER_NAME = "IsSpawned";
        private const string ANIMATOR_IS_ATTACKING_PARAMETER_NAME = "IsAttacking";
        private const string ANIMATOR_IS_ATTACKING_1_PARAMETER_NAME = "IsAttacking1";
        private const string ANIMATOR_IS_ATTACKING_2_PARAMETER_NAME = "IsAttacking2";
        private const string ANIMATOR_IS_ATTACKING_3_PARAMETER_NAME = "IsAttacking3";
        private const string ANIMATOR_IS_DASING_PARAMETER_NAME = "IsDashing";
        private const string ANIMATOR_IS_GRABBING_PARAMETER_NAME = "IsGrabbing";
        private const string ANIMATOR_IS_DAMAGING_PARAMETER_NAME = "IsDamaging";
        private const string ANIMATOR_IS_CONSUMING_POTION_PARAMETER_NAME = "IsConsumingPotion";
        private const string ANIMATOR_IS_TURNING_PARAMETER_NAME = "IsTurning";
        private const string ANIMATOR_IS_CASTING_PARAMETER_NAME = "IsSkilling";

        // trigger parameter

        private const string ANIMATOR_SPAWN_PARAMETER_NAME = "Spawn";
        private const string ANIMATOR_SPECIAL_IDLE_PARAMETER_NAME = "SpecialIdle";
        private const string ANIMATOR_INTERACT_PARAMETER_NAME = "Interact";
        private const string ANIMATOR_BLINK_PARAMETER_NAME = "Blink";
        private const string ANIMATOR_TELEPORT_PARAMETER_NAME = "Teleport";
        private const string ANIMATOR_TURN_PARAMETER_NAME = "Turn";

        protected const string ANIMATOR_DAMAGE_PARAMETER_NAME = "Damage";
        protected const string ANIMATOR_DEATH_PARAMETER_NAME = "Death";

        // float parameter

        private const string ANIMATOR_ATTACK_SPEED_PARAMETER_NAME = "AttackSpeed";
        private const string ANIMATOR_MOVE_SPEED_PARAMETER_NAME = "MoveSpeed";
        private const string ANIMATOR_BOSS_PHASE_PARAMETER_NAME = "Phase";
        private const string ANIMATOR_DAMAGE_TYPE_PARAMETER_NAME = "DamageType";
        private const string ANIMATOR_WEAPON_TYPE_PARAMETER_NAME = "WeaponType";

        private int ANIMATOR_IS_SPAWNING_PARAMETER_ID;
        private int ANIMATOR_IS_SPAWNED_PARAMETER_ID;

        private int ANIMATOR_IS_ATTACKING_PARAMETER_ID;
        private int ANIMATOR_IS_ATTACKING_1_PARAMETER_ID;
        private int ANIMATOR_IS_ATTACKING_2_PARAMETER_ID;
        private int ANIMATOR_IS_ATTACKING_3_PARAMETER_ID;
        private int ANIMATOR_IS_DASING_PARAMETER_ID;
        private int ANIMATOR_IS_GRABBING_PARAMETER_ID;
        private int ANIMATOR_IS_DAMAGING_PARAMETER_ID;
        private int ANIMATOR_IS_CONSUMING_POTION_PARAMETER_ID;
        private int ANIMATOR_IS_TURNING_PARAMETER_ID;
        private int ANIMATOR_IS_CASTING_PARAMETER_ID;

        private int ANIMATOR_SPAWN_PARAMETER_ID;
        private int ANIMATOR_SPECIAL_IDLE_PARAMETER_ID;
        private int ANIMATOR_INTERACT_PARAMETER_ID;
        private int ANIMATOR_BLINK_PARAMETER_ID;
        private int ANIMATOR_TELEPORT_PARAMETER_ID;
        private int ANIMATOR_TURN_PARAMETER_ID;

        protected int ANIMATOR_DAMAGE_PARAMETER_ID;
        protected int ANIMATOR_DEATH_PARAMETER_ID;

        private int ANIMATOR_ATTACK_SPEED_PARAMETER_ID;
        private int ANIMATOR_MOVE_SPEED_PARAMETER_ID;
        private int ANIMATOR_BOSS_PHASE_PARAMETER_ID;
        private int ANIMATOR_DAMAGE_TYPE_PARAMETER_ID;
        protected int ANIMATOR_WEAPON_TYPE_PARAMETER_ID;

        protected virtual void InitializeAnimatorParameters()
        {
            AnimatorParameters.Clear();

            _animator.AddAnimatorParameterIfExists(ANIMATOR_IS_SPAWNING_PARAMETER_NAME, out ANIMATOR_IS_SPAWNING_PARAMETER_ID, AnimatorControllerParameterType.Bool, AnimatorParameters);
            _animator.AddAnimatorParameterIfExists(ANIMATOR_IS_SPAWNED_PARAMETER_NAME, out ANIMATOR_IS_SPAWNED_PARAMETER_ID, AnimatorControllerParameterType.Bool, AnimatorParameters);

            _animator.AddAnimatorParameterIfExists(ANIMATOR_IS_ATTACKING_PARAMETER_NAME, out ANIMATOR_IS_ATTACKING_PARAMETER_ID, AnimatorControllerParameterType.Bool, AnimatorParameters);
            _animator.AddAnimatorParameterIfExists(ANIMATOR_IS_ATTACKING_1_PARAMETER_NAME, out ANIMATOR_IS_ATTACKING_1_PARAMETER_ID, AnimatorControllerParameterType.Bool, AnimatorParameters);
            _animator.AddAnimatorParameterIfExists(ANIMATOR_IS_ATTACKING_2_PARAMETER_NAME, out ANIMATOR_IS_ATTACKING_2_PARAMETER_ID, AnimatorControllerParameterType.Bool, AnimatorParameters);
            _animator.AddAnimatorParameterIfExists(ANIMATOR_IS_ATTACKING_3_PARAMETER_NAME, out ANIMATOR_IS_ATTACKING_3_PARAMETER_ID, AnimatorControllerParameterType.Bool, AnimatorParameters);
            _animator.AddAnimatorParameterIfExists(ANIMATOR_IS_DASING_PARAMETER_NAME, out ANIMATOR_IS_DASING_PARAMETER_ID, AnimatorControllerParameterType.Bool, AnimatorParameters);
            _animator.AddAnimatorParameterIfExists(ANIMATOR_IS_GRABBING_PARAMETER_NAME, out ANIMATOR_IS_GRABBING_PARAMETER_ID, AnimatorControllerParameterType.Bool, AnimatorParameters);
            _animator.AddAnimatorParameterIfExists(ANIMATOR_IS_DAMAGING_PARAMETER_NAME, out ANIMATOR_IS_DAMAGING_PARAMETER_ID, AnimatorControllerParameterType.Bool, AnimatorParameters);
            _animator.AddAnimatorParameterIfExists(ANIMATOR_IS_CONSUMING_POTION_PARAMETER_NAME, out ANIMATOR_IS_CONSUMING_POTION_PARAMETER_ID, AnimatorControllerParameterType.Bool, AnimatorParameters);
            _animator.AddAnimatorParameterIfExists(ANIMATOR_IS_TURNING_PARAMETER_NAME, out ANIMATOR_IS_TURNING_PARAMETER_ID, AnimatorControllerParameterType.Bool, AnimatorParameters);
            _animator.AddAnimatorParameterIfExists(ANIMATOR_IS_CASTING_PARAMETER_NAME, out ANIMATOR_IS_CASTING_PARAMETER_ID, AnimatorControllerParameterType.Bool, AnimatorParameters);

            _animator.AddAnimatorParameterIfExists(ANIMATOR_SPAWN_PARAMETER_NAME, out ANIMATOR_SPAWN_PARAMETER_ID, AnimatorControllerParameterType.Trigger, AnimatorParameters);
            _animator.AddAnimatorParameterIfExists(ANIMATOR_SPECIAL_IDLE_PARAMETER_NAME, out ANIMATOR_SPECIAL_IDLE_PARAMETER_ID, AnimatorControllerParameterType.Trigger, AnimatorParameters);
            _animator.AddAnimatorParameterIfExists(ANIMATOR_INTERACT_PARAMETER_NAME, out ANIMATOR_INTERACT_PARAMETER_ID, AnimatorControllerParameterType.Trigger, AnimatorParameters);
            _animator.AddAnimatorParameterIfExists(ANIMATOR_BLINK_PARAMETER_NAME, out ANIMATOR_BLINK_PARAMETER_ID, AnimatorControllerParameterType.Trigger, AnimatorParameters);
            _animator.AddAnimatorParameterIfExists(ANIMATOR_TELEPORT_PARAMETER_NAME, out ANIMATOR_TELEPORT_PARAMETER_ID, AnimatorControllerParameterType.Trigger, AnimatorParameters);
            _animator.AddAnimatorParameterIfExists(ANIMATOR_TURN_PARAMETER_NAME, out ANIMATOR_TURN_PARAMETER_ID, AnimatorControllerParameterType.Trigger, AnimatorParameters);
            _animator.AddAnimatorParameterIfExists(ANIMATOR_DAMAGE_PARAMETER_NAME, out ANIMATOR_DAMAGE_PARAMETER_ID, AnimatorControllerParameterType.Trigger, AnimatorParameters);
            _animator.AddAnimatorParameterIfExists(ANIMATOR_DEATH_PARAMETER_NAME, out ANIMATOR_DEATH_PARAMETER_ID, AnimatorControllerParameterType.Trigger, AnimatorParameters);

            _animator.AddAnimatorParameterIfExists(ANIMATOR_ATTACK_SPEED_PARAMETER_NAME, out ANIMATOR_ATTACK_SPEED_PARAMETER_ID, AnimatorControllerParameterType.Float, AnimatorParameters);
            _animator.AddAnimatorParameterIfExists(ANIMATOR_MOVE_SPEED_PARAMETER_NAME, out ANIMATOR_MOVE_SPEED_PARAMETER_ID, AnimatorControllerParameterType.Float, AnimatorParameters);
            _animator.AddAnimatorParameterIfExists(ANIMATOR_BOSS_PHASE_PARAMETER_NAME, out ANIMATOR_BOSS_PHASE_PARAMETER_ID, AnimatorControllerParameterType.Float, AnimatorParameters);
            _animator.AddAnimatorParameterIfExists(ANIMATOR_DAMAGE_TYPE_PARAMETER_NAME, out ANIMATOR_DAMAGE_TYPE_PARAMETER_ID, AnimatorControllerParameterType.Float, AnimatorParameters);
            _animator.AddAnimatorParameterIfExists(ANIMATOR_WEAPON_TYPE_PARAMETER_NAME, out ANIMATOR_WEAPON_TYPE_PARAMETER_ID, AnimatorControllerParameterType.Float, AnimatorParameters);
        }
    }
}