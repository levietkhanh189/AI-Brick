using Battlehub.Storage.EditorAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace Battlehub.Storage.Samples
{
    public class CreateAndSaveThumbnailExample : MonoBehaviour
    {
        private IAssetDatabase m_assetDatabase;
        private IThumbnailUtil m_thumbnailUtil;

        [Layer]
        public LayerMask ThumbnailLayer;

        [SerializeField]
        private RawImage m_thumbnailImage;

        private void Awake()
        {
            var thumbnailUtil = new GameObject("ThumbnailUtil").AddComponent<ThumbnailUtil>();
           
            // rotate thumbnail camera
            thumbnailUtil.transform.LookAt(-Vector3.one); 

            // set thumbnail camera layer
            thumbnailUtil.ThumbnailLayer = ThumbnailLayer;

            // set desired thumbnail res
            thumbnailUtil.SnapshotTextureWidth = 512;
            thumbnailUtil.SnapshotTextureHeight = 512;

            m_thumbnailUtil = thumbnailUtil;

            if (m_thumbnailImage == null)
            {
                Debug.LogWarning("Set thumbnail image");
            }
        }

        private async void Start()
        {
            m_assetDatabase = RuntimeAssetDatabase.Instance;

            string projectPath = $"{Application.persistentDataPath}/Example Project";
            await m_assetDatabase.LoadProjectAsync(projectPath);
            var rootID = m_assetDatabase.RootID;

            GameObject capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            var fileID = m_assetDatabase.GetUniqueFileID(rootID, $"Capsule");
            
            // Create Asset (capsule becomes instance of an asset)
            await m_assetDatabase.CreateAssetAsync(capsule, fileID);

            // Create, encode and save thumbnail
            var thumbnailTexture = await m_thumbnailUtil.CreateThumbnailAsync(capsule);

            // Release asset instance
            await m_assetDatabase.ReleaseAsync(capsule);

            // Encode and save thumbnail
            var thumbnailBytes = await m_thumbnailUtil.EncodeToPngAsync(thumbnailTexture);
            await m_assetDatabase.SaveThumbnailAsync(fileID, thumbnailBytes);

            if (m_thumbnailImage != null)
            {
                // Show thumbnail texture
                m_thumbnailImage.texture = thumbnailTexture;
            }
        }
    }
}
