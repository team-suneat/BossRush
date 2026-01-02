using Sirenix.OdinInspector;
using System.IO;
using UnityEngine;

namespace TeamSuneat.Data
{
    [CreateAssetMenu(fileName = "SkillAnimation", menuName = "TeamSuneat/Scriptable/SkillAnimation")]
    public class SkillAnimationAsset : XScriptableObject
    {
        public bool IsChangingAsset;

        [Title("#애니메이션 (Animation)")]
        [SuffixLabel("애니메이션 이름")]
        public string AnimationName;

        [GUIColor("GetIntFieldColor")]
        [SuffixLabel("애니메이션 순서")]
        public int Order;

        [EnableIf("IsChangingAsset")]
        [GUIColor("GetSkillNameFieldColor")]
        [SuffixLabel("연결된 기술 이름")]
        public SkillNames SkillName;

        [FoldoutGroup("#String")]
        [SuffixLabel("연결된 기술 이름(string)")]
        public string SkillNameString;

        [Title("#재시전 (Recast)")]
        [GUIColor("GetBoolFieldColor")]
        [SuffixLabel("재시전 시 애니메이션 정지 사용")]
        public bool UseStopByRecast;

        [GUIColor("GetFloatFieldColor")]
        [SuffixLabel("재시전 시 애니메이션을 정지시킬 수 있는 애니메이션 재생 시간")]
        public float RecastStopAnimationTime;

        [GUIColor("GetFloatFieldColor")]
        [SuffixLabel("재시전으로 정지한 기술 애니메이션을 재생시킬 수 있는 대기 시간")]
        public float RecastResumeAnimationDelayTime;

        [Title("#방향키 입력 움직임(Movement)")]
        [GUIColor("GetBoolFieldColor")]
        [SuffixLabel("기술 중 움직임 여부")]
        public bool Movable;

        [GUIColor("GetFloatFieldColor")]
        [SuffixLabel("기술 중 움직임 가능 시점 (0~100%)")]
        [Range(0, 1)]
        public float ReleaseMovableTimeRate = 1f;

        [Title("#반전(Flip)")]
        [GUIColor("GetBoolFieldColor")]
        [SuffixLabel("기술 중 반전 여부")]
        public bool isFlipable;

        [GUIColor("GetFloatFieldColor")]
        [Range(0, 1)]
        [SuffixLabel("기술 중 반전 가능 시점 (0~100%)")]
        public float ReleaseFlipableTimeRate = 1f;

        [Title("#애니메이션(Animation)")]
        [GUIColor("GetBoolFieldColor")]
        [SuffixLabel("애니메이션 연속 진행 여부")]
        public bool IsSequence;

        [GUIColor("GetIntFieldColor")]
        [SuffixLabel("애니메이션 우선순위")]
        public int Priority;

        [GUIColor("GetFloatFieldColor")]
        [SuffixLabel("기술 애니메이션 종료 후 재사용 대기")]
        public float AnimationCooldownOnStop = 0.05f;

        [FoldoutGroup("#String")]
        [SuffixLabel("무기 기술 카테고리 배열(string)")]
        public string[] WeaponCategoriesString;

        [Title("#토글 (Toggle)")]
        [GUIColor("GetBoolFieldColor")]
        [SuffixLabel("공중 기술 여부")]
        public bool IsInAir;

        [GUIColor("GetBoolFieldColor")]
        [SuffixLabel("쌍수 무기 여부")]
        public bool IsDualWeapon;

        [GUIColor("GetBoolFieldColor")]
        [SuffixLabel("피해 애니메이션 차단")]
        public bool IsBlockDamageAnimation;

        [Title("#강제 이동 (ForceVelocity)")]
        [GUIColor("GetForceVelocityFieldColor")]
        [SuffixLabel("시전 시 강제이동")]
        public FVNames ForceVelocityName;

        [FoldoutGroup("#String")]
        [SuffixLabel("시전 시 강제이동(string)")]
        public string ForceVelocityNameAsString;

