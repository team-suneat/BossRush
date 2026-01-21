using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Diagnostics;

namespace TeamSuneat
{
    public class PatternSystem : XBehaviour
    {
        [SuffixLabel("패턴 순서")]
        public Order PatternOrder;

        [SuffixLabel("패턴 확률")]
        public Gacha Gacha;

        [SuffixLabel("페이즈")]
        public CharacterPhase[] Phases;

        private List<CharacterPattern> _usablePatterns = new();
        private int _maxPhase = -1;

        public MonsterCharacter Owner { get; private set; }

        public bool IsStartPattern { get; protected set; }

        public CharacterPattern CurrentPattern
        {
            get
            {
                if (!_usablePatterns.IsValid())
                {
                    Log.Warning(LogTags.Pattern, "사용 가능한 패턴 리스트가 유효하지 않습니다.");
                    return null;
                }

                if (_usablePatterns.Count <= PatternOrder.Current)
                {
                    Log.Warning(LogTags.Pattern, "패턴 순서가 범위를 벗어났습니다. Current: {0}, Count: {1}. 순서를 0으로 초기화합니다.", PatternOrder.Current, _usablePatterns.Count);
                    PatternOrder.Set(0);
                }

                return _usablePatterns[PatternOrder.Current];
            }
        }

        public CharacterPatternStep CurrentPatternStep => CurrentPattern != null ? CurrentPattern.GetStep() : null;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();
            Phases = GetComponentsInChildren<CharacterPhase>();
        }

        private void OnValidate()
        {
            Gacha?.Validate();
        }

        private void Awake()
        {
            Owner = this.FindFirstParentComponent<MonsterCharacter>();
            Phases = GetComponentsInChildren<CharacterPhase>();
            Log.Info(LogTags.Pattern, "PatternSystem 초기화 완료. Phase 개수: {0}", Phases?.Length ?? 0);
        }

        protected override void OnStart()
        {
            base.OnStart();
            SetPatternProbabilities();
        }

        private void OnDrawGizmos()
        {
            if (CurrentPatternStep != null)
            {
                GizmoEx.DrawText(
                    $"StepName: {CurrentPatternStep.StepName}\n" +
                    $"OrderIndex: {CurrentPatternStep.OrderIndex}\n" +
                    $"IsStartPattern: {IsStartPattern}\n",
                transform.position);
            }
        }

        [FoldoutGroup("#Buttons", 999)]
        [Button("Auto Setup Gacha & Order", ButtonSizes.Medium)]
        [Conditional("UNITY_EDITOR")]
        private void SetPatternProbabilitiesForEditor()
        {
            AutoGetComponents();
            if (Phases != null)
            {
                for (int i = 0; i < Phases.Length; i++)
                {
                    if (Phases[i] != null)
                    {
                        Phases[i].AutoGetComponents();
                    }
                }
            }

            SetPatternProbabilities();
        }

        private void SetPatternProbabilities()
        {
            if (Phases == null)
            {
                Log.Warning(LogTags.Pattern, "Phases가 null입니다. 패턴 확률을 설정할 수 없습니다.");
                return;
            }

            int patternCount = 0;
            List<float> patternProbabilities = new();

            for (int i = 0; i < Phases.Length; i++)
            {
                CharacterPhase currentPhase = Phases[i];
                if (currentPhase == null)
                {
                    Log.Warning(LogTags.Pattern, "Phase[{0}]가 null입니다.", i);
                    continue;
                }

                patternCount += currentPhase.PatternLength;
                for (int j = 0; j < currentPhase.PatternLength; j++)
                {
                    CharacterPattern currentPattern = currentPhase.Patterns[j];
                    if (currentPattern != null)
                    {
                        patternProbabilities.Add(currentPattern.ProbabilityToPicked);
                    }
                }
            }

            PatternOrder.SetMax(patternCount);

            if (patternProbabilities.Count > 0 && Gacha != null)
            {
                Gacha.Clear();
                for (int i = 0; i < patternProbabilities.Count; i++)
                {
                    Gacha.Add(patternProbabilities[i], i);
                }
                Log.Info(LogTags.Pattern, "패턴 확률 설정 완료. 전체 패턴 개수: {0}", patternCount);
            }
            else
            {
                Log.Warning(LogTags.Pattern, "설정할 패턴이 없습니다. 인스펙터에 설정된 Gacha 값을 유지합니다.");
            }
        }

