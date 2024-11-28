using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainScreen : DTNView
{
    public ToggleManager toggleManager;
    private HomeScreen homeScreen;
    private LibraryScreen libraryScreen;

    public override void Init()
    {
        homeScreen = ShowSubView<HomeScreen>();
        libraryScreen = ShowSubView<LibraryScreen>();
        ScreenLoad(0);
        toggleManager.Init(ScreenLoad);
    }

    public void ScreenLoad(int id)
    {
        homeScreen.gameObject.SetActive(id == 0);
        libraryScreen.gameObject.SetActive(id == 1);
        toggleManager.SetHighlight(id);
    }
}
