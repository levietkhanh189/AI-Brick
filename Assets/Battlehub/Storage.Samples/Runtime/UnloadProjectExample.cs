using UnityEngine;
namespace Battlehub.Storage.Samples
{
    public class UnloadProjectExample : MonoBehaviour
    {
        private IAssetDatabase m_assetDatabase;

        private async void Start()
        {
            m_assetDatabase = RuntimeAssetDatabase.Instance;

            string projectPath = $"{Application.persistentDataPath}/Example Project";

            await m_assetDatabase.LoadProjectAsync(projectPath);
        }

        private async void OnDestroy()
        {
            if (m_assetDatabase != null)
            {
                if (m_assetDatabase.IsProjectLoaded)
                {
                    // unload the project and all assets

                    // destroy: true -> destroy the corresponding objects and game objects

                    await m_assetDatabase.UnloadProjectAsync(destroy: true);
                }
            }
        }
    }
}
