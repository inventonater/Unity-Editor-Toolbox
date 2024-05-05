using System;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;


public class AiScribe : EditorWindow
{
    const string ToolName = nameof(AiScribe);

    private readonly AiScribeFileContentCache _fileContentCache = new();

    [Serializable]
    public struct Options
    {
        public bool filePathDetails;
        public bool stripComments;
        public bool includeMarkDownFiles;
        public bool projectMetadata;
        public bool selectedGameObjects;
        public bool summarizeHierarchy;
    }

    public string userPrompt = "Choose a Template or enter your custom prompt here...";
    public Options _options;
    private List<PromptTemplate> _promptTemplates = new();

    bool showHowToUse = false;
    private bool showCachedFiles = false;
    Vector2 userPromptScrollPosition;
    private Vector2 fileCacheScrollPosition;
    private static float _defaultMinWindowHeight = 320;

    [MenuItem("Tools/" + ToolName)]
    public static void ShowWindow()
    {
        ShowWindow(_defaultMinWindowHeight);
    }

    private static void ShowWindow(float minWindowHeight)
    {
        var window = GetWindow<AiScribe>(ToolName);
        window.minSize = new Vector2(340, minWindowHeight);
    }

    void OnEnable()
    {
        _promptTemplates.Clear();
        string[] guids = AssetDatabase.FindAssets("t:" + nameof(PromptTemplate), new[] { "Assets", "Packages" });
        foreach (var guid in guids)
        {
            var loadAssetAtPath = AssetDatabase.LoadAssetAtPath<PromptTemplate>(AssetDatabase.GUIDToAssetPath(guid));
            if (loadAssetAtPath == null)
            {
                Debug.LogError($"Couldn't load PromptTemplate {guid}");
                continue;
            }

            _promptTemplates.Add(loadAssetAtPath);
        }

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
            RenderOptionsSection();
        }

        RenderFileList();
        if (GUILayout.Button($"Copy Prompt to Clipboard")) CopyToClipboard();
    }

    private void RenderFileList()
    {
        using var scope = new EditorGUILayout.VerticalScope(GUI.skin.box);

        var prev = showCachedFiles;
        showCachedFiles = EditorGUILayout.Foldout(prev,
            $"Included Files ({_fileContentCache.RelativePaths.Count}), Total Word Count ({AiScribeFileContentCache.CountWords(userPrompt) + _fileContentCache.WordCount})");

        if (prev != showCachedFiles) ShowWindow(_defaultMinWindowHeight + (showCachedFiles ? 200 : 0));

        if (!showCachedFiles) return;

        fileCacheScrollPosition = EditorGUILayout.BeginScrollView(fileCacheScrollPosition, GUILayout.Height(200));
        foreach (var filePath in _fileContentCache.RelativePaths) EditorGUILayout.LabelField(filePath);
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
            EditorGUILayout.LabelField($"{ToolName} is a tool for collecting the text content of selected C# files into your clipboard for use with browser-based LLMs for code analysis and generation.", EditorStyles.wordWrappedLabel);
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

    private int _promptTemplateIndex;

    void RenderCustomPromptSection()
    {
        var selectedIndex = EditorGUILayout.Popup("Select Prompt Template", _promptTemplateIndex, _promptTemplates.Select(pt => pt.Title.TrimStart('_')).ToArray());
        if (selectedIndex != _promptTemplateIndex && _promptTemplates.Count > selectedIndex)
        {
            _promptTemplateIndex = selectedIndex;
            var promptTemplate = _promptTemplates[_promptTemplateIndex];
            userPrompt = promptTemplate.text;
            _options = promptTemplate.options;
        }

        GUIStyle textAreaStyle = new GUIStyle(GUI.skin.textArea) { wordWrap = true };
        userPromptScrollPosition = EditorGUILayout.BeginScrollView(userPromptScrollPosition);
        userPrompt = EditorGUILayout.TextArea(userPrompt, textAreaStyle, GUILayout.ExpandHeight(true));
        EditorGUILayout.EndScrollView();
    }

    void RenderOptionsSection()
    {
        using var changeScope = new EditorGUI.ChangeCheckScope();

        _options.filePathDetails = EditorGUILayout.Toggle(new GUIContent("File Path Details", "Include file details such as filename and path"), _options.filePathDetails);
        _options.stripComments = EditorGUILayout.Toggle(new GUIContent("Strip Comments", "Useful for refreshing stale comments or reducing the word count"), _options.stripComments);
        _options.includeMarkDownFiles = EditorGUILayout.Toggle(new GUIContent("Include MarkDown files", "Include any selected .md files.  Can be helpful if the README.md files are mature and robust"), _options.includeMarkDownFiles);
        _options.selectedGameObjects = EditorGUILayout.Toggle(new GUIContent("Selected GameObjects", "List the components and hierarchy path of each selected GameObjects"), _options.selectedGameObjects);
        _options.summarizeHierarchy = EditorGUILayout.Toggle(new GUIContent("Summarize Hierarchy", "Include a JSON summary of the full scene hierarchy"), _options.summarizeHierarchy);
        _options.projectMetadata = EditorGUILayout.Toggle(new GUIContent("Project Metadata", "Inject content of manifest.json and the Unity version [not recommended]"), _options.projectMetadata);

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
}