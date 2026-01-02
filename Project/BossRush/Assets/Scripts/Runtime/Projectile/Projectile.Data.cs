using Lean.Pool;

using TeamSuneat.Data;

using UnityEngine;

namespace TeamSuneat
{
    public partial class Projectile
    {
        public float DefaultMoveSpeed
        {
            get
            {
                if (_projectileData != null)
                {
                    return _projectileData.Speed;
                }
                return 1f;
            }
        }

        public float MaxMoveDistance
        {
            get
            {
                if (false == Mathf.Approximately(_projectileData.Distance, 0f))
                {
                    return _projectileData.Distance;
                }
                else
                {
                    return _projectileData.Speed * _projectileData.HealthTime;
                }
            }
        }

        public bool IsThrough
        {
            get
            {
                if (_projectileData != null)
                {
                    return _projectileData.IsThrough;
                }
                return false;
            }
        }

        public HitmarkNames AttackHitmark
        {
            get
            {
                if (_projectileData != null)
                {
                    return _projectileData.Hitmark;
                }

                return HitmarkNames.None;
            }
        }

        public HitmarkNames AnotherHitmark
        {
            get
            {
                if (_projectileData != null)
                {
                    return _projectileData.AnotherHitmark;
                }

                return HitmarkNames.None;
            }
        }

        public HitmarkNames ReturnHitmark
        {
            get
            {
                if (_projectileData != null)
                {
                    return _projectileData.ReturnHitmark;
                }

                return HitmarkNames.None;
            }
        }

        public void LoadProjectileData()
        {
            _projectileData = ScriptableDataManager.Instance.FindProjectileClone(Name);

            _useLiftTime = false == _projectileData.HealthTime.IsZero();

            if (_projectileData == null || _projectileData.Name == ProjectileNames.None)
            {
                Log.Error("Failed to load projectile data. {0}, {1}", Name.ToLogString(), this.GetHierarchyPath());
            }
        }

        private void SetLayers()
        {
            if (_projectileData != null)
            {
                m_targetLayer = LayerEx.ConcatMask(_projectileData.DamageLayer);

                m_eraseLayer = LayerEx.ConcatMask(_projectileData.EraseLayer);

                m_collisionLayer = LayerEx.ConcatMask(_projectileData.CollisionLayer);
            }
        }

        public float CurrentMoveSpeed
        {
            get; private set;
        }

        public void RefreshMoveSpeed()
        {
            float additionalSpeed = 0f;

            if (false == Mathf.Approximately(0f, _projectileData.Acceleration))
            {
                additionalSpeed = _projectileData.Acceleration * ElapsedTime;
            }

            if (Level > 1)
            {
                for (int i = 0; i < _projectileData.AddSpeed.Length; i++)
                {
                    additionalSpeed += _projectileData.AddSpeed[i];
                }
            }

            CurrentMoveSpeed = DefaultMoveSpeed + additionalSpeed;
        }

        #region Additional HealthTime

        private float m_additionalHealthTime;

        public float AdditionalHealthTime => m_additionalHealthTime;

        private void SetAddtionalHealthTime()
        {
            m_additionalHealthTime = 0;

            if (_projectileData.Name != ProjectileNames.None && Level > 1)
            {
                m_additionalHealthTime += _projectileData.AddHealthTime * (Level - 1);
            }

            if (false == Mathf.Approximately(0f, m_additionalHealthTime))
            {
                Log.Info(LogTags.Projectile, "발사체({0}) 추가 지속시간 변경: {1}, Level:{2}", Name.ToLogString(), m_additionalHealthTime, Level);
            }
        }

        #endregion Additional HealthTime

        #region Additional Distance

        private float m_addtionalDistance;

        public float AdditionalDistance => m_addtionalDistance;

        private void SetAddtionalDistance()
        {
            m_addtionalDistance = 0;

            if (false == Mathf.Approximately(0f, m_addtionalDistance))
            {
                Log.Info(LogTags.Projectile, "Projectile({0})`s Additional Health Time : {1}, Level:{2}", Name.ToLogString(), m_addtionalDistance, Level);
            }
        }

        #endregion Additional Distance
    }
}