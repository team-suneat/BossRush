using UnityEngine;

namespace TeamSuneat.UserInterface
{
    /// <summary>
    /// Notice UI 관리자 - 일시적으로 표시되는 알림 메시지들을 관리
    /// </summary>
    public class UINoticeManager : XBehaviour
    {
        [SerializeField]
        private string _announcementPrefabName = "UIAnnouncement";
        
        private UIToastMessageGroup _toastGroup; // 토스트 그룹 참조
        private UIAnnouncement _currentAnnouncement; // 현재 표시 중인 큰 알림 (단일)

        private void Awake()
        {
            _toastGroup = GetComponentInChildren<UIToastMessageGroup>();
        }

        public void LogicUpdate()
        {
            // 기본 구현은 비어있음
        }

        #region 큰 알림 (Announcement)

        public void ShowAnnouncement(string content, float duration = 3f)
        {
            if (string.IsNullOrEmpty(content))
            {
                return;
            }

            ClearCurrentAnnouncement();

            UIAnnouncement announcement = SpawnAnnouncement();
            if (announcement != null)
            {
                _currentAnnouncement = announcement;
                announcement.ShowAnnouncement(content, duration);
                announcement.OnCompleted += OnAnnouncementCompleted;
            }
        }

        public void ShowAnnouncementByKey(string stringKey, float duration = 3f)
        {
            if (string.IsNullOrEmpty(stringKey))
            {
                return;
            }

            ClearCurrentAnnouncement();

            UIAnnouncement announcement = SpawnAnnouncement();
            if (announcement != null)
            {
                _currentAnnouncement = announcement;
                announcement.ShowAnnouncementByKey(stringKey, duration);
                announcement.OnCompleted += OnAnnouncementCompleted;
            }
        }

        private void ClearCurrentAnnouncement()
        {
            if (_currentAnnouncement != null)
            {
                _currentAnnouncement.OnCompleted -= OnAnnouncementCompleted;
                _currentAnnouncement.Despawn();
                _currentAnnouncement = null;
            }
        }

        private UIAnnouncement SpawnAnnouncement()
        {
            CanvasOrder noticeCanvas = UIManager.Instance?.GetCanvas(CanvasOrderNames.Notice);
            Transform parent = noticeCanvas != null ? noticeCanvas.transform : transform;
            GameObject prefab = ResourcesManager.SpawnPrefab(_announcementPrefabName, parent);
            if (prefab == null)
            {
                Log.Warning(LogTags.UI_Notice, "알림 프리팹을 생성할 수 없습니다: {0}", _announcementPrefabName);
                return null;
            }

            UIAnnouncement announcement = prefab.GetComponent<UIAnnouncement>();
            if (announcement == null)
            {
                Log.Warning(LogTags.UI_Notice, "알림 프리팹에 UIAnnouncement 컴포넌트가 없습니다: {0}", _announcementPrefabName);
                return null;
            }

            prefab.transform.localPosition = Vector3.zero;
            return announcement;
        }

        private void OnAnnouncementCompleted()
        {
            if (_currentAnnouncement != null)
            {
                _currentAnnouncement.OnCompleted -= OnAnnouncementCompleted;
            }
            _currentAnnouncement = null;
        }

        #endregion 큰 알림 (Announcement)

        #region 토스트 메시지 (Toast)

        public void ShowToast(string content, float duration = 2f)
        {
            if (string.IsNullOrEmpty(content))
            {
                return;
            }

            if (_toastGroup == null)
            {
                Log.Warning(LogTags.UI_Notice, "토스트 그룹이 설정되지 않았습니다.");
                return;
            }

            _toastGroup.AddToast(content, duration);
        }

        public void ClearAllToasts()
        {
            _toastGroup?.ClearAll();
        }

        #endregion 토스트 메시지 (Toast)

        public void ClearAll()
        {
            ClearCurrentAnnouncement();
            ClearAllToasts();
        }
    }
}