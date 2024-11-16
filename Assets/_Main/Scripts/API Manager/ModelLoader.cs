using UnityEngine;
using System.IO;
using System;

public class ModelLoader : MonoBehaviour
{
    private ModelGenerationAPI modelAPI;

    void Start()
    {
        modelAPI = gameObject.AddComponent<ModelGenerationAPI>();
    }

    [Sirenix.OdinInspector.Button]
    public void GenerateModel(string imagePath)
    {
        if (File.Exists(imagePath))
        {
            byte[] imageData = File.ReadAllBytes(imagePath);
            modelAPI.Generate3DModel(imageData, OnModelGenerated, OnError);
        }
        else
        {
            Debug.LogError("Không tìm thấy tệp hình ảnh: " + imagePath);
        }
    }

    private void OnModelGenerated(GameObject model)
    {
        Debug.Log("Mô hình 3D đã được tạo và hiển thị trong scene.");

        // Bạn có thể di chuyển hoặc gán các thuộc tính cho mô hình tại đây
        model.transform.position = Vector3.zero;
    }

    private void OnError(string error)
    {
        Debug.LogError("Lỗi: " + error);
    }
}
