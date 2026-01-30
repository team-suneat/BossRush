using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TeamSuneat
{
    public class CharacterPatternStep : XBehaviour
    {
        [Title("#PatternStep")]
        public PatternStepNames StepName;

        [Title("#Face")]
        [ShowIf("StepName", PatternStepNames.FaceDirectional)]
        public FacingDirections FacingDirection;

        [ShowIf("StepName", PatternStepNames.FaceToPositionGroup)]
        [SuffixLabel("목표 포지션 그룹의 이름")]
        public PositionGroupNames FacePositionGroupName;

        [SuffixLabel("벽 충돌 거리")]
        public float FaceAgainstWallDistance;

        [Title("#Interrupt")]
        [SuffixLabel("InterruptCurrentPattern으로 스텝 넘기기 허용")]
        [SerializeField]
        private bool _canInterruptStep = true;

        public bool CanInterruptStep => _canInterruptStep;

        [Title("#Order")]
        [SuffixLabel("랜덤 순서 사용")]
        public bool UseRandomOrder;

        [SuffixLabel("순서 인덱스")]
        public int OrderIndex;

        [ShowIf("UseRandomOrder")]
        [SuffixLabel("순서 최대 인덱스")]
        public int OrderMaxIndex;

        [Title("#Repeat")]
        [SuffixLabel("반복 사용")]
        public bool UseRepeat;

        [EnableIf("UseRepeat")]
        [SuffixLabel("반복 최대 횟수")]
        public int CurrentRepeatMaxCount;

        [EnableIf("UseRepeat")]
        [SuffixLabel("랜덤 반복 사용")]
        public bool UseRandomRepeat;

        [EnableIf("UseRandomRepeat")]
        [SuffixLabel("무작위 반복 최소 횟수")]
        public int RepeatMinCount;

        [EnableIf("UseRandomRepeat")]
        [SuffixLabel("무작위 반복 최대 횟수")]
        public int RepeatMaxCount;

        public int CurrentRepeatCount { get; private set; }

        [Title("#Jump")]
        [ShowIf("StepName", PatternStepNames.JumpToPositionGroup)]
        [SuffixLabel("착지 포지션 그룹의 이름")]
        public PositionGroupNames JumpPositionGroupName;

        [FoldoutGroup("#String")]
        public string StepNameString;

        [FoldoutGroup("#String")]
        public string FacingDirectionString;

        [FoldoutGroup("#String")]
        public string FacePositionGroupNameString;

        [FoldoutGroup("#String")]
        public string JumpPositionGroupNameString;

        [FoldoutGroup("#Event")]
        public UnityEvent OnFailureCallback;

        private Coroutine _nextStepCoroutine;

        public bool IsCompleteStepRepeat => CurrentRepeatCount >= CurrentRepeatMaxCount;
        public MonsterCharacter Owner { get; private set; }
        public PatternSystem System { get; private set; }
        public CharacterPattern Pattern { get; private set; }

        public override void AutoSetting()
        {
            base.AutoSetting();
            StepNameString = StepName.ToString();
            FacingDirectionString = FacingDirection.ToString();
            FacePositionGroupNameString = FacePositionGroupName.ToString();
            JumpPositionGroupNameString = JumpPositionGroupName.ToString();
        }

        private void OnValidate()
        {
            _ = EnumEx.ConvertTo(ref StepName, StepNameString);
            _ = EnumEx.ConvertTo(ref FacingDirection, FacingDirectionString);
            _ = EnumEx.ConvertTo(ref FacePositionGroupName, FacePositionGroupNameString);
            _ = EnumEx.ConvertTo(ref JumpPositionGroupName, JumpPositionGroupNameString);
        }

        public override void AutoNaming()
        {
            if (StepName is PatternStepNames.Attack or PatternStepNames.AttackWithFace or PatternStepNames.AttackWithCheckArea)
            {
                SetGameObjectName($"Step ({StepName} {OrderIndex})");
            }
            else
            {
                SetGameObjectName($"Step ({StepName})");
            }
        }

        private void Awake()
        {
            Owner = this.FindFirstParentComponent<MonsterCharacter>();
            System = this.FindFirstParentComponent<PatternSystem>();
            Pattern = this.FindFirstParentComponent<CharacterPattern>();

            if (Owner == null)
            {
                Log.Warning(LogTags.Pattern, "{0}, Owner(MonsterCharacter)를 찾을 수 없습니다.", this.GetHierarchyPath());
            }

            if (System == null)
            {
                Log.Warning(LogTags.Pattern, "{0}, System(PatternSystem)를 찾을 수 없습니다.", this.GetHierarchyPath());
            }

            if (Pattern == null)
            {
                Log.Warning(LogTags.Pattern, "{0}, Pattern(CharacterPattern)를 찾을 수 없습니다.", this.GetHierarchyPath());
            }
        }

        public void RefreshRepeatMaxCount()
        {
            if (UseRepeat && UseRandomRepeat)
            {
                CurrentRepeatMaxCount = RandomEx.Range(RepeatMinCount - 1, RepeatMaxCount);
                Log.Info(LogTags.Pattern, "(Step) {0}, 랜덤 반복 횟수 설정. 최소: {1}, 최대: {2}, 설정된 값: {3}",
                    Pattern?.Name.ToSelectString() ?? "Unknown", RepeatMinCount, RepeatMaxCount, CurrentRepeatMaxCount);
            }
        }

        public void ProcessStep()
        {
            if (Pattern == null)
            {
                Log.Error("{0}, Pattern is null.", this.GetHierarchyPath());
            }

            if (!Owner.IsAlive)
            {
                Log.Warning(LogTags.Pattern, "{0}, 오너 캐릭터가 죽어 패턴을 진행할 수 없습니다. ", Owner.Name.ToLogString());

                return;
            }

            Log.Info(LogTags.Pattern, "(Step) {0}, 패턴을 진행합니다. 단계:{1}, 순서: {2}", Pattern.Name.ToSelectString(), StepName.ToSelectString(), OrderIndex.ToSelectString());

            switch (StepName)
            {
                case PatternStepNames.ConditionalGround:
                    {
                        if (ExecuteConditionalGroundStep())
                        {
                            Pattern.StartWait(System.PickPattern);
                        }
                    }
                    break;

                case PatternStepNames.ConditionalPlatform:
                    {
                        if (ExecuteConditionalPlatformStep())
                        {
                            Pattern.StartWait(System.PickPattern);
                        }
                    }
                    break;

                case PatternStepNames.Face:
                    {
                        ExecuteFaceToTargetStep();

                        ExecuteNextStep();
                    }
                    break;

                case PatternStepNames.FaceDirectional:
                    {
                        ExecuteFaceDirectionalStep();

                        ExecuteNextStep();
                    }
                    break;

                case PatternStepNames.FaceToPositionGroup:
                    {
                        ExecuteFaceToPositionGroupStep();

                        ExecuteNextStep();
                    }
                    break;

                case PatternStepNames.ChaseGround:
                    {
                        ExecuteChaseGroundStep();
                    }
                    break;

                case PatternStepNames.JumpToTarget:
                    {
                        ExecuteJumpToTargetStep();
                    }
                    break;

                case PatternStepNames.JumpToPositionGroup:
                    {
                        ExecuteJumpToPositionGroupStep();
                    }
                    break;

                case PatternStepNames.Attack:
                    {
                        ExecuteAttackStep();
                    }
                    break;

                case PatternStepNames.AttackWithFace:
                    {
                        ExecuteFaceToTargetStep();
                        ExecuteAttackStep();
                    }
                    break;

                case PatternStepNames.AttackWithCheckArea:
                    {
                        ExecuteAttackWithCheckAreaStep();
                    }
                    break;

                case PatternStepNames.Complete:
                    {
                        Pattern.StartCooldown();
                        Pattern.StartWait(System.PickPattern);
                    }
                    break;
            }
        }

        public void ResetCurrentRepeatCount()
        {
            CurrentRepeatCount = 0;

            Log.Info(LogTags.Pattern, "(Step) {0}, 패턴의 반복 횟수를 초기화합니다. 단계: {1}", Pattern.Name.ToSelectString(), StepName.ToSelectString());
        }

        protected void AddRepeatCount()
        {
            if (IsCompleteStepRepeat)
            {
                CurrentRepeatCount = 0;
            }
            else
            {
                CurrentRepeatCount++;
            }

            Log.Info(LogTags.Pattern, "(Step) {0}, 패턴의 반복 횟수를 설정합니다. 단계: {1}", Pattern.Name.ToSelectString(), StepName.ToSelectString());
        }

        protected void ProcessNextStep()
        {
            Log.Info(LogTags.Pattern, "(Step) {0}, 패턴의 다음 단계로 넘어갑니다. 단계: {1}", Pattern.Name.ToSelectString(), StepName.ToSelectString());

            System.ProcessNextStep();
            _nextStepCoroutine = null;
        }

        protected void SkipToNextStep()
        {
            Log.Info(LogTags.Pattern, "(Step) {0}, 패턴 스텝을 건너뛰고 다음 단계로 이동합니다. 단계: {1}", Pattern.Name.ToSelectString(), StepName.ToSelectString());

            System.SkipToNextStep();
            _nextStepCoroutine = null;
        }

        #region Execute

        public void ExecuteNextStep()
        {
            if (_nextStepCoroutine != null)
            {
                Log.Warning(LogTags.Pattern, "{0}, 이미 다음 단계를 진행 중입니다. 다음 단계를 진행할 수 없습니다.", Pattern.Name.ToSelectString());
                return;
            }

            Log.Info(LogTags.Pattern, "(Step) {0}, 패턴의 다음 단계를 진행합니다. 단계: {1}", Pattern.Name.ToSelectString(), StepName.ToSelectString());
            AddRepeatCount();
            _nextStepCoroutine = CoroutineNextFrame(ProcessNextStep);
        }

        private void ExecuteFaceToTargetStep()
        {
            if (Owner.TryFlip())
            {
                Log.Info(LogTags.Pattern, "(Step) {0}, 타겟을 향해 방향을 전환합니다.", Pattern?.Name.ToSelectString() ?? "Unknown");
                Owner.FaceToTarget();
            }

            if (CheckAgainstWall(FaceAgainstWallDistance))
            {
                FacingDirections direction = Owner.IsFacingRight ? FacingDirections.Right : FacingDirections.Left;
                Log.Info(LogTags.Pattern, "(Step) {0}, 벽에 부딪혀 방향을 강제로 변경합니다. 거리: {1}, 방향: {2}",
                    Pattern?.Name.ToSelectString() ?? "Unknown", FaceAgainstWallDistance, direction);
                Owner.ForceFace(direction);
            }
        }

        private void ExecuteFaceDirectionalStep()
        {
            if (FacingDirection == FacingDirections.Left)
            {
                Log.Info(LogTags.Pattern, "(Step) {0}, 왼쪽 방향으로 강제 전환합니다.", Pattern?.Name.ToSelectString() ?? "Unknown");
                Owner.ForceFace(FacingDirections.Left);
            }
            else if (FacingDirection == FacingDirections.Right)
            {
                Log.Info(LogTags.Pattern, "(Step) {0}, 오른쪽 방향으로 강제 전환합니다.", Pattern?.Name.ToSelectString() ?? "Unknown");
                Owner.ForceFace(FacingDirections.Right);
            }
        }

        private void ExecuteFaceToPositionGroupStep()
        {
            if (FacePositionGroupName == PositionGroupNames.None)
            {
                Log.Warning(LogTags.Pattern, "{0}, FacePositionGroupName이 None입니다. FaceToPositionGroup 스텝을 건너뜁니다.",
                    Pattern?.Name.ToSelectString() ?? "Unknown");
                return;
            }

            PositionGroup positionGroup = PositionGroupManager.Instance.Find(FacePositionGroupName);
            if (positionGroup == null)
            {
                Log.Warning(LogTags.Pattern, "{0}, 포지션 그룹을 찾을 수 없습니다. 그룹: {1}. FaceToPositionGroup 스텝을 건너뜁니다.",
                    Pattern?.Name.ToSelectString() ?? "Unknown", FacePositionGroupName.ToSelectString());
                return;
            }

            Vector3 targetPosition = positionGroup.GetPosition(Owner.position);
            Log.Info(LogTags.Pattern, "(Step) {0}, 포지션 그룹 목표를 바라봅니다. 그룹: {1}",
                Pattern?.Name.ToSelectString() ?? "Unknown", FacePositionGroupName.ToSelectString());
            Owner.ForceFace(targetPosition);
        }

        private void ExecuteChaseGroundStep()
        {
            if (Owner.Chase != null)
            {
                Log.Info(LogTags.Pattern, "(Step) {0}, 지상 추적 패턴을 시작합니다.", Pattern?.Name.ToSelectString() ?? "Unknown");
                Owner.Chase.StartChaseGroundPattern(ExecuteNextStep);
            }
            else
            {
                Log.Warning(LogTags.Pattern, "{0}, Owner.Chase가 null입니다. 지상 추적 패턴을 실행할 수 없습니다.",
                    Pattern?.Name.ToSelectString() ?? "Unknown");
            }
        }

        private void ExecuteJumpToTargetStep()
        {
            if (!Owner.Physics.IsGrounded)
            {
                Log.Warning(LogTags.Pattern, "{0}, 지상이 아니어서 점프 스텝을 건너뜁니다. 다음 스텝으로 이동합니다.",
                    Pattern?.Name.ToSelectString() ?? "Unknown");
                ExecuteNextStep();
                return;
            }

            if (Owner.TargetJump == null)
            {
                Log.Warning(LogTags.Pattern, "{0}, Owner.TargetJump가 null입니다. 점프 스텝을 실행할 수 없습니다. 다음 스텝으로 이동합니다.",
                    Pattern?.Name.ToSelectString() ?? "Unknown");
                ExecuteNextStep();
                return;
            }

            Log.Info(LogTags.Pattern, "(Step) {0}, 타겟 방향 점프 패턴을 시작합니다.",
                Pattern?.Name.ToSelectString() ?? "Unknown");

            Owner.TargetJump.StartJumpToPattern(JumpDestinationType.OwnerTarget, PositionGroupNames.None, ExecuteNextStep);
        }

        private void ExecuteJumpToPositionGroupStep()
        {
            if (!Owner.Physics.IsGrounded)
            {
                Log.Warning(LogTags.Pattern, "{0}, 지상이 아니어서 점프 스텝을 건너뜁니다. 다음 스텝으로 이동합니다.",
                    Pattern?.Name.ToSelectString() ?? "Unknown");
                ExecuteNextStep();
                return;
            }

            if (Owner.TargetJump == null)
            {
                Log.Warning(LogTags.Pattern, "{0}, Owner.TargetJump가 null입니다. 점프 스텝을 실행할 수 없습니다. 다음 스텝으로 이동합니다.",
                    Pattern?.Name.ToSelectString() ?? "Unknown");
                ExecuteNextStep();
                return;
            }

            if (JumpPositionGroupName == PositionGroupNames.None)
            {
                Log.Warning(LogTags.Pattern, "{0}, JumpPositionGroupName이 None입니다. 점프 스텝을 건너뜁니다.",
                    Pattern?.Name.ToSelectString() ?? "Unknown");
                ExecuteNextStep();
                return;
            }

            Log.Info(LogTags.Pattern, "(Step) {0}, 포지션 그룹 점프 패턴을 시작합니다. 그룹: {1}",
                Pattern?.Name.ToSelectString() ?? "Unknown", JumpPositionGroupName.ToSelectString());

            Owner.TargetJump.StartJumpToPattern(JumpDestinationType.PositionGroup, JumpPositionGroupName, ExecuteNextStep);
        }

        private void ExecuteAttackStep()
        {
            int stepOrder = Pattern.GetCurrentStepOrder();

            if (Owner.StateMachine is MonsterStateMachine monsterStateMachine)
            {
                Log.Info(LogTags.Pattern, "(Step) {0}, 공격 패턴을 실행합니다. StepOrder: {1}",
                    Pattern?.Name.ToSelectString() ?? "Unknown", stepOrder);
                monsterStateMachine.SetAttackOrder(new List<int> { stepOrder });
                Owner.Command.SetAttackPressed(true);
            }
            else
            {
                Log.Warning(LogTags.Pattern, "{0}, StateMachine이 MonsterStateMachine이 아니거나 null입니다. 공격 패턴을 실행할 수 없습니다.",
                    Pattern?.Name.ToSelectString() ?? "Unknown");
            }
        }

        private void ExecuteAttackWithCheckAreaStep()
        {
            if (Owner.Attack == null)
            {
                Log.Warning(LogTags.Pattern, "{0}, Owner.Attack이 null입니다. 공격 범위 체크 패턴을 실행할 수 없습니다.",
                    Pattern?.Name.ToSelectString() ?? "Unknown");
                return;
            }

            if (!Owner.Attack.CheckTargetInAttackableArea(OrderIndex))
            {
                Log.Info(LogTags.Pattern, "(Step) {0}, 공격 가능 범위에 타겟이 없습니다. 다음 스텝으로 이동합니다. OrderIndex: {1}",
                    Pattern?.Name.ToSelectString() ?? "Unknown", OrderIndex);
                SkipToNextStep();
                return;
            }

            int stepOrder = Pattern.GetCurrentStepOrder();

            if (Owner.StateMachine is MonsterStateMachine monsterStateMachine)
            {
                Log.Info(LogTags.Pattern, "(Step) {0}, 공격 범위 체크 패턴을 실행합니다. StepOrder: {1}, OrderIndex: {2}",
                    Pattern?.Name.ToSelectString() ?? "Unknown", stepOrder, OrderIndex);
                monsterStateMachine.SetAttackOrder(new List<int> { stepOrder });
                Owner.Command.SetAttackPressed(true);
            }
            else
            {
                Log.Warning(LogTags.Pattern, "{0}, StateMachine이 MonsterStateMachine이 아니거나 null입니다. 공격 범위 체크 패턴을 실행할 수 없습니다.",
                    Pattern?.Name.ToSelectString() ?? "Unknown");
            }
        }

        private bool ExecuteConditionalGroundStep()
        {
            if (Owner.Physics.IsOnOneWayPlatform)
            {
                Log.Info(LogTags.Pattern, "(Step) {0}, 일방향 플랫폼 위에 있어 다음 스텝으로 이동합니다.",
                    Pattern?.Name.ToSelectString() ?? "Unknown");
                SkipToNextStep();
                return false;
            }

            Log.Info(LogTags.Pattern, "(Step) {0}, 지면 조건을 만족합니다. 패턴을 계속 진행합니다.",
                Pattern?.Name.ToSelectString() ?? "Unknown");
            return true;
        }

        private bool ExecuteConditionalPlatformStep()
        {
            if (Owner.Physics.IsOnOneWayPlatform)
            {
                Log.Info(LogTags.Pattern, "(Step) {0}, 일방향 플랫폼 위에 있어 다음 스텝으로 이동합니다.",
                    Pattern?.Name.ToSelectString() ?? "Unknown");
                SkipToNextStep();
                return false;
            }

            Log.Info(LogTags.Pattern, "(Step) {0}, 플랫폼 조건을 만족합니다. 패턴을 계속 진행합니다.",
                Pattern?.Name.ToSelectString() ?? "Unknown");
            return true;
        }

        #endregion Execute

        private void OnFailurePatternStep()
        {
            OnFailureCallback?.Invoke();
        }

        private void StartExecuteCommand(CharacterCommand command)
        {
            _ = StartXCoroutine(ProcessExecuteCommand(command));
        }

        private IEnumerator ProcessExecuteCommand(CharacterCommand command)
        {
            while (true)
            {
                yield return new WaitForEndOfFrame();

                Owner.Command.CopyFrom(command);
            }
        }

        private bool CheckAgainstWall(float distance)
        {
            if (distance <= 0f)
            {
                return false;
            }

            Vector2 direction = Owner.IsFacingRight ? Vector2.right : Vector2.left;
            RaycastHit2D hit = Physics2D.Raycast(position, direction, distance, GameLayers.Mask.Collision);

            if (hit.collider != null)
            {
                DebugEx.DrawLine(position, hit.point, Color.red, 3f);
                return true;
            }

            DebugEx.DrawRay(position, Vector2.right * direction * distance, Color.green, 3f);
            return false;
        }
    }
}