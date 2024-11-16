using System;
using System.Reflection;
using UnityWeld.Binding;

namespace Battlehub.Storage.Samples
{
    [Binding]
    public class AssetRefPropViewModel : System.ComponentModel.INotifyPropertyChanged
    {
        private object m_target;
        private MemberInfo m_memberInfo;
        private int m_index;
        private IAssetDatabase m_assetDatabase;

        [Binding]
        public string Name
        {
            get
            {
                if (m_target == null)
                {
                    return null;
                }

                var obj = GetValue() as UnityEngine.Object;
                if (obj != null)
                {
                    return $"{obj.name} ({GetPropertyOrElementType().Name})";
                }

                if (ReferenceEquals(obj, null))
                {
                    return $"None  ({GetPropertyOrElementType().Name})";
                }

                return $"Missing Reference ({GetPropertyOrElementType().Name})";
            }
        }

        private Action<object, string> OnChange
        {
            get;
            set;
        }

        public AssetRefPropViewModel()
        {
        }

        public AssetRefPropViewModel(IAssetDatabase assetDatabase, object target, string propertyName, Action<object, string> onChange) :
            this(assetDatabase, target, propertyName, -1, onChange)
        {
        }


        public AssetRefPropViewModel(IAssetDatabase assetDatabase, object target, string propertyName, int index, Action<object, string> onChange)
        {
            m_target = target;
            m_assetDatabase = assetDatabase;

            var componentType = target.GetType();
            m_memberInfo = componentType.GetProperty(propertyName);
            if (m_memberInfo == null)
            {
                m_memberInfo = componentType.GetField(propertyName);
            }
            m_index = index;
            OnChange = onChange;
        }

        private AssetViewModel m_dragSourceAsset;
        [Binding]
        public object DragSourceAsset
        {
            get { return m_dragSourceAsset; }
            set
            {
                if (DragSourceAsset != value)
                {
                    m_dragSourceAsset = value as AssetViewModel;
                    RaisePropertyChanged(nameof(DragSourceAsset));
                    RaisePropertyChanged(nameof(CanDrop));
                }
            }
        }

        [Binding]
        public bool CanDrop
        {
            get { return m_dragSourceAsset != null && GetPropertyOrElementType().IsAssignableFrom(GetAssetType(m_dragSourceAsset.Meta.ID)); }
        }

        [Binding]
        public async void Drop()
        {
            var dragSourceAsset = m_dragSourceAsset;
            if (!m_assetDatabase.IsLoaded(dragSourceAsset.Meta.ID))
            {
                await m_assetDatabase.LoadAssetAsync(dragSourceAsset.Meta.ID);
            }

            SetValue(m_assetDatabase.GetAsset(dragSourceAsset.Meta.ID));
            RaisePropertyChanged(nameof(Name));
        }

        [Binding]
        public void Clear()
        {
            SetValue(null);

            RaisePropertyChanged(nameof(Name));
        }

        private Type GetAssetType(Guid id)
        {
            if (m_assetDatabase == null)
            {
                return null;
            }

            return m_assetDatabase.GetAssetType(id);
        }

        private Type GetPropertyOrElementType()
        {
            var propertyType = GetPropertyType();
            if (m_index >= 0 && propertyType.IsArray)
            {
                propertyType = propertyType.GetElementType();
            }

            return propertyType;
        }

        private Type GetPropertyType()
        {
            if (m_memberInfo is PropertyInfo)
            {
                var propertyInfo = (PropertyInfo)m_memberInfo;
                return propertyInfo.PropertyType;
            }

            var fieldInfo = (FieldInfo)m_memberInfo;
            return fieldInfo.FieldType;
        }

        private object GetValue()
        {
            object value = GetPropertyValue();
            if (m_index >= 0 && value is Array)
            {
                Array array = (Array)value;
                return array.GetValue(m_index);
            }

            return value;
        }

        private object GetPropertyValue()
        {
            if (m_memberInfo is PropertyInfo)
            {
                var propertyInfo = (PropertyInfo)m_memberInfo;
                return propertyInfo.GetValue(m_target);
            }

            var fieldInfo = (FieldInfo)m_memberInfo;
            return fieldInfo.GetValue(m_target);
        }

        private void SetValue(object value)
        {
            if (m_index >= 0)
            {
                Array array = GetPropertyValue() as Array;
                if (array != null)
                {
                    array.SetValue(value, m_index);
                    value = array;
                }
            }

            SetPropertyValue(value);
            OnChange?.Invoke(m_target, m_memberInfo.Name);
        }

        private void SetPropertyValue(object value)
        {
            if (m_memberInfo is PropertyInfo)
            {
                var propertyInfo = (PropertyInfo)m_memberInfo;
                propertyInfo.SetValue(m_target, value);
            }
            else
            {
                var fieldInfo = (FieldInfo)m_memberInfo;
                fieldInfo.SetValue(m_target, value);
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(name));
        }
    }
}

