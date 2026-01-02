using System.Collections;
using Lean.Pool;
using TeamSuneat.Setting;

using UnityEngine;

namespace TeamSuneat
{
    public partial class SkillEntity : XBehaviour, IPoolable
    {
        private Coroutine _cooldownCoroutine;

        public bool IsCooldown { get; private set; }

        public float CooldownTime { get; private set; }

        /// <summary> 이 기술이 재사용 대기시간을 사용해야 하는지 확인합니다. </summary>
        /// <returns> 재사용 대기시간을 사용해야 하면 true, 아니면 false </returns>
        public bool ShouldUseCooldownTime()
        {
            if (_skillData.IsValid() && Owner != null)
            {
                switch (_skillData.Category)
                {
                    case SkillCategories.Dash:
                    case SkillCategories.Assistant:
                    case SkillCategories.Ultimate:
                        return true;

                    case SkillCategories.Power:
                        {
                            if (Owner.Stat.ContainsKey(StatNames.PowerSkillResourceCostPerCooldown))
                            {
                                // 숙련기술이 재사용 대기시간을 사용하지 않고, 재사용 대기시간에 비례한 자원을 소모합니다.
                                return false;
                            }
                            else if (_skillData.Name == SkillNames.WolfBrawl)
                            {
                                if (Owner.Stat.ContainsKey(StatNames.ResourceCostForWolfBrawl))
                                {
                                    // 늑대 난투 자원 소모량이 설정되어있다면, 자원을 소모합니다.
                                    return false;
                                }
                            }

                            return true;
                        }
                }
            }

            return false;
        }

        private void SetupCooldownTime()
        {
            if (Owner == null)
            {
                LogWarningOwnerNotSet();
                return;
            }
            if (!_skillData.IsValid() || _skillData.IsBlock)
            {
                LogWarningSkillDataInvalid();
                return;
            }

            CooldownTime = _skillHandler.GetCooldownTime(_skillData, Level);
            LogInfoCooldownTimeSet();
        }

        public bool CheckCooldowning()
        {
            return CooldownTime > 0 && _cooldownCoroutine != null;
        }

        public void StartCooldown()
        {
            if (ShouldIgnoreCooldown())
            {
                LogInfoIgnoreCooldownTime();
                return;
            }

            if (!IsCooldownStartable())
            {
                return;
            }

            BeginCooldown();
        }

