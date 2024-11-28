using System.Collections;
using MVoxelizer.MVRuntime;
using UnityEngine;
using System;

public class VoxelizerController : MonoBehaviour
{
    public GameObject sourceObject;
    private MeshVoxelizerRuntime voxelizer;
    public MeshVoxelizerSetting setting;
    public bool isProcess;
    void Start()
    {
    }

    [Sirenix.OdinInspector.Button]
    public void VoxelizeMesh(int subLevel = 27, Action<GameObject> action = null)
    {
        Debug.Log("VoxelizeMesh");
        if (isProcess)
            return;
        voxelizer = new MeshVoxelizerRuntime();

        voxelizer.sourceGameObject = sourceObject;
        setting.subdivisionLevel = subLevel;
        voxelizer.ApplySetting(setting);

        StartCoroutine(VoxelizeMeshCoroutine(action));
    }

    IEnumerator VoxelizeMeshCoroutine(Action<GameObject> action)
    {
        isProcess = true;
        Debug.Log("Start VoxelizeMeshCoroutine");

        yield return null;

        GameObject voxelizedObject = voxelizer.VoxelizeMesh();

        if (voxelizedObject != null)
        {
            voxelizedObject.transform.SetParent(transform);
            voxelizedObject.transform.position = Vector3.zero;

            yield return null;
            isProcess = false;
            action(voxelizedObject);
        }
    }
}
