using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class DataPaths
{
    public List<string> ImagePaths = new List<string>();
    public List<string> ModelPaths = new List<string>();
}

public class DataStorage
{
    private static DataStorage instance;
    public static DataStorage Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new DataStorage();
                instance.LoadData();
            }
            return instance;
        }
    }

    private DataPaths dataPaths = new DataPaths();
    private string dataFilePath = Path.Combine(Application.persistentDataPath, "dataPaths.json");

    public List<string> ImagePaths => dataPaths.ImagePaths;
    public List<string> ModelPaths => dataPaths.ModelPaths;

    public void AddImagePath(string path)
    {
        if (!dataPaths.ImagePaths.Contains(path))
            dataPaths.ImagePaths.Add(path);
        SaveData();
    }

    public void AddModelPath(string path)
    {
        if (!dataPaths.ModelPaths.Contains(path))
            dataPaths.ModelPaths.Add(path);
        SaveData();
    }

    private void SaveData()
    {
        string json = JsonUtility.ToJson(dataPaths, true);
        File.WriteAllText(dataFilePath, json);
    }

    private void LoadData()
    {
        if (File.Exists(dataFilePath))
        {
            string json = File.ReadAllText(dataFilePath);
            dataPaths = JsonUtility.FromJson<DataPaths>(json);
        }
    }

    public DataPaths GetDataPaths()
    {
        if (File.Exists(dataFilePath))
        {
            string json = File.ReadAllText(dataFilePath);
            return dataPaths = JsonUtility.FromJson<DataPaths>(json);
        }
        return null;
    }
}
