using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

public class ModelGenerationAPI : MonoBehaviour
{
    private RuntimeImportBehaviour importer;
    /// <summary>
    /// Gửi yêu cầu tới API để tạo mô hình 3D từ hình ảnh.
    /// </summary>
    /// <param name="imageData">Mảng byte của tệp hình ảnh.</param>
    /// <param name="onSuccess">Callback khi thành công, trả về GameObject của mô hình 3D.</param>
    /// <param name="onError">Callback khi có lỗi, trả về thông báo lỗi.</param>
    public void Generate3DModel(byte[] imageData, Action<GameObject> onSuccess, Action<string> onError)
    {
        StartCoroutine(Generate3DModelCoroutine(imageData, onSuccess, onError));
    }

    private IEnumerator Generate3DModelCoroutine(byte[] imageData, Action<GameObject> onSuccess, Action<string> onError)
    {
        // Tạo form data
        WWWForm form = new WWWForm();
        form.AddBinaryData("image", imageData, "image.png", "image/png");

        // Tạo yêu cầu UnityWebRequest
        using (UnityWebRequest request = UnityWebRequest.Post(ApiManager.ModelGenerationUrl, form))
        {
            request.SetRequestHeader("authorization", $"Bearer {ApiManager.Get()}");

            // Gửi yêu cầu và chờ phản hồi
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                // Lưu dữ liệu GLB vào persistentDataPath
                byte[] modelData = request.downloadHandler.data;
                string fileName = $"model_{DateTime.Now.Ticks}.glb";
                string filePath = Path.Combine(Application.persistentDataPath, fileName);
                File.WriteAllBytes(filePath, modelData);

                // Lưu đường dẫn vào DataStorage
                DataStorage.Instance.AddModelPath(filePath);

                // Sử dụng Piglet để tải mô hình GLB
                LoadModelWithPiglet(filePath, onSuccess, onError);
            }
            else
            {
                string errorMessage = request.downloadHandler.text;
                onError?.Invoke($"Lỗi: {request.error}\n{errorMessage}");
            }
        }
    }

    private void LoadModelWithPiglet(string glbFilePath, Action<GameObject> onSuccess, Action<string> onError)
    {
        // Sử dụng Piglet để tải mô hình GLB
        if(importer == null)
            importer = gameObject.AddComponent<RuntimeImportBehaviour>();
        importer.ImportModel(glbFilePath, onSuccess, onError);
    }
}
