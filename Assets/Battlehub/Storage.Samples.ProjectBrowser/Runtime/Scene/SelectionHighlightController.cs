using UnityEngine;

namespace Battlehub.Storage.Samples
{
    public class SelectionHighlightController : MonoBehaviour
    {
        [SerializeField]
        private Camera m_camera;

        [SerializeField]
        private Material m_outlineMaterial;

        private GameObject m_selectedGameObject;
        private SelectionHighlight m_selectionHighlight;

        public GameObject SelectedGameObject
        {
            get { return m_selectedGameObject; }
            set
            {
                if (m_selectedGameObject != value)
                {
                    m_selectedGameObject = value;

                    if (m_selectionHighlight != null)
                    {
                        Destroy(m_selectionHighlight);
                    }

                    if (m_selectedGameObject != null)
                    {
                        m_selectionHighlight = m_selectedGameObject.AddComponent<SelectionHighlight>();
                        m_selectionHighlight.Camera = m_camera;
                        m_selectionHighlight.Material = m_outlineMaterial;
                    }
                }
            }
        }
    }
}
