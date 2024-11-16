using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Battlehub.Storage.Samples
{
    public struct SelectionEventArgs
    {
        public GameObject SelectedGameObject
        {
            get;
            private set;
        }

        public GameObject UnselectedGameObject
        {
            get;
            private set;
        }

        public SelectionEventArgs(GameObject selected, GameObject unselected)
        {
            SelectedGameObject = selected;
            UnselectedGameObject = unselected;
        }
    }

    public struct AssetSelectionEventArgs
    {
        public Guid SelectedAsset
        {
            get;
            private set;
        }

        public Guid UnselectedAsset
        {
            get;
            private set;
        }

        public AssetSelectionEventArgs(Guid selectedAsset, Guid unselectedAsset)
        {
            SelectedAsset = selectedAsset;
            UnselectedAsset = unselectedAsset;
        }
    }


    public struct InstanceEventArgs
    {
        public GameObject Instance
        {
            get;
            private set;
        }

        public InstanceEventArgs(GameObject instance)
        {
            Instance = instance;
        }
    }

    public struct AssetEventArgs
    {
        public Guid AssetID
        {
            get;
            private set;
        }

        public AssetEventArgs(Guid assetID)
        {
            AssetID = assetID;
        }
    }

    public interface IEditorModel
    {
        event EventHandler LoadProject;

        event EventHandler UnloadProject;

        event EventHandler RefreshSelection;

        event EventHandler<SelectionEventArgs> ChangeSelection;

        event EventHandler<AssetSelectionEventArgs> ChangeAssetSelection;

        event EventHandler<AssetEventArgs> CreateFolder;

        event EventHandler<AssetThumbnailEventArgs> CreateAsset;

        event EventHandler<AssetThumbnailEventArgs> SaveAsset;

        event EventHandler InitializeNewScene;

        event EventHandler<AssetEventArgs> CreateScene;

        event EventHandler<AssetEventArgs> OverwriteScene;

        event EventHandler<AssetThumbnailEventArgs> UpdateThumbnail;

        event EventHandler<AssetEventArgs> BeforeDeleteAsset;

        event EventHandler<AssetEventArgs> DeleteAsset;

        event EventHandler<AssetEventArgs> OpenPrefab;

        event EventHandler ClosePrefab;

        event EventHandler<InstanceEventArgs> InstantiateAsset;

        event EventHandler<AssetEventArgs> CyclicNestingDetected;

        event EventHandler<InstanceEventArgs> Detach;

        event EventHandler<InstanceEventArgs> SetDirty;

        event EventHandler<InstanceEventArgs> Duplicate;

        event EventHandler<InstanceEventArgs> Release;

        event EventHandler<InstanceEventArgs> ApplyChanges;

        event EventHandler<InstanceEventArgs> ApplyChangesToBase;

        event EventHandler<InstanceEventArgs> RevertChangesToBase;

        bool CanSaveScene { get; }

        bool CanInitializeNewScene { get; }

        GameObject CurrentScene { get; }

        Guid CurrentSceneID { get; }

        bool CanCreatePrefab(UnityObject obj);

        bool CanCreatePrefabVariant(UnityObject obj);

        bool CanImportExternalAsset { get; }

        bool CanOpenPrefab { get; }

        bool CanClosePrefab { get; }

        bool CanOpenAsset(Guid assetID);

        bool CanInstantiateAsset(Guid assetID);

        bool CanDetach { get; }

        bool CanDuplicate { get; }

        bool CanRelease { get; }

        bool CanApplyChanges { get; }

        bool CanApplyToBase { get; }

        bool CanRevertToBase { get; }

        GameObject CurrentHierarchyParent { get; set; }

        GameObject CurrentPrefab { get; }

        GameObject SelectedGameObject { get; set; }

        Guid SelectedAsset { get; set; }

        Guid CurrentFolder { get; set; }

        IAssetDatabase AssetDatabase { get; }

        bool IsProjectLoaded { get; }

        Task LoadProjectAsync(string projectID);

        Task UnloadProjectAsync();

        bool IsPrefabOperationAllowed(object instance);

        bool IsScene(in IMeta<Guid, string> meta);

        bool IsScene(Guid id);

        bool IsPrefab(in IMeta<Guid, string> meta);

        bool IsPrefab(Guid id);

        bool IsFolder(in IMeta<Guid, string> meta);

        bool IsFolder(Guid id);

        bool IsInFolder(Guid folderID, Guid assetID);

        void RefreshSelectedGameObject();

        Task<bool> SetParentAsync(GameObject obj, Transform parent, bool worldPositionStays);

        IEnumerable<UnityObject> ExtractSubAssets(UnityObject asset);

        Task DontDestroySubAssetsAsync(UnityObject obj);

        Task<Guid> CreateFolderAsync(string path);

        Task<Guid> CreateAssetAsync(Guid folderID, UnityObject obj);

        Task<Guid> CreatePrefabAsync(Guid folderID, UnityObject obj, bool variant);

        Task<Guid> ImportExternalAssetAsync(Guid folderID, string key, string loaderID, string desiredName);

        Task InitializeNewSceneAsync();

        Task<Guid> CreateSceneAsync(Guid folderID, UnityObject obj);

        Task<Guid> OverwriteSceneAsync(Guid sceneID, UnityObject obj);

        Task SaveAssetAsync(Guid assetID);

        Task DeleteAssetAsync(Guid assetID);

        Task OpenPrefabAsync();

        Task ClosePrefabAsync();

        Task OpenAssetAsync(Guid assetID);

        Task InstantiateAssetAsync(Guid assetID, Transform parent = null);

        Task DetachAsync(bool completely);

        Task SetDirtyAsync(Component component);

        Task DuplicateAsync();

        Task ReleaseAsync();

        bool IsCyclicNestingAfterApplyingChanges(bool toBase);

        Task ApplyChangesAsync();

        Task ApplyToBaseAsync();

        Task RevertToBaseAsync();
    }

    [DefaultExecutionOrder(-20)]
    public class EditorModel : MonoBehaviour, IEditorModel
    {
        private const bool k_unloadAssetsBeforeInitializingNewScene = true;
        private const bool k_unloadAssetsBeforeLoadingScene = false;

        public event EventHandler LoadProject;

        public event EventHandler UnloadProject;

        public event EventHandler RefreshSelection;

        public event EventHandler<SelectionEventArgs> ChangeSelection;

        public event EventHandler<AssetSelectionEventArgs> ChangeAssetSelection;

        public event EventHandler<AssetEventArgs> CreateFolder;

        public event EventHandler<AssetThumbnailEventArgs> CreateAsset;

        public event EventHandler<AssetThumbnailEventArgs> SaveAsset;

        public event EventHandler InitializeNewScene;

        public event EventHandler<AssetEventArgs> CreateScene;

        public event EventHandler<AssetEventArgs> OverwriteScene;

        public event EventHandler<AssetThumbnailEventArgs> UpdateThumbnail;

        public event EventHandler<AssetEventArgs> BeforeDeleteAsset;

        public event EventHandler<AssetEventArgs> DeleteAsset;

        public event EventHandler<AssetEventArgs> OpenPrefab;

        public event EventHandler ClosePrefab;

        public event EventHandler<InstanceEventArgs> InstantiateAsset;

        public event EventHandler<AssetEventArgs> CyclicNestingDetected;

        public event EventHandler<InstanceEventArgs> Detach;

        public event EventHandler<InstanceEventArgs> SetDirty;

        public event EventHandler<InstanceEventArgs> Duplicate;

        public event EventHandler<InstanceEventArgs> Release;

        public event EventHandler<InstanceEventArgs> ApplyChanges;

        public event EventHandler<InstanceEventArgs> ApplyChangesToBase;

        public event EventHandler<InstanceEventArgs> RevertChangesToBase;

        private string m_projectID;

        public bool CanInitializeNewScene
        {
            get { return CurrentPrefab == null; }
        }

        public bool CanSaveScene
        {
            get { return CurrentPrefab == null; }
        }

        [SerializeField]
        private GameObject m_currentScene;

        public GameObject CurrentScene
        {
            get { return m_currentScene; }
            set { m_currentScene = value; }
        }

        public Guid CurrentSceneID
        {
            get;
            private set;
        }

        public bool CanCreatePrefab(UnityObject obj)
        {
            return IsPrefabOperationAllowed(obj) && m_assetDatabase.CanCreateAsset(obj);
        }

        public bool CanCreatePrefabVariant(UnityObject obj)
        {
            return IsPrefabOperationAllowed(obj) && m_assetDatabase.CanCreateAssetVariant(obj);
        }

        public bool CanImportExternalAsset
        {
            get { return true; }
        }

        public bool CanOpenPrefab
        {
            get
            {
                if (!IsPrefabOperationAllowed(SelectedGameObject))
                {
                    return false;
                }

                if (m_assetDatabase.IsExternalAssetInstance(SelectedGameObject))
                {
                    return false;
                }

                if (SelectedGameObject == CurrentPrefab)
                {
                    object asset = m_assetDatabase.GetAssetByInstance(SelectedGameObject);
                    return asset != null && m_assetDatabase.IsInstanceRoot(asset);
                }

                if (m_assetDatabase.IsInstanceRoot(SelectedGameObject))
                {
                    return true;
                }

                if (m_assetDatabase.IsInstanceRootRef(SelectedGameObject))
                {
                    return true;
                }

                return false;
            }
        }

        public bool CanClosePrefab
        {
            get { return CurrentPrefab != null; }
        }

        public bool CanOpenAsset(Guid assetID)
        {
            if (m_assetDatabase.IsExternalAsset(assetID))
            {
                return false;
            }

            return IsPrefab(assetID) || IsScene(assetID);
        }

        public bool CanInstantiateAsset(Guid assetID)
        {
            return m_assetDatabase.CanInstantiateAsset(assetID);
        }

        public bool CanDetach
        {
            get { return IsPrefabOperationAllowed(SelectedGameObject) && m_assetDatabase.CanDetach(SelectedGameObject) && SelectedGameObject != CurrentPrefab; }
        }

        public bool CanDuplicate
        {
            get { return SelectedGameObject != null && SelectedGameObject != CurrentPrefab && SelectedGameObject != CurrentScene; }
        }

        public bool CanRelease
        {
            get { return SelectedGameObject != null && SelectedGameObject != CurrentPrefab && SelectedGameObject != CurrentScene; }
        }

        public bool CanApplyChanges
        {
            get { return SelectedGameObject != null && m_assetDatabase.CanApplyChangesAndSaveAsync(SelectedGameObject, CurrentPrefab); }
        }

        public bool CanApplyToBase
        {
            get { return SelectedGameObject != null && m_assetDatabase.CanApplyChangesToBaseAndSaveAsync(SelectedGameObject, CurrentPrefab); }
        }

        public bool CanRevertToBase
        {
            get { return SelectedGameObject != null && m_assetDatabase.CanRevertChangesToBaseAndSaveAsync(SelectedGameObject, CurrentPrefab); }
        }

        private GameObject m_currentHierarchyParent;
        public GameObject CurrentHierarchyParent
        {
            get
            {
                if (m_currentHierarchyParent != null)
                {
                    return m_currentHierarchyParent;
                }

                return CurrentPrefab != null ? CurrentPrefab : CurrentScene;
            }
            set
            {
                m_currentHierarchyParent = value;
            }
        }

        private readonly Stack<GameObject> m_openedPrefabs = new Stack<GameObject>();
        public GameObject CurrentPrefab
        {
            get { return m_openedPrefabs.Count > 0 ? m_openedPrefabs.Peek() : null; }
        }

        private GameObject m_selectedGameObject;

        public GameObject SelectedGameObject
        {
            get { return m_selectedGameObject; }
            set
            {
                if (m_selectedGameObject != value)
                {
                    var unselectedGameObject = m_selectedGameObject;

                    m_selectedGameObject = value;

                    ChangeSelection?.Invoke(this, new SelectionEventArgs(m_selectedGameObject, unselectedGameObject));
                }
            }
        }

        private Guid m_selectedAsset;

        public Guid SelectedAsset
        {
            get { return m_selectedAsset; }
            set
            {
                if (m_selectedAsset != value)
                {
                    var unselectedAsset = m_selectedAsset;

                    m_selectedAsset = value;

                    ChangeAssetSelection?.Invoke(this, new AssetSelectionEventArgs(m_selectedAsset, unselectedAsset));
                }
            }
        }

        public Guid CurrentFolder
        {
            get;
            set;
        }

        private static IEditorModel m_instance;
        public static IEditorModel Instance
        {
            get { return m_instance; }
        }

        private static IAssetDatabase m_assetDatabase;
        public IAssetDatabase AssetDatabase
        {
            get { return m_assetDatabase; }
        }

        public bool IsProjectLoaded
        {
            get { return m_assetDatabase != null && m_assetDatabase.IsProjectLoaded; }
        }

        private const string k_sceneExt = ".scene";
        private const string k_prefabExt = ".prefab";
        private const string k_assetExt = ".asset";

        private GameObject m_runtimeAssetDatabaseHost;

        [SerializeField]
        private ThumbnailUtil m_thumbnailUtil;

        private void Awake()
        {
            if (m_instance != null)
            {
                Debug.LogWarning($"Another instance of {nameof(EditorModel)} already exists");
                Destroy(this);
                return;
            }

            m_instance = this;

            m_assetDatabase = RuntimeAssetDatabase.Instance;
            if (m_assetDatabase == null)
            {
                m_runtimeAssetDatabaseHost = new GameObject("AssetDatabase");
                m_runtimeAssetDatabaseHost.transform.SetParent(transform, false);

                var assetDatabaseHostType = Type.GetType(RuntimeAssetDatabase.AssetDatabaseHostTypeName);
                if (assetDatabaseHostType == null)
                {
                    Debug.LogError("Cannot find script Battlehub.Storage.RuntimeAssetDatabaseHost. Click Tools->Runtime Asset Database->Build All");
                    return;
                }
                else
                {
                    m_runtimeAssetDatabaseHost.AddComponent(assetDatabaseHostType);
                    m_assetDatabase = RuntimeAssetDatabase.Instance;
                }
            }

            m_assetDatabase.RegisterExternalAssetLoaderAsync(nameof(ResourcesLoader), new ResourcesLoader());
#if UNITY_ADDRESSABLES
            m_assetDatabase.RegisterExternalAssetLoaderAsync(nameof(AddressablesLoader), new AddressablesLoader());
#endif

            if (m_thumbnailUtil == null)
            {
                m_thumbnailUtil = GetComponentInChildren<ThumbnailUtil>();
                if (m_thumbnailUtil == null)
                {
                    m_thumbnailUtil = gameObject.AddComponent<ThumbnailUtil>();
                }
            }
        }

        private void OnDestroy()
        {
            m_instance = null;

            m_assetDatabase = null;
            m_thumbnailUtil = null;
            m_openedPrefabs.Clear();
            m_selectedGameObject = null;
            CurrentScene = null;

            Destroy(m_runtimeAssetDatabaseHost);
            m_runtimeAssetDatabaseHost = null;
        }

        public async Task LoadProjectAsync(string projectID)
        {
            m_projectID = projectID;

            Debug.Log("Project Folder: " + m_projectID);

            if (m_currentScene != null)
            {
                // this is to prevent 'Destroying assets is not permitted to avoid data loss.' message
                // The runtime resource asset will not attempt to destroy assets with the Don't Destroy flag.
                await DontDestroySubAssetsAsync(m_currentScene);
            }

            await m_assetDatabase.LoadProjectAsync(m_projectID);

            LoadProject?.Invoke(this, EventArgs.Empty);
        }

        public async Task UnloadProjectAsync()
        {
            if (m_assetDatabase != null)
            {
                await m_assetDatabase.UnloadProjectAsync(true);
            }

            UnloadProject?.Invoke(this, EventArgs.Empty);

            m_projectID = null;
        }

        public bool IsPrefabOperationAllowed(object instance)
        {
            if (instance == null)
            {
                return false;
            }

            if (ReferenceEquals(instance, CurrentScene))
            {
                return false;
            }

            object instanceRoot = m_assetDatabase.GetInstanceRoot(instance);
            return instanceRoot == null ||
                ReferenceEquals(instanceRoot, CurrentPrefab) ||
                m_assetDatabase.IsAddedObject(instance);
        }

        public bool IsScene(in IMeta<Guid, string> meta)
        {
            return meta.FileID != null && meta.FileID.EndsWith(k_sceneExt);
        }

        public bool IsScene(Guid id)
        {
            return m_assetDatabase.TryGetMeta(id, out var meta) && IsScene(meta);
        }

        public bool IsPrefab(in IMeta<Guid, string> meta)
        {
            return meta.FileID != null && meta.FileID.EndsWith(k_prefabExt);
        }

        public bool IsPrefab(Guid id)
        {
            return m_assetDatabase.TryGetMeta(id, out var meta) && IsPrefab(meta);
        }

        public bool IsFolder(in IMeta<Guid, string> meta)
        {
            return m_assetDatabase.IsFolder(meta.ID);
        }

        public bool IsFolder(Guid id)
        {
            return m_assetDatabase.IsFolder(id);
        }

        public bool IsInFolder(Guid folderID, Guid id)
        {
            return m_assetDatabase.TryGetParent(id, out var parentID) && folderID == parentID;
        }

        public void RefreshSelectedGameObject()
        {
            RefreshSelection?.Invoke(this, EventArgs.Empty);
        }

        public async Task<bool> SetParentAsync(GameObject obj, Transform parent, bool worldPositionStays)
        {
            if (parent != null)
            {
                object asset = m_assetDatabase.GetAssetByInstance(obj);
                if (asset != null && m_assetDatabase.IsCyclicNesting(asset, parent))
                {
                    CyclicNestingDetected?.Invoke(this, new AssetEventArgs(m_assetDatabase.GetAssetID(asset)));
                    return false;
                }
            }

            if (obj.transform.parent != parent)
            {
                obj.transform.SetParent(parent, worldPositionStays);
                await SetDirtyAsync(obj.transform);
            }

            return true;
        }

        public IEnumerable<UnityObject> ExtractSubAssets(UnityObject obj)
        {
            var decomposition = new RuntimeAssetEnumerable(obj);
            foreach (UnityObject subAsset in decomposition)
            {
                if (subAsset is Component)
                {
                    continue;
                }

                if (m_assetDatabase.Exists(subAsset))
                {
                    continue;
                }

                if (m_assetDatabase.IsExternalAsset(subAsset))
                {
                    continue;
                }

                yield return subAsset;
            }
        }

        public async Task DontDestroySubAssetsAsync(UnityObject obj)
        {
            foreach (UnityObject subAsset in ExtractSubAssets(obj))
            {
                await m_assetDatabase.SetDontDestroyFlagAsync(subAsset);
            }
        }

        public async Task<Guid> CreateFolderAsync(string path)
        {
            var normalizedPath = m_assetDatabase.NormalizePath(path);
            if (m_assetDatabase.Exists(normalizedPath))
            {
                throw new ArgumentException($"Folder {path} already exists");
            }

            var parent = Path.GetDirectoryName(normalizedPath);
            var parentPath = m_assetDatabase.NormalizePath(parent);

            if (!m_assetDatabase.TryGetMeta(parentPath, out IMeta<Guid, string> parentMeta))
            {
                await CreateFolderAsync(parentPath);
                if (!m_assetDatabase.TryGetMeta(parentPath, out parentMeta))
                {
                    throw new ArgumentException("Parent folder not found", parentPath);
                }
            }

            await m_assetDatabase.CreateFolderAsync(path);
            var meta = m_assetDatabase.GetMeta(path);
            CreateFolder?.Invoke(this, new AssetEventArgs(meta.ID));
            return meta.ID;
        }

        public Task<Guid> CreateAssetAsync(Guid folderID, UnityObject obj)
        {
            return CreateAssetAsync(folderID, obj, false, k_assetExt);
        }

        public Task<Guid> CreatePrefabAsync(Guid folderID, UnityObject obj, bool variant)
        {
            return CreateAssetAsync(folderID, obj, variant, k_prefabExt);
        }

        private async Task<Guid> CreateAssetAsync(Guid folderID, UnityObject obj, bool variant, string ext)
        {
            if (!variant)
            {
                if (!CanCreatePrefab(obj))
                {
                    throw new InvalidOperationException("Can't create prefab");
                }

                await m_assetDatabase.DetachAsync(obj, completely: false);
            }
            else
            {
                if (!CanCreatePrefabVariant(obj))
                {
                    throw new InvalidOperationException("Can't create prefab variant");
                }
            }

            var fileID = m_assetDatabase.GetUniqueFileID(folderID, $"{obj.name}{ext}");
            var texture = await m_thumbnailUtil.CreateThumbnailAsync(obj);
            var thumbnail = await m_thumbnailUtil.EncodeToPngAsync(texture);

            await m_assetDatabase.CreateAssetAsync(obj, fileID, thumbnail);
            obj.name = Path.GetFileNameWithoutExtension(fileID);

            var assetID = m_assetDatabase.GetMeta(fileID).ID;

            if (CreateAsset != null)
            {
                CreateAsset.Invoke(this, new AssetThumbnailEventArgs(assetID, texture));
            }
            else
            {
                Destroy(texture);
            }

            return assetID;
        }

        public async Task<Guid> ImportExternalAssetAsync(Guid folderID, string key, string loaderID, string desiredName)
        {
            if (!CanImportExternalAsset)
            {
                throw new InvalidOperationException("Can't import external asset");
            }

            var loader = m_assetDatabase.GetExternalAssetLoader(loaderID);
            var tempRoot = m_assetDatabase.AssetsRoot;
            var externalAsset = await loader.LoadAsync(key, tempRoot, null);
            if (externalAsset == null)
            {
                throw new ArgumentException($"Can't load external asset with key {key}");
            }

            desiredName = (externalAsset is GameObject) ?
                    $"{desiredName}{k_prefabExt}" :
                    $"{desiredName}{k_assetExt}";

            string fileID = m_assetDatabase.GetUniqueFileID(folderID, desiredName);
            await m_assetDatabase.ImportExternalAssetAsync(externalAsset, key, loaderID, fileID);

            var obj = m_assetDatabase.GetAsset<UnityObject>(fileID);
            var texture = await m_thumbnailUtil.CreateThumbnailAsync(obj);
            var thumbnail = await m_thumbnailUtil.EncodeToPngAsync(texture);

            await m_assetDatabase.SaveThumbnailAsync(fileID, thumbnail);

            var assetID = m_assetDatabase.GetMeta(fileID).ID;

            if (CreateAsset != null)
            {
                CreateAsset?.Invoke(this, new AssetThumbnailEventArgs(assetID, texture));
            }
            else
            {
                Destroy(texture);
            }

            return assetID;
        }

        public async Task InitializeNewSceneAsync()
        {
            if (!CanInitializeNewScene)
            {
                throw new InvalidOperationException("Can't create new scene");
            }

            if (CurrentScene != null)
            {
                if (k_unloadAssetsBeforeInitializingNewScene)
                {
                    await m_assetDatabase.UnloadAllAssetsAsync(destroy: true);
                    await m_assetDatabase.ClearDontDestroyFlagsAsync();
                }

                Destroy(CurrentScene);
            }

            CurrentScene = new GameObject("Scene");
            CurrentSceneID = default;
            InitializeNewScene?.Invoke(this, EventArgs.Empty);
        }

        public async Task<Guid> CreateSceneAsync(Guid folderID, UnityObject obj)
        {
            if (!CanSaveScene)
            {
                throw new InvalidOperationException("Can't create scene");
            }

            var fileID = m_assetDatabase.GetUniqueFileID(folderID, $"Scene{k_sceneExt}");

            await m_assetDatabase.CreateAssetAsync(obj, fileID);
            await m_assetDatabase.DetachAsync(obj, completely: false);

            obj.name = Path.GetFileNameWithoutExtension(fileID);

            var sceneID = m_assetDatabase.GetMeta(fileID).ID;
            CurrentSceneID = sceneID;

            CreateScene?.Invoke(this, new AssetEventArgs(sceneID));
            return sceneID;
        }

        public async Task<Guid> OverwriteSceneAsync(Guid sceneID, UnityObject obj)
        {
            if (!CanSaveScene)
            {
                throw new InvalidOperationException("Can't overwrite scene");
            }

            var meta = m_assetDatabase.GetMeta(sceneID);

            await m_assetDatabase.DeleteAssetAsync(meta.ID);
            await m_assetDatabase.CreateAssetAsync(obj, meta.FileID);
            await m_assetDatabase.DetachAsync(obj, completely: false);

            obj.name = Path.GetFileNameWithoutExtension(meta.FileID);

            sceneID = m_assetDatabase.GetMeta(meta.FileID).ID;
            CurrentSceneID = sceneID;

            OverwriteScene?.Invoke(this, new AssetEventArgs(sceneID));
            return sceneID;
        }

        public async Task SaveAssetAsync(Guid assetID)
        {
            await m_assetDatabase.SaveAssetAndUpdateThumbnailsAsync(assetID, GetThumbnailCreatorContext());
        }

        public async Task DeleteAssetAsync(Guid assetID)
        {
            if (IsFolder(assetID))
            {
                BeforeDeleteAsset?.Invoke(this, new AssetEventArgs(assetID));
                await m_assetDatabase.DeleteFolderAsync(assetID);
                DeleteAsset?.Invoke(this, new AssetEventArgs(assetID));
            }
            else
            {
                if (CurrentPrefab != null && m_assetDatabase.GetAssetIDByInstance(CurrentPrefab) == assetID)
                {
                    await ClosePrefabAsync();
                }

                var affectedIDs = m_assetDatabase.GetAssetsAffectedBy(assetID);

                BeforeDeleteAsset?.Invoke(this, new AssetEventArgs(assetID));
                await m_assetDatabase.DeleteAssetAsync(assetID);
                DeleteAsset?.Invoke(this, new AssetEventArgs(assetID));

                await Task.Yield();

                foreach (var affectedID in affectedIDs)
                {
                    await m_assetDatabase.UpdateThumbnailAsync(affectedID, GetThumbnailCreatorContext());
                }

                if (CurrentScene == null)
                {
                    await InitializeNewSceneAsync();
                }
            }
        }

        public async Task OpenPrefabAsync()
        {
            if (!CanOpenPrefab)
            {
                throw new InvalidOperationException("Can't open prefab");
            }

            Guid assetID;
            if (SelectedGameObject == CurrentPrefab)
            {
                object asset = m_assetDatabase.GetAssetByInstance(SelectedGameObject);
                if (!m_assetDatabase.IsInstanceRoot(asset))
                {
                    throw new InvalidOperationException();
                }

                assetID = m_assetDatabase.GetAssetIDByInstance(asset);
            }
            else
            {
                if (CurrentPrefab != null)
                {
                    assetID = m_assetDatabase.GetAssetIDByInstance(m_assetDatabase.GetAssetByInstance(SelectedGameObject));
                }
                else
                {
                    assetID = m_assetDatabase.GetAssetIDByInstance(SelectedGameObject);
                }
            }

            await OpenPrefabAsync(assetID);
        }

        private async Task OpenPrefabAsync(Guid assetID)
        {
            if (!m_assetDatabase.IsLoaded(assetID))
            {
                await m_assetDatabase.LoadAssetAsync(assetID);
            }

            SelectedGameObject = null;

            if (CurrentPrefab != null)
            {
                CurrentPrefab.transform.SetParent(m_assetDatabase.AssetsRoot, true);
            }
            else
            {
                CurrentScene.SetActive(false);
            }

            var instance = await m_assetDatabase.InstantiateAssetAsync<GameObject>(assetID);

            m_openedPrefabs.Push(instance);

            SelectedGameObject = CurrentPrefab;

            OpenPrefab?.Invoke(this, new AssetEventArgs(assetID));
        }

        public async Task ClosePrefabAsync()
        {
            if (!CanClosePrefab)
            {
                throw new InvalidOperationException("Can't close prefab");
            }

            await CloseCurrentPrefabAsync(true);
        }

        private async Task CloseCurrentPrefabAsync(bool raiseEvent)
        {
            SelectedGameObject = null;
            if (CurrentPrefab != null)
            {
                await m_assetDatabase.ReleaseAsync(CurrentPrefab);
                m_openedPrefabs.Pop();
            }

            if (CurrentPrefab != null)
            {
                CurrentPrefab.transform.SetParent(null, true);
                SelectedGameObject = CurrentPrefab;
            }
            else
            {
                CurrentScene.SetActive(true);
            }

            if (raiseEvent)
            {
                ClosePrefab?.Invoke(this, EventArgs.Empty);
            }
        }

        public async Task OpenAssetAsync(Guid assetID)
        {
            if (!CanOpenAsset(assetID))
            {
                throw new InvalidOperationException("Can't open asset");
            }

            if (IsPrefab(assetID))
            {
                while (CurrentPrefab != null)
                {
                    await CloseCurrentPrefabAsync(false);
                }

                await OpenPrefabAsync(assetID);
            }
            else if (IsScene(assetID))
            {
                while (CurrentPrefab != null)
                {
                    await CloseCurrentPrefabAsync(false);
                }

                await InstantiateAssetAsync(assetID);
            }
        }

        public async Task InstantiateAssetAsync(Guid assetID, Transform parent = null)
        {
            if (m_assetDatabase.GetAssetType(assetID) == typeof(GameObject))
            {
                bool isScene = IsScene(assetID);

                if (isScene && k_unloadAssetsBeforeLoadingScene)
                {
                    await m_assetDatabase.UnloadAllAssetsAsync(destroy: true);
                }

                if (!m_assetDatabase.IsLoaded(assetID))
                {
                    await m_assetDatabase.LoadAssetAsync(assetID);
                }

                if (isScene)
                {
                    var scene = await m_assetDatabase.InstantiateAssetAsync<GameObject>(assetID);
                    await m_assetDatabase.DetachAsync(scene, completely: false);

                    if (scene != CurrentScene)
                    {
                        if (CurrentScene != null)
                        {
                            await m_assetDatabase.ReleaseAsync(CurrentScene);
                        }

                        CurrentScene = scene;
                        CurrentSceneID = assetID;
                    }

                    InstantiateAsset?.Invoke(this, new InstanceEventArgs(scene));
                }
                else
                {
                    if (parent == null)
                    {
                        parent = CurrentHierarchyParent.transform;
                    }

                    object asset = m_assetDatabase.GetAsset(assetID);
                    if (m_assetDatabase.IsCyclicNesting(asset, parent))
                    {
                        CyclicNestingDetected?.Invoke(this, new AssetEventArgs(assetID));
                    }
                    else
                    {
                        var instance = await m_assetDatabase.InstantiateAssetAsync<GameObject>(assetID, parent);
                        InstantiateAsset?.Invoke(this, new InstanceEventArgs(instance));

                        SelectedGameObject = instance;
                    }
                }
            }
            else
            {
                if (IsFolder(assetID))
                {
                    var children = m_assetDatabase.GetChildren(assetID);
                    foreach (var childID in children)
                    {
                        if (IsFolder(childID))
                        {
                            continue;
                        }

                        if (IsScene(childID))
                        {
                            continue;
                        }

                        await InstantiateAssetAsync(childID);
                    }
                }
            }
        }

        public async Task DetachAsync(bool completely)
        {
            if (!CanDetach)
            {
                throw new InvalidOperationException("Can't detach");
            }

            var instance = SelectedGameObject;

            await m_assetDatabase.DetachAsync(instance, completely: completely);

            Detach?.Invoke(this, new InstanceEventArgs(instance));
        }

        public async Task SetDirtyAsync(Component component)
        {
            await m_assetDatabase.SetDirtyAsync(component);

            SetDirty?.Invoke(this, new InstanceEventArgs(component.gameObject));
        }

        public async Task DuplicateAsync()
        {
            if (!CanDuplicate)
            {
                throw new InvalidOperationException("Can't duplicate");
            }

            var duplicate = await m_assetDatabase.InstantiateAsync(SelectedGameObject, SelectedGameObject.transform.parent);

            Duplicate?.Invoke(this, new InstanceEventArgs((GameObject)duplicate));

            SelectedGameObject = (GameObject)duplicate;
        }

        public async Task ReleaseAsync()
        {
            if (!CanRelease)
            {
                return;
            }

            var instance = SelectedGameObject;

            await m_assetDatabase.ReleaseAsync(instance);
            SelectedGameObject = null;

            Release?.Invoke(this, new InstanceEventArgs(instance));
        }

        public bool IsCyclicNestingAfterApplyingChanges(bool toBase)
        {
            if (CurrentPrefab != null && m_assetDatabase.IsCyclicNestingAfterApplyingChanges(CurrentPrefab, false))
            {
                return true;
            }

            return SelectedGameObject != null && m_assetDatabase.IsCyclicNestingAfterApplyingChanges(SelectedGameObject, toBase);
        }

        public async Task ApplyChangesAsync()
        {
            if (!CanApplyChanges)
            {
                throw new InvalidOperationException("Can't apply to base");
            }

            var instance = SelectedGameObject;

            await m_assetDatabase.ApplyChangesAndSaveAsync(instance, CurrentPrefab, GetThumbnailCreatorContext());

            ApplyChanges?.Invoke(this, new InstanceEventArgs(instance));
        }

        public async Task ApplyToBaseAsync()
        {
            if (!CanApplyToBase)
            {
                throw new InvalidOperationException("Can't apply to base");
            }

            var instance = SelectedGameObject;

            await m_assetDatabase.ApplyChangesToBaseAndSaveAsync(instance, CurrentPrefab, GetThumbnailCreatorContext());

            ApplyChangesToBase?.Invoke(this, new InstanceEventArgs(instance));
        }

        public async Task RevertToBaseAsync()
        {
            if (!CanRevertToBase)
            {
                throw new InvalidOperationException("Can't reset to base");
            }

            GameObject instance = SelectedGameObject;

            await m_assetDatabase.RevertChangesToBaseAndSaveAsync(instance, CurrentPrefab, GetThumbnailCreatorContext());

            RevertChangesToBase?.Invoke(this, new InstanceEventArgs(instance));
        }

        private IThumbnailCreatorContext GetThumbnailCreatorContext()
        {
            return new ThumbnailCreatorContext(m_thumbnailUtil,
                args => SaveAsset?.Invoke(this, args),
                args => UpdateThumbnail?.Invoke(this, args),
                noThumbnailExtensions: k_sceneExt);
        }

    }
}
