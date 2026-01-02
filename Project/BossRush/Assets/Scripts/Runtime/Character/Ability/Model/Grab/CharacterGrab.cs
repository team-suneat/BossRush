using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat
{
    public class CharacterGrab : CharacterAbility
    {
        [Title("#캐릭터 잡기 능력 (Grab Ability)")]
        [LabelText("잡기 중 캐릭터 충돌체 변경")]
        public bool UseResizeColliderWhileGrabbing;

        [EnableIf("UseResizeColliderWhileGrabbing")]
        [LabelText("잡기 중 캐릭터 충돌체 크기")]
        public Vector2 GrabbingColliderSize;

        [LabelText("잡기 중 캐릭터 충돌체 오프셋")]
        [EnableIf("UseResizeColliderWhileGrabbing")]
        public Vector2 GrabbingColliderOffset;

        [Title("#캐릭터 잡기 능력 (Grab Ability)", "Point")]
        [ChildGameObjectsOnly]
        public Transform GrabAttachmentPoint;

        [ChildGameObjectsOnly]
        public Transform GrabEndPoint;

        private Character _grabbedCharacter;
        private CharacterGrabbed _grabbedAbility;
        private Vector2 _defaultColliderSize;
        private Vector2 _defaultColliderOffset;
        private bool _isGrabbing;

        private const string ANIMATOR_GRAB_SUCCESS_PARAMETER_NAME = "IsGrabSuccess";

        private int _grabSuccessAnimationParameter;

        public override Types Type => Types.Grab;

        public bool IsGrabbing => _isGrabbing;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            AbilityStartFeedbacks = this.FindComponent<GameFeedbacks>("Feedbacks/Grab");
            GrabAttachmentPoint = this.FindTransform("Model/Point-GrabAttachment");
            GrabEndPoint = this.FindTransform("Model/Point-GrabEnd");
        }

        protected void OnDrawGizmosSelected()
        {
            if (Application.isPlaying)
            {
                return;
            }

            if (!GrabbingColliderSize.IsZero())
            {
                TSGizmoEx.DrawGizmoRectangle(position + (Vector3)GrabbingColliderOffset, GrabbingColliderSize, TSColors.CadetBlue);
            }
        }

        protected override void OnDeath(DamageResult damageResult)
        {
            base.OnDeath(damageResult);

            ForceStopGrab();
        }

        public void StartGrab(Character grabbedCharacter)
        {
            LogInfo($"그랩 캐릭터({Owner.Name.ToLogString()})가 잡기 상태를 설정합니다.");

            _isGrabbing = true;

            SetGrabbedCharacter(grabbedCharacter);
            SetGrabbedCharacterPositionOnEndGrab();
            SetGrabAnimationParameter(true);
        }

        public void StopGrab()
        {
            LogInfo($"그랩 캐릭터({Owner.Name.ToLogString()})가 잡기 상태를 해제합니다.");

            _isGrabbing = false;

            SetGrabbedCharacterPositionOnStartGrab();
            ResetGrabbedCharacter();
            SetGrabAnimationParameter(false);
        }

        public void ForceStopGrab()
        {
            if (_grabbedAbility != null)
            {
                _grabbedAbility.StopGrabbed();
            }
        }

        //

        private void SetGrabbedCharacter(Character grabbedCharacter)
        {
            if (grabbedCharacter != null)
            {
                _grabbedCharacter = grabbedCharacter;
                _grabbedAbility = grabbedCharacter.FindAbility<CharacterGrabbed>();
            }
        }

        private void ResetGrabbedCharacter()
        {
            if (_grabbedCharacter != null)
            {
                _grabbedCharacter = null;
                _grabbedAbility = null;
            }
        }

        private void SetGrabbedCharacterPositionOnEndGrab()
        {
            if (_grabbedCharacter != null)
            {
                if (GrabAttachmentPoint != null)
                {
                    LogInfo($"잡기 캐릭터가 잡은 캐릭터를 GrabAttachmenetPoint의 자식으로 설정합니다.");

                    _grabbedCharacter.transform.SetParent(GrabAttachmentPoint, true);
                    _grabbedCharacter.localPosition = Vector3.zero;
                    _grabbedCharacter.rotation = Quaternion.identity;
                }
            }
        }

        private void SetGrabbedCharacterPositionOnStartGrab()
        {
            if (_grabbedCharacter != null)
            {
                LogInfo($"잡기 캐릭터가 잡은 캐릭터를 GrabAttachmenetPoint의 자식에서 해제합니다.");

                _grabbedCharacter.transform.SetParent(null);
                if (GrabEndPoint != null)
                {
                    LogInfo($"잡기 캐릭터가 잡은 캐릭터를 지정된 GrabEndPoint의 위치로 보냅니다.");
                    _grabbedCharacter.position = GrabEndPoint.position;
                }
            }
        }

        //

        public void ResizeGrabbingCollider()
        {
            if (UseResizeColliderWhileGrabbing)
            {
                _defaultColliderSize = Owner.Controller.BoxCollider.size;
                _defaultColliderOffset = Owner.Controller.BoxCollider.offset;

                Owner.Controller.BoxCollider.size = GrabbingColliderSize;

                if (Owner.IsFacingRight)
                {
                    Owner.Controller.BoxCollider.offset = GrabbingColliderOffset;
                }
                else
                {
                    Owner.Controller.BoxCollider.offset = GrabbingColliderOffset.FlipX();
                }
            }
        }

        public void ResizeDefaultCollider()
        {
            if (UseResizeColliderWhileGrabbing)
            {
                Owner.Controller.BoxCollider.size = _defaultColliderSize;
                Owner.Controller.BoxCollider.offset = _defaultColliderOffset;
            }
        }

        //

        protected override void InitializeAnimatorParameters()
        {
            Animator?.AddAnimatorParameterIfExists(ANIMATOR_GRAB_SUCCESS_PARAMETER_NAME, out _grabSuccessAnimationParameter, AnimatorControllerParameterType.Bool, Owner.AnimatorParameters);
        }

        private void SetGrabAnimationParameter(bool value)
        {
            Animator?.UpdateAnimatorBool(_grabSuccessAnimationParameter, value, Owner.AnimatorParameters);
        }
    }
}