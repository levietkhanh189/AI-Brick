using UnityEngine;

namespace Battlehub.Storage.Samples
{
    public class ModifyInstanceAndApplyChangesToBaseExample : MonoBehaviour
    {
        private IAssetDatabase m_assetDatabase;
        
        private async void Start()
        {
            m_assetDatabase = RuntimeAssetDatabase.Instance;

            string projectPath = $"{Application.persistentDataPath}/Example Project";
            await m_assetDatabase.LoadProjectAsync(projectPath);
            var rootID = m_assetDatabase.RootID;

            GameObject capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            var rend = capsule.GetComponent<MeshRenderer>();
            rend.material.color = Color.red;

            var fileID = m_assetDatabase.GetUniqueFileID(rootID, $"Capsule");
            var fileVariantID = m_assetDatabase.GetUniqueFileID(rootID, $"Capsule Variant");
            var materialFileID = m_assetDatabase.GetUniqueFileID(rootID, "Material");

            // Create an asset (the capsule becomes an instance attached to the asset)
            await m_assetDatabase.CreateAssetAsync(capsule, fileID);

            // Modify variant instance transform
            capsule.transform.Rotate(45, 0, 45);
            await m_assetDatabase.SetDirtyAsync(capsule.transform);

            // Modify variant instance renderer
            rend.sharedMaterial = Instantiate(rend.sharedMaterial);
            rend.sharedMaterial.color = Color.green;
            await m_assetDatabase.SetDirtyAsync(rend);

            // Create material asset
            await m_assetDatabase.CreateAssetAsync(rend.sharedMaterial, materialFileID);

            // Create variant of the asset 
            await m_assetDatabase.CreateAssetAsync(capsule, fileVariantID);

            GameObject instance = await m_assetDatabase.InstantiateAssetAsync<GameObject>(fileID);
            instance.transform.position = Vector3.right * 2;

            // Mark the base asset instance's transformation as dirty to prevent ApplyChangesToBase from overriding it.
            await m_assetDatabase.SetDirtyAsync(instance.transform);

            if (m_assetDatabase.CanApplyChangesToBaseAndSaveAsync(capsule))
            {
                // Propagate the changes to the base asset and save it.
                await m_assetDatabase.ApplyChangesToBaseAndSaveAsync(capsule);
            }
        }
    }
}
