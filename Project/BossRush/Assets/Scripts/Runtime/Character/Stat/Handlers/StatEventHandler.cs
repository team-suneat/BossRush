using UnityEngine.Events;

namespace TeamSuneat
{
    /// <summary>
    /// 능력치 이벤트 처리 핸들러
    /// StatSystem의 이벤트 관련 책임을 담당합니다.
    /// </summary>
    public class StatEventHandler
    {
        private readonly UnityEvent<StatNames, float> _onRefresh = new();
        private readonly UnityEvent<StatNames, float> _onRefreshed = new();
        private readonly UnityEvent<StatNames, float, float> _onStatChanged = new();

        // 배치 처리 이벤트
        private readonly UnityEvent<StatNames[]> _onBatchStart = new();
        private readonly UnityEvent<StatNames[]> _onBatchComplete = new();

        /// <summary>
        /// StatEventHandler 생성자
        /// </summary>
        public StatEventHandler()
        {
        }

        /// <summary>
        /// 능력치 새로고침 시작 이벤트를 호출합니다.
        /// </summary>
        /// <param name="statName">능력치 이름</param>
        /// <param name="value">변경될 값</param>
        public void CallRefreshEvent(StatNames statName, float value)
        {
            _onRefresh.Invoke(statName, value);
        }

        /// <summary>
        /// 능력치 새로고침 완료 이벤트를 호출합니다.
        /// </summary>
        /// <param name="statName">능력치 이름</param>
        /// <param name="value">변경된 값</param>
        public void CallRefreshedEvent(StatNames statName, float value)
        {
            _onRefreshed.Invoke(statName, value);
        }

        /// <summary>
        /// 능력치 변경 이벤트를 호출합니다.
        /// </summary>
        /// <param name="statName">능력치 이름</param>
        /// <param name="oldValue">이전 값</param>
        /// <param name="newValue">새로운 값</param>
        public void CallStatChangedEvent(StatNames statName, float oldValue, float newValue)
        {
            _onStatChanged.Invoke(statName, oldValue, newValue);
        }

        /// <summary>
        /// 능력치 새로고침 시작 이벤트에 리스너를 등록합니다.
        /// </summary>
        /// <param name="listener">리스너</param>
        public void RegisterRefreshListener(UnityAction<StatNames, float> listener)
        {
            _onRefresh.AddListener(listener);
        }

        /// <summary>
        /// 능력치 새로고침 완료 이벤트에 리스너를 등록합니다.
        /// </summary>
        /// <param name="listener">리스너</param>
        public void RegisterRefreshedListener(UnityAction<StatNames, float> listener)
        {
            _onRefreshed.AddListener(listener);
        }

        /// <summary>
        /// 능력치 변경 이벤트에 리스너를 등록합니다.
        /// </summary>
        /// <param name="listener">리스너</param>
        public void RegisterStatChangedListener(UnityAction<StatNames, float, float> listener)
        {
            _onStatChanged.AddListener(listener);
        }

        /// <summary>
        /// 능력치 새로고침 시작 이벤트에서 리스너를 제거합니다.
        /// </summary>
        /// <param name="listener">리스너</param>
        public void UnregisterRefreshListener(UnityAction<StatNames, float> listener)
        {
            _onRefresh.RemoveListener(listener);
        }

        /// <summary>
        /// 능력치 새로고침 완료 이벤트에서 리스너를 제거합니다.
        /// </summary>
        /// <param name="listener">리스너</param>
        public void UnregisterRefreshedListener(UnityAction<StatNames, float> listener)
        {
            _onRefreshed.RemoveListener(listener);
        }

        /// <summary>
        /// 능력치 변경 이벤트에서 리스너를 제거합니다.
        /// </summary>
        /// <param name="listener">리스너</param>
        public void UnregisterStatChangedListener(UnityAction<StatNames, float, float> listener)
        {
            _onStatChanged.RemoveListener(listener);
        }

        /// <summary>
        /// 배치 처리 시작 이벤트를 호출합니다.
        /// </summary>
        public void CallBatchStartEvent(StatNames[] affectedStats)
        {
            _onBatchStart.Invoke(affectedStats);
        }

        /// <summary>
        /// 배치 처리 완료 이벤트를 호출합니다.
        /// </summary>
        public void CallBatchCompleteEvent(StatNames[] affectedStats)
        {
            _onBatchComplete.Invoke(affectedStats);
        }

        /// <summary>
        /// 배치 처리 시작 이벤트에 리스너를 등록합니다.
        /// </summary>
        public void RegisterBatchStartListener(UnityAction<StatNames[]> listener)
        {
            _onBatchStart.AddListener(listener);
        }

        /// <summary>
        /// 배치 처리 완료 이벤트에 리스너를 등록합니다.
        /// </summary>
        public void RegisterBatchCompleteListener(UnityAction<StatNames[]> listener)
        {
            _onBatchComplete.AddListener(listener);
        }

        /// <summary>
        /// 모든 이벤트 리스너를 제거합니다.
        /// </summary>
        public void RemoveAllListeners()
        {
            _onRefresh.RemoveAllListeners();
            _onRefreshed.RemoveAllListeners();
            _onStatChanged.RemoveAllListeners();
            _onBatchStart.RemoveAllListeners();
            _onBatchComplete.RemoveAllListeners();
        }

        /// <summary>
        /// 능력치 변경 이벤트를 등록합니다.
        /// </summary>
        /// <param name="callback">콜백 함수</param>
        public void RegisterOnStatChanged(UnityAction<StatNames, float, float> callback)
        {
            _onStatChanged.AddListener(callback);
        }

        /// <summary>
        /// 능력치 변경 이벤트를 해제합니다.
        /// </summary>
        /// <param name="callback">콜백 함수</param>
        public void UnregisterOnStatChanged(UnityAction<StatNames, float, float> callback)
        {
            _onStatChanged.RemoveListener(callback);
        }

        /// <summary>
        /// 능력치 새로고침 이벤트를 등록합니다.
        /// </summary>
        /// <param name="callback">콜백 함수</param>
        public void RegisterOnRefresh(UnityAction<StatNames, float> callback)
        {
            _onRefresh.AddListener(callback);
        }

        /// <summary>
        /// 능력치 새로고침 이벤트를 해제합니다.
        /// </summary>
        /// <param name="callback">콜백 함수</param>
        public void UnregisterOnRefresh(UnityAction<StatNames, float> callback)
        {
            _onRefresh.RemoveListener(callback);
        }

        /// <summary>
        /// 능력치 새로고침 완료 이벤트를 등록합니다.
        /// </summary>
        /// <param name="callback">콜백 함수</param>
        public void RegisterOnRefreshed(UnityAction<StatNames, float> callback)
        {
            _onRefreshed.AddListener(callback);
        }

        /// <summary>
        /// 능력치 새로고침 완료 이벤트를 해제합니다.
        /// </summary>
        /// <param name="callback">콜백 함수</param>
        public void UnregisterOnRefreshed(UnityAction<StatNames, float> callback)
        {
            _onRefreshed.RemoveListener(callback);
        }
    }
}