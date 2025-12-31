using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

namespace TeamSuneat.CameraSystem.Implementations
{
    [System.Serializable]
    public class CameraShakeInfo
    {
        public CameraShakeNames ShakeType;
        public float Amplitude;
        public float Frequency;
        public float Duration;
        public float Priority;
    }

    public class CameraShake : XBehaviour
    {
        public CameraShakeInfo[] ShakeInfoArray;

        private Dictionary<CameraShakeNames, CameraShakeInfo> _shakeInfos;
        private CinemachineBasicMultiChannelPerlin _perlin;
        private CinemachineCamera _virtualCamera;

        private CameraShakeInfo _currentShakeInfo;
        private Dictionary<CameraShakeNames, Coroutine> _shakeCoroutines;

        private void Awake()
        {
            _virtualCamera = GetComponent<CinemachineCamera>();
            _perlin = GetComponent<CinemachineBasicMultiChannelPerlin>();

            _shakeInfos = new Dictionary<CameraShakeNames, CameraShakeInfo>();
            _shakeCoroutines = new Dictionary<CameraShakeNames, Coroutine>();

            if (ShakeInfoArray.IsValid())
            {
                for (int i = 0; i < ShakeInfoArray.Length; i++)
                {
                    if (ShakeInfoArray[i].ShakeType == CameraShakeNames.None)
                    {
                        continue;
                    }

                    if (_shakeInfos.ContainsKey(ShakeInfoArray[i].ShakeType))
                    {
                        continue;
                    }

                    _shakeInfos.Add(ShakeInfoArray[i].ShakeType, ShakeInfoArray[i]);
                }

                ShakeInfoArray = null;
            }
        }

        private CameraShakeInfo Find(CameraShakeNames shakeType)
        {
            if (_shakeInfos.ContainsKey(shakeType))
            {
                return _shakeInfos[shakeType];
            }

            return null;
        }

        private void InitializePerlin()
        {
            if (_perlin == null)
            {
                _perlin = GetComponent<CinemachineBasicMultiChannelPerlin>();
            }
        }

        private bool CheckPriority(CameraShakeInfo shakeInfo)
        {
            if (_currentShakeInfo != null)
            {
                if (_currentShakeInfo.Priority == shakeInfo.Priority)
                {
                    Log.Warning(LogTags.Camera, "현재 카메라 쉐이크의 우선순위와 같아 무시합니다. {0}", shakeInfo.ShakeType);
                    return false;
                }
                else if (_currentShakeInfo.Priority > shakeInfo.Priority)
                {
                    Log.Warning(LogTags.Camera, "현재 카메라 쉐이크의 우선순위가 높아 무시합니다. {0}", shakeInfo.ShakeType);
                    return false;
                }
                else
                {
                    StopShake(shakeInfo.ShakeType);
                }
            }

            return true;
        }

        public void StartShake(CameraShakeNames shakeType, float duration = 0)
        {
            CameraShakeInfo shakeInfo = Find(shakeType);
            if (shakeInfo == null || !CheckPriority(shakeInfo))
            {
                return;
            }

            InitializePerlin();
            if (_perlin == null)
            {
                Log.Warning(LogTags.Camera, "CinemachineBasicMultiChannelPerlin 컴포넌트를 찾을 수 없습니다.");
                return;
            }

            Log.Info(LogTags.Camera, "카메라 쉐이크를 시작합니다. {0}, {1}", _virtualCamera.name, shakeType);

            _currentShakeInfo = shakeInfo;

            if (!_shakeCoroutines.ContainsKey(shakeType))
            {
                Coroutine coroutine = StartXCoroutine(ProcessShake(_perlin, shakeInfo, duration));
                _shakeCoroutines.Add(shakeType, coroutine);
            }
        }

        private void StopShake(CameraShakeNames shakeType)
        {
            if (_shakeCoroutines.ContainsKey(shakeType))
            {
                StopCoroutine(_shakeCoroutines[shakeType]);
                _shakeCoroutines.Remove(shakeType);
            }

            if (_currentShakeInfo != null)
            {
                if (_currentShakeInfo.ShakeType == shakeType)
                {
                    _currentShakeInfo = null;
                }
            }
        }

        private IEnumerator ProcessShake(CinemachineBasicMultiChannelPerlin perlin, CameraShakeInfo shakeInfo, float duration = 0)
        {
            if (duration > 0)
            {
                float lastTime = duration;

                while (lastTime > 0)
                {
                    lastTime -= Time.deltaTime;

                    perlin.AmplitudeGain = Mathf.Lerp(shakeInfo.Amplitude, 0f, lastTime / duration);
                    perlin.FrequencyGain = Mathf.Lerp(shakeInfo.Frequency, 0f, lastTime / duration);

                    yield return null;
                }
            }
            else
            {
                float lastTime = shakeInfo.Duration;

                while (lastTime > 0)
                {
                    lastTime -= Time.deltaTime;

                    perlin.AmplitudeGain = Mathf.Lerp(shakeInfo.Amplitude, 0f, lastTime / shakeInfo.Duration);
                    perlin.FrequencyGain = Mathf.Lerp(shakeInfo.Frequency, 0f, lastTime / shakeInfo.Duration);

                    yield return null;
                }
            }

            ResetShakeValue(perlin);

            StopShake(shakeInfo.ShakeType);
        }

        private void ResetShakeValue(CinemachineBasicMultiChannelPerlin perlin)
        {
            perlin.AmplitudeGain = 0;
            perlin.FrequencyGain = 0;
        }
    }
}