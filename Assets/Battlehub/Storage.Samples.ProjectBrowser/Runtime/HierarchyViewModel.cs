using System;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityWeld.Binding;

namespace Battlehub.Storage.Samples
{
    [Binding, DefaultExecutionOrder(-10)]
    public class HierarchyViewModel : MonoBehaviour, INotifyPropertyChanged
    {
        private GameObject m_root;
        private GameObject Root
        {
            get { return m_root; }
            set
            {
                if (m_root != value)
                {
                    m_root = value;
                    Parent = value;
                    Refresh();
                }
            }
        }

        private GameObject Parent
        {
            get { return m_editor.CurrentHierarchyParent; }
            set { m_editor.CurrentHierarchyParent = value; }
        }

        private HierarchyItemViewModel m_selectedItem;
        protected HierarchyItemViewModel SelectedItem
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
                    m_selectedItem = value as HierarchyItemViewModel;
                    RaisePropertyChanged(nameof(Selection));
                    OnSelectedItemChanged();
                }
            }
        }

        private AssetViewModel m_dragSourceAsset;
        private HierarchyItemViewModel m_dragSourceObject;
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
                    m_dragSourceObject = m_dragSource as HierarchyItemViewModel;
                    m_dragSourceAsset = m_dragSource as AssetViewModel;
                    RaisePropertyChanged(nameof(DragSource));
                }
            }
        }

        private HierarchyItemViewModel m_dropTargetObject;
        [Binding]
        public object DropTarget
        {
            get { return m_dropTargetObject; }
            set
            {
                if (DropTarget != value)
                {
                    m_dropTargetObject = value as HierarchyItemViewModel;
                    RaisePropertyChanged(nameof(DropTarget));
                    RaisePropertyChanged(nameof(CanDrop));
                }
            }
        }

        [Binding]
        public bool CanDrop
        {
            get
            {
                if (m_dragSourceAsset != null)
                {
                    return true;
                }

                if (m_dropTargetObject == null)
                {
                    return false;
                }

                if (m_dragSourceObject != null)
                {
                    return m_dragSourceObject != m_dropTargetObject && !m_dragSourceObject.IsParent && !m_dropTargetObject.IsRoot;
                }

                return false;
            }
        }

        private ObservableList<HierarchyItemViewModel> m_items = new ObservableList<HierarchyItemViewModel>();
        [Binding]
        public ObservableList<HierarchyItemViewModel> Items
        {
            get { return m_items; }
            set
            {
                if (m_items != null)
                {
                    m_items = value;
                    RaisePropertyChanged(nameof(Items));
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

        private IEditorModel m_editor;
        private IDialogHelpers m_dialog;

        private void Awake()
        {
            m_dialog = GetComponentInParent<IDialogHelpers>();
            m_editor = EditorModel.Instance;
        }

        private void Start()
        {
            m_editor.LoadProject += OnLoadProject;
            m_editor.InitializeNewScene += OnInitializeNewScene;
            m_editor.OpenPrefab += OnOpenPrefab;
            m_editor.ClosePrefab += OnClosePrefab;
            m_editor.CreateAsset += OnCreateAsset;
            m_editor.CreateScene += OnCreateScene;
            m_editor.OverwriteScene += OnOverwriteScene;
            m_editor.BeforeDeleteAsset += OnBeforeDeleteAsset;
            m_editor.InstantiateAsset += OnInstantiate;
            m_editor.Detach += OnDetach;
            m_editor.SetDirty += OnSetDirty;
            m_editor.Duplicate += OnDuplicate;
            m_editor.Release += OnRelease;
            m_editor.ApplyChanges += OnApplyChanges;
            m_editor.ApplyChangesToBase += OnApplyChangesToBase;
            m_editor.RevertChangesToBase += OnRevertChangesToBase;
            m_editor.ChangeSelection += OnChangeSelection;
            m_editor.RefreshSelection += OnRefreshSelection;

            if (m_editor.IsProjectLoaded)
            {
                Root = m_editor.CurrentPrefab != null ? m_editor.CurrentPrefab : m_editor.CurrentScene;
            }
        }

        private void OnDestroy()
        {
            if (m_editor != null)
            {
                m_editor.LoadProject -= OnLoadProject;
                m_editor.InitializeNewScene -= OnInitializeNewScene;
                m_editor.OpenPrefab -= OnOpenPrefab;
                m_editor.ClosePrefab -= OnClosePrefab;
                m_editor.CreateAsset -= OnCreateAsset;
                m_editor.CreateScene -= OnCreateScene;
                m_editor.OverwriteScene -= OnOverwriteScene;
                m_editor.BeforeDeleteAsset -= OnBeforeDeleteAsset;
                m_editor.InstantiateAsset -= OnInstantiate;
                m_editor.Detach -= OnDetach;
                m_editor.SetDirty -= OnSetDirty;
                m_editor.Duplicate -= OnDuplicate;
                m_editor.Release -= OnRelease;
                m_editor.ApplyChanges -= OnApplyChanges;
                m_editor.ApplyChangesToBase -= OnApplyChangesToBase;
                m_editor.RevertChangesToBase -= OnRevertChangesToBase;
                m_editor.ChangeSelection -= OnChangeSelection;
                m_editor.RefreshSelection -= OnRefreshSelection;
                m_editor = null;
            }
            m_editor = null;
        }

        [Binding]
        public async void Drop()
        {
            var dropTarget = m_dropTargetObject;
            var dragSource = m_dragSourceObject;

            if (dragSource != null && dropTarget != null)
            {
                if (dropTarget.IsParent)
                {
                    if (await m_editor.SetParentAsync(dragSource.Object, dropTarget.Object.transform.parent, worldPositionStays: true))
                    {
                        m_items.Remove(dragSource);
                    }
                }
                else
                {
                    if (await m_editor.SetParentAsync(dragSource.Object, dropTarget.Object.transform, worldPositionStays: true))
                    {
                        m_items.Remove(dragSource);
                    }
                }
            }
            else
            {
                if (m_dragSourceAsset != null)
                {
                    using var bi = m_dialog.BusyIndicator("Loading");
                    await m_editor.InstantiateAssetAsync(m_dragSourceAsset.Meta.ID, dropTarget != null ? dropTarget.Object.transform : Parent.transform);
                }
            }

            if (dropTarget != null)
            {
                Refresh(dropTarget.Object);
            }
        }

        [Binding]
        public void ClearSelection()
        {
            Selection = null;
        }

        private void OnLoadProject(object sender, EventArgs e)
        {
            Root = m_editor.CurrentScene;
        }

        private void OnInitializeNewScene(object sender, EventArgs e)
        {
            Root = m_editor.CurrentScene;
        }

        private void OnCreateScene(object sender, AssetEventArgs e)
        {
            if (Parent == Root)
            {
                m_root = null;
                Root = m_editor.CurrentScene;
            }
        }

        private void OnOverwriteScene(object sender, AssetEventArgs e)
        {
            if (Parent == Root)
            {
                m_root = null;
                Root = m_editor.CurrentScene;
            }
        }

        private void OnOpenPrefab(object sender, AssetEventArgs e)
        {
            Root = m_editor.CurrentPrefab;
        }

        private void OnClosePrefab(object sender, EventArgs e)
        {
            Root = m_editor.CurrentPrefab != null ? m_editor.CurrentPrefab : m_editor.CurrentScene;
        }

        private void OnCreateAsset(object sender, AssetThumbnailEventArgs e)
        {
            var instances = m_editor.AssetDatabase.GetInstances<GameObject>(e.AssetID);
            foreach (var instance in instances)
            {
                Refresh(instance);
            }
        }

        private void OnBeforeDeleteAsset(object sender, AssetEventArgs e)
        {
            var instances = m_editor.AssetDatabase.GetInstances<GameObject>(e.AssetID);

            GameObject newParent = null;
            foreach (var instance in instances)
            {
                if (IsDescendant(Parent.transform, instance.transform))
                {
                    newParent = instance.transform.parent.gameObject;
                    break;
                }
            }

            if (newParent != null)
            {
                Parent = newParent;
                Refresh();

                foreach (var instance in instances)
                {
                    Remove(instance);
                }
            }
            else
            {
                foreach (var instance in instances)
                {
                    var children = instance.GetComponentsInChildren<Transform>(true);
                    foreach (var transform in children)
                    {
                        Remove(transform.gameObject);
                    }
                }
            }
        }

        private bool IsDescendant(Transform descendant, Transform parent)
        {
            while (descendant != null)
            {
                if (descendant == parent)
                {
                    return true;
                }
                descendant = descendant.parent;
            }
            return false;
        }

        private async void OnInstantiate(object sender, InstanceEventArgs e)
        {
            if (m_editor.CurrentScene == e.Instance)
            {
                Root = m_editor.CurrentScene;
            }
            else
            {
                NewInstance = e.Instance;

                await m_editor.SetDirtyAsync(e.Instance.transform);

                if (e.Instance == m_editor.SelectedGameObject)
                {
                    m_editor.RefreshSelectedGameObject();
                }

                if (e.Instance.transform.parent == Parent.transform)
                {
                    Add(e.Instance);
                }
                else
                {
                    Refresh(e.Instance.transform.parent.gameObject);
                }
            }
        }

        private void OnDetach(object sender, InstanceEventArgs e)
        {
            Refresh(e.Instance);
        }

        private void OnDuplicate(object sender, InstanceEventArgs e)
        {
            if (m_editor.CurrentScene == e.Instance)
            {
                Root = m_editor.CurrentScene;
            }
            else
            {
                if (e.Instance.transform.parent == Parent.transform)
                {
                    Add(e.Instance);
                }
                else
                {
                    Refresh(e.Instance.transform.parent.gameObject);
                }
            }
        }

        private void OnRelease(object sender, InstanceEventArgs e)
        {
            if (IsDescendant(Parent.transform, e.Instance.transform))
            {
                var newParent = e.Instance.transform.parent.gameObject;
                if (newParent != null)
                {
                    Parent = newParent;
                    Refresh();
                }
            }

            Remove(e.Instance);
        }

        private void OnApplyChanges(object sender, InstanceEventArgs e)
        {
            Refresh();
        }

        private void OnApplyChangesToBase(object sender, InstanceEventArgs e)
        {
            Refresh();
        }

        private void OnRevertChangesToBase(object sender, InstanceEventArgs e)
        {
            Refresh();
        }

        private void OnSetDirty(object sender, InstanceEventArgs e)
        {
            GetItemAndSetState(e.Instance, true);
        }

        private void OnSelectedItemChanged()
        {
            m_editor.SelectedGameObject = m_selectedItem != null ? m_selectedItem.Object : null;
        }

        private void OnChangeSelection(object sender, SelectionEventArgs e)
        {
            SetSelectedItem(e.SelectedGameObject);
        }

        private void OnRefreshSelection(object sender, EventArgs e)
        {
            RefreshItem(m_editor.SelectedGameObject);

            var instanceRoot = m_editor.AssetDatabase.GetInstanceRoot(m_editor.SelectedGameObject);
            if (instanceRoot != null)
            {
                RefreshItem(instanceRoot as GameObject);
            }
        }

        private void RefreshItem(GameObject go)
        {
            var item = GetHierarchyItem(go);
            if (item != null)
            {
                SetState(item, go);
            }
        }

        private void SetSelectedItem(GameObject selectedGameObject)
        {
            if (selectedGameObject != null)
            {
                var selectedItem = GetSelectedItem();
                if (selectedItem == null)
                {
                    var parent = selectedGameObject.transform.parent;
                    Parent = parent != null ? parent.gameObject : null;
                    Refresh();
                }
                else
                {
                    Selection = selectedItem;
                }
            }
            else
            {
                Selection = null;
            }
        }

        private void OnClick(HierarchyItemViewModel item)
        {
            if (item.IsParentAndIsNotRoot)
            {
                Parent = item.Object.transform.parent.gameObject;
            }
            else
            {
                Parent = item.Object;
            }

            Refresh();
        }

        private ObservableList<HierarchyItemViewModel> GetHierarchyItemsList()
        {
            var items = new ObservableList<HierarchyItemViewModel>();
            if (Parent != null)
            {
                Add(items, Parent, isParent: true, isRoot: Parent == Root);

                int childCount = Parent.transform.childCount;

                if (childCount > 50)
                {
                    Debug.LogWarning("Too many children. For performance reasons, only first 50 will be displayed.");
                    childCount = 50;
                }

                for (int i = 0; i < childCount; ++i)
                {
                    var child = Parent.transform.GetChild(i).gameObject;
                    Add(items, child);
                }
            }
            return items;
        }

        private void Add(ObservableList<HierarchyItemViewModel> items, GameObject obj, bool isParent = false, bool isRoot = false)
        {
            var childItem = new HierarchyItemViewModel(obj, OnClick, isParent, isRoot);

            SetState(childItem, obj);

            items.Add(childItem);
        }

        private void Add(GameObject obj)
        {
            Add(m_items, obj);
        }

        private void Remove(GameObject obj)
        {
            var item = GetHierarchyItem(obj);
            if (item != null)
            {
                m_items.Remove(item);
            }
        }

        private void Refresh()
        {
            m_items = GetHierarchyItemsList();
            RaisePropertyChanged(nameof(Items));

            var selectedItem = GetSelectedItem();
            if (selectedItem != null)
            {
                Selection = selectedItem;
            }
        }

        private void Refresh(GameObject obj)
        {
            var item = GetHierarchyItem(obj);
            if (item != null)
            {
                SetState(item, obj);
                item.Refresh();
            }
        }

        private void GetItemAndSetState(GameObject go, bool updateParent)
        {
            var item = GetHierarchyItem(go);
            if (item != null)
            {
                SetState(item, go);

                if (updateParent && go.transform.parent != null)
                {
                    var parentItem = GetHierarchyItem(go.transform.parent.gameObject);
                    if (parentItem != null)
                    {
                        parentItem.IsModifiedPrefab = m_editor.AssetDatabase.HasChanges(go.transform.parent.gameObject, m_editor.CurrentPrefab);
                    }                    
                }
            }
        }

        private void SetState(HierarchyItemViewModel childItem, GameObject obj)
        {
            bool isInstanceOfAssetVariant = m_editor.AssetDatabase.IsInstanceOfAssetVariant(obj) || m_editor.AssetDatabase.IsInstanceOfAssetVariantRef(obj);
            bool isInstanceRoot = m_editor.AssetDatabase.IsInstanceRoot(obj) || m_editor.AssetDatabase.IsInstanceRootRef(obj);
            bool isInstance = m_editor.AssetDatabase.IsInstance(obj);
            bool isAddedInstance = m_editor.AssetDatabase.IsAddedObject(obj);

            bool canDrag = m_editor.IsPrefabOperationAllowed(obj);
            bool hasChanges = m_editor.AssetDatabase.HasChanges(obj, m_editor.CurrentPrefab);

            childItem.IsPrefabVariant = isInstanceOfAssetVariant;
            childItem.IsPrefab = isInstanceRoot && !isInstanceOfAssetVariant;
            childItem.IsPrefabOrPrefabChild = isInstance;
            childItem.IsGameObject = !isInstanceRoot && !isInstanceOfAssetVariant;
            childItem.IsAddedPrefab = isAddedInstance;
            childItem.IsModifiedPrefab = hasChanges;
            childItem.CanDrag = canDrag;
        }

        private HierarchyItemViewModel GetHierarchyItem(GameObject obj)
        {
            return m_items.Where(item => item.Object == obj).FirstOrDefault();
        }

        private HierarchyItemViewModel GetSelectedItem()
        {
            return GetHierarchyItem(m_editor.SelectedGameObject);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
