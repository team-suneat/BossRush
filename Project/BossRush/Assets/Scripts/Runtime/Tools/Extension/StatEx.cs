namespace TeamSuneat
{
    public static class StatEx
    {
        public static int GetValueByLevel(int value, int valueByLevel, int level)
        {
            if (level == 0)
            {
                return 0;
            }
            else if (level == 1)
            {
                return value;
            }
            else if (valueByLevel == 0)
            {
                return value;
            }
            else
            {
                return value + (valueByLevel * (level - 1));
            }
        }

        public static float GetValueByLevel(float value, float valueByLevel, int level)
        {
            if (level == 0)
            {
                return 0f;
            }
            else if (level == 1)
            {
                return value;
            }
            else if (valueByLevel.IsZero())
            {
                return value;
            }
            else
            {
                return value + (valueByLevel * (level - 1));
            }
        }

        public static float GetValueByStack(float value, float valueByStack, int stack)
        {
            if (stack == 0 || stack == 1)
            {
                return value;
            }
            else
            {
                return value + (valueByStack * (stack - 1));
            }
        }
    }
}