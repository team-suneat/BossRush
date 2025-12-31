using Sirenix.OdinInspector;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace TeamSuneat.Data
{
    [CreateAssetMenu(fileName = "Effect", menuName = "TeamSuneat/Scriptable/Passive Setting/EffectSettings")]
    public class PassiveEffectSettings : XScriptableObject
    {
        public bool IsChangingAsset;

        [FoldoutGroup("패시브 적용")]
        public PassiveNames Name;

        #region 적용 대상

        [FoldoutGroup("대상")]
        [GUIColor("GetPassiveTargetTypeColor")]
        [EnableIf("IsChangingAsset")]
        [SuffixLabel("패시브 적용 타겟 설정")]
        public PassiveTargetTypes ApplyTarget;

        [FoldoutGroup("대상")]
        [GUIColor("GetBoolColor")]
        [SuffixLabel("자기 자신도 패시브 적용")]
        public bool UseApplySelf;

        #endregion 적용 대상

        #region 적용 횟수

        [FoldoutGroup("횟수")]
        [GUIColor("GetBoolColor")]
        [SuffixLabel("발동 여부와는 관계없이 발동 여부를 저장합니다")]
        public bool StoreTriggerAttempt;

        [FoldoutGroup("횟수")]
        [GUIColor("GetBoolColor")]
        [SuffixLabel("일회성 패시브 적용")]
        public bool ApplyOneTime;

        [FoldoutGroup("횟수")]
        [GUIColor("GetBoolColor")]
        [SuffixLabel("아이템별 일회성 패시브 적용")]
        public bool ApplyOneTimePerItem;

        [FoldoutGroup("횟수")]
        [GUIColor("GetIntColor")]
        [Tooltip("패시브 적용 횟수가 최대에 도달하면 초기화 시간동안 패시브가 발동하지 않습니다.")]
        [SuffixLabel("패시브 적용 최대 횟수*")]
        [DisableIf("ApplyOneTime")]
        public int ApplyMaxCount;

        [FoldoutGroup("횟수")]
        [GUIColor("GetFloatColor")]
        [Tooltip("패시브 적용 횟수가 최대에 도달하면 초기화 시간동안 패시브가 발동하지 않습니다.")]
        [SuffixLabel("패시브 적용 최대 횟수 초기화 시간*")]
        [DisableIf("ApplyOneTime")]
        public float ResetTimeApplyMaxCount;

        [FoldoutGroup("횟수")]
        [GUIColor("GetStateEffectColor")]
        [EnableIf("IsChangingAsset")]
        [SuffixLabel("지속 피해 한 번에 적용")]
        public StateEffects ApplyDamageOverTimeAtOnce;

        #endregion 적용 횟수

        #region 적용 시간

        [FoldoutGroup("시간")]
        [GUIColor("GetFloatColor")]
        [Tooltip("패시브가 지연시간 이후 적용됩니다.")]
        [SuffixLabel("적용 지연시간*")]
        public float ApplyDelayTime;

        #endregion 적용 시간

        #region 적용 버프

        [FoldoutGroup("버프")]
        [GUIColor("GetBoolColor")]
        [Tooltip("선택된 버프 중에서 무작위로 1개를 뽑아 적용")]
        [SuffixLabel("무작위 여부*")]
        public bool RendomApplyBuffs;

        [FoldoutGroup("버프")]
        [EnableIf("IsChangingAsset")]
        [GUIColor("GetBuffsColor")]
        [SuffixLabel("적용되는 버프 이름")]
        public BuffNames[] Buffs;

        [FoldoutGroup("버프")]
        [GUIColor("GetReleaseBuffsColor")]
        [EnableIf("IsChangingAsset")]
        [SuffixLabel("적용시 해제 버프")]
        public BuffNames[] ReleaseBuffs;

        [FoldoutGroup("버프")]
        [GUIColor("GetReleaseStateEffectsColor")]
        [EnableIf("IsChangingAsset")]
        [SuffixLabel("적용시 해제 상태이상")]
        public StateEffects[] ReleaseStateEffects;

        [FoldoutGroup("버프")]
        [GUIColor("GetBuffReleaseApplicationsColor")]
        [EnableIf("IsChangingAsset")]
        [SuffixLabel("적용시 해제 방식")]
        public BuffReleaseApplications ReleaseBuffApplication;

        [FoldoutGroup("버프")]
        [SuffixLabel("적용시 스택을 불러와 설정")]
        public bool IsSetStackByLoad;

        #endregion 적용 버프

        #region 적용 히트마크

        [FoldoutGroup("히트마크")]
        [GUIColor("GetHitmarksColor")]
        [EnableIf("IsChangingAsset")]
        [SuffixLabel("적용되는 히트마크")]
        public HitmarkNames[] Hitmarks;

        [FoldoutGroup("히트마크")]
        [GUIColor("GetBoolColor")]
        [Tooltip("히트마크 목록 중 무작위로 하나를 골라 적용")]
        [SuffixLabel("히트마크 무작위 적용 여부")]
        public bool UseRandomHitmark;

        [FoldoutGroup("히트마크")]
        [GUIColor("GetBoolColor")]
        [Tooltip("패시브의 공격 독립체 위치를 패시브를 발동시킨 공격 독립체의 위치로 설정")]
        [SuffixLabel("패시브의 트리거 위치에 따라 공격 위치 설정*")]
        public bool SetPositionToAttackPosition;

        [FoldoutGroup("히트마크")]
        [GUIColor("GetBoolColor")]
        [Tooltip("패시브의 공격 독립체 위치를 타겟의 위치로 설정")]
        [SuffixLabel("패시브의 타겟 위치에 따라 공격 위치 설정*")]
        public bool SetPositionToTargetPosition;

        #endregion 적용 히트마크

        #region 물약

        [FoldoutGroup("물약")]
        [EnableIf("IsChangingAsset")]
        [GUIColor("GetBoolColor")]
        [SuffixLabel("적용 물약 사용")]
        public bool IsAutoUsePotion;

        [FoldoutGroup("물약")]
        [EnableIf("IsChangingAsset")]
        [GUIColor("GetBoolColor")]
        [SuffixLabel("물약 사용 실패시 적용 안함")]
        public bool IgnoreEffectOnPotionFail;

        #endregion 물약

        #region 지속 시간

        [FoldoutGroup("지속 시간")]
        [GUIColor("GetStateEffectColor")]
        [EnableIf("IsChangingAsset")]
        [SuffixLabel("변동 지속시간 상태이상")]
        public StateEffects DurationStateEffect;

        [FoldoutGroup("지속 시간")]
        [GUIColor("GetFloatColor")]
        [SuffixLabel("추가 지속시간")]
        public float AddDuration;

        [FoldoutGroup("지속 시간")]
        [GUIColor("GetFloatColor")]
        [SuffixLabel("추가 지속 시간으로 추가할 수 있는 최대 지속 시간")]
        public float AddMaxDuration;

        //

        [FoldoutGroup("지속 시간")]
        [GUIColor("GetFloatColor")]
        [SuffixLabel("범위 내 최소 지속 시간")]
        public float MinAddDuration;

        [FoldoutGroup("지속 시간")]
        [GUIColor("GetFloatColor")]
        [SuffixLabel("범위 내 최대 추가 지속시간")]
        public float MaxAddDuration;

        [FoldoutGroup("지속 시간")]
        [GUIColor("GetFloatColor")]
        [SuffixLabel("성장 추가 지속시간")]
        public float GrowAddDuration;

        #endregion 지속 시간

        #region 보상

        [FoldoutGroup("보상")]
        [EnableIf("IsChangingAsset")]
        [SuffixLabel("보상 조건 연산자")]
        public ConditionalOperators RewardOperator;

        [FoldoutGroup("보상")]
        [EnableIf("IsChangingAsset")]
        [GUIColor("GetCurrencyRewardsColor")]
        [SuffixLabel("재화 보상")]
        public PassiveRewardCurrency[] CurrencyRewards;

        [FoldoutGroup("보상")]
        [GUIColor("GetIntColor")]
        [SuffixLabel("레벨 보상")]
        public int RewardLevel;

        [FoldoutGroup("보상")]
        [GUIColor("GetIntColor")]
        [SuffixLabel("경험치 보상")]
        public int RewardExperience;

        #endregion 보상

        #region 안내

        [FoldoutGroup("안내")]
        [GUIColor("GetBoolColor")]
        public bool UseSoliloquy;

        #endregion 안내

        #region 유휴

        [FoldoutGroup("유휴")]
        [GUIColor("GetFloatColor")]
        [SuffixLabel("유휴 시간동안 패시브 미발동")]
        public float RestTime;

        [FoldoutGroup("유휴")]
        [GUIColor("GetFloatColor")]
        [SuffixLabel("패시브 레벨 증가 시 유휴 시간이 증가")]
        public float RestTimeByLevel;

        [FoldoutGroup("유휴")]
        [GUIColor("GetBoolColor")]
        [SuffixLabel("버프의 지속시간만큼 유휴 시간이 증가")]
        public bool IsBuffDurationAddedToRestTime;

        [FoldoutGroup("유휴")]
        [GUIColor("GetBoolColor")]
        [SuffixLabel("패시브의 유휴가 완료 시 UI 표시")]
        public bool IsSoliloquyOnRestCompleted;

        #endregion 유휴

        #region String

        [FoldoutGroup("#String/Custom", 3)] public string ApplyTargetAsString;
        [FoldoutGroup("#String/Custom", 3)] public string ApplyDamageOverTimeAtOnceAsString;
        [FoldoutGroup("#String/Custom", 3)] public string[] BuffsAsString;

        [FoldoutGroup("#String/Custom", 3)] public string[] ReleaseBuffsAsString;
        [FoldoutGroup("#String/Custom", 3)] public string[] ReleaseStateEffectsAsString;
        [FoldoutGroup("#String/Custom", 3)] public string ReleaseBuffApplicationAsString;

        [FoldoutGroup("#String/Custom", 3)] public string[] HitmarkNameStrings;
        [FoldoutGroup("#String/Custom", 3)] public string DurationStateEffectAsString;
        [FoldoutGroup("#String/Custom", 3)] public string[] ReduceSkillCooldownsAsString;

        [FoldoutGroup("#String/Custom", 3)] public string RewardOperatorAsString;
        [FoldoutGroup("#String/Custom", 3)] public string[] RewardPotionsAsString;
        [FoldoutGroup("#String/Custom", 3)] public string[] RewardCurrenciesAsString;

        #endregion String

        public override void Rename()
        {
            Rename("Effect");
        }

        public override void Validate()
        {
            if (!IsChangingAsset)
            {
                if (!EnumEx.ConvertTo(ref Name, NameString))
                {
                    Log.Error("{0}({1}) 패시브의 NameString 값을 갱신할 수 없습니다. {2}", Name, Name.ToLogString(), NameString);
                }

                if (!EnumEx.ConvertTo(ref ApplyTarget, ApplyTargetAsString))
                {
                    Log.Error("{0}({1}) 패시브의 ApplyTarget값을 갱신할 수 없습니다. {2}", Name, Name.ToLogString(), ApplyTargetAsString);
                }
                if (BuffsAsString.IsValidArray())
                {
                    if (!EnumEx.ConvertTo(ref Buffs, BuffsAsString))
                    {
                        Log.Error("{0}({1}) 패시브의 Buffs 값을 갱신할 수 없습니다. {2}", Name, Name.ToLogString(), BuffsAsString.JoinToString());
                    }
                }

                if (!EnumEx.ConvertTo(ref Hitmarks, HitmarkNameStrings))
                {
                    Log.Error("{0}({1}) 패시브의 Hitmarks 값을 갱신할 수 없습니다. {2}", Name, Name.ToLogString(), HitmarkNameStrings);
                }
                if (!EnumEx.ConvertTo(ref DurationStateEffect, DurationStateEffectAsString))
                {
                    Log.Error("{0}({1}) 패시브의 DurationStateEffect 값을 갱신할 수 없습니다. {2}", Name, Name.ToLogString(), DurationStateEffectAsString);
                }

                if (!EnumEx.ConvertTo(ref ApplyDamageOverTimeAtOnce, ApplyDamageOverTimeAtOnceAsString))
                {
                    Log.Error("{0}({1}) 패시브의 ApplyDamageOverTimeAtOnce 값을 갱신할 수 없습니다. {2}", Name, Name.ToLogString(), ApplyDamageOverTimeAtOnceAsString);
                }

                try
                {
                    if (CurrencyRewards.IsValid())
                    {
                        for (int i = 0; i < CurrencyRewards.Length; i++)
                        {
                            PassiveRewardCurrency item = CurrencyRewards[i];
                            item.Validate();
                        }
                    }
                }
                catch (System.Exception e)
                {
                    Log.Warning($"{name}: {e}");
                }

                if (ReleaseBuffsAsString.IsValidArray())
                {
                    if (!EnumEx.ConvertTo(ref ReleaseBuffs, ReleaseBuffsAsString))
                    {
                        Log.Error("{0}({1}) 패시브의 ReleaseBuffs 값을 갱신할 수 없습니다. {2}", Name, Name.ToLogString(), ReleaseBuffsAsString.JoinToString());
                    }
                }
                if (ReleaseStateEffectsAsString.IsValidArray())
                {
                    if (!EnumEx.ConvertTo(ref ReleaseStateEffects, ReleaseStateEffectsAsString))
                    {
                        Log.Error("{0}({1}) 패시브의 ReleaseStateEffects 값을 갱신할 수 없습니다. {2}", Name, Name.ToLogString(), ReleaseStateEffectsAsString.JoinToString());
                    }
                }
                if (!EnumEx.ConvertTo(ref ReleaseBuffApplication, ReleaseBuffApplicationAsString))
                {
                    Log.Error("{0}({1}) 패시브의 ReleaseBuffApplicationAsString 값을 갱신할 수 없습니다. {2}", Name, Name.ToLogString(), ReleaseBuffApplicationAsString);
                }
            }
        }

        public override void Refresh()
        {
            NameString = Name.ToString();
            ApplyTargetAsString = ApplyTarget.ToString();
            BuffsAsString = Buffs.ToStringArray();
            HitmarkNameStrings = Hitmarks.ToStringArray();
            DurationStateEffectAsString = DurationStateEffect.ToString();
            ApplyDamageOverTimeAtOnceAsString = ApplyDamageOverTimeAtOnce.ToString();
            ReleaseBuffsAsString = ReleaseBuffs.ToStringArray();
            ReleaseStateEffectsAsString = ReleaseStateEffects.ToStringArray();
            ReleaseBuffApplicationAsString = ReleaseBuffApplication.ToString();

            if (CurrencyRewards.IsValid())
            {
                for (int i = 0; i < CurrencyRewards.Length; i++)
                {
                    CurrencyRewards[i].Refresh();
                }
            }

            IsChangingAsset = false;

            base.Refresh();
        }

        public override bool RefreshWithoutSave()
        {
            _hasChangedWhiteRefreshAll = false;

            UpdateIfChanged(ref NameString, Name);
            UpdateIfChanged(ref ApplyTargetAsString, ApplyTarget);
            UpdateIfChangedArray(ref BuffsAsString, Buffs.ToStringArray());
            UpdateIfChangedArray(ref HitmarkNameStrings, Hitmarks.ToStringArray());
            UpdateIfChanged(ref DurationStateEffectAsString, DurationStateEffect);
            UpdateIfChanged(ref ApplyDamageOverTimeAtOnceAsString, ApplyDamageOverTimeAtOnce);
            UpdateIfChangedArray(ref ReleaseBuffsAsString, ReleaseBuffsAsString.ToStringArray());
            UpdateIfChangedArray(ref ReleaseStateEffectsAsString, ReleaseStateEffects.ToStringArray());
            UpdateIfChanged(ref ReleaseBuffApplicationAsString, ReleaseBuffApplication);

            if (CurrencyRewards.IsValid())
            {
                for (int i = 0; i < CurrencyRewards.Length; i++)
                {
                    if (CurrencyRewards[i].RefreshWithoutSave())
                    {
                        _hasChangedWhiteRefreshAll = true;
                    }
                }
            }

            _ = base.RefreshWithoutSave();
            IsChangingAsset = false;

            return _hasChangedWhiteRefreshAll;
        }

        protected override void RefreshAll()
        {
#if UNITY_EDITOR
            if (Selection.objects.Length > 1)
            {
                Debug.LogWarning("여러 개의 스크립터블 오브젝트가 선택되었습니다. 하나만 선택한 상태에서 실행하세요.");
                return;
            }
            PassiveNames[] passiveNames = EnumEx.GetValues<PassiveNames>();
            int passiveCount = 0;

            Log.Info("모든 패시브 효과 에셋의 갱신을 시작합니다: {0}", passiveNames.Length);

            base.RefreshAll();

            for (int i = 1; i < passiveNames.Length; i++)
            {
                if (passiveNames[i] != PassiveNames.None)
                {
                    PassiveAsset asset = ScriptableDataManager.Instance.FindPassive(passiveNames[i]);
                    if (asset.IsValid())
                    {
                        if (asset.EffectSettings.RefreshWithoutSave())
                        {
                            passiveCount += 1;
                        }
                    }
                }

                float progressRate = (i + 1).SafeDivide(passiveNames.Length);
                EditorUtility.DisplayProgressBar("모든 패시브 효과 에셋의 갱신", passiveNames[i].ToString(), progressRate);
            }

            EditorUtility.ClearProgressBar();
            OnRefreshAll();

            Log.Info("모든 패시브 효과 에셋의 갱신을 종료합니다: {0}/{1}", passiveCount.ToSelectString(passiveNames.Length), passiveNames.Length);
#endif
        }

        public override void OnLoadData()
        {
            base.OnLoadData();

            LogErrorInvalid();
            EnumLog();

            if (CurrencyRewards.IsValid())
            {
                for (int i = 0; i < CurrencyRewards.Length; i++)
                {
                    PassiveRewardCurrency item = CurrencyRewards[i];
                    item.OnLoadData();
                }
            }
        }

        public void Compare(PassiveEffectSettings other)
        {
            if (Name != other.Name) { Log.Warning($"{Name} != {other.Name}"); }
            if (ApplyTarget != other.ApplyTarget) { Log.Warning($"{ApplyTarget} != {other.ApplyTarget}"); }
            if (UseApplySelf != other.UseApplySelf) { Log.Warning($"{UseApplySelf} != {other.UseApplySelf}"); }
            if (StoreTriggerAttempt != other.StoreTriggerAttempt) { Log.Warning($"{StoreTriggerAttempt} != {other.StoreTriggerAttempt}"); }
            if (ApplyOneTime != other.ApplyOneTime) { Log.Warning($"{ApplyOneTime} != {other.ApplyOneTime}"); }
            if (ApplyOneTimePerItem != other.ApplyOneTimePerItem) { Log.Warning($"{ApplyOneTimePerItem} != {other.ApplyOneTimePerItem}"); }
            if (ApplyMaxCount != other.ApplyMaxCount) { Log.Warning($"{ApplyMaxCount} != {other.ApplyMaxCount}"); }
            if (ResetTimeApplyMaxCount != other.ResetTimeApplyMaxCount) { Log.Warning($"{ResetTimeApplyMaxCount} != {other.ResetTimeApplyMaxCount}"); }
            if (ApplyDelayTime != other.ApplyDelayTime) { Log.Warning($"{ApplyDelayTime} != {other.ApplyDelayTime}"); }
            if (ApplyDamageOverTimeAtOnce != other.ApplyDamageOverTimeAtOnce) { Log.Warning($"{ApplyDamageOverTimeAtOnce} != {other.ApplyDamageOverTimeAtOnce}"); }
            if (RendomApplyBuffs != other.RendomApplyBuffs) { Log.Warning($"{RendomApplyBuffs} != {other.RendomApplyBuffs}"); }
            if (Buffs != other.Buffs) { Log.Warning($"{Buffs.ToLogString()} != {other.Buffs.ToLogString()}"); }
            if (ReleaseBuffs != other.ReleaseBuffs) { Log.Warning($"{ReleaseBuffs} != {other.ReleaseBuffs}"); }
            if (ReleaseStateEffects != other.ReleaseStateEffects) { Log.Warning($"{ReleaseStateEffects} != {other.ReleaseStateEffects}"); }
            if (ReleaseBuffApplication != other.ReleaseBuffApplication) { Log.Warning($"{ReleaseBuffApplication} != {other.ReleaseBuffApplication}"); }
            if (Hitmarks != other.Hitmarks) { Log.Warning($"{Hitmarks.ToLogString()} != {other.Hitmarks.ToLogString()}"); }
            if (SetPositionToAttackPosition != other.SetPositionToAttackPosition) { Log.Warning($"{SetPositionToAttackPosition} != {other.SetPositionToAttackPosition}"); }
            if (SetPositionToTargetPosition != other.SetPositionToTargetPosition) { Log.Warning($"{SetPositionToTargetPosition} != {other.SetPositionToTargetPosition}"); }
            if (IsAutoUsePotion != other.IsAutoUsePotion) { Log.Warning($"{IsAutoUsePotion} != {other.IsAutoUsePotion}"); }
            if (DurationStateEffect != other.DurationStateEffect) { Log.Warning($"{DurationStateEffect} != {other.DurationStateEffect}"); }
            if (AddDuration != other.AddDuration) { Log.Warning($"{AddDuration} != {other.AddDuration}"); }
            if (AddMaxDuration != other.AddMaxDuration) { Log.Warning($"{AddMaxDuration} != {other.AddMaxDuration}"); }

            if (RewardOperator != other.RewardOperator) { Log.Warning($"{RewardOperator} != {other.RewardOperator}"); }

            _ = CurrencyRewards.Compare(other.CurrencyRewards);

            if (RewardLevel != other.RewardLevel) { Log.Warning($"{RewardLevel} != {other.RewardLevel}"); }
            if (RewardExperience != other.RewardExperience) { Log.Warning($"{RewardExperience} != {other.RewardExperience}"); }
            if (RestTime != other.RestTime) { Log.Warning($"{RestTime} != {other.RestTime}"); }
            if (RestTimeByLevel != other.RestTimeByLevel) { Log.Warning($"{RestTimeByLevel} != {other.RestTimeByLevel}"); }
            if (IsBuffDurationAddedToRestTime != other.IsBuffDurationAddedToRestTime) { Log.Warning($"{IsBuffDurationAddedToRestTime} != {other.IsBuffDurationAddedToRestTime}"); }
            if (IsSoliloquyOnRestCompleted != other.IsSoliloquyOnRestCompleted) { Log.Warning($"{IsSoliloquyOnRestCompleted} != {other.IsSoliloquyOnRestCompleted}"); }
        }

        private void LogErrorInvalid()
        {
#if UNITY_EDITOR
            if (IsChangingAsset)
            {
                Log.Error("패시브의 효과 에셋의 IsChangingAsset 변수가 활성화되어있습니다. {0}", name);
            }
            if (string.IsNullOrEmpty(NameString))
            {
                Log.Error("패시브 이름이 설정되지 않았습니다: {0}", name);
            }

            if (ReleaseBuffs.IsValidArray() || ReleaseStateEffects.IsValidArray())
            {
                if (ReleaseBuffApplication == BuffReleaseApplications.None)
                {
                    Log.Error("패시브의 버프 해제 효과가 설정되어있지만 버프 해제 방식이 설정되지 않았습니다. {0}", Name.ToLogString());
                }
            }
            if (ReleaseBuffApplication != BuffReleaseApplications.None)
            {
                if (!ReleaseBuffs.IsValidArray() && !ReleaseStateEffects.IsValidArray())
                {
                    Log.Error("패시브의 버프 해제 방식이 설정되어있지만 해제하는 버프가 설정되지 않았습니다. {0}", Name.ToLogString());
                }
            }
            if (RewardOperator == ConditionalOperators.None)
            {
                if (CurrencyRewards != null && CurrencyRewards.Length > 0)
                {
                    Log.Error($"패시브({Name.ToLogString()})의 보상 조건 연산자가 설정되지 않아 재화 보상을 얻을 수 없습니다.");
                }

                if (RewardLevel != 0 || RewardExperience != 0)
                {
                    Log.Error($"패시브({Name.ToLogString()})의 보상 조건 연산자가 설정되지 않아 경험치 보상을 얻을 수 없습니다.");
                }
            }
            if (Buffs.IsValid())
            {
                for (int i = 0; i < Buffs.Length; i++)
                {
                    BuffNames buffName = Buffs[i];
                    if (!Name.ToString().Contains(buffName.ToString()) && !buffName.ToString().Contains(Name.ToString()))
                    {
                        BuffAsset buffAsset = ScriptableDataManager.Instance.FindBuff(buffName);
                        if (buffAsset.IsValid() && buffAsset.Data.StateEffect == StateEffects.None)
                        {
                            Log.Warning(LogTags.ScriptableData, "{0} : {1} 패시브에서 이 버프는 올바르지 않을 가능성이 있습니다.", Name.ToLogString(), buffName.ToLogString());
                        }
                    }
                }
            }

            if (IsBuffDurationAddedToRestTime)
            {
                if (!Buffs.IsValidArray())
                {
                    Log.Error($"패시브({Name.ToLogString()})에 버프가 설정되지 않았습니다. 버프의 지속시간만큼 유휴 시간을 증가시킬 수 없습니다.");
                }
            }

#endif
        }

        private void EnumLog()
        {
#if UNITY_EDITOR
            string type = "PassiveEffect".ToColorString(GameColors.Passive);
            EnumExplorer.LogBuff(type, Name.ToString(), Buffs);
            EnumExplorer.LogBuff(type, Name.ToString(), ReleaseBuffs);
#endif
        }

        #region GUI-Color

        protected Color GetPassiveTargetTypeColor(PassiveTargetTypes triggerName)
        {
            if (triggerName == 0)
            {
                return GameColors.DarkGray;
            }
            else
            {
                return GameColors.GreenYellow;
            }
        }

        private Color GetBuffsColor()
        {
            if (Buffs.IsValidArray())
            {
                return GameColors.GreenYellow;
            }

            return GameColors.DarkGray;
        }

        private Color GetReleaseBuffsColor()
        {
            if (ReleaseBuffs.IsValidArray())
            {
                return GameColors.GreenYellow;
            }

            return GameColors.DarkGray;
        }

        private Color GetReleaseStateEffectsColor()
        {
            if (ReleaseStateEffects.IsValidArray())
            {
                return GameColors.GreenYellow;
            }
            return GameColors.DarkGray;
        }

        private Color GetBuffReleaseApplicationsColor()
        {
            if (ReleaseBuffApplication != BuffReleaseApplications.None)
            {
                return GameColors.GreenYellow;
            }

            if (ReleaseBuffs.IsValidArray() || ReleaseStateEffects.IsValidArray())
            {
                return GameColors.BestRed;
            }

            return GameColors.DarkGray;
        }

        private Color GetCurrencyRewardsColor()
        {
            if (CurrencyRewards.IsValid())
            {
                return GameColors.GreenYellow;
            }

            return GameColors.DarkGray;
        }

        protected Color GetHitmarksColor()
        {
            if (Hitmarks.IsValidArray())
            {
                return GameColors.GreenYellow;
            }
            else
            {
                return GameColors.DarkGray;
            }
        }

        #endregion GUI-Color

        public string GetString()
        {
            StringBuilder sb = new();

            // bool 타입 필드들 (true일 때만 포함)
            if (UseApplySelf)
            {
                _ = sb.Append($"자기 자신도 패시브 적용: {UseApplySelf.ToBoolString()}, ");
            }

            if (StoreTriggerAttempt)
            {
                _ = sb.Append($"발동 여부와는 관계없이 발동 여부를 저장합니다: {StoreTriggerAttempt.ToBoolString()}, ");
            }

            if (ApplyOneTime)
            {
                _ = sb.Append($"일회성 패시브 적용: {ApplyOneTime.ToBoolString()}, ");
            }

            if (ApplyOneTimePerItem)
            {
                _ = sb.Append($"아이템별 일회성 패시브 적용: {ApplyOneTimePerItem.ToBoolString()}, ");
            }

            if (RendomApplyBuffs)
            {
                _ = sb.Append($"무작위 여부: {RendomApplyBuffs.ToBoolString()}, ");
            }

            if (IsSetStackByLoad)
            {
                _ = sb.Append($"적용시 스택을 불러와 설정: {IsSetStackByLoad.ToBoolString()}, ");
            }

            if (UseRandomHitmark)
            {
                _ = sb.Append($"히트마크 무작위 적용 여부: {UseRandomHitmark.ToBoolString()}, ");
            }

            if (SetPositionToAttackPosition)
            {
                _ = sb.Append($"패시브의 트리거 위치에 따라 공격 위치 설정: {SetPositionToAttackPosition.ToBoolString()}, ");
            }

            if (SetPositionToTargetPosition)
            {
                _ = sb.Append($"패시브의 타겟 위치에 따라 공격 위치 설정: {SetPositionToTargetPosition.ToBoolString()}, ");
            }

            if (IsAutoUsePotion)
            {
                _ = sb.Append($"적용 물약 사용: {IsAutoUsePotion.ToBoolString()}, ");
            }

            if (IgnoreEffectOnPotionFail)
            {
                _ = sb.Append($"물약 사용 실패시 적용 안함: {IgnoreEffectOnPotionFail.ToBoolString()}, ");
            }

            if (UseSoliloquy)
            {
                _ = sb.Append($"안내 사용: {UseSoliloquy.ToBoolString()}, ");
            }

            if (IsBuffDurationAddedToRestTime)
            {
                _ = sb.Append($"버프의 지속시간만큼 유휴 시간이 증가: {IsBuffDurationAddedToRestTime.ToBoolString()}, ");
            }

            if (IsSoliloquyOnRestCompleted)
            {
                _ = sb.Append($"패시브의 유휴가 완료 시 UI 표시: {IsSoliloquyOnRestCompleted.ToBoolString()}, ");
            }

            // int 타입 필드들 (0이 아닐 때만 포함)
            if (ApplyMaxCount != 0)
            {
                _ = sb.Append($"패시브 적용 최대 횟수: {ApplyMaxCount.ToSelectString(0)}, ");
            }

            if (RewardLevel != 0)
            {
                _ = sb.Append($"레벨 보상: {RewardLevel.ToSelectString(0)}, ");
            }

            if (RewardExperience != 0)
            {
                _ = sb.Append($"경험치 보상: {RewardExperience.ToSelectString(0)}, ");
            }

            // float 타입 필드들 (0이 아닐 때만 포함)
            if (ResetTimeApplyMaxCount != 0f)
            {
                _ = sb.Append($"패시브 적용 최대 횟수 초기화 시간: {ResetTimeApplyMaxCount.ToSelectString(0)}, ");
            }

            if (ApplyDelayTime != 0f)
            {
                _ = sb.Append($"적용 지연시간: {ApplyDelayTime.ToSelectString(0)}, ");
            }

            if (AddDuration != 0f)
            {
                _ = sb.Append($"추가 지속시간: {AddDuration.ToSelectString(0)}, ");
            }

            if (AddMaxDuration != 0f)
            {
                _ = sb.Append($"추가 지속 시간으로 추가할 수 있는 최대 지속 시간: {AddMaxDuration.ToSelectString(0)}, ");
            }

            if (MinAddDuration != 0f)
            {
                _ = sb.Append($"범위 내 최소 지속 시간: {MinAddDuration.ToSelectString(0)}, ");
            }

            if (MaxAddDuration != 0f)
            {
                _ = sb.Append($"범위 내 최대 추가 지속시간: {MaxAddDuration.ToSelectString(0)}, ");
            }

            if (GrowAddDuration != 0f)
            {
                _ = sb.Append($"성장 추가 지속시간: {GrowAddDuration.ToSelectString(0)}, ");
            }

            if (RestTime != 0f)
            {
                _ = sb.Append($"유휴 시간동안 패시브 미발동: {RestTime.ToSelectString(0)}, ");
            }

            if (RestTimeByLevel != 0f)
            {
                _ = sb.Append($"패시브 레벨 증가 시 유휴 시간이 증가: {RestTimeByLevel.ToSelectString(0)}, ");
            }

            // enum 타입 필드들 (0이 아닐 때만 포함)
            if (ApplyTarget != 0)
            {
                _ = sb.Append($"패시브 적용 타겟 설정: {ApplyTarget}, ");
            }

            if (ApplyDamageOverTimeAtOnce != 0)
            {
                _ = sb.Append($"지속 피해 한 번에 적용: {ApplyDamageOverTimeAtOnce.ToLogString()}, ");
            }

            if (ReleaseBuffApplication != 0)
            {
                _ = sb.Append($"적용시 해제 방식: {ReleaseBuffApplication}, ");
            }

            if (DurationStateEffect != 0)
            {
                _ = sb.Append($"변동 지속시간 상태이상: {DurationStateEffect.ToLogString()}, ");
            }

            if (RewardOperator != 0)
            {
                _ = sb.Append($"보상 조건 연산자: {RewardOperator}, ");
            }

            // 배열 필드들 (유효한 배열일 때만 포함)
            if (Buffs.IsValidArray())
            {
                _ = sb.Append($"적용되는 버프 이름: {string.Join(", ", Buffs.ToLogString())}, ");
            }

            if (ReleaseBuffs.IsValidArray())
            {
                _ = sb.Append($"적용시 해제 버프: {string.Join(", ", ReleaseBuffs.ToLogString())}, ");
            }

            if (ReleaseStateEffects.IsValidArray())
            {
                _ = sb.Append($"적용시 해제 상태이상: {string.Join(", ", ReleaseStateEffects.ToLogString())}, ");
            }

            if (Hitmarks.IsValidArray())
            {
                _ = sb.Append($"적용되는 히트마크: {string.Join(", ", Hitmarks.ToLogString())}, ");
            }

            if (CurrencyRewards.IsValid())
            {
                _ = sb.AppendLine();
                foreach (PassiveRewardCurrency item in CurrencyRewards)
                {
                    _ = sb.Append($"재화 보상: {item.GetString()}, ");
                }
            }

            // 마지막 쉼표와 공백 제거
            if (sb.Length > 2)
            {
                sb.Length -= 2; // 마지막 ", " 제거
            }

            return sb.ToString();
        }
    }
}