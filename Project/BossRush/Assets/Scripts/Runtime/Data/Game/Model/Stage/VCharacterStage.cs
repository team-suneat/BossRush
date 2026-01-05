using System;

namespace TeamSuneat.Data.Game
{
    [System.Serializable]
    public class VCharacterStage
    {
        [NonSerialized]
        public StageNames CurrentStage;
        public string CurrentStageString;

        public void OnLoadGameData()
        {
            EnumEx.ConvertTo(ref CurrentStage, CurrentStageString);
        }

        public void SelectStage(StageNames stageName)
        {
            CurrentStage = stageName;
            CurrentStageString = stageName.ToString();

            Log.Info(LogTags.GameData_Stage, "현재 스테이지를 선택합니다. {0}, {1}", stageName.ToLogString(), CurrentStageString);

            GlobalEvent.Send(GlobalEventType.GAME_DATA_STAGE_SET);
        }

        public static VCharacterStage CreateDefault()
        {
            VCharacterStage defaultStage = new VCharacterStage();
            defaultStage.SelectStage(StageNames.Boss1);
            return defaultStage;
        }
    }
}