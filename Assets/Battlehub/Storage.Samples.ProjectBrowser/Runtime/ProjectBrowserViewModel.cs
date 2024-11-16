using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityWeld.Binding;

namespace Battlehub.Storage.Samples
{
    [Binding, DefaultExecutionOrder(-10)]
    public class ProjectBrowserViewModel : ProjectViewModel, IDialogHelpers, INotifyPropertyChanged
    {
        [SerializeField]
        private string m_projectName;

        [Binding]
        public bool CanOpenPrefab
        {
            get { return Editor.CanOpenPrefab; }
        }

        [Binding]
        public bool CanClosePrefab
        {
            get { return Editor.CanClosePrefab; }
        }

        [Binding]
        public bool CanCreateObject
        {
            get { return true; }
        }

        [Binding]
        public bool CanCreatePrefab
        {
            get { return Editor.CanCreatePrefab(Editor.SelectedGameObject); }
        }

        [Binding]
        public bool CanCreatePrefabVariant
        {
            get { return Editor.CanCreatePrefabVariant(Editor.SelectedGameObject); }
        }

        [Binding]
        public bool CanDetachPrefab
        {
            get { return Editor.CanDetach; }
        }

        [Binding]
        public bool CanDuplicate
        {
            get { return Editor.CanDuplicate; }
        }

        [Binding]
        public bool CanDestroy
        {
            get { return Editor.CanRelease; }
        }

        [Binding]
        public bool CanCreateNewScene
        {
            get { return Editor.CanInitializeNewScene; }
        }

        [Binding]
        public bool CanSaveScene
        {
            get { return Editor.CanSaveScene; }
        }

        [Binding]
        public bool CanCreateFolder
        {
            get { return true; }
        }

        [Binding]
        public bool CanImportResource
        {
            get { return Editor.CanImportExternalAsset; }
        }

        [Binding]
        public override bool CanOpen
        {
            get
            {
                if (Selection == null)
                {
                    return false;
                }

                return base.CanOpen || Editor.CanOpenAsset(SelectedItem.Meta.ID);
            }
        }

        [Binding]
        public bool CanInstantiate
        {
            get { return Selection != null && Editor.CanInstantiateAsset(SelectedItem.Meta.ID); }
        }

        [Binding]
        public bool CanRename
        {
            get { return Selection != null; }
        }

        [Binding]
        public bool CanCopy
        {
            get { return Selection != null; }
        }

        [Binding]
        public bool CanCut
        {
            get { return Selection != null; }
        }

        [Binding]
        public bool CanPaste
        {
            get { return m_bufferedID != Guid.Empty; }
        }

        [Binding]
        public bool CanDelete
        {
            get { return Selection != null; }
        }

        [Binding]
        public bool CanDropAsset
        {
            get
            {
                if (m_dragSourceObject != null)
                {
                    return false;
                }

                if (m_dropTargetAsset != null && m_dragSourceAsset != null)
                {
                    if (m_dropTargetAsset.Meta.ID == m_dragSourceAsset.Meta.ID)
                    {
                        return false;
                    }

                    return Editor.AssetDatabase.IsFolder(m_dropTargetAsset.Meta.ID);
                }

                return false;
            }
        }

        [Binding]
        public bool CanDropObject
        {
            get
            {
                if (m_dragSourceAsset != null)
                {
                    return false;
                }

                if (m_dragSourceObject != null)
                {
                    return Editor.CanCreatePrefab(m_dragSourceObject.Object) || Editor.CanCreatePrefabVariant(m_dragSourceObject.Object);
                }

                return false;
            }
        }

        private HierarchyItemViewModel m_dragSourceObject;
        private AssetViewModel m_dragSourceAsset;
        private object m_dragSource;
        [Binding]
        public object DragSource
        {
            get { return m_dragSource; }
            set
            {
                if (DragSource != value)
                {
                    m_dragSource = value;
                    m_dragSourceAsset = value as AssetViewModel;
                    m_dragSourceObject = value as HierarchyItemViewModel;
                    RaisePropertyChanged(nameof(DragSource));
                }
            }
        }

