using System.Collections.Generic;
using UnityEngine;

public class FileManager : MonoBehaviour
{
    public static FileManager Instance;
    public FileIdsData fileIdsData;  // Tham chiếu tới ScriptableObject
    private string playerPrefsKey = "FileIdsDataKey";  // Key cho PlayerPrefs

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        fileIdsData.LoadData(playerPrefsKey);

        foreach (var fileId in fileIdsData.fileIds)
        {
            Debug.Log($"ID: {fileId.id}, Description: {fileId.description}");
        }
    }

    public List<FileId> GetFileIds()
    {
        return fileIdsData.fileIds;
    }

    [Sirenix.OdinInspector.Button]
    void Test()
    {
        string newId = "NewFileId_" + Random.Range(1, 100);
        string newDescription = "Description for " + newId;
        fileIdsData.fileIds.Add(new FileId(newId, newDescription));

        // Lưu dữ liệu vào PlayerPrefs
        fileIdsData.SaveData(playerPrefsKey);
    }

    public void NewFile(string name)
    {
        fileIdsData.fileIds.Add(new FileId(name, ""));
        // Lưu dữ liệu vào PlayerPrefs
        fileIdsData.SaveData(playerPrefsKey);

    }
}