        private void LoadPatternProbabilityToPicked()
        {
            if (!_usablePatterns.IsValid() || Gacha == null)
            {
                return;
            }

            Gacha.Clear();
            for (int i = 0; i < _usablePatterns.Count; i++)
            {
                if (_usablePatterns[i] == null)
                {
                    continue;
                }

                float probability = _usablePatterns[i].ProbabilityToPicked;
                Gacha.Add(probability, i);
            }
        }

        public void LoadPatterns()
        {
            if (Phases == null)
            {
                Log.Warning(LogTags.Pattern, "Phases가 null입니다. 패턴을 로드할 수 없습니다.");
                return;
            }

            if (Owner == null)
            {
                Log.Error(LogTags.Pattern, "Owner가 null입니다.");
                return;
            }

            float healthRate = Owner.MyVital?.GetRate(VitalResourceTypes.Life) ?? -1f;
            Log.Info(LogTags.Pattern, "패턴 로드 시작. Owner 체력 비율: {0}", healthRate);

            int maxPhase = GetMaxPhase();
            if (_maxPhase >= maxPhase)
            {
                Log.Info(LogTags.Pattern, "이미 최대 페이즈({0})에 도달했습니다. 패턴 로드를 건너뜁니다.", _maxPhase);
                return;
            }

            Log.Info(LogTags.Pattern, "패턴 로드 시작. 이전 MaxPhase: {0}, 새 MaxPhase: {1}", _maxPhase, maxPhase);

            _maxPhase = maxPhase;

            for (int i = 0; i < Phases.Length; i++)
            {
                if (Phases[i] == null)
                {
                    continue;
                }

                Phases[i].SetMaxPhase(maxPhase);

                Log.Info(LogTags.Pattern, "Phase[{0}]: IsLocked={1}, ConditionHealthRate={2}, PatternCount={3}",
                    i, Phases[i].IsLocked, Phases[i].ConditionHealthRate, Phases[i].PatternLength);

                if (Phases[i].TryLock())
                {
                    Log.Info(LogTags.Pattern, "Phase[{0}] 잠금 처리됨", i);
                    Phases[i].Lock();
                    RemoveUsablePattern(Phases[i].Patterns);
                }
                else if (Phases[i].TryUnlock(Owner))
                {
                    Log.Info(LogTags.Pattern, "Phase[{0}] 잠금 해제됨", i);
                    Phases[i].Unlock();
                    AddUsablePattern(Phases[i].Patterns);
                }
                else
                {
                    Log.Warning(LogTags.Pattern, "Phase[{0}] 잠금/해제 조건을 만족하지 않음", i);
                }
            }

            LoadPatternProbabilityToPicked();
            Log.Info(LogTags.Pattern, "패턴 로드 완료. 사용 가능한 패턴 개수: {0}", _usablePatterns.Count);
        }

        private int GetMaxPhase()
        {
            if (Phases == null || Owner == null)
            {
                return 0;
            }

            int maxPhase = 0;
            for (int i = Phases.Length - 1; i >= 0; i--)
            {
                if (Phases[i] != null && Phases[i].CheckConditionHealthRate(Owner))
                {
                    if (maxPhase < i)
                    {
                        maxPhase = i;
                    }
                }
            }

            Log.Info(LogTags.Pattern, "최대 페이즈 계산 완료: {0}", maxPhase);
            return maxPhase;
        }