        private AssetViewModel m_dropTargetAsset;
        [Binding]
        public object DropTarget
        {
            get { return m_dropTargetAsset; }
            set
            {
                if (DropTarget != value)
                {
                    m_dropTargetAsset = value as AssetViewModel;
                    RaisePropertyChanged(nameof(DropTarget));
                    RaisePropertyChanged(nameof(CanDropAsset));
                    RaisePropertyChanged(nameof(CanDropObject));
                }
            }
        }

        private GameObject m_newInstance;

        [Binding]
        public GameObject NewInstance
        {
            get { return m_newInstance; }
            set
            {
                if (m_newInstance != value)
                {
                    m_newInstance = value;
                    RaisePropertyChanged(nameof(NewInstance));
                }
            }
        }


        private readonly DialogViewModel m_renameDialog = new DialogViewModel();
        [Binding]
        public DialogViewModel RenameDialog
        {
            get { return m_renameDialog; }
        }

        private readonly DialogViewModel m_confirmationDialog = new DialogViewModel();
        [Binding]
        public DialogViewModel ConfirmationDialog
        {
            get { return m_confirmationDialog; }
        }

        private readonly DialogViewModel m_messageBox = new DialogViewModel();
        [Binding]
        public DialogViewModel MessageBox
        {
            get { return m_messageBox; }
        }

        private readonly ImportAssetViewModel m_importAssetViewModel = new ImportAssetViewModel();
        [Binding]
        public ImportAssetViewModel ImportAssetDialog
        {
            get { return m_importAssetViewModel; }
        }

        private Guid m_bufferedID;
        private bool m_copy;

        protected override async void Start()
        {
            base.Start();

            if (Editor.AssetDatabase == null)
            {
                return;
            }

            Editor.ChangeSelection += OnChangeSelection;
            Editor.RefreshSelection += OnRefreshSelection;
            Editor.CreateFolder += OnCreateFolder;
            Editor.CreateAsset += OnCreateAsset;
            Editor.SaveAsset += OnSaveAsset;
            Editor.InitializeNewScene += OnInitializeNewScene;
            Editor.CreateScene += OnCreateScene;
            Editor.OverwriteScene += OnOverwriteScene;
            Editor.UpdateThumbnail += OnUpdateThumbnail;
            Editor.DeleteAsset += OnDeleteAsset;
            Editor.OpenPrefab += OnOpenPrefab;
            Editor.ClosePrefab += OnClosePrefab;
            Editor.Detach += OnDetach;
            Editor.ApplyChanges += OnApply;
            Editor.ApplyChangesToBase += OnApplyToBase;
            Editor.RevertChangesToBase += OnResetToBase;
            Editor.CyclicNestingDetected += OnCyclicNestingDetected;

            await LoadProject();

            Editor.CurrentFolder = Editor.AssetDatabase.RootID;

            await DataBind();

            RaisePropertyChanged(nameof(Items));

            if (Editor.CurrentScene == null)
            {
                NewScene();
            }
        }

        protected override async void OnDestroy()
        {
            if (Editor != null)
            {
                Editor.ChangeSelection -= OnChangeSelection;
                Editor.RefreshSelection -= OnRefreshSelection;
                Editor.CreateFolder -= OnCreateFolder;
                Editor.CreateAsset -= OnCreateAsset;
                Editor.SaveAsset -= OnSaveAsset;
                Editor.InitializeNewScene -= OnInitializeNewScene;
                Editor.CreateScene -= OnCreateScene;
                Editor.OverwriteScene -= OnOverwriteScene;
                Editor.UpdateThumbnail -= OnUpdateThumbnail;
                Editor.DeleteAsset -= OnDeleteAsset;
                Editor.OpenPrefab -= OnOpenPrefab;
                Editor.ClosePrefab += OnClosePrefab;
                Editor.Detach -= OnDetach;
                Editor.ApplyChanges -= OnApply;
                Editor.ApplyChangesToBase -= OnApplyToBase;
                Editor.RevertChangesToBase -= OnResetToBase;
                Editor.CyclicNestingDetected -= OnCyclicNestingDetected;
            }

            await UnloadProject();

            base.OnDestroy();
        }