        #region Replaced

        [Title("#대체 (Replaced)")]
        [SuffixLabel("대체 기술 애니메이션 이름")]
        public string ReplacedAnimationName;

        [EnableIf("IsChangingAsset")]
        [SuffixLabel("대체 기술 이름")]
        public SkillNames ReplacedSkillName;

        [FoldoutGroup("#String")]
        [SuffixLabel("대체 기술 이름")]
        public string ReplacedSkillNameString;

        [Title("#대체 조건 (Replaced Conditions)")]
        [EnableIf("IsChangingAsset")]
        [SuffixLabel("대체 조건 기술 이름")]
        public SkillNames ConditionSkillName;

        [FoldoutGroup("#String")]
        [SuffixLabel("대체 조건 기술 이름")]
        public string ConditionSkillNameString;

        [EnableIf("IsChangingAsset")]
        [SuffixLabel("대체 조건 능력치 이름")]
        public StatNames[] ConditionStatNames;

        [FoldoutGroup("#String")]
        [SuffixLabel("대체 조건 능력치 이름")]
        public string[] ConditionStatNameStrings;

        #endregion Replaced

        #region VFX

        [Title("#비주얼 이펙트")]
        [EnableIf("IsChangingAsset")]
        [SuffixLabel("시전 VFX 정보")]
        public VisualEffectSpawnData CastVFX;

        #endregion VFX

        public override void OnLoadData()
        {
            base.OnLoadData();

            CustomLog();
        }

        private void CustomLog()
        {
#if UNITY_EDITOR

            if (!string.IsNullOrEmpty(ReplacedAnimationName))
            {
                Log.Error("애니메이션 이름이 설정된 기술 애니메이션. {0}, {1}", AnimationName, ReplacedAnimationName);
            }

            string type = "SkillAnimation".ToColorString(GameColors.Value);
            EnumExplorer.LogStat(type, AnimationName, ConditionStatNames);
#endif
        }

        //

        public bool CheckPassGrounded(bool isGrounded)
        {
            return IsInAir == !isGrounded;
        }

        public bool CheckReplacedConditions(Character ownerCharacter)
        {
            if (!CheckReplacedSkillCondition(ownerCharacter.Skill))
            {
                Log.Progress(LogTags.Skill, "기술의 대체 기술 애니메이션 설정할 수 없습니다. 대체 조건 기술을 배우지 못했습니다. Animation: {0}, ReplacedSkill: {1}, ConditionSkill: {2}",
                    AnimationName.ToSelectString(), ReplacedSkillName.ToLogString(), ConditionSkillName.ToLogString());
                return false;
            }

            if (!CheckReplacedStatCondition(ownerCharacter.Stat))
            {
                Log.Progress(LogTags.Skill, "기술의 대체 기술 애니메이션 설정할 수 없습니다. 대체 조건 능력치를 얻지 못했습니다. Animation: {0}, ReplacedSkill: {1}, ConditionStat: {2}",
                    AnimationName.ToSelectString(), ReplacedSkillName.ToLogString(), ConditionStatNames.JoinToString().ToErrorString());

                return false;
            }

            return true;
        }

        private bool CheckReplacedSkillCondition(SkillSystem skillSystem)
        {
            if (ConditionSkillName != SkillNames.None)
            {
                SkillEntity skillEntity = skillSystem.Find(ConditionSkillName);
                if (skillEntity == null)
                {
                    return false;
                }

                if (skillEntity.Level <= 0)
                {
                    return false;
                }
            }

            return true;
        }

        private bool CheckReplacedStatCondition(StatSystem statSystem)
        {
            if (ConditionStatNames.IsValidArray())
            {
                for (int i = 0; i < ConditionStatNames.Length; i++)
                {
                    if (statSystem.FindValueOrDefault(ConditionStatNames[i]) > 0)
                    {
                        return true;
                    }
                }
                return false;
            }
            else
            {
                // 설정한 대체 조건 능력치가 없다면 가지고 있다고 판단합니다.
                return true;
            }
        }

