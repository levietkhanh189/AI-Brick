using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

namespace Battlehub.Storage.Samples
{
    /// <summary>
    /// Ví dụ về cách sử dụng AssetManager trong thực tế.
    /// </summary>
    public class AssetUsageExample : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        [Tooltip("Tham chiếu đến AssetManager.")]
        private AssetManager assetManager;
        public Image image;

        private async void Start()
        {
            if (assetManager == null)
            {
                Debug.LogError("AssetManager chưa được gán. Vui lòng gán AssetManager vào AssetUsageExample.");
                return;
            }

            // Khởi tạo cơ sở dữ liệu asset
            await assetManager.InitializeAssetDatabaseAsync();
        }

        [Sirenix.OdinInspector.Button]
        public async void CreateAsset(string name,GameObject gameObject)
        {
            string fileId = await assetManager.SaveAssetAsync(name, gameObject);

            Debug.Log("Save with file id : " + fileId);
        }

        [Sirenix.OdinInspector.Button]
        public async Task<GameObject> LoadAsset(string name)
        {
            return await assetManager.LoadAssetAsync(name);
        }

        [Sirenix.OdinInspector.Button]
        public async Task<Sprite> LoadThumb(string name)
        {
            Sprite sprite = await assetManager.LoadThumbnailAsync(name);
            image.sprite = sprite;
            return sprite;
        }
    }
}
