using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Battlehub.Storage.Samples
{
    public class DoubleClickableItem : MonoBehaviour, IPointerClickHandler
    {
        public UnityEvent DoubleClick;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.clickCount == 2)
            {
                DoubleClick?.Invoke();
            }
        }
    }
}
