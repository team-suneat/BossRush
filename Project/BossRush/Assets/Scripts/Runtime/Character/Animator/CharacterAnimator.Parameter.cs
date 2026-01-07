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
        private const string ANIMATOR_IS_DAMAGING_PARAMETER_NAME = "IsDamaging";
        private const string ANIMATOR_IS_GROUNDED_PARAMETER_NAME = "IsGrounded";
        private const string ANIMATOR_IS_LEFT_COLLISION_PARAMETER_NAME = "IsLeftCollision";
        private const string ANIMATOR_IS_RIGHT_COLLISION_PARAMETER_NAME = "IsRightCollision";
        private const string ANIMATOR_IS_SLIPPERY_PARAMETER_NAME = "IsSlippery";
        private const string ANIMATOR_IS_PARRYING_PARAMETER_NAME = "IsParrying";
        private const string ANIMATOR_USE_WALL_SLIDING_PARAMETER_NAME = "UseWallSliding";

        // trigger parameter

        private const string ANIMATOR_SPAWN_PARAMETER_NAME = "Spawn";
        private const string ANIMATOR_DASH_PARAMETER_NAME = "Dash";
        private const string ANIMATOR_INTERACT_PARAMETER_NAME = "Interact";
        private const string ANIMATOR_PARRY_PARAMETER_NAME = "Parry";
        protected const string ANIMATOR_DAMAGE_PARAMETER_NAME = "Damage";
        protected const string ANIMATOR_DEATH_PARAMETER_NAME = "Death";
        private const string ANIMATOR_DISABLE_PARAMETER_NAME = "Disable";

        // float parameter

        private const string ANIMATOR_ATTACK_SPEED_PARAMETER_NAME = "AttackSpeed";

        private const string ANIMATOR_BOSS_PHASE_PARAMETER_NAME = "Phase";
        private const string ANIMATOR_DAMAGE_TYPE_PARAMETER_NAME = "DamageType";
        private const string ANIMATOR_DIRECTIONAL_X_PARAMETER_NAME = "DirectionalX";
        private const string ANIMATOR_DIRECTIONAL_Y_PARAMETER_NAME = "DirectionalY";
        private const string ANIMATOR_SPEED_X_PARAMETER_NAME = "SpeedX";
        private const string ANIMATOR_SPEED_Y_PARAMETER_NAME = "SpeedY";
        private const string ANIMATOR_FORCE_SPEED_X_PARAMETER_NAME = "ForceSpeedX";
        private const string ANIMATOR_FORCE_SPEED_Y_PARAMETER_NAME = "ForceSpeedY";
        private const string ANIMATOR_PARRYING_TYPE_PARAMETER_NAME = "ParryingType";

        private int ANIMATOR_IS_SPAWNING_PARAMETER_ID;
        private int ANIMATOR_IS_SPAWNED_PARAMETER_ID;
        private int ANIMATOR_IS_ATTACKING_PARAMETER_ID;
        private int ANIMATOR_IS_DAMAGING_PARAMETER_ID;
        private int ANIMATOR_IS_GROUNDED_PARAMETER_ID;
        private int ANIMATOR_IS_LEFT_COLLISION_PARAMETER_ID;
        private int ANIMATOR_IS_RIGHT_COLLISION_PARAMETER_ID;
        private int ANIMATOR_IS_SLIPPERY_PARAMETER_ID;
        private int ANIMATOR_IS_PARRYING_PARAMETER_ID;
        private int ANIMATOR_USE_WALL_SLIDING_PARAMETER_ID;

        private int ANIMATOR_SPAWN_PARAMETER_ID;
        private int ANIMATOR_DASH_PARAMETER_ID;
        private int ANIMATOR_INTERACT_PARAMETER_ID;
        protected int ANIMATOR_PARRY_PARAMETER_ID;
        protected int ANIMATOR_DAMAGE_PARAMETER_ID;
        protected int ANIMATOR_DEATH_PARAMETER_ID;
        private int ANIMATOR_DISABLE_PARAMETER_ID;

        private int ANIMATOR_ATTACK_SPEED_PARAMETER_ID;

        private int ANIMATOR_BOSS_PHASE_PARAMETER_ID;
        private int ANIMATOR_DAMAGE_TYPE_PARAMETER_ID;

        private int ANIMATOR_DIRECTIONAL_X_PARAMETER_ID;
        private int ANIMATOR_DIRECTIONAL_Y_PARAMETER_ID;
        private int ANIMATOR_SPEED_X_PARAMETER_ID;
        private int ANIMATOR_SPEED_Y_PARAMETER_ID;
        private int ANIMATOR_FORCE_SPEED_X_PARAMETER_ID;
        private int ANIMATOR_FORCE_SPEED_Y_PARAMETER_ID;
        private int ANIMATOR_PARRYING_TYPE_PARAMETER_ID;

        protected virtual void InitializeAnimatorParameters()
        {
            AnimatorParameters.Clear();

            _animator.AddAnimatorParameterIfExists(ANIMATOR_IS_SPAWNING_PARAMETER_NAME, out ANIMATOR_IS_SPAWNING_PARAMETER_ID, AnimatorControllerParameterType.Bool, AnimatorParameters);
            _animator.AddAnimatorParameterIfExists(ANIMATOR_IS_SPAWNED_PARAMETER_NAME, out ANIMATOR_IS_SPAWNED_PARAMETER_ID, AnimatorControllerParameterType.Bool, AnimatorParameters);
            _animator.AddAnimatorParameterIfExists(ANIMATOR_IS_ATTACKING_PARAMETER_NAME, out ANIMATOR_IS_ATTACKING_PARAMETER_ID, AnimatorControllerParameterType.Bool, AnimatorParameters);
            _animator.AddAnimatorParameterIfExists(ANIMATOR_IS_DAMAGING_PARAMETER_NAME, out ANIMATOR_IS_DAMAGING_PARAMETER_ID, AnimatorControllerParameterType.Bool, AnimatorParameters);
            _animator.AddAnimatorParameterIfExists(ANIMATOR_IS_GROUNDED_PARAMETER_NAME, out ANIMATOR_IS_GROUNDED_PARAMETER_ID, AnimatorControllerParameterType.Bool, AnimatorParameters);
            _animator.AddAnimatorParameterIfExists(ANIMATOR_IS_LEFT_COLLISION_PARAMETER_NAME, out ANIMATOR_IS_LEFT_COLLISION_PARAMETER_ID, AnimatorControllerParameterType.Bool, AnimatorParameters);
            _animator.AddAnimatorParameterIfExists(ANIMATOR_IS_RIGHT_COLLISION_PARAMETER_NAME, out ANIMATOR_IS_RIGHT_COLLISION_PARAMETER_ID, AnimatorControllerParameterType.Bool, AnimatorParameters);
            _animator.AddAnimatorParameterIfExists(ANIMATOR_IS_SLIPPERY_PARAMETER_NAME, out ANIMATOR_IS_SLIPPERY_PARAMETER_ID, AnimatorControllerParameterType.Bool, AnimatorParameters);
            _animator.AddAnimatorParameterIfExists(ANIMATOR_IS_PARRYING_PARAMETER_NAME, out ANIMATOR_IS_PARRYING_PARAMETER_ID, AnimatorControllerParameterType.Bool, AnimatorParameters);
            _animator.AddAnimatorParameterIfExists(ANIMATOR_USE_WALL_SLIDING_PARAMETER_NAME, out ANIMATOR_USE_WALL_SLIDING_PARAMETER_ID, AnimatorControllerParameterType.Bool, AnimatorParameters);
            _animator.AddAnimatorParameterIfExists(ANIMATOR_DISABLE_PARAMETER_NAME, out ANIMATOR_DISABLE_PARAMETER_ID, AnimatorControllerParameterType.Bool, AnimatorParameters);

            _animator.AddAnimatorParameterIfExists(ANIMATOR_SPAWN_PARAMETER_NAME, out ANIMATOR_SPAWN_PARAMETER_ID, AnimatorControllerParameterType.Trigger, AnimatorParameters);
            _animator.AddAnimatorParameterIfExists(ANIMATOR_DASH_PARAMETER_NAME, out ANIMATOR_DASH_PARAMETER_ID, AnimatorControllerParameterType.Trigger, AnimatorParameters);
            _animator.AddAnimatorParameterIfExists(ANIMATOR_INTERACT_PARAMETER_NAME, out ANIMATOR_INTERACT_PARAMETER_ID, AnimatorControllerParameterType.Trigger, AnimatorParameters);
            _animator.AddAnimatorParameterIfExists(ANIMATOR_PARRY_PARAMETER_NAME, out ANIMATOR_PARRY_PARAMETER_ID, AnimatorControllerParameterType.Trigger, AnimatorParameters);
            _animator.AddAnimatorParameterIfExists(ANIMATOR_DAMAGE_PARAMETER_NAME, out ANIMATOR_DAMAGE_PARAMETER_ID, AnimatorControllerParameterType.Trigger, AnimatorParameters);
            _animator.AddAnimatorParameterIfExists(ANIMATOR_DEATH_PARAMETER_NAME, out ANIMATOR_DEATH_PARAMETER_ID, AnimatorControllerParameterType.Trigger, AnimatorParameters);

            _animator.AddAnimatorParameterIfExists(ANIMATOR_ATTACK_SPEED_PARAMETER_NAME, out ANIMATOR_ATTACK_SPEED_PARAMETER_ID, AnimatorControllerParameterType.Float, AnimatorParameters);

            _animator.AddAnimatorParameterIfExists(ANIMATOR_BOSS_PHASE_PARAMETER_NAME, out ANIMATOR_BOSS_PHASE_PARAMETER_ID, AnimatorControllerParameterType.Float, AnimatorParameters);
            _animator.AddAnimatorParameterIfExists(ANIMATOR_DAMAGE_TYPE_PARAMETER_NAME, out ANIMATOR_DAMAGE_TYPE_PARAMETER_ID, AnimatorControllerParameterType.Float, AnimatorParameters);
            _animator.AddAnimatorParameterIfExists(ANIMATOR_DIRECTIONAL_X_PARAMETER_NAME, out ANIMATOR_DIRECTIONAL_X_PARAMETER_ID, AnimatorControllerParameterType.Float, AnimatorParameters);
            _animator.AddAnimatorParameterIfExists(ANIMATOR_DIRECTIONAL_Y_PARAMETER_NAME, out ANIMATOR_DIRECTIONAL_Y_PARAMETER_ID, AnimatorControllerParameterType.Float, AnimatorParameters);
            _animator.AddAnimatorParameterIfExists(ANIMATOR_SPEED_X_PARAMETER_NAME, out ANIMATOR_SPEED_X_PARAMETER_ID, AnimatorControllerParameterType.Float, AnimatorParameters);
            _animator.AddAnimatorParameterIfExists(ANIMATOR_SPEED_Y_PARAMETER_NAME, out ANIMATOR_SPEED_Y_PARAMETER_ID, AnimatorControllerParameterType.Float, AnimatorParameters);
            _animator.AddAnimatorParameterIfExists(ANIMATOR_FORCE_SPEED_X_PARAMETER_NAME, out ANIMATOR_FORCE_SPEED_X_PARAMETER_ID, AnimatorControllerParameterType.Float, AnimatorParameters);
            _animator.AddAnimatorParameterIfExists(ANIMATOR_FORCE_SPEED_Y_PARAMETER_NAME, out ANIMATOR_FORCE_SPEED_Y_PARAMETER_ID, AnimatorControllerParameterType.Float, AnimatorParameters);
            _animator.AddAnimatorParameterIfExists(ANIMATOR_PARRYING_TYPE_PARAMETER_NAME, out ANIMATOR_PARRYING_TYPE_PARAMETER_ID, AnimatorControllerParameterType.Float, AnimatorParameters);
        }
    }
}