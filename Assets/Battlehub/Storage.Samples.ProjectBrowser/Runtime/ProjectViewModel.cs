#if UNITY_STANDALONE_WIN
#define LOAD_IMAGE_ASYNC
#endif

#if LOAD_IMAGE_ASYNC
using Battlehub.Utils;
#endif

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityWeld.Binding;

namespace Battlehub.Storage.Samples
{
    public struct BusyIndicator : IDisposable
    {
        private IProjectViewModel m_viewModel;
        public BusyIndicator(IProjectViewModel viewModel, string text)
        {
            m_viewModel = viewModel;
            m_viewModel.IsBusy = true;
            m_viewModel.BusyText = text + "\n";
        }

        public void Dispose()
        {
            m_viewModel.IsBusy = false;
            m_viewModel.BusyText = null;
        }
    }

    public interface IProjectViewModel
    {
        bool IsBusy
        {
            get;
            set;
        }

        string BusyText
        {
            get;
            set;
        }
    }

    [Binding]
    public class ProjectViewModel : MonoBehaviour, INotifyPropertyChanged, IProjectViewModel
    {
        [SerializeField]
        private Texture2D m_folderIcon;
        protected Texture2D FolderIcon
        {
            get { return m_folderIcon; }
        }

        [SerializeField]
        private Texture2D m_meshIcon;
        protected Texture2D MeshIcon
        {
            get { return m_meshIcon; }
        }

        [SerializeField]
        private Texture2D m_sceneIcon;
        protected Texture2D SceneIcon
        {
            get { return m_sceneIcon; }
        }

        [SerializeField]
        private Texture2D m_genericAssetIcon;
        protected Texture2D GenericAssetIcon
        {
            get { return m_genericAssetIcon; }
        }

        private AssetViewModel m_selectedItem;
        protected AssetViewModel SelectedItem
        {
            get { return m_selectedItem; }
        }

        [Binding]
        public object Selection
        {
            get { return SelectedItem; }
            set
            {
                if (SelectedItem != value)
                {
                    m_selectedItem = value as AssetViewModel;
                    RaisePropertyChanged(nameof(Selection));
                    OnSelectedItemChanged();
                }
            }
        }

        [Binding]
        public void ClearSelection()
        {
            Selection = null;
        }

        protected virtual void OnSelectedItemChanged()
        {
            RaisePropertyChanged(nameof(CanGoBack));
            RaisePropertyChanged(nameof(CanOpen));
        }

        private readonly ObservableList<AssetViewModel> m_items = new ObservableList<AssetViewModel>();
        [Binding]
        public ObservableList<AssetViewModel> Items
        {
            get { return m_items; }
        }

        private bool m_isBusy;
        [Binding]
        public bool IsBusy
        {
            get { return m_isBusy; }
            set
            {
                if (m_isBusy != value)
                {
                    m_isBusy = value;
                    RaisePropertyChanged(nameof(IsBusy));
                }
            }
        }

        private string m_busyText;
        private string m_defaultBusyText = "Working...";

        [Binding]
        public string BusyText
        {
            get { return m_busyText != null ? m_busyText : m_defaultBusyText; }
            set
            {
                if (m_busyText != value)
                {
                    m_busyText = value;
                    RaisePropertyChanged(nameof(BusyText));
                }
            }
        }

        [Binding]
        public virtual bool CanGoBack
        {
            get { return (m_editor.AssetDatabase != null) && (m_editor.CurrentFolder != m_editor.AssetDatabase.RootID); }
        }

        [Binding]
        public virtual bool CanOpen
        {
            get
            {
                if (Selection == null)
                {
                    return false;
                }

                return m_editor.IsFolder(SelectedItem.Meta);
            }
        }

        [Binding]
        public string CurrentPath
        {
            get
            {
                if (m_editor.CurrentFolder == default)
                {
                    return string.Empty;
                }

                var path = new Stack<string>();
                var folderID = m_editor.CurrentFolder;
                do
                {
                    if (m_editor.AssetDatabase.TryGetMeta(folderID, out var meta))
                    {
                        path.Push(meta.Name);
                    }
                }
                while (m_editor.AssetDatabase.TryGetParent(folderID, out folderID));

                var sb = new StringBuilder();
                do
                {
                    sb.Append(path.Pop());
                    if (path.Count == 0)
                    {
                        break;
                    }
                    sb.Append(" > ");
                }
                while (true);

                return sb.ToString();
            }
        }
        
