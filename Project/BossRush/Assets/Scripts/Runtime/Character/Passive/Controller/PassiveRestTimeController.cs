using System.Collections.Generic;
using TeamSuneat.Data;
using TeamSuneat.UserInterface;

using UnityEngine;

namespace TeamSuneat.Passive
{
    /// <summary>
    /// 패시브의 RestTime을 통합 관리하는 컨트롤러
    /// 캐릭터 레벨에서 패시브별 RestTime 상태를 관리하여 장비 착용/해제와 무관하게 일관된 RestTime을 보장합니다.
    /// </summary>
    public class PassiveRestTimeController
    {
        /// <summary>
        /// 패시브별 RestTime 종료 시점을 저장하는 Dictionary
        /// </summary>
        private Dictionary<PassiveNames, float> _restTimeEndTimes = new();

        /// <summary>
        /// 패시브별 RestTime 지속시간을 저장하는 Dictionary (디버깅용)
        /// </summary>
        private Dictionary<PassiveNames, float> _restTimeDurations = new();

        /// <summary>
        /// 다음 RestTime 체크 시점 (최적화용)
        /// </summary>
        private float _nextCheckTime = float.MaxValue;

        /// <summary>
        /// 소유자 캐릭터 (로깅용)
        /// </summary>
        private Character _owner;

        public PassiveRestTimeController(Character owner)
        {
            _owner = owner;
        }

        /// <summary>
        /// 패시브의 RestTime을 시작합니다.
        /// </summary>
        /// <param name="passiveName">패시브 이름</param>
        /// <param name="duration">지속시간 (초)</param>
        public void StartRestTime(PassiveNames passiveName, float duration)
        {
            if (duration <= 0f)
            {
                return;
            }

            // 기존 RestTime 정보 정리
            StopRestTime(passiveName);

            // 새로운 RestTime 정보 설정
            float endTime = Time.time + duration;
            _restTimeEndTimes[passiveName] = endTime;
            _restTimeDurations[passiveName] = duration;

            // 다음 체크 시점 업데이트 (최적화)
            if (endTime < _nextCheckTime)
            {
                _nextCheckTime = endTime;
            }

            Log.Info(LogTags.Passive, "[RestTime] {0}의 {1} 패시브 RestTime 시작: {2}초", _owner.Name.ToLogString(), passiveName.ToLogString(), duration);
        }

        /// <summary>
        /// 패시브의 RestTime을 강제로 종료합니다.
        /// </summary>
        /// <param name="passiveName">패시브 이름</param>
        public void StopRestTime(PassiveNames passiveName)
        {
            if (_restTimeEndTimes.ContainsKey(passiveName))
            {
                _restTimeEndTimes.Remove(passiveName);
                _restTimeDurations.Remove(passiveName);

                // 다음 체크 시점 재계산 (최적화)
                RecalculateNextCheckTime();

                Log.Info(LogTags.Passive, "[RestTime] {0}의 {1} 패시브 RestTime 종료", _owner.Name.ToLogString(), passiveName.ToLogString());
            }
        }

        /// <summary>
        /// 패시브가 현재 RestTime 상태인지 확인합니다.
        /// </summary>
        /// <param name="passiveName">패시브 이름</param>
        /// <returns>RestTime 상태 여부</returns>
        public bool IsResting(PassiveNames passiveName)
        {
            if (_restTimeEndTimes.TryGetValue(passiveName, out float endTime))
            {
                return Time.time < endTime;
            }

            return false;
        }

        /// <summary>
        /// 패시브의 남은 RestTime을 조회합니다.
        /// </summary>
        /// <param name="passiveName">패시브 이름</param>
        /// <returns>남은 시간 (초), RestTime이 없으면 0</returns>
        public float GetRemainingTime(PassiveNames passiveName)
        {
            if (_restTimeEndTimes.TryGetValue(passiveName, out float endTime))
            {
                return Mathf.Max(0f, endTime - Time.time);
            }

            return 0f;
        }

