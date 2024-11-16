using Battlehub.Storage.EditorAttributes;
using UnityEngine;
namespace Battlehub.Storage.Samples
{
    public class CreateAssetExample : MonoBehaviour
    {
        private IAssetDatabase m_assetDatabase;
        private IThumbnailUtil m_thumbnailUtil;

        [Layer]
        public LayerMask ThumbnailLayer;

        private void Awake()
        {
            var thumbnailUtil = gameObject.AddComponent<ThumbnailUtil>();
            thumbnailUtil.ThumbnailLayer = ThumbnailLayer;
            m_thumbnailUtil = thumbnailUtil;
        }

        private async void Start()
        {
            m_assetDatabase = RuntimeAssetDatabase.Instance;

            string projectPath = $"{Application.persistentDataPath}/Example Project";
            await m_assetDatabase.LoadProjectAsync(projectPath);
            var rootID = m_assetDatabase.RootID;

            GameObject capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            // ----------------------------------------------------------------------
            // Asset can be created with or without thumbnail.
            // To generate thumbnail data you can use ThumbnailUtil.
            // ----------------------------------------------------------------------
            var thumbnailTexture = await m_thumbnailUtil.CreateThumbnailAsync(capsule);
            var thumbnailBytes = await m_thumbnailUtil.EncodeToPngAsync(thumbnailTexture);
            
            var fileID = m_assetDatabase.GetUniqueFileID(rootID, $"Capsule");
            await m_assetDatabase.CreateAssetAsync(capsule, fileID, thumbnailBytes);
        }
    }
}
