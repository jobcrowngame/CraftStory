using UnityEngine;
using UnityEngine.UI;

public class ShopSubscriptionItemCell : UIBase
{
    Image Icon { get => GetComponent<Image>(); }
    Text Count { get => FindChiled<Text>("Count"); }

    public void Set(int itemId, int count)
    {
        Icon.sprite = ReadResources<Sprite>(ConfigMng.E.Item[itemId].IconResourcesPath);
        Count.text = "X" + count;
    }
}