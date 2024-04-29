using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;

public class AICodeScribe : EditorWindow
{
    const string ToolName = nameof(AICodeScribe);

    string customPrompt = "Enter your prompt here...";
    bool includeFileDetails = true;
    bool includeSeparators = true;
    bool includeProjectMetadata = true;
    RequestType requestType = RequestType.NotSpecified;
    bool showHowToUse = false;

    Vector2 scrollPosition;

    Dictionary<string, string> fileContentsCache = new Dictionary<string, string>();
    HashSet<string> selectedPaths = new HashSet<string>();

    int cachedFileContentCharacterCount = 0;
    private int _totalFileCount;
    private int _directoryCount;

    public enum RequestType
    {
        NotSpecified,
        RequestDocumentation,
        RequestFixes,
        RequestAdditionalFunctions,
        RequestCodeReview
    }

    [MenuItem("Tools/" + ToolName)]
    public static void ShowWindow()
    {
        var window = GetWindow<AICodeScribe>(ToolName);
        window.minSize = new Vector2(340, 320);
    }

    void OnEnable()
    {
        Selection.selectionChanged += UpdateSelectedItemsInfo;
        UpdateSelectedItemsInfo();
    }

    void OnDisable()
    {
        Selection.selectionChanged -= UpdateSelectedItemsInfo;
    }

    void UpdateSelectedItemsInfo()
    {
        selectedPaths.Clear();
        fileContentsCache.Clear();

        HashSet<string> selectedDirectories = new HashSet<string>();
        HashSet<string> selectedFiles = new HashSet<string>();

        foreach (var obj in Selection.objects)
        {
            var path = AssetDatabase.GetAssetPath(obj);
            if (Directory.Exists(path))
            {
                selectedDirectories.Add(path);
            }
            else if (path.EndsWith(".cs"))
            {
                selectedFiles.Add(path);
            }
        }

        // Discard file selections that are descendants of selected directories
        foreach (var filePath in selectedFiles)
        {
            if (selectedDirectories.Any(directoryPath => filePath.StartsWith(directoryPath + "/"))) continue;

            selectedPaths.Add(filePath);
            fileContentsCache[filePath] = File.ReadAllText(filePath);
        }

        // Cache files from selected directories
        foreach (var directoryPath in selectedDirectories)
        {
            CacheFilesFromDirectory(directoryPath, selectedPaths);
        }

        _totalFileCount = selectedPaths.Count;
        _directoryCount = selectedDirectories.Count;
        cachedFileContentCharacterCount = CalculateFileContentCharacterCount();
        Repaint();
    }

    void CacheFilesFromDirectory(string directoryPath, HashSet<string> processedPaths)
    {
        foreach (var filePath in Directory.GetFiles(directoryPath, "*.cs", SearchOption.AllDirectories))
        {
            if (!processedPaths.Contains(filePath))
            {
                selectedPaths.Add(filePath);
                fileContentsCache[filePath] = File.ReadAllText(filePath);
                processedPaths.Add(filePath);
            }
        }
    }

