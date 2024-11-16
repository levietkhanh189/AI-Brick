using System;
using System.ComponentModel;
using UnityWeld.Binding;

namespace Battlehub.Storage.Samples
{
    public interface IDialog
    {
        string Text { get; }

        public void Show(string text, Action<bool> callback = null);

        public void Show(string text, string okText, string cancelText, Action<bool> callback);
    }

    [Binding]
    public class DialogViewModel : IDialog, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

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

        private string m_text;

        [Binding]
        public string Text
        {
            get { return m_text; }
            set
            {
                if (m_text != value)
                {
                    m_text = value;
                    RaisePropertyChanged(nameof(Text));
                }
            }
        }

        private string m_okText = "Ok";
        [Binding]
        public string OkText
        {
            get { return m_okText; }
            private set
            {
                if (m_okText != value)
                {
                    m_okText = value;
                    RaisePropertyChanged(nameof(OkText));
                }
            }
        }

        private string m_cancelText = "Cancel";
        [Binding]
        public string CancelText
        {
            get { return m_cancelText; }
            private set
            {
                if (m_cancelText != value)
                {
                    m_cancelText = value;
                    RaisePropertyChanged(nameof(CancelText));
                }
            }
        }

        private Action<bool> m_callback;

        public void Show(string text, Action<bool> callback = null)
        {
            IsVisible = true;
            Text = text;
            m_callback = callback;
        }

        public void Show(string text, string okText, string cancelText, Action<bool> callback)
        {
            IsVisible = true;
            Text = text;
            OkText = okText;
            CancelText = cancelText;
            m_callback = callback;
        }

        [Binding]
        public void OnOk()
        {
            IsVisible = false;
            m_callback?.Invoke(true);
            m_callback = null;
        }

        [Binding]
        public void OnCancel()
        {
            IsVisible = false;
            m_callback?.Invoke(false);
            m_callback = null;
        }

        public void RaisePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}