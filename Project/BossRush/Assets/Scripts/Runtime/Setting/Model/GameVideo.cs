using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace TeamSuneat.Setting
{
    /// <summary>
    /// 게임의 비디오 설정을 관리하는 클래스 (Unity 6 대응 버전)
    /// </summary>
    public class GameVideo
    {
        public int ResolutionX;
        public int ResolutionY;

        public bool IsFullScreen;
        public bool IsBorderless;
        public bool UseVSync;

        public const int MAX_FPS = 120;

        public FullScreenMode ScreenMode { get; private set; }
        public Vector2 ResolutionRate { get; private set; }
        public Vector2Int DefaultResolution { get; private set; }
        public List<Vector2Int> Resolutions { get; private set; } = new();
        public int CurrentResolutionIndex { get; private set; }

        public int LastResolutionIndex
        {
            get
            {
                if (Resolutions.IsValid())
                    return Resolutions.Count - 1;

                return 0;
            }
        }

        public DisplayInfo CurrentDisplay
        {
            get
            {
                if (_displayInfos.IsValid(DisplayCurrentIndex))
                    return _displayInfos[DisplayCurrentIndex];
                else if (_displayInfos.IsValid())
                    return _displayInfos[0];
                else
                    return default;
            }
        }

        public int DisplayCount => _displayInfos.Count;

        private int DisplayCurrentIndex { get; set; }
        private List<DisplayInfo> _displayInfos = new();

        public void Load()
        {
            LoadFullScreen();
            LoadBorderless();
            RefreshScreenMode();

            LoadVSync();
            ApplyVSync();

            LoadDisplay();
            SelectMainDisplay();

            LoadResolutions();
            SetDefaultResolution();
            LoadResolution();
            SetResolutionIndex();
            RefreshResolutionRate();

            ApplyResolution();
        }

        private void LoadFullScreen()
        {
            IsFullScreen = GamePrefs.GetBoolOrDefault(GamePrefTypes.OPTION_USE_FULLSCREEN, GameDefine.DEFAULT_GAME_OPTION_VIDEO_FULL_SCREEN);
        }

        private void LoadBorderless()
        {
            IsBorderless = GamePrefs.GetBoolOrDefault(GamePrefTypes.OPTION_USE_BORDERLESS, GameDefine.DEFAULT_GAME_OPTION_VIDEO_BORDERLESS);
        }

        private void LoadVSync()
        {
            UseVSync = GamePrefs.GetBoolOrDefault(GamePrefTypes.OPTION_USE_VSYNC, GameDefine.DEFAULT_GAME_OPTION_VIDEO_V_SYNC);
        }

        public void LoadDisplay()
        {
#if !(UNITY_SWITCH || UNITY_PLAYSTATION || UNITY_GAMECORE )
            Screen.GetDisplayLayout(_displayInfos);
#endif
        }

        public void LoadResolutions()
        {
            Resolutions.Clear();

            // 현재 디스플레이의 실제 해상도를 기준으로 함
            Vector2Int nativeRes = GetDisplaySize(DisplayCurrentIndex);
            if (nativeRes == Vector2Int.zero)
            {
                // 폴백: 주 디스플레이의 실제 해상도
                nativeRes = new Vector2Int(Screen.currentResolution.width, Screen.currentResolution.height);
            }

            Resolution[] supported = Screen.resolutions;

            // 16:9 해상도 목록 (최대 4K)
            Vector2Int[] standardResolutions = new Vector2Int[]
            {
                new Vector2Int(640, 360),   // nHD
                new Vector2Int(854, 480),   // FWVGA
                new Vector2Int(1024, 576),  // PAL widescreen
                new Vector2Int(1280, 720),  // HD
                new Vector2Int(1600, 900),  // HD+
                new Vector2Int(1920, 1080), // Full HD
                new Vector2Int(2560, 1440), // QHD
                new Vector2Int(3840, 2160), // 4K UHD
            };

            for (int i = 0; i < standardResolutions.Length; i++)
            {
                Vector2Int standardRes = standardResolutions[i];

                // 현재 디스플레이의 최대 해상도 이하인지 확인
                if (nativeRes.x >= standardRes.x && nativeRes.y >= standardRes.y)
                {
                    // Screen.resolutions가 비어있거나 지원하지 않는 경우 폴백
                    bool isSupported = supported.Length == 0;

                    if (!isSupported)
                    {
                        // Screen.resolutions에서 지원되는 해상도인지 확인
                        for (int j = 0; j < supported.Length; j++)
                        {
                            Resolution r = supported[j];
                            if (r.width == standardRes.x && r.height == standardRes.y)
                            {
                                isSupported = true;
                                break;
                            }
                        }
                    }

                    if (isSupported)
                    {
                        Resolutions.Add(standardRes);
                        Log.Info(LogTags.Video, "지원 해상도: {0}x{1}", standardRes.x, standardRes.y);
                    }
                }
            }
        }

        private void LoadResolution()
        {
            ResolutionX = GamePrefs.GetInt(GamePrefTypes.OPTION_RESOLUTION_X, DefaultResolution.x);
            ResolutionY = GamePrefs.GetInt(GamePrefTypes.OPTION_RESOLUTION_Y, DefaultResolution.y);

            Log.Info(LogTags.Video, "해상도 불러오기: {0}x{1}", ResolutionX, ResolutionY);
        }

        //

        public void SwitchFullScreen()
        {
            IsFullScreen = !IsFullScreen;
            GamePrefs.SetBool(GamePrefTypes.OPTION_USE_FULLSCREEN, IsFullScreen);
            RefreshScreenMode();
            ApplyResolution();
        }

        public void SwitchBorderless()
        {
            IsBorderless = !IsBorderless;
            GamePrefs.SetBool(GamePrefTypes.OPTION_USE_BORDERLESS, IsBorderless);
            if (IsFullScreen)
            {
                RefreshScreenMode();
                ApplyResolution();
            }
        }

        //

        private void SelectMainDisplay()
        {
#if !(UNITY_SWITCH || UNITY_PLAYSTATION || UNITY_GAMECORE )
            if (_displayInfos.IsValid())
            {
                DisplayCurrentIndex = 0;
                DisplayInfo mainDisplay = _displayInfos[0];
                Vector2Int center = new Vector2Int(mainDisplay.width / 2, mainDisplay.height / 2);
                Screen.MoveMainWindowTo(mainDisplay, center);
            }
#endif
        }

        //

        private void SetDefaultResolution()
        {
            if (Resolutions.IsValid())
            {
                DefaultResolution = Resolutions[Resolutions.Count - 1];
                Log.Info(LogTags.Video, "기본 해상도: {0}x{1}", DefaultResolution.x, DefaultResolution.y);
            }
        }

        private void SetResolutionIndex()
        {
            for (int i = 0; i < Resolutions.Count; i++)
            {
                if (Resolutions[i].x == ResolutionX && Resolutions[i].y == ResolutionY)
                {
                    CurrentResolutionIndex = i;
                    return;
                }
            }
            CurrentResolutionIndex = LastResolutionIndex;
            SetResolution(CurrentResolutionIndex);
        }

        public void SetResolution(int index)
        {
            if (Resolutions.IsValid(index))
            {
                CurrentResolutionIndex = index;
                ResolutionX = Resolutions[index].x;
                ResolutionY = Resolutions[index].y;

                GamePrefs.SetInt(GamePrefTypes.OPTION_RESOLUTION_X, ResolutionX);
                GamePrefs.SetInt(GamePrefTypes.OPTION_RESOLUTION_Y, ResolutionY);

                ApplyResolution();
                RefreshResolutionRate();
            }
        }

        private void ApplyResolution()
        {
#if !(UNITY_SWITCH || UNITY_PLAYSTATION || UNITY_GAMECORE )
            Screen.SetResolution(ResolutionX, ResolutionY, ScreenMode);
#endif
            Log.Progress(LogTags.Video, "해상도 적용: {0}x{1}, 모드: {2}", ResolutionX, ResolutionY, ScreenMode);
        }

        public void ResetResolution()
        {
            ResolutionX = DefaultResolution.x;
            ResolutionY = DefaultResolution.y;

            GamePrefs.SetInt(GamePrefTypes.OPTION_RESOLUTION_X, ResolutionX);
            GamePrefs.SetInt(GamePrefTypes.OPTION_RESOLUTION_Y, ResolutionY);

            ApplyResolution();
            SetResolutionIndex();
        }

        //

        public void RefreshScreenMode()
        {
            if (IsFullScreen)
            {
                if (IsBorderless)
                    ScreenMode = FullScreenMode.FullScreenWindow;
                else
                    ScreenMode = FullScreenMode.ExclusiveFullScreen;
            }
            else
            {
                ScreenMode = FullScreenMode.Windowed;
            }

            Log.Progress(LogTags.Video, "스크린 모드 갱신: {0}", ScreenMode);
        }

        public void RefreshResolutionRate()
        {
            float rateX = (float)ResolutionX / Screen.width;
            float rateY = (float)ResolutionY / Screen.height;

            ResolutionRate = new Vector2(rateX, rateY);
            // Log.Info(LogTags.Video, "해상도 비율: {0}", ResolutionRate);
        }

        //

        public void ResetVSync()
        {
            UseVSync = GameDefine.DEFAULT_GAME_OPTION_VIDEO_V_SYNC;
            GamePrefs.SetBool(GamePrefTypes.OPTION_USE_VSYNC, UseVSync);
            ApplyVSync();
        }

        public void SwitchVSync()
        {
            UseVSync = !UseVSync;
            GamePrefs.SetBool(GamePrefTypes.OPTION_USE_VSYNC, UseVSync);
            ApplyVSync();
        }

        private void ApplyVSync()
        {
            QualitySettings.vSyncCount = UseVSync ? 1 : 0;
            Application.targetFrameRate = UseVSync ? -1 : MAX_FPS;
        }

        //

        public void SetDefaultValues()
        {
            ResetResolution();
            ResetFullScreen();
            ResetBorderless();
            ResetVSync();

            RefreshResolutionRate();
            RefreshScreenMode();
        }

        public void ResetFullScreen()
        {
            IsFullScreen = GameDefine.DEFAULT_GAME_OPTION_VIDEO_FULL_SCREEN;
            GamePrefs.SetBool(GamePrefTypes.OPTION_USE_FULLSCREEN, IsFullScreen);
        }

        public void ResetBorderless()
        {
            IsBorderless = GameDefine.DEFAULT_GAME_OPTION_VIDEO_BORDERLESS;
            GamePrefs.SetBool(GamePrefTypes.OPTION_USE_BORDERLESS, IsBorderless);
        }

        //

        public Vector2Int GetDisplaySize(int index)
        {
            if (_displayInfos.IsValid(index))
            {
                DisplayInfo display = _displayInfos[index];
                return new Vector2Int(display.width, display.height);
            }

            return Vector2Int.zero;
        }

        public bool TryPrevDisplay()
        {
            int newIndex;

            if (DisplayCurrentIndex == 0)
            {
                newIndex = _displayInfos.Count - 1;
            }
            else
            {
                newIndex = DisplayCurrentIndex - 1;
            }

            if (DisplayCurrentIndex != newIndex)
            {
                DisplayCurrentIndex = newIndex;
                return true;
            }

            return false;
        }

        public bool TryNextDisplay()
        {
            int newIndex;

            if (DisplayCurrentIndex >= _displayInfos.Count - 1)
            {
                newIndex = 0;
            }
            else
            {
                newIndex = DisplayCurrentIndex + 1;
            }

            if (DisplayCurrentIndex != newIndex)
            {
                DisplayCurrentIndex = newIndex;
                return true;
            }

            return false;
        }

        public IEnumerator ProcessMoveDisplay()
        {
            Vector2Int center = new Vector2Int(CurrentDisplay.width, CurrentDisplay.height) / 2;
#if !(UNITY_SWITCH || UNITY_PLAYSTATION || UNITY_GAMECORE )
            yield return Screen.MoveMainWindowTo(CurrentDisplay, center);
#else
            yield return null;
#endif
        }

        public void OnMoveDisplay()
        {
            LoadResolutions();
            SetResolutionIndex();
        }
    }
}