using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelImage : MonoBehaviour
{
    public Material material;

    [Sirenix.OdinInspector.Button]
    public void SetImage(Texture2D texture2D)
    {
        material.mainTexture = texture2D;
    }
}
