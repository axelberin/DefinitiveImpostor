using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class GameLocalization
{
    public const string UiTable = "UI";
    public const string ContentTable = "Content";
    public const string SpanishArgentina = "es-AR";
    public const string English = "en";

    private const string LegacyLocalePreferenceKey = "game.locale";

    private static bool initialized;
    private static bool initializing;
    private static bool subscribed;

    public static event Action LanguageChanged;

    public static bool IsInitialized => initialized;

    public static string CurrentLanguageCode
    {
        get
        {
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

        var operation = LocalizationSettings.InitializationOperation;
        yield return operation;

        if (operation.Status != AsyncOperationStatus.Succeeded)
        {
            initializing = false;
            Debug.LogError("Unity Localization could not be initialized.");
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

        Locale locale = GetSupportedLocale(requestedCode) ?? GetSupportedLocale(English);
        if (locale != null && LocalizationSettings.SelectedLocale != locale)
            LocalizationSettings.SelectedLocale = locale;

        if (locale != null)
            PersistLanguage(locale.Identifier.Code);

        initialized = true;
        initializing = false;
        LanguageChanged?.Invoke();
    }

    public static bool SetLanguage(string localeCode)
    {
        Locale locale = GetSupportedLocale(localeCode) ?? GetSupportedLocale(English);
        if (locale == null)
        {
            Debug.LogError("No supported localization locale is available.");
            return false;
        }

        if (LocalizationSettings.SelectedLocale == locale)
        {
            PersistLanguage(locale.Identifier.Code);
            LanguageChanged?.Invoke();
            return true;
        }

        LocalizationSettings.SelectedLocale = locale;
        return true;
    }

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

        string value = LocalizationSettings.StringDatabase.GetLocalizedString(
            table,
            key,
            LocalizationSettings.SelectedLocale,
            FallbackBehavior.UseProjectSettings,
            arguments ?? Array.Empty<object>());

        return string.IsNullOrWhiteSpace(value) ? $"[{key}]" : value;
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
        {
            PersistLanguage(locale.Identifier.Code);
        }

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
}
