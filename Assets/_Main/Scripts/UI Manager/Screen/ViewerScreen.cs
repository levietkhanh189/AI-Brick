using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewerScreen : DTNView
{
    private VoxelGroupController voxelGroup;
    public Button buildButton;
    public Button arView;

    public override void Init()
    {
        buildButton.onClick.AddListener(() =>
        {
            MainController.Instance.MainScreenActive(false);
            BuildingScreen buildingScreen = DTNWindow.FindTopWindow().ShowSubView<BuildingScreen>();
            buildingScreen.SetModel(voxelGroup);
        });

        arView.onClick.AddListener(() =>
        {
            SceneController.Instance.LoadScene(1);
        });
    }

    public void SetModel(VoxelGroupController voxelGroup)
    {
        this.voxelGroup = voxelGroup;
    }
}
