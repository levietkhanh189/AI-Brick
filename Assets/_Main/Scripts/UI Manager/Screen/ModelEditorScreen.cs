using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ModelEditorScreen : DTNView
{
    public TextMeshProUGUI countText;
    public Slider slider;
    private VoxelGroupController voxelGroup;
    public TMP_InputField inputField;
    public Button quitButton;
    public Button editButton;
    public Button saveButton;
    public Button addButton;
    public Button backButton;

    public GameObject namePanel;

    private CountVoxel countVoxel;
    public override void Init()
    {
        editButton.onClick.AddListener(() => {
            if(voxelGroup != null)
            {
                MainController.Instance.VoxelizeMesh((int)slider.value);
                Hide();
            }
        });

        addButton.onClick.AddListener(() => {
            namePanel.gameObject.SetActive(true);
        });

        backButton.onClick.AddListener(() => {
            namePanel.gameObject.SetActive(false);
        });

        saveButton.onClick.AddListener(() => {
            if(inputField.text != "")
            {
                MainController.Instance.SaveBlockModel(voxelGroup.gameObject, inputField.text);
                Hide();
            }
        });

        quitButton.onClick.AddListener(() => {
            Hide();
        });
    }

    public override void Show()
    {
        MainController.Instance.MainScreenActive(false);
        base.Show();
    }

    public override void Hide()
    {
        MainController.Instance.MainScreenActive(true);
        base.Hide();
    }

    public void SetModel(VoxelGroupController voxelGroup)
    {
        this.voxelGroup = voxelGroup;
        countText.text = voxelGroup.CountVoxel() + " blocks";
    }
}
