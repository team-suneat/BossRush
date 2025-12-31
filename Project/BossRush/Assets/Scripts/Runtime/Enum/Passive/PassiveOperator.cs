namespace TeamSuneat.Passive
{
    public enum PassiveOperator
    {
        None,

        /// <summary>
        /// 미만
        /// </summary>
        Under,

        /// <summary>
        /// 초과
        /// </summary>
        Over,

        /// <summary>
        /// 동일
        /// </summary>
        Equal,

        /// <summary>
        /// 이하
        /// </summary>
        Below,

        /// <summary>
        /// 이상
        /// </summary>
        More,
    }

    public static class PassiveOperatorEx
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="passiveOperator">연산자</param>
        /// <param name="current">현재 값</param>
        /// <param name="criteria">기준 값</param>
        /// <returns></returns>
        public static bool Compare(this PassiveOperator passiveOperator, int current, int criteria)
        {
            switch (passiveOperator)
            {
                case PassiveOperator.Under:
                    if (current < criteria)
                    {
                        return true;
                    }
                    break;

                case PassiveOperator.Over:
                    if (current > criteria)
                    {
                        return true;
                    }
                    break;

                case PassiveOperator.Equal:
                    if (current == criteria)
                    {
                        return true;
                    }
                    break;

                case PassiveOperator.Below:
                    if (current <= criteria)
                    {
                        return true;
                    }
                    break;

                case PassiveOperator.More:
                    if (current >= criteria)
                    {
                        return true;
                    }
                    break;
            }

            return false;
        }

        public static bool Compare(this PassiveOperator passiveOperator, float current, float criteria)
        {
            switch (passiveOperator)
            {
                case PassiveOperator.Under:
                    if (current < criteria)
                    {
                        return true;
                    }
                    break;

                case PassiveOperator.Over:
                    if (current > criteria)
                    {
                        return true;
                    }
                    break;

                case PassiveOperator.Equal:
                    if (current == criteria)
                    {
                        return true;
                    }
                    break;

                case PassiveOperator.Below:
                    if (current <= criteria)
                    {
                        return true;
                    }
                    break;

                case PassiveOperator.More:
                    if (current >= criteria)
                    {
                        return true;
                    }
                    break;
            }

            return false;
        }
    }
}