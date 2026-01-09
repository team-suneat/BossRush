using Sirenix.OdinInspector;
using System.Linq;
using UnityEngine;

namespace TeamSuneat
{
    public partial class CharacterAnimator : XBehaviour, IAnimatorStateMachine
    {
        //-------------------------------------------------------------------------------------------

        [Title("#Character Animator")]
        public bool IgnoreFlipOnAttacking;

        // 공격 중 피격 애니메이션을 재생할 수 없도록 금지합니다.
        protected bool IsBlockingDamageAnimationWhileAttack;

        protected CharacterAnimatorLog AnimatorLog;

        public bool IsAttacking { get; set; }

        public bool IsDashing { get; set; }

        public bool IsDamaging { get; set; }

        public bool IsParrying { get; set; }

        public bool IsConsumingPotion { get; set; }

        public bool IsBlinking { get; set; }

        public bool IsTurning { get; set; }

        public bool IsBlockDeathAnimation { get; set; }

        // Component

        protected Character _owner;
        protected Animator _animator;

        public delegate void OnStateEnterDelegate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex);
        public delegate void OnStateExitDelegate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex);

        private OnStateEnterDelegate OnStateEnter;
        private OnStateExitDelegate OnStateExit;

        private void Awake()
        {
            _animator ??= GetComponent<Animator>();
            _owner ??= this.FindFirstParentComponent<Character>();
            AnimatorLog = new CharacterAnimatorLog(_owner);
        }

        public virtual void Initialize()
        {
            IsAttacking = false;
            IsDashing = false;
            IsDamaging = false;
            IsParrying = false;
            IsConsumingPotion = false;
            IsBlinking = false;
            IsTurning = false;
            IsBlockDeathAnimation = false;

            _animator.UpdateAnimatorBoolIfExists("IsSuicide", false);

            InitializeAnimatorParameters();

            _animator.UpdateAnimatorBool(ANIMATOR_IS_SPAWNING_PARAMETER_ID, false, AnimatorParameters);
            _animator.UpdateAnimatorBool(ANIMATOR_IS_SPAWNED_PARAMETER_ID, false, AnimatorParameters);
            _animator.UpdateAnimatorBool(ANIMATOR_IS_ATTACKING_PARAMETER_ID, false, AnimatorParameters);
            _animator.UpdateAnimatorBool(ANIMATOR_IS_DAMAGING_PARAMETER_ID, false, AnimatorParameters);
            _animator.UpdateAnimatorBool(ANIMATOR_IS_GROUNDED_PARAMETER_ID, false, AnimatorParameters);
            _animator.UpdateAnimatorBool(ANIMATOR_IS_LEFT_COLLISION_PARAMETER_ID, false, AnimatorParameters);
            _animator.UpdateAnimatorBool(ANIMATOR_IS_RIGHT_COLLISION_PARAMETER_ID, false, AnimatorParameters);
            _animator.UpdateAnimatorBool(ANIMATOR_IS_SLIPPERY_PARAMETER_ID, false, AnimatorParameters);
            _animator.UpdateAnimatorBool(ANIMATOR_IS_PARRYING_PARAMETER_ID, false, AnimatorParameters);

            Log.Info(LogTags.Animation, "등록된 애니메이터 파라메터: {0}, {1}", _animator.parameters.JoinToString(), this.GetHierarchyPath());
        }

        public void SetAttackSpeed(float attackSpeed)
        {
            _animator.UpdateAnimatorFloat(ANIMATOR_ATTACK_SPEED_PARAMETER_ID, attackSpeed, AnimatorParameters);
        }

        public virtual void SetDamageTriggerIndex(int index)
        {
            _damageTriggerIndex = index;
        }

        public virtual void OnEquipItem(ItemNames itemName)
        {
        }

        #region Parameter

        public void SetPhaseParameter(float parameter)
        {
            _animator.UpdateAnimatorFloat(ANIMATOR_BOSS_PHASE_PARAMETER_ID, parameter, AnimatorParameters);
        }

        public void SetDamageTypeParameter(bool isPowerfulAttack)
        {
            if (_owner.AssetData.SuperArmor)
            {
                return;
            }

            if (isPowerfulAttack)
            {
                _animator.UpdateAnimatorFloat(ANIMATOR_DAMAGE_TYPE_PARAMETER_ID, 1, AnimatorParameters);
                _damageTypeIndex = 1;
            }
            else if (!(_owner.Physics?.IsGrounded ?? false))
            {
                _animator.UpdateAnimatorFloat(ANIMATOR_DAMAGE_TYPE_PARAMETER_ID, 1, AnimatorParameters);
                _damageTypeIndex = 1;
            }
            else
            {
                _animator.UpdateAnimatorFloat(ANIMATOR_DAMAGE_TYPE_PARAMETER_ID, 0, AnimatorParameters);
                _damageTypeIndex = 0;
            }
        }

        public void SetIsGrounded(bool isGrounded)
        {
            _animator.UpdateAnimatorBool(ANIMATOR_IS_GROUNDED_PARAMETER_ID, isGrounded, AnimatorParameters);
        }

        public void SetIsLeftCollision(bool isLeftCollision)
        {
            _animator.UpdateAnimatorBool(ANIMATOR_IS_LEFT_COLLISION_PARAMETER_ID, isLeftCollision, AnimatorParameters);
        }

        public void SetIsRightCollision(bool isRightCollision)
        {
            _animator.UpdateAnimatorBool(ANIMATOR_IS_RIGHT_COLLISION_PARAMETER_ID, isRightCollision, AnimatorParameters);
        }

        public void SetDirectionalX(float directionalX)
        {
            _animator.UpdateAnimatorFloat(ANIMATOR_DIRECTIONAL_X_PARAMETER_ID, directionalX, AnimatorParameters);
        }

        public void SetDirectionalY(float directionalY)
        {
            _animator.UpdateAnimatorFloat(ANIMATOR_DIRECTIONAL_Y_PARAMETER_ID, directionalY, AnimatorParameters);
        }

        public void SetSpeedX(float speedX)
        {
            _animator.UpdateAnimatorFloat(ANIMATOR_SPEED_X_PARAMETER_ID, speedX, AnimatorParameters);
        }

        public void SetSpeedY(float speedY)
        {
            _animator.UpdateAnimatorFloat(ANIMATOR_SPEED_Y_PARAMETER_ID, speedY, AnimatorParameters);
        }

        #endregion Parameter

        #region Play

        public void PlaySpawnAnimation()
        {
            _animator.UpdateAnimatorTrigger(ANIMATOR_SPAWN_PARAMETER_ID, AnimatorParameters);
        }

        public void PlayDashAnimation()
        {
            _animator.UpdateAnimatorTrigger(ANIMATOR_DASH_PARAMETER_ID, AnimatorParameters);
        }

        public virtual bool PlayDamageAnimation()
        {
            return _animator.UpdateAnimatorTrigger(ANIMATOR_DAMAGE_PARAMETER_ID, AnimatorParameters);
        }

        public virtual void PlayDeathAnimation()
        {
            if (!IsBlockDeathAnimation)
            {
                _animator.UpdateAnimatorTrigger(ANIMATOR_DEATH_PARAMETER_ID, AnimatorParameters);
            }
        }

        public void PlayInteractAnimation()
        {
            AnimatorLog.LogInfo("{0} 애니메이션 파라미터를 설정합니다. {1}", ANIMATOR_INTERACT_PARAMETER_NAME, true.ToBoolString());

            _animator.UpdateAnimatorBool(ANIMATOR_INTERACT_PARAMETER_ID, true, AnimatorParameters);
        }

        #endregion Play

        #region Stop

        public void StopInteractAnimation()
        {
            AnimatorLog.LogInfo("{0} 애니메이션 파라미터를 초기화합니다. {1}", ANIMATOR_INTERACT_PARAMETER_ID, false.ToBoolString());

            _animator.UpdateAnimatorBool(ANIMATOR_INTERACT_PARAMETER_ID, false, AnimatorParameters);
        }

        #endregion Stop

        public virtual void OnAnimatorStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (CheckStateName(stateInfo, "Spawn"))
            {
                OnAnimatorSpawnStateEnter();
            }
            else if (CheckStateNames(stateInfo, "Dash"))
            {
                OnAnimatorDashStateEnter();
            }
            else if (CheckStateNames(stateInfo, "Damage", "DamageGround"))
            {
                OnAnimatorDamageStateEnter();
            }
            else if (CheckStateName(stateInfo, "Stun"))
            {
                AnimatorLog.LogInfo("기절 상태의 애니메이션에 진입했습니다.");
            }
            else if (CheckStateNames(stateInfo, "Phase2", "Phase3", "Phase4"))
            {
                LockMovement();
            }
            else if (IsAttackState(stateInfo, true))
            {
                OnAnimatorAttackStateEnter();
            }
            else if (CheckStateName(stateInfo, "Parry"))
            {
                OnAnimatorParryStateEnter();
            }

            OnStateEnter?.Invoke(animator, stateInfo, layerIndex);
        }

        public virtual void OnAnimatorStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (CheckStateName(stateInfo, "Spawn"))
            {
                OnAnimatorSpawnStateExit();
            }
            else if (CheckStateNames(stateInfo, "Dash"))
            {
                OnAnimatorDashStateExit();
            }
            else if (CheckStateNames(stateInfo, "Damage", "DamageGround"))
            {
                OnAnimatorDamageStateExit();
            }
            else if (CheckStateName(stateInfo, "DashEnd"))
            {
                OnAnimatorDashStateExit();
            }
            else if (CheckStateNames(stateInfo, "Phase2", "Phase3", "Phase4"))
            {
                UnlockMovement();
            }
            else if (IsAttackState(stateInfo, false))
            {
                OnAnimatorAttackStateExit();
            }
            else if (CheckStateName(stateInfo, "Parry"))
            {
                OnAnimatorParryStateExit();
            }

            OnStateExit?.Invoke(animator, stateInfo, layerIndex);
        }

        protected bool CheckStateName(AnimatorStateInfo stateInfo, string animationName)
        {
            return stateInfo.IsName(animationName);
        }

        protected bool CheckStateNames(AnimatorStateInfo stateInfo, params string[] animationNames)
        {
            if (animationNames != null)
            {
                for (int i = 0; i < animationNames.Length; i++)
                {
                    string animationName = animationNames[i];
                    if (stateInfo.IsName(animationName))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        #region On Animator State Delegate

        public void RegisterOnStateEnterDelegate(OnStateEnterDelegate action)
        {
            if (OnStateEnter != null)
            {
                System.Delegate[] delegateArray = OnStateEnter.GetInvocationList();
                if (delegateArray.Contains(action))
                {
                    Log.Warning(LogTags.Animation, "중복된 애니메이션 상태 입장 이벤트를 등록할 수 없습니다. {0}", action.Method);
                    return;
                }
            }

            OnStateEnter += action;
        }

        public void RegisterOnStateExitDelegate(OnStateExitDelegate action)
        {
            if (OnStateExit != null)
            {
                System.Delegate[] delegateArray = OnStateExit.GetInvocationList();
                if (delegateArray.Contains(action))
                {
                    Log.Warning(LogTags.Animation, "중복된 애니메이션 상태 퇴장 이벤트를 등록할 수 없습니다. {0}", action.Method);
                    return;
                }
            }

            OnStateExit += action;
        }

        public void UnregisterOnStateEnterDelegate(OnStateEnterDelegate action)
        {
            if (OnStateEnter != null)
            {
                System.Delegate[] delegateArray = OnStateEnter.GetInvocationList();
                if (delegateArray.Contains(action))
                {
                    OnStateEnter -= action;
                    return;
                }
            }

            Log.Warning(LogTags.Animation, "등록되지않은 애니메이션 상태 입장 이벤트를 등록 해제할 수 없습니다. {0}", action.Method);
        }

        public void UnregisterOnStateExitDelegate(OnStateExitDelegate action)
        {
            if (OnStateExit != null)
            {
                System.Delegate[] delegateArray = OnStateExit.GetInvocationList();
                if (delegateArray.Contains(action))
                {
                    OnStateExit -= action;
                    return;
                }
            }

            Log.Warning(LogTags.Animation, "등록되지않은 애니메이션 상태 퇴장 이벤트를 등록 해제할 수 없습니다. {0}", action.Method);
        }

        #endregion On Animator State Delegate

        #region On Animator State Changed

        protected virtual void OnAnimatorSpawnStateEnter()
        {
            if (_animator != null)
            {
                _animator.UpdateAnimatorBool(ANIMATOR_IS_SPAWNING_PARAMETER_ID, true, AnimatorParameters);
            }
        }

        protected virtual void OnAnimatorSpawnStateExit()
        {
            if (_animator != null)
            {
                _animator.UpdateAnimatorBool(ANIMATOR_IS_SPAWNING_PARAMETER_ID, false, AnimatorParameters);
                _animator.UpdateAnimatorBool(ANIMATOR_IS_SPAWNED_PARAMETER_ID, true, AnimatorParameters);
            }
        }

        protected virtual void OnAnimatorDamageStateEnter()
        {
            AnimatorLog.LogInfo("피격 상태의 애니메이션에 진입했습니다.");
            LockMovement();

            _animator.UpdateAnimatorBool(ANIMATOR_IS_DAMAGING_PARAMETER_ID, true, AnimatorParameters);
            IsDamaging = true;
        }

        protected virtual void OnAnimatorDamageStateExit()
        {
            UnlockMovement();

            _animator.UpdateAnimatorBool(ANIMATOR_IS_DAMAGING_PARAMETER_ID, false, AnimatorParameters);
            IsDamaging = false;
        }

        protected void OnAnimatorDashStateEnter()
        {
            AnimatorLog.LogInfo("대시 상태의 애니메이션에 진입했습니다.");

            IsDashing = true;

            LockMovement();
        }

        protected void OnAnimatorDashStateExit()
        {
            IsDashing = false;

            UnlockMovement();
        }

        #endregion On Animator State Changed

        private void OnDrawGizmos()
        {
            if (IsDamaging)
            {
                GizmoEx.DrawText("Damaging", position);
            }
        }
    }
}