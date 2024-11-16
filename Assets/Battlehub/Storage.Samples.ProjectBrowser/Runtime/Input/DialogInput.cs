using UnityEngine;
using UnityEngine.Events;

namespace Battlehub.Storage.Samples
{
    public class DialogInput : MonoBehaviour
    {
        public UnityEvent Submit;

        public UnityEvent Close;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                Submit.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Close.Invoke();
            }
        }
    }
}
