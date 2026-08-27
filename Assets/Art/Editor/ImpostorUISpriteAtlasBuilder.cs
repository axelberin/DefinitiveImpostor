using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using Object = UnityEngine.Object;

[InitializeOnLoad]
internal static class ImpostorUISpriteAtlasBuilder
{
    private const string ScriptFileName = "ImpostorUISpriteAtlasBuilder.cs";
    private const string AtlasFileName = "ImpostorUI.spriteatlasv2";
    private const string AnimatedBackgroundFileName = "StarsBackground.png";

    static ImpostorUISpriteAtlasBuilder()
    {
        EditorApplication.delayCall += EnsureAtlasExists;
    }

    [MenuItem("Tools/Impostor/Rebuild UI Sprite Atlas")]
    private static void RebuildAtlas()
    {
        if (!TryResolvePaths(out string artRoot, out string uiFolder, out string atlasPath))
            return;

        if (AssetDatabase.LoadMainAssetAtPath(atlasPath) != null)
            AssetDatabase.DeleteAsset(atlasPath);

        CreateAtlas(artRoot, uiFolder, atlasPath);
    }

    private static void EnsureAtlasExists()
    {
        if (!TryResolvePaths(out string artRoot, out string uiFolder, out string atlasPath))
            return;

        if (AssetDatabase.LoadMainAssetAtPath(atlasPath) == null)
            CreateAtlas(artRoot, uiFolder, atlasPath);
    }

    private static bool TryResolvePaths(
        out string artRoot,
        out string uiFolder,
        out string atlasPath)
    {
        string expectedSuffix = $"/Editor/{ScriptFileName}";
        string scriptPath = AssetDatabase
            .FindAssets($"{Path.GetFileNameWithoutExtension(ScriptFileName)} t:MonoScript")
            .Select(AssetDatabase.GUIDToAssetPath)
            .FirstOrDefault(path => path.EndsWith(expectedSuffix, StringComparison.Ordinal));

        if (string.IsNullOrEmpty(scriptPath))
        {
            artRoot = uiFolder = atlasPath = string.Empty;
            Debug.LogError("No se pudo localizar la carpeta Art para crear el Sprite Atlas de UI.");
            return false;
        }

        artRoot = Path.GetDirectoryName(Path.GetDirectoryName(scriptPath))?.Replace('\\', '/');
        if (string.IsNullOrEmpty(artRoot))
        {
            uiFolder = atlasPath = string.Empty;
            return false;
        }

        uiFolder = $"{artRoot}/UI";
        atlasPath = $"{artRoot}/Atlases/{AtlasFileName}";
        return AssetDatabase.IsValidFolder(uiFolder);
    }

    private static void CreateAtlas(string artRoot, string uiFolder, string atlasPath)
    {
        string atlasFolder = $"{artRoot}/Atlases";
        if (!AssetDatabase.IsValidFolder(atlasFolder))
            AssetDatabase.CreateFolder(artRoot, "Atlases");

        string[] texturePaths = AssetDatabase
            .FindAssets("t:Texture2D", new[] { uiFolder })
            .Select(AssetDatabase.GUIDToAssetPath)
            .Where(path => !path.EndsWith(AnimatedBackgroundFileName, StringComparison.OrdinalIgnoreCase))
            .OrderBy(path => path, StringComparer.Ordinal)
            .ToArray();

        ConfigureSourceTextures(texturePaths);

        Object[] packables = texturePaths
            .Select(path => AssetDatabase.LoadAssetAtPath<Texture2D>(path))
            .Where(texture => texture != null)
            .Cast<Object>()
            .ToArray();

        if (packables.Length == 0)
        {
            Debug.LogError("No se encontraron sprites de UI para empaquetar.");
            return;
        }

        var atlasAsset = new SpriteAtlasAsset();
        atlasAsset.Add(packables);
        SpriteAtlasAsset.Save(atlasAsset, atlasPath);
        AssetDatabase.ImportAsset(atlasPath, ImportAssetOptions.ForceSynchronousImport);

        var importer = AssetImporter.GetAtPath(atlasPath) as SpriteAtlasImporter;
        if (importer == null)
        {
            Debug.LogError($"No se pudo configurar el Sprite Atlas: {atlasPath}");
            return;
        }

        importer.includeInBuild = true;
        importer.packingSettings = new SpriteAtlasPackingSettings
        {
            blockOffset = 1,
            padding = 4,
            enableRotation = false,
            enableTightPacking = false,
            enableAlphaDilation = true
        };

        SpriteAtlasTextureSettings textureSettings = importer.textureSettings;
        textureSettings.readable = false;
        textureSettings.generateMipMaps = false;
        textureSettings.filterMode = FilterMode.Bilinear;
        textureSettings.anisoLevel = 1;
        importer.textureSettings = textureSettings;

        TextureImporterPlatformSettings platformSettings =
            importer.GetPlatformSettings("DefaultTexturePlatform");
        platformSettings.overridden = false;
        platformSettings.maxTextureSize = 4096;
        platformSettings.format = TextureImporterFormat.Automatic;
        platformSettings.textureCompression = TextureImporterCompression.CompressedHQ;
        platformSettings.compressionQuality = 100;
        platformSettings.crunchedCompression = false;
        importer.SetPlatformSettings(platformSettings);

        importer.SaveAndReimport();
        AssetDatabase.SaveAssets();
        Debug.Log($"Sprite Atlas de UI creado correctamente: {atlasPath}");
    }

    private static void ConfigureSourceTextures(string[] texturePaths)
    {
        int updatedTextures = 0;

        foreach (string texturePath in texturePaths)
        {
            var textureImporter = AssetImporter.GetAtPath(texturePath) as TextureImporter;
            if (textureImporter == null)
                continue;

            if (textureImporter.textureCompression == TextureImporterCompression.Uncompressed &&
                !textureImporter.crunchedCompression)
            {
                continue;
            }

            textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
            textureImporter.crunchedCompression = false;
            textureImporter.SaveAndReimport();
            updatedTextures++;
        }

        if (updatedTextures > 0)
        {
            Debug.Log(
                $"Se configuraron {updatedTextures} texturas fuente sin compresión. " +
                "La compresión de alta calidad se aplica únicamente al Sprite Atlas final.");
        }
    }
}