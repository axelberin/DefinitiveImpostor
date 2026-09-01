using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class LocalizationSceneSetup
{
    private const string CanvasPrefabGuid = "78bf6ea867aa0104197222911dcb2707";
    private const string UiCsvPath = "Assets/ImpostorLocalization/Data/Source/UI.csv";

    private static readonly string[] DynamicTextProperties =
    {
        "selectedCategoriesText",
        "impostorCountText",
        "revealPlayersCounterText",
        "playerNameText",
        "playerCounterText",
        "categoryText",
        "roleText",
        "wordText",
        "hintText",
        "wordDescriptionText",
        "discussionTitleText",
        "impostorsResultText",
        "wordResultText",
        "errorText"
    };

    private static readonly string[] StartupDisabledScreenProperties =
    {
        "setupScreen",
        "menuSetupScreen",
        "impostorSetupScreen",
        "playersSetupScreen",
        "categorySetupScreen",
        "revealScreen",
        "playerRevealSelectionScreen",
        "roleHiddenScreen",
        "roleVisibleScreen",
        "discussionScreen",
        "resultsScreen",
        "errorScreen",
        "wordDescriptionPanel"
    };

    private static readonly Dictionary<string, string> LegacyTextAliases = new(StringComparer.Ordinal)
    {
        ["Configura tu partida"] = "ui.setup.title",
        ["Ajusta las reglas del Impostor"] = "ui.impostors.subtitle",
        ["Agrega al menos 3 jugadores para comenzar"] = "ui.players.empty_help",
        ["Rolear palabra"] = "ui.players.reroll_label",
        ["A JUGAR!"] = "ui.reveal_selection.title",
        ["Toca tu nombre para revelar tu palabra \ny luego pasa el dispositivo al siguiente\n jugador."] = "ui.reveal_selection.subtitle",
        ["Cada jugador dice una palabra relacionada con la palabra secreta, luego voten a quien crean que es el Impostor"] = "ui.discussion.instructions",
        ["'Impostores:'"] = "ui.results.impostors_label",
        ["'Palabra secreta:'"] = "ui.results.word_label"
    };

    [MenuItem("Tools/Impostor/2. Configurar Canvas y selector de idioma")]
    public static void ConfigureCanvas()
    {
        string prefabPath = FindCanvasPrefab();
        if (string.IsNullOrWhiteSpace(prefabPath))
        {
            EditorUtility.DisplayDialog(
                "Impostor Localization",
                "No se encontró un Canvas.prefab que contenga UIManager.",
                "Aceptar");
            return;
        }

        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            UIManager uiManager = root.GetComponentInChildren<UIManager>(true);
            if (uiManager == null)
                throw new InvalidOperationException("Canvas.prefab does not contain UIManager.");

            Dictionary<string, string> staticKeys = LoadStaticTextKeys();
            HashSet<TMP_Text> dynamicTexts = GetDynamicTexts(uiManager);
            int boundCount = BindStaticTexts(root, staticKeys, dynamicTexts);
            EnsureLanguageSelector(root);
            ConfigureSafeStartupState(uiManager);

            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
            AssetDatabase.SaveAssets();

            Debug.Log($"Localization UI setup completed: {boundCount} static TMP texts bound in {prefabPath}.");
            EditorUtility.DisplayDialog(
                "Impostor Localization",
                $"Canvas configurado.\n\nTextos estáticos enlazados: {boundCount}\nSelector persistente: ES/EN",
                "Aceptar");
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
            EditorUtility.DisplayDialog(
                "Error al configurar Canvas",
                exception.Message,
                "Aceptar");
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void ConfigureSafeStartupState(UIManager uiManager)
    {
        SerializedObject serializedManager = new(uiManager);
        SetReferencedObjectActive(serializedManager, "initialScreen", true);

        foreach (string propertyName in StartupDisabledScreenProperties)
            SetReferencedObjectActive(serializedManager, propertyName, false);
    }

    private static void SetReferencedObjectActive(
        SerializedObject serializedManager,
        string propertyName,
        bool isActive)
    {
        SerializedProperty property = serializedManager.FindProperty(propertyName);
        if (property?.objectReferenceValue is not GameObject target)
            return;

        target.SetActive(isActive);
        EditorUtility.SetDirty(target);
    }

    private static string FindCanvasPrefab()
    {
        string path = AssetDatabase.GUIDToAssetPath(CanvasPrefabGuid);
        if (!string.IsNullOrWhiteSpace(path))
        {
            GameObject exact = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (exact != null && exact.GetComponentInChildren<UIManager>(true) != null)
                return path;
        }

        foreach (string guid in AssetDatabase.FindAssets("Canvas t:Prefab"))
        {
            string candidatePath = AssetDatabase.GUIDToAssetPath(guid);
            GameObject candidate = AssetDatabase.LoadAssetAtPath<GameObject>(candidatePath);
            if (candidate != null && candidate.GetComponentInChildren<UIManager>(true) != null)
                return candidatePath;
        }

        return null;
    }

    private static Dictionary<string, string> LoadStaticTextKeys()
    {
        CsvFile uiCsv = CsvFile.Read(UiCsvPath);
        uiCsv.RequireColumns("Key", "IsSmartString", "es-AR");

        Dictionary<string, string> result = new(StringComparer.Ordinal);
        foreach (CsvRow row in uiCsv.Rows)
        {
            if (!bool.TryParse(row.Require("IsSmartString"), out bool isSmart) || isSmart)
                continue;

            string key = row.Require("Key");
            if (key.StartsWith("content.", StringComparison.Ordinal))
                continue;

            result[row.Require("es-AR")] = key;
        }

        foreach ((string text, string key) in LegacyTextAliases)
            result[text] = key;

        return result;
    }

    private static HashSet<TMP_Text> GetDynamicTexts(UIManager uiManager)
    {
        SerializedObject serialized = new(uiManager);
        HashSet<TMP_Text> result = new();

        foreach (string propertyName in DynamicTextProperties)
        {
            SerializedProperty property = serialized.FindProperty(propertyName);
            if (property?.objectReferenceValue is TMP_Text text)
                result.Add(text);
        }

        return result;
    }

    private static int BindStaticTexts(
        GameObject root,
        IReadOnlyDictionary<string, string> keys,
        HashSet<TMP_Text> dynamicTexts)
    {
        int count = 0;
        foreach (TMP_Text text in root.GetComponentsInChildren<TMP_Text>(true))
        {
            if (dynamicTexts.Contains(text))
                continue;

            string currentValue = text.text?.Trim();
            if (string.IsNullOrWhiteSpace(currentValue)
                || !keys.TryGetValue(currentValue, out string key))
            {
                continue;
            }

            LocalizedTextUI localized = text.GetComponent<LocalizedTextUI>();
            if (localized == null)
                localized = text.gameObject.AddComponent<LocalizedTextUI>();

            localized.Setup(GameLocalization.UiTable, key);
            EditorUtility.SetDirty(localized);
            count++;
        }

        return count;
    }

    private static void EnsureLanguageSelector(GameObject root)
    {
        LanguageSelectorUI existing = root.GetComponentInChildren<LanguageSelectorUI>(true);
        if (existing != null)
        {
            existing.transform.SetAsLastSibling();
            return;
        }

        Transform canvasTransform = root.GetComponentsInChildren<Canvas>(true)
            .OrderBy(canvas => canvas.isRootCanvas ? 0 : 1)
            .Select(canvas => canvas.transform)
            .FirstOrDefault() ?? root.transform;

        GameObject selectorObject = new(
            "LanguageSelector",
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(Image),
            typeof(Button),
            typeof(LanguageSelectorUI));
        selectorObject.transform.SetParent(canvasTransform, false);
        selectorObject.transform.SetAsLastSibling();

        RectTransform rect = selectorObject.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.one;
        rect.anchorMax = Vector2.one;
        rect.pivot = Vector2.one;
        rect.sizeDelta = new Vector2(76f, 44f);
        rect.anchoredPosition = new Vector2(-20f, -20f);

        Image image = selectorObject.GetComponent<Image>();
        image.color = new Color(0.08f, 0.08f, 0.1f, 0.88f);

        Button button = selectorObject.GetComponent<Button>();
        button.targetGraphic = image;

        GameObject labelObject = new(
            "Label",
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(TextMeshProUGUI));
        labelObject.transform.SetParent(selectorObject.transform, false);

        RectTransform labelRect = labelObject.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;

        TextMeshProUGUI label = labelObject.GetComponent<TextMeshProUGUI>();
        label.text = "EN";
        label.alignment = TextAlignmentOptions.Center;
        label.fontSize = 20f;
        label.fontStyle = FontStyles.Bold;
        label.color = Color.white;
        label.raycastTarget = false;
        if (TMP_Settings.defaultFontAsset != null)
            label.font = TMP_Settings.defaultFontAsset;

        selectorObject.GetComponent<LanguageSelectorUI>().Setup(button, label);
    }
}
