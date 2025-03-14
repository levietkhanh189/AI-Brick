﻿using DA_Assets.FCU.Model;
using DA_Assets.Shared;
using DA_Assets.Shared.Extensions;
using DA_Assets.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if DABUTTON_EXISTS
using DA_Assets.DAB;
#endif

namespace DA_Assets.FCU.Drawers.CanvasDrawers
{
    [Serializable]
    public class ButtonDrawer : MonoBehaviourBinder<FigmaConverterUnity>
    {
        [SerializeField] List<FObject> buttons = new List<FObject>();
        public List<FObject> Buttons => buttons;

        public void Init()
        {
            buttons.Clear();
        }

        public void Draw(FObject fobject)
        {
            fobject.Data.ButtonComponent = monoBeh.Settings.ComponentSettings.ButtonComponent;

            switch (monoBeh.Settings.ComponentSettings.ButtonComponent)
            {
                case ButtonComponent.DAButton:
                    {
#if DABUTTON_EXISTS
                        fobject.Data.GameObject.TryAddComponent(out DAButton _);
#endif
                    }
                    break;
                case ButtonComponent.FcuButton:
                    {
                        fobject.Data.GameObject.TryAddComponent(out FcuButton _);
                    }
                    break;
                default:
                    {
                        fobject.Data.GameObject.TryAddComponent(out UnityEngine.UI.Button _);
                    }
                    break;
            }

            buttons.Add(fobject);
        }

        public IEnumerator SetTargetGraphics()
        {
            foreach (FObject fobject in buttons)
            {
                switch (fobject.Data.ButtonComponent)
                {
                    case ButtonComponent.FcuButton:
                        {
                            this.FcuButtonDrawer.SetupFcuButton(fobject.Data);
                        }
                        break;
                    case ButtonComponent.DAButton:
                        {
                            this.DAButtonDrawer.SetupDaButton(fobject.Data);
                        }
                        break;
                    default:
                        {
                            this.UnityButtonDrawer.SetupUnityButton(fobject.Data);
                        }
                        break;
                }

                yield return null;
            }
        }

        [SerializeField] DAButtonDrawer dabDrawer;
        [SerializeProperty(nameof(dabDrawer))]
        public DAButtonDrawer DAButtonDrawer => dabDrawer.SetMonoBehaviour(monoBeh);

        [SerializeField] FcuButtonDrawer fcubDrawer;
        [SerializeProperty(nameof(fcubDrawer))]
        public FcuButtonDrawer FcuButtonDrawer => fcubDrawer.SetMonoBehaviour(monoBeh);

        [SerializeField] UnityButtonDrawer ubDrawer;
        [SerializeProperty(nameof(ubDrawer))]
        public UnityButtonDrawer UnityButtonDrawer => ubDrawer.SetMonoBehaviour(monoBeh);
    }
}
