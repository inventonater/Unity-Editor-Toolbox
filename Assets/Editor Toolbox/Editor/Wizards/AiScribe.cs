using System;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Toolbox;
using Object = UnityEngine.Object;

public class AiScribe : EditorWindow
{
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

    public class FileContentCache
    {
        public string Content { get; private set; }
        public int WordCount { get; private set; }
        public HashSet<string> Files { get; } = new(StringComparer.OrdinalIgnoreCase);

        public void Refresh(Options options, List<string> assetPaths = null)
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

            Files.Clear();

            foreach (var path in assetPaths)
            {
                var fullPath = Path.GetFullPath(path);
                if (Directory.Exists(fullPath))
                {
                    var descendants = Directory.GetFiles(fullPath, "*.cs", SearchOption.AllDirectories).Select(Path.GetFullPath);
                    foreach (var descendant in descendants) Files.Add(descendant);
                }
                else if (fullPath.EndsWith(".cs"))
                {
                    Files.Add(fullPath);
                }
            }


            var allFileContent = new StringBuilder();
            if (options.includeProjectMetadata) AppendProjectMetadata(allFileContent);
            if (options.includeSelectedGameObjectDetails) AppendGameObjectDetails(gameObjects, allFileContent);
            if (options.summarizeHierarchy) AppendHierarchy(allFileContent);

            foreach (var file in Files)
            {
                if (options.includeFileDetails) allFileContent.Append($"// Filename: {ToRelative(Path.GetFileName(file))}\n// Path: {file}\n\n");
                var fileContent = File.ReadAllText(file);
                allFileContent.Append(options.stripComments ? StripCommentsAndBlankLines(fileContent) : fileContent);
            }

            Content = allFileContent.ToString();
            WordCount = CountWords(Content);
        }

        private static string StripCommentsAndBlankLines(string sourceCode)
        {
            var commentRegex = @"\/\/.*|\/\*[\s\S]*?\*\/";
            sourceCode = Regex.Replace(sourceCode, commentRegex, "", RegexOptions.Multiline);

            var blankLineRegex = @"\r\n\s*\r\n|\n\s*\n"; // matches empty lines or lines with only whitespace characters
            sourceCode = Regex.Replace(sourceCode, blankLineRegex, "\n", RegexOptions.Multiline); // replace with a single newline character

            return sourceCode.Trim(); // remove leading and trailing whitespace characters
        }

