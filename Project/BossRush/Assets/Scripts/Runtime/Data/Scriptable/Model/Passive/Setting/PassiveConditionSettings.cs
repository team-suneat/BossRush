// ConditionSettings.cs
using Sirenix.OdinInspector;
using System.Text;
using TeamSuneat.Passive;
using UnityEditor;
using UnityEngine;

namespace TeamSuneat.Data
{
    [CreateAssetMenu(fileName = "Condition", menuName = "TeamSuneat/Scriptable/Passive Setting/ConditionSettings")]
    public class PassiveConditionSettings : XScriptableObject
    {
        public bool IsChangingAsset;
        public PassiveNames Name;

        #region Target

        [FoldoutGroup("Target", true)]
        [GUIColor("GetPassiveTargetTypeColor")]
        [EnableIf("IsChangingAsset")]
        [SuffixLabel("패시브가 검색할 대상")]
        public PassiveTargetTypes ConditionTarget;

        #endregion Target

        #region Character

        [FoldoutGroup("Character", true)]
        [GUIColor("GetConditionCharactersColor")]
        [EnableIf("IsChangingAsset")]
        [SuffixLabel("조건 캐릭터")]
        public CharacterNames[] ConditionCharacters;

        [FoldoutGroup("Character", true)]
        [GUIColor("GetConditionMonsterGradesColor")]
        [EnableIf("IsChangingAsset")]
        [SuffixLabel("몬스터 등급")]
        public MonsterGrades[] ConditionMonsterGrades;

        #endregion Character

        #region Buff

        [FoldoutGroup("Buff", true)]
        [GUIColor("GetBuffNameColor")]
        [EnableIf("IsChangingAsset")]
        [SuffixLabel("버프 이름")]
        public BuffNames ConditionBuff;

        [FoldoutGroup("Buff", true)]
        [GUIColor("GetBuffNameColor")]
        [EnableIf("IsChangingAsset")]
        [SuffixLabel("버프 이름 2")]
        public BuffNames ConditionBuff2;

        [FoldoutGroup("Buff", true)]
        [GUIColor("GetIntColor")]
        [EnableIf("IsChangingAsset")]
        [SuffixLabel("버프 스택 개수")]
        public int ConditionBuffStack;

        [FoldoutGroup("Buff", true)]
        [GUIColor("GetBuffTypeColor")]
        [SuffixLabel("버프 타입 종류")]
        public BuffTypes ConditionBuffType;

        [FoldoutGroup("Buff", true)]
        [GUIColor("GetStateEffectColor")]
        [EnableIf("IsChangingAsset")]
        [SuffixLabel("버프 상태효과 이상")]
        public StateEffects ConditionStateEffect;

        [FoldoutGroup("Buff", true)]
        [GUIColor("GetIntColor")]
        [SuffixLabel("버프 상태효과 이상 수")]
        public int ConditionStateEffectCount;

        [FoldoutGroup("Buff", true)]
        [GUIColor("GetPassiveOperatorColor")]
        [InfoBox("$ConditionStateEffectOperatorMessege")]
        public PassiveOperator ConditionStateEffectOperator;

        #endregion Buff

        #region Buff-Ignore

        [FoldoutGroup("Buff-Ignore", true)]
        [GUIColor("GetPassiveTargetTypeColor")]
        [EnableIf("IsChangingAsset")]
        [SuffixLabel("버프가 검색할 대상")]
        public PassiveTargetTypes ConditionIgnoreTarget;

        [FoldoutGroup("Buff-Ignore", true)]
        [GUIColor("GetBuffNameColor")]
        [EnableIf("IsChangingAsset")]
        public BuffNames ConditionIgnoreBuff;

        [FoldoutGroup("Buff-Ignore", true)]
        [GUIColor("GetBuffTypeColor")]
        [EnableIf("IsChangingAsset")]
        public BuffTypes ConditionIgnoreBuffType;

        [FoldoutGroup("Buff-Ignore", true)]
        [GUIColor("GetConditionIgnoreStateEffectsColor")]
        [EnableIf("IsChangingAsset")]
        public StateEffects[] ConditionIgnoreStateEffects;

