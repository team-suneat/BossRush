using TeamSuneat.Data;

namespace TeamSuneat
{
    public enum VitalConsumeTypes
    {
        None,

        FixedLife,
        FixedBarrier,
        FixedResource,
        FixedPulse,
        FixedResourceAndPulse,
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

                case VitalConsumeTypes.FixedBarrier:
                    {
                        return JsonDataManager.FindStringData("Skill_Details_BarrierCost");
                    }
                case VitalConsumeTypes.FixedResource:
                    {
                        return JsonDataManager.FindStringData("Skill_Details_ResourceCost");
                    }
                case VitalConsumeTypes.FixedPulse:
                    {
                        return JsonDataManager.FindStringData("Skill_Details_PulseCost");
                    }
                default:
                    {
                        return null;
                    }
            }
        }

    }
}