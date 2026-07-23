using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// DungeonStone1024 텍스처의 임포트 설정을 맞추고,
/// 현재 프로젝트의 렌더 파이프라인에 맞는 머테리얼을 생성합니다.
/// </summary>
[InitializeOnLoad]
public static class DungeonMaterialSetup
{
    // 변경점: 특정 Assets 경로를 강제하지 않고
    // 이 스크립트의 실제 위치를 기준으로 루트 폴더를 찾습니다.
    private static string _rootPath;

    private static string RootPath
    {
        get
        {
            if (string.IsNullOrEmpty(_rootPath))
            {
                _rootPath = ResolveRootPath();
            }

            return _rootPath;
        }
    }

    // 변경점: 자동으로 찾은 RootPath 아래에 Materials 폴더를 생성합니다.
    private static string MaterialPath
    {
        get { return RootPath + "/Materials"; }
    }

    static DungeonMaterialSetup()
    {
        // 패키지를 프로젝트에 넣으면 머테리얼 생성을 한 번 시도합니다.
        EditorApplication.delayCall += CreateMaterialsIfMissing;
    }

    [MenuItem("Tools/Dungeon Materials/Rebuild Materials")]
    public static void RebuildMaterials()
    {
        CreateMaterials(overwriteExisting: true);
    }

    private static void CreateMaterialsIfMissing()
    {
        CreateMaterials(overwriteExisting: false);
    }

    private static void CreateMaterials(bool overwriteExisting)
    {
        EnsureMaterialFolder();

        // 현재 활성화된 렌더 파이프라인을 기준으로 Shader를 선택합니다.
        Shader shader = FindCompatibleLitShader(out string pipelineName);

        if (shader == null)
        {
            Debug.LogError(
                "[Dungeon Materials] 호환 가능한 Lit/Standard Shader를 찾지 못했습니다."
            );

            return;
        }

        ConfigureTextureImporters("Wall", "DungeonWall");
        ConfigureTextureImporters("Floor", "DungeonFloor");

        CreateMaterial(
            "Wall",
            "DungeonWall",
            "M_DungeonWall",
            shader,
            pipelineName,
            overwriteExisting
        );

        CreateMaterial(
            "Floor",
            "DungeonFloor",
            "M_DungeonFloor",
            shader,
            pipelineName,
            overwriteExisting
        );

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log(
            $"[Dungeon Materials] {pipelineName}용 벽/바닥 머테리얼 준비 완료: {MaterialPath}"
        );
    }

