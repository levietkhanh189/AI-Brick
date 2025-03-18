using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

[Serializable]
public class LegoPosition
{
    public float x;
    public float y;
    public float z;
}

[Serializable]
public class LegoSize
{
    public float width;
    public float height;
    public float length;
}

[Serializable]
public class LegoColor
{
    public float r;
    public float g;
    public float b;
}

[Serializable]
public class LegoBrick
{
    public LegoPosition position;
    public LegoSize size;
    public LegoColor color;
    public int layer;
    public string name;  // Thêm trường name để phù hợp với định dạng mới
}

[Serializable]
public class BrickType
{
    public float width;
    public float length;
    public float height;  // Thêm trường height cho BrickType
}

[Serializable]
public class LayerBrick
{
    public float x;
    public float y;
    public float width;
    public float length;
    public float height;  // Thêm trường height cho LayerBrick
    public LegoColor color;
}

[Serializable]
public class Layer
{
    public int layerIndex;  // Đổi tên từ layer thành layerIndex để khớp với voxel.py
    public LayerBrick[] bricks;
}

[Serializable]
public class LegoMetadata
{
    public BrickType[] brickTypes;
    public int layerCount;
}

[Serializable]
public class LegoModel
{
    public LegoBrick[] bricks;
    public Layer[] layers;
    public LegoMetadata metadata;
}

public class LegoBuilder : MonoBehaviour
{
    [Header("Lego Settings")]
    [SerializeField] private GameObject legoBasePrefab; // Prefab cơ bản của khối Lego (1x1)
    [SerializeField] private string legoModelPath = "lego_model"; // Đường dẫn tới file JSON (không cần .json)
    [SerializeField] private float scaleFactor = 1.0f; // Hệ số tỷ lệ

    [Header("Layer Building")]
    [SerializeField] private bool buildByLayer = false; // Xây dựng theo từng layer
    [SerializeField] private int currentLayer = 0; // Layer hiện tại đang xây
    [SerializeField] private Color highlightColor = Color.yellow; // Màu làm nổi bật layer hiện tại

    private LegoModel legoModel;
    private Dictionary<string, GameObject> brickObjects = new Dictionary<string, GameObject>();

    void Start()
    {
        LoadLegoModel();
        if (!buildByLayer)
        {
            BuildCompleteModel();
        }
        else
        {
            BuildCurrentLayer();
        }
    }

    private void LoadLegoModel()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(legoModelPath);