        private async Task LoadProject()
        {
            using var bi = new BusyIndicator(this, "Loading Project");

            string defaultProjectName = $"{Application.persistentDataPath}/SampleProject";

            if (!Path.IsPathRooted(m_projectName))
            {
                m_projectName = $"{Application.persistentDataPath}/{m_projectName}";
            }

            string projectName = !string.IsNullOrWhiteSpace(m_projectName) ?
                m_projectName :
                defaultProjectName;
                
            await Editor.LoadProjectAsync(projectName);
        }

        private async Task UnloadProject()
        {
            using var bi = new BusyIndicator(this, "Unloading Project");
            await Editor.UnloadProjectAsync();
        }

        [Binding]
        public async void OpenPrefab()
        {
            if (!CanOpenPrefab)
            {
                return;
            }

            using var bi = new BusyIndicator(this, "Loading Prefab");
            await Editor.OpenPrefabAsync();
        }

        [Binding]
        public async void ClosePrefab()
        {
            if (!CanClosePrefab)
            {
                return;
            }

            using var bi = new BusyIndicator(this, "Closing Prefab");
            await Editor.ClosePrefabAsync();
        }

        [Binding]
        public async void CreateObject()
        {
            if (!CanCreateObject)
            {
                return;
            }

            var values = Enum.GetValues(typeof(PrimitiveType));
            var type = (PrimitiveType)values.GetValue(UnityEngine.Random.Range(0, values.Length));

            var go = GameObject.CreatePrimitive(type);
            var parent = Editor.CurrentPrefab != null ?
                Editor.CurrentPrefab.transform :
                Editor.CurrentScene.transform;

            go.transform.SetParent(parent, false);
            Editor.SelectedGameObject = go;
            NewInstance = go;

            await Editor.DontDestroySubAssetsAsync(go);
        }

        [Binding]
        public void CreateOriginalPrefab()
        {
            if (!CanCreatePrefab)
            {
                return;
            }

            CreateOriginalPrefab(Editor.SelectedGameObject);
        }

        private async void CreateOriginalPrefab(GameObject go)
        {
            if (Editor.ExtractSubAssets(go).Any())
            {
                ConfirmationDialog.Show("Extract SubAssets?", "Yes", "No", async yes =>
                {
                    using var bi = new BusyIndicator(this, "Creating Prefab");
                    if (yes)
                    {
                        foreach (UnityEngine.Object subAsset in Editor.ExtractSubAssets(go))
                        {
                            await Editor.CreateAssetAsync(Editor.CurrentFolder, subAsset);
                        }

                        await Editor.CreatePrefabAsync(Editor.CurrentFolder, go, false);
                    }
                    else
                    {
                        await Editor.CreatePrefabAsync(Editor.CurrentFolder, go, false);
                    }
                });
            }
            else
            {
                using var bi = new BusyIndicator(this, "Creating Prefab");
                await Editor.CreatePrefabAsync(Editor.CurrentFolder, go, false);
            }  
        }

        [Binding]
        public void CreatePrefabVariant()
        {
            if (!CanCreatePrefabVariant)
            {
                return;
            }

            CreatePrefabVariant(Editor.SelectedGameObject);
        }

        private async void CreatePrefabVariant(GameObject go)
        {
            using var bi = new BusyIndicator(this, "Creating Prefab Variant");
            await Editor.CreatePrefabAsync(Editor.CurrentFolder, go, true);
        }

