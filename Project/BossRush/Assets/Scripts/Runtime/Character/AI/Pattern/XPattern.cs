using Sirenix.OdinInspector;

namespace TeamSuneat
{
    [System.Serializable]
    public class XPattern
    {
        [FoldoutGroup("#Pattern")][ReadOnly] public int Index;

        [FoldoutGroup("#Pattern")] public string Name;

        [FoldoutGroup("#Pattern")] public float CooldownTime;

        [FoldoutGroup("#Pattern")] public float DelayTime;

        [FoldoutGroup("#Pattern")][ReadOnly] public bool IsCooldown;

        [FoldoutGroup("#Pattern")] public Order Order;

        [FoldoutGroup("#Pattern")] public XPatternStep[] Steps;

        public XPatternStep GetStep()
        {
            if (Steps != null)
            {
                if (Steps.Length > Order.Current)
                {
                    return Steps[Order.Current];
                }
            }

            return null;
        }

        public PatternStepNames GetStepName()
        {
            if (Steps != null)
            {
                if (Steps.Length > Order.Current)
                {
                    return Steps[Order.Current].StepName;
                }
            }

            return PatternStepNames.None;
        }

        public int GetStepOrder()
        {
            if (Steps != null)
            {
                if (Steps.Length > Order.Current)
                {
                    if (Steps[Order.Current].UseRandomOrder)
                    {
                        return RandomEx.Range(0, Steps[Order.Current].OrderMaxIndex);
                    }
                    else
                    {
                        return Steps[Order.Current].OrderIndex;
                    }
                }
            }

            return 0;
        }

        public void FirstStep()
        {
            Order.First();
        }

        public void NextStep()
        {
            _ = Order.Next();
        }

        public void RefreshOrderMax()
        {
            Order.SetMax(Steps.Length - 1);
        }
    }
}