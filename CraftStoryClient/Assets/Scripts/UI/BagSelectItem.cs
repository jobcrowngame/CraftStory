using UnityEngine;
using UnityEngine.UI;

public class BagSelectItem : UIBase
{
    Image Icon;
    public int index { get; set; }

    private void Awake()
    {
        Icon = transform.GetComponent<Image>();
        transform.GetComponent<Button>().onClick.AddListener(OnClickSelectItem);
    }

    public void SetItem(ItemData itemData)
    {
        Icon.sprite = itemData == null
            ? null
            : ReadResources<Sprite>(itemData.Config().IconResourcesPath);
    }

    private void OnClickSelectItem()
    {
        if (BagLG.E.SelectItem == null)
            return;

        NWMng.E.EquitItem((rp) => 
        {
            HomeLG.E.UI.RefreshItemBtns(index, BagLG.E.SelectItem.ItemData);
            BagLG.E.UI.RefreshSelectItemBtns(index, BagLG.E.SelectItem.ItemData);
            BagLG.E.SelectItem = null;
        }, BagLG.E.SelectItem.ItemData.id, index + 1);
    }
}
