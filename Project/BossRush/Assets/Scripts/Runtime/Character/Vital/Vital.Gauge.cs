using TeamSuneat.Setting;
using TeamSuneat.UserInterface;

namespace TeamSuneat
{
    public partial class Vital : Entity
    {
        public virtual void SetHUD()
        {
            RefreshHealthGauge();
            RefreshResourceGauge();
            RefreshShieldGauge();
        }

        /// <summary> 생명력 게이지가 할당되어있다면 갱신합니다. </summary>
        public virtual void RefreshHealthGauge()
        {
            if (Health == null)
            {
                return;
            }

            if (EnemyGauge != null)
            {
                EnemyGauge.SetHealth(Health);
            }
            else if (PlayerGauge != null)
            {
                PlayerGauge.SetHealth(Health);
            }
        }

        public virtual void RefreshResourceGauge()
        {
            if (PlayerGauge != null)
            {
                PlayerGauge.SetResource(Mana);
            }
        }

        public virtual void RefreshShieldGauge()
        {
            if (PlayerGauge != null)
            {
                PlayerGauge.SetShield(Shield);
            }
        }

        //

        public void SpawnCharacterGauge()
        {
            if (!UseGauge) { return; }
            if (!IsAlive) { return; }
            if (Owner == null) { return; }

            if (UIManager.Instance == null || UIManager.Instance.GaugeManager == null) { return; }
            if (UIManager.Instance.GaugeManager.FindCharacter(this) != null) { return; }

            if (Owner.IsPlayer)
            {
                SpawnPlayerGauge();
            }
            else
            {
                SpawnEnemyGauge();
            }
        }

        private void SpawnPlayerGauge()
        {
            UIPlayerGauge view = UIManager.Instance.GaugeManager.SpawnPlayerGauge(Owner);
            if (view != null)
            {
                view.Bind(Owner);
                Log.Info(LogTags.UI_Gauge, "플레이어 게이지를 생성하여 바이탈에 바인드합니다. {0}, {1}",
                    view.GetHierarchyName(), this.GetHierarchyPath());
            }
        }

        private void SpawnEnemyGauge()
        {
            if (!GameSetting.Instance.Play.UseMonsterGauge) { return; }

            UIEnemyGauge view = UIManager.Instance.GaugeManager.SpawnEnemyGauge(Owner);
            if (view != null)
            {
                view.Bind(Owner);
                Log.Info(LogTags.UI_Gauge, "몬스터 게이지를 생성하여 바이탈에 바인드합니다. {0}, {1}",
                    view.GetHierarchyName(), this.GetHierarchyPath());
            }
        }

        //

        public virtual void ConsumeHealthPotion(int healValue, int healValueOverTime, float duration)
        { }
    }
}