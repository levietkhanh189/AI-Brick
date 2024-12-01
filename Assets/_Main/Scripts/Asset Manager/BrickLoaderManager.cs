using System.Collections;
using System.Collections.Generic;
using Battlehub.Storage.Brick;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class BrickLoaderManager : MonoBehaviour
{
    public BlockButton blockButton;
    private List<BlockButton> blockButtons;
    public Transform buttonContent;
    public ObjectSpawner objectSpawner;
    public ARTemplateMenuManager menuManager;
    private List<GameObject> blocks;
    private int indexCount;
    private void Start()
    {
        RespawnObjectForARShow();
    }

    private void RespawnObjectForARShow()
    {
        blockButtons = new List<BlockButton>();
        blocks = new List<GameObject>();
        indexCount = 0;
        foreach (var item in FileManager.Instance.GetFileIds())
        {
            BlockButton blockBtn = DTNPoolingGameManager.Instance.GenerateObject(blockButton.gameObject, buttonContent).GetComponent<BlockButton>();
            AssetUsage.Instance.LoadAsset(item.id, (bool value, GameObject block) => {
                block.transform.parent = this.transform;
                blocks.Add(block);
            });
            AssetUsage.Instance.LoadThumb(item.id, (bool value, Sprite sprite) => {
                blockBtn.Init(indexCount, sprite, BlockButtonClick);
            });
            indexCount++;
            blockButtons.Add(blockBtn);
        }

        StartCoroutine(SetBlockObjects());
    }

    private void BlockButtonClick(int index)
    {
        menuManager.SetObjectToSpawn(index);
        foreach (var item in blockButtons)
        {
            item.SelectionBox(index);
        }
    }


    IEnumerator SetBlockObjects()
    {
        while (blocks.Count < indexCount)
            yield return null;
        objectSpawner.SetListObject(blocks);
    }
}
