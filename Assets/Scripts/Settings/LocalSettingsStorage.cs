using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public sealed class LocalSettingsData
{
    public int Version = LocalSettingsStorage.CurrentVersion;
    public string LanguageCode = string.Empty;
    public LocalGameConfigurationData GameConfiguration;
}

[Serializable]
public sealed class LocalGameConfigurationData
{
    public List<string> PlayerNames = new();
    public bool HintsEnabled;
    public bool RerollEnabled;
    public int ImpostorCount = 1;
    public List<string> EnabledCategoryIds = new();
    public List<string> KnownCategoryIds = new();
}

public static class LocalSettingsStorage
{
    public const int CurrentVersion = 1;

    private const string FileName = "impostor_settings.json";
    private const string BackupExtension = ".bak";
    private const string TemporaryExtension = ".tmp";

    private static bool loadAttempted;
    private static bool hasUnsavedChanges;
    private static LocalSettingsData cachedData;

    public static bool TryGetGameConfiguration(out LocalGameConfigurationData configuration)
    {
        EnsureLoaded();
        configuration = cachedData?.GameConfiguration;
        return configuration != null;
    }

    public static bool TryGetLanguageCode(out string languageCode)
    {
        EnsureLoaded();
        languageCode = cachedData?.LanguageCode;
        return !string.IsNullOrWhiteSpace(languageCode);
    }

    public static bool SaveGameConfiguration(LocalGameConfigurationData configuration)
    {
        if (configuration == null)
        {
            Debug.LogError("Cannot save a null game configuration.");
            return false;
        }

        LocalSettingsData data = GetOrCreateData();
        if (!hasUnsavedChanges && AreConfigurationsEqual(data.GameConfiguration, configuration))
            return true;

        data.GameConfiguration = configuration;
        hasUnsavedChanges = true;
        return SaveInternal(data, true);
    }

    public static bool SaveLanguageCode(string languageCode)
    {
        if (string.IsNullOrWhiteSpace(languageCode))
            return false;

        LocalSettingsData data = GetOrCreateData();
        if (!hasUnsavedChanges
            && string.Equals(data.LanguageCode, languageCode, StringComparison.OrdinalIgnoreCase))
            return true;

        data.LanguageCode = languageCode;
        hasUnsavedChanges = true;
        return SaveInternal(data, true);
    }

    private static LocalSettingsData GetOrCreateData()
    {
        EnsureLoaded();
        cachedData ??= new LocalSettingsData();
        cachedData.Version = CurrentVersion;
        return cachedData;
    }

    private static void EnsureLoaded()
    {
        if (loadAttempted)
            return;

        loadAttempted = true;

        string filePath = GetFilePath();
        if (TryRead(filePath, out cachedData))
            return;

        string backupPath = filePath + BackupExtension;
        if (!TryRead(backupPath, out cachedData))
            return;

        Debug.LogWarning("The local settings file was invalid. Its backup was loaded instead.");
        hasUnsavedChanges = true;
        SaveInternal(cachedData, false);
    }

    private static bool TryRead(string filePath, out LocalSettingsData data)
    {
        data = null;

        if (!File.Exists(filePath))
            return false;

        try
        {
            string json = File.ReadAllText(filePath);
            if (string.IsNullOrWhiteSpace(json))
                return false;

            data = JsonUtility.FromJson<LocalSettingsData>(json);
            if (data == null)
                return false;

            Normalize(data);
            return true;
        }
        catch (Exception exception)
        {
            Debug.LogWarning($"Could not read local settings from '{filePath}': {exception.Message}");
            return false;
        }
    }

    private static void Normalize(LocalSettingsData data)
    {
        if (data.Version <= 0)
            data.Version = CurrentVersion;

        data.LanguageCode ??= string.Empty;

        if (data.GameConfiguration == null)
            return;

        data.GameConfiguration.PlayerNames ??= new List<string>();
        data.GameConfiguration.EnabledCategoryIds ??= new List<string>();
        data.GameConfiguration.KnownCategoryIds ??= new List<string>();
        data.GameConfiguration.ImpostorCount = Math.Max(1, data.GameConfiguration.ImpostorCount);
    }

    private static bool SaveInternal(LocalSettingsData data, bool createBackup)
    {
        string filePath = GetFilePath();
        string temporaryPath = filePath + TemporaryExtension;
        string backupPath = filePath + BackupExtension;

        try
        {
            string directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrWhiteSpace(directory))
                Directory.CreateDirectory(directory);

            data.Version = CurrentVersion;
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(temporaryPath, json);

            if (createBackup && File.Exists(filePath))
                File.Copy(filePath, backupPath, true);

            File.Copy(temporaryPath, filePath, true);
            File.Delete(temporaryPath);

            cachedData = data;
            hasUnsavedChanges = false;
            return true;
        }
        catch (Exception exception)
        {
            Debug.LogError($"Could not save local settings: {exception.Message}");

            try
            {
                if (File.Exists(temporaryPath))
                    File.Delete(temporaryPath);
            }
            catch (Exception cleanupException)
            {
                Debug.LogWarning($"Could not delete the temporary settings file: {cleanupException.Message}");
            }

            return false;
        }
    }

    private static bool AreConfigurationsEqual(
        LocalGameConfigurationData first,
        LocalGameConfigurationData second)
    {
        if (ReferenceEquals(first, second))
            return true;
        if (first == null || second == null)
            return false;

        return first.HintsEnabled == second.HintsEnabled
            && first.RerollEnabled == second.RerollEnabled
            && first.ImpostorCount == second.ImpostorCount
            && AreListsEqual(first.PlayerNames, second.PlayerNames)
            && AreListsEqual(first.EnabledCategoryIds, second.EnabledCategoryIds)
            && AreListsEqual(first.KnownCategoryIds, second.KnownCategoryIds);
    }

    private static bool AreListsEqual(List<string> first, List<string> second)
    {
        if (ReferenceEquals(first, second))
            return true;
        if (first == null || second == null || first.Count != second.Count)
            return false;

        for (int i = 0; i < first.Count; i++)
        {
            if (!string.Equals(first[i], second[i], StringComparison.Ordinal))
                return false;
        }

        return true;
    }

    private static string GetFilePath()
    {
        return Path.Combine(Application.persistentDataPath, FileName);
    }
}
