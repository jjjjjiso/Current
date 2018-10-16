using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

using WaterBlast.Game.Common;

namespace WaterBlast.Editor
{
    public class WaterBlastEditor : EditorWindow
    {
        private readonly List<EditorTab> tabs = new List<EditorTab>();

        private int selectedTabIndex = -1;
        private int prevSelectedTabIndex = -1;

        //에디터 윈도우의 정적 초기화.
        [MenuItem("Window/WaterBlast/Editor", false, 0)]
        private static void Init()
        {
            var window = GetWindow(typeof(WaterBlastEditor));
            window.titleContent = new GUIContent("Water Blast Editor");
        }

        private void OnEnable()
        {
            tabs.Add(new GameSettingTab(this));
            tabs.Add(new LevelEditorTab(this));
            selectedTabIndex = 0;
        }

        private void OnGUI()
        {
            selectedTabIndex = GUILayout.Toolbar(selectedTabIndex,
                new[] { "Game settings", "Level editor" });
            if (selectedTabIndex >= 0 && selectedTabIndex < tabs.Count)
            {
                var selectedEditor = tabs[selectedTabIndex];
                if (selectedTabIndex != prevSelectedTabIndex)
                {
                    selectedEditor.OnTabSelected();
                    GUI.FocusControl(null);
                }
                selectedEditor.Draw();
                prevSelectedTabIndex = selectedTabIndex;
            }
        }
    }
}