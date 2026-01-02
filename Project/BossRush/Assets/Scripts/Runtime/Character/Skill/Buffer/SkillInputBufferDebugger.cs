using System.Text;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat
{
    /// <summary>
    /// 기술 입력 버퍼의 디버깅을 위한 클래스입니다.
    /// </summary>
    public class SkillInputBufferDebugger : XBehaviour
    {
        [FoldoutGroup("#Debug Settings")]
        [LabelText("디버그 활성화")]
        public bool EnableDebug = false;

        [FoldoutGroup("#Debug Settings")]
        [LabelText("화면에 표시")]
        public bool ShowOnScreen = true;

        [FoldoutGroup("#Debug Settings")]
        [LabelText("로그 출력")]
        public bool EnableLogging = false;

        [FoldoutGroup("#Debug Settings")]
        [LabelText("자동 정리")]
        public bool AutoCleanup = true;

        [FoldoutGroup("#Display Settings")]
        [LabelText("표시 위치")]
        public Vector2 DisplayPosition = new Vector2(10, 10);

        [FoldoutGroup("#Display Settings")]
        [LabelText("폰트 크기")]
        public int FontSize = 14;

        [FoldoutGroup("#Display Settings")]
        [LabelText("배경 색상")]
        public Color BackgroundColor = new Color(0, 0, 0, 0.7f);

        [FoldoutGroup("#Display Settings")]
        [LabelText("텍스트 색상")]
        public Color TextColor = Color.white;

        private SkillInputBufferManager _bufferManager;
        private SkillInputBufferSettings _settings;
        private GUIStyle _debugStyle;
        private GUIStyle _backgroundStyle;

        protected override void OnStart()
        {
            if (!GameDefine.EDITOR_OR_DEVELOPMENT_BUILD)
            {
                SetActive(false);
                return;
            }

            base.OnStart();

            // GUI 스타일 초기화
            InitializeGUIStyles();
        }

        // base.OnStart

        #region Global Event

        protected override void RegisterGlobalEvent()
        {
            base.RegisterGlobalEvent();

            GlobalEvent.Register(GlobalEventType.PLAYER_CHARACTER_BATTLE_READY, OnPlayerCharacterBattleReady);
        }

        protected override void UnregisterGlobalEvent()
        {
            base.UnregisterGlobalEvent();

            GlobalEvent.Unregister(GlobalEventType.PLAYER_CHARACTER_BATTLE_READY, OnPlayerCharacterBattleReady);
        }

        private void OnPlayerCharacterBattleReady()
        {
            // 자동 대상 선택이 활성화되어 있으면 플레이어 캐릭터 사용
            if (CharacterManager.Instance?.Player != null)
            {
                _bufferManager = CharacterManager.Instance.Player.Skill.GetBufferManager();
                _settings = CharacterManager.Instance.Player.Skill.BufferSettings;
            }

            if (_bufferManager != null)
            {
                Log.Progress(LogTags.Skill_Buffer, "[Buffer] 버퍼 디버거가 초기화되었습니다.");
            }
        }

        #endregion Global Event

        private void InitializeGUIStyles()
        {
            _debugStyle = new GUIStyle();
            _debugStyle.fontSize = FontSize;
            _debugStyle.normal.textColor = TextColor;
            _debugStyle.wordWrap = true;

            _backgroundStyle = new GUIStyle();
            _backgroundStyle.normal.background = CreateTexture(BackgroundColor);
        }

        private Texture2D CreateTexture(Color color)
        {
            var texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, color);
            texture.Apply();
            return texture;
        }

        //

        private void Update()
        {
            if (!EnableDebug || _bufferManager == null)
            {
                return;
            }

            // 자동 정리
            if (AutoCleanup)
            {
                _bufferManager.CleanupExpiredBuffers();
            }

            // 로그 출력
            if (EnableLogging)
            {
                _bufferManager.LogBufferStatus();
            }
        }

        private void OnGUI()
        {
            if (!EnableDebug || !ShowOnScreen || _bufferManager == null)
            {
                return;
            }

            // GUI 스타일 업데이트
            _debugStyle.fontSize = FontSize;
            _debugStyle.normal.textColor = TextColor;

            // 디버그 정보 생성
            string debugInfo = GenerateDebugInfo();

            // 화면에 표시
            DisplayDebugInfo(debugInfo);
        }

        private string GenerateDebugInfo()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<기술 입력 버퍼>");
            sb.AppendLine($"버퍼 개수: {_bufferManager.BufferCount}");
            sb.AppendLine($"버퍼 비어있음: {_bufferManager.IsEmpty}");

            if (_settings != null)
            {
                sb.AppendLine($"버퍼 활성화: {_settings.EnableInputBuffer}");
                sb.AppendLine($"기본 지속시간: {_settings.DefaultBufferDuration:F2}s");
                sb.AppendLine($"최대 버퍼 개수: {_settings.MaxBufferCount}");
                sb.AppendLine($"입력 쿨타임 활성화: {_settings.EnableInputCooldown}");
                sb.AppendLine($"기본 쿨타임: {_settings.InputCooldownDuration:F2}s");

                // 공중/지면 상태 관련 설정
                if (_settings.EnableGroundStateCheck)
                {
                    sb.AppendLine($"공중/지면 상태 체크: 활성화");
                    sb.AppendLine($"착지 시 공중 버퍼 삭제: {_settings.ClearAirBufferOnLanding}");
                    sb.AppendLine($"상태 불일치 시 무시: {_settings.IgnoreStateMismatch}");
                }
                else
                {
                    sb.AppendLine($"공중/지면 상태 체크: 비활성화");
                }
            }

            // 상세 버퍼 정보
            if (!_bufferManager.IsEmpty)
            {
                sb.AppendLine("");
                sb.AppendLine("<버퍼 상세 정보>");
                sb.AppendLine(_bufferManager.GetDebugInfo());
            }

            // 쿨타임 상태 정보
            if (_settings?.EnableInputCooldown == true)
            {
                sb.AppendLine("");
                sb.AppendLine("<쿨타임 상태>");
                sb.AppendLine($"쿨타임 활성화: {_settings.EnableInputCooldown}");
                sb.AppendLine($"기본 쿨타임: {_settings.InputCooldownDuration:F2}s");

                // 최근 입력 시간 정보 (버퍼 매니저에서 제공하는 경우)
                if (_bufferManager != null)
                {
                    var lastInputTime = _bufferManager.GetLastInputTime();
                    if (lastInputTime > 0f)
                    {
                        float timeSinceLastInput = Time.time - lastInputTime;
                        bool isOnCooldown = timeSinceLastInput < _settings.InputCooldownDuration;
                        sb.AppendLine($"마지막 입력: {timeSinceLastInput:F2}s 전");
                        sb.AppendLine($"쿨타임 상태: {(isOnCooldown ? "쿨타임 중" : "사용 가능")}");
                    }
                }
            }

            return sb.ToString();
        }

        private void DisplayDebugInfo(string debugInfo)
        {
            // 텍스트 크기 계산
            Vector2 textSize = _debugStyle.CalcSize(new GUIContent(debugInfo));
            Rect backgroundRect = new Rect(DisplayPosition.x, DisplayPosition.y,
                textSize.x + 20, textSize.y + 20);

            // 배경 그리기
            GUI.Box(backgroundRect, "", _backgroundStyle);

            // 텍스트 그리기
            Rect textRect = new Rect(DisplayPosition.x + 10, DisplayPosition.y + 10,
                textSize.x, textSize.y);
            GUI.Label(textRect, debugInfo, _debugStyle);
        }

        private void OnDestroy()
        {
            // 텍스처 정리
            if (_backgroundStyle?.normal?.background != null)
            {
                DestroyImmediate(_backgroundStyle.normal.background);
            }
        }
    }
}