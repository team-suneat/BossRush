using System;
using System.Collections;
using UnityEngine;

namespace TeamSuneat
{
    public class GameTimer
    {
        public string Id { get; private set; }
        public float Duration { get; private set; }
        public float RemainingTime { get; private set; }
        public float ElapsedTime => Duration - RemainingTime;
        public bool IsActive { get; private set; }
        public bool IsPaused { get; private set; }

        private Action _onExpired;
        private Action<float> _onTick;
        private float _tickInterval;
        private float _lastTickTime;
        private Coroutine _coroutine;

        public GameTimer(string id, float duration,
            Action onExpired,
            Action<float> onTick = null,
            float tickInterval = 0.1f)
        {
            Id = id;
            Duration = duration;
            RemainingTime = duration;
            _onExpired = onExpired;
            _onTick = onTick;
            _tickInterval = tickInterval;
        }

        public void Start(MonoBehaviour coroutineRunner)
        {
            if (IsActive)
            {
                return;
            }

            IsActive = true;
            IsPaused = false;
            RemainingTime = Duration;
            _lastTickTime = Time.time;
            _coroutine = coroutineRunner.StartCoroutine(TimerCoroutine());
        }

        public void Stop(MonoBehaviour coroutineRunner)
        {
            if (!IsActive)
            {
                return;
            }

            IsActive = false;
            IsPaused = false;

            if (_coroutine != null && coroutineRunner != null)
            {
                coroutineRunner.StopCoroutine(_coroutine);
                _coroutine = null;
            }
        }

        public void Pause()
        {
            if (!IsActive || IsPaused)
            {
                return;
            }

            IsPaused = true;
        }

        public void Resume()
        {
            if (!IsActive || !IsPaused)
            {
                return;
            }

            IsPaused = false;
            _lastTickTime = Time.time;
        }

        private IEnumerator TimerCoroutine()
        {
            while (IsActive && RemainingTime > 0f)
            {
                if (!IsPaused)
                {
                    float deltaTime = Time.deltaTime;
                    RemainingTime = Mathf.Max(0f, RemainingTime - deltaTime);

                    // 틱 콜백 호출
                    if (_onTick != null && Time.time - _lastTickTime >= _tickInterval)
                    {
                        _onTick(RemainingTime);
                        _lastTickTime = Time.time;
                    }

                    // 만료 확인
                    if (RemainingTime <= 0f)
                    {
                        _onExpired?.Invoke();
                        IsActive = false;
                        yield break;
                    }
                }

                yield return null;
            }
        }
    }
}