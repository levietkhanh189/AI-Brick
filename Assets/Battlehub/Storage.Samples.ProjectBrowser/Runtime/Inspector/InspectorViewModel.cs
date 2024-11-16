using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityWeld.Binding;

namespace Battlehub.Storage.Samples
{
    [Binding, DefaultExecutionOrder(-10)]
    public class InspectorViewModel : MonoBehaviour, System.ComponentModel.INotifyPropertyChanged
    {
        private GameObject m_target;

        [Binding]
        public string Name
        {
            get { return m_target != null ? m_target.name : string.Empty; }
        }

        [Binding]
        public bool CanEdit
        {
            get { return m_editor.SelectedGameObject != null && m_editor.SelectedGameObject != m_editor.CurrentScene && CanEditExternal; }
        }

        private bool CanEditExternal
        {
            get { return m_editor.CurrentPrefab == null || !m_editor.AssetDatabase.IsExternalAssetInstance(m_editor.SelectedGameObject); }
        }

        [Binding]
        public bool CanApplyChanges
        {
            get { return m_editor.CanApplyChanges; }
        }

        [Binding]
        public bool CanApplyToBase
        {
            get { return m_editor.CanApplyToBase; }
        }

        [Binding]
        public bool CanResetToBase
        {
            get { return m_editor.CanRevertToBase; }
        }

        private bool m_isGameObject;
        [Binding]
        public bool IsGameObject
        {
            get { return m_isGameObject; }
            set
            {
                if (m_isGameObject != value)
                {
                    m_isGameObject = value;
                    RaisePropertyChanged(nameof(IsGameObject));
                }
            }
        }

        private bool m_isPrefab;
        [Binding]
        public bool IsPrefab
        {
            get { return m_isPrefab; }
            set
            {
                if (m_isPrefab != value)
                {
                    m_isPrefab = value;
                    RaisePropertyChanged(nameof(IsPrefab));
                }
            }
        }

        private bool m_isPrefabVariant;
        [Binding]
        public bool IsPrefabVariant
        {
            get { return m_isPrefabVariant; }
            set
            {
                if (m_isPrefabVariant != value)
                {
                    m_isPrefabVariant = value;
                    RaisePropertyChanged(nameof(IsPrefabVariant));
                }
            }
        }

        private ObservableList<ComponentViewModel> m_components = new ObservableList<ComponentViewModel>();

        [Binding]
        public ObservableList<ComponentViewModel> Components
        {
            get { return m_components; }
        }

        private AddComponentViewModel m_addComponentViewModel = new AddComponentViewModel();

        [Binding]
        public AddComponentViewModel AddComponentViewModel
        {
            get { return m_addComponentViewModel; }
        }

        private HashSet<Type> m_serializableTypes;
        private IEditorModel m_editor;
        private IDialogHelpers m_dialog;

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

            m_editor.RefreshSelection += OnRefreshSelection;
            m_editor.ChangeSelection += OnChangeSelection;
            m_editor.LoadProject += OnLoadProject;

            if (m_editor.IsProjectLoaded)
            {
                Init();
            }
        }

        private void OnDestroy()
        {
            if (m_editor != null)
            {
                m_editor.RefreshSelection -= OnRefreshSelection;
                m_editor.ChangeSelection -= OnChangeSelection;
                m_editor.LoadProject -= OnLoadProject;
                m_editor = null;
            }
            m_editor = null;
        }

        private void Init()
        {
            m_serializableTypes = new HashSet<Type>(m_editor.AssetDatabase.GetSerializableTypes());
            SetTarget(m_editor.SelectedGameObject);
        }

        [Binding]
        public async void ApplyChanges()
        {
            if (m_editor.IsCyclicNestingAfterApplyingChanges(toBase: false))
            {
                m_dialog.MessageBox.Show("Cyclic Prefab nesting not supported");
                return;
            }

            using var bi = m_dialog.BusyIndicator("Applying Changes");
            await m_editor.ApplyChangesAsync();
        }

        [Binding]
        public async void ApplyToBase()
        {
            if (m_editor.IsCyclicNestingAfterApplyingChanges(toBase: true))
            {
                m_dialog.MessageBox.Show("Cyclic Prefab nesting not supported");
                return;
            }

            using var bi = m_dialog.BusyIndicator("Applying Changes");
            await m_editor.ApplyToBaseAsync();
        }

