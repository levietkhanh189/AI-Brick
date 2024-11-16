
using Battlehub.Storage;
using System;
using UnityEngine;
using UnityWeld.Binding;

namespace Battlehub.Storage.Samples
{
    [Binding]
    public class MeshRendererViewModel : ComponentViewModel
    {
        private ObservableList<AssetRefPropViewModel> m_sharedMaterials;

        [Binding]
        public ObservableList<AssetRefPropViewModel> SharedMaterials
        {
            get { return m_sharedMaterials; }
        }

        public MeshRendererViewModel(IAssetDatabase assetDatabase, Component component, Action<Component, string> onChange, Action<Component> onDelete) : base(component, onDelete)
        {
            var renderer = (Renderer)component;

            m_sharedMaterials = new ObservableList<AssetRefPropViewModel>();
            for (int i = 0; i < renderer.sharedMaterials.Length; ++i)
            {
                m_sharedMaterials.Add(new AssetRefPropViewModel(assetDatabase, component, nameof(MeshRenderer.sharedMaterials), i, (o, s) => onChange?.Invoke((Component)o, s)));
            }
        }
    }

}
