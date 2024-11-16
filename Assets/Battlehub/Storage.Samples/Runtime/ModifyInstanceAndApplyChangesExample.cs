using Battlehub.Storage.EditorAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace Battlehub.Storage.Samples
{
    public class ModifyInstanceAndApplyChangesExample : MonoBehaviour
    {
        private IAssetDatabase m_assetDatabase;
        private IThumbnailUtil m_thumbnailUtil;

        [Layer]
        public LayerMask ThumbnailLayer;

        [SerializeField]
        private RawImage m_thumbnailImage;

        private void Awake()
        {
            var thumbnailUtil = gameObject.AddComponent<ThumbnailUtil>();

            // rotate thumbnail camera
            thumbnailUtil.transform.LookAt(-Vector3.one);
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
            var rend = capsule.GetComponent<MeshRenderer>();
            rend.material.color = Color.red;

            // Create an asset (the capsule becomes an instance attached to the asset)
            var fileID = m_assetDatabase.GetUniqueFileID(rootID, $"Capsule");
            await m_assetDatabase.CreateAssetAsync(capsule, fileID);

            // Modify instance transform
            capsule.transform.Rotate(45, 0, 45);
            await m_assetDatabase.SetDirtyAsync(capsule.transform);

            // Modify instance renderer
            rend.sharedMaterial.color = Color.blue;
            await m_assetDatabase.SetDirtyAsync(rend);

            // Apply the changes to the asset and save it.
            // This method also updates the thumbnails.
            var ctx = new ThumbnailCreatorContext(m_thumbnailUtil); 
            await m_assetDatabase.ApplyChangesAndSaveAsync(capsule, ctx);

            if (m_thumbnailImage != null)
            {
                // Load thumbnail data
                await m_assetDatabase.LoadThumbnailAsync(fileID); 
                var thumbnailBytes = m_assetDatabase.GetThumbnail(fileID);

                // Load thumbnail texture
                var texture = new Texture2D(1, 1);
                texture.LoadImage(thumbnailBytes);

                m_thumbnailImage.gameObject.SetActive(true);
                m_thumbnailImage.texture = texture;
            }
        }
    }
}
