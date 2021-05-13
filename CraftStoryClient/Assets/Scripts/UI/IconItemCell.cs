using Gs2.Unity.Gs2Showcase.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconItemCell : UIBase
{
    Text Name;
    Text Count;

    private void InitUI()
    {
        Name = FindChiled<Text>("Name");
        Count = FindChiled<Text>("Count");
    }

    public void Add(string itemName, int count)
    {
        obj = this.gameObject;

        InitUI();

        Name.text = itemName;
        Count.text = "x" + count;
    }
}
