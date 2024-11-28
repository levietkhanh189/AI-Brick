using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct FileId
{
    public string id;
    public string description;

    public FileId(string id, string description)
    {
        this.id = id;
        this.description = description;
    }
}

[CreateAssetMenu(fileName = "FileIdsData", menuName = "Data/FileIdsData", order = 1)]
public class FileIdsData : ScriptableObject
{
    // Danh sách các FileId
    public List<FileId> fileIds = new List<FileId>();

    // Phương thức lưu dữ liệu dưới dạng JSON vào PlayerPrefs
    [Sirenix.OdinInspector.Button]
    public void SaveData(string key)
    {
        // Chuyển đối tượng thành chuỗi JSON
        string json = JsonUtility.ToJson(this, true);

        // Lưu chuỗi JSON vào PlayerPrefs với key truyền vào
        PlayerPrefs.SetString(key, json);
        PlayerPrefs.Save();
    }

    // Phương thức tải dữ liệu từ PlayerPrefs
    [Sirenix.OdinInspector.Button]
    public void LoadData(string key)
    {
        if (PlayerPrefs.HasKey(key))
        {
            // Đọc dữ liệu JSON từ PlayerPrefs
            string json = PlayerPrefs.GetString(key);

            // Chuyển chuỗi JSON thành đối tượng này
            JsonUtility.FromJsonOverwrite(json, this);
        }
        else
        {
            Debug.LogWarning("PlayerPrefs key not found: " + key);
        }
    }
}
