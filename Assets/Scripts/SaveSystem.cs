using System;
using System.IO;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class SaveSystem : Singleton<SaveSystem>
{
    protected override bool DestroyOnLoad => false;

    [Serializable]
    public class SaveData
    {
        public bool initialised = false;

        public string name;
        public DateTime savedTime;

        public int score;
    }

    [Serializable]
    public class ConfigurationData
    {
        public float masterVolume = 0.75f;
        public float musicVolume = 1f;
    }

    [Serializable]
    public class SaveFile
    {
        public SaveData[] saveData;
        public ConfigurationData config;

        public void InitialiseSaveFile()
        {
            if(saveData == null)
            {
                saveData = new SaveData[3];

                saveData[0] = new SaveData();
                saveData[1] = new SaveData();
                saveData[2] = new SaveData();
            }

            if(config == null)
            {
                config = new ConfigurationData();
            }
        }
    }

    private SaveFile save;
    private int currentSaveData = 0;

    public void Save()
    {
        string json = JsonUtility.ToJson(save);
        File.WriteAllText(Application.persistentDataPath + $"/save.json", json);
    }

    public void Load()
    {
        string path = Application.persistentDataPath + "/save.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            save = JsonUtility.FromJson<SaveFile>(json);
            return;
        }

        save = new SaveFile();
    }

    private void Awake()
    {
        Load();
        save.InitialiseSaveFile();
    }

    public void ClearSaveData()
    {
        File.Delete(Application.persistentDataPath + $"/save.json");
    }

    public void SetSaveIndex(int value)
    {
        currentSaveData = Mathf.Clamp(value, 0, 3);
    }

    public SaveData GetSaveFile()
    {
        return save.saveData[currentSaveData];
    }

    public ConfigurationData GetConfig()
    {
        return save.config;
    }


    public void SetSaveTime()
    {
        save.saveData[currentSaveData].savedTime = DateTime.Now;
    }
}
