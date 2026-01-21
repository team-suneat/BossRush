using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TeamSuneat
{
    [System.Serializable]
    public class Gacha
    {
        [SerializeField] private List<float> _baseProbabilities = new();
        private List<int> _resultValues = new();
        private List<float> _activeProbabilities = new();
        protected float _maxProbability;
        protected int _pickedIndex;
        private int _baseVersion;
        private int _lastCheckedVersion;
        private bool _hasLoggedNormalization;

        public bool Validate()
        {
            if (_resultValues == null || _resultValues.Count <= 0)
            {
                return false;
            }
            if (_baseProbabilities == null)
            {
                return false;
            }
            if (_baseProbabilities.Count <= 0)
            {
                return false;
            }
            if (_baseProbabilities.Count != _resultValues.Count)
            {
                Log.Warning("가챠 설정 오류: Probabilities와 ResultValues의 요소 수가 동일하지 않습니다.");
                return false;
            }
            if (_baseProbabilities.Sum() <= 0f)
            {
                Log.Warning("가챠 설정 오류: 전체 확률 합이 0입니다.");
                return false;
            }
            return true;
        }

        public void Refresh()
        {
            if (_baseProbabilities.IsValid())
            {
                _activeProbabilities = new List<float>(_baseProbabilities);
                RecalculateMaxProbability();
            }
        }

        private void RecalculateMaxProbability()
        {
            if (_activeProbabilities.Count <= 0)
            {
                _maxProbability = 0f;
                return;
            }

            _maxProbability = _activeProbabilities.Sum();

            if (_maxProbability <= 0f)
            {
                if (_activeProbabilities.Count == 1)
                {
                    _activeProbabilities[0] = 1f;
                    _maxProbability = 1f;
                    return;
                }
                Log.Warning("가챠의 확률 총합이 0이거나 음수입니다.");
                return;
            }

            if (!ApproximatelyEqual(_maxProbability, 1f))
            {
                float oldMax = _maxProbability;
                float scale = 1f.SafeDivide(_maxProbability);
                for (int i = 0; i < _activeProbabilities.Count; i++)
                {
                    _activeProbabilities[i] *= scale;
                }
                if (!_hasLoggedNormalization)
                {
                    Log.Warning(
                        $"가챠 확률 총합이 {ValueStringEx.GetPercentString(oldMax, true)}입니다. " +
                        $"모든 확률을 비례 보정하여 총합을 100%로 조정했습니다.");
                    _hasLoggedNormalization = true;
                }
                _maxProbability = 1f;
            }
        }

        private bool ApproximatelyEqual(float a, float b, float tolerance = 0.0001f)
        {
            return Mathf.Abs(a - b) < tolerance;
        }

        public bool CheckLockAll()
        {
            EnsureActiveProbabilities();
            return ApproximatelyEqual(_maxProbability, 0f);
        }

        public void LockAt(int index)
        {
            EnsureActiveProbabilities();
            if (index < 0 || index >= _activeProbabilities.Count)
            {
                Log.Error($"LockAt: 인덱스 {index}는 유효하지 않습니다.");
                return;
            }

            _activeProbabilities[index] = 0f;
            RecalculateMaxProbability();
        }

        public void Add(float probability, int resultValue)
        {
            if (probability < 0f)
            {
                Log.Warning($"Add: 확률({probability})이 음수입니다. 0으로 처리됩니다.");
                probability = 0f;
            }

            _baseProbabilities.Add(probability);
            _resultValues.Add(resultValue);
            _baseVersion++;
            _hasLoggedNormalization = false;

            Refresh();
        }

        public void SetAll(float[] probabilities, int[] resultValues)
        {
            if (probabilities == null || resultValues == null)
            {
                Log.Error("Set: 확률 배열이나 결과 값 배열이 null입니다.");
                return;
            }
            if (probabilities.Length != resultValues.Length)
            {
                Log.Error("Set: 확률 배열과 결과 값 배열의 요소 수가 동일하지 않습니다.");
                return;
            }

            for (int i = 0; i < probabilities.Length; i++)
            {
                if (probabilities[i] < 0f)
                {
                    Log.Warning($"Set: {i + 1}번째 확률({probabilities[i]})이 음수입니다. 0으로 처리됩니다.");
                }
            }

            _baseProbabilities = new List<float>(probabilities);
            _activeProbabilities = new List<float>(probabilities);
            _resultValues = new List<int>(resultValues);
            _baseVersion++;
            _hasLoggedNormalization = false;
            Refresh();
        }

        public void Clear()
        {
            _baseProbabilities.Clear();
            _resultValues.Clear();
            _activeProbabilities.Clear();
            _maxProbability = 0f;
            _pickedIndex = -1;
            _baseVersion++;
            _lastCheckedVersion = _baseVersion;
            _hasLoggedNormalization = false;
        }

        public int Pick()
        {
            EnsureActiveProbabilities();
            if (ApproximatelyEqual(_maxProbability, 0f))
            {
                Refresh();
            }

            float randomValue = RandomEx.Range(0f, _maxProbability);
            float cumulativeProbability = 0f;

            for (int i = 0; i < _activeProbabilities.Count; i++)
            {
                cumulativeProbability += _activeProbabilities[i];

                if (randomValue <= cumulativeProbability)
                {
                    _pickedIndex = i;
                    return _pickedIndex;
                }
            }

            Log.Error("가챠 선택에 실패했습니다. 후보 목록을 확인하세요.");
            return -1;
        }

        private void EnsureActiveProbabilities()
        {
            if (_activeProbabilities.Count != _baseProbabilities.Count || _baseVersion != _lastCheckedVersion)
            {
                Refresh();
                _lastCheckedVersion = _baseVersion;
            }
        }

        #region Editor

        [Button("균등 분배 생성", ButtonSizes.Medium)]
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public void GenerateEqualDistribution(int candidateCount)
        {
            if (candidateCount <= 0)
            {
                Log.Error("균등 분배 생성 실패: 최대 후보 개수가 0 이하입니다.");
                return;
            }

            float equalProbability = 1f / candidateCount;

            _baseProbabilities.Clear();
            _resultValues.Clear();
            _activeProbabilities.Clear();

            for (int i = 0; i < candidateCount; i++)
            {
                _baseProbabilities.Add(equalProbability);
                _activeProbabilities.Add(equalProbability);
                _resultValues.Add(i);
            }

            _maxProbability = 1f;
            _hasLoggedNormalization = false;
            _baseVersion++;
            _lastCheckedVersion = _baseVersion;
        }

        #endregion Editor
    }
}