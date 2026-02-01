namespace TeamSuneat
{
    public enum VitalResourceTypes
    {
        None = 0,
        Life,    // 체력 : 기본 생명 자원
        Barrier, // 보호막 : 체력보다 먼저 소모되는 생명 자원
        Mana,    // 마나 : 기술 사용
        Pulse,   // 스태미너 : 패링, 대시 사용
        Poise,   // 포이즈: 균형, 최대 도달 시 기절
    }
}