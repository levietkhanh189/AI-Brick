﻿namespace DA_Assets.FCU
{
    public enum FcuTag
    {
        None = 0,

        Container = 1,
        Frame = 2,
        Page = 3,

        AutoLayoutGroup = 100,
        ContentSizeFitter = 101,
        AspectRatioFitter = 102,

        Text = 200,
        Image = 201,
        //Vector = 202,
        Background = 203,

        Button = 300,
        InputField = 301,
        Placeholder = 302,
        ScrollView = 303,
        PasswordField = 304,
        Toggle = 305,
        ToggleGroup = 306,

        BtnDefault = 400,
        BtnHover = 401,
        BtnPressed = 402,
        BtnSelected = 403,
        BtnDisabled = 404,
        BtnLooped = 405,

        Shadow = 500,
        CanvasGroup = 501,
        Mask = 502,
        Blur = 503,

        Ignore = 600,
    }
}