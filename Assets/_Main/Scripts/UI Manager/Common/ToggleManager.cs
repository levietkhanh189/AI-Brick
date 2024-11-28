using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ToggleManager : MonoBehaviour
{
    public List<ToggleButton> toggleButtons;
    public void Init(Action<int> onClick)
    {
        foreach (var item in toggleButtons)
        {
            item.Init((int a)=> {
                onClick(a);
            });
        }
    }

    public void SetHighlight(int id)
    {
        foreach (var item in toggleButtons)
        {
            item.SetColor(item.id == id);
        }
    }
}
