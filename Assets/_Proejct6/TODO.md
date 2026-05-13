# Project6 Card Battle Handoff

이 문서는 `_Proejct6` 전투 시스템을 이어서 작업하기 위한 인수인계 문서다.

현재 구현 범위:
- 카드 정의 데이터
- 카드 인스턴스
- 덱 / 손패 / 버림더미
- 숫자 드로우
- 스킬 배치
- 숫자 장착
- 효과 실행
- 턴 시작 / 실행 / 종료 플로우

아직 구현하지 않은 범위:
- View
- Presenter
- Drag & Drop
- 적 카드 AI
- 시너지 시스템
- 연출 / 애니메이션

---

## 1. 핵심 구조 요약

### Definition

카드의 원본 데이터다.

위치:
- `Component/Card/CardDefinition.cs`
- `Component/NumCard/NumCardDefinition.cs`
- `Component/SkillCard/SkillCardDefinition.cs`

역할:
- `NumCardDefinition`: 숫자 값 보관
- `SkillCardDefinition`: 이름, 설명, 역할, 숫자 조건, 효과 목록 보관

### Instance

전투 중 실제 카드 한 장이다.

위치:
- `Runtime/CardInstance.cs`

예:
```text
NumCardDefinition(5)는 하나의 데이터
CardInstance는 전투 중 실제 5 카드 한 장
```

### Pile

전투 중 카드 묶음이다.

위치:
- `Runtime/CardPile.cs`

사용 예:
- `NumberDeck`
- `NumberHand`
- `NumberDiscard`
- `SkillDeck`
- `SkillHand`
- `SkillDiscard`

### State

전투 상태 전체를 들고 있다.

위치:
- `Runtime/BattleState.cs`

여기에 덱, 손패, 버림더미, 슬롯, 사용 숫자 기록이 있다.

### System

상태를 변경하는 로직이다.

위치:
- `System/`

View는 System을 직접 만지지 않고, 보통 `BattleFlow`만 사용한다.

---

## 2. 가장 중요한 진입점

위치:
- `System/BattleFlow.cs`

Presenter는 이 클래스만 알면 된다.

기본 사용:
```csharp
BattleFlow battleFlow = new BattleFlow();

battleFlow.Setup(
    numCardPool,
    skillCardPool,
    50,
    50,
    3
);

battleFlow.StartTurn();
```

View 갱신용 메소드:
```csharp
battleFlow.GetNumberHandCards();
battleFlow.GetSkillHandCards();
battleFlow.GetSkillSlots();
battleFlow.GetOrderSlotNumber();
battleFlow.GetPreviewNextNumberDrawCount();
```

입력 연결용 메소드:
```csharp
battleFlow.PlaceNumberInOrderSlot(numberCard);
battleFlow.ReturnOrderSlotNumber();

battleFlow.PlaceSkillCard(skillCard);
battleFlow.PlaceNumberOnSkill(skillCard, numberCard);
battleFlow.ReturnNumberFromSkill(skillCard, numberCard);

battleFlow.ExecuteTurn();
battleFlow.StartTurn();
```

이벤트:
```csharp
battleFlow.BattleChanged += RefreshView;
battleFlow.TurnStarted += OnTurnStarted;
battleFlow.TurnExecuted += OnTurnExecuted;
battleFlow.TurnEnded += OnTurnEnded;
```

---

## 3. Presenter 연결 방식

Presenter의 역할:
```text
View 입력을 BattleFlow 명령으로 바꾼다.
BattleFlow 상태를 View 표시 데이터로 넘긴다.
```

추천 형태:
```csharp
public class BattlePresenter : MonoBehaviour
{
    [SerializeField] private NumCardPool numCardPool;
    [SerializeField] private SkillCardPool skillCardPool;

    private BattleFlow battleFlow;

    private void Start()
    {
        battleFlow = new BattleFlow();
        battleFlow.BattleChanged += RefreshView;

        battleFlow.Setup(numCardPool, skillCardPool, 50, 50, 3);
        battleFlow.StartTurn();
    }

    private void RefreshView()
    {
        // NumberHandView.Show(battleFlow.GetNumberHandCards());
        // SkillHandView.Show(battleFlow.GetSkillHandCards());
        // SkillHandView.Show(battleFlow.GetSkillHandCards(), battleFlow.GetSkillSlots());
        // OrderSlotView.Show(battleFlow.GetOrderSlotNumber());
    }
}
```

