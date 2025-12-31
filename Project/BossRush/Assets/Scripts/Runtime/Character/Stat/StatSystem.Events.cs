using UnityEngine.Events;

namespace TeamSuneat
{
    public partial class StatSystem : XBehaviour
    {
        #region 이벤트 등록

        /// <summary>
        /// 능력치 변경 이벤트를 등록합니다.
        /// </summary>
        public void RegisterOnStatChanged(UnityAction<StatNames, float, float> callback)
        {
            _eventHandler.RegisterOnStatChanged(callback);
        }

        /// <summary>
        /// 능력치 변경 이벤트를 해제합니다.
        /// </summary>
        public void UnregisterOnStatChanged(UnityAction<StatNames, float, float> callback)
        {
            _eventHandler.UnregisterOnStatChanged(callback);
        }

        /// <summary>
        /// 능력치 새로고침 이벤트를 등록합니다.
        /// </summary>
        public void RegisterOnRefresh(UnityAction<StatNames, float> callback)
        {
            _eventHandler.RegisterOnRefresh(callback);
        }

        /// <summary>
        /// 능력치 새로고침 이벤트를 해제합니다.
        /// </summary>
        public void UnregisterOnRefresh(UnityAction<StatNames, float> callback)
        {
            _eventHandler.UnregisterOnRefresh(callback);
        }

        /// <summary>
        /// 능력치 새로고침 완료 이벤트를 등록합니다.
        /// </summary>
        public void RegisterOnRefreshed(UnityAction<StatNames, float> callback)
        {
            _eventHandler.RegisterOnRefreshed(callback);
        }

        /// <summary>
        /// 능력치 새로고침 완료 이벤트를 해제합니다.
        /// </summary>
        public void UnregisterOnRefreshed(UnityAction<StatNames, float> callback)
        {
            _eventHandler.UnregisterOnRefreshed(callback);
        }

        #endregion 이벤트 등록
    }
}