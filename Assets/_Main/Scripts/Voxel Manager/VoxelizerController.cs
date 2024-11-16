using System.Collections;
using MVoxelizer.MVRuntime;
using UnityEngine;

public class VoxelizerController : MonoBehaviour
{
    public GameObject sourceObject;
    private MeshVoxelizerRuntime voxelizer;
    public MeshVoxelizerSetting setting;
    void Start()
    {
        voxelizer = new MeshVoxelizerRuntime();
    }

    [Sirenix.OdinInspector.Button]
    public void VoxelizeMesh()
    {
        voxelizer.sourceGameObject = sourceObject;
        voxelizer.ApplySetting(setting);

        StartCoroutine(VoxelizeMeshCoroutine());
    }

    IEnumerator VoxelizeMeshCoroutine()
    {
        yield return new WaitForEndOfFrame();

        GameObject voxelizedObject = voxelizer.VoxelizeMesh();

        if (voxelizedObject != null)
        {
            voxelizedObject.transform.SetParent(transform);
        }
    }
}
