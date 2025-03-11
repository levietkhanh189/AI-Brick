using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Brick
{
    public int x;
    public int y;
    public int z;
    public int width;
    public int length;
    public int height;
    public int[] color;
    public string name;
}

[System.Serializable]
public class LegoLayer
{
    public int layerIndex;
    public List<Brick> bricks;
}

[System.Serializable]
public class LegoModel
{
    public List<LegoLayer> layers;
}

public class LegoBuilder : MonoBehaviour
{
    public GameObject brickPrefab; // Prefab của viên gạch 1x1x1 (kích thước 1x0.4x1)

    void Start()
    {
        if (brickPrefab == null)
        {
            Debug.LogError("brickPrefab chưa được gán trong Inspector!");
            return;
        }
        LoadLegoModel();
    }

    void LoadLegoModel()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("lego_model");
        if (jsonFile == null)
        {
            Debug.LogError("Không tìm thấy file 'lego_model.json' trong Assets/Resources!");
            return;
        }
        LoadLegoModel(jsonFile.text);
    }
    [Sirenix.OdinInspector.Button]
    void LoadLegoModel(string jsonText)
    {
        LegoModel model = JsonUtility.FromJson<LegoModel>(jsonText);
        if (model == null || model.layers == null)
        {
            Debug.LogError("Không thể parse file JSON hoặc layers rỗng!");
            return;
        }

        foreach (LegoLayer layer in model.layers)
        {
            int layerY = layer.layerIndex;
            GameObject layerObject = new GameObject($"Layer_{layerY}");
            layerObject.transform.parent = transform;

            foreach (Brick brick in layer.bricks)
            {
                // Tạo parent cho brick
                GameObject brickParent = new GameObject(brick.name);
                brickParent.transform.parent = layerObject.transform;
                brickParent.transform.localPosition = new Vector3(
                    brick.x,        // X
                    brick.y * 0.4f, // Y (mỗi đơn vị y trong JSON là 1 brick 1x1x1, cao 0.4)
                    brick.z         // Z
                );

                // Tạo các cube 1x1x1 trong brick
                for (int i = 0; i < brick.width; i++)
                {
                    for (int k = 0; k < brick.length; k++)
                    {
                        for (int j = 0; j < brick.height; j++)
                        {
                            GameObject brickInstance = Instantiate(brickPrefab, brickParent.transform);
                            // Đặt vị trí khít nhau
                            brickInstance.transform.localPosition = new Vector3(
                                i,       // X
                                j * 0.4f, // Y (mỗi đơn vị là 0.4 để xếp chồng khít)
                                k        // Z
                            );
                            // Không cần thay đổi scale, giữ nguyên 1x0.4x1 từ prefab

                            Renderer renderer = brickInstance.GetComponent<Renderer>();
                            if (renderer != null)
                            {
                                renderer.material.color = new Color(brick.color[0] / 255f, brick.color[1] / 255f, brick.color[2] / 255f);
                            }
                        }
                    }
                }
            }
        }

        Debug.Log("Đã tạo mô hình LEGO trong Unity.");
    }
}