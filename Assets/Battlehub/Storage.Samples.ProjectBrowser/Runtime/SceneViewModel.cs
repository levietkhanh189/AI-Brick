using System.ComponentModel;
using System.Threading.Tasks;
using UnityEngine;
using UnityWeld.Binding;

using UnityObject = UnityEngine.Object;

namespace Battlehub.Storage.Samples
{
    [Binding]
    public class SceneViewModel : MonoBehaviour, INotifyPropertyChanged
    {
        private GameObject m_selectedGameObject;
        [Binding]
        public GameObject SelectedGameObject
        {
            get { return m_selectedGameObject; }
            set
            {
                if (m_selectedGameObject != value)
                {
                    m_selectedGameObject = value;
                    if (!IsPartSelectionMode && m_selectedGameObject != null)
                    {
                        var root = GetRoot();
                        m_selectedGameObject = root.gameObject;
                    }

                    RaisePropertyChanged(nameof(SelectedGameObject));
                    m_editor.SelectedGameObject = m_selectedGameObject;
                }
            }
        }

        private Transform GetRoot()
        {
            var obj = m_selectedGameObject.transform;
            while (true)
            {
                if (obj.parent == null)
                {
                    break;
                }

                if (obj.parent.gameObject == m_editor.CurrentPrefab)
                {
                    break;
                }

                if (obj.parent.gameObject == m_editor.CurrentScene)
                {
                    break;
                }

                obj = obj.parent;
            }

            return obj;
        }

        private GameObject m_manipulationTarget;
        [Binding]
        public GameObject ManipulationTarget
        {
            get { return m_manipulationTarget; }
            set { m_manipulationTarget = value; }
        }

        private Vector3 m_hitGroundPosition;
        [Binding]
        public Vector3 HitGroundPosition
        {
            get { return m_hitGroundPosition; }
            set { m_hitGroundPosition = value; }
        }

        private AssetViewModel m_dragSourceAsset;
        [Binding]
        public object DragSourceObject
        {
            get { return m_dragSourceAsset; }
            set
            {
                if (DragSourceObject != value)
                {
                    m_dragSourceAsset = value as AssetViewModel;
                    RaisePropertyChanged(nameof(DragSourceObject));
                }
            }
        }

        [Binding]
        public object DropTargetObject
        {
            get { return null; }
            set { RaisePropertyChanged(nameof(CanDrop)); }
        }

