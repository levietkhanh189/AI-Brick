using System.Collections.Generic;
using UnityEngine;

public class VoxelGroupByY : MonoBehaviour
{
    private List<GameObject> groupParents;
    public bool isGroup;

    public void Init()
    {
        GroupVoxelsByY();
    }

    [Sirenix.OdinInspector.Button]
    public List<GameObject> GetGroupParents()
    {
        if(isGroup)
            return groupParents;
        return null;
    }

    [Sirenix.OdinInspector.Button]
    public void GroupVoxelsByY()
    {
        if (isGroup)
            return;

        groupParents = new List<GameObject>();
        Dictionary<float, List<Transform>> yGroups = new Dictionary<float, List<Transform>>();

        // Iterate through each child of the GameObject this script is attached to
        foreach (Transform child in transform)
        {
            float y = child.position.y;

            // Check if the Y value already has a group
            if (!yGroups.ContainsKey(y))
            {
                yGroups[y] = new List<Transform>();
            }

            yGroups[y].Add(child);
        }

        // Create a parent object for each Y group and reparent the cubes
        foreach (var group in yGroups)
        {
            GameObject groupParent = new GameObject($"Level_Y_{group.Key}");
            groupParent.transform.parent = transform;
            foreach (Transform voxel in group.Value)
            {
                voxel.parent = groupParent.transform;
            }
            groupParents.Add(groupParent);
        }
        isGroup = true;
        Debug.Log("Voxel grouping by Y complete.");
    }
}
