using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Toolbox;
using UnityEditor;
using UnityEngine;

public class AiScribeFileContentCache
{
    public string Content { get; private set; }
    public int WordCount { get; private set; }
    private HashSet<string> _files { get; } = new(StringComparer.OrdinalIgnoreCase);
    public IReadOnlyList<string> RelativePaths => _files.Select(RelativePath).ToList();

    public void Refresh(AiScribe.Options options, List<string> assetPaths = null)
    {
        assetPaths ??= Selection.objects.Select(AssetDatabase.GetAssetPath).Where(p => !string.IsNullOrWhiteSpace(p)).ToList();
        var gameObjects = Selection.gameObjects;

        foreach (var gameObject in gameObjects)
        {
            var monoBehaviours = gameObject.GetComponents<MonoBehaviour>();

            foreach (var behaviour in monoBehaviours)
            {
                MonoScript monoScript = MonoScript.FromMonoBehaviour(behaviour);
                var assetPath = Path.GetFullPath(Path.Combine(Application.dataPath, "..", AssetDatabase.GetAssetPath(monoScript)));
                if (!string.IsNullOrWhiteSpace(assetPath) && assetPath.EndsWith(".cs"))
                {
                    assetPaths.Add(assetPath);
                }
            }
        }

        _files.Clear();

        foreach (var path in assetPaths)
        {
            var fullPath = Path.GetFullPath(path);
            if (Directory.Exists(fullPath))
            {
                var files = Directory.GetFiles(fullPath, "*.cs", SearchOption.AllDirectories).ToList();
                if (options.includeMarkDownFiles) files.AddRange(Directory.GetFiles(fullPath, "*.md", SearchOption.AllDirectories));
                var descendants = files.Select(Path.GetFullPath);
                foreach (var descendant in descendants) _files.Add(descendant);
            }
            else if (fullPath.EndsWith(".cs") || (options.includeMarkDownFiles && fullPath.EndsWith(".md")))
            {
                _files.Add(fullPath);
            }
        }

        var allFileContent = new StringBuilder();
        if (options.projectMetadata) AppendProjectMetadata(allFileContent);
        if (options.selectedGameObjects) AppendGameObjectDetails(gameObjects, allFileContent);
        if (options.summarizeHierarchy) AppendHierarchy(allFileContent);

        foreach (var file in _files) WriteFile(options, allFileContent, file);

        Content = allFileContent.ToString();
        WordCount = CountWords(Content);
    }

    private static void WriteFile(AiScribe.Options options, StringBuilder allFileContent, string file)
    {
        if (options.filePathDetails) allFileContent.AppendLine($"// Begin: {Path.GetFileName(file)} ({file})\n");
        var fileContent = File.ReadAllText(file);
        allFileContent.Append(options.stripComments ? StripCommentsAndBlankLines(fileContent) : fileContent);
        allFileContent.AppendLine("\n");
        if (options.filePathDetails) allFileContent.AppendLine($"// End: {Path.GetFileName(file)}");
        allFileContent.AppendLine("\n\n");
    }

    private static string StripCommentsAndBlankLines(string sourceCode)
    {
        var commentRegex = @"\/\/.*|\/\*[\s\S]*?\*\/";
        sourceCode = Regex.Replace(sourceCode, commentRegex, "", RegexOptions.Multiline);

        var blankLineRegex = @"\r\n\s*\r\n|\n\s*\n"; // matches empty lines or lines with only whitespace characters
        sourceCode = Regex.Replace(sourceCode, blankLineRegex, "\n", RegexOptions.Multiline); // replace with a single newline character

        return sourceCode.Trim(); // remove leading and trailing whitespace characters
    }

    private static string RelativePath(string descendant) => Path.GetRelativePath(Application.dataPath, descendant);

    private void AppendProjectMetadata(StringBuilder fileContent)
    {
        string projectName = PlayerSettings.productName;
        fileContent.AppendLine($"// Unity Version: {Application.unityVersion}");
        fileContent.AppendLine($"// Project Name: {projectName}");

        string manifestJsonPath = Path.Combine(Application.dataPath, "../Packages/manifest.json");
        fileContent.AppendLine($"// Manifest.json Contents: {File.ReadAllText(manifestJsonPath)}");
    }

    private void AppendGameObjectDetails(GameObject[] gameObjects, StringBuilder fileContent)
    {
        foreach (var gameObject in gameObjects)
        {
            var components = gameObject.GetComponents<Component>();
            var componentStrings = string.Join(", ", components.ToList());
            fileContent.Append(
                $"There is a GameObject called '{gameObject.name}'.  It's path in the hierarchy is: {gameObject.GetPath()}'.  It has {components.Length} components on it [{componentStrings}]");

            fileContent.Append($"\n{gameObject.name} has these children: \n");
            foreach (Transform child in gameObject.transform)
                fileContent.Append($"  - {child.gameObject.name}\n");

            fileContent.Append($"\n\n");
        }
    }

    private void AppendHierarchy(StringBuilder allFileContent)
    {
        List<Transform> FindRootNodes()
        {
            List<Transform> roots = new List<Transform>();
            foreach (GameObject obj in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects()) roots.Add(obj.transform);
            return roots;
        }

        string WriteString(List<Transform> roots = null, string indent = "")
        {
            roots ??= FindRootNodes();
            StringBuilder sb = new StringBuilder();
            foreach (var rootNode in roots) AddNodeToStringBuilder(rootNode, sb, indent);
            return sb.ToString();
        }

        void AddNodeToStringBuilder(Transform transform, StringBuilder sb, string indent)
        {
            sb.AppendLine($"{indent}{transform.gameObject.name}");
            for (int i = 0; i < transform.childCount; i++) AddNodeToStringBuilder(transform.GetChild(i), sb, indent + "  ");
        }

        allFileContent.Append("This is the hierarchy of the Unity scene:\n");
        allFileContent.Append(WriteString());
        allFileContent.Append("\n\n");
    }

    public static int CountWords(in string s)
    {
        var wordCount = 0;
        bool inWord = false;

        foreach (char c in s)
        {
            if (char.IsWhiteSpace(c) || char.IsPunctuation(c))
            {
                inWord = false;
            }
            else if (!inWord)
            {
                wordCount++;
                inWord = true;
            }
        }

        return wordCount;
    }

}