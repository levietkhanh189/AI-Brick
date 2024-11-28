using System.Collections;
using System.Collections.Generic;
using MVoxelizer;
using UnityEngine;

public class VoxelAssemblyController : MonoBehaviour
{
    public CountVoxel countVoxel;
    public VoxelGroupByY voxelGroupByY;
    public VoxelGroupController groupController;
    public VoxelMeshManager voxelMeshManager;
    public VoxelGroup voxelGroup;

    public void Awake()
    {
        countVoxel = gameObject.AddComponent<CountVoxel>();
        voxelGroupByY = gameObject.AddComponent<VoxelGroupByY>();
        groupController = gameObject.AddComponent<VoxelGroupController>();
        voxelMeshManager = gameObject.AddComponent<VoxelMeshManager>();
        voxelGroup = gameObject.GetComponent<VoxelGroup>();

        voxelGroupByY.Init();
        groupController.Init(voxelGroup, voxelGroupByY);
        voxelMeshManager.Init(voxelGroup);

    }
}
