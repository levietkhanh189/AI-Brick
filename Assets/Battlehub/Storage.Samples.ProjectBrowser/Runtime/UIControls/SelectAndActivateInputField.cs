using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Battlehub.Storage.Samples
{
    public class SelectAndActivateInputField : MonoBehaviour
    {
        private void OnEnable()
        {
            StartCoroutine(CoActivate());
        }  

        private IEnumerator CoActivate()
        {
            yield return null;
            var inputField = GetComponent<InputField>();
            if (inputField != null)
            {
                inputField.ActivateInputField();
                inputField.Select();
            }
        }
    }
}