        [FoldoutGroup("Buff-Ignore", true)]
        [GUIColor("GetPassiveOperatorColor")]
        [InfoBox("$ConditionStateEffectOperatorMessege")]
        public PassiveOperator ConditionIgnoreStateEffectOperator;

        #endregion Buff-Ignore

        #region Vital Resource

        [FoldoutGroup("Vital Resource", true)]
        [EnableIf("IsChangingAsset")]
        [GUIColor("GetVitalResourceColor")]
        public VitalResourceTypes ConditionVitalResource;

        [FoldoutGroup("Vital Resource", true)]
        [EnableIf("IsChangingAsset")]
        [GUIColor("GetPassiveOperatorColor")]
        [InfoBox("$ConditionVitalResourceOperatorMessege")]
        public PassiveOperator ConditionVitalResourceOperator;

        [FoldoutGroup("Vital Resource", true)]
        [GUIColor("GetIntColor")]
        public int ConditionVitalResourceValue;

        [FoldoutGroup("Vital Resource", true)]
        [GUIColor("GetFloatColor")]
        [Range(0f, 1f)]
        public float ConditionVitalResourceRatio;

        #endregion Vital Resource

        #region String

        [FoldoutGroup("#String/Custom", 3)] public string ConditionTargetAsString;
        [FoldoutGroup("#String/Custom", 3)] public string[] ConditionCharactersAsString;
        [FoldoutGroup("#String/Custom", 3)] public string[] ConditionMonsterGradesAsString;
        [FoldoutGroup("#String/Custom", 3)] public string ConditionBuffAsString;
        [FoldoutGroup("#String/Custom", 3)] public string ConditionBuff2AsString;
        [FoldoutGroup("#String/Custom", 3)] public string ConditionBuffTypeAsString;
        [FoldoutGroup("#String/Custom", 3)] public string ConditionStateEffectAsString;
        [FoldoutGroup("#String/Custom", 3)] public string ConditionStateEffectOperatorAsString;
        [FoldoutGroup("#String/Custom", 3)] public string ConditionIgnoreCharacterCampAsString;
        [FoldoutGroup("#String/Custom", 3)] public string ConditionIgnoreBuffAsString;
        [FoldoutGroup("#String/Custom", 3)] public string ConditionIgnoreBuffTypeAsString;
        [FoldoutGroup("#String/Custom", 3)] public string[] ConditionIgnoreStateEffectsAsString;
        [FoldoutGroup("#String/Custom", 3)] public string ConditionIgnoreStateEffectOperatorAsString;
        [FoldoutGroup("#String/Custom", 3)] public string ConditionVitalResourceAsString;
        [FoldoutGroup("#String/Custom", 3)] public string ConditionVitalResourceOperatorAsString;
        [FoldoutGroup("#String/Custom", 3)] public string ConditionRelicNameAsString;
        [FoldoutGroup("#String/Custom", 3)] public string ConditionRelicOperatorAsString;

        #endregion String

        //────────────────────────────────────────────────────────────────────────────────────────────────

        public override void Rename()
        {
            Rename("Condition");
        }

