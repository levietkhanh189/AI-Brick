using Battlehub.Storage;
using System;
using System.Linq;
using UnityEngine;
using UnityWeld.Binding;

namespace Battlehub.Storage.Samples
{
    [Binding]
    public class ComponentInfoViewModel
    {
        [Binding]
        public string Name
        {
            get;
            private set;
        }

        public Type ComponentType
        {
            get;
            private set;
        }

        public ComponentInfoViewModel(string name, Type componentType)
        {
            Name = name;
            ComponentType = componentType;
        }
    }

    [Binding]
    public class AddComponentViewModel : System.ComponentModel.INotifyPropertyChanged
    {
        private bool m_isVisible;

        [Binding]
        public bool IsVisible
        {
            get { return m_isVisible; }
            set
            {
                if (m_isVisible != value)
                {
                    m_isVisible = value;
                    RaisePropertyChanged(nameof(IsVisible));
                }
            }
        }

        private ObservableList<ComponentInfoViewModel> m_components = new ObservableList<ComponentInfoViewModel>();

        [Binding]
        public ObservableList<ComponentInfoViewModel> Components
        {
            get { return m_components; }
        }

        private ComponentInfoViewModel m_selectedComponent;

        [Binding]
        public object Selection
        {
            get { return m_selectedComponent; }
            set
            {
                if (m_selectedComponent != value)
                {
                    m_selectedComponent = value as ComponentInfoViewModel;
                    if (m_selectedComponent != null)
                    {
                        OnAddComponent(m_selectedComponent.ComponentType);
                        m_selectedComponent = null;
                        RaisePropertyChanged(nameof(Selection));
                    }
                }
            }
        }

        private Action<Type> OnAddComponent
        {
            get;
            set;
        }

        public AddComponentViewModel(IAssetDatabase assetDatabase = null, Action<Type> onAddComponent = null)
        {
            if (assetDatabase == null)
            {
                return;
            }

            var componentTypes = assetDatabase.GetSerializableTypes()
                .Where(t => typeof(Component).IsAssignableFrom(t))
                .Where(t => !typeof(Transform).IsAssignableFrom(t));

            foreach (var componentType in componentTypes) 
            {
                m_components.Add(new ComponentInfoViewModel(componentType.Name, componentType));
            }
            RaisePropertyChanged(nameof(Components));

            OnAddComponent = onAddComponent;
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName) 
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
    }
}