    void OnGUI()
    {
        using (new EditorGUILayout.VerticalScope(GUI.skin.box))
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("?", GUILayout.Width(20))) showHowToUse = !showHowToUse;
                showHowToUse = EditorGUILayout.Foldout(showHowToUse, "How to Use");
            }
        }

        if (showHowToUse)
        {
            using (new EditorGUILayout.VerticalScope(GUI.skin.box))
            {
                EditorGUILayout.LabelField($"{ToolName} is a tool for collecting the text content of selected C# files into your clipboard for use with browser-based LLMs for code analysis and generation.", EditorStyles.wordWrappedLabel);
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("1. Select C# script files or directories in your Unity project.", EditorStyles.wordWrappedLabel);
                EditorGUILayout.LabelField("2. Enter a custom prompt to guide code processing.", EditorStyles.wordWrappedLabel);
                EditorGUILayout.LabelField("3. Customize settings for file details, separators, and project metadata.", EditorStyles.wordWrappedLabel);
                EditorGUILayout.LabelField("4. Choose an optional Request Type if you'd like to inject additional pre-tuned steering for the prompt.", EditorStyles.wordWrappedLabel);
                EditorGUILayout.LabelField("5. Click 'Copy to Clipboard' to copy the assembled text content.", EditorStyles.wordWrappedLabel);
                EditorGUILayout.LabelField("6. Paste the clipboard content into your preferred web-based code tool.", EditorStyles.wordWrappedLabel);
                EditorGUILayout.LabelField("7. Have a calzone, you are living in the future my friend!", EditorStyles.wordWrappedLabel);
            }
        }

        EditorGUI.BeginChangeCheck();

        using (new EditorGUILayout.VerticalScope(GUI.skin.box))
        {
            GUIStyle textAreaStyle = new GUIStyle(GUI.skin.textArea) { wordWrap = true };
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            customPrompt = EditorGUILayout.TextArea(customPrompt, textAreaStyle, GUILayout.ExpandHeight(true));
            EditorGUILayout.EndScrollView();
        }

        using (new EditorGUILayout.VerticalScope(GUI.skin.box))
        {
            EditorGUILayout.LabelField("Selected C# Files to Inject:", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"{_totalFileCount} script file(s)");
            EditorGUILayout.LabelField($"{_directoryCount} directory/directories");
            EditorGUILayout.LabelField($"{cachedFileContentCharacterCount} characters cached from selected files");
            EditorGUILayout.LabelField($"Character Count Total: {AppendPromptAndRequestType().Length + cachedFileContentCharacterCount}", EditorStyles.boldLabel);
        }

        using (new EditorGUILayout.VerticalScope(GUI.skin.box))
        {
            includeFileDetails = EditorGUILayout.Toggle(new GUIContent("Include File Details", "Include file details such as filename, path, and size"), includeFileDetails);
            if (EditorGUI.EndChangeCheck())
            {
                UpdateCharacterCount();
            }

            EditorGUI.BeginChangeCheck();
            includeSeparators = EditorGUILayout.Toggle(new GUIContent("Include Separators", "Include separators (extra line) between code snippets"), includeSeparators);
            if (EditorGUI.EndChangeCheck())
            {
                UpdateCharacterCount();
            }

            EditorGUI.BeginChangeCheck();
            includeProjectMetadata = EditorGUILayout.Toggle(new GUIContent("Include Project Metadata", "Include project metadata in the copied content"), includeProjectMetadata);
            if (EditorGUI.EndChangeCheck())
            {
                UpdateCharacterCount();
            }

            requestType = (RequestType)EditorGUILayout.EnumPopup(new GUIContent("Request Type", "Select the type of request to include in the prompt"), requestType);
        }

        if (GUILayout.Button("Copy to Clipboard")) CopyToClipboard();
    }

    void CopyToClipboard()
    {
        StringBuilder contentBuilder = new StringBuilder(AppendPromptAndRequestType());
        AppendSelectedFileContents(contentBuilder);

        string clipboardContent = contentBuilder.ToString();
        EditorGUIUtility.systemCopyBuffer = clipboardContent;
        Debug.Log($"{EditorGUIUtility.systemCopyBuffer.Length} characters copied to clipboard: \n{EditorGUIUtility.systemCopyBuffer}");
    }

    string AppendPromptAndRequestType()
    {
        return $"{customPrompt}\n\n{GetRequestTypeMessage()}\n\n";
    }

    void AppendSelectedFileContents(StringBuilder builder)
    {
        foreach (var filePath in selectedPaths)
        {
            if (includeFileDetails)
            {
                AppendFileDetails(builder, filePath);
            }

            builder.AppendLine(fileContentsCache[filePath]);

            if (includeSeparators)
            {
                builder.AppendLine();
            }
        }
    }

    void AppendFileDetails(StringBuilder builder, string filePath)
    {
        builder.AppendLine($"// Filename: {Path.GetFileName(filePath)}");
        builder.AppendLine($"// Path: {filePath}");
        builder.AppendLine($"// File size: {new FileInfo(filePath).Length} bytes");
        builder.AppendLine("// Begin File Content");
    }

    void UpdateCharacterCount()
    {
        cachedFileContentCharacterCount = CalculateFileContentCharacterCount();
        Repaint();
    }

    int CalculateFileContentCharacterCount()
    {
        int characterCount = 0;

        foreach (var filePath in selectedPaths)
        {
            characterCount += fileContentsCache[filePath].Length;

            if (includeFileDetails)
            {
                characterCount += $"// Filename: {Path.GetFileName(filePath)}\n".Length;
                characterCount += $"// Path: {filePath}\n".Length;
                characterCount += $"// File size: {new FileInfo(filePath).Length} bytes\n".Length;
                characterCount += "// Begin File Content\n".Length;
            }

            if (includeSeparators)
            {
                characterCount += "\n".Length;
            }
        }

        return characterCount;
    }

    string GetRequestTypeMessage()
    {
        switch (requestType)
        {
            case RequestType.RequestDocumentation:
                return "Please provide documentation for the following code snippets:";
            case RequestType.RequestFixes:
                return "Please provide fixes for the following code snippets:";
            case RequestType.RequestAdditionalFunctions:
                return "Please provide additional functions for the following code snippets:";
            case RequestType.RequestCodeReview:
                return "Please review the following code snippets:";
            default:
                return "";
        }
    }
}