중요:
```text
View는 BattleFlow를 몰라도 된다.
View는 클릭/드래그 이벤트만 Presenter에게 알려준다.
```

---

## 4. View 구성 추천

### NumberHandView

역할:
- 숫자카드 손패 표시
- 오른쪽 아래 부채꼴 배치
- 숫자카드 클릭 / 드래그 시작 이벤트 발생

입력:
```csharp
IReadOnlyList<CardInstance> numberCards
```

출력 이벤트:
```csharp
OnNumberCardClicked(CardInstance card)
OnNumberCardDragStarted(CardInstance card)
```

### SkillHandView

역할:
- 스킬카드 손패 표시
- ScrollView Content 아래 가로 나열
- 스킬카드 클릭 이벤트 발생

입력:
```csharp
IReadOnlyList<CardInstance> skillCards
```

출력 이벤트:
```csharp
OnSkillCardClicked(CardInstance card)
```

### SkillCardView

역할:
- 플레이어가 배치한 스킬카드 표시
- 장착된 숫자카드 표시
- 숫자 드롭 받기
- 숫자 회수 이벤트 발생

입력:
```csharp
IReadOnlyList<CardInstance> skillCards
IReadOnlyList<SkillSlotState> equippedSlots
```

출력 이벤트:
```csharp
OnSkillCardClicked(CardInstance skillCard)
OnNumberDroppedOnSkill(CardInstance skillCard, CardInstance numberCard)
```

### OrderSlotView

역할:
- 순서 슬롯 표시
- 숫자 드롭 받기
- 순서 숫자 회수

입력:
```csharp
CardInstance orderNumber
```

출력 이벤트:
```csharp
OnNumberDroppedOnOrderSlot(CardInstance numberCard)
OnOrderNumberReturned()
```

### BattleStatusView

역할:
- 플레이어 HP
- 적 HP
- 이번 턴 사용 숫자 수
- 다음 턴 숫자 드로우 예상

입력:
```csharp
BattleState state
int nextNumberDrawCount
```

---

## 5. 시스템 수정 위치

### 숫자 드로우 규칙 수정

파일:
- `Runtime/BattleState.cs`

메소드:
```csharp
GetNextNumberDrawCount()
```

현재 규칙:
```text
첫 턴 5장
이후 지난 턴 사용 숫자 수 + 1
최소 2장
최대 5장
```

### 실제 드로우 처리 수정

파일:
- `System/DrawSystem.cs`

수정할 때:
- 덱 부족 시 버림더미 섞기
- 손패 최대치
- 특수 드로우

### 카드 배치 규칙 수정

파일:
- `System/PlacementSystem.cs`

수정할 때:
- 숫자카드를 순서 슬롯에 넣는 규칙
- 스킬카드를 필드에 놓는 규칙
- 숫자를 스킬카드에 장착하는 규칙
- 장착 회수 규칙

### 숫자 조건 수정

파일:
- `System/NumberRequirementChecker.cs`

수정할 때:
- 이상 / 이하 / 같은 값
- 홀수 / 짝수
- 범위 조건
- 여러 숫자 조합 조건

### 효과 실행 순서 수정

파일:
- `System/SkillResolveSystem.cs`
- `System/EffectExecutor.cs`

수정할 때:
- 방어 선처리
- 일반 스킬 처리
- 스킬 후처리
- 특정 카테고리 효과만 실행

### 턴 종료 처리 수정

파일:
- `System/TurnEndSystem.cs`

수정할 때:
- 사용한 숫자 버림더미 이동
- 사용한 스킬 버림더미 이동
- 미사용 숫자 유지 / 버림
- 미사용 스킬 유지 / 버림
- 지난 턴 사용 숫자 수 저장

### 전체 플로우 수정

파일:
- `System/BattleFlow.cs`

수정할 때:
- 턴 시작 순서
- 턴 실행 순서
- 턴 종료 후 다음 턴 진입
- 외부에 공개할 메소드 추가

---

## 6. Effect 추가 방법

새 효과는 `CardEffect`를 상속한다.