        [Binding]
        public bool CanDrop
        {
            get { return m_dragSourceAsset != null || m_handlingDragAndDrop; }
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

        private UnityObject m_dragAsset;
        [Binding]
        public UnityObject DragAsset
        {
            get { return m_dragAsset; }
            set
            {
                if (m_dragAsset != value)
                {
                    m_dragAsset = value;
                    RaisePropertyChanged(nameof(DragAsset));
                }
            }
        }

        private GameObject m_dropTarget;
        [Binding]
        public GameObject DropTarget
        {
            get { return m_dropTarget; }
            set
            {
                if (m_dropTarget != value)
                {
                    m_dropTarget = value;
                    RaisePropertyChanged(nameof(DropTarget));
                }
            }
        }

        private bool m_isSceneActive;
        [Binding]
        public bool IsSceneActive
        {
            get { return m_isSceneActive; }
            set
            {
                if (m_isSceneActive != value)
                {
                    m_isSceneActive = value;
                    RaisePropertyChanged(nameof(IsSceneActive));
                }
            }
        }

        private bool m_isPartSelectionMode = true;
        [Binding]
        public bool IsPartSelectionMode
        {
            get { return m_isPartSelectionMode; }
            set
            {
                if (m_isPartSelectionMode != value)
                {
                    m_isPartSelectionMode = value;
                    RaisePropertyChanged(nameof(IsPartSelectionMode));

                    if (!m_isPartSelectionMode && m_selectedGameObject != null)
                    {
                        var root = GetRoot();
                        if (root != null)
                        {
                            SelectedGameObject = root.gameObject;
                        }
                    }
                }
            }
        }

        private Vector3 m_prevPosition;
        private Quaternion m_prevRotation;
        private Vector3 m_prevScale;

        private IEditorModel m_editor;
        private IDialogHelpers m_dialog;
        private void Awake()
        {
            m_dialog = GetComponentInParent<IDialogHelpers>();
            m_editor = EditorModel.Instance;
        }

        private void Start()
        {
            m_editor.ChangeSelection += OnChangeSelection;
            m_editor.ApplyChanges += OnApplyChanges;
            m_editor.ApplyChangesToBase += OnApplyChangesToBase;
            m_editor.RevertChangesToBase += OnRevertChangesToBase;
            SelectedGameObject = m_editor.SelectedGameObject;
        }


        private void OnDestroy()
        {
            if (m_editor != null)
            {
                m_editor.ChangeSelection -= OnChangeSelection;
                m_editor.ApplyChanges -= OnApplyChanges;
                m_editor.ApplyChangesToBase -= OnApplyChangesToBase;
                m_editor.RevertChangesToBase -= OnRevertChangesToBase;
                m_editor = null;
            }
        }

        private void OnChangeSelection(object sender, SelectionEventArgs e)
        {
            SelectedGameObject = e.SelectedGameObject;
        }

        private void OnApplyChanges(object sender, InstanceEventArgs e)
        {
            RefreshSelection();
        }

        private void OnApplyChangesToBase(object sender, InstanceEventArgs e)
        {
            RefreshSelection();
        }

        private void OnRevertChangesToBase(object sender, InstanceEventArgs e)
        {
            RefreshSelection();
        }

        private async void OnInstantiateAsset(object sender, InstanceEventArgs e)
        {
            NewInstance = e.Instance;

            await m_editor.SetDirtyAsync(e.Instance.transform);

            if (e.Instance == SelectedGameObject)
            {
                m_editor.RefreshSelectedGameObject();
            }
        }

        private bool m_handlingDragAndDrop;
        [Binding]
        public async void Drop()
        {
            var meta = m_dragSourceAsset.Meta;
            using var bi = m_dialog.BusyIndicator("Loading");

            m_editor.InstantiateAsset += OnInstantiateAsset;
            try
            {
                m_handlingDragAndDrop = true;
                await Task.Yield(); // wait for cusor to update

                if (m_editor.CanInstantiateAsset(meta.ID))
                {
                    await m_editor.InstantiateAssetAsync(meta.ID);
                }
                else
                {
                    var assetDatabase = m_editor.AssetDatabase;
                    await assetDatabase.LoadAssetAsync(meta.ID);
                    DragAsset = assetDatabase.GetAsset<UnityObject>(meta.ID);
                    if (DropTarget != null)
                    {
                        Material material = DragAsset as Material;
                        if (material != null)
                        {
                            var meshRenderer = DropTarget.GetComponentInChildren<MeshRenderer>();
                            if (meshRenderer != null)
                            {
                                var materials = meshRenderer.sharedMaterials;
                                for (int i = 0; i < materials.Length; ++i)
                                {
                                    materials[i] = material;
                                }
                                meshRenderer.sharedMaterials = materials;
                                await m_editor.SetDirtyAsync(meshRenderer);

                                if (DropTarget == m_editor.SelectedGameObject)
                                {
                                    m_editor.RefreshSelectedGameObject();
                                }
                            }
                        }


                        DragAsset = null;
                        DropTarget = null;
                    }
                }
            }
            finally
            {
                m_handlingDragAndDrop = false;
                m_editor.InstantiateAsset -= OnInstantiateAsset;
            }
        }

        [Binding]
        public void BeginManipulate()
        {
            var transform = SelectedGameObject.transform;

            m_prevPosition = transform.position;
            m_prevRotation = transform.rotation;
            m_prevScale = transform.localScale;
        }

        [Binding]
        public async void EndManipulate()
        {
            var transform = SelectedGameObject.transform;

            bool transformChanged =
                m_prevPosition != transform.position ||
                m_prevRotation != transform.rotation ||
                m_prevScale != transform.localScale;

            if (transformChanged)
            {
                await m_editor.SetDirtyAsync(transform);
            }

            m_editor.RefreshSelectedGameObject();
        }

        private void RefreshSelection()
        {
            // refresh selection highlight

            m_selectedGameObject = null;
            RaisePropertyChanged(nameof(SelectedGameObject));
            m_selectedGameObject = m_editor.SelectedGameObject;
            RaisePropertyChanged(nameof(SelectedGameObject));
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}

