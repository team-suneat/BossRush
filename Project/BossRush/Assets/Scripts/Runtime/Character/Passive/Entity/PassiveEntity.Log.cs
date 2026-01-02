using Lean.Pool;


namespace TeamSuneat.Passive
{
    public partial class PassiveEntity : XBehaviour, IPoolable
    {
        #region Log

        private string FormatEntityLog(string content)
        {
            if (Owner != null)
            {
                return string.Format("{0}:{1}({2}), {3}", Owner.Name.ToLogString(), Name.ToLogString(), Asset.Skill.ToLogString(), content);
            }
            else
            {
                return string.Format("{0}({1}) {2}", Name.ToLogString(), Asset.Skill.ToLogString(), content);
            }
        }

        protected virtual void LogProgress(string content)
        {
            if (Log.LevelProgress)
            {
                Log.Progress(LogTags.Passive, FormatEntityLog(content));
            }
        }

        protected virtual void LogProgress(string format, params object[] args)
        {
            if (Log.LevelProgress)
            {
                string formattedContent = FormatEntityLog(string.Format(format, args));
                Log.Progress(LogTags.Passive, formattedContent);
            }
        }

        protected virtual void LogInfo(string content)
        {
            if (Log.LevelInfo)
            {
                Log.Info(LogTags.Passive, FormatEntityLog(content));
            }
        }

        protected virtual void LogInfo(string format, params object[] args)
        {
            if (Log.LevelInfo)
            {
                string formattedContent = FormatEntityLog(string.Format(format, args));
                Log.Info(LogTags.Passive, formattedContent);
            }
        }

        protected virtual void LogWarning(string content)
        {
            if (Log.LevelWarning)
            {
                Log.Warning(LogTags.Passive, FormatEntityLog(content));
            }
        }

        protected virtual void LogWarning(string format, params object[] args)
        {
            if (Log.LevelWarning)
            {
                string formattedContent = FormatEntityLog(string.Format(format, args));
                Log.Warning(LogTags.Passive, formattedContent);
            }
        }

        protected virtual void LogError(string content)
        {
            if (Log.LevelError)
            {
                Log.Error(LogTags.Passive, FormatEntityLog(content));
            }
        }

        protected virtual void LogError(string format, params object[] args)
        {
            if (Log.LevelError)
            {
                string formattedContent = FormatEntityLog(string.Format(format, args));
                Log.Error(LogTags.Passive, formattedContent);
            }
        }

        #endregion Log

        //--------------------------------------------------------------------------------------

        private void LogSetLevel()
        {
            if (Log.LevelProgress)
            {
                LogProgress("패시브 레벨이 설정되었습니다. 현재 레벨: {0}", Level);
            }
        }

        private void LogActivate()
        {
            if (Log.LevelProgress)
            {
                LogProgress("패시브가 활성화되었습니다. 현재 레벨: {0}", Level);
            }
        }

        private void LogDeactivate()
        {
            if (Log.LevelProgress)
            {
                LogProgress("패시브가 비활성화되었습니다. 현재 레벨: {0}", Level);
            }
        }

        private void LogExecute(PassiveTrigger triggerInfo)
        {
            if (Log.LevelInfo)
            {
                if (triggerInfo.SkillName != SkillNames.None)
                {
                    LogInfo("패시브가 실행되었습니다. 현재 레벨: {0}, 트리거 메시지: {1}, 스킬 이름: {2}", Level, TriggerSettings.TriggerMessage, triggerInfo.SkillName.ToLogString());
                }
                else if (triggerInfo.BuffName != BuffNames.None)
                {
                    LogInfo("패시브가 실행되었습니다. 현재 레벨: {0}, 트리거 메시지: {1}, 버프 이름: {2}", Level, TriggerSettings.TriggerMessage, triggerInfo.BuffName.ToLogString());
                }
                else if (triggerInfo.HitmarkName != HitmarkNames.None)
                {
                    LogInfo("패시브가 실행되었습니다. 현재 레벨: {0}, 트리거 메시지: {1}, 히트마크: {2}", Level, TriggerSettings.TriggerMessage, triggerInfo.HitmarkName.ToLogString());
                }
                else
                {
                    LogInfo("패시브가 실행되었습니다. 현재 레벨: {0}, 트리거 메시지: {1}", Level, TriggerSettings.TriggerMessage);
                }
            }
        }

        private void LogAddBuff(BuffNames buffName, Character targetCharacter)
        {
            if (Log.LevelProgress)
            {
                LogProgress("패시브에 버프가 추가되었습니다: {0}, 타겟 캐릭터: {1}", buffName.ToLogString(), targetCharacter.GetHierarchyName());
            }
        }

        //
        private void LogNotValidAsset()
        {
            if (Log.LevelError)
            {
                LogError("패시브 엔티티의 데이터가 유효하지 않습니다. {0}", Name);
            }
        }

        private void LogFailedAddBuff(BuffNames[] buffNames)
        {
            if (Log.LevelError)
            {
                LogError("패시브 버프({0})를 추가할 수 없습니다. 패시브의 타겟 캐릭터가 존재하지 않습니다.", buffNames.ToLogString());
            }
        }

        private void LogFailedLoadForceVelocity()
        {
            if (Log.LevelWarning)
            {
                LogWarning("패시브 엔티티의 FV 데이터를 불러오는데 실패했습니다. {0}", EffectSettings.ForceVelocityName.ToLogString());
            }
        }

        private void LogFailedLoadHitmark()
        {
            if (Log.LevelWarning)
            {
                LogWarning("패시브 엔티티의 히트마크 데이터 로드에 실패했습니다. {0}", EffectSettings.Hitmarks.ToLogString());
            }
        }

        private void LogFailedLoadBuff()
        {
            if (Log.LevelWarning)
            {
                LogWarning("패시브 엔티티의 버프 데이터 로드에 실패했습니다. {0}", EffectSettings.Buffs.ToLogString());
            }
        }
    }
}