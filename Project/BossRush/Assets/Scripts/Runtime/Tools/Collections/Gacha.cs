using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TeamSuneat
{
    [System.Serializable]
    public class Gacha
    {
        /// <summary>
        /// 원본 확률 리스트입니다.
        /// </summary>
        public List<float> Probabilities = new List<float>();

        /// <summary>
        /// 결과 값 리스트입니다. Probabilities와 요소 수가 동일해야 합니다.
        /// </summary>
        public List<float> ResultValues = new List<float>();

        /// <summary>
        /// 실제 가챠 선택 시 사용되는 확률 리스트입니다. 외부에서 직접 접근하지 않도록 private으로 설정했습니다.
        /// </summary>
        private List<float> _probabilities = new List<float>();

        /// <summary>
        /// Candidates 리스트의 확률 총합(보통 1f)을 저장합니다.
        /// </summary>
        protected float _maxProbability;

        /// <summary>
        /// 마지막으로 선택된 후보의 인덱스입니다.
        /// </summary>
        protected int _pickedIndex;

        /// <summary>
        /// 가챠 클래스가 정상적으로 사용할 수 있는 상태인지 검사합니다.
        /// 검사 조건:
        /// 1. Probabilities와 ResultValues가 null이 아니고, 요소 수가 0보다 커야 합니다.
        /// 2. Probabilities와 ResultValues의 요소 수가 동일해야 합니다.
        /// 3. Probabilities의 총합이 0보다 커야 합니다.
        /// </summary>
        /// <returns>유효하면 true, 아니면 false</returns>
        public bool IsValid()
        {
            // 리스트가 null인지, 요소가 존재하는지 검사
            if (Probabilities == null || ResultValues == null)
            {
                return false;
            }
            if (Probabilities.Count <= 0 || ResultValues.Count <= 0)
            {
                return false;
            }
            // 두 리스트의 요소 수가 동일한지 검사
            if (Probabilities.Count != ResultValues.Count)
            {
                Log.Warning("가챠 설정 오류: Probabilities와 ResultValues의 요소 수가 동일하지 않습니다.");
                return false;
            }
            // 확률 총합이 0보다 큰지 검사
            float sum = Probabilities.Sum();
            if (sum <= 0f)
            {
                Log.Warning("가챠 설정 오류: 전체 확률 합이 0입니다.");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Probabilities 값을 기반으로 _probabilities를 재설정하고 최대 확률을 갱신합니다.
        /// </summary>
        public void Refresh()
        {
            if (Probabilities.IsValid())
            {
                // Probabilities를 그대로 복사하여 _probabilities를 새로 생성합니다.
                _probabilities = new List<float>(Probabilities);
                RecalculateMaxProbability();
            }
        }

        /// <summary>
        /// 후보들의 확률 총합을 재계산하고, 전체 합이 100%(=1f)가 되도록 비례 보정합니다.
        /// </summary>
        private void RecalculateMaxProbability()
        {
            // 현재 _probabilities의 총합 계산
            _maxProbability = _probabilities.Sum();

            // 총합이 0 이하이면 오류 로그 출력 후 종료
            if (_maxProbability <= 0f)
            {
                Log.Error("가챠의 확률 총합이 0이거나 음수입니다.");
                return;
            }

            // 보정: 모든 후보를 비례로 조정해서 총합이 정확히 1f가 되도록 함
            if (!ApproximatelyEqual(_maxProbability, 1f))
            {
                float oldMax = _maxProbability;
                for (int i = 0; i < _probabilities.Count; i++)
                {
                    float oldProbability = _probabilities[i];
                    // 각 확률을 전체 합으로 나누어 비례 보정
                    _probabilities[i] = oldProbability / _maxProbability;
                    Log.Warning(
                        $"가챠의 확률 총합이 {ValueStringEx.GetPercentString(oldMax, true)}입니다. " +
                        $"{i + 1}번째 선택지의 확률을 {ValueStringEx.GetPercentString(_probabilities[i])}로 조정합니다.");
                }
                _maxProbability = 1f;
            }
        }

        /// <summary>
        /// 두 실수가 지정한 오차 범위 내에서 동일한지 확인합니다.
        /// </summary>
        /// <param name="a">첫 번째 값</param>
        /// <param name="b">두 번째 값</param>
        /// <param name="tolerance">허용 오차</param>
        /// <returns>동일하면 true, 아니면 false</returns>
        private bool ApproximatelyEqual(float a, float b, float tolerance = 0.0001f)
        {
            return Mathf.Abs(a - b) < tolerance;
        }

        /// <summary>
        /// 모든 후보가 잠긴(확률이 0인) 상태인지 확인합니다.
        /// </summary>
        /// <returns>잠겨 있으면 true, 아니면 false</returns>
        public bool CheckLockAll()
        {
            return ApproximatelyEqual(_maxProbability, 0f);
        }

        /// <summary>
        /// 지정한 인덱스의 후보를 잠급니다.
        /// </summary>
        /// <param name="index">잠글 후보의 인덱스</param>
        public void LockAt(int index)
        {
            if (index < 0 || index >= _probabilities.Count)
            {
                Log.Error($"LockAt: 인덱스 {index}는 유효하지 않습니다.");
                return;
            }
            _probabilities[index] = 0f;
            RecalculateMaxProbability();
        }

        /// <summary>
        /// 지정한 인덱스의 후보를 원래 확률로 복구합니다.
        /// </summary>
        /// <param name="index">해제할 후보의 인덱스</param>
        public void UnlockAt(int index)
        {
            if (index < 0 || index >= _probabilities.Count)
            {
                Log.Error($"UnlockAt: 인덱스 {index}는 유효하지 않습니다.");
                return;
            }
            _probabilities[index] = Probabilities[index];
            RecalculateMaxProbability();
        }

        /// <summary>
        /// 새로운 확률과 결과 값을 함께 추가합니다.
        /// 확률이 0인 경우 추가하지 않습니다.
        /// </summary>
        /// <param name="probability">추가할 확률 값</param>
        /// <param name="resultValue">추가할 결과 값</param>
        public void Add(float probability, float resultValue)
        {
            if (probability.IsZero())
            {
                Log.Warning("Add: 0 확률은 추가되지 않습니다.");
                return;
            }
            Probabilities.Add(probability);
            _probabilities.Add(probability);
            ResultValues.Add(resultValue);
            RecalculateMaxProbability();
        }

        /// <summary>
        /// 확률과 결과 값 배열을 설정하고 내부 상태를 갱신합니다.
        /// 확률 배열과 결과 값 배열의 요소 수가 동일해야 합니다.
        /// </summary>
        /// <param name="probabilities">설정할 확률 배열</param>
        /// <param name="resultValues">설정할 결과 값 배열</param>
        public void Set(float[] probabilities, float[] resultValues)
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

            // 새로운 리스트로 깊은 복사하여 할당
            Probabilities = new List<float>(probabilities);
            _probabilities = new List<float>(probabilities);
            this.ResultValues = new List<float>(resultValues);
            Refresh();
        }

        /// <summary>
        /// 가챠에서 무작위로 후보를 선택합니다.
        /// </summary>
        /// <returns>선택된 후보의 인덱스. 선택 실패 시 -1 반환</returns>
        public int Pick()
        {
            // _maxProbability가 0이면 내부 상태를 갱신
            if (ApproximatelyEqual(_maxProbability, 0f))
            {
                Refresh();
            }

            float randomValue = RandomEx.Range(0f, _maxProbability);
            float cumulativeProbability = 0f;

            for (int i = 0; i < _probabilities.Count; i++)
            {
                cumulativeProbability += _probabilities[i];

                if (randomValue <= cumulativeProbability)
                {
                    _pickedIndex = i;
                    return _pickedIndex;
                }
            }

            Log.Error("가챠 선택에 실패했습니다. 후보 목록을 확인하세요.");
            return -1;
        }

        /// <summary>
        /// 가챠에서 선택된 후보의 결과 값을 반환합니다.
        /// </summary>
        /// <returns>선택된 후보에 해당하는 결과 값. 선택 실패 시 0 반환</returns>
        public float PickValue()
        {
            int index = Pick();
            return ResultValues.IsValid(index) ? ResultValues[index] : 0f;
        }

        /// <summary>
        /// 현재 Gacha 객체의 깊은 복사본을 생성하여 반환합니다.
        /// </summary>
        /// <returns>깊은 복사된 Gacha 객체</returns>
        public Gacha Clone()
        {
            Gacha clone = new Gacha();
            // 깊은 복사: 각 리스트를 새로 생성하여 복사합니다.
            clone.Probabilities = new List<float>(this.Probabilities);
            clone._probabilities = new List<float>(this._probabilities);
            clone.ResultValues = new List<float>(this.ResultValues);
            clone._maxProbability = this._maxProbability;
            clone._pickedIndex = this._pickedIndex;
            return clone;
        }
    }
}