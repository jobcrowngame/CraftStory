using UnityEngine;
using UnityEngine.UI;

public class EmailDetailsItem : UIBase
{
    Image Icon { get => FindChiled<Image>("Icon"); }
    Text Count { get => FindChiled<Text>("Count"); }

    public void Set(string id, string count)
    {
        int itemId = int.Parse(id);

        Icon.sprite = ReadResources<Sprite>(ConfigMng.E.Item[itemId].IconResourcesPath);
        Count.text = "X" + count;
    }
}