        [Binding]
        public void DetachPrefab()
        {
            if (!CanDetachPrefab)
            {
                return;
            }

            ConfirmationDialog.Show("Unpack Prefab Completely?", "Yes", "No", yes =>
            {
                using var bi = new BusyIndicator(this, "Unpacking Prefab");
                Editor.DetachAsync(completely: yes);
            });
        }

        [Binding]
        public async void Duplicate()
        {
            if (!CanDuplicate)
            {
                return;
            }

            using var bi = new BusyIndicator(this, "Duplicating");
            await Editor.DuplicateAsync();
        }

        [Binding]
        public async void Destroy()
        {
            if (!CanDestroy)
            {
                return;
            }

            using var bi = new BusyIndicator(this, "Destroying");
            await Editor.ReleaseAsync();
        }

        [Binding]
        public async void SaveScene()
        {
            if (!CanSaveScene)
            {
                return;
            }

            if (SelectedItem != null && Editor.IsScene(SelectedItem.Meta))
            {
                if (SelectedItem.Meta.ID != Editor.CurrentSceneID)
                {
                    ConfirmationDialog.Show($"Scene with {SelectedItem.DisplayName} already exists. Overwrite it?", "Yes", "No", async yes =>
                    {
                        if (yes)
                        {
                            using var bi = new BusyIndicator(this, "Saving Scene");
                            await Editor.OverwriteSceneAsync(SelectedItem.Meta.ID, Editor.CurrentScene);
                        }
                    });
                }
                else
                {
                    using var bi = new BusyIndicator(this, "Saving Scene");
                    await Editor.OverwriteSceneAsync(SelectedItem.Meta.ID, Editor.CurrentScene);
                }
            }
            else
            {
                using var bi = new BusyIndicator(this, "Saving Scene");
                await Editor.CreateSceneAsync(Editor.CurrentFolder, Editor.CurrentScene);
            }
        }

        [Binding]
        public async void NewScene()
        {
            if (!CanCreateNewScene)
            {
                return;
            }

            using var bi = new BusyIndicator(this, "Initializing New Scene");
            await Editor.InitializeNewSceneAsync();
        }

        [Binding]
        public override async void Open()
        {
            if (!CanOpen)
            {
                return;
            }

            bool isPrefab = Editor.IsPrefab(SelectedItem.Meta);
            bool isScene = Editor.IsScene(SelectedItem.Meta);
            if (isPrefab || isScene)
            {
                using var bi = new BusyIndicator(this, isPrefab ? "Loading Prefab" : "Loading Scene");
                await Editor.OpenAssetAsync(SelectedItem.Meta.ID);
            }
            else
            {
                base.Open();
            }
        }

        [Binding]
        public async void CreateFolder()
        {
            if (!CanCreateFolder)
            {
                return;
            }

            var assetDatabase = Editor.AssetDatabase;
            var folderID = assetDatabase.GetUniqueFileID(Editor.CurrentFolder, "Folder");

            using var bi = new BusyIndicator(this, "Creating Folder");
            await Editor.CreateFolderAsync(folderID);
        }

        [Binding]
        public void ImportResource()
        {
            if (!CanImportResource)
            {
                return;
            }

            ImportAssetDialog.Show(this, this);
        }

        [Binding]
        public void Rename()
        {
            if (!CanRename)
            {
                return;
            }

            RenameDialog.Show(SelectedItem.DisplayName, async ok =>
            {
                if (ok)
                {
                    var assetDatabase = Editor.AssetDatabase;
                    string name = RenameDialog.Text;
                    if (string.IsNullOrEmpty(name))
                    {
                        MessageBox.Show("Name cannot be empty");
                        return;
                    }

                    if (name == SelectedItem.DisplayName)
                    {
                        return;
                    }

                    string ext = Path.GetExtension(SelectedItem.Name);
                    string newFileID = assetDatabase.GetUniqueFileID(Editor.CurrentFolder, $"{name}{ext}");
                    if (Path.GetFileNameWithoutExtension(newFileID) != name)
                    {
                        MessageBox.Show("Asset with same name already exists");
                        return;
                    }

                    if (Editor.IsFolder(SelectedItem.Meta))
                    {
                        await assetDatabase.MoveFolderAsync(SelectedItem.Meta.FileID, newFileID);
                    }
                    else
                    {
                        await assetDatabase.MoveAssetAsync(SelectedItem.Meta.ID, newFileID);
                    }

                    SelectedItem.Meta = assetDatabase.GetMeta(newFileID);
                    SelectedItem.Name = SelectedItem.Meta.Name;
                    Items.Remove(SelectedItem);
                    await Task.Yield();
                    InsertOrdered(SelectedItem);

                    var selectedItem = SelectedItem;
                    Selection = null;
                    await Task.Yield();
                    Selection = selectedItem;
                }
            });
        }