        /// <summary>
        /// 만료된 RestTime을 정리합니다.
        /// 최적화된 방식으로 매 프레임 호출되어야 합니다.
        /// </summary>
        public void UpdateRestTimes()
        {
            // 다음 체크 시점이 되지 않았으면 스킵 (최적화)
            if (Time.time < _nextCheckTime)
            {
                return;
            }

            List<PassiveNames> expiredPassives = new();
            float newNextCheckTime = float.MaxValue;

            foreach (KeyValuePair<PassiveNames, float> kvp in _restTimeEndTimes)
            {
                if (Time.time >= kvp.Value)
                {
                    expiredPassives.Add(kvp.Key);
                }
                else
                {
                    // 아직 만료되지 않은 패시브 중 가장 빠른 만료 시점 기록
                    newNextCheckTime = Mathf.Min(newNextCheckTime, kvp.Value);
                }
            }

            // 만료된 패시브 정리
            for (int i = 0; i < expiredPassives.Count; i++)
            {
                PassiveNames passiveName = expiredPassives[i];
                StopRestTime(passiveName);
                OnCompleteRestTime(passiveName);
            }

            _nextCheckTime = newNextCheckTime;
        }

        /// <summary>
        /// 다음 체크 시점을 재계산합니다.
        /// </summary>
        private void RecalculateNextCheckTime()
        {
            _nextCheckTime = float.MaxValue;

            foreach (KeyValuePair<PassiveNames, float> kvp in _restTimeEndTimes)
            {
                _nextCheckTime = Mathf.Min(_nextCheckTime, kvp.Value);
            }
        }

        private void OnCompleteRestTime(PassiveNames passiveName)
        {
            if (ScriptableDataManager.Instance == null) return;

            PassiveAsset passiveAsset = ScriptableDataManager.Instance.FindPassive(passiveName);
            if (!passiveAsset.IsValid()) return;
            if (passiveAsset.EffectSettings == null) return;
            if (!passiveAsset.EffectSettings.UseSoliloquy) return;
            if (!passiveAsset.EffectSettings.IsSoliloquyOnRestCompleted) return;

            string[] values = new string[]
            {
                passiveAsset.EffectSettings.SoliloquySkillName.GetLocalizedString(),
                passiveAsset.EffectSettings.RestTime.ToString()
            };

            UIManager.Instance.SpawnSoliloquyIngame(SoliloquyTypes.PassiveCooldownComplete, values);
        }

        /// <summary>
        /// 모든 패시브의 RestTime 정보를 제거합니다.
        /// </summary>
        public void ClearAllRestTimes()
        {
            _restTimeEndTimes.Clear();
            _restTimeDurations.Clear();
            _nextCheckTime = float.MaxValue;

            Log.Info(LogTags.Passive, "[RestTime] {0}의 모든 패시브 RestTime 정보 제거", _owner.Name.ToLogString());
        }

        /// <summary>
        /// 현재 RestTime 상태인 모든 패시브 목록을 반환합니다.
        /// </summary>
        /// <returns>RestTime 상태인 패시브 목록</returns>
        public List<PassiveNames> GetRestingPassives()
        {
            List<PassiveNames> restingPassives = new();

            foreach (KeyValuePair<PassiveNames, float> kvp in _restTimeEndTimes)
            {
                if (Time.time < kvp.Value)
                {
                    restingPassives.Add(kvp.Key);
                }
            }

            return restingPassives;
        }

        #region Debug

        /// <summary>
        /// 현재 RestTime 상태를 로그로 출력합니다. (디버그용)
        /// </summary>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public void LogRestTimeStatus()
        {
            if (_restTimeEndTimes.Count == 0)
            {
                Log.Info(LogTags.Passive, "[RestTime] {0}의 RestTime 정보가 없습니다.", _owner.Name.ToLogString());
                return;
            }

            Log.Info(LogTags.Passive, "[RestTime] {0}의 RestTime 상태:", _owner.Name.ToLogString());
            foreach (KeyValuePair<PassiveNames, float> kvp in _restTimeEndTimes)
            {
                bool isResting = Time.time < kvp.Value;
                float remainingTime = GetRemainingTime(kvp.Key);

                Log.Info(LogTags.Passive, "  - {0}: {1} (남은시간: {2:F1}초)",
                    kvp.Key.ToLogString(),
                    isResting ? "유휴중" : "대기중",
                    remainingTime);
            }
        }

        #endregion Debug
    }
}