        public override void Validate()
        {
            if (!IsChangingAsset)
            {
                if (!EnumEx.ConvertTo(ref Name, NameString))
                {
                    Log.Error("{0}({1}) 패시브 NameString 값이 변환되지 않았습니다. {2}", Name, Name.ToLogString(), NameString);
                }
                if (!EnumEx.ConvertTo(ref ConditionTarget, ConditionTargetAsString))
                {
                    Log.Error("{0}({1}) 패시브 ConditionTarget 값이 변환되지 않았습니다. {2}", Name, Name.ToLogString(), ConditionTargetAsString);
                }

                if (ConditionCharactersAsString.IsValidArray())
                {
                    if (!EnumEx.ConvertTo(ref ConditionCharacters, ConditionCharactersAsString))
                    {
                        Log.Error("{0}({1}) 패시브 ConditionCharacters 값이 변환되지 않았습니다. {2}", Name, Name.ToLogString(), ConditionCharactersAsString);
                    }
                }

                if (ConditionMonsterGradesAsString.IsValidArray())
                {
                    if (!EnumEx.ConvertTo(ref ConditionMonsterGrades, ConditionMonsterGradesAsString))
                    {
                        Log.Error("{0}({1}) 패시브 ConditionMonsterGrades 값이 변환되지 않았습니다. {2}", Name, Name.ToLogString(), ConditionMonsterGradesAsString);
                    }
                }
                if (!EnumEx.ConvertTo(ref ConditionBuff, ConditionBuffAsString))
                {
                    Log.Error("{0}({1}) 패시브 ConditionBuff 값이 변환되지 않았습니다. {2}", Name, Name.ToLogString(), ConditionBuffAsString);
                }
                if (!EnumEx.ConvertTo(ref ConditionBuff2, ConditionBuff2AsString))
                {
                    Log.Error("{0}({1}) 패시브 ConditionBuff 값이 변환되지 않았습니다. {2}", Name, Name.ToLogString(), ConditionBuffAsString);
                }
                if (!EnumEx.ConvertTo(ref ConditionBuffType, ConditionBuffTypeAsString))
                {
                    Log.Error("{0}({1}) 패시브 ConditionBuffType 값이 변환되지 않았습니다. {2}", Name, Name.ToLogString(), ConditionBuffTypeAsString);
                }
                if (!EnumEx.ConvertTo(ref ConditionStateEffectOperator, ConditionStateEffectOperatorAsString))
                {
                    Log.Error("{0}({1}) 패시브 ConditionStateEffectOperator 값이 변환되지 않았습니다. {2}", Name, Name.ToLogString(), ConditionStateEffectOperatorAsString);
                }
                if (!EnumEx.ConvertTo(ref ConditionStateEffect, ConditionStateEffectAsString))
                {
                    Log.Error("{0}({1}) 패시브 ConditionStateEffect 값이 변환되지 않았습니다. {2}", Name, Name.ToLogString(), ConditionStateEffectAsString);
                }

                if (!EnumEx.ConvertTo(ref ConditionIgnoreTarget, ConditionIgnoreCharacterCampAsString))
                {
                    Log.Error("{0}({1}) 패시브 ConditionIgnoreCharacterCamp 값이 변환되지 않았습니다. {2}", Name, Name.ToLogString(), ConditionIgnoreCharacterCampAsString);
                }
                if (!EnumEx.ConvertTo(ref ConditionIgnoreBuff, ConditionIgnoreBuffAsString))
                {
                    Log.Error("{0}({1}) 패시브 ConditionIgnoreBuff 값이 변환되지 않았습니다. {2}", Name, Name.ToLogString(), ConditionIgnoreBuffAsString);
                }
                if (!EnumEx.ConvertTo(ref ConditionIgnoreBuffType, ConditionIgnoreBuffTypeAsString))
                {
                    Log.Error("{0}({1}) 패시브 ConditionIgnoreBuffType 값이 변환되지 않았습니다. {2}", Name, Name.ToLogString(), ConditionIgnoreBuffTypeAsString);
                }
                if (!EnumEx.ConvertTo(ref ConditionIgnoreStateEffects, ConditionIgnoreStateEffectsAsString))
                {
                    Log.Error("{0}({1}) 패시브 ConditionIgnoreStateEffects 값이 변환되지 않았습니다. {2}", Name, Name.ToLogString(), ConditionIgnoreStateEffectsAsString);
                }
                if (!EnumEx.ConvertTo(ref ConditionIgnoreStateEffectOperator, ConditionIgnoreStateEffectOperatorAsString))
                {
                    Log.Error("{0}({1}) 패시브 ConditionIgnoreStateEffectOperator 값이 변환되지 않았습니다. {2}", Name, Name.ToLogString(), ConditionIgnoreStateEffectOperatorAsString);
                }
            }

            ConditionStateEffectOperatorMessege = GetPassiveOperatorMassege(ConditionStateEffectOperator);
            ConditionVitalResourceOperatorMessege = GetPassiveOperatorMassege(ConditionVitalResourceOperator);
        }

        //

