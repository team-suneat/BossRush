using System.Collections.Generic;
using UnityEngine;

namespace TeamSuneat
{
    public partial class CharacterAnimator
    {
        protected HashSet<int> AnimatorParameters { get; set; } = new HashSet<int>();

        // bool parameter

        private const string ANIMATOR_IS_SPAWNED_PARAMETER_NAME = "IsSpawned";
        private const string ANIMATOR_IS_DAMAGING_PARAMETER_NAME = "IsDamaging";

        // trigger parameter

        private const string ANIMATOR_SPAWN_PARAMETER_NAME = "Spawn";
        protected const string ANIMATOR_DAMAGE_PARAMETER_NAME = "Damage";
        protected const string ANIMATOR_DEATH_PARAMETER_NAME = "Death";
        protected const string ANIMATOR_ATTACK_PARAMETER_NAME = "Attack";

        // float parameter

        private const string ANIMATOR_ATTACK_SPEED_PARAMETER_NAME = "AttackSpeed";

        private int ANIMATOR_IS_SPAWNED_PARAMETER_ID;
        private int ANIMATOR_IS_DAMAGING_PARAMETER_ID;

        protected int ANIMATOR_SPAWN_PARAMETER_ID;
        protected int ANIMATOR_DAMAGE_PARAMETER_ID;
        protected int ANIMATOR_DEATH_PARAMETER_ID;
        protected int ANIMATOR_ATTACK_PARAMETER_ID;

        private int ANIMATOR_ATTACK_SPEED_PARAMETER_ID;

        protected virtual void InitializeAnimatorParameters()
        {
            AnimatorParameters.Clear();

            AddAnimatorParameterIfExists(ANIMATOR_IS_SPAWNED_PARAMETER_NAME, out ANIMATOR_IS_SPAWNED_PARAMETER_ID, AnimatorControllerParameterType.Bool);
            AddAnimatorParameterIfExists(ANIMATOR_IS_DAMAGING_PARAMETER_NAME, out ANIMATOR_IS_DAMAGING_PARAMETER_ID, AnimatorControllerParameterType.Bool);

            AddAnimatorParameterIfExists(ANIMATOR_SPAWN_PARAMETER_NAME, out ANIMATOR_SPAWN_PARAMETER_ID, AnimatorControllerParameterType.Trigger);
            AddAnimatorParameterIfExists(ANIMATOR_DAMAGE_PARAMETER_NAME, out ANIMATOR_DAMAGE_PARAMETER_ID, AnimatorControllerParameterType.Trigger);
            AddAnimatorParameterIfExists(ANIMATOR_DEATH_PARAMETER_NAME, out ANIMATOR_DEATH_PARAMETER_ID, AnimatorControllerParameterType.Trigger);
            AddAnimatorParameterIfExists(ANIMATOR_ATTACK_PARAMETER_NAME, out ANIMATOR_ATTACK_PARAMETER_ID, AnimatorControllerParameterType.Trigger);

            AddAnimatorParameterIfExists(ANIMATOR_ATTACK_SPEED_PARAMETER_NAME, out ANIMATOR_ATTACK_SPEED_PARAMETER_ID, AnimatorControllerParameterType.Float);

            _animator.UpdateAnimatorBool(ANIMATOR_IS_SPAWNED_PARAMETER_ID, true, AnimatorParameters);
            _animator.UpdateAnimatorBool(ANIMATOR_IS_DAMAGING_PARAMETER_ID, false, AnimatorParameters);

            _animator.UpdateAnimatorFloat(ANIMATOR_ATTACK_SPEED_PARAMETER_ID, 1.0f, AnimatorParameters);

            Log.Info(LogTags.Animation, "캐릭터 애니메이터의 파라메터를 초기화를 완료합니다. Path: {0}, Parameters Count: {1}", 
                this.GetHierarchyPath(), AnimatorParameters.Count);
        }

        public void AddAnimatorParameterIfExists(string parameterName, out int parameterId, AnimatorControllerParameterType parameterType)
        {
            _animator.AddAnimatorParameterIfExists(parameterName, out parameterId, parameterType, AnimatorParameters);

            Log.Info(LogTags.Animation, "캐릭터 애니메이터의 파라메터를 추가합니다. {0}, ParameterId: {1}", parameterName, parameterId);
        }
    }
}