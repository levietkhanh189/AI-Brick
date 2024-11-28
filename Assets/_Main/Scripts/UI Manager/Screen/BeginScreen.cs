using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeginScreen : DTNView
{
    public Button libraryButton;
    public Button createModelButton;

    public override void Init()
    {
        AddOnclickButton();
    }

    public void AddOnclickButton()
    {
        libraryButton.onClick.AddListener(() => {
            MainController.Instance.LoadMainScreen(1);
        });

        createModelButton.onClick.AddListener(() => {
            DTNWindow.FindTopWindow().ShowSubView<OptionGenerateScreen>();
        });
    }
}
