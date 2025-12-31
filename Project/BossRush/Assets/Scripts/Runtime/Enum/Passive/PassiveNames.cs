namespace TeamSuneat
{
    public enum PassiveNames
    {
        None,

        // 캐릭터 패시브

        ShieldBash,     // 방패 치기 - 쉴드량의 50% 만큼 추가 데미지
        BeginningAndEnd,// 시작과 끝 - 첫번째 슬롯과 마지막 슬롯의 공격 치명타 확률 +30%, 치명타 피해 +100%
        Slaughter,      // 살육 - 최대 체력 100당 데미지 20% 증가
        ShimmeringLuck, // 번쩍이는 행운 - 같은 슬롯 3개 이상이 나올 때 마다 데미지 +0.5%
        FestivalOfPain, // 고통의 축제 - 도트 피해 입고 있는 적 한명 당 데미지 +1% (출혈, 독, 화염)
        Blacksmith,     // 대장장이 - 무기 옵션이 3개로 등장할 확률 15%
        TruthOfAlchemy, // 연성의 진 - 물약 먹을 때마다 데미지 +0.2%
        Counter,        // 카운터 - 회피 시 가장 좌측 슬롯 1개를 돌려 공격 한다
        BloodSplash,    // 피 뿌리기 - 최대 체력 상태에서 흡혈에 성공한 수치만큼 필드 내 모든 적에게 피해
        CoinToss,       // 동전 던지기 - 보유한 골드의 0.1% 만큼 데미지 증가
        ThornySlot,     // 가시 돋친 슬롯 - 슬롯에서 송곳 등장 (송곳: 보유한 가시 데미지 입히고 다시 슬롯 돌림)
        BattlefieldOfCarnage, // 학살의 전장 - 발사체가 튕길 때 추가 발사체 1개 (최대 3개) 1-2-4-8
        Gambler,        // 타짜 - 자동으로 슬롯이 멈춘다. 한턴에 한번 원하는 슬롯만 다시 돌려볼 수 있다

        // 무기 패시브

        WarriorSword,   // 전사의 검 - 적중 시 쉴드 1 획득
        ExecutionerDagger,  // 처형의 단검 - 처형 확률 2% (보스에게는 치명타 확정 및 치명타 피해 x3.0)
        CrimsonAxe, // 피의 도끼 - 적 턴 종료 시 출혈 (데미지 50%)
        LightningSpear, // 번개의 창 - 15% 확률로 1턴 감전 (보스는 행동불가 면역)
        FlameMace,  // 화염 방망이 - 적 턴 종료 시 화염 (데미지 30%)
    }
}