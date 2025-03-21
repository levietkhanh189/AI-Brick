﻿using DA_Assets.FCU.Model;
using DA_Assets.Shared;
using DA_Assets.Shared.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using System;


#if DABUTTON_EXISTS
using DA_Assets.DAB;
#endif

namespace DA_Assets.FCU.Drawers.CanvasDrawers
{
    [Serializable]
    public class DAButtonDrawer : MonoBehaviourBinder<FigmaConverterUnity>
    {
        public void SetupDaButton(SyncData btnSyncData)
        {
#if DABUTTON_EXISTS
            DAButton daButton = btnSyncData.GameObject.GetComponent<DAButton>();
            daButton.BlendMode = monoBeh.Settings.DabSettings.BlendMode;
            daButton.BlendIntensity = monoBeh.Settings.DabSettings.BlendIntensity;

            SyncHelper[] syncHelpers = btnSyncData.GameObject.GetComponentsInChildren<SyncHelper>(true).Skip(1).ToArray();
            Dictionary<int, List<SyncHelper>> graphics = new Dictionary<int, List<SyncHelper>>();

            foreach (SyncHelper syncHelper in syncHelpers)
            {
                bool exists = syncHelper.TryGetComponentSafe(out Graphic gr);

                if (!exists)
                    continue;

                bool success = TryGetNumberBeforeDash(syncHelper.Data.ObjectName, out int number);

                if (success)
                {
                    if (!graphics.ContainsKey(number))
                    {
                        graphics[number] = new List<SyncHelper>();
                    }

                    graphics[number].Add(syncHelper);
                }
            }

            daButton.TargetGraphics.Clear();

            foreach (var item in graphics)
            {
                DATargetGraphic tg = DAButtonDefaults.Instance.CopyTargetGraphic(monoBeh.Settings.DabSettings.DefaultTargetGraphic);

                foreach (SyncHelper sh in item.Value)
                {
                    sh.TryGetComponentSafe(out Graphic gr);

                    bool sprite = sh.gameObject.TryGetComponentSafe(out Image img) && img.sprite != null;

                    if (sh.Data.Tags.Contains(FcuTag.BtnDefault))
                    {
                        tg.TargetGraphic = gr;
                        AnimationItem n = tg.NormalState;

                        if (sprite)
                        {
                            TProp<Sprite> sp = n.Sprite;
                            sp.Value = img.sprite;
                            n.Sprite = sp;
                        }
                        else
                        {
                            TProp<Color> sp = n.Color;
                            if (daButton.BlendMode == DAG.DAColorBlendMode.Overlay)
                            {
                                sp.Value = gr.color;
                            }
                            else
                            {
                                sp.Value = Color.white;
                            }
                            n.Color = sp;
                        }

                        tg.NormalState = n;
                    }
                    else
                    {
                        if (sh.Data.Tags.Contains(FcuTag.BtnDisabled))
                        {
                            UpdateAnimationColor(DAPointerEvent.OnDisable);
                        }
                        else if (sh.Data.Tags.Contains(FcuTag.BtnHover))
                        {
                            UpdateAnimationColor(DAPointerEvent.OnPointerEnter);
                        }
                        else if (sh.Data.Tags.Contains(FcuTag.BtnPressed))
                        {
                            UpdateAnimationColor(DAPointerEvent.OnPointerClick);
                        }
                        else if (sh.Data.Tags.Contains(FcuTag.BtnLooped))
                        {
                            UpdateAnimationColor(DAPointerEvent.OnLoopStart);
                        }
                        else if (sh.Data.Tags.Contains(FcuTag.BtnSelected))
                        {
                            //TODO: add BtnSelected
                        }

                        void UpdateAnimationColor(DAPointerEvent pointerEvent)
                        {
                            for (int i = 0; i < tg.AnimationBlocks.Count; i++)
                            {
                                if (tg.AnimationBlocks[i].Event != pointerEvent)
                                    continue;

                                AnimationBlock animBlock = tg.AnimationBlocks[i];

                                for (int j = 0; j < animBlock.AnimationItems.Count; j++)
                                {
                                    AnimationItem tempItem = animBlock.AnimationItems[j];

                                    if (sprite)
                                    {
                                        TProp<Sprite> sp = tempItem.Sprite;
                                        sp.Value = img.sprite;
                                        tempItem.Sprite = sp;
                                    }
                                    else
                                    {
                                        TProp<Color> sp = tempItem.Color;
                                        sp.Value = gr.color;
                                        tempItem.Color = sp;
                                    }

                                    animBlock.AnimationItems[j] = tempItem;
                                }

                                tg.AnimationBlocks[i] = animBlock;
                            }
                        }

                        gr.gameObject.Destroy();
                    }
                }

                daButton.TargetGraphics.Add(tg);
            }
#endif
        }

        private bool TryGetNumberBeforeDash(string input, out int number)
        {
            string pattern = @"\d+(?=\s*-)";
            Match match = Regex.Match(input, pattern);

            if (match.Success)
            {
                number = int.Parse(match.Value);
                return true;
            }
            else
            {
                number = 0;
                return false;
            }
        }
    }
}