    private static void CreateMaterial(
        string category,
        string textureStem,
        string materialName,
        Shader shader,
        string pipelineName,
        bool overwriteExisting)
    {
        string materialAssetPath =
            $"{MaterialPath}/{materialName}.mat";

        Material material =
            AssetDatabase.LoadAssetAtPath<Material>(materialAssetPath);

        string textureRoot =
            $"{RootPath}/Textures/{category}/{textureStem}";

        Texture2D baseColor =
            LoadTexture(textureRoot + "_BaseColor.png");

        Texture2D normal =
            LoadTexture(textureRoot + "_Normal.png");

        Texture2D ao =
            LoadTexture(textureRoot + "_AO.png");

        Texture2D mask =
            LoadTexture(textureRoot + "_Mask.png");

        Texture2D height =
            LoadTexture(textureRoot + "_Height.png");

        if (baseColor == null ||
            normal == null ||
            ao == null ||
            mask == null)
        {
            Debug.LogError(
                $"[Dungeon Materials] {category} 텍스처 일부를 불러오지 못했습니다.\n" +
                $"확인한 경로: {textureRoot}"
            );

            return;
        }

        if (material != null &&
            overwriteExisting == false &&
            HasAssignedBaseTexture(material, pipelineName))
        {
            return;
        }

        // 텍스처 로드 성공을 확인한 뒤에만 .mat 파일을 생성합니다.
        if (material == null)
        {
            material = new Material(shader)
            {
                name = materialName
            };

            AssetDatabase.CreateAsset(
                material,
                materialAssetPath
            );
        }
        else
        {
            material.shader = shader;
        }

        // 파이프라인별 Shader Property 이름에 맞춰 연결합니다.
        if (pipelineName == "URP")
        {
            SetTexture(material, "_BaseMap", baseColor);
            SetTexture(material, "_BumpMap", normal);
            SetTexture(material, "_MetallicGlossMap", mask);
            SetTexture(material, "_OcclusionMap", ao);
            SetTexture(material, "_ParallaxMap", height);

            SetFloat(material, "_Metallic", 0f);
            SetFloat(material, "_Smoothness", 0.25f);
            SetFloat(material, "_BumpScale", 1f);
            SetFloat(material, "_OcclusionStrength", 0.8f);
            SetFloat(material, "_Parallax", 0.01f);

            material.EnableKeyword("_NORMALMAP");
            material.EnableKeyword("_METALLICSPECGLOSSMAP");
            material.EnableKeyword("_OCCLUSIONMAP");
        }
        else if (pipelineName == "HDRP")
        {
            SetTexture(material, "_BaseColorMap", baseColor);
            SetTexture(material, "_NormalMap", normal);
            SetTexture(material, "_MaskMap", mask);
            SetTexture(material, "_HeightMap", height);

            SetFloat(material, "_Metallic", 0f);
            SetFloat(material, "_Smoothness", 0.25f);
            SetFloat(material, "_NormalScale", 1f);
            SetFloat(material, "_HeightAmplitude", 0.01f);

            material.EnableKeyword("_NORMALMAP_TANGENT_SPACE");
            material.EnableKeyword("_MASKMAP");
            material.EnableKeyword("_HEIGHTMAP");
        }
        else
        {
            SetTexture(material, "_MainTex", baseColor);
            SetTexture(material, "_BumpMap", normal);
            SetTexture(material, "_MetallicGlossMap", mask);
            SetTexture(material, "_OcclusionMap", ao);
            SetTexture(material, "_ParallaxMap", height);

            SetFloat(material, "_Metallic", 0f);
            SetFloat(material, "_Glossiness", 0.25f);
            SetFloat(material, "_BumpScale", 1f);
            SetFloat(material, "_OcclusionStrength", 0.8f);
            SetFloat(material, "_Parallax", 0.01f);

            material.EnableKeyword("_NORMALMAP");
            material.EnableKeyword("_METALLICGLOSSMAP");
        }

        EditorUtility.SetDirty(material);
    }

    private static bool HasAssignedBaseTexture(
        Material material,
        string pipelineName)
    {
        string propertyName = pipelineName switch
        {
            "URP" => "_BaseMap",
            "HDRP" => "_BaseColorMap",
            _ => "_MainTex"
        };

        return material.HasProperty(propertyName) &&
               material.GetTexture(propertyName) != null;
    }

    private static Shader FindCompatibleLitShader(
        out string pipelineName)
    {
        RenderPipelineAsset currentPipeline =
            GraphicsSettings.currentRenderPipeline;

        if (currentPipeline == null)
        {
            pipelineName = "Built-in";
            return Shader.Find("Standard");
        }

        string pipelineTypeName =
            currentPipeline.GetType().Name;

        if (pipelineTypeName.IndexOf(
                "Universal",
                StringComparison.OrdinalIgnoreCase) >= 0)
        {
            pipelineName = "URP";

            return Shader.Find(
                "Universal Render Pipeline/Lit"
            );
        }

        if (pipelineTypeName.IndexOf(
                "HDRender",
                StringComparison.OrdinalIgnoreCase) >= 0 ||
            pipelineTypeName.IndexOf(
                "HighDefinition",
                StringComparison.OrdinalIgnoreCase) >= 0)
        {
            pipelineName = "HDRP";
            return Shader.Find("HDRP/Lit");
        }

        pipelineName = "Built-in";
        return Shader.Find("Standard");
    }

    /// <summary>
    /// 변경점:
    /// DungeonMaterialSetup.cs가 들어 있는 Editor 폴더의 부모를
    /// 머테리얼 패키지의 루트 경로로 사용합니다.
    ///
    /// 현재 구조라면 다음 경로가 RootPath가 됩니다.
    /// Assets/ThirdParty/DungeonStone1024/Assets/DungeonStone1024
    /// </summary>
    private static string ResolveRootPath()
    {
        string[] scriptGuids =
            AssetDatabase.FindAssets(
                "DungeonMaterialSetup t:MonoScript"
            );

        foreach (string scriptGuid in scriptGuids)
        {
            string scriptPath =
                AssetDatabase.GUIDToAssetPath(scriptGuid);

            if (scriptPath.EndsWith(
                    "/Editor/DungeonMaterialSetup.cs",
                    StringComparison.OrdinalIgnoreCase) == false)
            {
                continue;
            }

            string editorFolderPath =
                Path.GetDirectoryName(scriptPath);

            if (string.IsNullOrEmpty(editorFolderPath))
            {
                continue;
            }

            string resolvedRootPath =
                Path.GetDirectoryName(editorFolderPath);

            if (string.IsNullOrEmpty(resolvedRootPath) == false)
            {
                return resolvedRootPath.Replace('\\', '/');
            }
        }

        // 자동 탐색 실패 시 현재 팀 프로젝트 규칙의 경로를 사용합니다.
        return
            "Assets/ThirdParty/DungeonStone1024/Assets/DungeonStone1024";
    }

