using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Impostor/Word Database")]
public class WordDatabase : ScriptableObject
{
    [SerializeField] private List<WordCategory> categories = new();
    [SerializeField] private List<CategoryVisualData> categoryVisualData = new();

    private readonly Dictionary<string, WordCategory> categoriesById = new(StringComparer.Ordinal);

    public IReadOnlyList<WordCategory> Categories => categories;
    public IReadOnlyList<CategoryVisualData> CategoryVisuals => categoryVisualData;

    private void OnEnable()
    {
        RebuildLookup();
    }

    public WordCategory GetCategory(string categoryId)
    {
        if (string.IsNullOrWhiteSpace(categoryId))
            return null;

        if (categoriesById.Count != categories.Count)
            RebuildLookup();

        categoriesById.TryGetValue(categoryId, out WordCategory category);
        return category;
    }

    public WordData GetRandomWordFromEnabledCategories(IReadOnlyCollection<string> enabledCategoryIds, out string selectedCategoryId)
    {
        selectedCategoryId = string.Empty;

        if (enabledCategoryIds == null || enabledCategoryIds.Count == 0)
            return null;

        List<WordCategory> available = new();
        foreach (WordCategory category in categories)
        {
            if (category.CategoryType == CategoryType.Normal
                && category.Words != null
                && category.Words.Count > 0
                && enabledCategoryIds.Contains(category.CategoryId))
            {
                available.Add(category);
            }
        }

        if (available.Count == 0)
            return null;

        WordCategory selected = available[UnityEngine.Random.Range(0, available.Count)];
        selectedCategoryId = selected.CategoryId;
        return selected.Words[UnityEngine.Random.Range(0, selected.Words.Count)];
    }

    public void ReplaceImportedData(List<WordCategory> importedCategories, List<CategoryVisualData> visuals)
    {
        categories = importedCategories ?? new List<WordCategory>();
        categoryVisualData = visuals ?? new List<CategoryVisualData>();
        ApplyVisualsToCategories();
        RebuildLookup();
    }

    private void ApplyVisualsToCategories()
    {
        Dictionary<string, CategoryVisualData> visualsById = new(StringComparer.Ordinal);
        foreach (CategoryVisualData visual in categoryVisualData)
        {
            if (visual != null && !string.IsNullOrWhiteSpace(visual.CategoryId))
                visualsById[visual.CategoryId] = visual;
        }

        foreach (WordCategory category in categories)
        {
            if (!visualsById.TryGetValue(category.CategoryId, out CategoryVisualData visual))
                continue;

            category.CategoryColor = visual.CategoryColor;
            category.CategoryImage = visual.CategoryImage;
        }
    }

    private void RebuildLookup()
    {
        categoriesById.Clear();
        if (categories == null)
            categories = new List<WordCategory>();

        foreach (WordCategory category in categories)
        {
            if (category != null && !string.IsNullOrWhiteSpace(category.CategoryId))
                categoriesById[category.CategoryId] = category;
        }
    }
}

[Serializable]
public class CategoryVisualData
{
    [FormerlySerializedAs("CategoryName")]
    public string CategoryId;

    [FormerlySerializedAs("Description")]
    [HideInInspector]
    public string LegacyDescription;

    public Color CategoryColor = Color.white;
    public Sprite CategoryImage;
}