예:
```csharp
[CreateAssetMenu(fileName = "DrawNumberEffect", menuName = "Card Effect/Draw Number")]
public class DrawNumberEffect : CardEffect
{
    public override void Apply(BattleContext context, SkillResolveData data)
    {
        // 구현
    }
}
```

Effect에는 두 가지 분류가 있다.

### EffectCategory

위치:
- `Component/SkillCard/EffectCategory.cs`

용도:
```text
이 효과가 어떤 종류인지 나타낸다.
Damage / Block / Heal / Draw / Buff
```

### EffectTiming

위치:
- `Component/SkillCard/EffectTiming.cs`

용도:
```text
언제 실행할지 나타낸다.
Normal
BeforeEnemyAttack
AfterSkillResolve
```

방어처럼 적 공격 전에 적용되어야 하는 효과는:
```text
Timing = BeforeEnemyAttack
```

일반 공격 / 회복은:
```text
Timing = Normal
```

---

## 7. 연출 추가 방식

시스템 안에 애니메이션 코드를 넣지 않는다.

추천 흐름:
```text
BattleFlow 이벤트 발생
Presenter가 이벤트 받음
View가 연출 실행
연출 종료 후 다음 명령 호출
```

예:
```text
카드 드로우
-> BattleChanged
-> Presenter.RefreshView()
-> NumberHandView가 카드 생성 / 이동 연출
```

턴 실행 연출:
```text
Execute 버튼 클릭
-> Presenter가 View 입력 잠금
-> battleFlow.ExecuteTurn()
-> TurnExecuted 이벤트
-> View가 공격/방어/피해 연출
-> 연출 끝나면 battleFlow.StartTurn()
```

나중에 연출을 더 세밀하게 하고 싶으면 `BattleLog` 또는 `BattleEvent`를 추가한다.

추천 추가 구조:
```text
BattleEvent
- DrawCard
- PlaceCard
- DealDamage
- GainBlock
- Heal
- DiscardCard
```

이 구조가 생기면:
```text
System은 BattleEvent만 기록
View는 BattleEvent를 읽고 순서대로 연출
```

---

## 8. 다음 구현 TODO

### 1순위: View 없이 시스템 테스트

목표:
```text
숫자 덱 20장 생성
첫 턴 숫자 5장 드로우
스킬카드 3장 드로우
숫자 장착
스킬 실행
턴 종료 후 다음 드로우 수 확인
```

필요:
- 간단한 테스트 MonoBehaviour
- 또는 Debug.Log 기반 테스트 러너

### 2순위: 기본 View

만들 것:
- `NumberHandView`
- `SkillHandView`
- `SkillCardView`
- `OrderSlotView`
- `BattleStatusView`

### 3순위: Presenter

만들 것:
- `BattlePresenter`

역할:
- `BattleFlow` 생성
- View 이벤트 연결
- 상태 변경 시 View 갱신

### 4순위: Drag & Drop

연결:
- 숫자카드 -> 순서 슬롯
- 숫자카드 -> 스킬 슬롯
- 장착 숫자 회수

### 5순위: 시너지 시스템

만들 것:
- `SynergyDefinition`
- `SynergyCondition`
- `SynergyEffect`
- `SynergySystem`

읽을 데이터:
- `BattleState.UsedNumbersThisTurn`
- `BattleState.OrderSlotNumber`
- `BattleState.SkillSlots`

### 6순위: 적 카드 시스템

만들 것:
- 적 숫자 덱 / 손패 / 버림더미
- 적 스킬 덱 / 손패 / 버림더미
- `EnemyCardAiSystem`

초기 AI:
```text
가장 큰 숫자를 순서 슬롯에 둔다.
첫 번째 공격 스킬에 가장 큰 숫자를 장착한다.
```

---

## 9. 작업 원칙

지켜야 할 것:
- View는 전투 계산을 하지 않는다.
- Presenter는 중계만 한다.
- System은 View를 모른다.
- Definition은 원본 데이터다.
- Instance는 전투 중 실제 카드다.
- Pile은 카드 묶음이다.
- Effect는 `Apply()`로만 실행한다.

헷갈릴 때 기준:
```text
데이터인가? -> Definition
전투 중 카드 한 장인가? -> CardInstance
카드 여러 장인가? -> CardPile
상태 변경 규칙인가? -> System
화면 표시인가? -> View
입력 연결인가? -> Presenter
```
