using UnityEngine;
using UnityEngine.UI;

public class BlueprintCell : UIBase
{
    Image Icon { get => GetComponent<Image>(); }
    Text Count { get => FindChiled<Text>("Text"); }

    public void Init(int itemId, int count)
    {
        Icon.sprite = ReadResources<Sprite>(ConfigMng.E.Item[itemId].IconResourcesPath);
        Count.text = "x" + count.ToString();
    }
}
