using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

public class ImageGenerationAPI : MonoBehaviour
{
    /// <summary>
    /// Gửi yêu cầu tới API để tạo hình ảnh từ prompt.
    /// </summary>
    /// <param name="prompt">Prompt mô tả hình ảnh.</param>
    /// <param name="outputFormat">Định dạng hình ảnh đầu ra (ví dụ: "webp").</param>
    /// <param name="onSuccess">Callback khi thành công, trả về đường dẫn tới tệp hình ảnh.</param>
    /// <param name="onError">Callback khi có lỗi, trả về thông báo lỗi.</param>
    public void GenerateImage(string prompt, string outputFormat, Action<string> onSuccess, Action<string> onError)
    {
        StartCoroutine(GenerateImageCoroutine(prompt, outputFormat, onSuccess, onError));
    }

    private IEnumerator GenerateImageCoroutine(string prompt, string outputFormat, Action<string> onSuccess, Action<string> onError)
    {
        // Tạo form data
        WWWForm form = new WWWForm();
        form.AddField("prompt", prompt);
        form.AddField("output_format", outputFormat);

        // Thêm tệp trống vào "files" với key "none"
        byte[] emptyData = new byte[0];
        form.AddBinaryData("none", emptyData, "", "application/octet-stream");

        // Tạo yêu cầu UnityWebRequest
        using (UnityWebRequest request = UnityWebRequest.Post(ApiManager.ImageGenerationUrl, form))
        {
            request.SetRequestHeader("authorization", $"Bearer {ApiManager.Get()}");
            request.SetRequestHeader("accept", "image/*");

            // Gửi yêu cầu và chờ phản hồi
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                // Lưu hình ảnh vào persistentDataPath
                byte[] imageData = request.downloadHandler.data;
                string fileName = $"image_{DateTime.Now.Ticks}.{outputFormat}";
                string filePath = Path.Combine(Application.persistentDataPath, fileName);
                File.WriteAllBytes(filePath, imageData);

                // Lưu đường dẫn vào DataStorage
                DataStorage.Instance.AddImagePath(filePath);

                onSuccess?.Invoke(filePath);
            }
            else
            {
                string errorMessage = request.downloadHandler.text;
                onError?.Invoke($"Lỗi: {request.error}\n{errorMessage}");
            }
        }
    }
}
