using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace Battlehub.Storage.Samples
{
    /// <summary>
    /// -----------------------------------------------------------------------------
    /// Sometimes you don't want certain assets to appear in your runtime project
    /// managed by the RuntimeAssetDatabase (or you can't serialize/deserialize them), 
    /// but you still want other assets to be able to reference them.
    /// -----------------------------------------------------------------------------
    /// A good example of an external asset is the default materials or meshes that
    /// exist in your Unity editor project.
    /// -----------------------------------------------------------------------------
    /// The RegisterExternalAssetsAsync method solves this problem.
    /// -----------------------------------------------------------------------------
    /// You should generate some guids to use them as an external asset identifiers
    /// https://guidgenerator.com/
    /// -----------------------------------------------------------------------------
    /// Assets passed to RegisterExternalAssetsAsync are never stored in data files 
    /// and do not have a corresponding metadata file in the runtime project.
    /// -----------------------------------------------------------------------------
    /// </summary>
    public class RegisterExternalAssetExample : MonoBehaviour
    {
        private IAssetDatabase m_assetDatabase;

        private async void Start()
        {
            m_assetDatabase = RuntimeAssetDatabase.Instance;

            string projectPath = $"{Application.persistentDataPath}/Example Project";
            await m_assetDatabase.LoadProjectAsync(projectPath);
            var rootID = m_assetDatabase.RootID;

            GameObject capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            Material material = capsule.GetComponent<MeshRenderer>().sharedMaterial;
            Mesh mesh = capsule.GetComponent<MeshFilter>().sharedMesh;

            /// https://guidgenerator.com/
            var externalAssets = new Dictionary<Guid, object>()
            {
                {  new Guid("c872b08a-8b5e-41df-bf89-3522b8219dd6"), material },
                {  new Guid("3bad1a26-d851-49b5-a11c-6dfe74ee5341"), mesh }
            };

            // ----------------------------------------------------------------------
            // Comment out the following line and you will notice that the
            // size of the data file written to the console becomes larger.
            // This is because without registering as external assets,
            // the mesh and material are serialized into the data file.
            // ----------------------------------------------------------------------
            await m_assetDatabase.RegisterExternalAssetsAsync(externalAssets);

            var fileID = m_assetDatabase.GetUniqueFileID(rootID, $"Capsule");
            await m_assetDatabase.CreateAssetAsync(capsule, fileID);

            Debug.Log($"Size of the data file: {new FileInfo(fileID).Length} bytes");
            //Debug.Log($"Size of the data file with mesh and material: {new FileInfo(fileID).Length} bytes");
        }
    }
}
