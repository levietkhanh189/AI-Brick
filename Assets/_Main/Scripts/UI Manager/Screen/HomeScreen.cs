using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeScreen : DTNView
{
    private BeginScreen beginScreen;
    private ViewerScreen viewerScreen;
    public VoxelGroupController voxelGroup;
    public override void Init()
    {
        beginScreen = ShowSubView<BeginScreen>();
        viewerScreen = ShowSubView<ViewerScreen>();
        viewerScreen.gameObject.SetActive(false);

        MainController.Instance.ConnectViewerScreen(UseViewerScreen);
    }

    public void UseViewerScreen(VoxelGroupController voxelGroup)
    {
        this.voxelGroup = voxelGroup;
        if (voxelGroup != null)
        {
            beginScreen.gameObject.SetActive(false);
            viewerScreen.gameObject.SetActive(true);

            viewerScreen.SetModel(voxelGroup);
        }
        else
        {
            beginScreen.gameObject.SetActive(true);
            viewerScreen.gameObject.SetActive(false);
        }
    }
}
