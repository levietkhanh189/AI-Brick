using UnityEngine;

namespace Battlehub.Storage.Samples
{
    public class GettingStartedExample : MonoBehaviour
    {
        async void Start()
        {
            // Define your project path
            string projectPath = $"MyProject";

            // Obtain a reference to the asset database
            IAssetDatabase m_assetDatabase = RuntimeAssetDatabase.Instance;

            // Load the project
            await m_assetDatabase.LoadProjectAsync(projectPath);

            // Delete the "Assets" folder if it exists
            if (m_assetDatabase.Exists("Assets"))
                await m_assetDatabase.DeleteFolderAsync("Assets");

            // Create a new "Assets" folder
            await m_assetDatabase.CreateFolderAsync("Assets");

            // Create a primitive object (capsule) and make some modifications
            var go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            var filter = go.GetComponent<MeshFilter>();
            var renderer = go.GetComponent<Renderer>();

            var mesh = filter.mesh;
            var material = renderer.material;
            material.color = new Color32(0x0, 0x74, 0xFF, 0x0);

            // Create a mesh asset
            await m_assetDatabase.CreateAssetAsync(mesh, "Assets/Mesh.asset");

            // Create a material asset
            await m_assetDatabase.CreateAssetAsync(material, "Assets/Material.asset");

            // Create a "prefab" asset
            await m_assetDatabase.CreateAssetAsync(go, "Assets/Capsule.prefab");

            // Unload the project and destroy all assets to free up memory
            await m_assetDatabase.UnloadProjectAsync(destroy: true);

            // Load the project again
            await m_assetDatabase.LoadProjectAsync(projectPath);

            // Instantiate the prefab. At this point, the capsule will be visible in the scene
            await m_assetDatabase.InstantiateAssetAsync("Assets/Capsule.prefab");
        }
    }
}