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

        protected Coroutine _coroutine;

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
            if (this == null || gameObject == null)
            {
                return;
            }

            if (IsDestroyed)
            {
                return;
            }

            if (OnDespawnEvent != null)
            {
                OnDespawnEvent.Invoke();
                OnDespawnEvent.RemoveAllListeners();
            }

            if (!_isDespawned)
            {
                transform.SetParent(null);
                ResourcesManager.Despawn(gameObject, Time.unscaledDeltaTime);

                _isDespawned = true;
            }
        }

        public void ForceDespawn()
        {
            StopDespawnTimer();
            Despawn();
        }

        public void RegisterDespawnEvent(UnityAction despawnEvent)
        {
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