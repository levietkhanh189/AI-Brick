using UnityEngine;

namespace Battlehub.Storage.Samples
{
    public class SceneController : MonoBehaviour
    {
        [SerializeField]
        private Camera m_camera;

        private Transform m_cameraTransform;

        [SerializeField]
        private Vector3 m_initalCameraPosition = new Vector3(0, 5, -5);

        [SerializeField]
        private float m_rotationSpeed = 3.0f;

        [SerializeField]
        private float m_zoomSpeed = 10.0f;

        [SerializeField]
        private Vector3 m_pivot;
        public Vector3 Pivot
        {
            get { return m_pivot; }
            set { m_pivot = value; }
        }

        private bool m_isSceneActive;
        public bool IsSceneActive
        {
            get { return m_isSceneActive; }
            set { m_isSceneActive = value; }
        }

        private bool m_rotating = false;
        private bool m_panning = false;
        private Plane m_panGroundPlane;
        private Vector3 m_prevMousePosition;
        private IEditorModel m_editor;
        
        private void Start()
        {
            m_editor = EditorModel.Instance;
            m_cameraTransform = m_camera.transform;

            Reset();
        }

        public void Reset()
        {
            if (m_camera != null)
            {
                m_camera.transform.position = m_initalCameraPosition;
                m_pivot = Vector3.zero;
                m_camera.transform.LookAt(m_pivot);
            }
        }

        private void Update()
        {
            if (!m_isSceneActive && !m_rotating && !m_panning)
            {
                return;
            }

            if (FocusAction())
            {
                Focus();
            }

            m_rotating = RotateAction();
            if (m_rotating)
            {
                float mouseX = PointerXAxis();
                float mouseY = PointerYAxis();

                RotateCamera(mouseX, mouseY);
            }

            if (BeginPanAction())
            {
                BeginPanCamera();
            }

            m_panning = PanAction();
            if (m_panning)
            {
                PanCamera();
            }

            ZoomCamera();
        }

        public void Focus()
        {
            GameObject go = m_editor.SelectedGameObject;
            if (go == null)
            {
                return;
            }

            Bounds bounds = TransformUtil.CalculateBounds(go.transform);
            float objSize = Mathf.Max(bounds.extents.y, bounds.extents.x, bounds.extents.z) * 3.0f;
            float fov = m_camera.fieldOfView * Mathf.Deg2Rad;
            float distance = Mathf.Abs(objSize / Mathf.Sin(fov / 2.0f));

            m_pivot = go.transform.position;
            m_camera.transform.position = m_pivot - m_camera.transform.forward * distance;
        }

        private void RotateCamera(float mouseX, float mouseY)
        {
            Vector3 rotation = new Vector3(mouseY * m_rotationSpeed, mouseX * m_rotationSpeed, 0);
            m_cameraTransform.RotateAround(m_pivot, Vector3.up, rotation.y);
            m_cameraTransform.RotateAround(m_pivot, m_cameraTransform.right, -rotation.x);
        }

        private void BeginPanCamera()
        {
            m_prevMousePosition = GetPointer();
            m_panGroundPlane = GetGroundPlane(m_camera.ScreenPointToRay(m_prevMousePosition));
        }

        private void PanCamera()
        {
            Vector3 mousePosition = GetPointer();
            if (Mathf.Abs(Vector3.Dot(Vector3.up, m_cameraTransform.forward)) > 0.33f)
            {
                Ray prevRay = m_camera.ScreenPointToRay(m_prevMousePosition);
                Ray ray = m_camera.ScreenPointToRay(mousePosition);

                if (GetPointOnGroundPlane(ray, m_panGroundPlane, out var hitPoint) && GetPointOnGroundPlane(prevRay, m_panGroundPlane, out var prevHitPoint))
                {
                    Vector3 offset = prevHitPoint - hitPoint;
                    m_pivot += offset;
                    m_cameraTransform.position += offset;
                }
            }
            else
            {
                Vector3 offset = m_prevMousePosition - mousePosition;
                offset = new Vector3(offset.x, 0, 0) * Time.deltaTime * 10;
                m_cameraTransform.Translate(offset, Space.Self);
            }

            m_prevMousePosition = mousePosition;
        }

        private void ZoomCamera()
        {
            float scrollInput = ZoomAxis();

            if (Mathf.Abs(scrollInput) > 0.0f)
            {
                float zoomOffset = scrollInput * m_zoomSpeed;

                Vector3 newPosition = m_cameraTransform.position + m_cameraTransform.forward * zoomOffset;

                m_cameraTransform.position = newPosition;
            }
        }

        public bool Raycast(out RaycastHit hit)
        {
            Ray ray = m_camera.ScreenPointToRay(GetPointer());
            return Physics.Raycast(ray, out hit, 1000, ~LayerMask.GetMask("Ignore Raycast"));
        }

        public Plane GetGroundPlane(Ray ray)
        {
            if (Physics.Raycast(ray, out RaycastHit hit, 1000, ~LayerMask.GetMask("Ignore Raycast")))
            {
                return new Plane(Vector3.up, hit.point);
            }

            return new Plane(Vector3.up, Vector3.zero);
        }

        public bool GetPointOnGroundPlane(out Vector3 point)
        {
            Ray ray = m_camera.ScreenPointToRay(GetPointer());
            return GetPointOnGroundPlane(ray, GetGroundPlane(ray), out point);
        }

        public bool GetPointOnGroundPlane(Ray ray, Plane groundPlane, out Vector3 point)
        {
            if (!groundPlane.Raycast(ray, out float hitDistance) || hitDistance > 300)
            {
                point = default;
                return false;
            }

            point = ray.GetPoint(hitDistance);
            return true;
        }

        private Vector3 GetPointer()
        {
            return Input.mousePosition;
        }

        private float ZoomAxis()
        {
            return Input.GetAxis("Mouse ScrollWheel");
        }

        private float PointerXAxis()
        {
            return Input.GetAxis("Mouse X");
        }

        private float PointerYAxis()
        {
            return Input.GetAxis("Mouse Y");
        }

        private bool RotateAction()
        {
            return Input.GetMouseButton(1);
        }

        private bool BeginPanAction()
        {
            return Input.GetMouseButtonDown(2);
        }

        private bool PanAction()
        {
            return Input.GetMouseButton(2);
        }

        private bool FocusAction()
        {
            return Input.GetKeyDown(KeyCode.F);
        }
    }
}
