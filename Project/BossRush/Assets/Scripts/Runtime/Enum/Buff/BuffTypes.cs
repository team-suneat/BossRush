namespace TeamSuneat
{
    public enum BuffTypes
    {
        None,

        /// <summary>
        /// ��� ����
        /// </summary>
        InstantDamage,

        /// <summary>
        /// ��� ����� ȸ��
        /// </summary>
        InstantHeal,

        /// <summary>
        /// ����� ���� ȸ��
        /// </summary>
        HealOverTime,

        /// <summary>
        /// �ɷ�ġ
        /// </summary>
        Stat,

        /// <summary>
        /// �����̻�
        /// </summary>
        StateEffect,

        /// <summary>
        /// ���� �ð� ���� ���� ����
        /// </summary>
        IntervalLevel,

        /// <summary>
        /// �鿪
        /// </summary>
        Immune,
    }

    public static class BuffTypeChecker
    {
        public static bool IsHeal(this BuffTypes buffType)
        {
            switch (buffType)
            {
                case BuffTypes.InstantHeal:
                case BuffTypes.HealOverTime:
                    return true;
            }

            return false;
        }
    }
}