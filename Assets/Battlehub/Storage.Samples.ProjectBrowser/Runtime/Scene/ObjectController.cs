using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;

namespace Battlehub.Storage.Samples
{
    public class ObjectController : MonoBehaviour
    {
        public UnityEvent BeginManipulate;

        public UnityEvent EndManipulate;

        public UnityEvent TargetChanged;

        [SerializeField]
        private Camera m_camera;

        private GameObject m_hitTarget;
        public GameObject HitTraget
        {
            get { return m_hitTarget; }
            set
            {
                if (m_hitTarget != value)
                {
                    m_hitTarget = value;
                    TargetChanged?.Invoke();
                }
            }
        }

        private GameObject m_selectedGameObject;
        public GameObject SelectedGameObject
        {
            get { return m_selectedGameObject; }
            set { m_selectedGameObject = value; }
        }

        private bool m_isSceneActive;
        public bool IsSceneActive
        {
            get { return m_isSceneActive; }
            set { m_isSceneActive = value; }
        }

        private Vector3 m_offset;
        private Plane m_groundPlane;

        private void Update()
        {
            if (!m_isSceneActive)
            {
                if (!Input.GetMouseButton(0))
                {
                    DeselectObject();
                }

                if (HitTraget == null)
                {
                    return;
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = m_camera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    float dot = Vector3.Dot(m_camera.transform.forward, Vector3.up);
                    
                    m_groundPlane = Mathf.Abs(dot) < 0.33f ?
                        new Plane(-m_camera.transform.forward, TransformUtil.CalculateBounds(hit.collider.transform).center) :
                        new Plane(Vector3.up, TransformUtil.CalculateBounds(hit.collider.transform).center);

                    // Left mouse button clicked on this object
                    SelectObject(hit.collider.transform);
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                DeselectObject();
            }

            if (m_hitTarget != null && SelectedGameObject != null)
            {
                MoveObject();
            }
        }

        
        private void SelectObject(Transform obj)
        {
            HitTraget = obj.gameObject;

            if (m_hitTarget != null)
            {
                // Calculate the ray from the camera to the ground plane
                Ray ray = m_camera.ScreenPointToRay(Input.mousePosition);
              
                float hitDistance;

                if (m_groundPlane.Raycast(ray, out hitDistance))
                {
                    Vector3 hitPoint = ray.GetPoint(hitDistance);

                    // Calculate the offset between the selected object's position and the hit point
                    m_offset = SelectedGameObject.transform.position - hitPoint;
                }
            }

            BeginManipulate?.Invoke();
        }

        private void DeselectObject()
        {
            if (HitTraget != null)
            {
                EndManipulate.Invoke();
                HitTraget = null;
            }
        }

        private void MoveObject()
        {
            // Calculate the ray from the camera to the ground plane
            Ray ray = m_camera.ScreenPointToRay(Input.mousePosition);
            float hitDistance;

            if (m_groundPlane.Raycast(ray, out hitDistance))
            {
                Vector3 hitPoint = ray.GetPoint(hitDistance);

                // Ensure that the object stays on the ground plane
                SelectedGameObject.transform.position = hitPoint + m_offset;
            }
        }
    }
}
