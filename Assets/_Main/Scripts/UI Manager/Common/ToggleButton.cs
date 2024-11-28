using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ToggleButton : MonoBehaviour
{
    public int id;
    public Color normalColor;
    public Color pressColor;
    public Image iconImage;
    public TextMeshProUGUI nameText;
    private Button button;

    public void Init(Action<int> onClick)
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(()=> {
            onClick(id);
        });
    }

    public void SetColor(bool isTrue)
    {
        iconImage.color = isTrue ? pressColor : normalColor;
        nameText.color = isTrue ? pressColor : normalColor;
    }
}
