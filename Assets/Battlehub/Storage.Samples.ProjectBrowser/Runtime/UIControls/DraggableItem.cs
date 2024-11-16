using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityWeld.Binding;

namespace Battlehub.Storage.Samples
{
    public class DraggableItem : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField]
        private RectTransform m_cursor;

        private object m_item;

        public UnityEvent DragItemChanged;
        private object m_dragItem;
        public object DragItem
        {
            get { return m_dragItem; }
            set
            {
                if (m_dragItem != value)
                {
                    m_dragItem = value;
                    DragItemChanged.Invoke();
                }
            }
        }

        private bool m_canDrag = true;
        public bool CanDrag
        {
            get { return m_canDrag; }
            set { m_canDrag = value; }
        }

        public static DraggableItem Active
        {
            get;
            private set;
        }

        private CanvasScaler m_canvasScaler;

        private void Start()
        {
            var template = GetComponent<Template>();
            if (template != null)
            {
                m_item = template.GetViewModel();
            }

            m_canvasScaler = GetComponentInParent<CanvasScaler>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            eventData.Use();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.button != 0)
            {
                return;
            }

            if (!CanDrag)
            {
                return;
            }

            Active = this;
            DragItem = m_item;
            if (m_cursor != null)
            {
                m_cursor.gameObject.SetActive(true);
            }   
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (Active != this)
            {
                return;
            }

            float scaleFactor = m_canvasScaler.scaleFactor;
            m_cursor.anchoredPosition = eventData.position / scaleFactor;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            EndDrag();
        }

        public void EndDrag()
        {
            if (Active != this)
            {
                return;
            }

            Active = null;
            DragItem = null;
            if (m_cursor != null)
            {
                m_cursor.gameObject.SetActive(false);
            }
        }
    }
}
