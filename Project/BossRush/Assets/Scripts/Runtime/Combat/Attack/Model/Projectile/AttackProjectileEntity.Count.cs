

namespace TeamSuneat
{
    public partial class AttackProjectileEntity : AttackEntity
    {
        private void RegisterStatEventOfBaseProjectile()
        {
            if (OwnerStat != null)
            {
                if (StatNameOfExtraShots != StatNames.None)
                {
                    OwnerStat.RegisterRefreshEvent(OnRefreshStatProjectileCountExtraShots);
                }
                if (StatNameOfPerShot != StatNames.None)
                {
                    OwnerStat.RegisterRefreshEvent(OnRefreshStatProjectileCountPerShot);
                }
            }
        }

        private void UnregisterStatEventOfBaseProjectile()
        {
            if (OwnerStat != null)
            {
                if (StatNameOfExtraShots != StatNames.None)
                {
                    OwnerStat.UnregisterRefreshEvent(OnRefreshStatProjectileCountExtraShots);
                }
                if (StatNameOfPerShot != StatNames.None)
                {
                    OwnerStat.UnregisterRefreshEvent(OnRefreshStatProjectileCountPerShot);
                }
            }
        }

        private void RefreshBaseProjectilePerShot()
        {
            _baseProjectilesPerShot = ProjectilesPerShot;

            if (StatNameOfExtraShots != StatNames.None)
            {
                SetBaseProjectilePerShotAddedExtraShot();
            }
            else if (StatNameOfPerShot != StatNames.None)
            {
                SetBaseProjectilePerShotToStat();
            }

            LogProgress("기본 발사 갯수가 지정됩니다. : {0}", _baseProjectilesPerShot);
        }

        private void SetBaseProjectilePerShotAddedExtraShot()
        {
            if (OwnerStat != null)
            {
                int statValue = OwnerStat.FindValueOrDefaultToInt(StatNameOfExtraShots);
                if (statValue != 0)
                {
                    _baseProjectilesPerShot = ProjectilesPerShot + statValue;

                    LogInfo("능력치({0})에 따른 추가 발사 갯수를 적용한 기본 발사 갯수를 설정합니다. : {1}({2}+{3})",
                        StatNameOfExtraShots.ToLogString(), _baseProjectilesPerShot.ToSelectString(), ProjectilesPerShot, statValue.ToSelectString());
                }
            }
        }

        private void SetBaseProjectilePerShotToStat()
        {
            if (OwnerStat != null)
            {
                _baseProjectilesPerShot = OwnerStat.FindValueOrDefaultToInt(StatNameOfPerShot);
                if (_baseProjectilesPerShot != 0)
                {
                    LogInfo("능력치({0})에 따른 기본 발사 갯수를 설정합니다. : {1}",
                        StatNameOfPerShot.ToLogString(), _baseProjectilesPerShot.ToSelectString());
                }
            }
        }
    }
}