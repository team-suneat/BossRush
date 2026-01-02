# 기술 입력 버퍼 시스템 (Skill Input Buffer System)

## 1. 목적 및 개요
- **기술 입력 버퍼**는 캐릭터의 스킬(기술) 애니메이션이 재생 중일 때 입력된 추가 기술을 임시로 저장하고, 애니메이션 종료 시 즉시 시전할 수 있도록 지원하는 시스템입니다.
- 기존에는 애니메이션이 끝나야만 다음 입력이 가능했으나, 버퍼 시스템을 통해 연속적이고 자연스러운 기술 연계가 가능합니다.

## 2. 주요 클래스 및 구조
- **SkillInputBufferManager**: 입력 버퍼의 저장/관리/만료/쿨타임 체크 등 핵심 로직 담당
- **SkillInputBufferSettings**: Inspector에서 조정 가능한 버퍼 관련 설정값(버퍼 지속시간, 최대 개수, 쿨타임 등) 관리
- **SkillInputBufferDebugger**: 버퍼 상태를 실시간으로 시각화 및 로그로 출력, 테스트 기능 제공
- **SkillSystem.Buffer.cs**: 캐릭터의 SkillSystem에 버퍼 기능을 통합하는 partial 클래스

## 3. 동작 방식
1. **입력 감지**: 스킬 입력이 들어오면, 애니메이션 재생 중 여부를 확인
2. **우선순위 캔슬 체크**: 현재 재생 중인 기술의 우선순위와 비교하여 캔슬 가능 여부 확인
3. **즉시 시전 또는 버퍼 저장**: 
   - 우선순위가 높으면 → 현재 기술 캔슬하고 즉시 시전
   - 우선순위가 낮거나 같으면 → 버퍼에 저장 (기술 카테고리별 우선순위, 지속시간, 쿨타임, 공중/지면 상태 고려)
4. **애니메이션 종료 감지**: 종료 시점에 버퍼에서 가장 우선순위 높은 입력을 꺼내 즉시 시전
5. **쿨타임 처리**: 동일 입력이 프레임 단위로 중복 저장되는 것을 방지하기 위해 입력 쿨타임 적용
6. **우선순위**: 기술 카테고리(SkillCategories)별 우선순위(int값)로 처리
7. **공중/지면 상태 처리**: 입력 시점의 상태를 저장하고, 처리 시점에 상태 불일치를 체크하여 적절히 처리

### 3.1 우선순위에 따른 즉시 시전 취소 시스템
- **우선순위 비교**: 새로 입력된 기술의 우선순위 > 현재 재생 중인 기술의 우선순위
- **캔슬 조건**: `newPriority > currentPriority`일 때만 캔슬 가능
- **캔슬 처리**: 
  - 현재 애니메이션 중단 (`StopPlayingSkillAnimation(false)`)
  - 새 기술 즉시 시전
- **우선순위가 같을 때**: 최신 입력이 우선 처리됨 (`ThenByDescending(buffer => buffer.InputTime)`)

#### 주요 메서드:
- `CanCancelByPriority(int newPriority, int currentPriority)`: 캔슬 가능 여부 확인
- `HasCancellableBuffer(int currentPriority)`: 캔슬 가능한 버퍼 존재 여부 확인
- `GetCancellableBuffer(int currentPriority)`: 캔슬할 버퍼 가져오기

## 4. 버퍼 기능 활용 흐름
### 입력부터 버퍼 처리까지의 전체 과정
1. **TSInputManager**: 플레이어의 키보드/마우스 입력을 감지하여 ActionNames로 변환
2. **CharacterHandleSkill**: 입력된 ActionNames를 받아서 해당하는 스킬을 찾고 시전 시도
3. **SkillSystem.TryCastWithBuffer**: 
   - 애니메이션 재생 중인지 확인
   - 재생 중이면 → 우선순위 캔슬 체크 후 즉시 시전 또는 버퍼 저장
   - 재생 중이 아니면 → 즉시 스킬 시전
4. **SkillInputBufferManager**: 
   - 입력 쿨타임 체크 (중복 입력 방지)
   - 우선순위 비교 및 캔슬 여부 결정
   - 우선순위, 지속시간, 공중/지면 상태를 고려하여 버퍼에 저장
   - 만료된 버퍼 자동 정리
