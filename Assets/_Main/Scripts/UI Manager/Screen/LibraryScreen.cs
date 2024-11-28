using System.Collections;
using System.Collections.Generic;
using Battlehub.Storage.Brick;
using UnityEngine;
using UnityEngine.UI;

public class LibraryScreen : DTNView
{
    public BlockItem blockItem;
    public List<BlockItem> blockItems = new List<BlockItem>();
    public Button createModelButton;
    public Transform itemContent;

    public override void Init()
    {
        createModelButton.onClick.AddListener(() => {
            DTNWindow.FindTopWindow().ShowSubView<OptionGenerateScreen>();
        });
    }

    private void OnEnable()
    {
        LoadItem();
    }

    public void LoadItem()
    {
        foreach (var item in blockItems)
            DTNPoolingGameManager.Instance.DestroyObject(item.gameObject);
        blockItems = new List<BlockItem>();

        foreach (var item in FileManager.Instance.GetFileIds())
        {
            BlockItem block = DTNPoolingGameManager.Instance.GenerateObject(blockItem.gameObject, itemContent).GetComponent<BlockItem>();
            AssetUsage.Instance.LoadThumb(item.id,(bool value,Sprite sprite)=> {
                block.Init(item.id, sprite, ShowBlock);
            });
            blockItems.Add(block);
        }
    }

    public void ShowBlock(string id)
    {
        MainController.Instance.ShowBlockModel(id);
        MainController.Instance.LoadMainScreen(0);
    }
}