        private bool ShouldIgnoreCooldown()
        {
            if (!Owner.IsPlayer)
            {
                return false;
            }

            if (GameSetting.Instance.Cheat.NoCooldownTime)
            {
                return true;
            }

            if (Owner.Stat.FindValueOrDefault(StatNames.IgnoreCooldownTime) > 0)
            {
                return true;
            }

            if (_skillData.Category == SkillCategories.Power)
            {
                if (Owner.Stat.FindValueOrDefault(StatNames.IgnoreCooldownTimeForPowerSkill) > 0)
                {
                    return true;
                }

                if (_skillData.Name == SkillNames.WolfBrawl &&
                    Owner.Stat.FindValueOrDefault(StatNames.IgnoreCooldownForWolfBrawl) > 0)
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsCooldownStartable()
        {
            if (CooldownTime <= 0f)
            {
                LogInfoCooldownTimeNotSet();
                return false;
            }

            if (_skillMaxCount < 0)
            {
                LogInfoSkillMaxCountNotSet();
                return false;
            }

            return _cooldownCoroutine == null;
        }

        private void BeginCooldown()
        {
            LogInfoStartCooldown();
            IsCooldown = true;
            StartCooldownOfHUD();
            _cooldownCoroutine = StartXCoroutine(ProcessCooldown());
            _ = GlobalEvent<SkillNames>.Send(GlobalEventType.PLAYER_CHARACTER_START_SKILL_COOLDOWN, Name);
        }

        private void StopCooldown()
        {
            IsCooldown = false;

            ResetElapsedTimeOnCast();
            ResetCooldownOfHUD();
            StopCooldownOfHUD();
            RefreshAllowOfHUD();

            LogInfoStopCooldown();

            StopXCoroutine(ref _cooldownCoroutine);

            _ = GlobalEvent<SkillNames>.Send(GlobalEventType.PLAYER_CHARACTER_STOP_SKILL_COOLDOWN, Name);
        }

        private IEnumerator ProcessCooldown()
        {
            float lastTime;
            float cooldownRate;

            while (true)
            {
                ElapsedTimeOnCast += Time.deltaTime;

                cooldownRate = ElapsedTimeOnCast.SafeDivide(CooldownTime);
                lastTime = CooldownTime - ElapsedTimeOnCast;

                if (lastTime <= 0)
                {
                    AddSkillCount();

                    if (_skillCount < _skillMaxCount)
                    {
                        ResetElapsedTimeOnCast();
                    }
                    else
                    {
                        Order skillOrder = GetCurrentOrder();
                        if (skillOrder != null)
                        {
                            SetSkillOrderToFirst(skillOrder);
                        }

                        StopCooldown();
                        yield break;
                    }
                }
                else
                {
                    SetCooldownOfHUD(cooldownRate, CooldownTime - ElapsedTimeOnCast);
                }

                yield return null;
            }
        }

        private bool DetermineReduceCooldown()
        {
            if (_cooldownCoroutine != null ||
                (_skillOrderOnGround != null && _skillOrderOnGround.DropCoroutine != null) ||
                (_skillOrderInAir != null && _skillOrderInAir.DropCoroutine != null))
            {
                return true;
            }

            LogWarningNoCooldownOrResetOrder();
            return false;
        }

        private void SetCastTime()
        {
            CastTime = Time.time;
            LogInfoCastTimeSet();
        }

        public void ReduceCooldownTime(float addElapsedTime)
        {
            if (DetermineReduceCooldown())
            {
                ElapsedTimeOnCast += addElapsedTime;
                LogInfoReduceCooldownTime(addElapsedTime);
            }
        }

        public void ReduceCooldownTimeRate(float addElapsedTimeRate)
        {
            if (DetermineReduceCooldown())
            {
                float reduceCooldownTime = CooldownTime * addElapsedTimeRate;
                ElapsedTimeOnCast += reduceCooldownTime;
                LogInfoReduceCooldownTimeRate(reduceCooldownTime);
            }
        }

        private void ResetElapsedTimeOnCast()
        {
            LogInfoResetElapsedTimeOnCast();
            ElapsedTimeOnCast = 0f;
        }

        #region Log

        private void LogWarningOwnerNotSet()
        {
            if (Log.LevelWarning)
            {
                Log.Warning(LogTags.Skill_Cooldown, "[Entity] {0}의 소유 캐릭터가 설정되지 않아 재사용 대기시간을 설정할 수 없습니다.", Name.ToLogString());
            }
        }

        private void LogWarningSkillDataInvalid()
        {
            if (Log.LevelWarning)
            {
                Log.Warning(LogTags.Skill_Cooldown, "[Entity] {0}의 정보가 올바르지 않아 재사용 대기시간을 설정할 수 없습니다.", Name.ToLogString());
            }
        }

        private void LogInfoCooldownTimeSet()
        {
            if (Log.LevelInfo)
            {
                Log.Info(LogTags.Skill_Cooldown, "[Entity] {0}의 레벨({1})에 따라 재사용 대기시간({2})을 설정합니다.", Name.ToLogString(), Level, CooldownTime.ToSelectString(0));
            }
        }

        private void LogInfoIgnoreCooldownTime()
        {
            if (Log.LevelInfo)
            {
                Log.Info(LogTags.Skill_Cooldown, "[Entity] {0}의 재사용 대기를 무시하는 능력치를 가집니다. 재사용 대기를 사용하지 않습니다.", Name.ToLogString());
            }
        }

        private void LogInfoCooldownTimeNotSet()
        {
            if (Log.LevelInfo)
            {
                Log.Info(LogTags.Skill_Cooldown, "[Entity] {0}의 재사용 대기시간이 설정되어있지 않습니다. 재사용 대기를 사용하지 않습니다.", Name.ToLogString());
            }
        }

        private void LogInfoSkillMaxCountNotSet()
        {
            if (Log.LevelInfo)
            {
                Log.Info(LogTags.Skill_Cooldown, "[Entity] {0}의 최대 횟수가 설정되어있지 않습니다. 재사용 대기를 사용하지 않습니다.", Name.ToLogString());
            }
        }

        private void LogInfoStartCooldown()
        {
            if (Log.LevelInfo)
            {
                Log.Info(LogTags.Skill_Cooldown, "[Entity] {0}의 재사용 대기를 시작합니다. 재사용 대기시간: {1}/{2}", Name.ToLogString(), ElapsedTimeOnCast, CooldownTime);
            }
        }

        private void LogInfoStopCooldown()
        {
            if (Log.LevelInfo)
            {
                Log.Info(LogTags.Skill_Cooldown, "[Entity] {0}의 재사용 대기를 종료합니다.", Name.ToLogString());
            }
        }

        private void LogWarningNoCooldownOrResetOrder()
        {
            if (Log.LevelWarning)
            {
                Log.Warning(LogTags.Skill_Cooldown, "[Entity] {0}이 재사용 대기 또는 순서 초기화 대기를 사용하지 않습니다.", Name.ToLogString());
            }
        }

        private void LogInfoCastTimeSet()
        {
            if (Log.LevelInfo)
            {
                Log.Info(LogTags.Skill_Cooldown, "[Entity] {0}의 시전 시간({1})을 등록합니다.", Name.ToLogString(), CastTime);
            }
        }

        private void LogInfoReduceCooldownTime(float addElapsedTime)
        {
            if (Log.LevelInfo)
            {
                Log.Info(LogTags.Skill_Cooldown, "[Entity] {0} 시전 후 재사용 대기 시간을 일정량({1}) 감소시킵니다. 재사용 대기시간:{2}/{3}", Name.ToLogString(), ValueStringEx.GetValueString(-addElapsedTime, true), ElapsedTimeOnCast, CooldownTime);
            }
        }

        private void LogInfoReduceCooldownTimeRate(float reduceCooldownTime)
        {
            if (Log.LevelInfo)
            {
                Log.Info(LogTags.Skill_Cooldown, "[Entity] {0} 시전 후 재사용 대기 시간을 일정량({1}) 감소시킵니다. 재사용 대기시간:{2}/{3}", Name.ToLogString(), ValueStringEx.GetValueString(-reduceCooldownTime, true), ElapsedTimeOnCast, CooldownTime);
            }
        }

        private void LogInfoResetElapsedTimeOnCast()
        {
            if (Log.LevelInfo)
            {
                Log.Info(LogTags.Skill_Cooldown, "[Entity] {0} 시전 후 지난 시간({1})을 초기화합니다.", Name.ToLogString(), ElapsedTimeOnCast);
            }
        }

        #endregion Log
    }
}