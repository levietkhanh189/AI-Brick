using System.Collections.Generic;
using UnityEngine;

namespace MVoxelizer.MVRuntime
{
    public class VoxelColorConverter : MonoBehaviour
    {
        public List<Material> materialsList; // Danh sách vật liệu đã tạo

        // Tìm vật liệu có màu gần nhất từ danh sách materialsList
        private Material FindClosestMaterial(Color targetColor)
        {
            Material closestMaterial = null;
            float minDistance = float.MaxValue;

            foreach (var material in materialsList)
            {
                Color materialColor = material.color;
                float distance = ColorDistance(targetColor, materialColor);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestMaterial = material;
                }
            }

            return closestMaterial;
        }

        // Hàm tính khoảng cách Euclidean giữa hai màu
        private float ColorDistance(Color color1, Color color2)
        {
            float rDiff = color1.r - color2.r;
            float gDiff = color1.g - color2.g;
            float bDiff = color1.b - color2.b;
            return Mathf.Sqrt(rDiff * rDiff + gDiff * gDiff + bDiff * bDiff);
        }

        [Sirenix.OdinInspector.Button]
        public void ConvertMaterials(GameObject go)
        {
            if (go != null && go.transform.childCount > 0)
            {
                MeshRenderer[] voxelRenderers = go.GetComponentsInChildren<MeshRenderer>();
                MeshFilter[] voxelMeshFilters = go.GetComponentsInChildren<MeshFilter>();
                Texture2D texture = voxelRenderers[0].sharedMaterial.GetTexture("_baseColorTexture") as Texture2D;

                for (int i = 0; i < voxelRenderers.Length; i++)
                {
                    MeshRenderer renderer = voxelRenderers[i];
                    MeshFilter meshFilter = voxelMeshFilters[i];

                    if (meshFilter != null && renderer.sharedMaterial != null)
                    {
                        if (texture != null)
                        {
                            Mesh mesh = meshFilter.sharedMesh;
                            Vector2[] uv = mesh.uv;

                            // Giả sử chúng ta lấy UV đầu tiên để đơn giản (bạn có thể điều chỉnh logic này)
                            if (uv.Length > 0)
                            {
                                Vector2 uvCoord = uv[0];
                                Color currentColor = texture.GetPixelBilinear(uvCoord.x, uvCoord.y);

                                // Tìm vật liệu có màu gần nhất từ danh sách
                                Material closestMaterial = FindClosestMaterial(currentColor);
                                if (closestMaterial != null)
                                {
                                    renderer.sharedMaterial = closestMaterial;
                                }
                            }
                        }
                    }
                }
            }

        }
    }
}
