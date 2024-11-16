using System;
using UnityEngine;
using UnityWeld.Binding;

namespace Battlehub.Storage.Samples
{
    [Binding]
    public class TransformViewModel : ComponentViewModel
    {
        [Binding]
        public float TX
        {
            get { return T.x; }
            set { T = new Vector3(value, T.y, T.z); }
        }

        [Binding]
        public float TY
        {
            get { return T.y; }
            set { T = new Vector3(T.x, value, T.z); }
        }

        [Binding]
        public float TZ
        {
            get { return T.z; }
            set { T = new Vector3(T.x, T.y, value); }
        }

        [Binding]
        public float RX
        {
            get { return R.x; }
            set { R = new Vector3(value, R.y, R.z); }
        }

        [Binding]
        public float RY
        {
            get { return R.y; }
            set { R = new Vector3(R.x, value, R.z); }
        }

        [Binding]
        public float RZ
        {
            get { return R.z; }
            set { R = new Vector3(R.x, R.y, value); }
        }

        [Binding]
        public float SX
        {
            get { return S.x; }
            set { S = new Vector3(value, S.y, S.z); }
        }

        [Binding]
        public float SY
        {
            get { return S.y; }
            set { S = new Vector3(S.x, value, S.z); }
        }

        [Binding]
        public float SZ
        {
            get { return S.z; }
            set { S = new Vector3(S.x, S.y, value); }
        }

        private Vector3 T
        {
            get { return TargetComponent.transform.localPosition; }
            set
            {
                if (TargetComponent.transform.localPosition != value)
                {
                    TargetComponent.transform.localPosition = value;
                    RaiseOnComponentChange(nameof(Transform.localPosition));
                }
            }
        }

        private Vector3 R
        {
            get { return TargetComponent.transform.localEulerAngles; }
            set
            {
                if (TargetComponent.transform.localEulerAngles != value)
                {
                    TargetComponent.transform.localEulerAngles = value;
                    RaiseOnComponentChange(nameof(Transform.localEulerAngles));
                }
            }
        }

        private Vector3 S
        {
            get { return TargetComponent.transform.localScale; }
            set
            {
                if (TargetComponent.transform.localScale != value)
                {
                    TargetComponent.transform.localScale = value;
                    RaiseOnComponentChange(nameof(Transform.localScale));
                }
            }
        }

        private Action<Component, string> OnChangeComponent
        {
            get;
            set;
        }

        public TransformViewModel(Component component, Action<Component, string> onChangeComponent) : base(component, o => { })
        {
            OnChangeComponent = onChangeComponent;
        }

        private void RaiseOnComponentChange(string propertyName)
        {
            OnChangeComponent?.Invoke(TargetComponent, propertyName);
        }
    }
}
