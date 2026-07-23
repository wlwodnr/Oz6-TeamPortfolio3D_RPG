using UnityEditor;
using UnityEngine;
using System.IO;

public static class GuildMapMaterialGenerator
{
    // 변경 지점:
    // 사용자가 지정한 ThirdParty 경로 아래에서 텍스처를 읽고,
    // URP/Lit 머테리얼 2개를 자동 생성합니다.
    private const string RootPath = "Assets/ThirdParty/GuildMapMaterials";
    private const string TexturePath = RootPath + "/Textures";
    private const string MaterialPath = RootPath + "/Materials";

    [MenuItem("Tools/Guild Map Materials/Generate Materials")]
    public static void GenerateMaterials()
    {
        EnsureDirectory(MaterialPath);

        Shader shader = Shader.Find("Universal Render Pipeline/Lit");

        if (shader == null)
        {
            Debug.LogError(
                "URP/Lit 셰이더를 찾을 수 없습니다. " +
                "현재 프로젝트가 URP인지 확인한 뒤 다시 실행해 주세요.");
            return;
        }

        ConfigureTextureImporters();

        CreateMaterial(
            "MAT_Guild_WoodFloor",
            shader,
            TexturePath + "/Guild_WoodFloor_Albedo.png",
            TexturePath + "/Guild_WoodFloor_Normal.png",
            TexturePath + "/Guild_WoodFloor_AO.png",
            TexturePath + "/Guild_WoodFloor_Height.png",
            TexturePath + "/Guild_WoodFloor_MetallicSmoothness.png",
            new Vector2(2.0f, 2.0f),
            0.10f);

        CreateMaterial(
            "MAT_Guild_StoneWall",
            shader,
            TexturePath + "/Guild_StoneWall_Albedo.png",
            TexturePath + "/Guild_StoneWall_Normal.png",
            TexturePath + "/Guild_StoneWall_AO.png",
            TexturePath + "/Guild_StoneWall_Height.png",
            TexturePath + "/Guild_StoneWall_MetallicSmoothness.png",
            new Vector2(2.0f, 2.0f),
            0.06f);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log(
            "길드 맵 머테리얼 생성 완료: " +
            MaterialPath + "/MAT_Guild_WoodFloor.mat, " +
            MaterialPath + "/MAT_Guild_StoneWall.mat");
    }

    private static void CreateMaterial(
        string materialName,
        Shader shader,
        string albedoPath,
        string normalPath,
        string aoPath,
        string heightPath,
        string metallicSmoothnessPath,
        Vector2 tiling,
        float parallax)
    {
        string materialAssetPath = MaterialPath + "/" + materialName + ".mat";

        Material material = AssetDatabase.LoadAssetAtPath<Material>(materialAssetPath);

        if (material == null)
        {
            material = new Material(shader);
            material.name = materialName;
            AssetDatabase.CreateAsset(material, materialAssetPath);
        }
        else
        {
            material.shader = shader;
        }

        Texture2D albedo = AssetDatabase.LoadAssetAtPath<Texture2D>(albedoPath);
        Texture2D normal = AssetDatabase.LoadAssetAtPath<Texture2D>(normalPath);
        Texture2D ao = AssetDatabase.LoadAssetAtPath<Texture2D>(aoPath);
        Texture2D height = AssetDatabase.LoadAssetAtPath<Texture2D>(heightPath);
        Texture2D metallicSmoothness =
            AssetDatabase.LoadAssetAtPath<Texture2D>(metallicSmoothnessPath);

        material.SetTexture("_BaseMap", albedo);
        material.SetTextureScale("_BaseMap", tiling);

        material.SetTexture("_BumpMap", normal);
        material.SetFloat("_BumpScale", 1.0f);
        material.EnableKeyword("_NORMALMAP");

        material.SetTexture("_OcclusionMap", ao);
        material.SetFloat("_OcclusionStrength", 1.0f);
        material.EnableKeyword("_OCCLUSIONMAP");

        material.SetTexture("_ParallaxMap", height);
        material.SetFloat("_Parallax", parallax);
        material.EnableKeyword("_PARALLAXMAP");

        material.SetTexture("_MetallicGlossMap", metallicSmoothness);
        material.SetFloat("_Metallic", 0.0f);
        material.SetFloat("_Smoothness", 1.0f);
        material.EnableKeyword("_METALLICSPECGLOSSMAP");

        material.SetColor("_BaseColor", Color.white);
        material.SetFloat("_Surface", 0.0f);
        material.SetFloat("_AlphaClip", 0.0f);

        EditorUtility.SetDirty(material);
    }

    private static void ConfigureTextureImporters()
    {
        ConfigureTexture(TexturePath + "/Guild_WoodFloor_Albedo.png", false, true);
        ConfigureTexture(TexturePath + "/Guild_WoodFloor_Normal.png", true, false);
        ConfigureTexture(TexturePath + "/Guild_WoodFloor_AO.png", false, false);
        ConfigureTexture(TexturePath + "/Guild_WoodFloor_Height.png", false, false);
        ConfigureTexture(TexturePath + "/Guild_WoodFloor_MetallicSmoothness.png", false, false);

        ConfigureTexture(TexturePath + "/Guild_StoneWall_Albedo.png", false, true);
        ConfigureTexture(TexturePath + "/Guild_StoneWall_Normal.png", true, false);
        ConfigureTexture(TexturePath + "/Guild_StoneWall_AO.png", false, false);
        ConfigureTexture(TexturePath + "/Guild_StoneWall_Height.png", false, false);
        ConfigureTexture(TexturePath + "/Guild_StoneWall_MetallicSmoothness.png", false, false);

        AssetDatabase.Refresh();
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
            Debug.LogWarning("텍스처를 찾지 못했습니다: " + assetPath);
            return;
        }

        importer.textureType = isNormalMap
            ? TextureImporterType.NormalMap
            : TextureImporterType.Default;

        importer.sRGBTexture = useSrgb;
        importer.wrapMode = TextureWrapMode.Repeat;
        importer.filterMode = FilterMode.Bilinear;
        importer.mipmapEnabled = true;
        importer.maxTextureSize = 1024;
        importer.textureCompression = TextureImporterCompression.Compressed;

        importer.SaveAndReimport();
    }

    private static void EnsureDirectory(string assetDirectory)
    {
        if (AssetDatabase.IsValidFolder(assetDirectory))
        {
            return;
        }

        string[] parts = assetDirectory.Split('/');
        string current = parts[0];

        for (int i = 1; i < parts.Length; i++)
        {
            string next = current + "/" + parts[i];

            if (!AssetDatabase.IsValidFolder(next))
            {
                AssetDatabase.CreateFolder(current, parts[i]);
            }

            current = next;
        }
    }
}
