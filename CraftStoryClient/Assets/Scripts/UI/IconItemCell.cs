using JsonConfigData;
using UnityEngine;
using UnityEngine.UI;

public class IconItemCell : UIBase
{
    Text Name;
    Text Count;
    Image Icon;

    private void InitUI()
    {
        Name = FindChiled<Text>("Name");
        Count = FindChiled<Text>("Count");
        Icon = FindChiled<Image>("Icon");
    }

    public void Add(Item item, int count)
    {
        InitUI();

        Name.text = item.Name;
        Count.text = "x" + count;
        Icon.sprite = ReadResources<Sprite>(item.IconResourcesPath);
    }
}
