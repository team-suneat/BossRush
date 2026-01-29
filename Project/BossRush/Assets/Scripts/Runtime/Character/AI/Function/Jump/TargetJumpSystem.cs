using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace TeamSuneat
{
    public class TargetJumpSystem : XBehaviour
    {
        [Title("#Jump Settings")]
        [SuffixLabel("착지 검사 전 대기 시간")]
        [SerializeField]
        private float _landingCheckDelay = 0.2f;

        public MonsterCharacter Owner { get; private set; }

        private CharacterPhysicsCore _physicsCore;
        private CharacterStateMachine _stateMachine;
        private CharacterAnimator _animator;
        private TargetJumpEntity[] _entities;

        private Vector2 _initialVelocity;
        private UnityAction _onCompleted;
        private Coroutine _jumpCoroutine;

        private void Awake()
        {
            Owner = this.FindFirstParentComponent<MonsterCharacter>();
        }

        private void Start()
        {
            _entities = GetComponentsInChildren<TargetJumpEntity>(true);

            if (Owner != null)
            {
                _stateMachine = Owner.StateMachine;
                _animator = Owner.CharacterAnimator;
                _physicsCore = Owner.GetComponent<CharacterPhysicsCore>();
            }
        }

        public void StartJumpToPattern(JumpDestinationType destinationType, PositionGroupNames positionGroupName, UnityAction onCompleted)
        {
            if (!TryGetResolvedDestination(destinationType, positionGroupName, out Vector2 destination))
            {
                Log.Warning(LogTags.TargetJump, "{0}, 목적지 획득 실패로 점프 없이 완료 콜백을 호출합니다. Type: {1}, PositionGroupName: {2}",
                    Owner?.Name.ToLogString() ?? "Unknown", destinationType, positionGroupName);
                onCompleted?.Invoke();
                return;
            }
            StartJumpToPattern(destination, onCompleted);
        }

        private void StartJumpToPattern(Vector2 targetWorldPosition, UnityAction onCompleted)
        {
            if (Owner == null || _physicsCore == null || _stateMachine == null || _animator == null)
            {
                Log.Warning(LogTags.TargetJump, "필수 컴포넌트가 null이라 점프를 시작하지 않습니다. Owner: {0}, Physics: {1}, StateMachine: {2}, Animator: {3}",
                    Owner != null, _physicsCore != null, _stateMachine != null, _animator != null);
                return;
            }

            Vector2 groundPosition = FindGroundPosition(targetWorldPosition);
            Vector2 startPosition = Owner.position;
            float dx = groundPosition.x - startPosition.x;
            TargetJumpEntity entity = GetPresetForDistance(dx);
            if (entity == null)
            {
                Log.Warning(LogTags.TargetJump, "{0}, 점프 프리셋을 찾을 수 없습니다.", Owner.Name.ToLogString());
                return;
            }

            DebugEx.DrawLine(startPosition, groundPosition, Color.red, 1f);
            DebugEx.DrawCross(groundPosition, 0.1f, Color.red, 1f);

            float gravity = Physics2D.gravity.magnitude * _physicsCore.Rigidbody.gravityScale;
            _initialVelocity = entity.CalculateParabolicVelocity(startPosition, groundPosition, gravity);

            // 애니메이션 트리거
            _animator.PlayJumpAnimation();

            // 콜백 저장
            _onCompleted = onCompleted;

            Log.Info(LogTags.TargetJump, "{0}, 점프 패턴을 시작합니다. {1}", Owner.Name.ToLogString(), entity.Name.ToLogString());

            // 코루틴 시작
            if (_jumpCoroutine != null)
            {
                StopCoroutine(_jumpCoroutine);
            }
            _jumpCoroutine = StartXCoroutine(ProcessJumpPattern());
        }

        private bool TryGetResolvedDestination(JumpDestinationType destinationType, PositionGroupNames positionGroupName, out Vector2 destination)
        {
            destination = default;
            if (Owner == null)
            {
                Log.Warning(LogTags.TargetJump, "Owner가 null이라 목적지를 조회할 수 없습니다.");
                return false;
            }

            switch (destinationType)
            {
                case JumpDestinationType.PositionGroup:
                    return TryGetPositionGroupDestination(positionGroupName, out destination);

                case JumpDestinationType.OwnerTarget:
                    if (Owner.TargetCharacter != null)
                    {
                        destination = Owner.TargetCharacter.position;
                        return true;
                    }
                    Log.Warning(LogTags.TargetJump, "{0}, OwnerTarget인데 TargetCharacter가 null입니다.", Owner.Name.ToLogString());
                    return false;

                default:
                    Log.Warning(LogTags.TargetJump, "{0}, 지원하지 않는 목적지 타입입니다. Type: {1}", Owner.Name.ToLogString(), destinationType);
                    return false;
            }
        }

        private bool TryGetPositionGroupDestination(PositionGroupNames positionGroupName, out Vector2 destination)
        {
            destination = default;
            if (positionGroupName == PositionGroupNames.None)
            {
                Log.Warning(LogTags.TargetJump, "{0}, 포지션 그룹 이름이 None입니다.", Owner.Name.ToLogString());
                return false;
            }

            PositionGroup positionGroup = PositionGroupManager.Instance?.Find(positionGroupName);
            if (positionGroup == null)
            {
                Log.Warning(LogTags.TargetJump, "{0}, 포지션 그룹을 찾을 수 없습니다. PositionGroupName: {1}, Instance null: {2}",
                    Owner.Name.ToLogString(), positionGroupName, PositionGroupManager.Instance == null);
                return false;
            }

            Vector3 origin = Owner.position;
            destination = (Vector2)positionGroup.GetPosition(origin);
            return true;
        }

        private TargetJumpEntity GetPresetForDistance(float distance)
        {
            if (_entities == null || _entities.Length == 0)
            {
                return null;
            }

            float d = Mathf.Abs(distance);
            TargetJumpEntity best = null;

            for (int i = 0; i < _entities.Length; i++)
            {
                TargetJumpEntity p = _entities[i];
                if (p == null)
                {
                    continue;
                }

                if (d <= p.MaxDistance)
                {
                    if (best == null || p.MaxDistance < best.MaxDistance)
                    {
                        best = p;
                    }
                }
            }

            if (best == null)
            {
                for (int i = 0; i < _entities.Length; i++)
                {
                    TargetJumpEntity p = _entities[i];
                    if (p == null)
                    {
                        continue;
                    }

                    if (best == null || p.MaxDistance > best.MaxDistance)
                    {
                        best = p;
                    }
                }
            }

            return best;
        }

        public void ExecuteJump()
        {
            if (_physicsCore == null)
            {
                return;
            }

            Log.Info(LogTags.TargetJump, "{0}, 점프 패턴을 실행합니다. {1}", Owner.Name.ToLogString(), _initialVelocity);

            // 상태 머신 전환
            _stateMachine.ChangeState(CharacterState.Jumping);

            // 점프 상태 설정
            _physicsCore.SetJumping(true);

            // 저장된 초기 속도 적용
            _physicsCore.ApplyVelocity(_initialVelocity);
        }

        private IEnumerator ProcessJumpPattern()
        {
            if (Owner == null || _stateMachine == null)
            {
                yield break;
            }

            // 떠오를 시간 동안 착지 검사 무시
            float elapsed = 0f;
            float delay = Mathf.Max(0f, _landingCheckDelay);
            while (elapsed < delay)
            {
                if (!Owner.IsAlive)
                {
                    yield break;
                }

                elapsed += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            while (true)
            {
                if (!Owner.IsAlive)
                {
                    Log.Info(LogTags.TargetJump, "{0}, 점프 패턴을 중단합니다. 캐릭터가 사망했습니다.", Owner.Name.ToLogString());
                    yield break;
                }

                // 상태 기반: Idle/Walk면 착지 완료
                CharacterState current = _stateMachine.CurrentState;
                if (current is CharacterState.Idle or CharacterState.Walk)
                {
                    _onCompleted?.Invoke();
                    Log.Info(LogTags.TargetJump, "{0}, 점프 패턴을 완료했습니다.", Owner.Name.ToLogString());
                    yield break;
                }

                yield return new WaitForFixedUpdate();
            }
        }

        private Vector2 FindGroundPosition(Vector2 targetPosition)
        {
            // targetWorldPosition에서 아래로 레이캐스트하여 실제 지면 위치 찾기
            float raycastDistance = 50f; // 충분히 긴 거리
            RaycastHit2D hit = Physics2D.Raycast(targetPosition, Vector2.down, raycastDistance, GameLayers.Mask.Collision);

            if (hit.collider != null)
            {
                return hit.point;
            }

            // 레이캐스트 실패 시 원래 위치 반환
            return targetPosition;
        }
    }
}