        private async Task MoveAsync(IMeta<Guid, string> meta, string newFileID)
        {
            var assetDatabase = Editor.AssetDatabase;
            if (Editor.CurrentFolder == assetDatabase.GetParent(meta.ID))
            {
                Items.Remove(GetAssetItem(meta.ID));
            }

            if (Editor.IsFolder(meta))
            {
                await assetDatabase.MoveFolderAsync(meta.FileID, newFileID);
            }
            else
            {
                await assetDatabase.MoveAssetAsync(meta.ID, newFileID);
            }
        }

        [Binding]
        public async void Instantiate()
        {
            if (!CanInstantiate)
            {
                return;
            }

            var meta = SelectedItem.Meta;

            using var bi = new BusyIndicator(this, "Loading");

            await Editor.InstantiateAssetAsync(meta.ID);
        }

        [Binding]
        public void Copy()
        {
            if (!CanCopy)
            {
                return;
            }

            var meta = SelectedItem.Meta;
            m_bufferedID = meta.ID;
            m_copy = true;
            RaisePropertyChanged(nameof(CanPaste));
        }

        [Binding]
        public void Cut()
        {
            if (!CanCut)
            {
                return;
            }

            var meta = SelectedItem.Meta;
            m_bufferedID = meta.ID;
            m_copy = false;
            RaisePropertyChanged(nameof(CanPaste));
        }

        [Binding]
        public async void Paste()
        {
            if (!CanPaste)
            {
                return;
            }

            var assetDatabase = Editor.AssetDatabase;
            bool gotMeta = assetDatabase.TryGetMeta(m_bufferedID, out var meta);
            if (!m_copy || !gotMeta)
            {
                m_bufferedID = default;
            }
            RaisePropertyChanged(nameof(CanPaste));

            if (!gotMeta)
            {
                Debug.LogWarning($"No meta found for buffered ID {m_bufferedID}");
                return;
            }

            var parentID = assetDatabase.GetParent(meta.ID);
            bool sameParent = parentID == Editor.CurrentFolder;
            if (sameParent && !m_copy)
            {
                return;
            }

            var newFileID = assetDatabase.GetUniqueFileID(Editor.CurrentFolder, meta.Name);
            if (Path.GetFileName(newFileID) != meta.Name)
            {
                if (!sameParent)
                {
                    ShowSameAssetOrFolderAlreadyExistMessage(meta);
                    return;
                }
            }

            if (m_copy)
            {
                using var bi = new BusyIndicator(this, "Copying");

                if (Editor.IsFolder(meta))
                {
                    await assetDatabase.DuplicateFolderAsync(meta.ID, newFileID);
                }
                else
                {

                    try
                    {
                        await assetDatabase.DuplicateAssetAsync(meta.ID, newFileID);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                        MessageBox.Show("Can't duplicate external asset");
                        return;
                    }
                }
            }
            else
            {
                using var bi = new BusyIndicator(this, "Moving");

                await MoveAsync(meta, newFileID);
            }

            var newMeta = assetDatabase.GetMeta(newFileID);
            var thumbnail = await LoadThumbnailAsync(newMeta.ID);
            InsertOrdered(new AssetViewModel(newMeta, thumbnail, newMeta.Name));
        }

