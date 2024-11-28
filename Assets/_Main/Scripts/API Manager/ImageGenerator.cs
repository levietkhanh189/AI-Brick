using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

public class ImageGenerator : MonoBehaviour
{
    private ImageGenerationAPI imageAPI;

    void Start()
    {
        imageAPI = gameObject.AddComponent<ImageGenerationAPI>();
    }

    [Sirenix.OdinInspector.Button]
    public void GenerateImage(string prompt, Action<string, Texture2D> action = null,string outputFormat = "png")
    {
        imageAPI.GenerateImage(prompt, outputFormat,(string s)=> { OnImageGenerated(s, action); } , OnError);
    }

    private void OnImageGenerated(string imagePath, Action<string,Texture2D> action)
    {
        Debug.Log("Hình ảnh đã được tạo và lưu tại: " + imagePath);

        // Tạo Texture2D từ tệp hình ảnh
        byte[] imageData = File.ReadAllBytes(imagePath);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(imageData);
        action(imagePath, texture);
        //GameObject imageObject = new GameObject("GeneratedImage");
        //SpriteRenderer renderer = imageObject.AddComponent<SpriteRenderer>();
        //renderer.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        //action(Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero));
    }

    private void OnError(string error)
    {
        Debug.LogError("Lỗi: " + error);
    }
}