        if (jsonFile != null)
        {
            legoModel = JsonUtility.FromJson<LegoModel>(jsonFile.text);
            Debug.Log($"Đã tải mô hình LEGO với {legoModel.bricks.Length} viên gạch và {legoModel.layers.Length} layer.");
        }
        else
        {
            Debug.LogError($"Không thể tìm thấy file JSON tại Resources/{legoModelPath}.json");
        }
    }

    private void BuildCompleteModel()
    {
        if (legoModel == null || legoBasePrefab == null)
        {
            Debug.LogError("Mô hình LEGO hoặc prefab cơ bản không được cấu hình đúng!");
            return;
        }

        ClearExistingModel();
        Transform modelParent = new GameObject("LegoModel").transform;
        modelParent.SetParent(transform);

        foreach (LegoBrick brick in legoModel.bricks)
        {
            // Tạo game object cho viên gạch
            GameObject brickObject = CreateBrick(brick, modelParent);
            // Lưu vào dictionary để truy cập sau này
            string brickKey = $"{brick.position.x}_{brick.position.y}_{brick.position.z}";
            brickObjects[brickKey] = brickObject;
        }

        Debug.Log("Hoàn thành xây dựng mô hình LEGO.");
    }

    // Xây dựng layer hiện tại
    public void BuildCurrentLayer()
    {
        if (legoModel == null || legoBasePrefab == null)
        {
            Debug.LogError("Mô hình LEGO hoặc prefab cơ bản không được cấu hình đúng!");
            return;
        }

        ClearExistingModel();
        Transform modelParent = new GameObject("LegoModel").transform;
        modelParent.SetParent(transform);

        // Xây dựng các layer đã hoàn thành
        for (int i = 0; i < currentLayer; i++)
        {
            BuildLayer(i, modelParent, false);
        }

        // Xây dựng layer hiện tại với highlight
        BuildLayer(currentLayer, modelParent, true);

        Debug.Log($"Hoàn thành xây dựng đến layer {currentLayer}.");
    }

    // Xây dựng một layer cụ thể
    private void BuildLayer(int layerIndex, Transform parent, bool highlight)
    {
        if (layerIndex < 0 || layerIndex >= legoModel.layers.Length)
        {
            Debug.LogWarning($"Layer {layerIndex} không tồn tại trong mô hình.");
            return;
        }

        Layer layer = null;

        // Tìm layer theo layerIndex
        foreach (Layer l in legoModel.layers)
        {
            if (l.layerIndex == layerIndex)
            {
                layer = l;
                break;
            }
        }

        if (layer == null)
        {
            Debug.LogWarning($"Không tìm thấy layer với layerIndex = {layerIndex}");
            return;
        }

        Transform layerParent = new GameObject($"Layer_{layerIndex}").transform;
        layerParent.SetParent(parent);

        foreach (LayerBrick layerBrick in layer.bricks)
        {
            // Tạo parent cho viên gạch
            GameObject brickParent = new GameObject($"Brick_{layerBrick.width}x{layerBrick.length}x{layerBrick.height}");
            brickParent.transform.SetParent(layerParent);

            // Đặt vị trí cho parent
            brickParent.transform.localPosition = new Vector3(
                layerBrick.x * scaleFactor,
                layerIndex * 0.4f * scaleFactor, // Chiều cao dựa vào số layer * chiều cao thực của một viên gạch
                layerBrick.y * scaleFactor
            );

            // Tạo các viên gạch 1x1 để tạo thành viên gạch lớn
            for (int x = 0; x < layerBrick.width; x++)
            {
                for (int z = 0; z < layerBrick.length; z++)
                {
                    GameObject unitBrick = Instantiate(legoBasePrefab, brickParent.transform);
                    unitBrick.name = "Unit_1x1";

                    // Đặt vị trí tương đối trong parent
                    unitBrick.transform.localPosition = new Vector3(
                        x * scaleFactor,
                        0,
                        z * scaleFactor
                    );

                    // Kích thước của gạch đơn vị là 1x0.4x1, điều chỉnh chiều cao dựa trên height
                    // Chiều cao tiêu chuẩn của một viên gạch là 0.4f, nhân với height để có chiều cao đúng
                    unitBrick.transform.localScale = new Vector3(
                        1 * scaleFactor,
                        layerBrick.height * 0.4f * scaleFactor,
                        1 * scaleFactor
                    );

                    // Đặt màu sắc
                    Renderer renderer = unitBrick.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        if (highlight)
                        {
                            // Làm nổi bật layer hiện tại
                            renderer.material.color = highlightColor;
                        }
                        else
                        {
                            renderer.material.color = new Color(
                                layerBrick.color.r,
                                layerBrick.color.g,
                                layerBrick.color.b
                            );
                        }
                    }
                }
            }

            // Lưu vào dictionary
            string brickKey = $"{layerBrick.x}_{layerIndex}_{layerBrick.y}";
            brickObjects[brickKey] = brickParent;
        }
    }

    private GameObject CreateBrick(LegoBrick brick, Transform parent)
    {
        // Tạo parent cho viên gạch lớn
        GameObject brickParent = new GameObject(brick.name ?? $"Brick_{brick.size.width}x{brick.size.length}x{brick.size.height}");
        brickParent.transform.SetParent(parent);
        brickParent.transform.localPosition = new Vector3(
            brick.position.x * scaleFactor,
            brick.position.y * scaleFactor,
            brick.position.z * scaleFactor
        );

        // Tạo các viên gạch 1x1 để tạo thành viên gạch lớn
        for (int x = 0; x < brick.size.width; x++)
        {
            for (int z = 0; z < brick.size.length; z++)
            {
                GameObject unitBrick = Instantiate(legoBasePrefab, brickParent.transform);
                unitBrick.name = "Unit_1x1";

                // Đặt vị trí tương đối trong parent
                unitBrick.transform.localPosition = new Vector3(
                    x * scaleFactor,
                    0,
                    z * scaleFactor
                );

                // Điều chỉnh chiều cao của gạch dựa trên chiều cao trong dữ liệu
                unitBrick.transform.localScale = new Vector3(
                    1 * scaleFactor,
                    brick.size.height * 0.4f * scaleFactor,
                    1 * scaleFactor
                );

                // Đặt màu sắc
                Renderer renderer = unitBrick.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = new Color(
                        brick.color.r,
                        brick.color.g,
                        brick.color.b
                    );
                }
            }
        }

        return brickParent;
    }

    // Xóa mô hình hiện tại
    private void ClearExistingModel()
    {
        Transform existingModel = transform.Find("LegoModel");
        if (existingModel != null)
        {
            DestroyImmediate(existingModel.gameObject);
        }
        brickObjects.Clear();
    }

    // Phương thức để chuyển đến layer tiếp theo
    public void NextLayer()
    {
        if (currentLayer < legoModel.metadata.layerCount - 1)
        {
            currentLayer++;
            BuildCurrentLayer();
        }
    }

    // Phương thức để quay lại layer trước
    public void PreviousLayer()
    {
        if (currentLayer > 0)
        {
            currentLayer--;
            BuildCurrentLayer();
        }
    }

    // Phương thức để chuyển đổi giữa xây toàn bộ mô hình và xây theo layer
    public void ToggleBuildByLayer()
    {
        buildByLayer = !buildByLayer;
        if (buildByLayer)
        {
            BuildCurrentLayer();
        }
        else
        {
            BuildCompleteModel();
        }
    }

    // Phương thức này có thể được gọi từ Inspector để tái xây dựng mô hình
    public void RebuildModel()
    {
        // Tải lại và xây dựng mô hình mới
        LoadLegoModel();
        if (buildByLayer)
        {
            BuildCurrentLayer();
        }
        else
        {
            BuildCompleteModel();
        }
    }
}