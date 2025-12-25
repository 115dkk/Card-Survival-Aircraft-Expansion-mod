# Card Survival: Tropical Island - Aircraft Expansion Mod

## 프로젝트 개요

관광객 캐릭터의 비행기 거처에 공간 확장 기능을 추가하고, 기존에 막혀있던 흰 벽/가죽 바닥 업그레이드를 비행기에서도 사용 가능하게 하는 모드.
게임 설정상 비행기가 절벽 옆에 박혀 있어, 한쪽 벽을 부수고 흙을 파서 거주 공간을 넓히는 컨셉.

## 기술 스택

- Unity 게임 (C#)
- BepInEx 5.x 모딩 프레임워크
- Harmony 2.x 패칭 라이브러리
- ModCore API (reference/ModCore 참조)

## 디렉토리 구조

- `reference/Assembly-CSharp/` - 게임 본체 디컴파일 소스 (읽기 전용)
- `reference/ModCore/` - 모딩 API 디컴파일 소스 (읽기 전용)
- `src/AircraftExpansion/` - 모드 소스 코드

## 핵심 참조 파일

- `reference/ModCore/ModCore.Data/Loader.cs` - 데이터 로딩 API
- `reference/ModCore/ModCore.Data/DataMap.cs` - CardData 매핑
- `reference/ModCore/ModCore.Patcher/` - Harmony 패치 예제

## 구현 목표

### 목표 1: 비행기 공간 확장 시스템

#### Stage 1: 비행기 내벽 해체
- 소요: 6 TP (1.5시간)
- 필요: 날카로운 도끼 1회
- 부산물: 고철 2-3개

#### Stage 2-5: 초기 굴착
- 소요: 24 TP (총 6시간, 4단계. 각 Stage 당 1.5시간.)
- 필요: 삽 8회(각 Stage마다 삽 2회씩 소모)
- 부산물: 마른 흙 12-16개

#### Stage 6-7: 구조 보강
- 소요: 8 TP (총 2시간, 각 Stage마다 1시간)
- 필요: 긴 막대기 6개, 섬유 끈 4개 (각 Stage마다 긴 막대기 3개, 섬유 끈 2개 소모)

#### Stage 8: 접합부 밀봉
- 소요: 4 TP (1시간)
- 필요: 점토 6개, 섬유 2개

#### Stage 9 (확장된 방의 추가 개선으로 존재): 벽면 마감
- 소요: 4 TP (1시간)
- 필요: 생석회 4개, 물(0.8그릇 이상 담김, 코코넛 그릇/점토 그릇/플라스틱 병에 담긴 (깨끗한) 물/오염된 물만 허용)
- 효과: 편안함 보너스, 흰 벽 업그레이드 전제조건

### 목표 2: 기존 집 업그레이드를 비행기에 적용
현재 게임에서 비행기는 흰 벽 업그레이드와 가죽 바닥 업그레이드가 차단되어 있음.
이 모드는 해당 제한을 해제하여 비행기에서도 두 업그레이드를 사용 가능하게 함.
만일 별도의 업그레이드 스타일이고 이것이 비행기에만 누락된 경우, 원본 게임 파일을 참고하여 비행기에도 추가할 필요가 있음.

#### 흰 벽 업그레이드 (White Walls)
- 기존 게임 메커니즘을 비행기에 적용
- 공간 확장 Stage 9(벽면 마감) 완료 시 해금되도록 연동

#### 가죽 바닥 업그레이드 (Leather Floor)
- 기존 게임 메커니즘을 비행기에 적용
- 공간 확장 완료와 무관하게 독립적으로 적용 가능

## 게임 메커니즘 참고

- 1 TP = 15분 게임 내 시간
- 집 업그레이드는 Stage 기반으로 작동
- 각 Stage는 시간, 도구, 재료 소모
- 부산물은 작업 완료 시 손패에 추가(손패가 모자라면 자동으로 바닥에 버리므로)
- 비행기는 현재 흰 벽/가죽 바닥 업그레이드가 막혀있음 (이 모드의 해제 대상)

## 코딩 컨벤션

- 네임스페이스: AircraftExpansion
- Harmony 패치: src/AircraftExpansion/Patches/
- 데이터 정의: src/AircraftExpansion/Data/
- 플러그인 진입점: src/AircraftExpansion/Plugin.cs

---

## ModCore API 참조

### Loader.cs 데이터 로딩 시스템

#### 로딩 순서

1. `LoadBeforeEvent` 발생
2. `InitDatabaseAndAutoRegisterType()` - 게임 내 모든 ScriptableObject 수집
3. `LoadAllTexture2D()` - 텍스처/스프라이트 로드
4. `LoadData()` - 각 등록된 타입별 JSON 파일 로드
5. `FixData()` - 참조 해결 (WarpData 처리)
6. `DataMap.Mapping()` - 카드 데이터 매핑
7. `LoadCompleteEvent` 발생

#### 파일 구조

| 타입                     | 경로                                              |
| ------------------------ | ------------------------------------------------- |
| UniqueIDScriptable 파생  | `[플러그인]/[타입명]/*.json`                      |
| 일반 ScriptableObject    | `[플러그인]/ScriptableObject/[타입명]/*.json`     |
| 텍스처                   | `[플러그인]/Resource/Texture2D/*.png\|jpg\|jpeg`  |

#### WarpType 열거형

JSON에서 기존 게임 객체를 참조/수정할 때 사용:

- `0 None` - 없음
- `1 Copy` - 복사
- `2 Custom` - 커스텀
- `3 Reference` - 기존 객체 참조
- `4 Add` - 배열에 새 요소 추가
- `5 Modify` - 기존 객체 수정
- `6 AddReference` - 기존 UnityEngine.Object 참조를 배열에 추가

#### 참조 해결 시스템 (WarpData)

Unity JsonUtility가 게임 객체 참조를 처리할 수 없으므로 별도 시스템 사용:

**단일 객체 참조:**

```json
{
  "CardImage": null,
  "CardImageWarpData": "SomeSpriteName"
}
```

**배열 참조:**

```json
{
  "SomeCards": [],
  "SomeCardsWarpData": ["Card_UniqueID_1", "Card_UniqueID_2"]
}
```

**기존 객체 수정 (배열에 추가):**

```json
{
  "SomeArrayWarpType": 4,
  "SomeArrayWarpData": [{"field": "value"}]
}
```

#### 주요 API

- `Loader.RegisterType(DataInfo)` - 로딩 전 데이터 타입 등록
- `Loader.PreloadData<T>(name, json)` - 로딩 전 데이터 미리 등록
- `Loader.FromJson<T>(json)` - JSON을 객체로 변환 (WarpData 포함)
- `Loader.FromJsonOverwrite(json, obj)` - 기존 객체에 JSON 덮어쓰기
- `Loader.GameSourceModify(FileInfo)` - 기존 게임 객체 수정

#### 특수 매칭 필드 (GameSourceModify용)

- `MatchTagWarpData`: 특정 태그를 가진 모든 카드에 수정 적용
- `MatchTypeWarpData`: 특정 CardTypes의 모든 카드에 수정 적용

#### 이벤트

- `Loader.LoadBeforeEvent` - 로딩 시작 전
- `Loader.LoadCompleteEvent` - 로딩 완료 후

---

## 게임 클래스 분석 (Assembly-CSharp)

### 집 업그레이드 시스템 개요

게임의 집 업그레이드는 `CardTypes.EnvImprovement` (환경 개선) 타입의 카드로 구현됩니다.
`Explorable` 타입 카드(집, 동굴, 비행기 등)는 `EnvironmentImprovements` 배열을 통해 가능한 업그레이드 목록을 정의합니다.

### 핵심 클래스

#### CardData.cs

카드의 모든 데이터를 정의하는 ScriptableObject:

```csharp
// 카드 종류
public CardTypes CardType;  // Item, Base, Blueprint, EnvImprovement, EnvDamage 등

// 블루프린트 조건 (건설 시작 가능 여부)
public CardOnBoardCondition[] BlueprintCardConditions;
public TagOnBoardCondition[] BlueprintTagConditions;
public StatValueTrigger[] BlueprintStatConditions;

// 건설 진행 조건 (각 단계 진행 가능 여부)
public CardOnBoardCondition[] BuildingCardConditions;
public TagOnBoardCondition[] BuildingTagConditions;
public StatValueTrigger[] BuildingStatConditions;

// 건설 단계 정의
public BlueprintStage[] BlueprintStages;

// 환경 개선/손상 목록 (Explorable 카드용)
public CardDataRef[] EnvironmentImprovements;  // 가능한 업그레이드 목록
public CardDataRef[] EnvironmentDamages;       // 가능한 손상 목록
```

#### CardTypes.cs (열거형)

```csharp
public enum CardTypes
{
    Item,           // 아이템
    Base,           // 기지
    Location,       // 위치
    Event,          // 이벤트
    Environment,    // 환경
    Weather,        // 날씨
    Hand,           // 손
    Blueprint,      // 건설 블루프린트
    Explorable,     // 탐험 가능 장소 (집, 동굴, 비행기 등)
    Liquid,         // 액체
    EnvImprovement, // 환경 개선 (흰 벽, 가죽 바닥 등)
    EnvDamage       // 환경 손상
}
```

#### BlueprintStage.cs

건설의 각 단계 정의:

```csharp
public class BlueprintStage
{
    public BlueprintElement[] RequiredElements;  // 이 단계에 필요한 재료들
}
```

#### BlueprintElement.cs

단계별 필요 재료 정의:

```csharp
public struct BlueprintElement
{
    private CardData RequiredCard;           // 필요한 특정 카드
    private CardTabGroup RequiredTabGroup;   // 또는 카드 그룹
    private int RequiredQuantity;            // 필요 수량
    private LiquidContentCondition RequiredLiquidContent;  // 액체 조건

    // 내구도 조건들
    private DurabilityRequirements Spoilage;
    private DurabilityRequirements Usage;
    private DurabilityRequirements Fuel;
    // ... 등

    public CardStateChange EffectOnIngredient;  // 재료에 적용할 효과
    public bool DontDestroy;                     // 재료 보존 여부
}
```

#### CardOnBoardCondition.cs

특정 카드가 보드에 있어야/없어야 하는 조건:

```csharp
public struct CardOnBoardCondition
{
    public CardData TriggerCard;      // 조건이 되는 카드
    public bool OnlyInHand;           // 손에 있어야 함
    public bool NotInHand;            // 손에 없어야 함
    public bool ExcludeInventories;   // 인벤토리 제외
    public bool Inverted;             // 조건 반전 (해당 카드가 "없어야" 함)
}
```

#### TagOnBoardCondition.cs

특정 태그를 가진 카드가 보드에 있어야/없어야 하는 조건:

```csharp
public struct TagOnBoardCondition
{
    public CardTag TriggerTag;        // 조건이 되는 태그
    public bool OnlyInHand;
    public bool NotInHand;
    public bool ExcludeInventories;
    public bool Inverted;             // 조건 반전
}
```

#### CardTag.cs

카드에 부여할 수 있는 태그:

```csharp
public class CardTag : ScriptableObject
{
    public LocalizedString InGameName;
    public bool UniqueOnBoard;              // 보드에 하나만 존재 가능
    public ActionModifier[] ActionModifiers;
}
```

### 업그레이드 활성화/비활성화 메커니즘

1. **Explorable 카드의 EnvironmentImprovements 배열**
   - 해당 장소에서 가능한 업그레이드 목록 정의
   - 비행기에 흰 벽/가죽 바닥이 없다면 이 배열에 추가 필요

2. **EnvImprovement 카드의 조건 필드**
   - `BlueprintCardConditions` - 특정 카드가 있어야/없어야 건설 시작 가능
   - `BlueprintTagConditions` - 특정 태그가 있어야/없어야 건설 시작 가능
   - `Inverted=true` 인 조건이 있으면 해당 카드/태그가 있을 때 건설 불가

### 모드 구현 방향

비행기에서 흰 벽/가죽 바닥을 활성화하려면:

1. **방법 A: 비행기 카드 수정**
   - 비행기의 `EnvironmentImprovements` 배열에 흰 벽/가죽 바닥 추가

2. **방법 B: 업그레이드 조건 수정**
   - 흰 벽/가죽 바닥의 `BlueprintCardConditions` 또는 `BlueprintTagConditions`에서
   - 비행기를 제외하는 `Inverted` 조건 제거

3. **방법 C: 새 업그레이드 생성**
   - 비행기 전용 새 EnvImprovement 카드 생성

---

## 카드 식별 정보

### 로컬라이제이션 키 기반 카드 이름

게임의 로컬라이제이션 파일(Kr.csv)에서 확인한 카드 이름 패턴:

| 카드 | 로컬라이제이션 키 | 영문명 | 한글명 |
| ---- | ----------------- | ------ | ------ |
| 비행기 (Explorable) | `PlaneCrash` | Plane Crash | 추락한 비행기 |
| 비행기 환경 | `Env_CrashedPlane` | Crashed Plane | 추락한 비행기 |
| 흰 벽 업그레이드 | `Imp_WhiteWashedWalls` | White Washed Walls | 백색 벽 |
| 가죽 바닥 업그레이드 | `Imp_StitchedHideFloor` | Stitched-Hide Floor | 가죽 장식 바닥 |
| 뗏목 가죽 바닥 | `Imp_RaftStitchedHideFloor` | Stitched-Hide Floor | 가죽 장식 바닥 |

### UniqueID 형식

- UniqueID는 32자 16진수 GUID 형식 (예: `e2afb8d82fc6ef040b0ba14929c8ef9a`)
- 게임 에셋 번들에 저장되어 직접 추출하려면 AssetRipper 등의 도구 필요
- 런타임에 `UniqueIDScriptable.AllUniqueObjects` 딕셔너리에서 조회 가능

### WarpData 참조 방식 (기존 모드 분석)

```json
{
  "SomeFieldWarpType": 3,
  "SomeFieldWarpData": "32자리_UniqueID_GUID"
}
```

- `WarpType: 3` = Reference (기존 객체 참조)
- 기존 모드들은 UniqueID를 알아내어 직접 참조

---

## 구현 방법 추천

### 권장: 런타임 이름 기반 검색

UniqueID GUID를 하드코딩하는 대신, 런타임에 카드 이름으로 검색하는 방식 권장:

**장점:**

- 게임 업데이트로 UniqueID가 변경되어도 동작
- 외부 도구(AssetRipper) 없이 구현 가능
- 디버깅 로그로 실제 카드 정보 확인 가능

**단점:**

- 약간의 시작 시간 오버헤드
- 이름 패턴이 변경되면 수정 필요

**구현 코드 위치:** `src/AircraftExpansion/Data/AircraftModifications.cs`

```csharp
// 카드 이름 패턴
private const string AircraftCardNamePattern = "PlaneCrash";
private const string WhiteWallsNamePattern = "Imp_WhiteWashedWalls";
private const string StitchedHideFloorNamePattern = "Imp_StitchedHideFloor";

// AllUniqueObjects에서 이름으로 검색
var aircraftCard = allCards.FirstOrDefault(card =>
    card.CardType == CardTypes.Explorable &&
    card.name.Contains(AircraftCardNamePattern));
```

### 대안: GameSourceModify JSON 방식

ModCore의 GameSourceModify 폴더를 사용하여 JSON으로 기존 카드 수정:

```text
src/AircraftExpansion/GameSourceModify/
└── PlaneCrash.json
```

```json
{
  "EnvironmentImprovementsWarpType": 4,
  "EnvironmentImprovementsWarpData": [
    "Imp_WhiteWashedWalls의_UniqueID",
    "Imp_StitchedHideFloor의_UniqueID"
  ]
}
```

**주의:** 이 방식은 정확한 UniqueID를 알아야 하므로 런타임 로그에서 먼저 ID를 확인한 후 적용

### 흰 벽 업그레이드의 추가 조건

흰 벽 업그레이드(`Imp_WhiteWashedWalls`)에 공간 확장 Stage 9(벽면 마감) 완료 조건을 추가하려면:

1. 공간 확장 완료를 나타내는 태그 또는 카드 생성
2. 흰 벽의 `BlueprintCardConditions` 또는 `BlueprintTagConditions`에 해당 조건 추가

이는 별도의 Harmony 패치나 조건부 로직으로 구현 가능

---

## 액체 시스템 분석

### LiquidContentCondition 구조체

`BlueprintElement`에서 액체 내용물 조건을 정의할 때 사용:

```csharp
public struct LiquidContentCondition
{
    public bool IsActive;                    // 조건 활성화 여부
    public CardData RequiredLiquid;          // 특정 액체 카드 (단일)
    public CardTabGroup RequiredGroup;       // 허용되는 액체 그룹
    public Vector2 RequiredQuantity;         // x=최소량, y=최대량
}
```

#### RequiredQuantity 해석

- `x <= y`: x~y 범위 내의 양 필요
- `x > y`: x 이상의 양 필요 (최소값만 검사)
- `(0, 0)`: 양 무관 (AnyQuantity)

### Stage 9 물 조건 구현

CLAUDE.md 51행에 정의된 조건:

```text
물(0.8그릇 이상 담김, 코코넛 그릇/점토 그릇/플라스틱 병에 담긴 (깨끗한) 물/오염된 물만 허용)
```

#### 필요한 카드/그룹

**용기 (CardTabGroup 필요):**

| 카드명 | 로컬라이제이션 키 | 한글명 |
| ------ | ----------------- | ------ |
| CoconutShell | `CoconutShell` | 코코넛 껍데기 |
| ClayBowl | `ClayBowl` | 점토 그릇 |
| PlasticBottle | `PlasticBottle` | 플라스틱 병 |

**액체 (CardTabGroup 필요):**

| 카드명 | 로컬라이제이션 키 | 한글명 |
| ------ | ----------------- | ------ |
| LQ_Water | `LQ_Water` | 물 |
| LQ_WaterUnsafe | `LQ_WaterUnsafe` | 안전하지 않은 물 |

#### BlueprintElement 구성 예시

```csharp
new BlueprintElement {
    RequiredTabGroup = waterContainerGroup,  // CoconutShell, ClayBowl, PlasticBottle
    RequiredQuantity = 1,
    RequiredLiquidContent = new LiquidContentCondition {
        IsActive = true,
        RequiredGroup = drinkableWaterGroup,  // LQ_Water, LQ_WaterUnsafe
        RequiredQuantity = new Vector2(0.8f, 0f)  // 0.8 이상
    }
}
```

### 용기 카드 (Containers)

Stage 9에서 물을 담을 수 있는 용기:

| 카드명 | UniqueID | 한글명 | 주요 태그 |
| ------ | -------- | ------ | --------- |
| CoconutShell | `80b480a28a4fbd544b78b5807fd999e4` | 코코넛 껍데기 | tag_WaterContainer, tag_ContainerOpen, tag_ContainerBowl |
| ClayBowl | `a968f3eaffc6b9743b82982b5af2ab8c` | 점토 그릇 | tag_WaterContainer, tag_ContainerOpen, tag_ContainerBowl |
| PlasticBottle | `66f68506e7f59d74e9502cbdd3eeec40` | 플라스틱 병 | tag_WaterContainer, tag_ContainerSealed |

**참고:** `PlasticBottleFull` (ID: `2e48255bf8840f3488c01cd7ca80ba4d`)은 게임 시작 시 물이 담긴 플라스틱 병.

### 액체 카드 (Liquids)

| 카드명 | UniqueID | 한글명 | Stage 9 허용 | 주요 태그 |
| ------ | -------- | ------ | ------------ | --------- |
| LQ_Water | `425259cb06b869d45be2e7f1b5b54aff` | 물 | ✓ | tag_WaterFresh, tag_WaterAny |
| LQ_WaterUnsafe | `aa33d3104cc682c4e9a163e7df9a7e13` | 안전하지 않은 물 | ✓ | tag_WaterFresh, tag_WaterAny |
| LQ_WaterBoiling | `782eb58b5bfd5d94dad5798de67584ed` | 끓는 물 | ✗ | - |
| LQ_WaterSalt | `9a74ca45c5a14744da6fbaf2fdb92722` | 소금물 | ✗ | tag_WaterAny |
| LQ_WaterToxic | `ede95df8e3a2b38408760d6bf7021d72` | 유독성 물 | ✗ | tag_WaterFresh, tag_WaterAny |
| LQ_WaterRice | `b941606d1bfac2d4286ce5f8f70525dc` | 쌀뜨물 | ✗ | tag_WaterFresh, tag_WaterAny |

### 용기 태그 시스템

| 태그 | 설명 |
| ---- | ---- |
| tag_WaterContainer | 물을 담을 수 있는 용기 |
| tag_ContainerOpen | 열린 용기 (빗물 수집 가능, 넘침 가능) |
| tag_ContainerSealed | 밀봉 용기 (빗물 수집 불가, 넘침 없음) |
| tag_ContainerBowl | 그릇형 용기 |
| tag_Spillable | 넘침 가능 |

---

## 확장 단계별 필요 아이템 UUID

### 도끼 (Stage 1용)

| 아이템 | 카드명 | UniqueID | 태그 |
| ------ | ------ | -------- | ---- |
| 돌 도끼 | AxeStone | `bd517943b89f4e149893ffe4489a96e6` | tag_Axe |
| 부싯돌 도끼 | AxeFlint | `5c22db296fcde9a4da7dab850195c04a` | tag_Axe, tag_AxeAdv |
| 구리 도끼 | AxeCopper | `968d89b358ccf624a9e34f659166a0a1` | tag_Axe, tag_AxeAdv |
| 고철 도끼 | AxeScrap | `9fc3486d28029834fabc24cae6420809` | tag_Axe, tag_AxeAdv |
| 서바이벌 도끼 | AxeSurvival | `8cf3d3dc5b48c6244a23f825d3528dce` | tag_Axe, tag_AxeAdv |

**Stage 1 부산물:**

| 아이템 | 카드명 | UniqueID |
| ------ | ------ | -------- |
| 고철 조각 | MetalScrap | `035841b923343024eafcbeece1775c4a` |

### 삽 (Stage 2-5용)

| 아이템 | 카드명 | UniqueID | 태그 |
| ------ | ------ | -------- | ---- |
| 나무 삽 | ShovelWooden | `2466c0bf349c4f84db55a4ee8ad67300` | tag_Shovel |
| 구리 삽 | ShovelCopper | `3587432ac7e29784991ff2cccf92891d` | tag_Shovel |
| 고철 삽 | ShovelScrap | `ed61904e423eab447a3bb42d0b3eea00` | tag_Shovel |

**Stage 2-5 부산물:**

| 아이템 | 카드명 | UniqueID |
| ------ | ------ | -------- |
| 마른 흙 | DirtPile | `6d47888db6018d04ea562476cde60440` |

### 구조재 (Stage 6-7용)

| 아이템 | 카드명 | UniqueID |
| ------ | ------ | -------- |
| 긴 막대기 | StickLong | `3db4c94184af274409f0d3eb16870f64` |
| 섬유 끈 | CordFiber | `f6e8281ffa0e6eb4b8bf1dc5e5ae1338` |

### 밀봉재 (Stage 8용)

| 아이템 | 카드명 | UniqueID |
| ------ | ------ | -------- |
| 점토 | Clay | `68c14d265ea6c874ba79444d2e1ef7b3` |
| 섬유 | Fibers | `b8885e78517570b4293c573871625e11` |

### 마감재 (Stage 9용)

| 아이템 | 카드명 | UniqueID |
| ------ | ------ | -------- |
| 생석회 | Quicklime | `f21870cdd29e5a048b6cc7a2d08d5e71` |
| 물 담긴 용기 | (위 용기 + 액체 참조) | - |
