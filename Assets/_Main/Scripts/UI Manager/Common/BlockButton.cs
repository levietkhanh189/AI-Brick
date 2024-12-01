using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class BlockButton : MonoBehaviour
{
    public Button button;
    public Image image;
    public GameObject selectionBox;
    private int index;

    public void Init(int index,Sprite sprite, Action<int> action)
    {
        this.index = index;
        image.sprite = sprite;
        button.onClick.AddListener(()=> {
            action(this.index);
        });
    }

    public void SelectionBox(int index)
    {
        selectionBox.SetActive(this.index == index);
    }
}
