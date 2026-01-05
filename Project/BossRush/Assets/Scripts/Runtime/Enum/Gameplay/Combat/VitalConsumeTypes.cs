using TeamSuneat.Data;

namespace TeamSuneat
{
    public enum VitalConsumeTypes
    {
        None,

        FixedLife,
        FixedBarrier,
        FixedResource,

        MaxLifePercent,
        MaxBarrierPercent,
        MaxResourcePercent,

        CurrentLifePercent,
        CurrentBarrierPercent,
        CurrentResourcePercent,
    }

    public static class VitalEnumExtension
    {
        public static StringData GetFormatStringData(this VitalConsumeTypes consumeType)
        {
            switch (consumeType)
            {
                case VitalConsumeTypes.FixedLife:
                    {
                        return JsonDataManager.FindStringData("Skill_Details_LifeCost");
                    }

                case VitalConsumeTypes.MaxLifePercent:
                    {
                        return JsonDataManager.FindStringData("Skill_Details_MaxLifeCost");
                    }
                case VitalConsumeTypes.CurrentLifePercent:
                    {
                        return JsonDataManager.FindStringData("Skill_Details_CurrentLifeCost");
                    }
                case VitalConsumeTypes.FixedBarrier:
                    {
                        return JsonDataManager.FindStringData("Skill_Details_BarrierCost");
                    }
                case VitalConsumeTypes.MaxBarrierPercent:
                    {
                        return JsonDataManager.FindStringData("Skill_Details_MaxBarrierCost");
                    }
                case VitalConsumeTypes.CurrentBarrierPercent:
                    {
                        return JsonDataManager.FindStringData("Skill_Details_CurrentBarrierCost");
                    }
                case VitalConsumeTypes.FixedResource:
                    {
                        return JsonDataManager.FindStringData("Skill_Details_ResourceCost");
                    }
                case VitalConsumeTypes.MaxResourcePercent:
                    {
                        return JsonDataManager.FindStringData("Skill_Details_MaxResourceCost");
                    }
                case VitalConsumeTypes.CurrentResourcePercent:
                    {
                        return JsonDataManager.FindStringData("Skill_Details_CurrentResourceCost");
                    }
                default:
                    {
                        return null;
                    }
            }
        }

        public static bool IsPercentMax(this VitalConsumeTypes consumeType)
        {
            switch (consumeType)
            {
                case VitalConsumeTypes.MaxLifePercent:
                case VitalConsumeTypes.MaxBarrierPercent:
                case VitalConsumeTypes.MaxResourcePercent:
                    return true;
            }
            return false;
        }

        public static bool IsPercentCurrent(this VitalConsumeTypes consumeType)
        {
            switch (consumeType)
            {
                case VitalConsumeTypes.CurrentLifePercent:
                case VitalConsumeTypes.CurrentBarrierPercent:
                case VitalConsumeTypes.CurrentResourcePercent:
                    return true;
            }
            return false;
        }
    }
}