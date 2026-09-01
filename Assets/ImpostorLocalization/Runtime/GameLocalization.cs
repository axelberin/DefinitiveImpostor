using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class GameLocalization
{
    public const string UiTable = "UI";
    public const string ContentTable = "Content";
    public const string SpanishArgentina = "es-AR";
    public const string English = "en";

    private const string LegacyLocalePreferenceKey = "game.locale";
    private const float OperationTimeoutSeconds = 12f;

    private static readonly List<TableReference> RequiredTables = new()
    {
        UiTable,
        ContentTable
    };

    private static bool initialized;
    private static bool initializationSucceeded;
    private static bool initializing;
    private static bool changingLanguage;
    private static bool subscribed;

    public static event Action LanguageChanged;

    /// <summary>
    /// True after the initialization attempt has finished, even if Unity Localization
    /// had to fall back to safe key placeholders.
    /// </summary>
    public static bool IsInitialized => initialized;

    /// <summary>
    /// True only when the locale catalog and the required string tables are available.
    /// </summary>
    public static bool IsReady => initialized && initializationSucceeded;
    public static bool IsChangingLanguage => changingLanguage;

    public static string CurrentLanguageCode
    {
        get
        {
            if (!IsReady)
                return English;

            Locale locale = LocalizationSettings.SelectedLocale;
            return locale != null ? locale.Identifier.Code : English;
        }
    }

    public static IEnumerator Initialize()
    {
        if (initialized)
            yield break;

        if (initializing)
        {
            while (initializing)
                yield return null;

            yield break;
        }

        initializing = true;

        AsyncOperationHandle operation;
        try
        {
            operation = LocalizationSettings.InitializationOperation;
        }
        catch (Exception exception)
        {
            FailInitialization("Unity Localization could not start initialization.", exception);
            yield break;
        }

        float startedAt = Time.realtimeSinceStartup;
        while (!operation.IsDone
               && Time.realtimeSinceStartup - startedAt < OperationTimeoutSeconds)
        {
            yield return null;
        }

        if (!operation.IsDone)
        {
            FailInitialization(
                $"Unity Localization initialization exceeded {OperationTimeoutSeconds:0} seconds.");
            yield break;
        }

        if (operation.Status != AsyncOperationStatus.Succeeded)
        {
            FailInitialization("Unity Localization could not be initialized.", operation.OperationException);
            yield break;
        }

        SubscribeOnce();

        string requestedCode;
        if (!LocalSettingsStorage.TryGetLanguageCode(out requestedCode))
        {
            requestedCode = PlayerPrefs.HasKey(LegacyLocalePreferenceKey)
                ? PlayerPrefs.GetString(LegacyLocalePreferenceKey, English)
                : DetectDeviceLanguage();
        }

        Locale englishLocale = GetSupportedLocale(English);
        if (englishLocale == null)
        {
            FailInitialization("The required English fallback locale is not available.");
            yield break;
        }

        Locale requestedLocale = GetSupportedLocale(requestedCode) ?? englishLocale;
        bool requestedLocaleReady = false;
        yield return PreloadRequiredTables(
            requestedLocale,
            result => requestedLocaleReady = result);

        if (!requestedLocaleReady && requestedLocale != englishLocale)
        {
            Debug.LogWarning(
                $"Locale '{requestedLocale.Identifier.Code}' could not be loaded. Falling back to English.");

            requestedLocale = englishLocale;
            yield return PreloadRequiredTables(
                requestedLocale,
                result => requestedLocaleReady = result);
        }

        if (!requestedLocaleReady)
        {
            FailInitialization("The required localization tables could not be loaded.");
            yield break;
        }

        initialized = true;
        initializationSucceeded = true;
        initializing = false;

        if (!ApplyLocale(requestedLocale))
        {
            initializationSucceeded = false;
            LanguageChanged?.Invoke();
        }
    }

    /// <summary>
    /// Changes language only after both required tables for that locale have loaded.
    /// The callback reports whether the requested language was actually applied.
    /// </summary>
    public static IEnumerator SetLanguageAsync(string localeCode, Action<bool> completed = null)
    {
        yield return Initialize();

        if (!IsReady)
        {
            completed?.Invoke(false);
            yield break;
        }

        if (changingLanguage)
        {
            while (changingLanguage)
                yield return null;

            completed?.Invoke(CurrentLanguageCode.Equals(
                localeCode,
                StringComparison.OrdinalIgnoreCase));
            yield break;
        }

        Locale locale = GetSupportedLocale(localeCode);
        if (locale == null)
        {
            Debug.LogError($"Locale '{localeCode}' is not available in this build.");
            completed?.Invoke(false);
            yield break;
        }

        if (LocalizationSettings.SelectedLocale == locale)
        {
            PersistLanguage(locale.Identifier.Code);
            LanguageChanged?.Invoke();
            completed?.Invoke(true);
            yield break;
        }

        changingLanguage = true;

        bool tablesReady = false;
        yield return PreloadRequiredTables(locale, result => tablesReady = result);

        bool applied = tablesReady && ApplyLocale(locale);
        changingLanguage = false;

        if (!applied)
        {
            Debug.LogError(
                $"Locale '{locale.Identifier.Code}' was not applied because its tables could not be loaded.");
            LanguageChanged?.Invoke();
        }

        completed?.Invoke(applied);
    }

    public static IEnumerator ToggleLanguageAsync(Action<bool> completed = null)
    {
        yield return Initialize();

        if (!IsReady)
        {
            completed?.Invoke(false);
            yield break;
        }

        string targetCode = CurrentLanguageCode.StartsWith(
            "es",
            StringComparison.OrdinalIgnoreCase)
            ? English
            : SpanishArgentina;

        yield return SetLanguageAsync(targetCode, completed);
    }

    // Kept for compatibility with existing callers. UI code should prefer SetLanguageAsync.
    public static bool SetLanguage(string localeCode)
    {
        if (!IsReady || changingLanguage)
            return false;

        Locale locale = GetSupportedLocale(localeCode);
        if (locale == null)
        {
            Debug.LogError($"Locale '{localeCode}' is not available in this build.");
            return false;
        }

        return ApplyLocale(locale);
    }

    // Kept for compatibility with existing callers. UI code should prefer ToggleLanguageAsync.
    public static void ToggleLanguage()
    {
        SetLanguage(CurrentLanguageCode.StartsWith("es", StringComparison.OrdinalIgnoreCase)
            ? English
            : SpanishArgentina);
    }

    public static string GetUi(string key, params object[] arguments)
    {
        return Get(UiTable, key, arguments);
    }

    public static string GetContent(string key, params object[] arguments)
    {
        return Get(ContentTable, key, arguments);
    }

    public static Dictionary<string, object> Args(params object[] keyValues)
    {
        if (keyValues == null || keyValues.Length % 2 != 0)
            throw new ArgumentException("Localization arguments must be supplied as key/value pairs.");

        Dictionary<string, object> result = new();
        for (int i = 0; i < keyValues.Length; i += 2)
            result[Convert.ToString(keyValues[i])] = keyValues[i + 1];

        return result;
    }

    private static string Get(string table, string key, object[] arguments)
    {
        if (string.IsNullOrWhiteSpace(key))
            return string.Empty;

        if (!IsReady)
            return $"[{key}]";

        try
        {
            string value = LocalizationSettings.StringDatabase.GetLocalizedString(
                table,
                key,
                LocalizationSettings.SelectedLocale,
                FallbackBehavior.UseProjectSettings,
                arguments ?? Array.Empty<object>());

            return string.IsNullOrWhiteSpace(value) ? $"[{key}]" : value;
        }
        catch (Exception exception)
        {
            Debug.LogError(
                $"Could not resolve localization key '{key}' from table '{table}': {exception.Message}");
            return $"[{key}]";
        }
    }

    private static IEnumerator PreloadRequiredTables(Locale locale, Action<bool> completed)
    {
        if (locale == null)
        {
            completed?.Invoke(false);
            yield break;
        }

        AsyncOperationHandle operation;
        try
        {
            operation = LocalizationSettings.StringDatabase.PreloadTables(RequiredTables, locale);
        }
        catch (Exception exception)
        {
            Debug.LogError(
                $"Could not start loading locale '{locale.Identifier.Code}': {exception.Message}");
            completed?.Invoke(false);
            yield break;
        }

        float startedAt = Time.realtimeSinceStartup;
        while (!operation.IsDone
               && Time.realtimeSinceStartup - startedAt < OperationTimeoutSeconds)
        {
            yield return null;
        }

        if (!operation.IsDone)
        {
            Debug.LogError(
                $"Loading locale '{locale.Identifier.Code}' exceeded {OperationTimeoutSeconds:0} seconds.");
            completed?.Invoke(false);
            yield break;
        }

        if (operation.Status != AsyncOperationStatus.Succeeded)
        {
            string reason = operation.OperationException != null
                ? operation.OperationException.Message
                : "Unknown Addressables error.";
            Debug.LogError($"Could not load locale '{locale.Identifier.Code}': {reason}");
            completed?.Invoke(false);
            yield break;
        }

        completed?.Invoke(true);
    }

    private static bool ApplyLocale(Locale locale)
    {
        if (locale == null)
            return false;

        try
        {
            if (LocalizationSettings.SelectedLocale != locale)
            {
                LocalizationSettings.SelectedLocale = locale;
            }
            else
            {
                PersistLanguage(locale.Identifier.Code);
                LanguageChanged?.Invoke();
            }

            return true;
        }
        catch (Exception exception)
        {
            Debug.LogError(
                $"Could not apply locale '{locale.Identifier.Code}': {exception.Message}");
            return false;
        }
    }

    private static void SubscribeOnce()
    {
        if (subscribed)
            return;

        LocalizationSettings.SelectedLocaleChanged += HandleSelectedLocaleChanged;
        subscribed = true;
    }

    private static void HandleSelectedLocaleChanged(Locale locale)
    {
        if (locale != null)
            PersistLanguage(locale.Identifier.Code);

        LanguageChanged?.Invoke();
    }

    private static Locale GetSupportedLocale(string localeCode)
    {
        if (LocalizationSettings.AvailableLocales == null || string.IsNullOrWhiteSpace(localeCode))
            return null;

        Locale exact = LocalizationSettings.AvailableLocales.GetLocale(new LocaleIdentifier(localeCode));
        if (exact != null)
            return exact;

        if (localeCode.StartsWith("es", StringComparison.OrdinalIgnoreCase))
            return LocalizationSettings.AvailableLocales.GetLocale(new LocaleIdentifier(SpanishArgentina));

        if (localeCode.StartsWith("en", StringComparison.OrdinalIgnoreCase))
            return LocalizationSettings.AvailableLocales.GetLocale(new LocaleIdentifier(English));

        return null;
    }

    private static string DetectDeviceLanguage()
    {
        return Application.systemLanguage == SystemLanguage.Spanish
            ? SpanishArgentina
            : English;
    }

    private static void PersistLanguage(string languageCode)
    {
        LocalSettingsStorage.SaveLanguageCode(languageCode);

        // Keep the previous preference as a fallback and migrate existing installations safely.
        if (!PlayerPrefs.HasKey(LegacyLocalePreferenceKey)
            || !string.Equals(
                PlayerPrefs.GetString(LegacyLocalePreferenceKey),
                languageCode,
                StringComparison.OrdinalIgnoreCase))
        {
            PlayerPrefs.SetString(LegacyLocalePreferenceKey, languageCode);
            PlayerPrefs.Save();
        }
    }

    private static void FailInitialization(string message, Exception exception = null)
    {
        initializationSucceeded = false;
        initialized = true;
        initializing = false;

        if (exception != null)
            Debug.LogError($"{message} {exception.Message}");
        else
            Debug.LogError(message);

        // Allow UI subscribers to leave their loading state and show safe placeholders.
        LanguageChanged?.Invoke();
    }
}
