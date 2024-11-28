using UnityEngine;
using UnityEngine.UI;
using Battlehub.Storage.EditorAttributes;
using System.Threading.Tasks;

namespace Battlehub.Storage.Samples
{
    /// <summary>
    /// Quản lý việc tạo, lưu, tải assets và thumbnails.
    /// </summary>
    public class AssetManager : MonoBehaviour
    {
        private IAssetDatabase assetDatabase;
        private IThumbnailUtil thumbnailUtil;

        [Header("Thumbnail Settings")]
        [Layer]
        [Tooltip("Layer cho camera chụp thumbnail.")]
        public LayerMask thumbnailLayer;

        private void Awake()
        {
            InitializeThumbnailUtil();
        }

        /// <summary>
        /// Khởi tạo cơ sở dữ liệu asset.
        /// </summary>
        public async Task InitializeAssetDatabaseAsync()
        {
            assetDatabase = RuntimeAssetDatabase.Instance;
            string projectPath = $"{Application.persistentDataPath}/MyProject";
            await assetDatabase.LoadProjectAsync(projectPath);
        }

        /// <summary>
        /// Tạo và lưu asset thực tế.
        /// </summary>
        /// <param name="assetName">Tên của asset.</param>
        /// <returns>Trả về fileID của asset.</returns>
        public async Task<string> SaveAssetAsync(string assetName, GameObject myAsset, bool isRelease = false)
        {
            if (assetDatabase == null)
            {
                Debug.LogError("AssetDatabase chưa được khởi tạo. Vui lòng gọi InitializeAssetDatabaseAsync() trước.");
                return null;
            }

            var rootID = assetDatabase.RootID;
            string fileID = assetDatabase.GetUniqueFileID(rootID, assetName);

            await assetDatabase.CreateAssetAsync(myAsset, fileID);

            // Tạo và lưu thumbnail
            await CreateAndSaveThumbnailAsync(myAsset, fileID);

            // Giải phóng instance của asset
            await assetDatabase.ReleaseAsync(myAsset);

            return fileID;
        }

        /// <summary>
        /// Tải asset từ cơ sở dữ liệu.
        /// </summary>
        /// <param name="fileID">ID của file.</param>
        /// <returns>Trả về GameObject của asset đã tải.</returns>
        public async Task<GameObject> LoadAssetAsync(string fileID)
        {
            if (assetDatabase == null)
            {
                Debug.LogError("AssetDatabase chưa được khởi tạo. Vui lòng gọi InitializeAssetDatabaseAsync() trước.");
                return null;
            }

            // Tải asset theo fileID
            await assetDatabase.LoadAssetAsync(fileID);

            // Khởi tạo asset đã tải và trả về GameObject
            var loadedAsset = await assetDatabase.InstantiateAssetAsync(fileID);
            return loadedAsset as GameObject;
        }

        /// <summary>
        /// Tải thumbnail từ cơ sở dữ liệu.
        /// </summary>
        /// <param name="fileID">ID của file.</param>
        /// <returns>Trả về Sprite của thumbnail.</returns>
        public async Task<Sprite> LoadThumbnailAsync(string fileID)
        {
            if (assetDatabase == null)
            {
                Debug.LogError("AssetDatabase chưa được khởi tạo. Vui lòng gọi InitializeAssetDatabaseAsync() trước.");
                return null;
            }

            // Tải thumbnail bytes từ cơ sở dữ liệu
            await assetDatabase.LoadThumbnailAsync(fileID);
            var thumbnailBytes = assetDatabase.GetThumbnail(fileID);

            if (thumbnailBytes != null && thumbnailBytes.Length > 0)
            {
                Texture2D texture = new Texture2D(2, 2);
                texture.LoadImage(thumbnailBytes);
                Sprite thumbnailSprite = Sprite.Create(
                    texture,
                    new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f)
                );
                return thumbnailSprite;
            }

            Debug.LogWarning("Không thể tải thumbnail cho fileID: " + fileID);
            return null;
        }

        /// <summary>
        /// Khởi tạo ThumbnailUtil để tạo thumbnails.
        /// </summary>
        private void InitializeThumbnailUtil()
        {
            GameObject thumbnailUtilObj = new GameObject("ThumbnailUtil");
            thumbnailUtilObj.transform.LookAt(-Vector3.one);

            ThumbnailUtil util = thumbnailUtilObj.AddComponent<ThumbnailUtil>();
            util.ThumbnailLayer = thumbnailLayer;
            util.SnapshotTextureWidth = 512;
            util.SnapshotTextureHeight = 512;

            thumbnailUtil = util;
        }

        /// <summary>
        /// Tạo và lưu thumbnail của asset.
        /// </summary>
        /// <param name="asset">GameObject của asset.</param>
        /// <param name="fileID">ID của file.</param>
        private async Task CreateAndSaveThumbnailAsync(GameObject asset, string fileID)
        {
            // Tạo thumbnail texture
            Texture2D thumbnailTexture = await thumbnailUtil.CreateThumbnailAsync(asset);

            // Mã hóa texture thành PNG
            byte[] thumbnailBytes = await thumbnailUtil.EncodeToPngAsync(thumbnailTexture);

            // Lưu thumbnail vào cơ sở dữ liệu
            await assetDatabase.SaveThumbnailAsync(fileID, thumbnailBytes);
        }
    }
}