        public override void Refresh()
        {
            NameString = Name.ToString();

            ConditionTargetAsString = ConditionTarget.ToString();
            ConditionCharactersAsString = ConditionCharacters.ToStringArray();
            ConditionMonsterGradesAsString = ConditionMonsterGrades.ToStringArray();

            ConditionBuffAsString = ConditionBuff.ToString();
            ConditionBuff2AsString = ConditionBuff2.ToString();
            ConditionBuffTypeAsString = ConditionBuffType.ToString();
            ConditionStateEffectOperatorAsString = ConditionStateEffectOperator.ToString();
            ConditionStateEffectAsString = ConditionStateEffect.ToString();

            ConditionIgnoreCharacterCampAsString = ConditionIgnoreTarget.ToString();
            ConditionIgnoreBuffAsString = ConditionIgnoreBuff.ToString();
            ConditionIgnoreBuffTypeAsString = ConditionIgnoreBuffType.ToString();
            ConditionIgnoreStateEffectsAsString = ConditionIgnoreStateEffects.ToStringArray();
            ConditionIgnoreStateEffectOperatorAsString = ConditionIgnoreStateEffectOperator.ToString();

            IsChangingAsset = false;
            base.Refresh();
        }

        public override bool RefreshWithoutSave()
        {
            _hasChangedWhiteRefreshAll = false;

            UpdateIfChanged(ref NameString, Name);

            UpdateIfChanged(ref ConditionTargetAsString, ConditionTarget);
            UpdateIfChangedArray(ref ConditionCharactersAsString, ConditionCharacters.ToStringArray());
            UpdateIfChangedArray(ref ConditionMonsterGradesAsString, ConditionMonsterGrades.ToStringArray());

            UpdateIfChanged(ref ConditionBuffAsString, ConditionBuff);
            UpdateIfChanged(ref ConditionBuff2AsString, ConditionBuff2);
            UpdateIfChanged(ref ConditionBuffTypeAsString, ConditionBuffType);
            UpdateIfChanged(ref ConditionStateEffectOperatorAsString, ConditionStateEffectOperator);
            UpdateIfChanged(ref ConditionStateEffectAsString, ConditionStateEffect);

            UpdateIfChanged(ref ConditionIgnoreCharacterCampAsString, ConditionIgnoreTarget);
            UpdateIfChanged(ref ConditionIgnoreBuffAsString, ConditionIgnoreBuff);
            UpdateIfChanged(ref ConditionIgnoreBuffTypeAsString, ConditionIgnoreBuffType);
            UpdateIfChangedArray(ref ConditionIgnoreStateEffectsAsString, ConditionIgnoreStateEffects.ToStringArray());
            UpdateIfChanged(ref ConditionIgnoreStateEffectOperatorAsString, ConditionIgnoreStateEffectOperator);

            _ = base.RefreshWithoutSave();

            IsChangingAsset = false;
            return _hasChangedWhiteRefreshAll;
        }

        protected override void RefreshAll()
        {
#if UNITY_EDITOR
            if (Selection.objects.Length > 1)
            {
                Debug.LogWarning("여러 패시브 설정을 동시에 갱신할 수 없습니다. 하나의 패시브를 선택한 후 갱신하세요.");
                return;
            }
            PassiveNames[] passiveNames = EnumEx.GetValues<PassiveNames>();
            int passiveCount = 0;

            Log.Info("모든 패시브 효과 설정을 갱신합니다: {0}", passiveNames.Length);

            base.RefreshAll();

            for (int i = 1; i < passiveNames.Length; i++)
            {
                if (passiveNames[i] != PassiveNames.None)
                {
                    PassiveAsset asset = ScriptableDataManager.Instance.FindPassive(passiveNames[i]);
                    if (asset.IsValid())
                    {
                        if (asset.ConditionSettings == null)
                        {
                            continue;
                        }

                        if (asset.ConditionSettings.RefreshWithoutSave())
                        {
                            passiveCount += 1;
                        }
                    }
                }

                float progressRate = (i + 1).SafeDivide(passiveNames.Length);
                EditorUtility.DisplayProgressBar("모든 패시브 효과 설정을 갱신합니다", passiveNames[i].ToString(), progressRate);
            }

            EditorUtility.ClearProgressBar();
            OnRefreshAll();

            Log.Info("모든 패시브 효과 설정을 갱신합니다: {0}/{1}",
                passiveCount.ToSelectString(passiveNames.Length), passiveNames.Length);
#endif
        }

