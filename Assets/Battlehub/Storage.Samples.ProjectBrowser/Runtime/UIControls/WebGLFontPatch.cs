using UnityEngine;
#if UNITY_WEBGL
using UnityEngine.UI;
#endif

namespace Battlehub.Storage.Samples
{
    public class WebGLFontPatch : MonoBehaviour
    {
#if UNITY_WEBGL
        private void Awake()
        {
            foreach (Text text in GetComponentsInChildren<Text>(true))
            {
                text.fontStyle = FontStyle.Normal;
            }
        }
#endif
    }
}
