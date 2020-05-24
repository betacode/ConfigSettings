using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Collections.Generic;

public class ConfigSettings : EditorWindow
{
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

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/ConfigSettings.uss");

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/ConfigSettings.uxml");
        VisualElement uxmlVE = visualTree.CloneTree();
        uxmlVE.styleSheets.Add(styleSheet);
        root.Add(uxmlVE);

        // Add Game Select dropdown menu
        var gameSelect = GameSelectMenu();
        root.Q<VisualElement>("GameSelectPanel").Add(gameSelect); // add it is a child of a VisualElement

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
        string settingsFilePath = @"Assets/Editor/ConfigSettings.json";
        if(!File.Exists(settingsFilePath)) {
            Debug.Log($"file missing: {settingsFilePath}");
        }
        string jsonString = File.ReadAllText(settingsFilePath);
        var setupConfig = SettingsModel.CreateFromJSON(jsonString);
        return setupConfig;
    }
}