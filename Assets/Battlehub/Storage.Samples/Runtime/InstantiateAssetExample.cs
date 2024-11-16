using UnityEngine;
namespace Battlehub.Storage.Samples
{
    public class InstantiateAssetExample : MonoBehaviour
    {
        private IAssetDatabase m_assetDatabase;

        private async void Start()
        {
            m_assetDatabase = RuntimeAssetDatabase.Instance;

            string projectPath = $"{Application.persistentDataPath}/Example Project";
            await m_assetDatabase.LoadProjectAsync(projectPath);

            var rootID = m_assetDatabase.RootID;
            var fileID = m_assetDatabase.GetUniqueFileID(rootID, $"Cube");

            // Create GameObject
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

            // Create Asset
            await m_assetDatabase.CreateAssetAsync(cube, fileID);

            // Create 100 Asset Instances
            for (int i = 0; i < 100; ++i)
            {
                GameObject instance = await m_assetDatabase.InstantiateAssetAsync<GameObject>(fileID);
                instance.transform.position = Random.onUnitSphere * 10;
                instance.transform.rotation = Random.rotation;
            }
        }
    }
}
