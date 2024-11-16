using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Battlehub.Storage.Samples
{
    public class BusyIndicatior : MonoBehaviour
    {
        [SerializeField]
        private Text m_text;

        private string m_prevText;

        private WaitForSeconds m_waitForSeconds;

        private IEnumerator m_coAnimate;

        private void OnEnable()
        {
            m_waitForSeconds = new WaitForSeconds(0.15f);
            Reset();

            m_coAnimate = CoAnimate();

            StartCoroutine(m_coAnimate);
        }

        private void OnDisable()
        {
            StopCoroutine(m_coAnimate);
            m_coAnimate = null;
        }

        private void Reset()
        {
            m_text.text = m_text.text.TrimEnd('.');
            m_text.text = m_text.text.TrimEnd('\n');
            m_text.text += '\n';
            m_prevText = m_text.text;
        }

        private IEnumerator CoAnimate()
        {
            while (true)
            {
                string text = m_text.text;
                if (m_prevText != text || text.EndsWith("......."))
                {
                    Reset();
                }
                else
                {
                    m_text.text += ".";
                }
                m_prevText = m_text.text;
                yield return m_waitForSeconds;
            }
        }
    }

}
