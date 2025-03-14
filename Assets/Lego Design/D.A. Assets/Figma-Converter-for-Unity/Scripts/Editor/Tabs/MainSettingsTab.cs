﻿using DA_Assets.FCU.Extensions;
using DA_Assets.FCU.Model;
using DA_Assets.Shared;
using DA_Assets.Shared.Extensions;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DA_Assets.FCU
{
    internal class MainSettingsTab : ScriptableObjectBinder<FcuSettingsWindow, FigmaConverterUnity>
    {
        public void Draw()
        {
            gui.SectionHeader(FcuLocKey.label_main_settings.Localize());
            gui.Space15();

            DrawTokenField();

            gui.Space6();

            gui.EnumField(new GUIContent(FcuLocKey.label_ui_framework.Localize(), FcuLocKey.tooltip_ui_framework.Localize()),
                 monoBeh.Settings.MainSettings.UIFramework, onChange: (newValue) =>
                 {
                     monoBeh.Settings.MainSettings.UIFramework = newValue;
                     scriptableObject.CreateTabs();
                 });

            monoBeh.Settings.MainSettings.RawImport = gui.Toggle(
                new GUIContent(FcuLocKey.label_raw_import.Localize(), FcuLocKey.tooltip_raw_import.Localize()),
                monoBeh.Settings.MainSettings.RawImport);

            if (monoBeh.IsUITK() || monoBeh.IsDebug())
            {
#if UITK_LINKER_EXISTS
                monoBeh.Settings.MainSettings.UitkLinkingMode = gui.EnumField(
                  new GUIContent(FcuLocKey.label_uitk_linking_mode.Localize(), FcuLocKey.tooltip_uitk_linking_mode.Localize()),
                  monoBeh.Settings.MainSettings.UitkLinkingMode);
#endif

                monoBeh.Settings.MainSettings.UitkOutputPath = gui.DrawSelectPathField(
                    monoBeh.Settings.MainSettings.UitkOutputPath,
                    new GUIContent(FcuLocKey.label_uitk_output_path.Localize(), FcuLocKey.tooltip_uitk_output_path.Localize()),
                    new GUIContent(FcuLocKey.label_change.Localize()),
                    FcuLocKey.label_select_folder.Localize());

            }

            gui.Space15();

            monoBeh.Settings.MainSettings.ImageFormat = gui.EnumField(
                new GUIContent(FcuLocKey.label_images_format.Localize(), FcuLocKey.tooltip_images_format.Localize()),
                monoBeh.Settings.MainSettings.ImageFormat);

            monoBeh.Settings.MainSettings.ImageScale = gui.SliderField(
                 new GUIContent(FcuLocKey.label_images_scale.Localize(), FcuLocKey.tooltip_images_scale.Localize()),
                monoBeh.Settings.MainSettings.ImageScale, 0.25f, 4.0f).RoundToNearest025();

            monoBeh.Settings.MainSettings.PixelsPerUnit = gui.FloatField(
                 new GUIContent(FcuLocKey.label_pixels_per_unit.Localize(), FcuLocKey.tooltip_pixels_per_unit.Localize()),
                monoBeh.Settings.MainSettings.PixelsPerUnit);

            monoBeh.Settings.MainSettings.PreserveRatioMode = gui.EnumField(
                new GUIContent(FcuLocKey.label_preserve_ratio_mode.Localize(), FcuLocKey.tooltip_preserve_ratio_mode.Localize()),
                monoBeh.Settings.MainSettings.PreserveRatioMode, uppercase: false);

            monoBeh.Settings.MainSettings.RedownloadSprites = gui.Toggle(
                new GUIContent(FcuLocKey.label_redownload_sprites.Localize(), FcuLocKey.tooltip_redownload_sprites.Localize()),
                monoBeh.Settings.MainSettings.RedownloadSprites);

            monoBeh.Settings.MainSettings.DownloadMultipleFills = gui.Toggle(
                new GUIContent(FcuLocKey.label_download_multiple_fills.Localize(), FcuLocKey.tooltip_download_multiple_fills.Localize()),
                monoBeh.Settings.MainSettings.DownloadMultipleFills);

            monoBeh.Settings.MainSettings.DownloadUnsupportedGradients = gui.Toggle(
                new GUIContent(FcuLocKey.label_download_unsupported_gradients.Localize(), FcuLocKey.tooltip_download_unsupported_gradients.Localize()),
                monoBeh.Settings.MainSettings.DownloadUnsupportedGradients);

            monoBeh.Settings.MainSettings.SpritesPath = gui.DrawSelectPathField(
                monoBeh.Settings.MainSettings.SpritesPath,
                new GUIContent(FcuLocKey.label_sprites_path.Localize(), FcuLocKey.tooltip_sprites_path.Localize()),
                new GUIContent(FcuLocKey.label_change.Localize()),
                FcuLocKey.label_select_folder.Localize());

            gui.Space15();

            if (monoBeh.IsUGUI() || monoBeh.IsNova() || monoBeh.IsDebug())
            {
                monoBeh.Settings.MainSettings.GameObjectLayer = gui.LayerField(
                    new GUIContent(FcuLocKey.label_go_layer.Localize(), FcuLocKey.tooltip_go_layer.Localize()),
                    monoBeh.Settings.MainSettings.GameObjectLayer);
            }

            if (monoBeh.IsUGUI() || monoBeh.IsDebug())
            {
                monoBeh.Settings.MainSettings.PositioningMode = gui.EnumField(
                    new GUIContent(FcuLocKey.label_positioning_mode.Localize(), FcuLocKey.tooltip_positioning_mode.Localize()),
                    monoBeh.Settings.MainSettings.PositioningMode);

                monoBeh.Settings.MainSettings.PivotType = gui.EnumField(
                    new GUIContent(FcuLocKey.label_pivot_type.Localize(), FcuLocKey.tooltip_pivot_type.Localize()),
                    monoBeh.Settings.MainSettings.PivotType, uppercase: false);
            }

            gui.Space15();

            monoBeh.Settings.MainSettings.OverrideLetterSpacing = gui.Toggle(new GUIContent(FcuLocKey.label_override_tmp_letter_spacing.Localize(), FcuLocKey.tooltip_override_tmp_letter_spacing.Localize()),
                monoBeh.Settings.MainSettings.OverrideLetterSpacing);

            //monoBeh.Settings.MainSettings.OverrideLineSpacing = gui.Toggle(new GUIContent(FcuLocKey.label_override_line_spacing.Localize(), FcuLocKey.tooltip_override_line_spacing.Localize()),
            //monoBeh.Settings.MainSettings.OverrideLineSpacing);
        }

        private void DrawTokenField()
        {
            gui.DrawGroup(new Group
            {
                GroupType = GroupType.Horizontal,
                Body = () =>
                {
                    FigmaSessionItem session = monoBeh.FigmaSession.CurrentSession;

                    string token = gui.BigTextField(
                        monoBeh.FigmaSession.Token,
                        FcuLocKey.label_token.Localize(),
                        FcuLocKey.tooltip_token.Localize(),
                        password: true);

                    AuthResult authResult = session.AuthResult;
                    authResult.AccessToken = token;
                    session.AuthResult = authResult;
                    monoBeh.FigmaSession.CurrentSession = session;

                    gui.Space5();

                    var gr = new Group();
                    gr.Style = GuiStyle.Group2Buttons;

                    gr.GroupType = GroupType.Horizontal;
                    gr.Body = () =>
                    {
                        if (gui.SquareButton30x30(new GUIContent(gui.Resources.ImgViewRecent, FcuLocKey.tooltip_recent_tokens.Localize())))
                        {
                            ShowRecentSessionsPopup_OnClick();
                        }

                        gui.Space5();
                        if (gui.SquareButton30x30(new GUIContent(gui.Resources.IconAuth, FcuLocKey.tooltip_auth.Localize())))
                        {
                            monoBeh.EventHandlers.Auth_OnClick();
                        }
                    };
                    gui.DrawGroup(gr);

                }
            });

        }

        private void ShowRecentSessionsPopup_OnClick()
        {
            if (monoBeh.IsJsonNetExists() == false)
            {
                DALogger.LogError(FcuLocKey.log_cant_find_package.Localize(DAConstants.JsonNetPackageName));
                return;
            }

            List<FigmaSessionItem> recentSessions = monoBeh.FigmaSession.GetSessionItems();

            List<GUIContent> options = new List<GUIContent>();

            if (recentSessions.IsEmpty())
            {
                options.Add(new GUIContent(FcuLocKey.label_no_recent_sessions.Localize()));
            }
            else
            {
                foreach (FigmaSessionItem session in recentSessions)
                {
                    options.Add(new GUIContent($"{session.User.Name} | {session.User.Email}"));
                }
            }

            options.Add(new GUIContent($"Add new"));

            EditorUtility.DisplayCustomMenu(new Rect(11, 90, 0, 0), options.ToArray(), -1, (userData, ops, selected) =>
            {
                if (selected == options.Count - 1)
                {
                    monoBeh.FigmaSession.AddNew(new Model.AuthResult
                    {
                        AccessToken = monoBeh.FigmaSession.Token
                    });
                }
                else
                {
                    monoBeh.FigmaSession.CurrentSession = recentSessions[selected];
                }
            }, null);
        }
    }
}