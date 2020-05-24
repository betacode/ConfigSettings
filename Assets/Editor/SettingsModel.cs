using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SettingsModel
{
    public List<string> games;

    public static SettingsModel CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<SettingsModel>(jsonString);
    }

    // Given JSON input:
    // {"name":"Dr Charles","lives":3,"health":0.8}
    // this example will return a PlayerInfo object with
    // name == "Dr Charles", lives == 3, and health == 0.8f.
}
