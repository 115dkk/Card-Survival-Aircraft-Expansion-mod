# Card Survival: Tropical Island - Aircraft Expansion Mod

Card Survival: Tropical Island 게임의 관광객 캐릭터용 비행기 거처 확장 모드입니다.

## 프로젝트 목적

1. **기존 업그레이드 활성화**: 게임에서 비행기에 적용이 막혀있던 흰 벽(White Washed Walls)과 가죽 바닥(Stitched-Hide Floor) 업그레이드를 사용 가능하게 합니다.

2. **공간 확장 시스템** (계획): 비행기 내벽을 해체하고 절벽을 굴착하여 거주 공간을 넓히는 새로운 업그레이드 단계를 추가합니다.

## 구현 현황

### 완료

- [x] BepInEx 플러그인 기본 구조
- [x] ModCore Loader 이벤트 연동
- [x] 비행기에 흰 벽 업그레이드 추가
- [x] 비행기에 가죽 바닥 업그레이드 추가
- [x] 카드 덤프 디버깅 도구 (CardDumper)

### 예정

- [ ] 비행기 공간 확장 Stage 1: 내벽 해체
- [ ] 비행기 공간 확장 Stage 2-5: 초기 굴착
- [ ] 비행기 공간 확장 Stage 6-7: 구조 보강
- [ ] 비행기 공간 확장 Stage 8: 접합부 밀봉
- [ ] 비행기 공간 확장 Stage 9: 벽면 마감
- [ ] 흰 벽 업그레이드를 공간 확장 완료 조건에 연동

## 요구 사항

- Card Survival: Tropical Island (Steam)
- BepInEx 5.x
- ModCore (Pikachu-模组核心-ModCore)

## 프로젝트 구조

```text
AircraftExpansion/
├── reference/                    # 디컴파일된 참조 소스 (읽기 전용)
│   ├── Assembly-CSharp/          # 게임 본체
│   └── ModCore/                  # 모딩 API
├── src/
│   ├── AircraftExpansion/        # 메인 플러그인
│   │   ├── Plugin.cs             # 진입점
│   │   ├── Data/                 # 데이터 수정 로직
│   │   └── Patches/              # Harmony 패치
│   └── CardDumper/               # 디버깅용 덤프 도구
├── CLAUDE.md                     # 개발 참조 문서
└── README.md
```

## DLL 참조 목록

빌드에 필요한 DLL 파일들입니다. 게임 설치 경로에서 참조합니다.

### BepInEx (BepInEx/core/)

| DLL | 용도 |
|-----|------|
| BepInEx.dll | BepInEx 플러그인 프레임워크 |
| 0Harmony.dll | Harmony 패칭 라이브러리 |

### ModCore (BepInEx/plugins/Pikachu-模组核心-ModCore/)

| DLL | 용도 |
|-----|------|
| ModCore.dll | 모딩 API (데이터 로딩, WarpData 등) |

### Unity/Game (Card Survival - Tropical Island_Data/Managed/)

| DLL | 용도 |
|-----|------|
| UnityEngine.dll | Unity 엔진 기본 |
| UnityEngine.CoreModule.dll | Unity 코어 모듈 |
| Assembly-CSharp.dll | 게임 본체 코드 |

## reference/ 폴더 채우기 (ILSpy)

게임 코드를 분석하려면 DLL을 디컴파일하여 reference/ 폴더에 저장합니다.

### 1. ILSpy 설치

- [ILSpy GitHub Releases](https://github.com/icsharpcode/ILSpy/releases)에서 다운로드
- 또는 Visual Studio 확장으로 설치

### 2. Assembly-CSharp 디컴파일

```bash
# ILSpy 명령줄 도구 사용
ilspycmd "C:\Program Files (x86)\Steam\steamapps\common\Card Survival Tropical Island\Card Survival - Tropical Island_Data\Managed\Assembly-CSharp.dll" -p -o reference/Assembly-CSharp
```

또는 GUI에서:
1. ILSpy 실행
2. File → Open → `Assembly-CSharp.dll` 선택
3. 트리에서 Assembly-CSharp 우클릭 → Save Code
4. `reference/Assembly-CSharp/` 폴더에 저장

### 3. ModCore 디컴파일

```bash
ilspycmd "C:\Program Files (x86)\Steam\steamapps\common\Card Survival Tropical Island\BepInEx\plugins\Pikachu-模组核心-ModCore\ModCore.dll" -p -o reference/ModCore
```

### 4. 프로젝트 파일 생성 (선택)

디컴파일된 폴더에서 빌드하지 않을 것이므로, 간단한 .csproj만 만들어 IDE 지원을 받습니다:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net48</TargetFramework>
    <OutputType>Library</OutputType>
  </PropertyGroup>
</Project>
```

## 빌드

### 1. 게임 경로 확인

`src/AircraftExpansion/AircraftExpansion.csproj`의 `<GamePath>` 값이 게임 설치 경로와 일치하는지 확인합니다:

```xml
<GamePath>C:\Program Files (x86)\Steam\steamapps\common\Card Survival Tropical Island</GamePath>
```

### 2. 빌드 실행

```bash
cd src/AircraftExpansion
dotnet build -c Release
```

### 3. 자동 배포

빌드 성공 시 DLL이 자동으로 플러그인 폴더에 복사됩니다:
- `BepInEx/plugins/AircraftExpansion/AircraftExpansion.dll`

## 테스트

### 1. 게임 실행

Steam에서 게임을 실행합니다.

### 2. 로그 확인

`BepInEx/LogOutput.log`에서 플러그인 로드 확인:

```text
[Info   :AircraftExpansion] Plugin Aircraft Expansion v1.0.0 is loaded!
[Info   :AircraftExpansion] Found aircraft card: PlaneCrash (UniqueID: d20527131ef12624da48a0fd0e52cb11)
[Info   :AircraftExpansion] Added improvement 'Imp_WhiteWashedWalls' to 'PlaneCrash'
[Info   :AircraftExpansion] Added improvement 'Imp_StitchedHideFloor' to 'PlaneCrash'
```

### 3. 게임 내 확인

1. 관광객 캐릭터로 새 게임 시작 (또는 기존 저장 로드)
2. 비행기 내부로 이동
3. 업그레이드 메뉴에서 "백색 벽"과 "가죽 장식 바닥" 옵션 확인

### 4. 카드 덤프 (디버깅)

CardDumper 플러그인이 설치되어 있으면 게임 로드 후 `BepInEx/dumps/card_dump.txt`에 카드 정보가 저장됩니다.

## 라이선스

MIT License - 자세한 내용은 [LICENSE.md](LICENSE.md)를 참조하세요.

## 참고 자료

- [BepInEx Documentation](https://docs.bepinex.dev/)
- [Harmony Wiki](https://harmony.pardeike.net/)
- [ModCore GitHub](https://github.com/pika-pikachu/ModCore) (비공식)
