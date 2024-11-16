using UnityEngine;
namespace Battlehub.Storage.Samples
{
    /// <summary>
    /// -----------------------------------------------------------------------------
    /// First register an external asset loader. This should only be done once, 
    /// after that you can import multiple asses using this loader.
    /// -----------------------------------------------------------------------------
    /// In this example, I'm using the built-in ResourcesLoader for simplicity,
    /// but it could be any loader which implements the IExternalAssetLoader
    /// interface (AddressablesLoader, glTFLoader, FBXLoader, etc.)
    /// -----------------------------------------------------------------------------
    /// The resource loader in this example loads an asset from the Resources folder.
    /// In this particular example, the asset with the key "Hellephant" is in
    /// Assets/Battlehub/Storage.Samples.ProjectBrowser/Content/Resources
    /// -----------------------------------------------------------------------------
    /// </summary>
    public class ImportExternalAssetExample : MonoBehaviour
    {
        private IAssetDatabase m_assetDatabase;

        private async void Start()
        {
            m_assetDatabase = RuntimeAssetDatabase.Instance;

            string projectPath = $"{Application.persistentDataPath}/Example Project";
            await m_assetDatabase.LoadProjectAsync(projectPath);

            var rootID = m_assetDatabase.RootID;
            string key = "Hellephant";
            string loaderID = nameof(ResourcesLoader);

            IExternalAssetLoader loader = new ResourcesLoader();
            await m_assetDatabase.RegisterExternalAssetLoaderAsync(loaderID, loader);

            // convert externalAssetKey to unique file id
            var targetFileID = m_assetDatabase.GetUniqueFileID(rootID, $"{key}");

            // import external asset
            await m_assetDatabase.ImportExternalAssetAsync(key, loaderID, targetFileID);

            // instantiate imported asset
            await m_assetDatabase.InstantiateAssetAsync(targetFileID);
        }
    }
}
