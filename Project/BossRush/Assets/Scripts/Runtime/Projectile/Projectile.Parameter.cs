using Lean.Pool;
using UnityEngine;

namespace TeamSuneat
{
    public partial class Projectile
    {
        public Data.ProjectileAssetData Data => _projectileData;

        public ProjectileTypes ProjectileType
        {
            get
            {
                if (_projectileData != null)
                {
                    return _projectileData.Type;
                }

                return ProjectileTypes.None;
            }
        }

        public float ColliderHeight
        {
            get
            {
                if (m_boxCollider2D != null)
                {
                    return m_boxCollider2D.size.y * 0.5f;
                }
                else if (m_circleCollider2D != null)
                {
                    return m_circleCollider2D.radius;
                }

                return 0f;
            }
        }

        public bool FacingRightAtLaunch => _facingRightAtLaunch;

        public float MoveDistance => Vector2.Distance(_spawnPosition, _destroyPosition);

        public float ElapsedTime => Time.time - _spawnTime;
    }
}