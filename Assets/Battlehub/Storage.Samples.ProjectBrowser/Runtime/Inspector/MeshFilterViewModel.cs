using Battlehub.Storage;
using System;
using UnityEngine;
using UnityWeld.Binding;

namespace Battlehub.Storage.Samples
{
    [Binding]
    public class MeshFilterViewModel : ComponentViewModel
    {
        private AssetRefPropViewModel m_sharedMesh;

        [Binding]
        public AssetRefPropViewModel SharedMesh
        {
            get { return m_sharedMesh; }
        }

        public MeshFilterViewModel(IAssetDatabase assetDatabase, Component component, Action<Component, string> onChange, Action<Component> onDelete) : base(component, onDelete) 
        {
           m_sharedMesh = new AssetRefPropViewModel(assetDatabase, component, nameof(MeshFilter.sharedMesh), (o, s) => onChange?.Invoke((Component)o, s));
        }
    }
}
