using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace Battlehub.Storage.Samples
{
    public class SelectionHighlight : MonoBehaviour
    {
        public Camera Camera
        {
            get;
            set;
        }

        public Material Material
        {
            get;
            set;
        }

        private CommandBuffer m_commandBuffer;
        
        private void Start()
        {
            m_commandBuffer = new CommandBuffer();
            m_commandBuffer.name = "Outline";
            if (Material == null)
            {
                return;
            }

            const int maxRenderers = 50;
            /*
            static float Volume(in Bounds bounds)
            {
                var s = bounds.size;
                return Mathf.Max(s.x, 0.1f) * Mathf.Max(s.y, 0.1f) * Mathf.Max(s.z, 0.1f);
            }
            var renderers = gameObject.GetComponentsInChildren<Renderer>().OrderByDescending(r => Volume(r.bounds)).Take(maxRenderers);
            */
            var renderers = gameObject.GetComponentsInChildren<Renderer>();
            if (renderers.Length > maxRenderers)
            {
                return;
            }

            foreach (var rend in renderers)
            {
                for (int i = 0; i < rend.sharedMaterials.Length; i++)
                {
                    m_commandBuffer.DrawRenderer(rend, Material, i);
                }

                Camera.AddCommandBuffer(CameraEvent.AfterImageEffects, m_commandBuffer);
            }
        }

        private void OnDestroy()
        {
            if (Camera != null)
            {
                Camera.RemoveCommandBuffer(CameraEvent.AfterImageEffects, m_commandBuffer);
            }
            
        }
    }
}

