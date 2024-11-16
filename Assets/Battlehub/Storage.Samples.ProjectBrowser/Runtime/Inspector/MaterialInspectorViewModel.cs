using System;
using UnityEngine;
using UnityWeld.Binding;

namespace Battlehub.Storage.Samples
{
    [Binding, DefaultExecutionOrder(-10)]
    public class MaterialInspectorViewModel : MonoBehaviour, System.ComponentModel.INotifyPropertyChanged
    {
        private bool m_canEdit;
        [Binding]
        public bool CanEdit
        {
            get { return m_canEdit; }
        }

        private bool m_isLoaded;
        [Binding]
        public bool IsLoaded
        {
            get { return m_isLoaded; }
        }

        private Texture2D m_thumbnail;
        [Binding]
        public Texture Thumbnail
        {
            get { return m_thumbnail; }
        }

        private string m_name;
        [Binding]
        public string Name
        {
            get { return m_name; }
        }

        private bool m_hasMainTexure;
        [Binding]
        public bool HasMainTexture
        {
            get { return m_hasMainTexure; }
        }

        private AssetRefPropViewModel m_mainTexture = new AssetRefPropViewModel();
        [Binding]
        public AssetRefPropViewModel MainTexture
        {
            get { return m_mainTexture; }
        }

        private Guid m_targetID;
        private IEditorModel m_editor;
        private IDialogHelpers m_dialog;

        private IAssetDatabase AssetDatabase
        {
            get { return m_editor.AssetDatabase; }
        }

        private void Awake()
        {
            m_dialog = GetComponentInParent<IDialogHelpers>();
            m_editor = EditorModel.Instance;
        }

        private void Start()
        {
            if (m_editor.AssetDatabase == null)
            {
                return;
            }

            m_editor.ChangeAssetSelection += OnChangeAssetSelection;
            SetTarget(m_editor.SelectedAsset);
        }

        private void OnDestroy()
        {
            if (m_editor != null)
            {
                m_editor.ChangeAssetSelection -= OnChangeAssetSelection;
                m_editor = null;
            }
            m_editor = null;
        }

        [Binding]
        public async void LoadAsset()
        {
            using var bi = m_dialog.BusyIndicator("Loading");
            await AssetDatabase.LoadAssetAsync(m_targetID);

            SetTarget(m_targetID);
            OnRefreshSelection();
        }

        [Binding]
        public void SetRandomColor()
        {
            var material = AssetDatabase.GetAsset<Material>(m_targetID);
            material.color = UnityEngine.Random.ColorHSV();
            SaveAsset();
            LoadThumbnail();
            OnRefreshSelection();
        }

        private void OnChangeAssetSelection(object sender, AssetSelectionEventArgs e)
        {
            SetTarget(e.SelectedAsset);
            OnRefreshSelection();
        }

        private void SetTarget(Guid targetID)
        {
            m_targetID = targetID;

            m_canEdit = false;
            m_isLoaded = false;
            m_thumbnail = null;
            m_name = string.Empty;
            m_hasMainTexure = false;
            m_mainTexture = new AssetRefPropViewModel();

            if (AssetDatabase.TryGetMeta(m_targetID, out var meta) && !AssetDatabase.IsExternalAsset(m_targetID) && AssetDatabase.GetAssetType(m_targetID) == typeof(Material))
            {
                m_name = meta.Name;
                LoadThumbnail();

                m_isLoaded = AssetDatabase.IsLoaded(m_targetID);
                m_canEdit = true;

                if (m_isLoaded)
                {
                    var material = AssetDatabase.GetAsset<Material>(m_targetID);

                    m_hasMainTexure = material.HasProperty("_MainTex");
                    if (m_hasMainTexure)
                    {
                        m_mainTexture = new AssetRefPropViewModel(AssetDatabase, material, nameof(Material.mainTexture), (o, s) => SaveAsset());
                    }
                }
            }
        }

        private void LoadThumbnail()
        {
            var thumbnail = AssetDatabase.GetThumbnail(m_targetID);
            if (thumbnail != null)
            {
                m_thumbnail = new Texture2D(1, 1);
                m_thumbnail.LoadImage(thumbnail);
            }
        }

        private async void SaveAsset()
        {
            using var bi = m_dialog.BusyIndicator("");
            await m_editor.SaveAssetAsync(m_targetID);
        }

        private void OnRefreshSelection()
        {
            RaisePropertyChanged(nameof(CanEdit));
            RaisePropertyChanged(nameof(IsLoaded));
            RaisePropertyChanged(nameof(Thumbnail));
            RaisePropertyChanged(nameof(Name));
            RaisePropertyChanged(nameof(HasMainTexture));
            RaisePropertyChanged(nameof(MainTexture));
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(name));
        }

    }
}
