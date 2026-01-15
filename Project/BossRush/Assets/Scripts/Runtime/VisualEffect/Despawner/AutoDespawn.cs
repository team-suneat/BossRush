using System.Collections;
using Lean.Pool;
using UnityEngine;
using UnityEngine.Events;

namespace TeamSuneat
{
    public class AutoDespawn : XBehaviour, IPoolable
    {
        public float Duration;
        public UnityEvent OnDespawnEvent;

        private Coroutine _coroutine;
        private bool _isDespawned;

        public void OnSpawn()
        {
            _isDespawned = false;

            StopDespawnTimer();
            StartDespawnTimer();
        }

        public void OnDespawn()
        {
            StopDespawnTimer();
        }

        public void Despawn()
        {
            if (this == null || gameObject == null || IsDestroyed)
            {
                return;
            }

            if (_isDespawned)
            {
                return;
            }

            OnDespawnEvent?.Invoke();
            OnDespawnEvent?.RemoveAllListeners();

            transform.SetParent(null);
            ResourcesManager.Despawn(gameObject, Time.unscaledDeltaTime);
            _isDespawned = true;
        }

        public void ForceDespawn()
        {
            StopDespawnTimer();
            Despawn();
        }

        public void RegisterDespawnEvent(UnityAction despawnEvent)
        {
            if (despawnEvent == null)
            {
                return;
            }

            OnDespawnEvent.AddListener(despawnEvent);
        }

        public void StartDespawnTimer()
        {
            if (!Duration.IsZero())
            {
                _coroutine = StartXCoroutine(OnDespawnTimerCoroutine());
            }
        }

        public void StopDespawnTimer()
        {
            StopXCoroutine(ref _coroutine);
        }

        private IEnumerator OnDespawnTimerCoroutine()
        {
            yield return new WaitForSeconds(Duration);
            _coroutine = null;
            Despawn();
        }
    }
}