        [Binding]
        public void Delete()
        {
            if (!CanDelete)
            {
                return;
            }

            var selectedItem = SelectedItem;
            ConfirmationDialog.Show($"Delete Selected " + (Editor.IsFolder(selectedItem.Meta) ? "Folder ?" : "Asset ?"), "Yes", "No", async yes =>
            {
                if (!yes)
                {
                    return;
                }

                using var bi = new BusyIndicator(this, "Deleting");
                await Editor.DeleteAssetAsync(selectedItem.Meta.ID);
            });
        }

        [Binding]
        public async void Drop()
        {
            if (m_dropTargetAsset != null && Editor.IsFolder(m_dropTargetAsset.Meta))
            {
                if (m_dragSourceAsset != null)
                {
                    var assetDatabase = Editor.AssetDatabase;
                    var sourceMeta = m_dragSourceAsset.Meta;
                    var targetMeta = m_dropTargetAsset.Meta;

                    var newFileID = assetDatabase.GetUniqueFileID(targetMeta.ID, sourceMeta.Name);
                    if (Path.GetFileName(newFileID) != sourceMeta.Name)
                    {
                        if (targetMeta.ID != Editor.CurrentFolder)
                        {
                            ShowSameAssetOrFolderAlreadyExistMessage(sourceMeta);
                            return;
                        }
                    }
                    await MoveAsync(sourceMeta, newFileID);
                }
            }
            else
            {
                if (m_dragSourceObject != null)
                {
                    var dragSource = m_dragSourceObject.Object;
                    bool canCreatePrefab = Editor.CanCreatePrefab(dragSource);
                    bool canCreateVariant = Editor.CanCreatePrefabVariant(dragSource);

                    if (canCreateVariant && canCreatePrefab)
                    {
                        ConfirmationDialog.Show("Would you like to create a new original Prefab or a variant of this Prefab?", "Original Prefab", "Prefab Variant", async original =>
                        {
                            if (!original)
                            {
                                CreatePrefabVariant(dragSource);
                            }
                            else
                            {
                                await Task.Yield();
                                CreateOriginalPrefab(dragSource);
                            }
                        });
                    }
                    else if (canCreateVariant)
                    {
                        CreatePrefabVariant(dragSource);
                    }
                    else if (canCreatePrefab)
                    {
                        CreateOriginalPrefab(dragSource);
                    }
                }
            }
        }

        [Binding]
        public async void DropToParentFolder()
        {
            if (!Editor.AssetDatabase.TryGetParent(Editor.CurrentFolder, out var parentFolderID))
            {
                return;
            }

            var sourceMeta = m_dragSourceAsset.Meta;
            var newFileID = Editor.AssetDatabase.GetUniqueFileID(parentFolderID, m_dragSourceAsset.Meta.Name);
            if (Path.GetFileName(newFileID) != sourceMeta.Name)
            {
                ShowSameAssetOrFolderAlreadyExistMessage(sourceMeta);
                return;
            }

            await MoveAsync(m_dragSourceAsset.Meta, newFileID);
        }

        private void ShowSameAssetOrFolderAlreadyExistMessage(IMeta<Guid, string> sourceMeta)
        {
            if (Editor.IsFolder(sourceMeta))
            {
                MessageBox.Show($"Folder with the name '{sourceMeta.Name}' already exists.");
            }
            else
            {
                MessageBox.Show($"Asset with the name '{sourceMeta.Name}' already exists.");
            }
        }

        private void OnChangeSelection(object sender, SelectionEventArgs e)
        {
            if (e.SelectedGameObject != null)
            {
                Selection = null;
            }

            OnRefreshSelection();
        }

        private void OnRefreshSelection(object sender, EventArgs e)
        {
            OnRefreshSelection();
        }