        private static string ToRelative(string descendant) => Path.GetRelativePath(Application.dataPath, descendant);

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
                fileContent.Append($"There is a GameObject called '{gameObject.name}'.  It's path in the hierarchy is: {gameObject.GetPath()}'.  It has {components.Length} components on it [{componentStrings}]");

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
    }

    const string ToolName = nameof(AiScribe);

    string userPrompt = "Choose a Template or enter your custom prompt here...";
    private FileContentCache _fileContentCache = new();

    [Serializable]
    public class Options
    {
        public bool includeFileDetails = true;
        public bool includeProjectMetadata;
        public bool stripComments = true;
        public bool includeSelectedGameObjectDetails = true;
        public bool summarizeHierarchy = true;
    }
    public Options _options;

    PromptTemplate _promptTemplate = PromptTemplate.None;
    bool showHowToUse = false;
    private bool showCachedFiles = false;
    Vector2 userPromptScrollPosition;
    private Vector2 fileCacheScrollPosition;
    private static float _defaultMinWindowHeight = 320;

    public enum PromptTemplate
    {
        None,
        Explain,
        Fixes,
        AdditionalFunctions,
        Documentation,
        CodeReview,
        Performance,
    }

    [MenuItem("Tools/" + ToolName)]
    public static void ShowWindow()
    {;
        ShowWindow(_defaultMinWindowHeight);
    }

    private static void ShowWindow(float minWindowHeight)
    {
        var window = GetWindow<AiScribe>(ToolName);
        window.minSize = new Vector2(340, minWindowHeight);
    }

    void OnEnable()
    {
        Selection.selectionChanged += WhenSelectionChanged;
        WhenSelectionChanged();
    }

    void OnDisable()
    {
        Selection.selectionChanged -= WhenSelectionChanged;
    }

    void WhenSelectionChanged()
    {
        _fileContentCache.Refresh(_options);
        Repaint();
    }

    void OnGUI()
    {
        using (new EditorGUILayout.VerticalScope(GUI.skin.box))
        {
            RenderHowToUseSection();
            RenderCustomPromptSection();
            RenderFileInfoSection();
            RenderOptionsSection();
        }

        RenderFileList();

        if (GUILayout.Button("Copy Prompt to Clipboard")) CopyToClipboard();
    }

    private void RenderFileList()
    {
        using var scope = new EditorGUILayout.VerticalScope(GUI.skin.box);

        var prev = showCachedFiles;
        showCachedFiles = EditorGUILayout.Foldout(prev, $"Included Files ({_fileContentCache.Files.Count})");

        if(prev != showCachedFiles) ShowWindow(_defaultMinWindowHeight + (showCachedFiles ? 200 : 0));

        if (!showCachedFiles) return;

        fileCacheScrollPosition = EditorGUILayout.BeginScrollView(fileCacheScrollPosition, GUILayout.Height(200));
        foreach (var filePath in _fileContentCache.Files) EditorGUILayout.LabelField(filePath);
        EditorGUILayout.EndScrollView();
    }

    void RenderHowToUseSection()
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("?", GUILayout.Width(20))) showHowToUse = !showHowToUse;
            showHowToUse = EditorGUILayout.Foldout(showHowToUse, "How to Use");
        }

        if (!showHowToUse) return;

        using (new EditorGUILayout.VerticalScope(GUI.skin.box))
        {
            EditorGUILayout.LabelField(
                $"{ToolName} is a tool for collecting the text content of selected C# files into your clipboard for use with browser-based LLMs for code analysis and generation.",
                EditorStyles.wordWrappedLabel);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("- Choose a Template or enter a custom prompt to guide code processing.", EditorStyles.wordWrappedLabel);
            EditorGUILayout.LabelField("- Select C# script files or directories in your Unity project.", EditorStyles.wordWrappedLabel);
            EditorGUILayout.LabelField("- Click 'Copy to Clipboard' to copy the assembled text content.", EditorStyles.wordWrappedLabel);
            EditorGUILayout.LabelField("- Paste the clipboard content into your preferred web-based code tool.", EditorStyles.wordWrappedLabel);
            EditorGUILayout.LabelField("- Have a calzone, you are living in the future my friend!", EditorStyles.wordWrappedLabel);
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Ballpark of 2500 words tends to yield around 8000 tokens.  Check what the limit is for your LLM service.", EditorStyles.miniLabel);
        }
    }

    void RenderCustomPromptSection()
    {
        var oldPromptTemplate = _promptTemplate;
        _promptTemplate = (PromptTemplate)EditorGUILayout.EnumPopup(new GUIContent("Prompt Template", "Select the type of request to include in the prompt (will replace your current prompt)"), oldPromptTemplate);
        if (oldPromptTemplate != _promptTemplate) userPrompt = GetPromptTemplate(_promptTemplate);

        GUIStyle textAreaStyle = new GUIStyle(GUI.skin.textArea) { wordWrap = true };
        userPromptScrollPosition = EditorGUILayout.BeginScrollView(userPromptScrollPosition);
        userPrompt = EditorGUILayout.TextArea(userPrompt, textAreaStyle, GUILayout.ExpandHeight(true));
        EditorGUILayout.EndScrollView();
    }

    void RenderFileInfoSection()
    {
        EditorGUILayout.LabelField("Selected C# Files to Inject:", EditorStyles.boldLabel);
        EditorGUILayout.LabelField($"Word Count: {_fileContentCache.WordCount}");
    }

    void RenderOptionsSection()
    {
        using var changeScope = new EditorGUI.ChangeCheckScope();

        _options.includeFileDetails = EditorGUILayout.Toggle(new GUIContent("Include File Path", "Include file details such as filename and path"), _options.includeFileDetails);
        _options.stripComments = EditorGUILayout.Toggle(new GUIContent("Strip Comments", "Useful for refreshing stale comments or reducing the word count"), _options.stripComments);
        _options.includeSelectedGameObjectDetails = EditorGUILayout.Toggle(new GUIContent("Selected GameObject Details", "Include parent-child relationships and component list for selected game objects"), _options.includeSelectedGameObjectDetails);
        _options.summarizeHierarchy = EditorGUILayout.Toggle(new GUIContent("Summarize Hierarchy", "Include a JSON summary of the full scene hierarchy"), _options.summarizeHierarchy);
        _options.includeProjectMetadata = EditorGUILayout.Toggle(new GUIContent("Project Metadata", "Inject content of manifest.json and the Unity version [not recommended]"), _options.includeProjectMetadata);

        if (changeScope.changed)
        {
            _fileContentCache.Refresh(_options);
            Repaint();
        }
    }

    void CopyToClipboard()
    {
        EditorGUIUtility.systemCopyBuffer = BuildFullPrompt();
        Debug.Log($"{EditorGUIUtility.systemCopyBuffer.Length} characters copied to clipboard: \n{EditorGUIUtility.systemCopyBuffer}");
    }

    private string BuildFullPrompt()
    {
        return $"{userPrompt}\n\n\n{_fileContentCache.Content}";
    }

    string GetPromptTemplate(PromptTemplate promptTemplate)
    {
        return promptTemplate switch
        {
            PromptTemplate.Explain =>
                "Explain this code.  Tell me what it does and provide an explanation for any parts which might not be obvious to a junior developer.  If there are any missing implementation details which would be useful for you to see, tell me what those are.",
            PromptTemplate.Fixes => "Identify any potential bugs or issues with this code.  Explain potential fixes and show me the implementation.",
            PromptTemplate.AdditionalFunctions =>
                "Take a look at this code and suggest a bunch of new functions or capabilities which might make it more robust.  If there are any issues with the code, tell me about that first.  Show me the implementation.",
            PromptTemplate.Documentation => "Write documentation this code following C# best practices.  Make it easy for me to copy and paste into the source code.",
            PromptTemplate.CodeReview => "Analyze this code and give me a general code review.  Help me understand where things can be improved and extended.",
            PromptTemplate.Performance =>
                "Is this code performant for a low-compute device from Unity builds?  Are there any issues I can fix or areas that I can make improvements so that it runs efficiently and without excessive garbage collection?",
            _ => string.Empty
        };
    }
}
