# Guild Map Materials (Unity 6 URP)

## 포함 파일
- 나무 바닥: Albedo / Normal / AO / Height / MetallicSmoothness
- 석제 벽: Albedo / Normal / AO / Height / MetallicSmoothness
- Editor/GuildMapMaterialGenerator.cs
- 생성될 머테리얼 위치: Assets/ThirdParty/GuildMapMaterials/Materials

## 설치
1. ZIP 압축을 풉니다.
2. 내부의 Assets 폴더를 Unity 프로젝트 루트에 덮어씁니다.
3. Unity가 임포트를 마칠 때까지 기다립니다.
4. 상단 메뉴에서 Tools > Guild Map Materials > Generate Materials 를 실행합니다.
5. 아래 머테리얼이 생성됩니다.
   - MAT_Guild_WoodFloor.mat
   - MAT_Guild_StoneWall.mat

## 기준
- Unity 6
- Universal Render Pipeline(URP)
- 1024 x 1024
- 반복 타일링용
- 저사양 프로젝트를 고려한 압축 설정

## 참고
Built-in Render Pipeline 또는 HDRP에서는 셰이더 이름과 속성이 달라 별도 수정이 필요합니다.
