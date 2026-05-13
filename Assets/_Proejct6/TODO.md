# Project6 Card Battle TODO

현재 상태:
- `NumCardDefinition`: 숫자 값 데이터 있음
- `SkillCardDefinition`: 스킬 기본 데이터, 역할, 숫자 조건, 효과 목록 있음
- `CardEffect`: 추상 효과 자리만 있음
- `NumCardPool`, `SkillCardPool`, `CardInstance`: 아직 구현 전

## 1. CardInstance 먼저 완성

목표: 전투 중 실제 카드 1장을 표현한다.

할 일:
- `MonoBehaviour` 제거
- 일반 C# class로 변경
- `Definition` 참조 추가
- `InstanceId` 추가

예상 형태:
```csharp
public class CardInstance
{
    public object Definition { get; }
    public string InstanceId { get; }
}
```

## 2. CardPile 만들기

목표: 덱, 손패, 버림더미를 같은 방식으로 관리한다.

할 일:
- `Runtime/CardPile.cs` 생성
- `Add`
- `Remove`
- `DrawTop`
- `Shuffle`
- `Clear`
- `GetCards`

사용 위치:
- `NumberDeck`
- `NumberHand`
- `NumberDiscard`
- `SkillDeck`
- `SkillHand`
- `SkillDiscard`

## 3. NumCardPool 정리

목표: 숫자카드 1~10 데이터를 제공한다.

추천:
- `MonoBehaviour` 대신 `ScriptableObject` 또는 일반 Builder 사용

PoC 추천:
- 숫자카드는 고정이므로 C#에서 1~10 각 2장 자동 생성
- 에셋 10개를 꼭 만들 필요 없음

## 4. SkillCardPool 정리

목표: 사용할 스킬카드 정의 목록을 제공한다.

추천:
- `ScriptableObject`로 변경
- `List<SkillCardDefinition>` 보유
- `GetById(string id)` 제공

## 5. SkillCardDefinition에 public getter 추가

목표: 시스템이 스킬 데이터를 읽을 수 있게 한다.

필요 getter:
- `Id`
- `DisplayName`
- `Description`
- `Role`
- `NumberRequirementType`
- `RequiredNumber`
- `RequiredCount`
- `Effects`

## 6. CardEffect Apply 열기

목표: 스킬 효과를 공통 방식으로 실행한다.

할 일:
- `BattleContext` 만들기 전까지는 임시로 주석 유지 가능
- 다음 단계에서 아래 형태로 확정

```csharp
public abstract void Apply(BattleContext context, SkillResolveData data);
```

## 7. 최소 Effect 2개 만들기

목표: 스킬카드가 실제로 결과를 만든다.

만들 것:
- `DamageEffect`
- `BlockEffect`

## 8. BattleState 만들기

목표: 전투 상태를 한 곳에 모은다.

필드:
- 숫자 덱 / 손패 / 버림더미
- 스킬 덱 / 손패 / 버림더미
- 순서 슬롯
- 스킬 슬롯 목록
- 이번 턴 사용 숫자 목록
- 지난 턴 사용 숫자 개수

## 9. DrawSystem 만들기

목표: 숫자카드 순환을 먼저 구현한다.

규칙:
- 첫 턴 숫자 5장
- 이후 `clamp(lastTurnUsedNumberCount + 1, 2, 5)`
- 스킬카드 매 턴 3장
- 덱 부족 시 버림더미 셔플

## 10. PlacementSystem 만들기

목표: 숫자를 슬롯에 배치한다.

메소드:
- `PlaceNumberInOrderSlot`
- `PlaceSkillCard`
- `PlaceNumberOnSkill`
- `ReturnNumber`

## 11. SkillResolveSystem 만들기

목표: 장착된 숫자로 스킬을 발동한다.

흐름:
- 스킬 슬롯 확인
- 숫자 조건 확인
- 숫자 합산
- `CardEffect.Apply`
- 사용 숫자 기록

## 12. BattleFlow 만들기

목표: Presenter가 하나만 알면 되게 만든다.

메소드:
- `Setup`
- `StartTurn`
- `ExecuteTurn`
- `EndTurn`
- `GetNumberHandCards`
- `GetSkillHandCards`

## 바로 다음 추천 작업

1. `CardInstance`를 일반 C# 클래스로 바꾸기
2. `CardPile` 만들기
3. `BattleState` 만들기
4. `DrawSystem`으로 숫자 5장 뽑기까지 테스트

첫 성공 목표:
```text
숫자 덱 20장 생성
첫 턴 숫자 5장 드로우
남은 덱 15장 확인
```
