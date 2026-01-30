using System.Collections.Generic;
using System.Text;

namespace TeamSuneat
{
    public class PatternSystem : XBehaviour
    {
        private List<CharacterPattern> _usablePatterns = new();
        private int _maxPhase = -1;
        private MonsterCharacter _owner;
        private CharacterPhase[] _phases;
        private Gacha _gacha = new();
        private Order _patternOrder = new();

        private CharacterPattern CurrentPattern
        {
            get
            {
                if (!_usablePatterns.IsValid())
                {
                    Log.Warning(LogTags.Pattern, "(System) 사용 가능한 패턴 리스트가 유효하지 않습니다.");
                    return null;
                }

                if (_usablePatterns.Count <= _patternOrder.Current)
                {
                    Log.Warning(LogTags.Pattern, "(System) 패턴 순서가 범위를 벗어났습니다. Current: {0}, Count: {1}. 순서를 0으로 초기화합니다.", _patternOrder.Current, _usablePatterns.Count);
                    _patternOrder.Set(0);
                }

                return _usablePatterns[_patternOrder.Current];
            }
        }

        private CharacterPatternStep CurrentPatternStep
        {
            get
            {
                CharacterPattern currentPattern = CurrentPattern;
                if (currentPattern != null)
                {
                    return currentPattern.GetCurrentStep();
                }
                return null;
            }
        }

        public bool IsStartPattern { get; protected set; }

        private bool _isCurrentPatternInterrupted;

        #region Unity Lifecycle

        private void Awake()
        {
            _owner = this.FindFirstParentComponent<MonsterCharacter>();
            _phases = GetComponentsInChildren<CharacterPhase>();
            Log.Info(LogTags.Pattern, "(System) PatternSystem 초기화 완료. Phase 개수: {0}", _phases?.Length ?? 0);
        }

        protected override void OnStart()
        {
            base.OnStart();

            SetPatternProbabilities();
        }

        private void OnDrawGizmos()
        {
            if (CurrentPatternStep == null)
            {
                return;
            }

            var sb = new StringBuilder();
            sb.Append("StepName: ").Append(CurrentPatternStep.StepName).Append("\n");
            sb.Append("OrderIndex: ").Append(CurrentPatternStep.OrderIndex).Append("\n");
            sb.Append("IsStartPattern: ").Append(IsStartPattern).Append("\n");
            GizmoEx.DrawText(sb.ToString(), transform.position);
        }

        #endregion Unity Lifecycle

        #region 초기화 및 패턴 로드

        private void SetPatternProbabilities()
        {
            if (_phases == null)
            {
                Log.Warning(LogTags.Pattern, "(System) Phases가 null입니다. 패턴 확률을 설정할 수 없습니다.");
                return;
            }

            int patternCount = 0;
            List<float> patternProbabilities = new();

            for (int i = 0; i < _phases.Length; i++)
            {
                CharacterPhase currentPhase = _phases[i];
                if (currentPhase == null)
                {
                    Log.Warning(LogTags.Pattern, "(System) Phase[{0}]가 null입니다.", i);
                    continue;
                }

                patternCount += currentPhase.PatternLength;
                for (int j = 0; j < currentPhase.PatternLength; j++)
                {
                    CharacterPattern currentPattern = currentPhase.Patterns[j];
                    if (currentPattern != null)
                    {
                        currentPattern.RefreshOrderMax();
                        patternProbabilities.Add(currentPattern.ProbabilityToPicked);
                    }
                }
            }

            _patternOrder.SetMax(patternCount);
            if (patternProbabilities.Count > 0 && _gacha != null)
            {
                _gacha.Clear();
                for (int i = 0; i < patternProbabilities.Count; i++)
                {
                    _gacha.Add(patternProbabilities[i], i);
                }
                Log.Info(LogTags.Pattern, "(System) 패턴 확률 설정 완료. 전체 패턴 개수: {0}", patternCount);
            }
            else
            {
                Log.Warning(LogTags.Pattern, "(System) 설정할 패턴이 없습니다. 가챠 값을 유지합니다.");
            }
        }

