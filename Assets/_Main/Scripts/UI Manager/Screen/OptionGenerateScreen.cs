using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionGenerateScreen : DTNView
{
    public Button TextToBlock2D;
    public Button TextToBlock3D;
    public Button ImageToBlock2D;
    public Button ImageToBlock3D;
    public Button QuitButton;

    public override void Init()
    {
        AddOnclickAction();
    }

    private void AddOnclickAction()
    {
        QuitButton.onClick.AddListener(() =>
        {
            Hide();
        });
        TextToBlock2D.onClick.AddListener(() =>
        {
            ChooseOption(OptionGenerate.TextToBlock2D);
        });
        TextToBlock3D.onClick.AddListener(() =>
        {
            ChooseOption(OptionGenerate.TextToBlock3D);
        });
        ImageToBlock2D.onClick.AddListener(() =>
        {
            ChooseOption(OptionGenerate.ImageToBlock2D);
        });
        ImageToBlock3D.onClick.AddListener(() =>
        {
            ChooseOption(OptionGenerate.ImageToBlock3D);
        });
    }

    public void ChooseOption(OptionGenerate option)
    {
        GenerateScreen generateScreen = DTNWindow.FindTopWindow().ShowSubView<GenerateScreen>();
        generateScreen.SetupOption(option);
        Hide();
    }
}

public enum OptionGenerate
{
    TextToBlock2D,
    TextToBlock3D,
    ImageToBlock2D,
    ImageToBlock3D
}