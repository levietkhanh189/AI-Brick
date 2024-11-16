using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "DataPaths", menuName = "ScriptableObjects/DataPaths", order = 1)]
public class DataPathsScriptableObject : ScriptableObject
{
    public List<string> ImagePaths = new List<string>();
    public List<string> ModelPaths = new List<string>();

    [Sirenix.OdinInspector.Button]
    public void LoadDatas()
    {
        DataPaths dataPaths = DataStorage.Instance.GetDataPaths();
        ImagePaths = dataPaths.ImagePaths;
        ModelPaths = dataPaths.ModelPaths;
    }
}
