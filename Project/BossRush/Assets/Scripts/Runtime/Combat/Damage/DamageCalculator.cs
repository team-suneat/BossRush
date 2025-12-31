using System.Collections.Generic;
using System.Text;
using TeamSuneat.Data;

namespace TeamSuneat
{
    [System.Serializable]
    public partial class DamageCalculator
    {
#if UNITY_EDITOR
        private readonly StringBuilder _stringBuilder = new();
#endif
        private Vital _targetVital;

        // StringBuilder 헬퍼 메서드들

        private void AppendToLog(string value)
        {
#if UNITY_EDITOR
            _stringBuilder.Append(value);
#endif
        }

        private void AppendLineToLog(string value = "")
        {
#if UNITY_EDITOR
            _stringBuilder.AppendLine(value);
#endif
        }

        private void ClearLogBuilder()
        {
#if UNITY_EDITOR
            _stringBuilder.Clear();
#endif
        }

        private void InsertToLog(int index, string value)
        {
#if UNITY_EDITOR
            _stringBuilder.Insert(index, value);
#endif
        }

        private string GetLogString()
        {
#if UNITY_EDITOR
            return _stringBuilder.ToString();
#else
            return string.Empty;
#endif
        }

        public Vital TargetVital => _targetVital;
        public Character TargetCharacter => _targetVital?.Owner;

        public HitmarkAssetData HitmarkAssetData { get; set; }
        public Character Attacker { get; private set; }
        public AttackEntity AttackEntity { get; set; }
        public List<DamageResult> DamageResults { get; private set; }
        public float ReferenceValue { get; set; }
        public float DamageReferenceValue { get; private set; }
        public float ManaCostReferenceValue { get; private set; }
        public float CooldownReferenceValue { get; private set; }
        public float DiminishingReturns { get; private set; }
        public int Level { get; private set; } = 1;
        public int Stack { get; set; }
        public int Tick { get; set; }
        public float? WeaponDamageOverride { get; set; }

        //─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────

        private bool IsComputeValid()
        {
            if (!HitmarkAssetData.IsValid())
            {
                LogErrorHitmarkNotSet();
                return false;
            }

            return true;
        }

        public void Execute()
        {
            if (!IsComputeValid())
            {
                return;
            }

            ResetDamageResults();
            DetermineImmuneCC(HitmarkAssetData.IsCrowdControl);

            HitmarkAssetData damageAsset = HitmarkAssetData;
            DamageResult damageResult = CreateDamageResult(damageAsset);

            RefreshReferenceValue(damageAsset);
            ComputeByType(damageAsset, ref damageResult);

            // 치명타 이벤트 발송
            if (damageResult.IsCritical)
            {
                GlobalEvent<DamageResult>.Send(GlobalEventType.PLAYER_CHARACTER_ATTACK_MONSTER_CRITICAL, damageResult);
            }

            // 회심의 일격 이벤트 발송
            if (damageResult.IsDevastatingStrike)
            {
                GlobalEvent<DamageResult>.Send(GlobalEventType.PLAYER_CHARACTER_ATTACK_MONSTER_DEVASTATING_STRIKE, damageResult);
            }

            DamageResults.Add(damageResult);
        }

        public void ExecuteWithoutCritical()
        {
            if (!IsComputeValid())
            {
                return;
            }

            ResetDamageResults();

            HitmarkAssetData damageAsset = HitmarkAssetData;
            DamageResult damageResult = CreateDamageResult(damageAsset);
            RefreshReferenceValue(damageAsset);
            ComputeByType(damageAsset, ref damageResult);
            DamageResults.Add(damageResult);
        }

        private DamageResult CreateDamageResult(HitmarkAssetData damageAsset)
        {
            return new DamageResult
            {
                IsEvasion = false,
                Asset = damageAsset,
                HitmarkLevel = Level,
                Attacker = Attacker,
                AttackEntity = AttackEntity,
                TargetVital = TargetVital
            };
        }

        private void ResetDamageResults()
        {
            if (DamageResults == null)
            {
                DamageResults = new();
            }
            else
            {
                DamageResults.Clear();
            }

            LogProgressResetDamageResults();
        }

        public void SetAttacker(Character attacker)
        {
            Attacker = attacker;
            if (attacker != null)
            {
                LogProgressAttacker(attacker.GetHierarchyName());
            }
        }

        public void SetTargetVital(Vital targetVital)
        {
            if (targetVital != null)
            {
                _targetVital = targetVital;
                LogProgressTargetVital(_targetVital.GetHierarchyPath());
            }
            else
            {
                _targetVital = null;
                LogWarningTargetVital();
            }
        }

        public void SetResourceCostReferenceValue(int manaCost)
        {
            ManaCostReferenceValue = manaCost;
            LogProgressManaCostReferenceValue(ManaCostReferenceValue.ToSelectString(0));
        }

        public void SetDamageReferenceValue(int damageValue)
        {
            DamageReferenceValue = damageValue;
            LogProgressDamageReferenceValue(DamageReferenceValue.ToSelectString(0));
        }

        public void SetCooldownReferenceValue(float cooldownTime)
        {
            CooldownReferenceValue = cooldownTime;
            LogProgressCooldownReferenceValue(CooldownReferenceValue.ToSelectString(0));
        }

        #region Stack & Level

        public void SetStack(int stack)
        {
            Stack = stack;
        }

        public void SetLevel(int level)
        {
            Level = level;
        }

        #endregion Stack & Level

        // Floaty Text

        private void SpawnFloatyText(string label)
        {
            if (TargetVital != null)
            {
                string content = Data.JsonDataManager.FindStringClone(label);
                ResourcesManager.SpawnFloatyText(content, true, TargetVital.transform);
            }
        }
    }
}