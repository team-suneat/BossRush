using TeamSuneat.Data;

namespace TeamSuneat
{
    public partial class Vital : Entity
    {
        protected void LogProgress(string format)
        {
            if (Log.LevelProgress)
            {
                if (Owner != null)
                {
                    string addString = $"{Owner.Name.ToLogString()}({SID}), ";
                    Log.Progress(LogTags.Vital, addString + format);
                }
                else
                {
                    string addString = this.GetHierarchyPath() + ", ";
                    Log.Progress(LogTags.Vital, addString + format);
                }
            }
        }

        protected void LogInfo(string format)
        {
            if (Log.LevelInfo)
            {
                if (Owner != null)
                {
                    string addString = $"{Owner.Name.ToLogString()}({SID}), ";
                    Log.Info(LogTags.Vital, addString + format);
                }
                else
                {
                    string addString = this.GetHierarchyPath() + ", ";
                    Log.Info(LogTags.Vital, addString + format);
                }
            }
        }

        protected void LogWarning(string format)
        {
            if (Log.LevelWarning)
            {
                if (Owner != null)
                {
                    string addString = $"{Owner.Name.ToLogString()}({SID}), ";
                    Log.Warning(LogTags.Vital, addString + format);
                }
                else
                {
                    string addString = this.GetHierarchyPath() + ", ";
                    Log.Warning(LogTags.Vital, addString + format);
                }
            }
        }

        protected void LogError(string format)
        {
            if (Log.LevelError)
            {
                if (Owner != null)
                {
                    string addString = $"{Owner.Name.ToLogString()}({SID}), ";
                    Log.Error(addString + format);
                }
                else
                {
                    string addString = this.GetHierarchyPath() + ", ";
                    Log.Error(addString + format);
                }
            }
        }

        protected void LogProgress(string format, params object[] args)
        {
            if (Log.LevelProgress)
            {
                if (Owner != null)
                {
                    string addString = $"{Owner.Name.ToLogString()}({SID}), ";
                    Log.Progress(LogTags.Vital, addString + format, args);
                }
                else
                {
                    string addString = this.GetHierarchyPath() + ", ";
                    Log.Progress(LogTags.Vital, addString + format, args);
                }
            }
        }

        protected void LogInfo(string format, params object[] args)
        {
            if (Log.LevelInfo)
            {
                if (Owner != null)
                {
                    string addString = $"{Owner.Name.ToLogString()}({SID}), ";
                    Log.Info(LogTags.Vital, addString + format, args);
                }
                else
                {
                    string addString = this.GetHierarchyPath() + ", ";
                    Log.Info(LogTags.Vital, addString + format, args);
                }
            }
        }

        protected void LogWarning(string format, params object[] args)
        {
            if (Log.LevelWarning)
            {
                if (Owner != null)
                {
                    string addString = $"{Owner.Name.ToLogString()}({SID}), ";
                    Log.Warning(LogTags.Vital, addString + format, args);
                }
                else
                {
                    string addString = this.GetHierarchyPath() + ", ";
                    Log.Warning(LogTags.Vital, addString + format, args);
                }
            }
        }

        protected void LogError(string format, params object[] args)
        {
            if (Log.LevelError)
            {
                if (Owner != null)
                {
                    string addString = $"{Owner.Name.ToLogString()}({SID}), ";
                    Log.Error(addString + format, args);
                }
                else
                {
                    string addString = this.GetHierarchyPath() + ", ";
                    Log.Error(addString + format, args);
                }
            }
        }

        //────────────────────────────────────────────────────────────────────────────────────────────────

        private void LogCostCalculation(int baseCost, int fixedCostAdjustment, float multiplier, int finalCost)
        {
            if (Log.LevelProgress)
            {
                LogProgress("자원 소모량을 계산합니다. {0}, 계산식: [{1}(자원 소모량) + {2}(고정 자원 소모량)] * {3}(자원 소모 배율) ",
                    ValueStringEx.GetValueString(finalCost, baseCost),
                    baseCost,
                    ValueStringEx.GetValueString(fixedCostAdjustment, 0),
                    ValueStringEx.GetPercentString(multiplier, 1));
            }
        }

        private void LogDamageOnBuff(HitmarkNames hitmarkName, BuffNames buffName)
        {
            if (Log.LevelProgress)
            {
                LogProgress("캐릭터({0})가 공격({1})을 회피하면 적중시 버프 효과({2})도 함께 회피합니다.",
                   Owner.Name.ToLogString(), hitmarkName.ToLogString(), buffName.ToLogString());
            }
        }

        private void LogEvasionAttack(HitmarkNames hitmarkName, HitmarkNames hitmarkNameOnHit)
        {
            if (Log.LevelProgress)
            {
                LogProgress("캐릭터({0})가 공격({1})을 회피하면 적중시 히트마크({2})도 함께 회피합니다.",
                            Owner.Name.ToLogString(),
                            hitmarkName.ToLogString(),
                            hitmarkNameOnHit.ToLogString());
            }
        }

