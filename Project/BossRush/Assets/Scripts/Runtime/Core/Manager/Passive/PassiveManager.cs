using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TeamSuneat.Passive
{
    public class PassiveManager : Singleton<PassiveManager>
    {
        private class PassiveExecution
        {
            public float ExecuteTime;
            public PassiveTrigger TriggerInfo;
            public PassiveEntity Entity;

            public void Execute()
            {
                Entity.ExecuteExtern(TriggerInfo);
            }
        }

        private List<PassiveExecution> _passiveExecutions = new();
        private Dictionary<PassiveNames, int> _passiveExecuteCounts = new();
        private int _passiveExecuteCountPerFrame;

        private const int PASSIVE_EXECUTE_MAX_COUNT = 100;

        public void LateLogicUpdate()
        {
            if (IsPassiveExecutionsEmpty())
            {
                return;
            }

            InitializeFrameExecution();
            ProcessPassiveExecutions();
            LogFrameExecutionSummary();
        }

        #region LogicalUpdate Methods

        /// <summary>
        /// 패시브 실행 목록이 비어있는지 확인합니다.
        /// </summary>
        /// <returns>실행 목록이 비어있으면 true</returns>
        private bool IsPassiveExecutionsEmpty()
        {
            return _passiveExecutions == null || _passiveExecutions.Count == 0;
        }

        /// <summary>
        /// 프레임 실행을 초기화합니다.
        /// </summary>
        private void InitializeFrameExecution()
        {
            _passiveExecuteCountPerFrame = 0;
        }

        /// <summary>
        /// 패시브 실행들을 처리합니다.
        /// </summary>
        private void ProcessPassiveExecutions()
        {
            for (int i = _passiveExecutions.Count - 1; i >= 0; i--)
            {
                if (_passiveExecutions.Count <= i)
                {
                    continue;
                }

                PassiveExecution passiveExecution = _passiveExecutions[i];
                if (ShouldExecutePassive(passiveExecution))
                {
                    ExecutePassive(passiveExecution, i);
                }
                else if (ShouldRemovePassive(passiveExecution))
                {
                    RemoveInvalidPassive(passiveExecution, i);
                }
            }
        }

        /// <summary>
        /// 패시브를 실행해야 하는지 확인합니다.
        /// </summary>
        /// <param name="passiveExecution">패시브 실행 정보</param>
        /// <returns>실행해야 하면 true</returns>
        private bool ShouldExecutePassive(PassiveExecution passiveExecution)
        {
            return passiveExecution.ExecuteTime < Time.time;
        }

        /// <summary>
        /// 패시브를 제거해야 하는지 확인합니다.
        /// </summary>
        /// <param name="passiveExecution">패시브 실행 정보</param>
        /// <returns>제거해야 하면 true</returns>
        private bool ShouldRemovePassive(PassiveExecution passiveExecution)
        {
            return passiveExecution.Entity == null ||
                   passiveExecution.TriggerInfo.Name != passiveExecution.Entity.Name;
        }

        /// <summary>
        /// 패시브를 실행합니다.
        /// </summary>
        /// <param name="passiveExecution">패시브 실행 정보</param>
        /// <param name="index">실행 목록 인덱스</param>
        private void ExecutePassive(PassiveExecution passiveExecution, int index)
        {
            passiveExecution.Execute();
            _passiveExecutions.Remove(passiveExecution);

            RegisterExecuteCount(passiveExecution);

            _passiveExecuteCountPerFrame += 1;

            if (_passiveExecuteCountPerFrame >= PASSIVE_EXECUTE_MAX_COUNT)
            {
                return; // 최대 실행 횟수 도달 시 루프 종료
            }
        }

        /// <summary>
        /// 유효하지 않은 패시브를 제거합니다.
        /// </summary>
        /// <param name="passiveExecution">패시브 실행 정보</param>
        /// <param name="index">실행 목록 인덱스</param>
        private void RemoveInvalidPassive(PassiveExecution passiveExecution, int index)
        {
            if (passiveExecution.Entity == null)
            {
                Log.Warning(LogTags.Passive, "[Manager] 패시브 독립체가 null이므로 실행을 제거합니다: {0}",
                    passiveExecution.TriggerInfo.Name.ToLogString());
            }
            else if (passiveExecution.TriggerInfo.Name != passiveExecution.Entity.Name)
            {
                Log.Warning(LogTags.Passive, "[Manager] 패시브 이름이 불일치하므로 실행을 제거합니다. 예상: {0}, 실제: {1}",
                    passiveExecution.TriggerInfo.Name.ToLogString(),
                    passiveExecution.Entity.Name.ToLogString());
            }

            _passiveExecutions.RemoveAt(index);
        }

        /// <summary>
        /// 프레임 실행 결과를 로깅합니다.
        /// </summary>
        private void LogFrameExecutionSummary()
        {
            if (_passiveExecuteCountPerFrame > 0)
            {
                Log.Info(LogTags.Passive, "[Manager] 프레임별 패시브 실행 횟수: {0}, 남은 패시브 실행 수: {1}",
                    _passiveExecuteCountPerFrame.ToSelectString(),
                    _passiveExecutions.Count.ToSelectString(0));
            }
        }

        #endregion LogicalUpdate Methods

        public void RegisterExecute(float delayTime, PassiveTrigger triggerInfo, PassiveEntity passiveEntity)
        {
            _passiveExecutions.Add(new PassiveExecution()
            {
                ExecuteTime = Time.time + delayTime,
                TriggerInfo = triggerInfo,
                Entity = passiveEntity,
            });

            Log.Info(LogTags.Passive, "[Manager] 프레임별 실행할 패시브를 등록합니다: {0}, 지연시간: {1}초",
                    triggerInfo.Name.ToLogString(),
                    delayTime.ToSelectString(0));
        }

        private void RegisterExecuteCount(PassiveExecution passiveExecution)
        {
#if UNITY_EDITOR
            PassiveNames passiveName = passiveExecution.Entity.Name;
            if (_passiveExecuteCounts.ContainsKey(passiveName))
            {
                _passiveExecuteCounts[passiveName] += 1;
            }
            else
            {
                _passiveExecuteCounts.Add(passiveName, 1);
            }
#endif
        }

        public void LogExecuteCount()
        {
            if (_passiveExecuteCounts.IsValid())
            {
                IOrderedEnumerable<KeyValuePair<PassiveNames, int>> sorted = _passiveExecuteCounts.OrderByDescending(x => x.Value);
                StringBuilder stringBuilder = new();
                stringBuilder.AppendLine("[Manager] <b>패시브 실행 횟수:</b>");
                foreach (KeyValuePair<PassiveNames, int> item in sorted)
                {
                    SkillNames skillName = EnumEx.ConvertTo<SkillNames>(item.Key.ToString());
                    if (skillName != SkillNames.None)
                    {
                        stringBuilder.AppendLine($"{item.Key}({skillName.ToLogString()}) : {item.Value.ToSelectString()}");
                    }
                    else
                    {
                        stringBuilder.AppendLine($"{item.Key} : {item.Value.ToSelectString()}");
                    }
                }

                Log.Info(stringBuilder.ToString());
            }
        }

        public void ResetExeucteCount()
        {
            if (_passiveExecutions.IsValid())
            {
                _passiveExecuteCounts.Clear();
            }
        }

        public void RemoveExecutionsByEntity(PassiveEntity entity)
        {
            if (_passiveExecutions.IsValid())
            {
                _passiveExecutions.RemoveAll(exec => exec.Entity == entity);
            }
        }
    }
}