        //

        public override void OnLoadData()
        {
            base.OnLoadData();

            LogErrorInvalid();
            EnumLog();
        }

        public void Compare(PassiveConditionSettings other)
        {
            if (Name != other.Name) { Log.Warning($"{Name} != {other.Name}"); }
            if (ConditionTarget != other.ConditionTarget) { Log.Warning($"{ConditionTarget} != {other.ConditionTarget}"); }
            if (ConditionMonsterGrades != other.ConditionMonsterGrades) { Log.Warning($"{ConditionMonsterGrades} != {other.ConditionMonsterGrades}"); }
            if (ConditionCharacters != other.ConditionCharacters) { Log.Warning($"{ConditionCharacters} != {other.ConditionCharacters}"); }

            if (ConditionBuff != other.ConditionBuff) { Log.Warning($"{ConditionBuff} != {other.ConditionBuff}"); }
            if (ConditionBuff2 != other.ConditionBuff2) { Log.Warning($"{ConditionBuff2} != {other.ConditionBuff2}"); }
            if (ConditionBuffStack != other.ConditionBuffStack) { Log.Warning($"{ConditionBuffStack} != {other.ConditionBuffStack}"); }
            if (ConditionBuffType != other.ConditionBuffType) { Log.Warning($"{ConditionBuffType} != {other.ConditionBuffType}"); }
            if (ConditionStateEffect != other.ConditionStateEffect) { Log.Warning($"{ConditionStateEffect} != {other.ConditionStateEffect}"); }
            if (ConditionStateEffectCount != other.ConditionStateEffectCount) { Log.Warning($"{ConditionStateEffectCount} != {other.ConditionStateEffectCount}"); }
            if (ConditionStateEffectOperator != other.ConditionStateEffectOperator) { Log.Warning($"{ConditionStateEffectOperator} != {other.ConditionStateEffectOperator}"); }

            if (ConditionIgnoreBuff != other.ConditionIgnoreBuff) { Log.Warning($"{ConditionIgnoreBuff} != {other.ConditionIgnoreBuff}"); }
            if (ConditionIgnoreBuffType != other.ConditionIgnoreBuffType) { Log.Warning($"{ConditionIgnoreBuffType} != {other.ConditionIgnoreBuffType}"); }
            if (ConditionIgnoreStateEffects != other.ConditionIgnoreStateEffects) { Log.Warning($"{ConditionIgnoreStateEffects} != {other.ConditionIgnoreStateEffects}"); }
            if (ConditionIgnoreStateEffectOperator != other.ConditionIgnoreStateEffectOperator) { Log.Warning($"{ConditionIgnoreStateEffectOperator} != {other.ConditionIgnoreStateEffectOperator}"); }

            if (ConditionVitalResource != other.ConditionVitalResource) { Log.Warning($"{ConditionVitalResource} != {other.ConditionVitalResource}"); }
            if (ConditionVitalResourceOperator != other.ConditionVitalResourceOperator) { Log.Warning($"{ConditionVitalResourceOperator} != {other.ConditionVitalResourceOperator}"); }
            if (ConditionVitalResourceValue != other.ConditionVitalResourceValue) { Log.Warning($"{ConditionVitalResourceValue} != {other.ConditionVitalResourceValue}"); }
            if (ConditionVitalResourceRatio != other.ConditionVitalResourceRatio) { Log.Warning($"{ConditionVitalResourceRatio} != {other.ConditionVitalResourceRatio}"); }
        }

