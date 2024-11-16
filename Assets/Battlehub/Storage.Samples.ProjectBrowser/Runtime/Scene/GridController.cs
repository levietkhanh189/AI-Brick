using UnityEngine;

namespace Battlehub.Storage.Samples
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class GridController : MonoBehaviour
    {
        [SerializeField]
        private int m_gridSizeX = 100; // Number of grid points in the X direction

        [SerializeField]
        private int m_gridSizeY = 100; // Number of grid points in the Y direction

        [SerializeField]
        private float m_cellSize = 1.0f; // Size of each grid cell

        [SerializeField]
        private Material m_gridMaterial;

        private void Start()
        {
            GenerateGridMesh();

            var renderer = GetComponent<MeshRenderer>();
            if (m_gridMaterial == null)
            {
                Debug.LogWarning("Set Grid Material");
                m_gridMaterial = new Material(Shader.Find("Unlit/Color"));
            }

            renderer.sharedMaterial = m_gridMaterial;
        }


        private void GenerateGridMesh()
        {
            Mesh mesh = new Mesh();

            float offsetX = (m_gridSizeX * m_cellSize) / 2;
            float offsetY = (m_gridSizeY * m_cellSize) / 2;

            Vector3[] vertices = new Vector3[(m_gridSizeX + 1) * 2 + (m_gridSizeY + 1) * 2];
            int[] indices = new int[(m_gridSizeX + 1) * 2 + (m_gridSizeY + 1) * 2];

            for (int x = 0; x <= m_gridSizeX; x++)
            {
                vertices[x * 2] = new Vector3(x * m_cellSize - offsetX, 0, -offsetY);
                vertices[x * 2 + 1] = new Vector3(x * m_cellSize - offsetX, 0, m_gridSizeY * m_cellSize - offsetY);

                indices[x * 2] = x * 2;
                indices[x * 2 + 1] = x * 2 + 1;
            }

            for (int y = 0; y <= m_gridSizeY; y++)
            {
                vertices[(m_gridSizeX + 1) * 2 + y * 2] = new Vector3(-offsetX, 0, y * m_cellSize - offsetY);
                vertices[(m_gridSizeX + 1) * 2 + y * 2 + 1] = new Vector3(m_gridSizeX * m_cellSize - offsetX, 0, y * m_cellSize - offsetY);

                indices[(m_gridSizeX + 1) * 2 + y * 2] = (m_gridSizeX + 1) * 2 + y * 2;
                indices[(m_gridSizeX + 1) * 2 + y * 2 + 1] = (m_gridSizeX + 1) * 2 + y * 2 + 1;
            }

            mesh.vertices = vertices;
            mesh.SetIndices(indices, MeshTopology.Lines, 0);

            // Assign the mesh to the MeshFilter component
            MeshFilter meshFilter = GetComponent<MeshFilter>();
            meshFilter.mesh = mesh;
        }
    }
}