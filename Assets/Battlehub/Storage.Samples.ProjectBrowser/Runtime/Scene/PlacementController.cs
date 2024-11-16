using UnityEngine;
using UnityEngine.Events;

using UnityObject = UnityEngine.Object;

namespace Battlehub.Storage.Samples
{
    [RequireComponent(typeof(SceneController))]
    public class PlacementController : MonoBehaviour
    {
        private SceneController m_sceneController;

        public bool UseRaycastToPlaceInstance
        {
            get;
            set;
        }

        private GameObject m_newInstance;
        public GameObject NewInstance
        {
            get { return m_newInstance; }
            set
            {
                if (m_newInstance != value)
                {
                    m_newInstance = value;

                    if (m_newInstance == null)
                    {
                        return;
                    }

                    int layer = m_newInstance.layer;
                    m_newInstance.layer = LayerMask.NameToLayer("Ignore Raycast");
                    if (UseRaycastToPlaceInstance)
                    {
                        m_newInstance.transform.position = Vector3.zero;
                        var bounds = TransformUtil.CalculateBounds(m_newInstance.transform);
                        if (m_sceneController.GetPointOnGroundPlane(out var hitPoint))
                        {
                            m_newInstance.transform.position = new Vector3(hitPoint.x, hitPoint.y - bounds.min.y, hitPoint.z);
                        }
                    }

                    m_newInstance.layer = layer;
                }
            }
        }

        private UnityObject m_dragAsset;
        public UnityObject DragAsset
        {
            get { return m_dragAsset; }
            set
            {
                if (m_dragAsset != value)
                {
                    m_dragAsset = value;

                    if (m_dragAsset != null && m_sceneController.Raycast(out var hit))
                    {
                        DropTarget = hit.collider.gameObject;
                    }
                }
            }
        }


        public UnityEvent DropTargetChanged;

        private GameObject m_dropTarget;
        public GameObject DropTarget
        {
            get { return m_dropTarget; }
            set
            {
                if (m_dropTarget != value)
                {
                    m_dropTarget = value;
                    DropTargetChanged?.Invoke();
                }
            }
        }

        private void Start()
        {
            m_sceneController = GetComponent<SceneController>();
        }
    }
}