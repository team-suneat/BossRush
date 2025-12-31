using TeamSuneat.UserInterface;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace TeamSuneat
{
    public partial class Vital : Entity
    {
        [FoldoutGroup("#Vital-Components")]
        public Character Owner;

        [FoldoutGroup("#Vital-Components")]
        public Transform GaugePoint;

        //──────────────────────────────────────────────────────────────────────────────────────────────────────

        [FoldoutGroup("#Vital-Battle Resource")]
        public VitalResourceTypes ResourceType;

        [FoldoutGroup("#Vital-Battle Resource")]
        public Health Health;

        [FoldoutGroup("#Vital-Battle Resource")]
        public Shield Shield;

        [FoldoutGroup("#Vital-Battle Resource")]
        public Mana Mana;

        //──────────────────────────────────────────────────────────────────────────────────────────────────────
        [FoldoutGroup("#Vital-Gauge")]
        public bool UseGauge;

        [FoldoutGroup("#Vital-Gauge")]
        public bool UseSpawnGaugeOnInit;

        [FoldoutGroup("#Vital-Gauge")]
        [ReadOnly]
        public UIEnemyGauge EnemyGauge
        {
            get
            {
                if (UIManager.Instance != null)
                {
                    ICharacterGaugeView view = UIManager.Instance.GaugeManager.FindCharacter(this);
                    return view as UIEnemyGauge;
                }

                return null;
            }
        }

        [FoldoutGroup("#Vital-Gauge")]
        [ReadOnly]
        public UIPlayerGauge PlayerGauge
        {
            get
            {
                if (UIManager.Instance != null)
                {
                    ICharacterGaugeView view = UIManager.Instance.GaugeManager.FindCharacter(this);
                    return view as UIPlayerGauge;
                }

                return null;
            }
        }


        //──────────────────────────────────────────────────────────────────────────────────────────────────────

        [FoldoutGroup("#Vital-Event")]
        public UnityEvent DieEvent; // 파괴 가능한 오브젝트에서 사용
    }
}