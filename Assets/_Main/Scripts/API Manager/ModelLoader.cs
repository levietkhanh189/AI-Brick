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
    public void GenerateModel(string imagePath,Action<GameObject> action)
    {
        if (File.Exists(imagePath))
        {
            byte[] imageData = File.ReadAllBytes(imagePath);
            modelAPI.Generate3DModel(imageData,(GameObject model) => { OnModelGenerated(model, action); } , OnError);
        }
        else
        {
            Debug.LogError("Không tìm thấy tệp hình ảnh: " + imagePath);
        }
    }

    private void OnModelGenerated(GameObject model,Action<GameObject> action)
    {
        Debug.Log("Mô hình 3D đã được tạo và hiển thị trong scene.");

        model.transform.position = Vector3.zero;

        GameObject child = model.transform.GetChild(0).GetChild(0).gameObject;
        child.transform.parent = MainController.Instance.transform;
        child.transform.position = Vector3.zero;
        action(child);
    }

    private void OnError(string error)
    {
        Debug.LogError("Lỗi: " + error);
    }
}