        // Cast Visual Effect

        public void InitializeCastVFX(Character ownerCharacter)
        {
            CastVFX.SetOwner(ownerCharacter);
            CastVFX.LoadToggleValues();
        }

        public void SpawnCastVFX(Transform parent, Vector3 feedbackPosition)
        {
            if (CastVFX.TrySpawnVisualEffect())
            {
                Log.Info(LogTags.Skill, "[Asset] 기술 시전 VFX를 생성합니다. {0}, {1}, {2}", SkillName, SkillName.GetLocalizedString(), CastVFX.Prefabs.ToStringArray());

                if (CastVFX.UseParent)
                {
                    CastVFX.SpawnVisualEffect(parent, 0);
                }
                else
                {
                    CastVFX.SpawnVisualEffect(feedbackPosition, 0);
                }
            }
        }

        public void DespawnCastVFX()
        {
            Log.Info(LogTags.Skill, "[Asset] 기술 시전 VFX를 삭제합니다. {0}, {1}, {2}", SkillName, SkillName.GetLocalizedString(), CastVFX.Prefabs.ToStringArray());

            CastVFX.DespawnVisualEffect();
        }

        // Color

        private Color GetItemSubCategoriesFieldColor()
        {
            if (WeaponCategories.IsValidArray())
            {
                return GameColors.GreenYellow;
            }
            return GameColors.DarkGray;
        }

#if UNITY_EDITOR

        public override void Validate()
        {
            base.Validate();

            if (!IsChangingAsset)
            {
                EnumEx.ConvertTo(ref SkillName, SkillNameString);
                EnumEx.ConvertTo(ref WeaponCategories, WeaponCategoriesString);
                EnumEx.ConvertTo(ref ReplacedSkillName, ReplacedSkillNameString);
                EnumEx.ConvertTo(ref ConditionSkillName, ConditionSkillNameString);
                EnumEx.ConvertTo(ref ConditionStatNames, ConditionStatNameStrings);
                EnumEx.ConvertTo(ref ForceVelocityName, ForceVelocityNameAsString);
            }
        }

        public override void Refresh()
        {
            NameString = AnimationName;
            SkillNameString = SkillName.ToString();
            WeaponCategoriesString = WeaponCategories.ToStringArray();
            ReplacedSkillNameString = ReplacedSkillName.ToString();
            ConditionSkillNameString = ConditionSkillName.ToString();
            ConditionStatNameStrings = ConditionStatNames.ToStringArray();
            ForceVelocityNameAsString = ForceVelocityName.ToString();

            IsChangingAsset = false;
            base.Refresh();
        }

        public override bool RefreshWithoutSave()
        {
            NameString = AnimationName;

            UpdateIfChanged(ref SkillNameString, SkillName);
            UpdateIfChangedArray(ref WeaponCategoriesString, WeaponCategories.ToStringArray());
            UpdateIfChanged(ref ReplacedSkillNameString, ReplacedSkillName);
            UpdateIfChanged(ref ConditionSkillNameString, ConditionSkillName);
            UpdateIfChangedArray(ref ConditionStatNameStrings, ConditionStatNames.ToStringArray());
            UpdateIfChanged(ref ForceVelocityNameAsString, ForceVelocityName);

            return base.RefreshWithoutSave();
        }

        public override void Rename()
        {
            string assetPath = UnityEditor.AssetDatabase.GetAssetPath(GetInstanceID());
            string newName = $"SkillAnimation_{AnimationName}";
            string fileName = Path.GetFileName(assetPath);

            if (newName + ".asset" != fileName)
            {
                Log.Info(LogTags.ScriptableData, $"XScriptableObject.Rename(), {fileName} => {(newName + ".asset").ToSelectString()}");
                _ = UnityEditor.AssetDatabase.RenameAsset(assetPath, newName);
            }
        }

#endif
    }
}