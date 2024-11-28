using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using System;

namespace Battlehub.Storage.Brick
{

    public class AssetUsage : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        [Tooltip("Tham chiếu đến AssetManager.")]
        private AssetManager assetManager;
        public static AssetUsage Instance;

        private void Awake()
        {
            Instance = this;
        }

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
        public async void CreateAsset(string name, GameObject gameObject,Action<bool,string> action)
        {
            try
            {
                string fileId = await assetManager.SaveAssetAsync(name, gameObject);
                if (action != null)
                    action(true, name);
                Debug.Log("Save with file id : " + fileId);
            }
            catch (Exception ex)
            {
                action(false, null);
            }
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
            return sprite;
        }

        public async void LoadAsset(string name, Action<bool, GameObject> action)
        {
            try
            {
                GameObject gameObject = await assetManager.LoadAssetAsync(name);
                if (action != null)
                    action(true, gameObject);
            }
            catch (Exception ex)
            {
                action(false, null);
            }
        }

        public void Release(GameObject myAsset)
        {
            // Giải phóng instance của asset
            assetManager.ReleaseAsync(myAsset);
        }

        public async void LoadThumb(string name, Action<bool, Sprite> action)
        {
            try
            {
                Sprite sprite = await assetManager.LoadThumbnailAsync(name);
                if (action != null)
                    action(true, sprite);
            }
            catch (Exception ex)
            {
                action(false, null);
            }
        }
    }
}
