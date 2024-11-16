using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
public class CountVoxel : MonoBehaviour
{
    [Button]
    public int Count()
    {
        int result = transform.childCount;
        Debug.Log(this.gameObject.name + " has " + result + " voxels");
        return result;
    }
}
