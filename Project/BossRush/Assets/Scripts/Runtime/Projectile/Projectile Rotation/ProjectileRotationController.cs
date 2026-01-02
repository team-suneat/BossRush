using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

namespace TeamSuneat
{
    public class ProjectileRotationController : XBehaviour
    {
        [Title("#RotationController")]
        public Rigidbody2D Rigidbody;

        public ProjectileRenderer Renderer;

        [SuffixLabel("자동 회전")]
        public bool UseAutoRotate;

        [SuffixLabel("자동 회전 속도")]
        public float AutoRotateSpeed;

        [SuffixLabel("타겟을 향해 회전")]
        public bool UseRotateToTarget;

        [SuffixLabel("방향을 향해 회전")]
        public bool UseRotateToDirection;

        [SuffixLabel("회전 금지")]
        public bool UseFreezeRotation;

        [SuffixLabel("랜더러 회전 금지")]
        public bool UseRendererFreezeRotation;

        [SuffixLabel("회전 각도 오프셋")]
        public float RotateOffsetAngle;

        private float _rotateResultAngle;

        private Coroutine _autoRorateCoroutine;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            Rigidbody = GetComponent<Rigidbody2D>();

            Renderer = GetComponentInChildren<ProjectileRenderer>();
        }

        protected virtual void Awake()
        {
            

            if (Rigidbody == null)
            {
                Rigidbody = GetComponent<Rigidbody2D>();
            }

            if (Renderer == null)
            {
                Renderer = GetComponentInChildren<ProjectileRenderer>();
            }
        }

        public void ResetRotation()
        {
            if (transform != null)
            {
                transform.rotation = Quaternion.identity;
            }

            if (Renderer != null)
            {
                if (UseRotateToTarget || UseRotateToDirection)
                {
                    Renderer.transform.rotation = Quaternion.identity;
                }
            }
        }

        public void RefreshRendererRotation(Vector2 targetDirection)
        {
            if (Renderer == null)
            {
                return;
            }

            if (UseRendererFreezeRotation)
            {
                return;
            }

            if (UseRotateToDirection)
            {
                if (false == targetDirection.IsZero())
                {
                    _rotateResultAngle = AngleEx.ToAngle(position, position + (Vector3)targetDirection) + RotateOffsetAngle;

                    Renderer.transform.rotation = Quaternion.Slerp(Renderer.transform.rotation, Quaternion.Euler(0, 0, _rotateResultAngle), 1);
                }
                else
                {
                    Renderer.transform.rotation = Quaternion.identity;
                }
            }
            else
            {
                Renderer.ResetQuaternion(transform.rotation);
            }
        }

        public void RefreshRendererRotation(Vital homingTarget, Vector2 targetDirection)
        {
            if (Renderer == null)
            {
                Log.Warning(LogTags.Projectile, "Projectile Renderer를 회전할 수 없습니다. Projectile Renderer를 찾을 수 없습니다. {0}", this.GetHierarchyPath());
                return;
            }

            if (UseRotateToTarget)
            {
                if (homingTarget != null)
                {
                    // 랜더러만 변경
                    _rotateResultAngle = AngleEx.ToAngle(position, homingTarget.position) + RotateOffsetAngle;

                    Renderer.transform.rotation = Quaternion.Slerp(Renderer.transform.rotation, Quaternion.Euler(0, 0, _rotateResultAngle), 1);
                }
                else
                {
                    Renderer.transform.rotation = Quaternion.identity;
                }
            }
            else if (UseRotateToDirection)
            {
                if (false == targetDirection.IsZero())
                {
                    _rotateResultAngle = AngleEx.ToAngle(position, position + (Vector3)targetDirection) + RotateOffsetAngle;

                    Renderer.transform.rotation = Quaternion.Slerp(Renderer.transform.rotation, Quaternion.Euler(0, 0, _rotateResultAngle), 1);
                }
                else
                {
                    Renderer.transform.rotation = Quaternion.identity;
                }
            }
            else
            {
                Renderer.ResetQuaternion(transform.rotation);
            }
        }

        public void RefreshRotationParent(Vital homingTarget, Vector2 targetDirection)
        {
            if (UseRotateToTarget)
            {
                if (homingTarget != null)
                {
                    // 랜더러만 변경
                    _rotateResultAngle = AngleEx.ToAngle(position, homingTarget.position) + RotateOffsetAngle;
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, _rotateResultAngle), 1);
                }
                else
                {
                    transform.rotation = Quaternion.identity;
                }
            }
            else if (UseRotateToDirection)
            {
                if (false == targetDirection.IsZero())
                {
                    _rotateResultAngle = AngleEx.ToAngle(position, position + (Vector3)targetDirection) + RotateOffsetAngle;
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, _rotateResultAngle), 1);

                    Log.Info(LogTags.Projectile, "날아가는 방향으로 발사체를 회전시킨다. Angle: {0}, Path: {1}", _rotateResultAngle, this.GetHierarchyPath());
                }
                else
                {
                    transform.rotation = Quaternion.identity;
                }
            }
        }

        public void FreezeRotation()
        {
            if (Rigidbody != null)
            {
                Rigidbody.freezeRotation = true;
            }
        }

        public void RefreshFreezeRotation()
        {
            if (Rigidbody != null)
            {
                Rigidbody.freezeRotation = UseFreezeRotation;
            }
        }

        public void StartAutoRotate()
        {
            if (UseAutoRotate)
            {
                if (_autoRorateCoroutine == null)
                {
                    _autoRorateCoroutine = StartXCoroutine(ProcessAutoRotate());
                }
            }
        }

        public void StopAutoRotate()
        {
            StopXCoroutine(ref _autoRorateCoroutine);
        }

        private IEnumerator ProcessAutoRotate()
        {
            WaitForFixedUpdate wait = new WaitForFixedUpdate();

            _rotateResultAngle = 0;

            while (ActiveInHierarchy)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, _rotateResultAngle), 1);
                _rotateResultAngle += AutoRotateSpeed;

                yield return wait;
            }
        }
    }
}