using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace TeamSuneat
{
    public class ChaseSystem : XBehaviour
    {
        public enum Types
        {
            Chase,

            Kiting,

            None,
        }

        public Character Owner { get; private set; }

        public Types Type;

        public Vector3 ChasePointOffset;

        [ShowIf("Type", Types.Chase)]
        [SuffixLabel("추격 최소 거리")]
        public float ChaseMinDistance;

        [ShowIf("Type", Types.Kiting)]
        [SuffixLabel("너무 가까우면 도망가지 않는다.")]
        [SuffixLabel("카이팅 최소 거리.")]
        public float KitingMinDistance;

        [ShowIf("Type", Types.Kiting)]
        [SuffixLabel("너무 멀면 도망가지 않는다.")]
        [SuffixLabel("카이팅 최대 거리.")]
        public float KitingMaxDistance;

        [ShowIf("Type", Types.Kiting)]
        [SuffixLabel("카이팅 최소 높이.")]
        public float KitingMinHeight;

        private CharacterCommand _currentCommand;

        private float _distanceBetweenTarget;

        private float _defaultChaseMinDistance;

        private Vector3 _targetPosition
        {
            get
            {
                if (Owner?.TargetCharacter != null)
                {
                    return Owner.TargetCharacter.position;
                }

                return Vector3.zero;
            }
        }

        private Vector3 _ownerPosition => Owner?.position ?? Vector3.zero;

        public override void AutoNaming()
        {
            base.AutoNaming();

            Owner = this.FindFirstParentComponent<Character>();
            if (Owner != null)
            {
                SetGameObjectName(string.Format("Chase ({0})", Owner.Name));
            }
            else
            {
                SetGameObjectName("Chase");
            }
        }
        private void Awake()
        {
            Owner = this.FindFirstParentComponent<MonsterCharacter>();
            _currentCommand = new CharacterCommand();
            _defaultChaseMinDistance = ChaseMinDistance;
        }

        public void ResetValues()
        {
            _currentCommand.Reset();

            _distanceBetweenTarget = 0;

            ChaseMinDistance = _defaultChaseMinDistance;
        }

        public bool CheckKitingPossible()
        {
            if (Type != Types.Kiting)
            {
                return false;
            }

            if (Owner.AssetData.IsFlying)
            {
                _distanceBetweenTarget = Vector2.Distance(_ownerPosition, _targetPosition);
            }
            else
            {
                _distanceBetweenTarget = Mathf.Abs(_ownerPosition.x - _targetPosition.x);
            }

            if (_distanceBetweenTarget < KitingMinDistance)
            {
                return false;
            }

            return _distanceBetweenTarget < KitingMaxDistance;
        }

        #region Command

        private void ResetCommand()
        {
            _currentCommand.Reset();
        }

        private void ExecuteCommand()
        {
            Owner.Command.CopyFrom(_currentCommand);
        }

        #endregion Command

        #region Chase Air

        public bool TryChaseAir()
        {
            return true;
        }

        public void ChaseInAir()
        {
            ResetCommand();

            Vector3 direction = _targetPosition - _ownerPosition + ChasePointOffset;
            if (direction.magnitude < ChaseMinDistance)
            {
                direction = Vector3.zero;
            }
            else
            {
                direction.Normalize();
            }

            _currentCommand.HorizontalInput = direction.x;
            _currentCommand.VerticalInput = direction.y;

            ExecuteCommand();
        }

        #endregion Chase Air

        #region Chase

        public bool TryChaseInGround()
        {
            if (Owner.TargetCharacter == null)
            {
                return false;
            }

            _distanceBetweenTarget = Mathf.Abs(_ownerPosition.x - _targetPosition.x);

            Vector3 direction = _targetPosition - _ownerPosition;

            if (_distanceBetweenTarget < ChaseMinDistance)
            {
                if (Owner.IsFacingRight && direction.x < 0)
                {
                    return true;
                }

                if (!Owner.IsFacingRight && direction.x > 0)
                {
                    return true;
                }

                return false;
            }

            return true;
        }

        public void ChaseInGround()
        {
            ResetCommand();

            if (_targetPosition.x > _ownerPosition.x)
            {
                _currentCommand.HorizontalInput = 1f;
            }
            else if (_targetPosition.x < _ownerPosition.x)
            {
                _currentCommand.HorizontalInput = -1f;
            }

            ExecuteCommand();
        }

        #endregion Chase

        #region Kiting

        public bool TryKitingAir()
        {
            return false;
        }

        public bool TryKitingGround()
        {
            if (Owner.TargetCharacter == null)
            {
                return false;
            }

            _distanceBetweenTarget = Mathf.Abs(_ownerPosition.x - _targetPosition.x);

            if (_distanceBetweenTarget < KitingMinDistance)
            {
                return false;
            }

            if (_distanceBetweenTarget > KitingMaxDistance)
            {
                return false;
            }

            if (Owner.Physics.IsCollideX)
            {
                return false;
            }

            if (Owner.Physics.BelowCollider != Owner.TargetCharacter.Physics.BelowCollider)
            {
                return false;
            }

            return true;
        }

        public void Kiting()
        {
            if (Owner.TargetCharacter == null)
            {
                return;
            }

            _currentCommand.Reset();

            if (Owner.AssetData.IsFlying)
            {
                KitingInAir();
            }
            else
            {
                KitingInGround();
            }

            ExecuteCommand();
        }

        public void KitingInGround()
        {
            ResetCommand();

            Vector3 direction = _targetPosition - _ownerPosition;

            if (direction.magnitude < KitingMaxDistance)
            {
                if (direction.magnitude > KitingMinDistance)
                {
                    direction = new Vector3(-direction.x, 0);
                }
                else
                {
                    direction = Vector3.zero;
                }
            }

            if (!direction.IsZero())
            {
                direction.Normalize();

                if (Owner.Physics.IsCollideX)
                {
                    direction.x = 0;
                }
            }

            _currentCommand.HorizontalInput = direction.x;
            _currentCommand.VerticalInput = direction.y;

            ExecuteCommand();
        }

        public void KitingInAir()
        {
            Vector3 direction = _targetPosition - _ownerPosition;

            if (direction.magnitude < KitingMaxDistance)
            {
                if (direction.magnitude < KitingMinDistance)
                {
                    direction = new Vector3(-direction.x, 1);
                }
                else
                {
                    float height = Mathf.Abs(_ownerPosition.y - _targetPosition.y);

                    if (_targetPosition.y > _ownerPosition.y)
                    {
                        direction = new Vector3(0, 1);
                    }
                    else
                    {
                        if (height < KitingMinHeight)
                        {
                            direction = new Vector3(0, 1);
                        }
                        else
                        {
                            direction = new Vector3(0, 0);
                        }
                    }
                }
            }

            if (direction.magnitude > 0.001f)
            {
                direction.Normalize();
            }

            _currentCommand.HorizontalInput = direction.x;
            _currentCommand.VerticalInput = direction.y;
        }

        #endregion Kiting

        public void SetChaseMinDistance(float minDistance)
        {
            ChaseMinDistance = minDistance;
        }

        public void ResetChaseMinDistance()
        {
            ChaseMinDistance = _defaultChaseMinDistance;
        }

        public void StartChaseGroundPattern(UnityAction onCompleted)
        {
            _ = StartXCoroutine(ProcessChaseGroundPattern(onCompleted));
        }

        private IEnumerator ProcessChaseGroundPattern(UnityAction onCompleted)
        {
            WaitForFixedUpdate wait = new();

            while (TryChaseInGround())
            {
                yield return wait;

                ChaseInGround();
            }

            onCompleted?.Invoke();
        }
    }
}