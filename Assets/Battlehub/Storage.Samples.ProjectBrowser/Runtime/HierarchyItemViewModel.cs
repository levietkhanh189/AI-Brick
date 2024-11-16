using System;
using System.ComponentModel;
using UnityEngine;
using UnityWeld.Binding;

namespace Battlehub.Storage.Samples
{
    [Binding]
    public class HierarchyItemViewModel : INotifyPropertyChanged
    {
        [Binding]
        public string Name
        {
            get { return Object.name; }
        }

        [Binding]
        public bool HasChildren
        {
            get { return Object.transform.childCount > 0; }
        }

        [Binding]
        public bool HasChildrenAndIsNotParent
        {
            get { return HasChildren && !IsParent; }
        }

        private bool m_isGameObject;
        [Binding]
        public bool IsGameObject
        {
            get { return m_isGameObject; }
            set
            {
                if (m_isGameObject != value)
                {
                    m_isGameObject = value;
                    RaisePropertyChanged(nameof(IsGameObject));
                }
            }
        }

        private bool m_isPrefab;
        [Binding]
        public bool IsPrefab
        {
            get { return m_isPrefab; }
            set
            {
                if (m_isPrefab != value)
                {
                    m_isPrefab = value;
                    RaisePropertyChanged(nameof(IsPrefab));
                }
            }
        }

        private bool m_isPrefabOrPrefabChild;
        [Binding]
        public bool IsPrefabOrPrefabChild
        {
            get { return m_isPrefabOrPrefabChild; }
            set
            {
                if (m_isPrefabOrPrefabChild != value)
                {
                    m_isPrefabOrPrefabChild = value;
                    RaisePropertyChanged(nameof(IsPrefabOrPrefabChild));
                }
            }
        }

        private bool m_isPrefabVariant;
        [Binding]
        public bool IsPrefabVariant
        {
            get { return m_isPrefabVariant; }
            set
            {
                if (m_isPrefabVariant != value)
                {
                    m_isPrefabVariant = value;
                    RaisePropertyChanged(nameof(IsPrefabVariant));
                }
            }
        }

        private bool m_isAddedPrefab;
        [Binding]
        public bool IsAddedPrefab
        {
            get { return m_isAddedPrefab; }
            set
            {
                if (m_isAddedPrefab != value)
                {
                    m_isAddedPrefab = value;
                    RaisePropertyChanged(nameof(IsAddedPrefab));
                }
            }
        }

        private bool m_isModifiedPrefab;
        [Binding]
        public bool IsModifiedPrefab
        {
            get { return m_isModifiedPrefab; }
            set
            {
                if (m_isModifiedPrefab != value)
                {
                    m_isModifiedPrefab = value;
                    RaisePropertyChanged(nameof(IsModifiedPrefab));
                }
            }
        }

        private bool m_canDrag;
        [Binding]
        public bool CanDrag
        {
            get { return m_canDrag; }
            set
            {
                if (m_canDrag != value)
                {
                    m_canDrag = value;
                    RaisePropertyChanged(nameof(CanDrag));
                }
            }
        }

        [Binding]
        public bool IsRoot
        {
            get;
            private set;
        }

        [Binding]
        public bool IsParent
        {
            get;
            private set;
        }

        [Binding]
        public bool IsParentAndIsNotRoot
        {
            get { return IsParent && !IsRoot; }
        }

        public GameObject Object
        {
            get;
            set;
        }

        private Action<HierarchyItemViewModel> OnClick
        {
            get;
            set;
        }

        public HierarchyItemViewModel(GameObject obj, Action<HierarchyItemViewModel> onClick, bool isParent = false, bool isRoot = false)
        {
            Object = obj;
            OnClick = onClick;
            IsParent = isParent;
            IsRoot = isRoot;
        }

        [Binding]
        public void Click()
        {
            OnClick?.Invoke(this);
        }

        public void Refresh()
        {
            RaisePropertyChanged(nameof(IsRoot));
            RaisePropertyChanged(nameof(IsParent));
            RaisePropertyChanged(nameof(IsParentAndIsNotRoot));

            RaisePropertyChanged(nameof(Name));
            RaisePropertyChanged(nameof(HasChildren));
            RaisePropertyChanged(nameof(HasChildrenAndIsNotParent));
            
            RaisePropertyChanged(nameof(IsPrefab));
            RaisePropertyChanged(nameof(IsPrefabOrPrefabChild));
            RaisePropertyChanged(nameof(IsPrefabVariant));
            RaisePropertyChanged(nameof(IsAddedPrefab));
            RaisePropertyChanged(nameof(IsModifiedPrefab));

        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
