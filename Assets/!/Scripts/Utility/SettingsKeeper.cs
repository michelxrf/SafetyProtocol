using UnityEngine;

/// <summary>
/// Used to hold game data during play
/// </summary>
public class SettingsKeeper: MonoBehaviour
{
    public static SettingsKeeper Instance { get; private set; }

    public string dificultyLevel { get; private set; }
    public string classRoomName {get; private set; }
    public string gameMap { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void HoldLevelData(string _dificultyLevel, string _classRoomName, string _gameMap)
    {
        dificultyLevel = _dificultyLevel;
        classRoomName = _classRoomName;
        gameMap = _gameMap;
    }

}
