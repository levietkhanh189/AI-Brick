using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Battlehub.Storage.Samples
{
    public class SceneSelection : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField]
        private Camera m_camera;

        public UnityEvent SelectedObjectChanged;

        private GameObject m_selectedObject;
        public GameObject SelectedObject
        {
            get { return m_selectedObject; }
            set
            {
                if (m_selectedObject != value)
                {
                    m_selectedObject = value;
                    SelectedObjectChanged.Invoke();
                }
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != 0)
            {
                return;
            }

            Ray ray = m_camera.ScreenPointToRay(eventData.position);

            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                SelectedObject = hitInfo.collider.gameObject;
            }
            else
            {
                SelectedObject = null;
            }
        }
    }
}