        private void AddUsablePattern(CharacterPattern[] patterns)
        {
            if (patterns == null)
            {
                return;
            }

            for (int i = 0; i < patterns.Length; i++)
            {
                if (patterns[i] != null && !_usablePatterns.Contains(patterns[i]))
                {
                    _usablePatterns.Add(patterns[i]);
                    Log.Info(LogTags.Pattern, "사용하는 패턴을 추가합니다. Order: {0}, Name: {1}", patterns[i].Order, patterns[i].Name);
                }
            }

            if (_usablePatterns.Count > 0)
            {
                PatternOrder.SetMax(_usablePatterns.Count - 1);
            }
        }

        private void RemoveUsablePattern(CharacterPattern[] patterns)
        {
            if (patterns == null)
            {
                return;
            }

            for (int i = 0; i < patterns.Length; i++)
            {
                if (patterns[i] != null && _usablePatterns.Contains(patterns[i]))
                {
                    _usablePatterns.Remove(patterns[i]);
                    Log.Info(LogTags.Pattern, "사용하지 않는 패턴을 삭제합니다. Order: {0}, Name: {1}", patterns[i].Order, patterns[i].Name);
                }
            }

            if (_usablePatterns.Count > 0)
            {
                PatternOrder.SetMax(_usablePatterns.Count - 1);
            }
        }

        public void StartPattern()
        {
            IsStartPattern = true;

            if (CurrentPattern != null)
            {
                Log.Info(LogTags.Pattern, "패턴을 시작합니다. Order: {0}, Name: {1}", PatternOrder.Current, CurrentPattern.Name);
            }
            else
            {
                Log.Warning(LogTags.Pattern, "패턴을 시작할 수 없습니다. Order: {0}", PatternOrder.Current);
            }

            ProcessStep();
        }

        public void PickPattern()
        {
            if (CurrentPattern == null)
            {
                Log.Warning(LogTags.Pattern, "현재 패턴이 null입니다. 패턴을 선정할 수 없습니다.");
                return;
            }

            CurrentPattern.FirstStep();

            if (Gacha != null)
            {
                Gacha.Refresh();

                int cooldownCount = 0;
                for (int i = 0; i < _usablePatterns.Count; i++)
                {
                    if (_usablePatterns[i] != null && _usablePatterns[i].IsCooldown)
                    {
                        Gacha.LockAt(i);
                        cooldownCount++;
                    }
                }

                if (cooldownCount > 0)
                {
                    Log.Info(LogTags.Pattern, "쿨다운 중인 패턴 개수: {0}", cooldownCount);
                }

                if (!Gacha.CheckLockAll())
                {
                    int pickedOrder = Gacha.Pick();
                    PatternOrder.Set(pickedOrder);
                }
                else
                {
                    Log.Warning(LogTags.Pattern, "모든 패턴이 잠겨있습니다. 패턴을 선정할 수 없습니다.");
                }
            }
            else
            {
                Log.Info(LogTags.Pattern, "Gacha가 null입니다. 순서대로 패턴을 선택합니다.");
                PatternOrder.Shuffle();
            }

            IsStartPattern = false;

            if (CurrentPattern != null && CurrentPatternStep != null)
            {
                Log.Info(LogTags.Pattern, "패턴을 선정합니다. Order: {0}, Pattern: {1}, Step: {2}",
                    PatternOrder.Current, CurrentPattern.Name, CurrentPatternStep.StepName);
            }
        }

        public void NextStep()
        {
            if (CurrentPatternStep == null)
            {
                return;
            }

            if (CurrentPatternStep.IsCompleteStepRepeat)
            {
                Log.Info(LogTags.Pattern, "다음 스텝으로 이동. 현재 스텝: {0}", CurrentPatternStep.StepName);
                CurrentPattern.NextStep();
            }
        }

        public void ProcessStep()
        {
            if (CurrentPatternStep == null)
            {
                return;
            }

            Log.Info(LogTags.Pattern, "스텝 처리. Pattern: {0}, Step: {1}", CurrentPattern.Name, CurrentPatternStep.StepName);
            CurrentPatternStep.ProcessStep();
        }

        public void ProcessNextStep()
        {
            NextStep();
            ProcessStep();
        }
    }
}