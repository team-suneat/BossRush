using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace TeamSuneat.Data
{
    public partial class SkillAsset : XScriptableObject
    {
#if UNITY_EDITOR

        [FoldoutGroup("#Button")]
        [Button("문서 기반 모든 스킬 에셋 생성 (1회성)", ButtonSizes.Large)]
        private void CreateAllSkillsFromDocument()
        {
            base.CreateAll();

            SkillNames[] skillNames = EnumEx.GetValues<SkillNames>();
            int createdCount = 0;
            int skippedCount = 0;

            Debug.LogFormat("문서 기반 스킬 에셋 생성 시작: {0}개", skillNames.Length - 1);

            for (int i = 1; i < skillNames.Length; i++)
            {
                if (skillNames[i] is SkillNames.None)
                {
                    continue;
                }

                SkillAsset asset = ScriptableDataManager.Instance.FindSkill(skillNames[i]);
                if (asset == null)
                {
                    asset = CreateAsset<SkillAsset>("Skill", skillNames[i].ToString(), true);
                    if (asset != null)
                    {
                        asset.Data = CreateSkillData(skillNames[i]);
                        asset.NameString = skillNames[i].ToString();
                        createdCount++;

                        float progressRate = (i + 1).SafeDivide(skillNames.Length);
                        EditorUtility.DisplayProgressBar("스킬 에셋 생성", skillNames[i].ToString(), progressRate);
                    }
                }
                else
                {
                    skippedCount++;
                }
            }

            EditorUtility.ClearProgressBar();
            PathManager.UpdatePathMetaData();

            Debug.LogFormat("스킬 에셋 생성 완료: 생성 {0}개, 건너뜀 {1}개", createdCount, skippedCount);
        }

        private SkillAssetData CreateSkillData(SkillNames skillName)
        {
            SkillAssetData data = new SkillAssetData
            {
                Name = skillName,
                MaxLevel = 10,
                EffectMultiplier = 1.0f,
                BaseHitCount = 1
            };

            switch (skillName)
            {
                // 무속성
                case SkillNames.Rave: // 레이브 - 불멸 등급
                    data.Attribute = SkillAttributeTypes.None;
                    data.Type = SkillTypes.Other;
                    data.Grade = GradeNames.Immortal;
                    data.CooldownType = CooldownTypes.TimeBased;
                    data.BaseCooldownTime = 60f;
                    data.CooldownTimeByLevel = 0f;
                    data.BaseManaCost = 50;
                    data.ManaCostByLevel = 0;
                    data.BaseEffectValue = 70f;
                    data.EffectValueByLevel = 10f;
                    data.Description = "5초간 하나의 적에게 가한 피해의 n%만큼 추가 피해를 입힌다.";
                    break;

                case SkillNames.Mantra: // 만트라 - 불멸 등급
                    data.Attribute = SkillAttributeTypes.None;
                    data.Type = SkillTypes.Other;
                    data.Grade = GradeNames.Immortal;
                    data.CooldownType = CooldownTypes.None;
                    data.BaseManaCost = 0;
                    data.ManaCostByLevel = 0;
                    data.BaseEffectValue = 30f;
                    data.EffectValueByLevel = 10f;
                    data.Description = "최종 공격력, 최종 체력 n% 증가. 스킬 슬롯에 장착하지 않고 효과가 적용된다.";
                    break;

                default:
                    ApplyFireSkillData(data);
                    ApplyWaterSkillData(data);
                    ApplyWindSkillData(data);
                    ApplyEarthSkillData(data);
                    break;
            }

            return data;
        }

        private void ApplyFireSkillData(SkillAssetData data)
        {
            switch (data.Name)
            {
                // 불(Fire) 속성
                case SkillNames.FlameSlash: // 불꽃 베기 - 일반 등급
                    data.Attribute = SkillAttributeTypes.Fire;
                    data.Type = SkillTypes.Active;
                    data.Grade = GradeNames.Common;
                    data.CooldownType = CooldownTypes.AttackCountBased;
                    data.BaseCooldownAttackCount = 12;
                    data.CooldownAttackCountByLevel = -1;
                    data.BaseManaCost = 25;
                    data.ManaCostByLevel = 0;
                    data.BaseEffectValue = 400f;
                    data.EffectValueByLevel = 40f;
                    data.EffectMultiplier = 1.5f;
                    data.BaseHitCount = 1;
                    data.HitCountByLevel = 1;
                    data.BaseRange = 3f;
                    data.RangeByLevel = 0f;
                    data.Description = "범위 3 이내의 적 모두에게 공격력의 n%×1.5 ×3로 1 • 2회 공격";
                    break;

                case SkillNames.FireSword: // 불의 검 - 일반 등급
                    data.Attribute = SkillAttributeTypes.Fire;
                    data.Type = SkillTypes.Passive;
                    data.Grade = GradeNames.Common;
                    data.CooldownType = CooldownTypes.TimeBased;
                    data.BaseCooldownTime = 17f;
                    data.CooldownTimeByLevel = 0f;
                    data.BaseManaCost = 50;
                    data.ManaCostByLevel = 0;
                    data.BaseEffectValue = 80f;
                    data.EffectValueByLevel = 3f;
                    data.Description = "10초간 전체 공격력 +n%";
                    break;

                case SkillNames.HeatWave: // 열풍 - 고급 등급
                    data.Attribute = SkillAttributeTypes.Fire;
                    data.Type = SkillTypes.Active;
                    data.Grade = GradeNames.Grand;
                    data.CooldownType = CooldownTypes.TimeBased;
                    data.BaseCooldownTime = 12f;
                    data.CooldownTimeByLevel = 0f;
                    data.BaseManaCost = 45;
                    data.ManaCostByLevel = 0;
                    data.BaseEffectValue = 1500f;
                    data.EffectValueByLevel = 150f;
                    data.EffectMultiplier = 1.5f;
                    data.BaseHitCount = 1;
                    data.HitCountByLevel = 1;
                    data.BaseRange = 3f;
                    data.RangeByLevel = 0f;
                    data.HasTimeStop = true;
                    data.Description = "범위 3 이내의 적 모두에게 공격력의 n%×1.5 ×3로 1 • 2회 공격, 시전 중 시간 정지";
                    break;

                case SkillNames.FireSlash: // 화염 베기 - 레어 등급
                    data.Attribute = SkillAttributeTypes.Fire;
                    data.Type = SkillTypes.Active;
                    data.Grade = GradeNames.Rare;
                    data.CooldownType = CooldownTypes.AttackCountBased;
                    data.BaseCooldownAttackCount = 13;
                    data.CooldownAttackCountByLevel = -1;
                    data.BaseManaCost = 25;
                    data.ManaCostByLevel = 0;
                    data.BaseEffectValue = 1350f;
                    data.EffectValueByLevel = 135f;
                    data.EffectMultiplier = 1.5f;
                    data.BaseHitCount = 1;
                    data.HitCountByLevel = 1;
                    data.BaseRange = 4f;
                    data.RangeByLevel = 0f;
                    data.Description = "범위 4 이내의 적 모두에게 공격력의 n%×1.5 ×3로 1 • 2회 공격";
                    break;

                case SkillNames.BurningSword: // 타오르는 검 - 레어 등급
                    data.Attribute = SkillAttributeTypes.Fire;
                    data.Type = SkillTypes.Passive;
                    data.Grade = GradeNames.Rare;
                    data.CooldownType = CooldownTypes.TimeBased;
                    data.BaseCooldownTime = 5f;
                    data.CooldownTimeByLevel = 0f;
                    data.BaseManaCost = 0;
                    data.ManaCostByLevel = 0;
                    data.BaseEffectValue = 16f;
                    data.EffectValueByLevel = 0.8f;
                    data.Description = "전투 돌입 후, 5초마다 전체 공격력 +n% (최대 10회 적용)";
                    break;

                case SkillNames.FireWave: // 화염 파동 - 영웅 등급
                    data.Attribute = SkillAttributeTypes.Fire;
                    data.Type = SkillTypes.Active;
                    data.Grade = GradeNames.Epic;
                    data.CooldownType = CooldownTypes.TimeBased;
                    data.BaseCooldownTime = 4f;
                    data.CooldownTimeByLevel = 0f;
                    data.BaseManaCost = 15;
                    data.ManaCostByLevel = 0;
                    data.BaseEffectValue = 700f;
                    data.EffectValueByLevel = 70f;
                    data.EffectMultiplier = 3f;
                    data.BaseHitCount = 3;
                    data.HitCountByLevel = 0;
                    data.Description = "적 발견시 검격을 날린다. 스플래시 데미지 공격력의 n%×3 ×3, 검격 속도 및 거리 증가";
                    break;

                case SkillNames.HellfireSlash: // 연옥 화염 베기 - 전설 등급
                    data.Attribute = SkillAttributeTypes.Fire;
                    data.Type = SkillTypes.Active;
                    data.Grade = GradeNames.Legendary;
                    data.CooldownType = CooldownTypes.AttackCountBased;
                    data.BaseCooldownAttackCount = 15;
                    data.CooldownAttackCountByLevel = -1;
                    data.BaseManaCost = 30;
                    data.ManaCostByLevel = 0;
                    data.BaseEffectValue = 1920f;
                    data.EffectValueByLevel = 192f;
                    data.EffectMultiplier = 1.5f;
                    data.BaseHitCount = 1;
                    data.HitCountByLevel = 1;
                    data.BaseRange = 6f;
                    data.RangeByLevel = 0f;
                    data.Description = "범위 6 이내의 적 모두에게 공격력의 n%×1.5 ×3로 1 • 2회 공격";
                    break;

                case SkillNames.TrueHeatWave: // 진 열풍 - 전설 등급
                    data.Attribute = SkillAttributeTypes.Fire;
                    data.Type = SkillTypes.Active;
                    data.Grade = GradeNames.Legendary;
                    data.CooldownType = CooldownTypes.TimeBased;
                    data.BaseCooldownTime = 12f;
                    data.CooldownTimeByLevel = 0f;
                    data.BaseManaCost = 45;
                    data.ManaCostByLevel = 0;
                    data.BaseEffectValue = 3330f;
                    data.EffectValueByLevel = 333f;
                    data.EffectMultiplier = 1.5f;
                    data.BaseHitCount = 1;
                    data.HitCountByLevel = 1;
                    data.BaseRange = 3f;
                    data.RangeByLevel = 0f;
                    data.HasTimeStop = true;
                    data.Description = "범위 3 이내의 적 모두에게 공격력의 n%×1.5 ×3로 1 • 2회 공격, 시전 중 시간 정지";
                    break;

                case SkillNames.Rage: // 분노 - 전설 등급
                    data.Attribute = SkillAttributeTypes.Fire;
                    data.Type = SkillTypes.Passive;
                    data.Grade = GradeNames.Legendary;
                    data.CooldownType = CooldownTypes.TimeBased;
                    data.BaseCooldownTime = 15f;
                    data.CooldownTimeByLevel = 0f;
                    data.BaseManaCost = 50;
                    data.ManaCostByLevel = 0;
                    data.BaseEffectValue = 2f;
                    data.EffectValueByLevel = 0.1f;
                    data.Description = "체력이 감소한 만큼 10초간 전체 공격력 증가. 체력 1%당 +n%, 지속되는 동안 HP회복이 멈춘다.";
                    break;

                case SkillNames.FirePillar: // 불기둥 - 신화 등급
                    data.Attribute = SkillAttributeTypes.Fire;
                    data.Type = SkillTypes.Active;
                    data.Grade = GradeNames.Mythic;
                    data.CooldownType = CooldownTypes.TimeBased;
                    data.BaseCooldownTime = 10f;
                    data.CooldownTimeByLevel = -2f;
                    data.BaseManaCost = 40;
                    data.ManaCostByLevel = 0;
                    data.BaseEffectValue = 700f;
                    data.EffectValueByLevel = 70f;
                    data.EffectMultiplier = 3f;
                    data.BaseHitCount = 3;
                    data.HitCountByLevel = 0;
                    data.Description = "점차 강해지는 연속의 불기둥 소환. 공격력의 n%×3 ×3의 데미지";
                    break;

                case SkillNames.WarriorBurn: // 워리어번 - 신화 등급
                    data.Attribute = SkillAttributeTypes.Fire;
                    data.Type = SkillTypes.Passive;
                    data.Grade = GradeNames.Mythic;
                    data.CooldownType = CooldownTypes.TimeBased;
                    data.BaseCooldownTime = 18f;
                    data.CooldownTimeByLevel = 0f;
                    data.BaseManaCost = 30;
                    data.ManaCostByLevel = 0;
                    data.BaseEffectValue = 250f;
                    data.EffectValueByLevel = 7f;
                    data.Description = "현재 체력의 50%를 소모하고 5초간 전체 공격력이 n% 증가한다.";
                    break;
            }
        }

        private void ApplyWaterSkillData(SkillAssetData data)
        {
            switch (data.Name)
            {
                // 물(Water) 속성
                case SkillNames.IceStone: // 아이스 스톤 - 일반 등급
                    data.Attribute = SkillAttributeTypes.Water;
                    data.Type = SkillTypes.Active;
                    data.Grade = GradeNames.Common;
                    data.CooldownType = CooldownTypes.TimeBased;
                    data.BaseCooldownTime = 5f;
                    data.CooldownTimeByLevel = 0f;
                    data.BaseManaCost = 20;
                    data.ManaCostByLevel = 0;
                    data.BaseEffectValue = 110f;
                    data.EffectValueByLevel = 11f;
                    data.EffectMultiplier = 1.5f;
                    data.BaseHitCount = 3;
                    data.HitCountByLevel = 0;
                    data.BaseRange = 5f;
                    data.RangeByLevel = 0f;
                    data.HasFreezeEffect = true;
                    data.FreezeChance = 0f;
                    data.Description = "범위 5 이내의 임의의 적들에게 n%×1.5 ×3 데미지의 얼음유성 소환. 빙결된 적에게 가하는 데미지 증가";
                    break;

                case SkillNames.ManaBlessing: // 마나의 축복 - 일반 등급
                    data.Attribute = SkillAttributeTypes.Water;
                    data.Type = SkillTypes.Passive;
                    data.Grade = GradeNames.Common;
                    data.CooldownType = CooldownTypes.TimeBased;
                    data.BaseCooldownTime = 8f;
                    data.CooldownTimeByLevel = 0f;
                    data.BaseManaCost = 0;
                    data.ManaCostByLevel = 0;
                    data.BaseEffectValue = 30f;
                    data.EffectValueByLevel = 2f;
                    data.Description = "마나 회복 +n%";
                    break;

                case SkillNames.IceShower: // 아이스 샤워 - 고급 등급
                    data.Attribute = SkillAttributeTypes.Water;
                    data.Type = SkillTypes.Active;
                    data.Grade = GradeNames.Grand;
                    data.CooldownType = CooldownTypes.TimeBased;
                    data.BaseCooldownTime = 7f;
                    data.CooldownTimeByLevel = 0f;
                    data.BaseManaCost = 25;
                    data.ManaCostByLevel = 0;
                    data.BaseEffectValue = 310f;
                    data.EffectValueByLevel = 31f;
                    data.EffectMultiplier = 1.5f;
                    data.BaseHitCount = 3;
                    data.HitCountByLevel = 0;
                    data.BaseRange = 8f;
                    data.RangeByLevel = 0f;
                    data.HasFreezeEffect = true;
                    data.FreezeChance = 0f;
                    data.Description = "범위 8 이내의 임의의 적들에게 n%×1.5 ×3 데미지의 얼음유성 소환. 빙결된 적에게 가하는 데미지 증가";
                    break;

                case SkillNames.WaveSlash: // 파도베기 - 레어 등급
                    data.Attribute = SkillAttributeTypes.Water;
                    data.Type = SkillTypes.Active;
                    data.Grade = GradeNames.Rare;
                    data.CooldownType = CooldownTypes.AttackCountBased;
                    data.BaseCooldownAttackCount = 12;
                    data.CooldownAttackCountByLevel = 0;
                    data.BaseManaCost = 25;
                    data.ManaCostByLevel = 0;
                    data.BaseEffectValue = 170f;
                    data.EffectValueByLevel = 17f;
                    data.EffectMultiplier = 3f;
                    data.BaseHitCount = 7;
                    data.HitCountByLevel = 0;
                    data.BaseRange = 3f;
                    data.RangeByLevel = 0f;
                    data.Description = "범위 3 이내의 적들에게 7연발 데미지 n%×3 ×3. 시전 시간 감소";
                    break;

                case SkillNames.FlowingBlade: // 흐르는 칼날 - 레어 등급
                    data.Attribute = SkillAttributeTypes.Water;
                    data.Type = SkillTypes.Passive;
                    data.Grade = GradeNames.Rare;
                    data.CooldownType = CooldownTypes.TimeBased;
                    data.BaseCooldownTime = 18f;
                    data.CooldownTimeByLevel = 0f;
                    data.BaseManaCost = 30;
                    data.ManaCostByLevel = 0;
                    data.BaseEffectValue = 100f;
                    data.EffectValueByLevel = 4f;
                    data.Description = "10초간 공격속도 +n%";
                    break;

                case SkillNames.WindingBlade: // 굽이치는 칼날 - 영웅 등급
                    data.Attribute = SkillAttributeTypes.Water;
                    data.Type = SkillTypes.Passive;
                    data.Grade = GradeNames.Epic;
                    data.CooldownType = CooldownTypes.TimeBased;
                    data.BaseCooldownTime = 4f;
                    data.CooldownTimeByLevel = 0f;
                    data.BaseManaCost = 0;
                    data.ManaCostByLevel = 0;
                    data.BaseEffectValue = 18f;
                    data.EffectValueByLevel = 0.9f;
                    data.Description = "전투 돌입 후, 4초마다 공격속도 +n% (최대 10회)";
                    break;

                case SkillNames.DancingWave: // 춤추는 파도 - 영웅 등급
                    data.Attribute = SkillAttributeTypes.Water;
                    data.Type = SkillTypes.Passive;
                    data.Grade = GradeNames.Epic;
                    data.CooldownType = CooldownTypes.TimeBased;
                    data.BaseCooldownTime = 15f;
                    data.CooldownTimeByLevel = 0f;
                    data.BaseManaCost = 40;
                    data.ManaCostByLevel = 0;
                    data.BaseEffectValue = 3.5f;
                    data.EffectValueByLevel = 0.1f;
                    data.Description = "n초간 적의 공격을 회피 (일부 특수 공격 예외)";
                    break;

                case SkillNames.IceTime: // 아이스 타임 - 전설 등급
                    data.Attribute = SkillAttributeTypes.Water;
                    data.Type = SkillTypes.Active;
                    data.Grade = GradeNames.Legendary;
                    data.CooldownType = CooldownTypes.TimeBased;
                    data.BaseCooldownTime = 9f;
                    data.CooldownTimeByLevel = 0f;
                    data.BaseManaCost = 35;
                    data.ManaCostByLevel = 0;
                    data.BaseEffectValue = 670f;
                    data.EffectValueByLevel = 67f;
                    data.EffectMultiplier = 1.5f;
                    data.BaseHitCount = 3;
                    data.HitCountByLevel = 0;
                    data.BaseRange = 10f;
                    data.RangeByLevel = 0f;
                    data.HasFreezeEffect = true;
                    data.FreezeChance = 0f;
                    data.Description = "범위 10 이내의 임의의 적들에게 n%×1.5 ×3 데미지의 얼음유성 소환. 빙결된 적에게 가하는 데미지 증가";
                    break;

                case SkillNames.Meditation: // 메디테이션 - 전설 등급
                    data.Attribute = SkillAttributeTypes.Water;
                    data.Type = SkillTypes.Other;
                    data.Grade = GradeNames.Legendary;
                    data.CooldownType = CooldownTypes.TimeBased;
                    data.BaseCooldownTime = 15f;
                    data.CooldownTimeByLevel = 0f;
                    data.BaseManaCost = 40;
                    data.ManaCostByLevel = 0;
                    data.BaseEffectValue = 30f;
                    data.EffectValueByLevel = 1f;
                    data.Description = "집중해서 모든 스킬의 대기시간과 필요공격수 충전 n%";
                    break;

                case SkillNames.Blizzard: // 블리자드 - 신화 등급
                    data.Attribute = SkillAttributeTypes.Water;
                    data.Type = SkillTypes.Active;
                    data.Grade = GradeNames.Mythic;
                    data.CooldownType = CooldownTypes.TimeBased;
                    data.BaseCooldownTime = 10f;
                    data.CooldownTimeByLevel = 0f;
                    data.BaseManaCost = 35;
                    data.ManaCostByLevel = 0;
                    data.BaseEffectValue = 650f;
                    data.EffectValueByLevel = 65f;
                    data.EffectMultiplier = 3f;
                    data.HasFreezeEffect = true;
                    data.FreezeChance = 1f;
                    data.Description = "초당 데미지 공격력의 n%×3. 전방의 모든 적 빙결 100%";
                    break;

                case SkillNames.Torrent: // 격류 - 신화 등급
                    data.Attribute = SkillAttributeTypes.Water;
                    data.Type = SkillTypes.Passive;
                    data.Grade = GradeNames.Mythic;
                    data.CooldownType = CooldownTypes.TimeBased;
                    data.BaseCooldownTime = 50f;
                    data.CooldownTimeByLevel = 0f;
                    data.BaseManaCost = 0;
                    data.ManaCostByLevel = 0;
                    data.BaseEffectValue = 350f;
                    data.EffectValueByLevel = 10f;
                    data.Description = "전투돌입 2초 뒤 6초간 공격속도 증가 n%";
                    break;
            }
        }

        private void ApplyWindSkillData(SkillAssetData data)
        {
            switch (data.Name)
            {  // 바람(Wind) 속성
                case SkillNames.LightningSlash: // 번개 베기 - 일반 등급
                    data.Attribute = SkillAttributeTypes.Wind;
                    data.Type = SkillTypes.Active;
                    data.Grade = GradeNames.Common;
                    data.CooldownType = CooldownTypes.AttackCountBased;
                    data.BaseCooldownAttackCount = 6;
                    data.CooldownAttackCountByLevel = 0;
                    data.BaseManaCost = 15;
                    data.ManaCostByLevel = 0;
                    data.BaseEffectValue = 150f;
                    data.EffectValueByLevel = 15f;
                    data.EffectMultiplier = 3f;
                    data.BaseHitCount = 3;
                    data.HitCountByLevel = 0;
                    data.BaseRange = 5f;
                    data.RangeByLevel = 4f;
                    data.Description = "범위 5 • 9 이내의 적 5 • 9기에게 n%×3 ×3의 데미지를 입힌다.";
                    break;

                case SkillNames.ThunderStrike: // 뇌격 - 일반 등급
                    data.Attribute = SkillAttributeTypes.Wind;
                    data.Type = SkillTypes.Active;
                    data.Grade = GradeNames.Common;
                    data.CooldownType = CooldownTypes.AttackCountBased;
                    data.BaseCooldownAttackCount = 10;
                    data.CooldownAttackCountByLevel = 0;
                    data.BaseManaCost = 20;
                    data.ManaCostByLevel = 0;
                    data.BaseEffectValue = 80f;
                    data.EffectValueByLevel = 8f;
                    data.EffectMultiplier = 1.5f;
                    data.BaseHitCount = 3;
                    data.HitCountByLevel = 0;
                    data.BaseRange = 15f;
                    data.RangeByLevel = 0f;
                    data.Description = "15 범위 이내의 장소에 임의로 n%×1.5 ×3 데미지의 번개 30 • 60회 시전. 번개의 피격 범위 증가";
                    break;

                case SkillNames.HighSpeedMovement: // 고속이동 - 고급 등급
                    data.Attribute = SkillAttributeTypes.Wind;
                    data.Type = SkillTypes.Passive;
                    data.Grade = GradeNames.Grand;
                    data.CooldownType = CooldownTypes.TimeBased;
                    data.BaseCooldownTime = 20f;
                    data.CooldownTimeByLevel = 0f;
                    data.BaseManaCost = 30;
                    data.ManaCostByLevel = 0;
                    data.BaseEffectValue = 30f;
                    data.EffectValueByLevel = 3f;
                    data.Description = "10초간 이동속도 +n%";
                    break;

                case SkillNames.ThunderSlash: // 천둥 베기 - 레어 등급
                    data.Attribute = SkillAttributeTypes.Wind;
                    data.Type = SkillTypes.Active;
                    data.Grade = GradeNames.Rare;
                    data.CooldownType = CooldownTypes.AttackCountBased;
                    data.BaseCooldownAttackCount = 7;
                    data.CooldownAttackCountByLevel = 0;
                    data.BaseManaCost = 15;
                    data.ManaCostByLevel = 0;
                    data.BaseEffectValue = 500f;
                    data.EffectValueByLevel = 50f;
                    data.EffectMultiplier = 3f;
                    data.BaseHitCount = 3;
                    data.HitCountByLevel = 0;
                    data.BaseRange = 7f;
                    data.RangeByLevel = 5f;
                    data.Description = "범위 7 • 12 이내의 적 7 • 12기에게 n%×3 ×3의 데미지를 입힌다.";
                    break;

                case SkillNames.AccelerationSword: // 가속의 검 - 레어 등급
                    data.Attribute = SkillAttributeTypes.Wind;
                    data.Type = SkillTypes.Passive;
                    data.Grade = GradeNames.Rare;
                    data.CooldownType = CooldownTypes.AttackCountBased;
                    data.BaseCooldownAttackCount = 5;
                    data.CooldownAttackCountByLevel = 0;
                    data.BaseManaCost = 0;
                    data.ManaCostByLevel = 0;
                    data.BaseEffectValue = 7f;
                    data.EffectValueByLevel = 0.35f;
                    data.Description = "전투 돌입 후, 5회 공격마다 공격속도 +n% (최대 20회)";
                    break;

                case SkillNames.LightningFast: // 전광석화 - 영웅 등급
                    data.Attribute = SkillAttributeTypes.Wind;
                    data.Type = SkillTypes.Active;
                    data.Grade = GradeNames.Epic;
                    data.CooldownType = CooldownTypes.TimeBased;
                    data.BaseCooldownTime = 0.5f;
                    data.CooldownTimeByLevel = 0f;
                    data.BaseManaCost = 10;
                    data.ManaCostByLevel = -5;
                    data.BaseEffectValue = 80f;
                    data.EffectValueByLevel = 8f;
                    data.EffectMultiplier = 3f;
                    data.BaseHitCount = 1;
                    data.HitCountByLevel = 0;
                    data.BaseRange = 3f;
                    data.RangeByLevel = 0f;
                    data.Description = "빠르게 돌진하여 범위 3이내의 적 모두에게 공격력의 n%×3 ×3로 1회 공격";
                    break;

                case SkillNames.WindSword: // 바람의 검 - 영웅 등급
                    data.Attribute = SkillAttributeTypes.Wind;
                    data.Type = SkillTypes.Passive;
                    data.Grade = GradeNames.Epic;
                    data.CooldownType = CooldownTypes.None;
                    data.BaseManaCost = 0;
                    data.ManaCostByLevel = 0;
                    data.BaseEffectValue = 40f;
                    data.EffectValueByLevel = 2f;
                    data.Description = "공격속도 +n%";
                    break;

                case SkillNames.AsuraLightningSlash: // 수라 번개 베기 - 전설 등급
                    data.Attribute = SkillAttributeTypes.Wind;
                    data.Type = SkillTypes.Active;
                    data.Grade = GradeNames.Legendary;
                    data.CooldownType = CooldownTypes.AttackCountBased;
                    data.BaseCooldownAttackCount = 8;
                    data.CooldownAttackCountByLevel = 0;
                    data.BaseManaCost = 15;
                    data.ManaCostByLevel = 0;
                    data.BaseEffectValue = 860f;
                    data.EffectValueByLevel = 86f;
                    data.EffectMultiplier = 3f;
                    data.BaseHitCount = 3;
                    data.HitCountByLevel = 0;
                    data.BaseRange = 10f;
                    data.RangeByLevel = 2f;
                    data.Description = "범위 10 • 12 이내의 적 10 • 12기에게 n%×3 ×3의 데미지를 입힌다.";
                    break;

                case SkillNames.RedThunder: // 적뢰 - 전설 등급
                    data.Attribute = SkillAttributeTypes.Wind;
                    data.Type = SkillTypes.Active;
                    data.Grade = GradeNames.Legendary;
                    data.CooldownType = CooldownTypes.AttackCountBased;
                    data.BaseCooldownAttackCount = 12;
                    data.CooldownAttackCountByLevel = 0;
                    data.BaseManaCost = 25;
                    data.ManaCostByLevel = 0;
                    data.BaseEffectValue = 170f;
                    data.EffectValueByLevel = 17f;
                    data.EffectMultiplier = 1.5f;
                    data.BaseHitCount = 3;
                    data.HitCountByLevel = 0;
                    data.BaseRange = 20f;
                    data.RangeByLevel = 0f;
                    data.Description = "20 범위 이내의 장소에 임의로 n%×1.5 ×3 데미지의 번개 60 • 120회 시전. 번개의 피격 범위 증가";
                    break;

                case SkillNames.Swiftness: // 신속 - 신화 등급
                    data.Attribute = SkillAttributeTypes.Wind;
                    data.Type = SkillTypes.Active;
                    data.Grade = GradeNames.Mythic;
                    data.CooldownType = CooldownTypes.TimeBased;
                    data.BaseCooldownTime = 5f;
                    data.CooldownTimeByLevel = 0f;
                    data.BaseManaCost = 20;
                    data.ManaCostByLevel = 0;
                    data.BaseEffectValue = 260f;
                    data.EffectValueByLevel = 26f;
                    data.EffectMultiplier = 3f;
                    data.BaseHitCount = 6;
                    data.HitCountByLevel = 0;
                    data.BaseRange = 7f;
                    data.RangeByLevel = 0f;
                    data.Description = "돌격하며 범위 7 이내의 적들에게 공격력의 n%×3 ×3 데미지를 입힌다. 6연속 돌진 공격. 일반 몬스터에게 가하는 데미지 증가";
                    break;

                case SkillNames.ThunderGod: // 뇌신 - 신화 등급
                    data.Attribute = SkillAttributeTypes.Wind;
                    data.Type = SkillTypes.Passive;
                    data.Grade = GradeNames.Mythic;
                    data.CooldownType = CooldownTypes.TimeBased;
                    data.BaseCooldownTime = 20f;
                    data.CooldownTimeByLevel = 0f;
                    data.BaseManaCost = 30;
                    data.ManaCostByLevel = 0;
                    data.BaseEffectValue = 250f;
                    data.EffectValueByLevel = 6f;
                    data.Description = "현재 체력의 50%를 소모하고 5초 간 공격속도가 n% 증가한다.";
                    break;
            }
        }

        private void ApplyEarthSkillData(SkillAssetData data)
        {
            switch (data.Name)
            {
                case SkillNames.StoneStrike: // 스톤 스트라이크 - 일반 등급
                    data.Attribute = SkillAttributeTypes.Earth;
                    data.Type = SkillTypes.Active;
                    data.Grade = GradeNames.Common;
                    data.CooldownType = CooldownTypes.AttackCountBased;
                    data.BaseCooldownAttackCount = 9;
                    data.CooldownAttackCountByLevel = 0;
                    data.BaseManaCost = 20;
                    data.ManaCostByLevel = 0;
                    data.BaseEffectValue = 610f;
                    data.EffectValueByLevel = 61f;
                    data.EffectMultiplier = 3f;
                    data.BaseHitCount = 1;
                    data.HitCountByLevel = 0;
                    data.BaseRange = 1f;
                    data.RangeByLevel = 0f;
                    data.Description = "범위 1 이내의 적에게 땅 속성 데미지 n%×3 ×3로 1회 공격. 보스에게 가하는 데미지 증가";
                    break;

                case SkillNames.EarthBlessing: // 대지의 축복 - 일반 등급
                    data.Attribute = SkillAttributeTypes.Earth;
                    data.Type = SkillTypes.Passive;
                    data.Grade = GradeNames.Common;
                    data.CooldownType = CooldownTypes.TimeBased;
                    data.BaseCooldownTime = 7f;
                    data.CooldownTimeByLevel = 0f;
                    data.BaseManaCost = 30;
                    data.ManaCostByLevel = 0;
                    data.BaseEffectValue = 50f;
                    data.EffectValueByLevel = 2f;
                    data.Description = "체력회복 n%";
                    break;

                case SkillNames.PowerStrike: // 파워 스트라이크 - 고급 등급
                    data.Attribute = SkillAttributeTypes.Earth;
                    data.Type = SkillTypes.Active;
                    data.Grade = GradeNames.Grand;
                    data.CooldownType = CooldownTypes.AttackCountBased;
                    data.BaseCooldownAttackCount = 12;
                    data.CooldownAttackCountByLevel = 0;
                    data.BaseManaCost = 25;
                    data.ManaCostByLevel = 0;
                    data.BaseEffectValue = 660f;
                    data.EffectValueByLevel = 66f;
                    data.EffectMultiplier = 3f;
                    data.BaseHitCount = 2;
                    data.HitCountByLevel = 0;
                    data.BaseRange = 1f;
                    data.RangeByLevel = 0f;
                    data.Description = "범위 1 이내의 적에게 땅 속성 데미지 n%×3 ×3로 2회 공격. 보스에게 가하는 데미지 증가";
                    break;

                case SkillNames.PowerImpact: // 파워 임팩트 - 고급 등급
                    data.Attribute = SkillAttributeTypes.Earth;
                    data.Type = SkillTypes.Active;
                    data.Grade = GradeNames.Grand;
                    data.CooldownType = CooldownTypes.TimeBased;
                    data.BaseCooldownTime = 10f;
                    data.CooldownTimeByLevel = 0f;
                    data.BaseManaCost = 40;
                    data.ManaCostByLevel = 0;
                    data.BaseEffectValue = 1060f;
                    data.EffectValueByLevel = 106f;
                    data.EffectMultiplier = 3f;
                    data.BaseHitCount = 3;
                    data.HitCountByLevel = 0;
                    data.BaseRange = 5f;
                    data.RangeByLevel = 2f;
                    data.IsPiercing = true;
                    data.HasStunEffect = true;
                    data.StunChance = 0.5f;
                    data.Description = "범위 5 • 7 이내의 적을 관통하는 땅 속성 데미지 n%×3 ×3로 공격. 추가 스턴 50%";
                    break;

                case SkillNames.EarthWill: // 땅의 의지 - 레어 등급
                    data.Attribute = SkillAttributeTypes.Earth;
                    data.Type = SkillTypes.Passive;
                    data.Grade = GradeNames.Rare;
                    data.CooldownType = CooldownTypes.AttackCountBased;
                    data.BaseCooldownAttackCount = 7;
                    data.CooldownAttackCountByLevel = 0;
                    data.BaseManaCost = 0;
                    data.ManaCostByLevel = 0;
                    data.BaseEffectValue = 8f;
                    data.EffectValueByLevel = 0.4f;
                    data.Description = "전투 돌입 후, 공격 7회 마다 전체 공격력 +n% (최대 20회)";
                    break;

                case SkillNames.SteelWill: // 강철의 의지 - 영웅 등급
                    data.Attribute = SkillAttributeTypes.Earth;
                    data.Type = SkillTypes.Passive;
                    data.Grade = GradeNames.Epic;
                    data.CooldownType = CooldownTypes.None;
                    data.BaseManaCost = 0;
                    data.ManaCostByLevel = 0;
                    data.BaseEffectValue = 50f;
                    data.EffectValueByLevel = 2.5f;
                    data.Description = "전체 공격력 +n%";
                    break;

                case SkillNames.LifeMana: // 라이프 마나 - 영웅 등급
                    data.Attribute = SkillAttributeTypes.Earth;
                    data.Type = SkillTypes.Passive;
                    data.Grade = GradeNames.Epic;
                    data.CooldownType = CooldownTypes.AttackCountBased;
                    data.BaseCooldownAttackCount = 20;
                    data.CooldownAttackCountByLevel = 0;
                    data.BaseManaCost = 0;
                    data.ManaCostByLevel = 0;
                    data.BaseEffectValue = 30f;
                    data.EffectValueByLevel = 2f;
                    data.Description = "전체 체력의 n% 회복. 마나 30% 회복";
                    break;

                case SkillNames.GigaStrike: // 기가 스트라이크 - 전설 등급
                    data.Attribute = SkillAttributeTypes.Earth;
                    data.Type = SkillTypes.Active;
                    data.Grade = GradeNames.Legendary;
                    data.CooldownType = CooldownTypes.AttackCountBased;
                    data.BaseCooldownAttackCount = 15;
                    data.CooldownAttackCountByLevel = 0;
                    data.BaseManaCost = 30;
                    data.ManaCostByLevel = 0;
                    data.BaseEffectValue = 750f;
                    data.EffectValueByLevel = 75f;
                    data.EffectMultiplier = 3f;
                    data.BaseHitCount = 3;
                    data.HitCountByLevel = 0;
                    data.BaseRange = 1f;
                    data.RangeByLevel = 0f;
                    data.Description = "범위 1 이내의 적에게 땅 속성 데미지 n%×3 ×3로 3회 공격. 보스에게 가하는 데미지 증가";
                    break;

                case SkillNames.GigaImpact: // 기가 임팩트 - 전설 등급
                    data.Attribute = SkillAttributeTypes.Earth;
                    data.Type = SkillTypes.Active;
                    data.Grade = GradeNames.Legendary;
                    data.CooldownType = CooldownTypes.TimeBased;
                    data.BaseCooldownTime = 10f;
                    data.CooldownTimeByLevel = 0f;
                    data.BaseManaCost = 40;
                    data.ManaCostByLevel = 0;
                    data.BaseEffectValue = 2580f;
                    data.EffectValueByLevel = 258f;
                    data.EffectMultiplier = 3f;
                    data.BaseHitCount = 3;
                    data.HitCountByLevel = 0;
                    data.BaseRange = 5f;
                    data.RangeByLevel = 2f;
                    data.IsPiercing = true;
                    data.HasStunEffect = true;
                    data.StunChance = 1f;
                    data.Description = "범위 5 • 7 이내의 적을 관통하는 땅 속성 데미지 n%×3 ×3로 공격. 추가 스턴 100%";
                    break;

                case SkillNames.BeastHunt: // 마수 사냥 - 신화 등급
                    data.Attribute = SkillAttributeTypes.Earth;
                    data.Type = SkillTypes.Active;
                    data.Grade = GradeNames.Mythic;
                    data.CooldownType = CooldownTypes.AttackCountBased;
                    data.BaseCooldownAttackCount = 33;
                    data.CooldownAttackCountByLevel = 0;
                    data.BaseManaCost = 55;
                    data.ManaCostByLevel = 0;
                    data.BaseEffectValue = 1000f;
                    data.EffectValueByLevel = 100f;
                    data.EffectMultiplier = 3f;
                    data.BaseHitCount = 7;
                    data.HitCountByLevel = 0;
                    data.BaseRange = 4f;
                    data.RangeByLevel = 0f;
                    data.Description = "범위 4 이내의 적을 7회 공격. 1회 마다 데미지 n%×3. 7회째에는 훨씬 강한 데미지(6배)를 입힌다.";
                    break;

                case SkillNames.SuperhumanStrength: // 괴력난신 - 신화 등급
                    data.Attribute = SkillAttributeTypes.Earth;
                    data.Type = SkillTypes.Passive;
                    data.Grade = GradeNames.Mythic;
                    data.CooldownType = CooldownTypes.TimeBased;
                    data.BaseCooldownTime = 30f;
                    data.CooldownTimeByLevel = 0f;
                    data.BaseManaCost = 0;
                    data.ManaCostByLevel = 0;
                    data.BaseEffectValue = 500f;
                    data.EffectValueByLevel = 25f;
                    data.Description = "전투 돌입 20초 뒤 5초간 전체 공격력 증가 n%";
                    break;
            }
        }

#endif
    }
}
