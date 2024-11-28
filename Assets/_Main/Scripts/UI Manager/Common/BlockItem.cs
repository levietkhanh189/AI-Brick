using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class BlockItem : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public Button button;
    public Image image;

    public void Init(string name,Sprite sprite, Action<string> action)
    {
        nameText.text = name;
        image.sprite = sprite;

        button.onClick.AddListener(() => { action(name); });
    }
}
