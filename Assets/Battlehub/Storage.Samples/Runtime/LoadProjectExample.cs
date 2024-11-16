using System;
using UnityEngine;
namespace Battlehub.Storage.Samples
{
    public class LoadProjectExample : MonoBehaviour
    {
        private IAssetDatabase m_assetDatabase;

        private async void Start()
        {
            m_assetDatabase = RuntimeAssetDatabase.Instance;

            string projectPath = $"{Application.persistentDataPath}/Example Project";

            // load the project (creates a project folder if it does not exist)
            await m_assetDatabase.LoadProjectAsync(projectPath);

            // get root folder id
            Guid rootID = m_assetDatabase.RootID;

            // get child id by root id
            foreach (Guid childID in m_assetDatabase.GetChildren(rootID, sortByName: true))
            {
                // get asset metadata by id
                var meta = m_assetDatabase.GetMeta(childID);

                if (m_assetDatabase.IsFolder(childID))
                {
                    Debug.Log($"Folder {meta.Name} {meta.FileID}");
                }
                else
                {
                    Debug.Log($"{meta.Name} {meta.FileID}");
                }
            }
        }
    }
}


