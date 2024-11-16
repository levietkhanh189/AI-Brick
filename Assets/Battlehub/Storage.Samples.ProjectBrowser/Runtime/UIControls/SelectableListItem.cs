using UnityEngine;
using UnityEngine.UI;

namespace Battlehub.Storage.Samples
{
    public class SelectableListItem : SelectableItem
    {
        [SerializeField]
        private ScrollRect m_scrollRect;

        public override void Select()
        {
            base.Select();

            Canvas.ForceUpdateCanvases();

            if (!IsTargetInViewport(transform))
            {
                SnapTo(transform);
            }
        }

        public override void Unselect()
        {
            base.Unselect();
        }

        public bool IsTargetInViewport(Transform target)
        {
            Vector3 targetWorldPos = target.position;

            RectTransform viewportRect = m_scrollRect.viewport;
            Vector3[] corners = new Vector3[4];
            viewportRect.GetWorldCorners(corners);

            return targetWorldPos.x >= corners[0].x && targetWorldPos.x <= corners[2].x &&
                targetWorldPos.y >= corners[0].y && targetWorldPos.y <= corners[2].y;
        }

        public void SnapTo(Transform target)
        {
            var tr = m_scrollRect.transform;
            var cp = m_scrollRect.content.position;
            var tp = target.position;
            var anchoredPos = tr.InverseTransformPoint(cp) - tr.InverseTransformPoint(tp);

            Vector3 ap = m_scrollRect.content.anchoredPosition;
            ap.y = anchoredPos.y;
            m_scrollRect.content.anchoredPosition = ap;      
        }

    }
}
