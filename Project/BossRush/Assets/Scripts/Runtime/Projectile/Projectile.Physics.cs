using Lean.Pool;

using UnityEngine;

namespace TeamSuneat
{
    public partial class Projectile
    {
        private bool m_isIgnoreTrigger;

        public void StopMove()
        {
            _motionType = ProjectileMotionTypes.None;

            if (Body != null)
            {
                Body.ResetVelocity();

                Body.ResetGravity();

                Body.SetFreezeRotation(true);
            }
        }

        public void SetVelocity(Vector2 velocity)
        {
            if (Body != null)
            {
                Body.SetVelocity(velocity);
            }
        }

        public void ResetRigidBody()
        {
            if (Body != null)
            {
                Body.SetGravity(0f);

                Body.SetBodyType(RigidbodyType2D.Kinematic);

                Body.ResetVelocity();
            }
        }

        public void UpdateFreezeRotation()
        {
            if (RotationController != null)
            {
                RotationController.RefreshFreezeRotation();
            }
            else if (Body != null)
            {
                Body.SetFreezeRotation(false);
            }
        }

        public bool CompareCollider(Collider2D collider)
        {
            if (m_collider2D == null)
            {
                return false;
            }

            return m_collider2D == collider;
        }

        public void SetEnabledCollider(bool value)
        {
            if (m_collider2D != null)
            {
                m_collider2D.enabled = value;
            }
        }

        public void SetTriggerCollider(bool value)
        {
            if (m_collider2D != null)
            {
                m_collider2D.isTrigger = value;
            }
        }

        private bool CheckGround()
        {
            if (m_collider2D != null)
            {
                Vector3 checkPosition = position; //+ (Vector3.down * Owner.DistanceFromGround);

                float checkGroundDistance = _projectileData.GroundCheckDistance;

                if (m_boxCollider2D != null)
                {
                    checkGroundDistance += m_boxCollider2D.size.y * 0.5f;
                }
                else if (m_circleCollider2D != null)
                {
                    checkGroundDistance += m_circleCollider2D.radius;
                }

                RaycastHit2D hit = Physics2D.Raycast(checkPosition, Vector3.down, checkGroundDistance, GameLayers.Mask.Collision);

                DebugEx.DrawCross(checkPosition, Color.white, 3f);

                if (hit.collider != null)
                {
                    DebugEx.DrawCross(hit.point, Color.red, 3f);
                    DebugEx.DrawLine(checkPosition, hit.point, Color.red, 3f);
                    return true;
                }
                else
                {
                    DebugEx.DrawCross(checkPosition + new Vector3(0, -checkGroundDistance), Color.yellow, 3f);
                    DebugEx.DrawRay(checkPosition, Vector2.down * checkGroundDistance, Color.yellow, 3f);
                }
            }

            return false;
        }

        private void DoGroundedResult(bool isGrounded)
        {
            if (_projectileData.GroundCheck)
            {
                if (isGrounded)
                {
                    DoResult(_projectileData.LandingResult);
                }
                else
                {
                    DoResult(_projectileData.InAirResult);
                }

                if (Animator != null)
                {
                    Animator.UpdateGrounded(isGrounded);
                }
            }
        }

        private void DoArrivalResult()
        {
            if (false == _projectileData.ArrivalDistance.IsZero())
            {
                if (HomingTarget != null)
                {
                    float distance = Vector2.Distance(position, HomingTarget.position);
                    if (_projectileData.ArrivalDistance > distance)
                    {
                        DoResult(_projectileData.ArrivalResult);
                    }
                }
            }
        }
    }
}