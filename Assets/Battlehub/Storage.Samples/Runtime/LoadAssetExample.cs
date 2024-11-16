using System.Linq;
using UnityEngine;
namespace Battlehub.Storage.Samples
{
    public class LoadAssetExample : MonoBehaviour
    {
        private IAssetDatabase m_assetDatabase;

        private async void Start()
        {
            m_assetDatabase = RuntimeAssetDatabase.Instance;

            string projectPath = $"{Application.persistentDataPath}/Example Project";
            await m_assetDatabase.LoadProjectAsync(projectPath);

            var rootID = m_assetDatabase.RootID;
            var fileID = m_assetDatabase.GetUniqueFileID(rootID, $"Capsule");

            // Create GameObject
            var capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);

            // Make some changes
            ModifyGameObject(capsule);

            // Create Asset
            await m_assetDatabase.CreateAssetAsync(capsule, fileID);

            // Unload All assets and free up memory
            await m_assetDatabase.UnloadAllAssetsAsync(destroy: true);

            // Load asset by fileID
            await m_assetDatabase.LoadAssetAsync(fileID);

            // Instantiate loaded asset
            await m_assetDatabase.InstantiateAssetAsync(fileID);
        }

        private static void ModifyGameObject(GameObject capsule)
        {
            var meshRenderer = capsule.GetComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = new Material(Shader.Find("Unlit/Color"));
            meshRenderer.sharedMaterial.color = Color.blue;

            var meshFilter = capsule.GetComponent<MeshFilter>();
            meshFilter.sharedMesh = meshFilter.mesh;
            meshFilter.sharedMesh.vertices = meshFilter.sharedMesh.vertices
                .Zip(meshFilter.sharedMesh.normals, (v,n) => (v, n))
                .Select(vn => vn.v + vn.n).ToArray();
        }
    }
}
