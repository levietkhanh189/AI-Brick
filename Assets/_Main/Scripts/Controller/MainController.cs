using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MainController : MonoBehaviour
{
    public static MainController Instance;
    private Action<VoxelGroupController> viewerModelAction;
    private MainScreen mainScreen;
    public ImageGenerator imageGenerator;
    public VoxelImage voxelImage;
    public VoxelizerController voxelizerController;
    public ModelLoader modelLoader;
    private VoxelAssemblyController voxelAssemblyController;
    private GameObject sourceObject;
    private LoadModelScreen loadModelScreen;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        mainScreen = DTNWindow.FindTopWindow().ShowSubView<MainScreen>();
    }

    public void ConnectViewerScreen(Action<VoxelGroupController> action)
    {
        viewerModelAction = action;
    }

    public void SetModel(VoxelGroupController voxelGroup)
    {
        if(viewerModelAction != null)
            viewerModelAction(voxelGroup);
    }

    public void LoadMainScreen(int id)
    {
        mainScreen.ScreenLoad(id);
    }
    [Sirenix.OdinInspector.Button]
    public void GenerateTextToBlock2D(string promt, int subLevel = 27)
    {
        loadModelScreen = DTNWindow.FindTopWindow().ShowSubView<LoadModelScreen>();

        imageGenerator.GenerateImage(promt, (string path,Texture2D texture) =>
        {
            GenerateImageToBlock2D(texture, subLevel);
        });
    }

    [Sirenix.OdinInspector.Button]
    public void GenerateTextToBlock3D(string promt, int subLevel = 27)
    {
        loadModelScreen = DTNWindow.FindTopWindow().ShowSubView<LoadModelScreen>();

        imageGenerator.GenerateImage(promt, (string path, Texture2D texture) =>
        {
            GenerateImageToBlock3D(path, subLevel);
        });
    }

    [Sirenix.OdinInspector.Button]
    public void GenerateImageToBlock2D(Texture2D texture = null, int subLevel = 27)
    {
        loadModelScreen = DTNWindow.FindTopWindow().ShowSubView<LoadModelScreen>();

        voxelImage.gameObject.SetActive(true);
        voxelImage.SetImage(texture);

        VoxelizeMesh(subLevel, voxelImage.gameObject);
    }

    [Sirenix.OdinInspector.Button]
    public void GenerateImageToBlock3D(string imgPath = "", int subLevel = 27)
    {
        loadModelScreen = DTNWindow.FindTopWindow().ShowSubView<LoadModelScreen>();

        modelLoader.GenerateModel(imgPath,(GameObject child)=> {
            VoxelizeMesh(subLevel, child);
        });
    }

    public void VoxelizeMesh(int subLevel,GameObject gameObject = null)
    {
        if (gameObject != null)
            this.sourceObject = gameObject;
        voxelizerController.sourceObject = sourceObject;
        voxelizerController.VoxelizeMesh(subLevel, AfterVoxel);
        sourceObject.SetActive(false);
    }

    public void AfterVoxel(GameObject gameObject)
    {
        voxelImage.gameObject.SetActive(false);

        if(voxelAssemblyController != null)
        {
            ///Tesst
            Destroy(voxelAssemblyController.gameObject);
        }

        voxelAssemblyController = gameObject.AddComponent<VoxelAssemblyController>();
        loadModelScreen.Hide();

        ModelEditorScreen modelEditorScreen = DTNWindow.FindTopWindow().ShowSubView<ModelEditorScreen>();
        modelEditorScreen.SetModel(voxelAssemblyController.groupController);
    }

    public void MainScreenActive(bool value)
    {
        mainScreen.gameObject.SetActive(value);
    }
}
