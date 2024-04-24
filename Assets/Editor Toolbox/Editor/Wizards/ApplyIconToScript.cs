using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public static class ApplyIconToScript
{
    private const string NoTextureWarning = "No Texture2D found in the selection.";
    private const string MultipleTexturesWarning = "More than one Texture2D selected. Please select only one Texture2D to use as the icon.";
    private const string NoCompatibleAssetsWarning = "No compatible assets selected to apply the icon.";

    /// <summary>
    /// Applies the selected icon to the selected MonoScripts and script files within selected directories.
    /// </summary>
    [MenuItem("Tools/Apply Icon to Script")]
    public static void ApplyIcon()
    {
        var selectedAssets = Selection.objects;
        var selectedTextures = selectedAssets.OfType<Texture2D>().ToList();

        if (selectedTextures.Count == 0)
        {
            Debug.LogWarning(NoTextureWarning);
            return;
        }

        if (selectedTextures.Count > 1)
        {
            Debug.LogWarning(MultipleTexturesWarning);
            return;
        }

        var icon = selectedTextures[0];
        var scriptsToApplyIcon = new HashSet<MonoScript>(selectedAssets.OfType<MonoScript>());
        var directories = selectedAssets.OfType<DefaultAsset>().Where(asset => AssetDatabase.IsValidFolder(AssetDatabase.GetAssetPath(asset)));

        foreach (var directory in directories)
        {
            string directoryPath = AssetDatabase.GetAssetPath(directory);
            string[] scriptGuids = AssetDatabase.FindAssets("t:MonoScript", new[] { directoryPath });

            foreach (string scriptGuid in scriptGuids)
            {
                string scriptPath = AssetDatabase.GUIDToAssetPath(scriptGuid);
                var script = AssetDatabase.LoadMainAssetAtPath(scriptPath) as MonoScript;
                if (script != null) scriptsToApplyIcon.Add(script);
            }
        }

        foreach (var script in scriptsToApplyIcon)
        {
            var path = AssetDatabase.GetAssetPath(script);
            var monoImporter = AssetImporter.GetAtPath(path) as MonoImporter;
            monoImporter.SetIcon(icon);
            AssetDatabase.ImportAsset(path);
        }

        AssetDatabase.Refresh();

        int successCount = scriptsToApplyIcon.Count;
        if (successCount > 0) Debug.Log($"Successfully applied icon to {successCount} MonoScript(s).");
        else Debug.LogWarning(NoCompatibleAssetsWarning);
    }
}