    private static void ConfigureTextureImporters(
        string category,
        string textureStem)
    {
        string textureRoot =
            $"{RootPath}/Textures/{category}/{textureStem}";

        ConfigureTexture(
            textureRoot + "_BaseColor.png",
            isNormalMap: false,
            useSrgb: true
        );

        ConfigureTexture(
            textureRoot + "_Normal.png",
            isNormalMap: true,
            useSrgb: false
        );

        ConfigureTexture(
            textureRoot + "_AO.png",
            isNormalMap: false,
            useSrgb: false
        );

        ConfigureTexture(
            textureRoot + "_Roughness.png",
            isNormalMap: false,
            useSrgb: false
        );

        ConfigureTexture(
            textureRoot + "_Height.png",
            isNormalMap: false,
            useSrgb: false
        );

        ConfigureTexture(
            textureRoot + "_Mask.png",
            isNormalMap: false,
            useSrgb: false
        );
    }

    private static void ConfigureTexture(
        string assetPath,
        bool isNormalMap,
        bool useSrgb)
    {
        TextureImporter importer =
            AssetImporter.GetAtPath(assetPath) as TextureImporter;

        if (importer == null)
        {
            Debug.LogWarning(
                $"[Dungeon Materials] TextureImporter를 찾지 못했습니다: {assetPath}"
            );

            return;
        }

        bool changed = false;

        if (importer.wrapMode != TextureWrapMode.Repeat)
        {
            importer.wrapMode = TextureWrapMode.Repeat;
            changed = true;
        }

        if (importer.filterMode != FilterMode.Bilinear)
        {
            importer.filterMode = FilterMode.Bilinear;
            changed = true;
        }

        if (importer.maxTextureSize != 1024)
        {
            importer.maxTextureSize = 1024;
            changed = true;
        }

        if (importer.mipmapEnabled == false)
        {
            importer.mipmapEnabled = true;
            changed = true;
        }

        if (importer.sRGBTexture != useSrgb)
        {
            importer.sRGBTexture = useSrgb;
            changed = true;
        }

        if (importer.textureCompression !=
            TextureImporterCompression.Compressed)
        {
            importer.textureCompression =
                TextureImporterCompression.Compressed;

            changed = true;
        }

        if (importer.anisoLevel != 2)
        {
            importer.anisoLevel = 2;
            changed = true;
        }

        TextureImporterType targetType =
            isNormalMap
                ? TextureImporterType.NormalMap
                : TextureImporterType.Default;

        if (importer.textureType != targetType)
        {
            importer.textureType = targetType;
            changed = true;
        }

        if (changed)
        {
            importer.SaveAndReimport();
        }
    }

    private static Texture2D LoadTexture(string assetPath)
    {
        return AssetDatabase.LoadAssetAtPath<Texture2D>(
            assetPath
        );
    }

    private static void SetTexture(
        Material material,
        string propertyName,
        Texture texture)
    {
        if (material.HasProperty(propertyName))
        {
            material.SetTexture(propertyName, texture);
            material.SetTextureScale(
                propertyName,
                Vector2.one
            );

            material.SetTextureOffset(
                propertyName,
                Vector2.zero
            );
        }
    }

    private static void SetFloat(
        Material material,
        string propertyName,
        float value)
    {
        if (material.HasProperty(propertyName))
        {
            material.SetFloat(propertyName, value);
        }
    }

    private static void EnsureMaterialFolder()
    {
        if (AssetDatabase.IsValidFolder(MaterialPath))
        {
            return;
        }

        AssetDatabase.CreateFolder(
            RootPath,
            "Materials"
        );
    }
}