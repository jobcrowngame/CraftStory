using JsonConfigData;
using UnityEngine;
using UnityEngine.UI;

public class GachaBonusCell : UIBase
{
    Transform Item { get => FindChiled("Item"); }
    Text Count { get => FindChiled<Text>("Count"); }
    Image Icon { get => FindChiled<Image>("Icon"); }
    Image TreasureBoxIcon { get => FindChiled<Image>("TreasureBoxIcon"); }

    public void Add(Item item, int count, int rare)
    {
        Count.text = "x" + count;
        Icon.sprite = ReadResources<Sprite>(item.IconResourcesPath);
        TreasureBoxIcon.sprite = ReadResources<Sprite>("Textures/treasurebox" + rare);
    }

    public void ShowAnim()
    {
        GetComponent<Animator>().enabled = true;
    }
    public void ShowItem()
    {
        Item.gameObject.SetActive(true);
    }
}