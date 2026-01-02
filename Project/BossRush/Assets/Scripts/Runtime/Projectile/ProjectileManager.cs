using System.Collections.Generic;
using UnityEngine;

namespace TeamSuneat
{
    public class ProjectileManager : XBehaviour
    {
        public static ProjectileManager Instance;

        public List<Projectile> m_projectiles = new List<Projectile>();

        public int ProjectileCount => m_projectiles.Count;

        public int SerialNumber;

        protected virtual void Awake()
        {
            

            Instance = this;

            SerialNumber = 0;
        }

        public void Add(Projectile projectile)
        {
            if (false == m_projectiles.Contains(projectile))
            {
                m_projectiles.Add(projectile);
                SerialNumber++;
            }

            Log.Info(LogTags.Projectile, "발사체를 매니저에 추가합니다. {0}, Count: {1}", projectile.GetHierarchyPath(), ProjectileCount);
        }

        public void Remove(Projectile projectile)
        {
            if (m_projectiles.Contains(projectile))
            {
                m_projectiles.Remove(projectile);
            }

            Log.Info(LogTags.Projectile, "발사체를 매니저에서 삭제합니다. {0}, Count: {1}", projectile.GetHierarchyPath(), ProjectileCount);
        }

        public void DestroyAll()
        {
            for (int i = 0; i < m_projectiles.Count; i++)
            {
                if (m_projectiles[i] == null)
                {
                    continue;
                }

                if (false == m_projectiles[i].gameObject.activeSelf)
                {
                    continue;
                }

                m_projectiles[i].ForceApplyReturn();

                m_projectiles[i].Destroy();
            }
        }

        public void Clear()
        {
            Log.Info(LogTags.Projectile, "모든 발사체를 매니저에서 삭제합니다.");

            m_projectiles.Clear();
        }

        public void ResetSerialNumber()
        {
            SerialNumber = 0;
        }

        public Projectile Find(Collider2D collider)
        {
            for (int i = 0; i < m_projectiles.Count; i++)
            {
                if (m_projectiles[i].CompareCollider(collider))
                {
                    return m_projectiles[i];
                }
            }

            return null;
        }

        private void FixedUpdate()
        {
            if (m_projectiles != null && m_projectiles.Count > 0)
            {
                for (int i = 0; i < m_projectiles.Count; i++)
                {
                    if (m_projectiles[i].TryMove())
                    {
                        m_projectiles[i].Move();

                        m_projectiles[i].OnMove();
                    }
                }
            }
        }
    }
}