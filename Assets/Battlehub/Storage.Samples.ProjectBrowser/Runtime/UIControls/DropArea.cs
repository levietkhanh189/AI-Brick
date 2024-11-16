using System.Collections;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityWeld.Binding;

namespace Battlehub.Storage.Samples
{
    public class DropArea : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        private GameObject m_highlight = null;

        private object m_item;

        public UnityEvent DragEnter;
        public UnityEvent DragLeave;
        public UnityEvent Drop;        
        
        public UnityEvent DragSourceItemChanged;
        private object m_dragSourceItem;
        public object DragSourceItem
        {
            get { return m_dragSourceItem; }
            set
            {
                if (m_dragSourceItem != value)
                {
                    m_dragSourceItem = value;
                    DragSourceItemChanged.Invoke();
                }
            }
        }

        public UnityEvent DropTargetItemChanged;
        private object m_dropTargetItem;

        public object DropTargetItem
        {
            get { return m_dropTargetItem; }
            set
            {
                if (m_dropTargetItem != value)
                {
                    m_dropTargetItem = value;
                    DropTargetItemChanged.Invoke();
                }
            }
        }

        public bool CanDrop
        {
            get;
            set;
        }

        private void Start()
        {
            var template = GetComponent<Template>();
            if (template != null)
            {
                m_item = template.GetViewModel();
            }
            else
            {
                m_item = GetComponentInParent<INotifyPropertyChanged>();
            }
        }

        private void OnDisable()
        {
            Disable();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (DraggableItem.Active == null)
            {
                StartCoroutine(CoWaitForDraggableItemActivation());
                return;
            }

            HandleDragEnter();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Disable();
        }
 
        public void OnDrop(PointerEventData eventData)
        {
            if (DraggableItem.Active == null)
            {
                return;
            }

            if (CanDrop)
            {
                Drop?.Invoke();
                DragSourceItem = null;
            }

            ShowHighlight(false);
            DropTargetItem = null;
            DraggableItem.Active.EndDrag();
        }

        private IEnumerator CoWaitForDraggableItemActivation()
        {
            while (DraggableItem.Active == null)
            {
                yield return null;
            }

            HandleDragEnter();
        }

        private void HandleDragEnter()
        {
            StopAllCoroutines();
            CanDrop = true;
            DragSourceItem = DraggableItem.Active.DragItem;
            DropTargetItem = m_item;

            DragEnter?.Invoke();

            if (CanDrop)
            {
                ShowHighlight(true);
            }
        }

        private void Disable()
        {
            StopAllCoroutines();
            ShowHighlight(false);
            DragSourceItem = null;
            DropTargetItem = null;
        }

        private void ShowHighlight(bool visible)
        {
            if (m_highlight != null)
            {
                m_highlight.SetActive(visible);
            }
        }
    }
}
