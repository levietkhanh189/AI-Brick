using System.Collections;
using System.Collections.Generic;
using MVoxelizer;
using UnityEngine;

public class VoxelGroupController : MonoBehaviour
{
    public int levelCount;
    public int maxLevel;
    private VoxelGroup voxelGroup;
    private VoxelGroupByY voxelGroupByY;
    public Material defaultMaterial;
    public Material completeMaterial;
    public List<GroupManager> groupManagers;

    [Sirenix.OdinInspector.Button]
    public void Init(VoxelGroup voxelGroup,VoxelGroupByY voxelGroupByY)
    {
        this.voxelGroup = voxelGroup;
        this.voxelGroupByY = voxelGroupByY;
        this.defaultMaterial = voxelGroup.voxelMaterials[0];
        this.groupManagers = new List<GroupManager>();
        completeMaterial = Resources.Load<Material>("completeMaterial");
        List<GameObject> groupParents = voxelGroupByY.GetGroupParents();
        maxLevel = groupParents.Count;
        foreach (var item in groupParents)
        {
            GroupManager group = item.AddComponent<GroupManager>();
            group.Init(defaultMaterial, completeMaterial);
            groupManagers.Add(group);
        }
    }

    [Sirenix.OdinInspector.Button]
    public void Init()
    {
        List<GameObject> groupParents = GetChildObjects(gameObject);
        this.defaultMaterial = groupParents[0].GetComponentInChildren<MeshRenderer>().material;
        this.groupManagers = new List<GroupManager>();
        completeMaterial = Resources.Load<Material>("completeMaterial");
        maxLevel = groupParents.Count;
        foreach (var item in groupParents)
        {
            GroupManager group = item.AddComponent<GroupManager>();
            group.Init(defaultMaterial, completeMaterial);
            groupManagers.Add(group);
        }
    }

    List<GameObject> GetChildObjects(GameObject parent)
    {
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in parent.transform)
        {
            children.Add(child.gameObject);
        }
        return children;
    }

    public int CountVoxel()
    {
        return voxelGroupByY.CountVoxel();
    }

    [Sirenix.OdinInspector.Button]
    public bool LevelViewer(int level)
    {
       // if (level > maxLevel || level <0)
       //     return false;
        levelCount = level;
        for (int i = 0; i < maxLevel; i++)
        {
            if(i < levelCount)
            {
                groupManagers[i].SetCompleteMaterial();
            }
            else if (levelCount == i)
            {
                groupManagers[i].SetDefaultMaterial();
            }
            else
            {
                groupManagers[i].SetHideMaterial();
            }
        }
        return true;
    }

    [Sirenix.OdinInspector.Button]
    public void ViewNormal()
    {
        foreach (var item in groupManagers)
        {
            item.SetDefaultMaterial();
        }
        levelCount = 0;
    }

    [Sirenix.OdinInspector.Button]
    public bool NextLevelViewer()
    {
        return LevelViewer(levelCount + 1);
    }

    [Sirenix.OdinInspector.Button]
    public bool BackLevelViewer()
    {
        return LevelViewer(levelCount-1);
    }
}
