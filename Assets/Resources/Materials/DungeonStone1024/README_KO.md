# Dungeon Stone 1024

Unity 6용 어두운 중세 석조 던전 벽·바닥 머테리얼 세트입니다.

## 포함 파일

- 벽: Base Color, Normal, AO, Roughness, Height, Mask
- 바닥: Base Color, Normal, AO, Roughness, Height, Mask
- 해상도: 모든 텍스처 1024×1024
- 반복 방식: 상하좌우 무봉제 타일링
- 최적화: 최대 크기 1024, 압축 사용, Aniso Level 2

`Mask` 채널 구성:

- R: Metallic (검정, 비금속)
- G: Ambient Occlusion
- B: Detail Mask (흰색)
- A: Smoothness

## 적용 방법

1. 압축 파일을 풉니다.
2. 내부의 `Assets` 폴더를 Unity 프로젝트 최상위 폴더에 복사합니다.
3. Unity가 스크립트와 텍스처를 임포트할 때까지 기다립니다.
4. `Assets/DungeonStone1024/Materials`에 아래 머테리얼이 자동 생성됩니다.
   - `M_DungeonWall.mat`
   - `M_DungeonFloor.mat`
5. 벽 또는 바닥 오브젝트의 Mesh Renderer에 머테리얼을 드래그합니다.

자동 생성되지 않으면 Unity 상단 메뉴에서 다음을 실행합니다.

`Tools > Dungeon Materials > Rebuild Materials`

## 렌더 파이프라인

`DungeonMaterialSetup.cs`가 현재 프로젝트 설정을 확인해 다음 Shader 중 하나를 자동 선택합니다.

- Built-in: `Standard`
- URP: `Universal Render Pipeline/Lit`
- HDRP: `HDRP/Lit`

Unity 버전 번호만으로는 렌더 파이프라인을 알 수 없지만, 이 패키지는 별도 선택 없이 현재 프로젝트에 맞춥니다.

## 권장 조절값

- 돌 크기가 너무 크면 Material의 Tiling을 `(2, 2)` 또는 `(3, 3)`으로 올립니다.
- 표면이 너무 울퉁불퉁하면 Normal Scale을 `0.5~0.8`로 낮춥니다.
- 저사양 환경에서 더 가볍게 하려면 Height Map 연결을 해제합니다.
- 바닥이 멀리서 흐릿하면 Aniso Level을 `4`로 올립니다.

## 파일 위치

- 텍스처: `Assets/DungeonStone1024/Textures`
- 자동 설정 코드: `Assets/DungeonStone1024/Editor/DungeonMaterialSetup.cs`
- 생성 머테리얼: `Assets/DungeonStone1024/Materials`
