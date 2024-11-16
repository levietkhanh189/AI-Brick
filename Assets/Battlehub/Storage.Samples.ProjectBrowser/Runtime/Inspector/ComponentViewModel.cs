using System;
using UnityEngine;
using UnityWeld.Binding;

namespace Battlehub.Storage.Samples
{
    [Binding]
    public class ComponentViewModel
    {
        [Binding]
        public string Name
        {
            get { return TargetComponent.GetType().Name; }
        }

        public Component TargetComponent
        {
            get;
            private set;
        }

        private Action<Component> OnDeleteCallback
        {
            get;
            set;
        }

        public ComponentViewModel(Component targetComponent, Action<Component> onDelete)
        {
            TargetComponent = targetComponent;
            OnDeleteCallback = onDelete;
        }

        [Binding]
        public void Delete()
        {
            OnDeleteCallback(TargetComponent);
        }
    }
}
