using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconItemCell : UIBase
{
    Text Name;
    Text Count;

    public void Add()
    {
        InitUI();
    }

    private void InitUI()
    {
        Name = FindChiled<Text>("Name");
        Count = FindChiled<Text>("Count");
    }
}
