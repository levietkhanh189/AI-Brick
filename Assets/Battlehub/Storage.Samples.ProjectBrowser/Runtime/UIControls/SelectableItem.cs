using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityWeld.Binding;

namespace Battlehub.Storage.Samples
{
    public class SelectableItem : MonoBehaviour, IPointerClickHandler, IPointerDownHandler
    {
        [SerializeField]
        private GameObject m_selection;

        [SerializeField]
        private bool m_selectOnPointerDown = true;

        private object m_item;

        public UnityEvent SelectedItemChanged;
        private object m_selectedItem;
        public object SelectItem
        {
            get { return m_selectedItem; }
            set
            {
                if (m_selectedItem != value)
                {
                    m_selectedItem = value;

                    if (m_item == null)
                    {
                        return;
                    }

                    if (m_selectedItem == m_item)
                    {
                        Select();
                    }
                    else
                    {
                        Unselect();
                    }

                    SelectedItemChanged.Invoke();
                }
            }
        }

        protected virtual void Start()
        {
            var template = GetComponent<Template>();
            if (template != null)
            {
                m_item = template.GetViewModel();
                if (m_selectedItem == m_item)
                {
                    Select();
                }
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (m_selectOnPointerDown)
            {
                return;
            }

            SelectItem = m_item;
        }

        public virtual void Select()
        {
            m_selection.SetActive(true);

            var selectableItems = transform.parent.GetComponentsInChildren<SelectableItem>(true);
            foreach (SelectableItem item in selectableItems)
            {
                if (item != this)
                {
                    item.Unselect();
                }
            }
        }

        public virtual void Unselect()
        {
            m_selection.SetActive(false);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!m_selectOnPointerDown)
            {
                return;
            }

            SelectItem = m_item;
        }
    }

}
