using System;
using System.ComponentModel;
using System.IO;
using UnityEngine;
using UnityWeld.Binding;

namespace Battlehub.Storage.Samples
{
    [Binding]
    public class AssetViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Texture m_thumbnail;
        [Binding]
        public Texture Thumbnail
        {
            get { return m_thumbnail; }
            set
            {
                if (m_thumbnail != value)
                {
                    m_thumbnail = value;
                    RaisePropertyChanged(nameof(Thumbnail));
                }
            }
        }

        private string m_name;
        [Binding]
        public string Name
        {
            get { return m_name; }
            set
            {
                if (m_name != value)
                {
                    m_name = value;
                    RaisePropertyChanged(nameof(Name));
                    RaisePropertyChanged(nameof(DisplayName));
                }
            }
        }

        [Binding]
        public string DisplayName
        {
            get { return Path.GetFileNameWithoutExtension(Name); }
        }

        private IMeta<Guid, string> m_meta;
        public IMeta<Guid, string> Meta
        {
            get { return m_meta; }
            set { m_meta = value; }
        }

        public AssetViewModel(IMeta<Guid, string> meta, Texture thumbnail, string name)
        {
            m_name = name;
            m_thumbnail = thumbnail;
            m_meta = meta;
        }

        public void RaisePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
