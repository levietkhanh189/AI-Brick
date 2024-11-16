using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class MaterialCreator : MonoBehaviour
{
    // Mảng màu được định nghĩa trước
    private Color[] predefinedColors = new Color[]
    {
        Color.white,                              // Màu Trắng
        new Color(1f, 0.713f, 0.756f),            // Màu Hồng nhạt
        Color.red,                                // Màu Đỏ
        Color.yellow,                             // Màu Vàng
        new Color(0.678f, 0.847f, 0.902f),        // Màu Xanh dương Nhạt
        new Color(0.824f, 0.706f, 0.549f),        // Màu Nâu nhạt
        new Color(1f, 0.647f, 0f),                // Màu Cam
        new Color(0f, 0f, 0.545f),                // Màu Xanh Dương Đậm
        new Color(0.502f, 0f, 0.502f),            // Màu Tím
        new Color(0f, 0.392f, 0f),                // Màu Xanh Đậm
        new Color(0.867f, 0.627f, 0.867f),        // Màu Mận
        Color.gray,                               // Màu Xám nhạt
        new Color(0.251f, 0.878f, 0.816f),        // Màu Xanh ngọc
        Color.black,                              // Màu Đen
        new Color(0.596f, 0.984f, 0.596f),        // Màu xanh lá nhạt
        new Color(0f, 0f, 0f, 0f),                // Trong suốt
        new Color(1f, 0.894f, 0.768f),            // Màu da
        new Color(0.545f, 0.271f, 0.075f),        // Màu Nâu đậm
        new Color(0.729f, 0.333f, 0.827f),        // Màu Tím nhạt
        new Color(0.416f, 0.353f, 0.804f),        // Màu Tím đậm
        new Color(1f, 0.078f, 0.576f),            // Màu Hồng đậm
        new Color(0.855f, 0.647f, 0.125f)         // Vàng sậm
    };

    // Danh sách để lưu các Material
    public List<Material> materialsList = new List<Material>();

    // Tạo và lưu các Material
    [Sirenix.OdinInspector.Button]
    public void Create()
    {
        for (int i = 0; i < predefinedColors.Length; i++)
        {
            // Tạo một material mới
            Material newMaterial = new Material(Shader.Find("Standard"));
            newMaterial.color = predefinedColors[i];

            // Đặt tên cho Material
            newMaterial.name = "Material_Color_" + i;

            // Lưu Material vào thư mục "Assets/Materials"
            string path = "Assets/_Main/Materials/" + newMaterial.name + ".mat";
            AssetDatabase.CreateAsset(newMaterial, path);

            // Thêm Material vào danh sách
            materialsList.Add(newMaterial);

            // Log ra console để xác nhận
            Debug.Log("Saved Material: " + newMaterial.name + " at " + path);
        }

        // Lưu thay đổi vào AssetDatabase
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
