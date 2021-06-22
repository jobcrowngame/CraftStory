using UnityEngine;
using UnityEngine.UI;

public class CraftCostCell : UIBase
{
    Image Icon;
    Text Count;

    private void Awake()
    {
        Icon = FindChiled<Image>("Icon");
        Count = FindChiled<Text>("Count");
    }

    public void SetInfo(int itemId, int count, int creatCount)
    {
        if (itemId < 0)
        {
            Count.text = "";
            Icon.sprite = ReadResources<Sprite>("Textures/icon_noimg");
        }
        else
        {
            Count.text = "x" + count * creatCount;
            Icon.sprite = ReadResources<Sprite>(ConfigMng.E.Item[itemId].IconResourcesPath);

            Count.color = DataMng.E.GetItemCountByItemID(itemId) < count * creatCount
                ? Color.red
                : Color.white;
        }
    }
}
