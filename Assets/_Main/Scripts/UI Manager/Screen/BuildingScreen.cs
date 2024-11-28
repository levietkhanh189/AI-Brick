using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class BuildingScreen : DTNView
{
    public Button nextButton;
    public Button backButton;
    public Button quitButton;
    public TextMeshProUGUI stepText;
    private VoxelGroupController voxelGroup;

    public override void Init()
    {
        nextButton.onClick.AddListener(() =>
        {
            this.voxelGroup.NextLevelViewer();
            stepText.text = "Step " + voxelGroup.levelCount;
        });
        backButton.onClick.AddListener(() =>
        {
            this.voxelGroup.BackLevelViewer();
            stepText.text = "Step " + voxelGroup.levelCount;
        });

        quitButton.onClick.AddListener(() =>
        {
            this.voxelGroup.ViewNormal();
            MainController.Instance.MainScreenActive(true);
            Hide();
        });
    }
    public void SetModel(VoxelGroupController voxelGroup)
    {
        this.voxelGroup = voxelGroup;
        stepText.text = "Step " + voxelGroup.levelCount;
    }
}
