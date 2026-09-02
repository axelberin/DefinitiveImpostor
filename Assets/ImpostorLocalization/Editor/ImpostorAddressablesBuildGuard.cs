using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public sealed class ImpostorAddressablesBuildGuard : IPreprocessBuildWithReport
{
    private const string SpanishTablePath =
        "Assets/ImpostorLocalization/Data/Generated/String Tables/Content_es-AR.asset";

    public int callbackOrder => -10000;

    public void OnPreprocessBuild(BuildReport report)
    {
        if (report.summary.platform != BuildTarget.Android)
            return;

        List<string> errors = ValidateConfiguration();
        if (errors.Count > 0)
        {
            throw new BuildFailedException(
                "The Android build was stopped because Localization/Addressables is unsafe:\n- "
                + string.Join("\n- ", errors)
                + "\n\nRun Tools > Impostor > 1. Importar CSV y generar localización, "
                + "then rebuild Addressables before creating the AAB.");
        }

        Debug.Log("Impostor Android build guard: Localization and Addressables validation passed.");
    }

    private static List<string> ValidateConfiguration()
    {
        List<string> errors = new();
        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;

        if (settings == null)
        {
            errors.Add("Addressables Settings could not be found.");
            return errors;
        }

        if (settings.BuildAddressablesWithPlayerBuild
            != AddressableAssetSettings.PlayerBuildOption.BuildWithPlayer)
        {
            errors.Add("Build Addressables on Player Build must be set to Build with Player.");
        }

        string spanishTableGuid = AssetDatabase.AssetPathToGUID(SpanishTablePath);
        AddressableAssetEntry spanishEntry = settings.FindAssetEntry(spanishTableGuid);
        AddressableAssetGroup spanishGroup = spanishEntry?.parentGroup;

        if (spanishGroup == null)
        {
            errors.Add($"'{SpanishTablePath}' is not assigned to an Addressables group.");
            return errors;
        }

        BundledAssetGroupSchema schema = spanishGroup.GetSchema<BundledAssetGroupSchema>();
        if (schema == null)
        {
            errors.Add("The Spanish Addressables group has no BundledAssetGroupSchema.");
        }
        else
        {
            if (!schema.IncludeInBuild)
                errors.Add("The Spanish Addressables group is excluded from the Player build.");

            if (schema.BundleNaming != BundledAssetGroupSchema.BundleNamingStyle.OnlyHash)
            {
                errors.Add(
                    "The Spanish Addressables group Bundle Naming Mode must be 'Only Hash' "
                    + "so its Android runtime filename contains ASCII characters only. "
                    + "The group's visible name may remain localized.");
            }
        }

        return errors;
    }
}