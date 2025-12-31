using Lean.Pool;
using UnityEngine;

namespace TeamSuneat.UserInterface
{
    public class UIGaugePoolHandler : MonoBehaviour, IPoolable
    {
        public bool IsSpawned { get; private set; }
        public bool IsDespawned { get; private set; }

        public event System.Action OnClearRequested;

        private bool _despawnMark;

        public void OnSpawn()
        {
            IsSpawned = true;
            IsDespawned = false;
        }

        public void OnDespawn()
        {
        }

        public void Despawn()
        {
            if (IsSpawned && !IsDespawned)
            {
                IsDespawned = true;
                ResourcesManager.Despawn(gameObject);
            }
        }

        public void SetDespawnMark()
        {
            if (!_despawnMark)
            {
                _despawnMark = true;
            }
        }

        public void LogicUpdate()
        {
            if (_despawnMark)
            {
                _despawnMark = false;
                OnClearRequested?.Invoke();
            }
        }
    }
}

