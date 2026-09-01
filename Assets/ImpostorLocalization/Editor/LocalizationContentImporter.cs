using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.Localization;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Metadata;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

public static class LocalizationContentImporter
{
    private const string SourceFolder = "Assets/ImpostorLocalization/Data/Source";
    private const string GeneratedFolder = "Assets/ImpostorLocalization/Data/Generated";
    private const string LocaleFolder = GeneratedFolder + "/Locales";
    private const string TableFolder = GeneratedFolder + "/String Tables";
    private const string SettingsPath = GeneratedFolder + "/Localization Settings.asset";
    private const string DatabasePath = GeneratedFolder + "/WordsDataBase.asset";

    private static readonly Regex PlaceholderRegex = new(@"\{([A-Za-z_][A-Za-z0-9_]*)\}", RegexOptions.Compiled);

    private static readonly Dictionary<string, string> LegacyCategoryIds = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Famosos argentinos"] = "argentine_celebrities",
        ["Famosos globales"] = "global_celebrities",
        ["Países"] = "countries",
        ["Comidas"] = "foods",
        ["Objetos"] = "objects",
        ["Videojuegos"] = "video_games",
        ["Animales"] = "animals",
        ["Películas"] = "movies",
        ["Marcas"] = "brands",
        ["Profesiones"] = "professions",
        ["Superhéroes"] = "superheroes",
        ["Hot"] = "hot",
        ["Funable"] = "dark_humor",
        ["Jugadores"] = "current_players",
        ["Jugadores de esta partida"] = "current_players"
    };

    [MenuItem("Tools/Impostor/1. Importar CSV y generar localización")]
    public static void Import()
    {
        try
        {
            ImportResult result = ParseAndValidateSources();

            EnsureAssetFolder(GeneratedFolder);
            EnsureAssetFolder(LocaleFolder);
            EnsureAssetFolder(TableFolder);

            LocalizationSettings settings = EnsureLocalizationSettings();
            Locale spanish = EnsureLocale(GameLocalization.SpanishArgentina, "Español (Argentina)");
            Locale english = EnsureLocale(GameLocalization.English, "English");
            ConfigureFallbackAndProjectLocale(settings, spanish, english);

            WriteStringTable(GameLocalization.ContentTable, result.ContentEntries, spanish, english);
            WriteStringTable(GameLocalization.UiTable, result.UiEntries, spanish, english);
            WriteWordDatabase(result.Categories);
            ConfigureAddressablesBuild();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"Localization import completed: {result.Categories.Count} categories, "
                + $"{result.WordCount} words, {result.HintCount} hints, "
                + $"{result.UiEntries.Count} UI entries.");

            EditorUtility.DisplayDialog(
                "Impostor Localization",
                "Importación completada sin errores.\n\n"
                + $"Categorías: {result.Categories.Count}\n"
                + $"Palabras: {result.WordCount}\n"
                + $"Pistas: {result.HintCount}\n"
                + $"Textos de UI: {result.UiEntries.Count}",
                "Aceptar");
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
            EditorUtility.DisplayDialog(
                "Error de importación",
                "La importación se detuvo. Revisá el error antes de volver a ejecutarla.\n\n" + exception.Message,
                "Aceptar");
        }
    }

    private static ImportResult ParseAndValidateSources()
    {
        CsvFile categoriesCsv = CsvFile.Read($"{SourceFolder}/Categories.csv");
        CsvFile wordsCsv = CsvFile.Read($"{SourceFolder}/Words.csv");
        CsvFile hintsCsv = CsvFile.Read($"{SourceFolder}/Hints.csv");
        CsvFile uiCsv = CsvFile.Read($"{SourceFolder}/UI.csv");

        categoriesCsv.RequireColumns("CategoryId", "Type", "NameKey", "SubtitleKey",
            "Name_es-AR", "Subtitle_es-AR", "Name_en", "Subtitle_en");
        wordsCsv.RequireColumns("WordId", "CategoryId", "WordKey", "DescriptionKey",
            "Word_es-AR", "Description_es-AR", "Word_en", "Description_en");
        hintsCsv.RequireColumns("HintId", "WordId", "HintKey", "Hint_es-AR", "Hint_en");
        uiCsv.RequireColumns("Key", "Context", "IsSmartString", "es-AR", "en");

        ImportResult result = new();
        Dictionary<string, WordCategory> categoriesById = new(StringComparer.Ordinal);
        Dictionary<string, WordData> wordsById = new(StringComparer.Ordinal);
        HashSet<string> hintIds = new(StringComparer.Ordinal);
        HashSet<string> contentKeys = new(StringComparer.Ordinal);
        HashSet<string> uiKeys = new(StringComparer.Ordinal);

        foreach (CsvRow row in categoriesCsv.Rows)
        {
            string categoryId = row.Require("CategoryId");
            if (categoriesById.ContainsKey(categoryId))
                throw Duplicate(row, "CategoryId", categoryId);

            if (!Enum.TryParse(row.Require("Type"), true, out CategoryType categoryType))
                throw new InvalidDataException($"{row.File}:{row.Line} has an invalid category Type.");

            WordCategory category = new()
            {
                CategoryId = categoryId,
                CategoryType = categoryType,
                NameKey = row.Require("NameKey"),
                SubtitleKey = row.Require("SubtitleKey"),
                Words = new List<WordData>()
            };

            categoriesById.Add(categoryId, category);
            result.Categories.Add(category);

            AddEntry(result.ContentEntries, contentKeys, row, category.NameKey,
                row.Require("Name_es-AR"), row.Require("Name_en"), false, $"Category name: {categoryId}");
            AddEntry(result.ContentEntries, contentKeys, row, category.SubtitleKey,
                row.Require("Subtitle_es-AR"), row.Require("Subtitle_en"), false, $"Category subtitle: {categoryId}");
        }

        foreach (CsvRow row in wordsCsv.Rows)
        {
            string wordId = row.Require("WordId");
            string categoryId = row.Require("CategoryId");
            if (wordsById.ContainsKey(wordId))
                throw Duplicate(row, "WordId", wordId);
            if (!categoriesById.TryGetValue(categoryId, out WordCategory category))
                throw new InvalidDataException($"{row.File}:{row.Line} references unknown CategoryId '{categoryId}'.");
            if (category.CategoryType == CategoryType.PlayerNames)
                throw new InvalidDataException($"{row.File}:{row.Line} cannot add fixed words to PlayerNames category '{categoryId}'.");

            WordData word = new()
            {
                WordId = wordId,
                WordKey = row.Require("WordKey"),
                DescriptionKey = row.Require("DescriptionKey"),
                Hints = new List<HintData>()
            };

            category.Words.Add(word);
            wordsById.Add(wordId, word);

            AddEntry(result.ContentEntries, contentKeys, row, word.WordKey,
                row.Require("Word_es-AR"), row.Require("Word_en"), false, $"Word: {wordId}");
            AddEntry(result.ContentEntries, contentKeys, row, word.DescriptionKey,
                row.Require("Description_es-AR"), row.Require("Description_en"), false, $"Description: {wordId}");
        }

        foreach (CsvRow row in hintsCsv.Rows)
        {
            string hintId = row.Require("HintId");
            string wordId = row.Require("WordId");
            if (!hintIds.Add(hintId))
                throw Duplicate(row, "HintId", hintId);
            if (!wordsById.TryGetValue(wordId, out WordData word))
                throw new InvalidDataException($"{row.File}:{row.Line} references unknown WordId '{wordId}'.");

            HintData hint = new()
            {
                HintId = hintId,
                TextKey = row.Require("HintKey")
            };
            word.Hints.Add(hint);

            AddEntry(result.ContentEntries, contentKeys, row, hint.TextKey,
                row.Require("Hint_es-AR"), row.Require("Hint_en"), false, $"Hint: {hintId}");
        }

        foreach (CsvRow row in uiCsv.Rows)
        {
            string key = row.Require("Key");
            if (!bool.TryParse(row.Require("IsSmartString"), out bool isSmart))
                throw new InvalidDataException($"{row.File}:{row.Line} has an invalid IsSmartString value.");

            string spanish = row.Require("es-AR");
            string english = row.Require("en");
            ValidatePlaceholders(row, isSmart, spanish, english);

            List<LocalizedEntry> targetEntries = key.StartsWith("content.", StringComparison.Ordinal)
                ? result.ContentEntries
                : result.UiEntries;
            HashSet<string> targetKeys = key.StartsWith("content.", StringComparison.Ordinal)
                ? contentKeys
                : uiKeys;

            AddEntry(targetEntries, targetKeys, row, key, spanish, english, isSmart, row["Context"]);
        }

        result.WordCount = wordsById.Count;
        result.HintCount = result.Categories.Sum(category => category.Words.Sum(word => word.Hints.Count));

        if (result.Categories.Count == 0 || result.WordCount == 0)
            throw new InvalidDataException("The localization source must contain at least one category and one word.");

        return result;
    }

    private static LocalizationSettings EnsureLocalizationSettings()
    {
        LocalizationSettings settings = LocalizationEditorSettings.ActiveLocalizationSettings;
        if (settings != null)
            return settings;

        settings = AssetDatabase.LoadAssetAtPath<LocalizationSettings>(SettingsPath);
        if (settings == null)
        {
            settings = ScriptableObject.CreateInstance<LocalizationSettings>();
            settings.name = "Impostor Localization Settings";
            AssetDatabase.CreateAsset(settings, SettingsPath);
        }

        LocalizationEditorSettings.ActiveLocalizationSettings = settings;
        EditorUtility.SetDirty(settings);
        return settings;
    }

    private static Locale EnsureLocale(string code, string displayName)
    {
        Locale locale = LocalizationEditorSettings.GetLocales()
            .FirstOrDefault(value => value.Identifier.Code.Equals(code, StringComparison.OrdinalIgnoreCase));

        if (locale == null)
        {
            locale = Locale.CreateLocale(code);
            locale.LocaleName = displayName;
            AssetDatabase.CreateAsset(locale, $"{LocaleFolder}/{code}.asset");
            LocalizationEditorSettings.AddLocale(locale);
        }

        return locale;
    }

    private static void ConfigureFallbackAndProjectLocale(
        LocalizationSettings settings, Locale spanish, Locale english)
    {
        FallbackLocale invalidEnglishFallback = english.Metadata.GetMetadata<FallbackLocale>();
        if (invalidEnglishFallback != null)
            english.Metadata.RemoveMetadata(invalidEnglishFallback);

        FallbackLocale fallback = spanish.Metadata.GetMetadata<FallbackLocale>();
        if (fallback == null)
        {
            fallback = new FallbackLocale(english);
            spanish.Metadata.AddMetadata(fallback);
        }
        else
        {
            fallback.Locale = english;
        }

        LocalizationSettings.ProjectLocale = english;
        LocalizationSettings.StringDatabase.UseFallback = true;

        EditorUtility.SetDirty(spanish);
        EditorUtility.SetDirty(english);
        EditorUtility.SetDirty(settings);
    }

    private static void ConfigureAddressablesBuild()
    {
        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null)
            throw new InvalidOperationException("Addressables Settings could not be found.");

        settings.BuildAddressablesWithPlayerBuild =
            AddressableAssetSettings.PlayerBuildOption.BuildWithPlayer;
        EditorUtility.SetDirty(settings);
    }

    private static void WriteStringTable(
        string collectionName,
        List<LocalizedEntry> entries,
        Locale spanish,
        Locale english)
    {
        StringTableCollection collection = LocalizationEditorSettings.GetStringTableCollection(collectionName);
        if (collection == null)
        {
            collection = LocalizationEditorSettings.CreateStringTableCollection(
                collectionName,
                TableFolder,
                new List<Locale> { spanish, english });
        }

        StringTable spanishTable = GetOrCreateTable(collection, spanish);
        StringTable englishTable = GetOrCreateTable(collection, english);
        collection.ClearAllEntries();

        foreach (LocalizedEntry source in entries)
        {
            StringTableEntry spanishEntry = spanishTable.AddEntry(source.Key, source.Spanish);
            StringTableEntry englishEntry = englishTable.AddEntry(source.Key, source.English);
            spanishEntry.IsSmart = source.IsSmart;
            englishEntry.IsSmart = source.IsSmart;

            if (!string.IsNullOrWhiteSpace(source.Context))
            {
                spanishEntry.AddMetadata(new Comment { CommentText = source.Context });
                englishEntry.AddMetadata(new Comment { CommentText = source.Context });
            }
        }

        LocalizationEditorSettings.SetPreloadTableFlag(spanishTable, true);
        LocalizationEditorSettings.SetPreloadTableFlag(englishTable, true);
        EditorUtility.SetDirty(spanishTable);
        EditorUtility.SetDirty(englishTable);
        EditorUtility.SetDirty(collection.SharedData);
        EditorUtility.SetDirty(collection);
    }

    private static StringTable GetOrCreateTable(StringTableCollection collection, Locale locale)
    {
        return collection.GetTable(locale.Identifier) as StringTable
            ?? collection.AddNewTable(locale.Identifier, TableFolder) as StringTable;
    }

    private static void WriteWordDatabase(List<WordCategory> categories)
    {
        WordDatabase database = FindWordDatabase();
        List<CategoryVisualData> visuals = MigrateVisuals(database, categories);
        database.ReplaceImportedData(categories, visuals);
        EditorUtility.SetDirty(database);
    }

    private static WordDatabase FindWordDatabase()
    {
        string[] guids = AssetDatabase.FindAssets("t:WordDatabase");
        if (guids.Length > 0)
        {
            string existingPath = AssetDatabase.GUIDToAssetPath(guids[0]);
            WordDatabase existing = AssetDatabase.LoadAssetAtPath<WordDatabase>(existingPath);
            if (existing != null)
                return existing;
        }

        WordDatabase created = ScriptableObject.CreateInstance<WordDatabase>();
        AssetDatabase.CreateAsset(created, DatabasePath);
        return created;
    }

    private static List<CategoryVisualData> MigrateVisuals(
        WordDatabase database,
        List<WordCategory> categories)
    {
        Dictionary<string, CategoryVisualData> existingById = new(StringComparer.Ordinal);
        foreach (CategoryVisualData visual in database.CategoryVisuals)
        {
            if (visual == null || string.IsNullOrWhiteSpace(visual.CategoryId))
                continue;

            string id = LegacyCategoryIds.TryGetValue(visual.CategoryId, out string migrated)
                ? migrated
                : visual.CategoryId;
            existingById[id] = visual;
        }

        List<CategoryVisualData> result = new();
        foreach (WordCategory category in categories)
        {
            if (existingById.TryGetValue(category.CategoryId, out CategoryVisualData existing))
            {
                result.Add(new CategoryVisualData
                {
                    CategoryId = category.CategoryId,
                    CategoryColor = existing.CategoryColor,
                    CategoryImage = existing.CategoryImage
                });
            }
            else
            {
                result.Add(new CategoryVisualData
                {
                    CategoryId = category.CategoryId,
                    CategoryColor = Color.gray
                });
            }
        }

        return result;
    }

    private static void AddEntry(
        List<LocalizedEntry> target,
        HashSet<string> keys,
        CsvRow row,
        string key,
        string spanish,
        string english,
        bool isSmart,
        string context)
    {
        if (!keys.Add(key))
            throw Duplicate(row, "localization key", key);

        target.Add(new LocalizedEntry
        {
            Key = key,
            Spanish = spanish,
            English = english,
            IsSmart = isSmart,
            Context = context
        });
    }

    private static void ValidatePlaceholders(CsvRow row, bool isSmart, string spanish, string english)
    {
        HashSet<string> spanishNames = PlaceholderRegex.Matches(spanish)
            .Select(match => match.Groups[1].Value).ToHashSet(StringComparer.Ordinal);
        HashSet<string> englishNames = PlaceholderRegex.Matches(english)
            .Select(match => match.Groups[1].Value).ToHashSet(StringComparer.Ordinal);

        if (!spanishNames.SetEquals(englishNames))
            throw new InvalidDataException($"{row.File}:{row.Line} uses different placeholders between es-AR and en.");
        if (spanishNames.Count > 0 && !isSmart)
            throw new InvalidDataException($"{row.File}:{row.Line} contains placeholders but IsSmartString is false.");
        if (spanishNames.Count == 0 && isSmart)
            throw new InvalidDataException($"{row.File}:{row.Line} sets IsSmartString to true but contains no placeholders.");
    }

    private static InvalidDataException Duplicate(CsvRow row, string column, string value)
    {
        return new InvalidDataException($"{row.File}:{row.Line} duplicates {column} '{value}'.");
    }

    private static void EnsureAssetFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path))
            return;

        string parent = Path.GetDirectoryName(path)?.Replace('\\', '/');
        string folderName = Path.GetFileName(path);
        if (!string.IsNullOrEmpty(parent))
        {
            EnsureAssetFolder(parent);
            AssetDatabase.CreateFolder(parent, folderName);
        }
    }

    private sealed class LocalizedEntry
    {
        public string Key;
        public string Spanish;
        public string English;
        public bool IsSmart;
        public string Context;
    }

    private sealed class ImportResult
    {
        public readonly List<WordCategory> Categories = new();
        public readonly List<LocalizedEntry> ContentEntries = new();
        public readonly List<LocalizedEntry> UiEntries = new();
        public int WordCount;
        public int HintCount;
    }
}
