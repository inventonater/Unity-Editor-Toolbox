using UnityEditor;

namespace Toolbox.Editor
{
    public static class SelectionHistoryPreferences
    {
        private const string HistoryMaxKey = "HistoryPreferences.HistorySizePref";
        private const string HistoryShowKey = "HistoryPreferences.HistoryShowKey";

        private const int DefaultHistoryMax = 20;

        public static int HistoryMax
        {
            get => EditorPrefs.GetInt(HistoryMaxKey, DefaultHistoryMax);
            private set => EditorPrefs.SetInt(HistoryMaxKey, value);
        }

        public static bool AutoShowHistoryWindow
        {
            get => EditorPrefs.GetBool(HistoryShowKey, true);
            private set => EditorPrefs.SetBool(HistoryShowKey, value);
        }

        [SettingsProvider]
        public static SettingsProvider CreateSelectionHistorySettingsProvider()
        {
            var provider = new SettingsProvider("Preferences/Selection History", SettingsScope.User)
            {
                label = "Selection History",
                guiHandler = GUIHandler,
            };

            return provider;
        }

        private static void GUIHandler(string _)
        {
            EditorGUILayout.Space();

            EditorGUI.indentLevel++;

            EditorGUIUtility.labelWidth = 200;
            HistoryMax = EditorGUILayout.IntField("History Max", HistoryMax);
            AutoShowHistoryWindow = EditorGUILayout.Toggle("Auto Show History Window", AutoShowHistoryWindow);

            EditorGUI.indentLevel--;
        }
    }
}
