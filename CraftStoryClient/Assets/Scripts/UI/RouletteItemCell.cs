using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RouletteItemCell : UIBase
{
    Image Icon { get => FindChiled<Image>("Icon"); }
    Text Count { get => FindChiled<Text>("Text"); }
    public void Set(int itemId, int count)
    {
        Icon.sprite = ReadResources<Sprite>(ConfigMng.E.Item[itemId].IconResourcesPath);
        Count.text = "X" + count;
    }
}
