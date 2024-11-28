using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LibraryScreen : DTNView
{
    public Button createModelButton;

    public override void Init()
    {
        createModelButton.onClick.AddListener(() => {
            DTNWindow.FindTopWindow().ShowSubView<OptionGenerateScreen>();
        });
    }

}
