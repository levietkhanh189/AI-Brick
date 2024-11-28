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
    public Button quit;
    public Button edit;
    public Button save;

    private CountVoxel countVoxel;
    public override void Init()
    {
        edit.onClick.AddListener(() => {
            if(voxelGroup != null)
            {
                MainController.Instance.VoxelizeMesh((int)slider.value);
                Hide();
            }
        });

        save.onClick.AddListener(() => {
            MainController.Instance.SetModel(voxelGroup);
            Hide();
        });

        quit.onClick.AddListener(() => {
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

        /* countVoxel = voxelGroup.GetComponent<CountVoxel>();
         if(countVoxel == null)
             countVoxel = voxelGroup.gameObject.AddComponent<CountVoxel>();

         countText.text = countVoxel.Count() + " blocks";*/
        countText.text = "-- blocks";
    }
}
