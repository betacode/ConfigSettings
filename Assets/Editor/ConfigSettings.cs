using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Collections.Generic;

public class ConfigSettings : EditorWindow
{
    // must match "name" field in package.json
    const string PACKAGE_NAME = "com.betawaves.configsettings";

    [MenuItem("Tools/Config Settings")]
    public static void ShowExample()
    {
        ConfigSettings wnd = GetWindow<ConfigSettings>();
        wnd.titleContent = new GUIContent("ConfigSettings");
    }

    public void OnEnable()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Import Style Sheet
        var ussPath = packagePath("Assets/Editor/ConfigSettings.uss");
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(ussPath);

        // Import UXML
        var uxmlPath = packagePath("Assets/Editor/ConfigSettings.uxml");
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
        VisualElement uxmlVE = visualTree.CloneTree();
        uxmlVE.styleSheets.Add(styleSheet);
        root.Add(uxmlVE);

        // Add Game Select dropdown menu
        var gameSelect = GameSelectMenu();
        root.Q<VisualElement>("GameSelectPanel").Add(gameSelect); // add it as a child of a VisualElement

        var goName = root.Q<UnityEngine.UIElements.TextField>("GameObjectName");
        goName.value = "HELP";
    }

    private PopupField<string> GameSelectMenu() {
        // select dropdown
        var setupConfig = this.GetSettingsModel();
        var choices = setupConfig.games;
        choices.Insert(0, "-- Select a game --"); // add None value

        // Create a new field with label, choices and default selection
        var gameSelect = new PopupField<string>("Game", choices, 0);
        gameSelect.RegisterValueChangedCallback((evt) => {
            Debug.Log($"{evt.previousValue} => {evt.newValue}");
        });
        // gameSelect.RegisterCallback<ChangeEvent<string>>(x => Debug.Log("inner:" + x.newValue));
        return gameSelect;
    }

    private SettingsModel GetSettingsModel() {
        var settingsFilePath = packagePath( @"Assets/Editor/ConfigSettings.json");
        if(!File.Exists(settingsFilePath)) {
            Debug.Log($"file missing: {settingsFilePath}");
        }
        string jsonString = File.ReadAllText(settingsFilePath);
        var setupConfig = SettingsModel.CreateFromJSON(jsonString);
        return setupConfig;
    }

    private string packagePath(string projectPath) {
        // check package manager version first
        var packageManagerPath = $"Packages/{PACKAGE_NAME}/{projectPath}";
        if(File.Exists(packageManagerPath)) {
            return packageManagerPath;
        }
        // fallback to local project. Used for development
        if(File.Exists(projectPath)) {
            return projectPath;
        }
        Debug.Log($"file not found at either paths: {projectPath}, {packageManagerPath}");
        return "";
    }
}