        private void OnCreateFolder(object sender, AssetEventArgs e)
        {
            if (!Editor.IsInFolder(Editor.CurrentFolder, e.AssetID))
            {
                return;
            }

            var meta = Editor.AssetDatabase.GetMeta(e.AssetID);
            InsertOrdered(new AssetViewModel(meta, FolderIcon, meta.Name));
        }

        private void OnCreateAsset(object sender, AssetThumbnailEventArgs e)
        {
            if (!Editor.IsInFolder(Editor.CurrentFolder, e.AssetID))
            {
                return;
            }

            CreateAssetItem(e.AssetID, e.Thumbnail);
            OnPrefabOperationCompleted();
        }

        private void OnSaveAsset(object sender, AssetThumbnailEventArgs e)
        {
            var item = GetAssetItem(e.AssetID);
            if (item != null)
            {
                if (item.Thumbnail != null && !IsBuiltinThumbnail(item.Thumbnail))
                {
                    Destroy(item.Thumbnail);
                }

                item.Thumbnail = e.Thumbnail != null ? Instantiate(e.Thumbnail) : GetBuiltinThumbnail(e.AssetID);
                item.Meta = Editor.AssetDatabase.GetMeta(e.AssetID);
            }

            OnPrefabOperationCompleted();
        }

        private void OnUpdateThumbnail(object sender, AssetThumbnailEventArgs e)
        {
            var item = GetAssetItem(e.AssetID);
            if (item != null)
            {
                if (item.Thumbnail != null && !IsBuiltinThumbnail(item.Thumbnail))
                {
                    Destroy(item.Thumbnail);
                }

                item.Thumbnail = e.Thumbnail != null ? Instantiate(e.Thumbnail) : GetBuiltinThumbnail(e.AssetID);
            }
        }

        private void OnDeleteAsset(object sender, AssetEventArgs e)
        {
            var item = GetAssetItem(e.AssetID);

            int index = Items.IndexOf(item);
            Items.Remove(item);

            var newSelectedItem = index >= 0 ? Items.ElementAtOrDefault(index) : null;
            if (newSelectedItem == null)
            {
                newSelectedItem = Items.ElementAtOrDefault(index - 1);
            }

            Selection = newSelectedItem;
            OnPrefabOperationCompleted();
        }

        private void OnInitializeNewScene(object sender, EventArgs e)
        {
            Editor.SelectedGameObject = null;
        }

        private void OnCreateScene(object sender, AssetEventArgs e)
        {
            if (!Editor.IsInFolder(Editor.CurrentFolder, e.AssetID))
            {
                return;
            }

            CreateAssetItem(e.AssetID, null);

            Editor.RefreshSelectedGameObject();
        }

        private async void OnOverwriteScene(object sender, AssetEventArgs e)
        {
            if (!Editor.IsInFolder(Editor.CurrentFolder, e.AssetID))
            {
                return;
            }

            var fileId = Editor.AssetDatabase.GetMeta(e.AssetID).FileID;
            var item = Items.Where(item => item.Meta.FileID == fileId).FirstOrDefault();
            Items.Remove(item);

            item = CreateAssetItem(e.AssetID, null);

            await Task.Yield();
            Selection = item;

            Editor.RefreshSelectedGameObject();
        }

        private void OnOpenPrefab(object sender, AssetEventArgs e)
        {
            RaisePropertyChanged(nameof(CanClosePrefab));
            RaisePropertyChanged(nameof(CanSaveScene));
            RaisePropertyChanged(nameof(CanCreateNewScene));
        }

        private void OnClosePrefab(object sender, EventArgs e)
        {
            RaisePropertyChanged(nameof(CanClosePrefab));
            RaisePropertyChanged(nameof(CanSaveScene));
            RaisePropertyChanged(nameof(CanCreateNewScene));
        }

        private void OnDetach(object sender, InstanceEventArgs e)
        {
            OnPrefabOperationCompleted();
        }

        private void OnApply(object sender, InstanceEventArgs e)
        {
            OnPrefabOperationCompleted();
        }

        private void OnApplyToBase(object sender, InstanceEventArgs e)
        {
            OnPrefabOperationCompleted();
        }

