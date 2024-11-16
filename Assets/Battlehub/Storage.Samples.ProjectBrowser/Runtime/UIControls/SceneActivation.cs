using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Battlehub.Storage.Samples
{
    public class SceneActivation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public UnityEvent IsActiveChanged;

        private bool m_isActive;
        public bool IsActive
        {
            get { return m_isActive; }
            set
            {
                if (m_isActive != value)
                {
                    m_isActive = value;
                    IsActiveChanged?.Invoke();
                }
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            IsActive = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            IsActive = false;
        }
    }
}