        private void LogErrorInvalid()
        {
#if UNITY_EDITOR

            if (IsChangingAsset)
            {
                Log.Error("패시브 에셋이 IsChangingAsset 플래그가 활성화되어 있습니다. {0}", name);
            }

            if (string.IsNullOrEmpty(NameString))
            {
                Log.Error("패시브 이름이 설정되지 않았습니다: {0}", name);
            }

            if (ConditionTarget == PassiveTargetTypes.None)
            {
                Log.Error("패시브 대상이 설정되지 않았습니다: {0}", Name.ToLogString());
            }

            if (!ConditionCharacters.IsValidArray() &&
                !ConditionMonsterGrades.IsValidArray() &&
                ConditionBuff == 0 &&
                ConditionBuff2 == 0 &&
                ConditionBuffStack == 0 &&
                ConditionBuffType == 0 &&
                ConditionStateEffect == 0 &&
                ConditionStateEffectCount == 0 &&
                ConditionStateEffectOperator == 0 &&
                ConditionIgnoreBuff == 0 &&
                ConditionIgnoreBuffType == 0 &&
                !ConditionIgnoreStateEffects.IsValidArray() &&
                ConditionIgnoreStateEffectOperator == 0 &&
                ConditionVitalResource == 0 &&
                ConditionVitalResourceOperator == 0 &&
                ConditionVitalResourceValue == 0 &&
                ConditionVitalResourceRatio == 0)
            {
                Log.Error("패시브 조건이 하나도 설정되지 않았습니다: {0}", Name.ToLogString());
            }

            if (ConditionVitalResourceOperator is not PassiveOperator.None and not PassiveOperator.Equal)
            {
                if (ConditionVitalResourceValue == 0 && ConditionVitalResourceRatio == 0)
                {
                    Log.Error("패시브 조건 체력이 설정되지 않았습니다: {0}", Name.ToLogString());
                }
            }

            if (ConditionIgnoreTarget == 0)
            {
                if (ConditionIgnoreBuff != 0)
                {
                    Log.Error($"패시브가 제외할 캐릭터가 설정되어 있지 않습니다:{Name.ToLogString()}, 제외 버프: {ConditionIgnoreBuff.ToLogString()}");
                }
                if (ConditionIgnoreBuffType != 0)
                {
                    Log.Error($"패시브가 제외할 캐릭터가 설정되어 있지 않습니다:{Name.ToLogString()}, 제외 버프 타입: {ConditionIgnoreBuffType.ToLogString()}");
                }
                if (ConditionIgnoreStateEffects.IsValid())
                {
                    Log.Error($"패시브가 제외할 캐릭터가 설정되어 있지 않습니다:{Name.ToLogString()}, 제외 상태효과: {ConditionIgnoreStateEffects.JoinToString()}");
                }
                if (ConditionIgnoreStateEffectOperator != 0)
                {
                    Log.Error($"패시브가 제외할 캐릭터가 설정되어 있지 않습니다:{Name.ToLogString()}, 제외 상태효과 연산자: {ConditionIgnoreStateEffectOperator.ToSelectString()}");
                }
            }

#endif
        }

        private void EnumLog()
        {
#if UNITY_EDITOR
            string type = "PassiveCondition".ToColorString(GameColors.Passive);
            EnumExplorer.LogBuff(type, Name.ToString(), ConditionBuff);
            EnumExplorer.LogBuff(type, Name.ToString(), ConditionBuff2);
            EnumExplorer.LogBuff(type, Name.ToString(), ConditionIgnoreBuff);
#endif
        }

        #region Color

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

        protected Color GetConditionIgnoreStateEffectsColor()
        { if (!ConditionIgnoreStateEffects.IsValid()) { return GameColors.DarkGray; } return GameColors.GreenYellow; }

        protected Color GetConditionCharactersColor()
        { if (!ConditionCharacters.IsValid()) { return GameColors.DarkGray; } return GameColors.GreenYellow; }

        protected Color GetConditionMonsterGradesColor()
        { if (!ConditionMonsterGrades.IsValid()) { return GameColors.DarkGray; } return GameColors.GreenYellow; }

        protected Color GetPassiveOperatorColor(PassiveOperator key)
        { if (key == 0) { return GameColors.DarkGray; } return GameColors.GreenYellow; }

        protected Color GetVitalResourceColor(VitalResourceTypes key)
        { if (key == 0) { return GameColors.DarkGray; } return GameColors.GreenYellow; }

        #endregion Color

        private string ConditionStateEffectOperatorMessege;
        private string ConditionVitalResourceOperatorMessege;
        private readonly string ConditionRelicOperatorMessege;