        private void OnResetToBase(object sender, InstanceEventArgs e)
        {
            OnPrefabOperationCompleted();
        }

        private void OnCyclicNestingDetected(object sender, AssetEventArgs e)
        {
            MessageBox.Show("Cyclic Prefab nesting not supported");
        }

        private AssetViewModel CreateAssetItem(Guid assetID, Texture2D thumbnailTexture)
        {
            if (thumbnailTexture == null)
            {
                thumbnailTexture = GetBuiltinThumbnail(assetID);
            }
            else
            {
                thumbnailTexture = Instantiate(thumbnailTexture);
            }

            var meta = Editor.AssetDatabase.GetMeta(assetID);
            var item = new AssetViewModel(meta, thumbnailTexture, meta.Name);
            InsertOrdered(item);

            var asset = Editor.AssetDatabase.GetAsset<UnityEngine.Object>(assetID);
            asset.name = item.DisplayName;

            return item;
        }

        private void InsertOrdered(AssetViewModel item)
        {
            AssetNamesComparer comparer = new AssetNamesComparer();

            if (Editor.IsFolder(item.Meta))
            {
                int index = 0;
                for (int i = 0; i < Items.Count; ++i)
                {
                    if (!Editor.IsFolder(Items[i].Meta))
                    {
                        break;
                    }

                    if (comparer.Compare(Items[i].Name, item.Name) <= 0)
                    {
                        index++;
                    }
                }

                Items.Insert(index, item);
            }
            else
            {
                int index = 0;
                for (int i = 0; i < Items.Count; ++i)
                {
                    if (Editor.IsFolder(Items[i].Meta))
                    {
                        index++;
                        continue;
                    }

                    if (comparer.Compare(Items[i].Name, item.Name) <= 0)
                    {
                        index++;
                    }
                }
                Items.Insert(index, item);
            }
        }

        private AssetViewModel GetAssetItem(Guid id)
        {
            var item = Items.Where(item => item.Meta.ID == id).FirstOrDefault();
            return item;
        }

        protected override void OnSelectedItemChanged()
        {
            base.OnSelectedItemChanged();

            if (SelectedItem != null)
            {
                Editor.SelectedGameObject = null;
                Editor.SelectedAsset = SelectedItem.Meta.ID;
            }
            else
            {
                Editor.SelectedAsset = default;
            }

            RaisePropertyChanged(nameof(CanCreateFolder));
            RaisePropertyChanged(nameof(CanInstantiate));
            RaisePropertyChanged(nameof(CanRename));
            RaisePropertyChanged(nameof(CanCut));
            RaisePropertyChanged(nameof(CanCopy));
            RaisePropertyChanged(nameof(CanPaste));
            RaisePropertyChanged(nameof(CanDelete));
        }

        private void OnRefreshSelection()
        {
            RaisePropertyChanged(nameof(CanCreatePrefab));
            RaisePropertyChanged(nameof(CanCreatePrefabVariant));
            RaisePropertyChanged(nameof(CanDetachPrefab));
            RaisePropertyChanged(nameof(CanDuplicate));
            RaisePropertyChanged(nameof(CanDestroy));
            RaisePropertyChanged(nameof(CanOpenPrefab));
        }

        private void OnPrefabOperationCompleted()
        {
            Editor.RefreshSelectedGameObject();
        }

        IDialog IDialogHelpers.Rename => m_renameDialog;

        IDialog IDialogHelpers.Confirmation => m_confirmationDialog;

        IDialog IDialogHelpers.MessageBox => m_messageBox;

        BusyIndicator IDialogHelpers.BusyIndicator(string text)
        {
            return new BusyIndicator(this, text);
        }

    }

    public interface IDialogHelpers
    {
        public IDialog Rename { get; }
        public IDialog Confirmation { get; }
        public IDialog MessageBox { get; }
        public BusyIndicator BusyIndicator(string text);
    }
}
