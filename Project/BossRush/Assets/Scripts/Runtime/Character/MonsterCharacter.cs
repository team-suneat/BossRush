namespace TeamSuneat
{
    public class MonsterCharacter : Character
    {
        public AIBrain Brain { get; private set; }

        public ChaseSystem Chase { get; private set; }

        public PatternSystem Pattern { get; private set; }

        public TargetJumpSystem TargetJump { get; private set; }

        public override LogTags LogTag => LogTags.Monster;

        protected override void Awake()
        {
            base.Awake();
            Brain = GetComponent<AIBrain>();
            Chase = GetComponentInChildren<ChaseSystem>();
            Pattern = GetComponentInChildren<PatternSystem>();
            TargetJump = GetComponentInChildren<TargetJumpSystem>();
        }

        public override void Initialize()
        {
            base.Initialize();

            CharacterManager.Instance.Register(this);
        }

        public override void BattleReady()
        {
            base.BattleReady();

            Brain?.Activate();
            Pattern?.LoadPatterns();
            IsBattleReady = true;
        }

        public override void OnDespawn()
        {
            base.OnDespawn();

            CharacterManager.Instance.Unregister(this);
        }

        //

        public override void LateLogicUpdate()
        {
            if (!ActiveSelf || IsBlockInput)
            {
                return;
            }

            // 상태 머신 업데이트 (입력 처리 및 상태 전환)
            if (StateMachine != null)
            {
                StateMachine.LogicUpdate();
            }

            // 상태 머신 업데이트 이후 초기화
            base.LateLogicUpdate();
        }

        public override void PhysicsUpdate()
        {
            if (!ActiveSelf)
            {
                return;
            }

            base.PhysicsUpdate();

            // 1. 상태 머신 FixedUpdate
            if (StateMachine != null)
            {
                StateMachine.PhysisUpdate();
            }

            // 2. 이동 속도 적용 (대시 중일 때는 일반 이동 입력 무시)
            if (Physics != null)
            {
                // ForceVelocity가 적용 중일 때는 입력 무시
                // 점프 중일 때도 입력 무시
                if (!Physics.IsDashing && !Physics.IsForceVelocity && !Physics.IsJumping)
                {
                    // 공격 중 이동 잠금 확인
                    bool isMovementLocked = CharacterAnimator != null && CharacterAnimator.IsMovementLocked;
                    if (!isMovementLocked)
                    {
                        // 즉각적인 반응: 입력에 바로 속도 적용 (가속/감속 없음)
                        float targetVelocityX = Command.HorizontalInput * Physics.MoveSpeed;

                        // CharacterPhysics를 통해 수평 속도 적용 (Y축 속도는 자동으로 유지됨)
                        Physics.ApplyHorizontalInput(targetVelocityX);
                    }
                    else
                    {
                        // 이동 잠금 중에는 수평 속도를 0으로 설정
                        Physics.ApplyHorizontalInput(0f);
                    }
                }
            }

            // 3. Model 스프라이트 방향 반전
            UpdateModelDirection();
        }

        protected override void OnDeath(DamageResult damageResult)
        {
            base.OnDeath(damageResult);

            CharacterManager.Instance.Unregister(this);
            transform.SetParent(null);

            GlobalEvent<Character>.Send(GlobalEventType.MONSTER_CHARACTER_DEATH, this);
        }
    }
}