using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

// from https://gamedevbeginner.com/how-to-keep-score-in-unity-with-loading-and-saving/#save_with_xml
public class SaveManager : MonoSingleton<SaveManager>
{
    public SystemData systemData;
    public GameData gameData;
    public RuntimeData runtimeData; // for values that won't get saved to disk, but we use in-game
    private static string mPersistentDataPath;

    private void Awake()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        mPersistentDataPath = "idbfs/Pego"; 
#else
        mPersistentDataPath = Application.persistentDataPath;
#endif
        if (!Directory.Exists(mPersistentDataPath)) // webgl does not automatically make the folder
        {
            Directory.CreateDirectory(mPersistentDataPath);
        }
        systemData = LoadData<SystemData>("options.json");
        gameData = LoadData<GameData>("data.json");
        runtimeData = new();
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        SaveAll();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Autosave on scene load.
    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SaveAll();
    }

    private static string GetSaveFilePath(string fileName)
    {
        return Path.Combine(mPersistentDataPath, fileName);
    }

    private static void SaveAll()
    {
        if (Instance.systemData != null) { SaveData(Instance.systemData, "options.json"); }
        if (Instance.gameData != null) { SaveData(Instance.gameData, "data.json"); }
    }

    public LevelSave GetLevelSave(uint levelID)
    {
        LevelSave saveData;
        if (gameData.levelSave.ContainsKey(levelID))
        {
            saveData = gameData.levelSave[levelID];
        }
        else
        {
            gameData.levelSave[levelID] = saveData = new LevelSave { score = 0 };
        }
        return saveData;
    }
    public static bool DoesBackupExist()
    {
        if (File.Exists(GetSaveFilePath("backup_data.json")))
        {
            return true;
        }
        return false;
    }

    private static void SaveData<T>(T saveData, string fileName)
    {
        if (!typeof(T).IsSerializable)
        {
            throw new InvalidOperationException("A serializable type is required");
        }
        string json = JsonConvert.SerializeObject(saveData, Formatting.Indented);
        File.WriteAllText(GetSaveFilePath(fileName), json);
    }

    private static T LoadData<T>(string fileName) where T : new()
    {
        if (!typeof(T).IsSerializable)
        {
            throw new InvalidOperationException("A serializable type is required");
        }
        T data = new();
        if (File.Exists(GetSaveFilePath(fileName)))
        {
            string json = File.ReadAllText(GetSaveFilePath(fileName));
            data = JsonConvert.DeserializeObject<T>(json);
        }
        return data;
    }


}

[Serializable]
public class SystemData
{
    public float MasterVolume = 100f;
    public float SFXVolume = 50f;
    public float UIVolume = 40f;
    public float MusicVolume = 40f;
}

[Serializable]
public class GameData
{
    public int version = 1;
    public uint lastPlayedLevel = 1;
    public Dictionary<uint, LevelSave> levelSave = new();
}

[Serializable]
public class RuntimeData
{
    public string previousSceneName;
    public LevelData currentLevel;
    public bool seenTutorial = false;
}

[Serializable]
public class LevelSave
{
    public int score;
}
