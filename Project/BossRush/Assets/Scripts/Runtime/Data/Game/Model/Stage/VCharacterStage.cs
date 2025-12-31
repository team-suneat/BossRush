using System;

namespace TeamSuneat.Data.Game
{
    [System.Serializable]
    public class VCharacterStage
    {
        [NonSerialized]
        public StageNames CurrentStage;
        public string CurrentStageString;

        [NonSerialized]
        public AreaNames CurrentArea;
        public string CurrentAreaString;

        [NonSerialized]
        public StageNames MaxReachedStage;
        public string MaxReachedStageString;

        [NonSerialized]
        public AreaNames MaxReachedArea;
        public string MaxReachedAreaString;

        public int CurrentWave;

        public void OnLoadGameData()
        {
            EnumEx.ConvertTo(ref CurrentStage, CurrentStageString);
            EnumEx.ConvertTo(ref CurrentArea, CurrentAreaString);
            EnumEx.ConvertTo(ref MaxReachedStage, MaxReachedStageString);
            EnumEx.ConvertTo(ref MaxReachedArea, MaxReachedAreaString);

            CurrentWave = 0;
        }

        public void SelectStage(StageNames stageName)
        {
            CurrentStage = stageName;
            CurrentStageString = stageName.ToString();

            Log.Info(LogTags.GameData_Stage, "현재 스테이지를 선택합니다. {0}, {1}", stageName.ToLogString(), CurrentStageString);

            GlobalEvent.Send(GlobalEventType.GAME_DATA_STAGE_SET);
        }

        public void SelectArea(AreaNames areaName)
        {
            CurrentArea = areaName;
            CurrentAreaString = areaName.ToString();

            Log.Info(LogTags.GameData_Stage, "현재 지역을 선택합니다. {0}, {1}", areaName.ToLogString(), CurrentAreaString);
        }

        public void UpdateMaxReachedStage(StageNames stageName)
        {
            if (stageName > MaxReachedStage)
            {
                MaxReachedStage = stageName;
                MaxReachedStageString = stageName.ToString();
            }
        }

        public void UpdateMaxReachedArea(AreaNames areaName)
        {
            if (areaName > MaxReachedArea)
            {
                MaxReachedArea = areaName;
                MaxReachedAreaString = areaName.ToString();
            }
        }

        public void AddCurrentWave()
        {
            CurrentWave++;
        }

        public void ResetCurrentWave()
        {
            CurrentWave = 0;
        }

        public static VCharacterStage CreateDefault()
        {
            VCharacterStage defaultStage = new VCharacterStage();
            defaultStage.SelectArea(AreaNames.StartForest);
            defaultStage.SelectStage(StageNames.Area01_StartForest1);
            defaultStage.UpdateMaxReachedStage(StageNames.Area01_StartForest1);
            defaultStage.UpdateMaxReachedArea(AreaNames.StartForest);
            defaultStage.ResetCurrentWave();
            return defaultStage;
        }
    }
}