        private void LoadPatternProbabilityToPicked()
        {
            if (!_usablePatterns.IsValid() || _gacha == null)
            {
                return;
            }

            _gacha.Clear();
            for (int i = 0; i < _usablePatterns.Count; i++)
            {
                if (_usablePatterns[i] == null)
                {
                    continue;
                }

                float probability = _usablePatterns[i].ProbabilityToPicked;
                _gacha.Add(probability, i);
            }
        }

        public void LoadPatterns()
        {
            if (_phases == null)
            {
                Log.Warning(LogTags.Pattern, "(System) Phases가 null입니다. 패턴을 로드할 수 없습니다.");
                return;
            }

            if (_owner == null)
            {
                Log.Error(LogTags.Pattern, "(System) Owner가 null입니다.");
                return;
            }

            float healthRate = _owner.MyVital?.GetRate(VitalResourceTypes.Life) ?? -1f;
            Log.Info(LogTags.Pattern, "(System) 패턴 로드 시작. Owner 체력 비율: {0}", healthRate);

            int maxPhase = GetMaxPhase();
            if (_maxPhase >= maxPhase)
            {
                Log.Info(LogTags.Pattern, "(System) 이미 최대 페이즈({0})에 도달했습니다. 패턴 로드를 건너뜁니다.", _maxPhase);
                return;
            }

            Log.Info(LogTags.Pattern, "(System) 패턴 로드 시작. 이전 MaxPhase: {0}, 새 MaxPhase: {1}", _maxPhase, maxPhase);

            _maxPhase = maxPhase;

            for (int i = 0; i < _phases.Length; i++)
            {
                if (_phases[i] == null)
                {
                    continue;
                }

                _phases[i].SetMaxPhase(maxPhase);

                Log.Info(LogTags.Pattern, "(System) Phase[{0}]: IsLocked={1}, ConditionHealthRate={2}, PatternCount={3}",
                    i, _phases[i].IsLocked, _phases[i].ConditionHealthRate, _phases[i].PatternLength);

                if (_phases[i].TryLock())
                {
                    _phases[i].Lock();
                    RemoveUsablePattern(_phases[i].Patterns);
                }
                else if (_phases[i].TryUnlock(_owner))
                {
                    _phases[i].Unlock();
                    AddUsablePattern(_phases[i].Patterns);
                }
                else
                {
                    Log.Warning(LogTags.Pattern, "(System) Phase[{0}] 잠금/해제 조건을 만족하지 않음", i);
                }
            }

            LoadPatternProbabilityToPicked();
            Log.Info(LogTags.Pattern, "(System) 패턴 로드 완료. 사용 가능한 패턴 개수: {0}", _usablePatterns.Count);
        }

        private int GetMaxPhase()
        {
            if (_phases == null || _owner == null)
            {
                return 0;
            }

            int maxPhase = 0;
            for (int i = _phases.Length - 1; i >= 0; i--)
            {
                if (_phases[i] != null && _phases[i].CheckConditionHealthRate(_owner))
                {
                    if (maxPhase < i)
                    {
                        maxPhase = i;
                    }
                }
            }

            Log.Info(LogTags.Pattern, "(System) 최대 페이즈 계산 완료: {0}", maxPhase);
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
                    Log.Info(LogTags.Pattern, "(System) 사용 가능한 패턴을 추가합니다. Name: {0}", patterns[i].Name);
                }
            }