        private void LogCoreSkillResourceCostRate(float coreSkillResourceCostRate)
        {
            if (Log.LevelProgress)
            {
                LogProgress("핵심 기술 자원 소모량을 적용합니다. {0}", ValueStringEx.GetPercentString(coreSkillResourceCostRate, 0));
            }
        }

        private void LogErrorVitalLayer()
        {
            if (Log.LevelError)
            {
                LogError("바이탈 레이어가 설정되지 않았습니다. {0}", this.GetHierarchyPath());
            }
        }

        private void LogErrorUseShield()
        {
            if (Log.LevelError)
            {
                LogError("보호막을 소모하지 못합니다. 피해를 받지 못합니다.");
            }
        }

        private void LogErrorUseShield(VitalConsumeTypes vitalConsumeType, int value)
        {
            if (Log.LevelError)
            {
                LogError("전투 자원({0})을 소모할 수 없습니다. Value:{1}", vitalConsumeType, value);
            }
        }

        private void LogErrorUseBattleResource(HitmarkAssetData hitmarkAssetData, int value)
        {
            if (Log.LevelError)
            {
                if (Owner != null)
                {
                    LogError("이 공격({0})으로 캐릭터({1})의 전투 자원({2})을 소모할 수 없습니다. Value:{3}"
                        , hitmarkAssetData.Name.ToLogString(), Owner.Name, hitmarkAssetData.ResourceConsumeType, value);
                }
                else
                {
                    LogError("이 공격({0})으로 캐릭터의 전투 자원({1})을 소모할 수 없습니다. Value:{2}"
                        , hitmarkAssetData.Name.ToLogString(), hitmarkAssetData.ResourceConsumeType, value);
                }
            }
        }

        private void LogErrorAddResource(VitalConsumeTypes consumeType, int value)
        {
            if (Log.LevelError)
            {
                LogError("전투 자원({0})을 회복할 수 없습니다. Value:{1}", consumeType, value);
            }
        }

        private void LogErrorAddResource(VitalConsumeTypes consumeType, float rate)
        {
            if (Log.LevelError)
            {
                LogError("전투 자원({0})을 회복할 수 없습니다. Value Rate:{1}", consumeType, rate);
            }
        }

        private void LogErrorTakeDamageZero(HitmarkNames hitmarkName)
        {
            if (Log.LevelError)
            {
                LogError("{0}의 설정된 피해량이 0입니다. 피해를 받지 못합니다.", hitmarkName.ToLogString());
            }
        }

        private void LogErrorFindCurrentResource(VitalResourceTypes resourceType)
        {
            if (Log.LevelError)
            {
                LogError("Vital에서 전투 자원({0})의 현재 값을 찾을 수 없습니다. 경로: {1}", resourceType, this.GetHierarchyPath());
            }
        }

        private void LogErrorFindCurrentResource(VitalConsumeTypes consumeTypes)
        {
            if (Log.LevelError)
            {
                LogError("Vital에서 전투 자원({0})의 현재 값을 찾을 수 없습니다. 경로: {1}", consumeTypes, this.GetHierarchyPath());
            }
        }

        private void LogErrorFindMaxResource(VitalConsumeTypes consumeTypes)
        {
            if (Log.LevelError)
            {
                LogError("Vital에서 전투 자원({0})의 최대 값을 찾을 수 없습니다. 경로: {1}", consumeTypes, this.GetHierarchyPath());
            }
        }

        private void LogWarningFindResourceRate(VitalResourceTypes resourceType)
        {
            if (Log.LevelWarning)
            {
                LogWarning("Vital에서 전투 자원({0})의 비율을 찾을 수 없습니다. 경로: {1}", resourceType, this.GetHierarchyPath());
            }
        }

        private void LogWarningRegenerate()
        {
            if (Log.LevelWarning)
            {
                LogWarning("Vital에서 생명력/자원 재생 코루틴을 재생할 수 없습니다. 경로: {0}", this.GetHierarchyPath());
            }
        }

        private void LogProgressFailedToRegenerateByZeroPoint()
        {
            if (Log.LevelProgress)
            {
                LogProgress("재생력이 모두 0입니다. 생명력/마나 재생할 수 없습니다. {0}", this.GetHierarchyPath());
            }
        }

        private void LogProgressFailedToRegenerateByNotAlive()
        {
            if (Log.LevelProgress)
            {
                LogProgress("캐릭터의 현재 생명력이 0입니다. 생명력/마나 재생할 수 없습니다. {0}", this.GetHierarchyPath());
            }
        }

        private void LogInfoStartRegeneration()
        {
            if (Log.LevelInfo)
            {
                LogInfo("{0}, 생명력/마나 재생을 시작합니다.", Owner.Name.ToLogString());
            }
        }

        private void LogInfoHealthRegeneration()
        {
            if (Log.LevelInfo)
            {
                LogInfo("{0}, 생명력 재생력을 설정합니다. 재생량: {1}",
                    Owner.Name.ToLogString(),
                    ValueStringEx.GetValueString(HealthRegeneratePoint, true));
            }
        }
    }
}