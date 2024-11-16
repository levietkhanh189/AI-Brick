using UnityEngine;
using System.ComponentModel;
#if UNITY_ADDRESSABLES
using System.Collections;
using System.IO;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;
#endif
using UnityWeld.Binding;

namespace Battlehub.Storage.Samples
{
    [Binding]
    public class AssetKeyViewModel
    {
        [Binding]
        public string Key
        {
            get;
            private set;
        }

        public AssetKeyViewModel(string key)
        {
            Key = key;
        }
    }

    [Binding]
    public class ImportAssetViewModel : INotifyPropertyChanged
    {
        // https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/Labels.html
        private string m_importableLabel = "Importable";

        public string ImportableLabel
        {
            get { return m_importableLabel; }
            set { m_importableLabel = value; }
        }

        private ObservableList<AssetKeyViewModel> m_keys = new ObservableList<AssetKeyViewModel>();

        [Binding]
        public ObservableList<AssetKeyViewModel> Keys
        {
            get { return m_keys; }
        }

        private AssetKeyViewModel m_selectedKey;

        [Binding]
        public object Selection
        {
            get { return m_selectedKey; }
            set
            {
                if (m_selectedKey != value)
                {
                    m_selectedKey = value as AssetKeyViewModel;
                    RaisePropertyChanged(nameof(Selection));
                    RaisePropertyChanged(nameof(CanImport));
                }
            }
        }

        [Binding]
        public bool CanImport
        {
            get { return m_selectedKey != null; }
        }

        [Binding]
        public bool CanImportAll
        {
            get { return m_keys.Count > 0; }
        }

        private IEditorModel m_editor;
        private IDialogHelpers m_dialogHelpers;

        private bool m_isVisible;

        [Binding]
        public bool IsVisible
        {
            get { return m_isVisible; }
            set
            {
                if (m_isVisible != value)
                {
                    m_isVisible = value;
                    RaisePropertyChanged(nameof(IsVisible));
                }
            }
        }

#if UNITY_ADDRESSABLES
        private IEnumerator CoInit()
        {
            var loadResourceLocationsHandle = Addressables.LoadResourceLocationsAsync(ImportableLabel, typeof(Object));
            if (!loadResourceLocationsHandle.IsDone)
                yield return loadResourceLocationsHandle;

            m_keys.Clear();
            foreach (IResourceLocation location in loadResourceLocationsHandle.Result)
            {
                m_keys.Add(new AssetKeyViewModel(location.PrimaryKey.ToString()));
            }

            RaisePropertyChanged(nameof(CanImportAll));
        }
        public void Show(MonoBehaviour host, IDialogHelpers dialogHelpers)
        {
            m_editor = EditorModel.Instance;
            m_dialogHelpers = dialogHelpers;

            using var bi = m_dialogHelpers.BusyIndicator($"Importing {m_selectedKey}");

            host.StartCoroutine(CoInit());

            IsVisible = true;
        }

        [Binding]
        public async void ImportAll()
        {
            var assetDatabase = m_editor.AssetDatabase;
            string loaderID = nameof(AddressablesLoader);

            IsVisible = false;

            foreach (var assetKey in m_keys)
            {
                string key = assetKey.Key;
                string path = Path.GetDirectoryName(key);
                using var bi = m_dialogHelpers.BusyIndicator($"Importing {key}");

                if (!assetDatabase.Exists(path) && !string.IsNullOrWhiteSpace(path))
                {
                    await m_editor.CreateFolderAsync(path);
                }

                string desiredName = Path.GetFileNameWithoutExtension(key);
                try
                {
                    var folderID = assetDatabase.TryGetMeta(path, out var folderMeta) ?
                        folderMeta.ID :
                        m_editor.CurrentFolder;

                    await m_editor.ImportExternalAssetAsync(folderID, key, loaderID, desiredName);
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                }
            }

            Cancel();
        }

        [Binding]
        public async void Import()
        {
            if (m_selectedKey == null)
            {
                return;
            }

            using var bi = m_dialogHelpers.BusyIndicator($"");
            string loaderID = nameof(AddressablesLoader);

            var assetDatabase = m_editor.AssetDatabase;
            var folderID = m_editor.CurrentFolder;

            string desiredName = Path.GetFileNameWithoutExtension(m_selectedKey.Key);
            try
            {
                await m_editor.ImportExternalAssetAsync(m_editor.CurrentFolder, m_selectedKey.Key, loaderID, desiredName);
            }
            catch(System.Exception e)
            {
                Debug.LogException(e);

                IsVisible = false;
                m_dialogHelpers.MessageBox.Show($"Can't import {m_selectedKey.Key}", v => IsVisible = true);
            }
        }
#else

        [Binding]
        public void ImportAll()
        {

        }

        [Binding]
        public void Import()
        {
        }

        public async void Show(MonoBehaviour host, IDialogHelpers dialogHelpers)
        {
            m_editor = EditorModel.Instance;
            m_dialogHelpers = dialogHelpers;

            const string key = "Hellephant";
            using var bi = m_dialogHelpers.BusyIndicator($"Importing {key}");
            
            string loaderID = nameof(ResourcesLoader);
            await m_editor.ImportExternalAssetAsync(m_editor.CurrentFolder, key, loaderID, key);
        }
#endif

        [Binding]
        public void Cancel()
        {
            IsVisible = false;
            m_keys.Clear();
            RaisePropertyChanged(nameof(Keys));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
