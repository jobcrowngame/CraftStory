using UnityEngine;
using UnityEngine.UI;

public class ShopSubscriptionDetailsCell : UIBase
{
    Image Icon { get => FindChiled<Image>("Icon"); }
    Text Name { get => FindChiled<Text>("Name"); }
    Text Count { get => FindChiled<Text>("Count"); }

    public void Set(int itemId, int count)
    {
        Icon.sprite = ReadResources<Sprite>(ConfigMng.E.Item[itemId].IconResourcesPath);
        Name.text = ConfigMng.E.Item[itemId].Name;
        Count.text = "X" + count;
    }
}