            if (_usablePatterns.Count > 0)
            {
                _patternOrder.SetMax(_usablePatterns.Count - 1);
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
                    Log.Info(LogTags.Pattern, "(System) 사용하지 않는 패턴을 삭제합니다. Name: {0}", patterns[i].Name);
                }
            }

            if (_usablePatterns.Count > 0)
            {
                _patternOrder.SetMax(_usablePatterns.Count - 1);
            }
        }

        #endregion 초기화 및 패턴 로드

        #region 패턴 선정 및 시작

        public void PickPattern()
        {
            if (CurrentPattern == null)
            {
                Log.Warning(LogTags.Pattern, "(System) 현재 패턴이 null입니다. 패턴을 선정할 수 없습니다.");
                return;
            }

            CurrentPattern.MoveToFirstStep();

            if (_gacha != null)
            {
                _gacha.Refresh();

                int cooldownCount = 0;
                for (int i = 0; i < _usablePatterns.Count; i++)
                {
                    if (_usablePatterns[i] != null && _usablePatterns[i].IsCooldown)
                    {
                        _gacha.LockAt(i);
                        cooldownCount++;
                    }
                }

                if (cooldownCount > 0)
                {
                    Log.Info(LogTags.Pattern, "(System) 쿨다운 중인 패턴 개수: {0}", cooldownCount);
                }

                if (!_gacha.CheckLockAll())
                {
                    int pickedOrder = _gacha.Pick();
                    _patternOrder.Set(pickedOrder);
                }
                else
                {
                    Log.Warning(LogTags.Pattern, "(System) 모든 패턴이 잠겨있습니다. 패턴을 선정할 수 없습니다.");
                }
            }
            else
            {
                Log.Info(LogTags.Pattern, "(System) 가챠가 null입니다. 순서대로 패턴을 선택합니다.");
                _patternOrder.Shuffle();
            }

            IsStartPattern = false;
        }

        public void StartPattern()
        {
            IsStartPattern = true;

            if (CurrentPattern != null)
            {
                Log.Info(LogTags.Pattern, "(System) 패턴을 시작합니다. Order: {0}, Name: {1}", _patternOrder.Current, CurrentPattern.Name);
            }
            else
            {
                Log.Warning(LogTags.Pattern, "(System) 패턴을 시작할 수 없습니다. Order: {0}", _patternOrder.Current);
            }

            ProcessStep();
        }

        #endregion 패턴 선정 및 시작

        #region 스텝 실행

        public void ProcessStep()
        {
            if (CurrentPatternStep == null)
            {
                return;
            }

            CurrentPatternStep.ProcessStep();
        }

        public void NextStep()
        {
            if (CurrentPatternStep == null)
            {
                return;
            }

            if (CurrentPatternStep.IsCompleteStepRepeat)
            {
                CurrentPattern.MoveToNextStep();
            }
        }

        public void ProcessNextStep()
        {
            NextStep();
            ProcessStep();
        }

        #endregion 스텝 실행

        #region 스텝 제어

        public void SkipToNextStep()
        {
            if (CurrentPatternStep == null)
            {
                return;
            }

            CurrentPatternStep.ResetCurrentRepeatCount();
            CurrentPattern.MoveToNextStep();
            ProcessStep();
        }

        public void InterruptCurrentPattern()
        {
            if (CurrentPattern == null)
            {
                return;
            }

            if (CurrentPatternStep != null && !CurrentPatternStep.CanInterruptStep)
            {
                Log.Info(LogTags.Pattern, "(System) 현재 스텝은 InterruptCurrentPattern으로 넘길 수 없습니다. Pattern: {0}, Step: {1}", CurrentPattern.Name, CurrentPatternStep.StepName);
                return;
            }

            SkipToNextStep();
            _isCurrentPatternInterrupted = true;
            Log.Info(LogTags.Pattern, "(System) 현재 패턴 스탭이 방해 넘어갑니다. Pattern: {0}", CurrentPattern.Name);
        }

        #endregion 스텝 제어

        #region 콜백

        public void OnAttackStateExited()
        {
            if (_isCurrentPatternInterrupted)
            {
                _isCurrentPatternInterrupted = false;
                return;
            }

            if (CurrentPatternStep == null)
            {
                return;
            }

            if (CurrentPatternStep.IsCompleteStepRepeat)
            {
                CurrentPattern.MoveToNextStep();
                ProcessStep();
            }
            else
            {
                CurrentPatternStep.ExecuteNextStep();  // AddRepeatCount + 다음 프레임에 ProcessNextStep 예약
            }
        }

        #endregion 콜백
    }
}
