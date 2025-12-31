using System.Collections.Generic;
using UnityEngine;

namespace TeamSuneat.UserInterface
{
    /// <summary>
    /// Notice UI 관리자 - 일시적으로 표시되는 알림 메시지들을 관리
    /// </summary>
    public class UINoticeManager : XBehaviour
    {
        private const int MAX_ACTIVE_NOTICES = 5;

        [SerializeField]
        private Transform _obtainPoint;

        [SerializeField]
        private Transform _shortagePoint;

        private List<UINoticeBase> _activeShortageNotices = new List<UINoticeBase>();
        private List<UINoticeBase> _activeObtainNotices = new List<UINoticeBase>();

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _obtainPoint = this.FindTransform("ObtainPoint");
            _shortagePoint = this.FindTransform("ShortagePoint");
        }

        protected override void RegisterGlobalEvent()
        {
            base.RegisterGlobalEvent();
            GlobalEvent<CurrencyNames, int>.Register(GlobalEventType.CURRENCY_EARNED, OnCurrencyEarned);
            GlobalEvent<CurrencyNames>.Register(GlobalEventType.CURRENCY_SHORTAGE, OnCurrencyShortage);
            GlobalEvent.Register(GlobalEventType.STAT_POINT_SHORTAGE, OnStatPointShortage);
            GlobalEvent.Register(GlobalEventType.EXPERIENCE_SHORTAGE, OnExperienceShortage);
        }

        protected override void UnregisterGlobalEvent()
        {
            base.UnregisterGlobalEvent();
            GlobalEvent<CurrencyNames, int>.Unregister(GlobalEventType.CURRENCY_EARNED, OnCurrencyEarned);
            GlobalEvent<CurrencyNames>.Unregister(GlobalEventType.CURRENCY_SHORTAGE, OnCurrencyShortage);
            GlobalEvent.Unregister(GlobalEventType.STAT_POINT_SHORTAGE, OnStatPointShortage);
            GlobalEvent.Unregister(GlobalEventType.EXPERIENCE_SHORTAGE, OnExperienceShortage);
        }

        private void OnCurrencyEarned(CurrencyNames currencyName, int addAmount)
        {
            SpawnCurrencyObtainedNotice(currencyName, addAmount);
        }

        private void OnCurrencyShortage(CurrencyNames currencyName)
        {
            SpawnCurrencyShortageNotice(currencyName);
        }

        private void OnStatPointShortage()
        {
            SpawnStatPointShortageNotice();
        }

        private void OnExperienceShortage()
        {
            SpawnExperienceShortageNotice();
        }

        public UICurrencyShortageNotice SpawnCurrencyShortageNotice(CurrencyNames currencyName)
        {
            UICurrencyShortageNotice notice = ResourcesManager.SpawnCurrencyShortageNotice(currencyName, _shortagePoint);
            if (notice != null)
            {
                RegisterShortageNotice(notice);
                notice.OnCompleted += () => UnregisterShortageNotice(notice);
            }
            return notice;
        }

        public UIStageTitleNotice SpawnStageTitleNotice(StageNames stageName)
        {
            UIStageTitleNotice notice = ResourcesManager.SpawnStageTitleNotice(stageName);
            if (notice != null)
            {
                RegisterShortageNotice(notice);
                notice.OnCompleted += () => UnregisterShortageNotice(notice);
            }
            return notice;
        }

        public UIStatPointShortageNotice SpawnStatPointShortageNotice()
        {
            UIStatPointShortageNotice notice = ResourcesManager.SpawnStatPointShortageNotice(_shortagePoint);
            if (notice != null)
            {
                RegisterShortageNotice(notice);
                notice.OnCompleted += () => UnregisterShortageNotice(notice);
            }
            return notice;
        }

        public UIExperienceShortageNotice SpawnExperienceShortageNotice()
        {
            UIExperienceShortageNotice notice = ResourcesManager.SpawnExperienceShortageNotice(_shortagePoint);
            if (notice != null)
            {
                RegisterShortageNotice(notice);
                notice.OnCompleted += () => UnregisterShortageNotice(notice);
            }
            return notice;
        }

        public UICurrencyObtainedNotice SpawnCurrencyObtainedNotice(CurrencyNames currencyName, int amount)
        {
            UICurrencyObtainedNotice notice = ResourcesManager.SpawnCurrencyObtainedNotice(currencyName, amount, _obtainPoint);
            if (notice != null)
            {
                RegisterObtainNotice(notice);
                notice.OnCompleted += () => UnregisterObtainNotice(notice);
            }
            return notice;
        }

        //

        private void RegisterShortageNotice(UINoticeBase notice)
        {
            if (notice != null && !_activeShortageNotices.Contains(notice))
            {
                // 최대 개수를 초과하면 가장 오래된 notice 디스폰
                if (_activeShortageNotices.Count >= MAX_ACTIVE_NOTICES)
                {
                    UINoticeBase oldestNotice = _activeShortageNotices[0];
                    if (oldestNotice != null)
                    {
                        oldestNotice.Despawn();
                    }
                    _activeShortageNotices.RemoveAt(0);
                }

                _activeShortageNotices.Add(notice);
            }
        }

        private void UnregisterShortageNotice(UINoticeBase notice)
        {
            if (notice != null && _activeShortageNotices.Contains(notice))
            {
                _activeShortageNotices.Remove(notice);
            }
        }

        private void RegisterObtainNotice(UINoticeBase notice)
        {
            if (notice != null && !_activeObtainNotices.Contains(notice))
            {
                // 최대 개수를 초과하면 가장 오래된 notice 디스폰
                if (_activeObtainNotices.Count >= MAX_ACTIVE_NOTICES)
                {
                    UINoticeBase oldestNotice = _activeObtainNotices[0];
                    if (oldestNotice != null)
                    {
                        oldestNotice.Despawn();
                    }
                    _activeObtainNotices.RemoveAt(0);
                }

                _activeObtainNotices.Add(notice);
            }
        }

        private void UnregisterObtainNotice(UINoticeBase notice)
        {
            if (notice != null && _activeObtainNotices.Contains(notice))
            {
                _activeObtainNotices.Remove(notice);
            }
        }

        public void Clear()
        {
            int totalCount = (_activeShortageNotices?.Count ?? 0) + (_activeObtainNotices?.Count ?? 0);
            if (totalCount > 0)
            {
                Log.Info(LogTags.UI_Notice, "활성화된 Notice를 모두 제거합니다. 개수: {0}", totalCount);

                // Shortage Notice 제거
                if (_activeShortageNotices != null && _activeShortageNotices.Count > 0)
                {
                    List<XBehaviour> noticesToClear = new List<XBehaviour>(_activeShortageNotices);
                    for (int i = 0; i < noticesToClear.Count; i++)
                    {
                        if (noticesToClear[i] != null)
                        {
                            noticesToClear[i].gameObject.SetActive(false);
                        }
                    }
                    _activeShortageNotices.Clear();
                }

                // Obtain Notice 제거
                if (_activeObtainNotices != null && _activeObtainNotices.Count > 0)
                {
                    List<XBehaviour> noticesToClear = new List<XBehaviour>(_activeObtainNotices);
                    for (int i = 0; i < noticesToClear.Count; i++)
                    {
                        if (noticesToClear[i] != null)
                        {
                            noticesToClear[i].gameObject.SetActive(false);
                        }
                    }
                    _activeObtainNotices.Clear();
                }
            }
        }

        public void LogicUpdate()
        {
        }
    }
}