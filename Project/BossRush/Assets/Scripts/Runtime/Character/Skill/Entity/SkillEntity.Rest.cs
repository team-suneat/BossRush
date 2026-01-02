using System.Collections;
using Lean.Pool;

using UnityEngine;

namespace TeamSuneat
{
    public partial class SkillEntity : XBehaviour, IPoolable
    {
        // 시전 대기시간이 설정되어 있으면, 시전 대기시간을 사용합니다.
        // 시전 대기시간이 없으면, 시전 대기를 사용하지 않습니다.
        // 시전 대기시간 초기화 또는 시전 대기시간이 설정되어 있으면 시전 대기를 사용할 수 있는 기술입니다.

        [SerializeField]
        private float _castRestTime;
        private Coroutine _restCoroutine;

        public bool IsResting => _restCoroutine != null;

        private void StartRest()
        {
            if (_restCoroutine == null)
            {
                _restCoroutine = StartXCoroutine(ProgressRest());
            }
        }

        private void StopRest()
        {
            StopXCoroutine(ref _restCoroutine);
        }

        private IEnumerator ProgressRest()
        {
            yield return new WaitForSeconds(_castRestTime);

            _restCoroutine = null;
        }
    }
}