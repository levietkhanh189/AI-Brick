using System.Collections;
using System.Collections.Generic;
using MVoxelizer;
using UnityEngine;

public class VoxelMeshManager : MonoBehaviour
{
    private VoxelGroup voxelGroup;

    public Mesh voxelMeshDefault;
    public Vector3 voxelScaleDefault = Vector3.one;
    public Vector3 voxelRotationDefault = Vector3.zero;

    public Mesh voxelMeshMiniBlock;
    public Vector3 voxelScaleMiniBlock = Vector3.one*50f;
    public Vector3 voxelRotationMiniBlock = new Vector3(-90f,0f,0f);

    public void Init(VoxelGroup voxelGroup)
    {
        this.voxelGroup = voxelGroup;
        voxelMeshDefault = Resources.Load<Mesh>("DefaultVoxelCube");
        voxelMeshMiniBlock = Resources.Load<Mesh>("MiniBlock");
    }

    [Sirenix.OdinInspector.Button]
    public void ApplyMeshDefault()
    {
        voxelGroup.voxelMesh = voxelMeshDefault;
        voxelGroup.voxelScale = voxelScaleDefault;
        voxelGroup.voxelRotation = voxelRotationDefault;
        voxelGroup.RebuildVoxels();
    }

    [Sirenix.OdinInspector.Button]
    public void ApplyMeshMiniBlock()
    {
        voxelGroup.voxelMesh = voxelMeshMiniBlock;
        voxelGroup.voxelScale = voxelScaleMiniBlock;
        voxelGroup.voxelRotation = voxelRotationMiniBlock;
        voxelGroup.RebuildVoxels();
    }
}

[System.Serializable]
public struct VoxelMeshStyle
{
    public Mesh voxelMesh;
    public Vector3 voxelScale;
    public Vector3 voxelRotation;
}