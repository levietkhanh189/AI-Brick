using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Battlehub.Storage.Samples
{
    public class EditorInput : MonoBehaviour
    {
        public UnityEvent Duplicate;

        public UnityEvent Cut;

        public UnityEvent Copy;

        public UnityEvent Paste;

        public UnityEvent Delete;

        public UnityEvent GoBack;

        private KeyCode Modifier
        {
            get
            {
#if UNITY_EDITOR
                return KeyCode.LeftShift;
#else
                return KeyCode.LeftControl;
#endif
            }
        }

        private GameObject m_selectedGameObject;
        private InputField m_selectedInputField;

        private void Update()
        {
            if (EventSystem.current != null)
            {
                if (m_selectedGameObject != EventSystem.current.currentSelectedGameObject)
                {
                    m_selectedGameObject = EventSystem.current.currentSelectedGameObject;
                    if (m_selectedGameObject != null)
                    {
                        m_selectedInputField = m_selectedGameObject.GetComponent<InputField>();
                    }
                    else
                    {
                        m_selectedInputField = null;
                    }
                }
            }

            if (m_selectedInputField != null && m_selectedInputField.isFocused)
            {
                return;
            }

            var modifier = Input.GetKey(Modifier);
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                Delete?.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                GoBack?.Invoke();
            }

            if (modifier)
            {
                if (Input.GetKeyDown(KeyCode.D))
                {
                    Duplicate?.Invoke();
                }

                if (Input.GetKeyDown(KeyCode.C))
                {
                    Copy?.Invoke();
                }

                if (Input.GetKeyDown(KeyCode.X))
                {
                    Cut?.Invoke();
                }

                if (Input.GetKeyDown(KeyCode.V))
                {
                    Paste?.Invoke();
                }
            }
        }
    }
}