        private string GetPassiveOperatorMassege(PassiveOperator passiveOperator)
        {
            switch (passiveOperator)
            {
                case PassiveOperator.Under: { return "이하"; }
                case PassiveOperator.Over: { return "초과"; }
                case PassiveOperator.Equal: { return "같음"; }
                case PassiveOperator.Below: { return "미만"; }
                case PassiveOperator.More: { return "이상"; }
            }

            return string.Empty;
        }

        public string GetString()
        {
            StringBuilder sb = new();

            // int 타입 필드들 (0이 아닐 때만 포함)
            if (ConditionBuffStack != 0)
            {
                _ = sb.Append($"버프 스택 개수: {ConditionBuffStack.ToSelectString(0)}, ");
            }

            if (ConditionStateEffectCount != 0)
            {
                _ = sb.Append($"버프 상태효과 이상 수: {ConditionStateEffectCount.ToSelectString(0)}, ");
            }

            if (ConditionVitalResourceValue != 0)
            {
                _ = sb.Append($"자원 값: {ConditionVitalResourceValue.ToSelectString(0)}, ");
            }

            // float 타입 필드들 (0이 아닐 때만 포함)
            if (ConditionVitalResourceRatio != 0f)
            {
                _ = sb.Append($"자원 비율: {ConditionVitalResourceRatio.ToSelectString(0)}, ");
            }

            // enum 타입 필드들 (0이 아닐 때만 포함)
            if (ConditionTarget != 0)
            {
                _ = sb.Append($"패시브가 검색할 대상: {ConditionTarget}, ");
            }

            if (ConditionBuff != 0)
            {
                _ = sb.Append($"버프 이름: {ConditionBuff.ToLogString()}, ");
            }

            if (ConditionBuff2 != 0)
            {
                _ = sb.Append($"버프 이름 2: {ConditionBuff2.ToLogString()}, ");
            }

            if (ConditionBuffType != 0)
            {
                _ = sb.Append($"버프 타입 종류: {ConditionBuffType.ToLogString()}, ");
            }

            if (ConditionStateEffect != 0)
            {
                _ = sb.Append($"버프 상태효과 이상: {ConditionStateEffect.ToLogString()}, ");
            }

            if (ConditionStateEffectOperator != 0)
            {
                _ = sb.Append($"상태효과 연산자: {ConditionStateEffectOperator}, ");
            }

            if (ConditionIgnoreTarget != 0)
            {
                _ = sb.Append($"버프가 검색할 대상: {ConditionIgnoreTarget}, ");
            }

            if (ConditionIgnoreBuff != 0)
            {
                _ = sb.Append($"무시할 버프: {ConditionIgnoreBuff.ToLogString()}, ");
            }

            if (ConditionIgnoreBuffType != 0)
            {
                _ = sb.Append($"무시할 버프 타입: {ConditionIgnoreBuffType.ToLogString()}, ");
            }

            if (ConditionIgnoreStateEffectOperator != 0)
            {
                _ = sb.Append($"무시할 상태효과 연산자: {ConditionIgnoreStateEffectOperator}, ");
            }

            if (ConditionVitalResource != 0)
            {
                _ = sb.Append($"자원 타입: {ConditionVitalResource}, ");
            }

            if (ConditionVitalResourceOperator != 0)
            {
                _ = sb.Append($"자원 연산자: {ConditionVitalResourceOperator}, ");
            }

            // 배열 필드들 (유효한 배열일 때만 포함)
            if (ConditionCharacters.IsValidArray())
            {
                _ = sb.Append($"조건 캐릭터: {string.Join(", ", ConditionCharacters.ToLogString())}, ");
            }

            if (ConditionMonsterGrades.IsValidArray())
            {
                _ = sb.Append($"몬스터 등급: {string.Join(", ", ConditionMonsterGrades.JoinToString())}, ");
            }

            if (ConditionIgnoreStateEffects.IsValidArray())
            {
                _ = sb.Append($"무시할 상태효과들: {string.Join(", ", ConditionIgnoreStateEffects.ToLogString())}, ");
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