using System;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;

public class AICodeScribe : EditorWindow
{
    const string ToolName = nameof(AICodeScribe);

    string customPrompt = "Enter your prompt here...";
    bool includeFileDetails = true;
    bool includeProjectMetadata = true;
    PromptTemplate _promptTemplate = PromptTemplate.None;
    bool showHowToUse = false;

    Vector2 scrollPosition;

    Dictionary<string, string> fileContentsCache = new Dictionary<string, string>();
    HashSet<string> selectedPaths = new HashSet<string>();

    int _totalFileCount;
    int _directoryCount;

    private int _cachedFileContentWordCount;
    private int _projectMetadataWordCount;
    private int _cachedFileContentCharacterCount;
    private int _projectMetadataCharacterCount;

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

        CalculateProjectMetadata(out _projectMetadataWordCount, out _projectMetadataCharacterCount);
        CalculateFileContentCounts(out _cachedFileContentWordCount, out _cachedFileContentCharacterCount);

        Repaint(); // This will force the editor to redraw itself.
    }

    void CacheFilesFromDirectory(string directoryPath, HashSet<string> processedPaths)
    {
        foreach (var filePath in Directory.GetFiles(directoryPath, "*.cs", SearchOption.AllDirectories))
        {
            if (processedPaths.Contains(filePath)) continue;

            selectedPaths.Add(filePath);
            fileContentsCache[filePath] = File.ReadAllText(filePath);
            processedPaths.Add(filePath);
        }
    }

    void RenderFileInfoSection()
    {
        EditorGUILayout.LabelField("Selected C# Files to Inject:", EditorStyles.boldLabel);
        EditorGUILayout.LabelField($"{_totalFileCount} script file(s)");
        EditorGUILayout.LabelField($"{_directoryCount} directory/directories");

        var promptAndRequestType = AppendPromptAndRequestType();

        int characterCount = promptAndRequestType.Length + _cachedFileContentCharacterCount;
        int wordCount = CountWords(promptAndRequestType) + _cachedFileContentWordCount;
        if (includeProjectMetadata)
        {
            characterCount += _projectMetadataCharacterCount;
            wordCount += _projectMetadataWordCount;
        }

        EditorGUILayout.LabelField($"Character Count Total: {characterCount}", EditorStyles.boldLabel);
        EditorGUILayout.LabelField($"Total Word Count: {wordCount}", EditorStyles.boldLabel);
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

        if (GUILayout.Button("Copy to Clipboard")) CopyToClipboard();
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
            EditorGUILayout.LabelField($"{ToolName} is a tool for collecting the text content of selected C# files into your clipboard for use with browser-based LLMs for code analysis and generation.", EditorStyles.wordWrappedLabel);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("1. Select C# script files or directories in your Unity project.", EditorStyles.wordWrappedLabel);
            EditorGUILayout.LabelField("2. Enter a custom prompt to guide code processing.", EditorStyles.wordWrappedLabel);
            EditorGUILayout.LabelField("3. Customize settings for file details and project metadata.", EditorStyles.wordWrappedLabel);
            EditorGUILayout.LabelField("4. Choose an optional Request Type if you'd like to inject additional pre-tuned steering for the prompt.", EditorStyles.wordWrappedLabel);
            EditorGUILayout.LabelField("5. Click 'Copy to Clipboard' to copy the assembled text content.", EditorStyles.wordWrappedLabel);
            EditorGUILayout.LabelField("6. Paste the clipboard content into your preferred web-based code tool.", EditorStyles.wordWrappedLabel);
            EditorGUILayout.LabelField("7. Have a calzone, you are living in the future my friend!", EditorStyles.wordWrappedLabel);
        }
    }

    void RenderCustomPromptSection()
    {
        GUIStyle textAreaStyle = new GUIStyle(GUI.skin.textArea) { wordWrap = true };
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        customPrompt = EditorGUILayout.TextArea(customPrompt, textAreaStyle, GUILayout.ExpandHeight(true));
        EditorGUILayout.EndScrollView();
    }

    void RenderOptionsSection()
    {
        using var changeScope = new EditorGUI.ChangeCheckScope();

        includeFileDetails = EditorGUILayout.Toggle(new GUIContent("Include File Details", "Include file details such as filename, path, and size"), includeFileDetails);
        includeProjectMetadata = EditorGUILayout.Toggle(new GUIContent("Include Project Metadata", "Include project metadata in the copied content"), includeProjectMetadata);
        _promptTemplate = (PromptTemplate)EditorGUILayout.EnumPopup(new GUIContent("Request Type", "Select the type of request to include in the prompt"), _promptTemplate);

        if(changeScope.changed)
        {
            UpdateCharacterCounts();
            Repaint(); // Force the editor to redraw after changing options.
        }
    }

    void CopyToClipboard()
    {
        StringBuilder contentBuilder = new StringBuilder(AppendPromptAndRequestType());
        AppendSelectedFileContents(contentBuilder);
        if (includeProjectMetadata) contentBuilder.Append(GetProjectMetadata());

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
        }
    }

    void AppendFileDetails(StringBuilder builder, string filePath)
    {
        if (includeFileDetails)
        {
            builder.Append(GetFileInfo(filePath));
        }
    }
    void UpdateCharacterCounts()
    {
        CalculateProjectMetadata(out _projectMetadataWordCount, out _projectMetadataCharacterCount);
        CalculateFileContentCounts(out _cachedFileContentWordCount, out _cachedFileContentCharacterCount);
        Repaint(); // Force the editor to redraw after updating character counts.
    }

    void CalculateFileContentCounts(out int wordCount, out int characterCount)
    {
        wordCount = 0;
        characterCount = 0;

        foreach (var filePath in selectedPaths)
        {
            string fileContent = fileContentsCache[filePath];
            wordCount += CountWords(fileContent);
            characterCount += fileContent.Length;

            if (includeFileDetails)
            {
                string fileInfo = GetFileInfo(filePath);
                wordCount += CountWords(fileInfo);
                characterCount += fileInfo.Length;
            }
        }
    }

    void CalculateProjectMetadata(out int wordCount, out int characterCount)
    {
        string projectMetadata = GetProjectMetadata();
        wordCount = CountWords(projectMetadata);
        characterCount = projectMetadata.Length;
    }

    int CountWords(string text)
    {
        return text.Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;
    }

    string GetFileInfo(string filePath)
    {
        return $"// Filename: {Path.GetFileName(filePath)}\n" +
               $"// Path: {filePath}\n" +
               $"// File size: {new FileInfo(filePath).Length} bytes\n" +
               "// Begin File Content\n";
    }

    string GetProjectMetadata()
    {
        string unityVersion = Application.unityVersion;
        string manifestJsonPath = Path.Combine(Application.dataPath, "../Packages/manifest.json");

        try
        {
            string manifestJsonContent = File.ReadAllText(manifestJsonPath);
            string projectName = PlayerSettings.productName;
            string companyName = PlayerSettings.companyName;

            return $"// Unity Version: {unityVersion}\n" +
                   $"// Project Name: {projectName}\n" +
                   $"// Company Name: {companyName}\n" +
                   "// Manifest.json Contents:\n" +
                   manifestJsonContent +
                   "\n";
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            return "";
        }
    }

    string GetRequestTypeMessage()
    {
        return _promptTemplate switch
        {
            PromptTemplate.Explain => "Explain this code.  Tell me what it does and provide an explanation for any parts which might not be obvious to a junior developer.  If there are any missing implementation details which would be useful for you to see, tell me what those are.",
            PromptTemplate.Fixes => "Identify any potential bugs or issues with this code.  Explain potential fixes and show me the implementation.",
            PromptTemplate.AdditionalFunctions => "Take a look at this code and suggest a bunch of new functions or capabilities which might make it more robust.  If there are any issues with the code, tell me about that first.  Show me the implementation.",
            PromptTemplate.Documentation => "Write documentation this code following C# best practices.  Make it easy for me to copy and paste into the source code.",
            PromptTemplate.CodeReview => "Analyze this code and give me a general code review.  Help me understand where things can be improved and extended.",
            PromptTemplate.Performance => "Is this code performant for a low-compute device from Unity builds?  Are there any issues I can fix or areas that I can make improvements so that it runs efficiently and without excessive garbage collection?",
            _ => string.Empty
        };
    }
}
