using UnityEngine;
namespace Battlehub.Storage.Samples
{
    public class CreateAssetVariantExample : MonoBehaviour
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

            var fileID = m_assetDatabase.GetUniqueFileID(rootID, $"{key}");
            var variantFileID = m_assetDatabase.GetUniqueFileID(rootID, $"{key} Variant");

            // Import external asset
            await m_assetDatabase.ImportExternalAssetAsync(key, loaderID, fileID);

            // Instantiate external asset
            var hellephantVar = 
                await m_assetDatabase.InstantiateAssetAsync<GameObject>(fileID);
            
            // Modify its materials
            var renderer = hellephantVar.GetComponentInChildren<SkinnedMeshRenderer>();
            ModifyMaterials(renderer);

            // Mark the rendering component as "dirty".
            // This will let the CreateAssetAsync method know that this component
            // has changed and should be stored in the data file, thus creating a variant
            // of the asset that differs from the base only in that component
            await m_assetDatabase.SetDirtyAsync(renderer);
            await m_assetDatabase.CreateAssetAsync(hellephantVar, variantFileID);

            // Instantiate base asset
            var hellephant = 
                await m_assetDatabase.InstantiateAssetAsync<GameObject>(fileID);
            hellephant.transform.position = Vector3.right * 3;
            
            // Instantiate asset variant
            var hellephantVariant2 = 
                await m_assetDatabase.InstantiateAssetAsync<GameObject>(variantFileID);
            hellephantVariant2.transform.position = Vector3.left * 3;
        }

        private static void ModifyMaterials(SkinnedMeshRenderer renderer)
        {
            var materials = renderer.materials;
            materials[0].SetColor("_EmissionColor", Color.green);
            materials[1].SetColor("_EmissionColor", Color.red);
            renderer.sharedMaterials = materials;
        }
    }
}
