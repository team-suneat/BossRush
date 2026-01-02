using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace TeamSuneat
{
    [RequireComponent(typeof(Animator))]
    public class ProjectileAnimator : XBehaviour, IAnimatorStateMachine
    {
        [Title("#Projectile Animator")]
        [SuffixLabel("Rigidbody Dynimic 속도 비례 애니메이터 속도 사용")]
        public bool UseAnimatorSpeedByDynimic;

        private Projectile projectile;

        private Animator animator;

        private float m_maxMagnitude;

        private bool m_isAttacked;

        private bool m_isHited;

        private UnityAction m_hitAnimationExitCallback;

        private UnityAction m_attackAnimationExitCallback;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            projectile = this.FindFirstParentComponent<Projectile>();

            animator = GetComponent<Animator>();
        }

        protected virtual void Awake()
        {
            if (projectile == null)
            {
                projectile = this.FindFirstParentComponent<Projectile>();
            }

            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }
        }

        public void RegisterHitAnimationExitCallback(UnityAction callback)
        {
            m_hitAnimationExitCallback += callback;
        }

        public void RegisterAttackAnimationExitCallback(UnityAction callback)
        {
            m_attackAnimationExitCallback += callback;
        }

        public void UnregisterAllAnimationCallback()
        {
            m_hitAnimationExitCallback = null;

            m_attackAnimationExitCallback = null;
        }

        public void SetMaxMagnitude(float magnitude)
        {
            if (UseAnimatorSpeedByDynimic)
            {
                m_maxMagnitude = magnitude;
            }
        }

        public void SetAnimatorSpeed(Vector2 velocity)
        {
            if (false == UseAnimatorSpeedByDynimic)
            {
                return;
            }

            if (m_isHited)
            {
                return;
            }

            if (m_maxMagnitude.IsZero())
            {
                return;
            }

            if (velocity.magnitude.IsZero())
            {
                animator.speed = 0;
            }
            else
            {
                float animatorSpeed = velocity.magnitude / m_maxMagnitude;

                animator.speed = Mathf.Clamp01(animatorSpeed);
            }
        }

        protected void CallAttackAnimationEvent()
        {
            if (projectile != null)
            {
                projectile.DoAttack();

                projectile.SpawnAttackFX(projectile.FacingRightAtLaunch);
            }
        }

        protected void CallAnotherAnimationEvent()
        {
            if (projectile != null)
            {
                projectile.DoAnotherAttack();
            }
        }

        protected void CallStopAttackAnimationEvent()
        {
            if (projectile != null)
            {
                projectile.StopAttack();
            }
        }

        public virtual void OnAnimatorStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
        }

        public virtual void OnAnimatorStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (stateInfo.IsName("Attack"))
            {
                OnAnimatorAttackStateExit();
            }
            else if (stateInfo.IsName("Hit"))
            {
                OnAnimatorHitStateExit();
            }
        }

        private void OnAnimatorAttackStateExit()
        {
            if (m_attackAnimationExitCallback != null)
            {
                m_attackAnimationExitCallback.Invoke();
            }
        }

        private void OnAnimatorHitStateExit()
        {
            if (m_hitAnimationExitCallback != null)
            {
                m_hitAnimationExitCallback.Invoke();
            }
        }

        public void OnSpawn()
        {
            PlaySpawnAnimation();

            m_isHited = false;

            m_isAttacked = false;
        }

        public void UpdateGrounded(bool isGrounded)
        {
            if (animator != null)
            {
                animator.UpdateAnimatorBoolIfExists("IsGrounded", isGrounded);
            }
        }

        public bool TryPlayAnimation()
        {
            if (m_isHited)
            {
                Log.Warning(LogTags.Projectile, "발사체의 애니메이션을 재생할 수 없습니다. 이미 'Hit' 애니메이션을 재생중입니다. {0}", projectile.Name.ToLogString());

                return false;
            }

            if (m_isAttacked)
            {
                Log.Warning(LogTags.Projectile, "발사체의 애니메이션을 재생할 수 없습니다. 이미 'Attack' 애니메이션을 재생중입니다. {0}", projectile.Name.ToLogString());

                return false;
            }

            return true;
        }

        public bool PlaySpawnAnimation()
        {
            if (animator != null)
            {
                if (animator.UpdateAnimatorTriggerIfExists("Spawn"))
                {
                    Log.Info(LogTags.Projectile, "발사체의 Spawn 애니메이션을 재생합니다. {0}", projectile.Name.ToLogString());

                    return true;
                }
            }

            return false;
        }

        public bool PlayAttackAnimation()
        {
            if (animator.UpdateAnimatorTriggerIfExists("Attack"))
            {
                Log.Info(LogTags.Projectile, "발사체의 Attack 애니메이션을 재생합니다. {0}", projectile.Name.ToLogString());

                m_isAttacked = true;

                return true;
            }

            Log.Warning(LogTags.Projectile, "발사체의 애니메이션을 재생할 수 없습니다. 'Attack' 트리거가 설정되어있지 않습니다. {0}", projectile.Name.ToLogString());

            return false;
        }

        public bool PlayHitAnimation()
        {
            if (animator != null)
            {
                animator.speed = 1;

                if (animator.UpdateAnimatorTriggerIfExists("Hit"))
                {
                    Log.Info(LogTags.Projectile, "발사체의 Hit 애니메이션을 재생합니다. {0}", projectile.Name.ToLogString());

                    m_isHited = true;

                    return true;
                }
            }

            return false;
        }

        public void UpdateCollisionParameter(bool value)
        {
            if (animator != null)
            {
                animator.UpdateAnimatorBoolIfExists("IsCollide", value);
            }
        }
    }
}