5. **CharacterAnimator**: 애니메이션 종료 시점에 버퍼 처리 트리거
6. **SkillSystem.ProcessBufferedInput**: 
   - 버퍼에서 가장 우선순위 높은 입력을 꺼내서 즉시 시전
   - 우선순위 캔슬 가능한 버퍼가 있으면 현재 기술 캔슬하고 즉시 시전
   - 공중/지면 상태 불일치 시 버퍼 무시 (설정에 따라)
   - 안전장치를 통해 중복 시전 방지
7. **착지 이벤트**: 캐릭터가 지면에 착지할 때 공중 버퍼 자동 삭제 (설정에 따라)

### 핵심 연동 지점
- **입력 시점**: CharacterHandleSkill → SkillSystem.TryCastWithBuffer
- **버퍼 저장**: SkillSystem → SkillInputBufferManager.AddInputBuffer
- **버퍼 처리**: CharacterAnimator → SkillSystem.ProcessBufferedInput
- **설정 관리**: SkillInputBufferSettings를 통해 모든 버퍼 관련 설정값 통합 관리

## 5. 설정 가이드 (SkillInputBufferSettings)
- **EnableInputBuffer**: 버퍼 시스템 전체 활성화 여부
- **DefaultBufferDuration**: 입력이 버퍼에 저장되는 시간(초)
- **MaxBufferCount**: 동시에 저장 가능한 최대 입력 개수
- **EnableInputCooldown**: 입력 중복 방지 쿨타임 활성화
- **InputCooldownDuration**: 입력 쿨타임(초)
- **GetPriorityForCategory(SkillCategories)**: 기술 카테고리별 우선순위 반환 (카테고리 int값 사용)
- **EnableGroundStateCheck**: 공중/지면 상태 체크 활성화
- **ClearAirBufferOnLanding**: 착지 시 공중 버퍼 자동 삭제
- **IgnoreStateMismatch**: 상태 불일치 시 버퍼 무시

## 6. 애니메이션 연동 및 안전장치
- 애니메이션 상태 전환 시점과 버퍼 처리 시점이 겹쳐 중복 시전되는 문제를 방지하기 위해, 애니메이션 재생 명령 시점에 PlayingSkillAnimationName을 설정하고, IsSkillState에서 중복 설정을 방지
- 버퍼 처리 시점을 두 프레임 지연하거나, 중복 처리 방지 플래그를 도입하여 안전하게 동작
- 우선순위 캔슬 시 `StopPlayingSkillAnimation(false)`를 사용하여 애니메이션을 안전하게 중단

## 7. 디버깅 및 테스트
- **SkillInputBufferDebugger**를 통해 실시간 버퍼 상태, 쿨타임 상태, 입력 이력 등을 화면에 표시 가능
- Inspector 버튼으로 버퍼 추가/제거, 쿨타임 테스트, 우선순위 캔슬 테스트 등 다양한 시나리오를 즉시 검증 가능
- 상세 로그를 통해 버퍼 저장/만료/시전/쿨타임/우선순위 캔슬 등 모든 과정을 추적 가능

## 8. 자주 발생하는 문제 & 주의사항
- Inspector에서 설정값을 변경한 후에는 반드시 SkillSystem의 UpdateBufferSettings()를 호출해 실시간 반영 필요
- 버퍼/쿨타임 관련 설정값이 0 또는 음수로 잘못 입력되면 의도치 않은 동작이 발생할 수 있으니, SetDefaultValues()로 초기화 권장
- 애니메이션 상태와 버퍼 처리 타이밍이 꼬이지 않도록, 상태 플래그 및 로그를 적극 활용할 것
- 우선순위 캔슬 시 애니메이션 중단이 제대로 되지 않으면 중복 시전이 발생할 수 있으니 주의

## 9. 확장/개선 아이디어
- 입력 버퍼를 UI로 시각화하여 플레이어에게 피드백 제공
- 카테고리별 세부 쿨타임, 우선순위 커스터마이즈 기능 추가
- 네트워크 환경에서의 입력 버퍼 동기화 지원
- 우선순위 캔슬 시 시각적/청각적 피드백 추가