        private IEditorModel m_editor;
        
        protected IEditorModel Editor
        {
            get { return m_editor; }
        }

        protected virtual void Awake()
        {
            m_editor = EditorModel.Instance;
        }


        protected virtual async void Start()
        {
            await InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            if (m_editor.IsProjectLoaded)
            {
                m_editor.CurrentFolder = m_editor.AssetDatabase.RootID;
                await DataBind();
                RaisePropertyChanged(nameof(Items));
            }
        }

        protected virtual void OnDestroy()
        {
            m_editor.CurrentFolder = Guid.Empty;
            m_editor = null;
        }

        [Binding]
        public virtual async void GoBack()
        {
            if (!CanGoBack)
            {
                return;
            }

            m_editor.CurrentFolder = m_editor.AssetDatabase.GetParent(m_editor.CurrentFolder);
            Selection = null;
            await DataBind();
        }

        [Binding]
        public virtual async void Open()
        {
            if (!CanOpen)
            {
                return;
            }

            if (!m_editor.IsPrefab(SelectedItem.Meta))
            {
                m_editor.CurrentFolder = SelectedItem.Meta.ID;
                Selection = null;
                await DataBind();
            }
        }

        protected async Task DataBind()
        {
            using var bi = new BusyIndicator(this, "");

            foreach (var item in Items)
            {
                if (item.Thumbnail == null || IsBuiltinThumbnail(item.Thumbnail))
                {
                    continue;
                }

                Destroy(item.Thumbnail);
            }

            m_items.Clear();

            foreach (Guid childID in m_editor.AssetDatabase.GetChildren(m_editor.CurrentFolder, sortByName:true))
            {
                var meta = m_editor.AssetDatabase.GetMeta(childID);
                if (meta.Name.StartsWith(".")) // - hidden
                {
                    continue;
                }

                var thumbnail = await LoadThumbnailAsync(childID);
                m_items.Add(new AssetViewModel(meta, thumbnail, meta.Name));
            }

            RaisePropertyChanged(nameof(CanGoBack));
            RaisePropertyChanged(nameof(CanOpen));
            RaisePropertyChanged(nameof(CurrentPath));
        }

        protected async Task<Texture2D> LoadThumbnailAsync(Guid id)
        {
            Texture2D thumbnail = null;
            if (m_editor.IsFolder(id))
            {
                thumbnail = m_folderIcon;
            }
            else
            {
                var type = m_editor.AssetDatabase.GetAssetType(id);
                var meta = m_editor.AssetDatabase.GetMeta(id);

                if (!UsesBuiltinThumbnail(type) && !m_editor.IsScene(meta))
                {
                    await m_editor.AssetDatabase.LoadThumbnailAsync(id);
                    var thumbnailBytes = m_editor.AssetDatabase.GetThumbnail(id);

                    if (thumbnailBytes != null)
                    {
                        thumbnail = new Texture2D(1, 1);
#if LOAD_IMAGE_ASYNC
                        await thumbnail.LoadImageAsync(thumbnailBytes);
#else
                        thumbnail.LoadImage(thumbnailBytes);
#endif
                    }
                }
                else
                {
                    thumbnail = GetBuiltinThumbnail(id);
                }
            }

            return thumbnail;
        }

        protected bool UsesBuiltinThumbnail(Type type)
        {
            return type != typeof(GameObject) &&
                type != typeof(Material) &&
                type != typeof(Texture2D);
        }

        protected bool IsBuiltinThumbnail(Texture texture)
        {
            return
                texture == m_folderIcon ||
                texture == m_sceneIcon ||
                texture == m_meshIcon ||
                texture == m_genericAssetIcon;
        }

        protected Texture2D GetBuiltinThumbnail(Guid id)
        {
            Texture2D thumbnail = null;
            if (m_editor.IsFolder(id))
            {
                thumbnail = m_folderIcon;
            }
            else
            {
                var type = m_editor.AssetDatabase.GetAssetType(id);
                var meta = m_editor.AssetDatabase.GetMeta(id);

                if (type == typeof(GameObject))
                {
                    if (m_editor.IsScene(meta))
                    {
                        thumbnail = m_sceneIcon;
                    }
                }                
                else if (type == typeof(Mesh))
                {
                    thumbnail = m_meshIcon;
                }
                else
                {
                    thumbnail = m_genericAssetIcon;
                }
            }

            return thumbnail;
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
