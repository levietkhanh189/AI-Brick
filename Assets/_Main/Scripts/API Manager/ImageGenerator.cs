using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class ImageGenerator : MonoBehaviour
{
    private ImageGenerationAPI imageAPI;

    void Start()
    {
        imageAPI = gameObject.AddComponent<ImageGenerationAPI>();
    }

    [Sirenix.OdinInspector.Button]
    public void GenerateImage(string prompt, string outputFormat = "png")
    {
        imageAPI.GenerateImage(prompt, outputFormat, OnImageGenerated, OnError);
    }

    private void OnImageGenerated(string imagePath)
    {
        Debug.Log("Hình ảnh đã được tạo và lưu tại: " + imagePath);

        // Tạo Texture2D từ tệp hình ảnh
        byte[] imageData = File.ReadAllBytes(imagePath);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(imageData);

        // Tạo một GameObject để hiển thị hình ảnh
        GameObject imageObject = new GameObject("GeneratedImage");
        SpriteRenderer renderer = imageObject.AddComponent<SpriteRenderer>();
        renderer.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

        // Hoặc bạn có thể sử dụng UI Image nếu muốn
    }

    private void OnError(string error)
    {
        Debug.LogError("Lỗi: " + error);
    }
}
