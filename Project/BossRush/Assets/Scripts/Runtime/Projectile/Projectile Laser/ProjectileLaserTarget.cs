using System.Collections;
using UnityEngine;

namespace TeamSuneat
{
    public class ProjectileLaserTarget : XBehaviour
    {
        public Vector3 OriginPosition;

        public Vector3 TargetPosition;

        public float Duration;

        public float DelayTime;

        public float MoveSpeed;

        public float Acceleration;

        private Vector3 direction;

        private Coroutine _moveCoroutine;

#if UNITY_EDITOR

        private void OnDrawGizmosSelected()
        {
            DebugEx.DrawGizmoCircle(OriginPosition, 0.1f);

            DebugEx.DrawGizmoLine(OriginPosition, TargetPosition);

            DebugEx.DrawGizmoCircle(TargetPosition, 0.1f, Color.red);
        }

#endif

        public override void AutoNaming()
        {
            SetGameObjectName("Projectile Laser Target");
        }

        public void StartMove()
        {
            SetPositionAsOriginPosition();

            SetDirection();

            _moveCoroutine = StartXCoroutine(ProcessMove());
        }

        public void StopMove()
        {
            StopXCoroutine(ref _moveCoroutine);

            SetPositionAsOriginPosition();
        }

        private void SetPositionAsOriginPosition()
        {
            position = OriginPosition;
        }

        private void SetDirection()
        {
            direction = TargetPosition - OriginPosition;

            direction.Normalize();
        }

        private IEnumerator ProcessMove()
        {
            if (false == DelayTime.IsZero())
            {
                yield return new WaitForSeconds(DelayTime);
            }

            WaitForFixedUpdate wait = new WaitForFixedUpdate();

            float elapsedTime = 0f;

            float distance = Vector2.Distance(TargetPosition, position);

            float moveSpeedForDelta = MoveSpeed * Time.fixedDeltaTime;

            while (distance > moveSpeedForDelta)
            {
                position += direction * moveSpeedForDelta;

                yield return wait;

                distance = Vector2.Distance(TargetPosition, position);

                elapsedTime += Time.fixedDeltaTime;

                if (Duration < elapsedTime)
                {
                    break;
                }
            }

            _moveCoroutine = null;
        }
    }
}