        [Binding]
        public async void ResetToBase()
        {
            using var bi = m_dialog.BusyIndicator("Reverting Changes");
            await m_editor.RevertToBaseAsync();
        }

        private void OnLoadProject(object sender, EventArgs e)
        {
            Init();
        }

        private void OnRefreshSelection(object sender, EventArgs e)
        {
            SetTarget(m_editor.SelectedGameObject);
            OnRefreshSelection();
        }

        private void OnChangeSelection(object sender, SelectionEventArgs e)
        {
            SetTarget(e.SelectedGameObject);
            OnRefreshSelection();
        }

        private void SetTarget(GameObject target)
        {
            m_target = target;
            m_components = CreateComponentsList();
            RaisePropertyChanged(nameof(Components));
            RaisePropertyChanged(nameof(Name));

            if (m_target != null)
            {
                bool isInstanceOfAssetVariant = m_editor.AssetDatabase.IsInstanceOfAssetVariant(m_target) ||
                    m_editor.AssetDatabase.IsInstanceOfAssetVariantRef(m_target);

                bool isInstanceRoot = m_editor.AssetDatabase.IsInstanceRoot(m_target) ||
                    m_editor.AssetDatabase.IsInstanceRootRef(m_target);

                IsPrefabVariant = isInstanceOfAssetVariant;
                IsPrefab = isInstanceRoot && !isInstanceOfAssetVariant;
                IsGameObject = !isInstanceRoot && !isInstanceOfAssetVariant;
            }
        }

        private ObservableList<ComponentViewModel> CreateComponentsList()
        {
            var list = new ObservableList<ComponentViewModel>();
            if (m_target != null)
            {
                var components = m_target.GetComponents<Component>();
                foreach (var component in components)
                {
                    var type = component.GetType();
                    if (!m_serializableTypes.Contains(type))
                    {
                        continue;
                    }

                    if (component is Transform)
                    {
                        list.Add(new TransformViewModel(component, OnChangeComponent));
                    }
                    else if (component is MeshFilter)
                    {
                        list.Add(new MeshFilterViewModel(m_editor.AssetDatabase, component, OnChangeComponent, OnDeleteComponent));
                    }
                    else if (component is MeshRenderer)
                    {
                        list.Add(new MeshRendererViewModel(m_editor.AssetDatabase, component, OnChangeComponent, OnDeleteComponent));
                    }
                    else
                    {
                        list.Add(new ComponentViewModel(component, OnDeleteComponent));
                    }
                }
            }
            return list;
        }

        [Binding]
        public void AddComponent()
        {
            m_addComponentViewModel = new AddComponentViewModel(m_editor.AssetDatabase, OnAddComponent);
            RaisePropertyChanged(nameof(AddComponentViewModel));
            m_addComponentViewModel.IsVisible = true;
        }

        [Binding]
        public void CancelAddComponent()
        {
            m_addComponentViewModel.IsVisible = false;
        }

        public void OnAddComponent(Type componentType)
        {
            if (m_target.AddComponent(componentType))
            {
                m_components = CreateComponentsList();
                RaisePropertyChanged(nameof(Components));
                m_addComponentViewModel.IsVisible = false;
                m_editor.RefreshSelectedGameObject();
            }
        }

        private async void OnChangeComponent(Component component, string propertyName)
        {
            await m_editor.SetDirtyAsync(component);
            OnRefreshSelection();
        }

        private void OnDeleteComponent(Component component)
        {
            DestroyImmediate(component);

            var viewModel = GetComponentViewModel(component);
            if (viewModel != null)
            {
                m_components.Remove(viewModel);
            }

            m_editor.RefreshSelectedGameObject();
        }

        private void OnRefreshSelection()
        {
            RaisePropertyChanged(nameof(CanEdit));
            RaisePropertyChanged(nameof(CanApplyChanges));
            RaisePropertyChanged(nameof(CanApplyToBase));
            RaisePropertyChanged(nameof(CanResetToBase));
        }

        private ComponentViewModel GetComponentViewModel(Component component)
        {
            return m_components.Where(c => c.TargetComponent == component).FirstOrDefault();
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(name));
        }
    }
}