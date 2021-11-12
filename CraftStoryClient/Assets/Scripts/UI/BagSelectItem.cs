using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 持ち物の装備欄のサブ
/// </summary>
public class BagSelectItem : UIBase
{
    Image Icon;
    public int Index { get; set; }

    private ItemData itemData;

    private void Awake()
    {
        Icon = transform.GetComponent<Image>();
        transform.GetComponent<Button>().onClick.AddListener(OnClickSelectItem);
    }

    /// <summary>
    /// 更新
    /// </summary>
    public void Refresh()
    {
        itemData = DataMng.E.GetItemByEquipedSite(Index + 1);

        Icon.sprite = itemData == null
            ? ReadResources<Sprite>("Textures/icon_noimg")
            : ReadResources<Sprite>(itemData.Config().IconResourcesPath);

        if (itemData != null && !string.IsNullOrEmpty(itemData.textureName))
        {
            AWSS3Mng.E.DownLoadTexture2D(Icon, itemData.textureName);
        }
    }

    /// <summary>
    /// クリックした場合のイベント
    /// </summary>
    private void OnClickSelectItem()
    {
        if (BagLG.E.SelectItem == null)
            return;

        if (BagLG.E.SelectItem.ItemData.Config().CanEquip != 1)
            return;

        // 空の場合
        if (itemData == null)
        {
            NWMng.E.EquitItem((rp) =>
            {
                // 装備箇所更新
                BagLG.E.SelectItem.ItemData.equipSite = Index + 1;

                RefreshUIBtns();
            }, BagLG.E.SelectItem.ItemData.id, Index + 1);
        }
        // アイテムがある場合
        else
        {
            NWMng.E.EquitItem((rp) =>
            {
                NWMng.E.EquitItem((rp2) =>
                {
                    // 装備箇所更新
                    itemData.equipSite = 0;
                    BagLG.E.SelectItem.ItemData.equipSite = Index + 1;

                    RefreshUIBtns();
                }, BagLG.E.SelectItem.ItemData.id, Index + 1);
            }, itemData.id, 0);
        }

        GuideLG.E.Next();
    }

    /// <summary>
    /// UI アイテムボタンのデータを更新
    /// </summary>
    private void RefreshUIBtns()
    {
        // UI　更新
        if (BagLG.E.UI != null) BagLG.E.UI.RefreshSelectItemBtns();
        if (HomeLG.E.UI != null) HomeLG.E.UI.RefreshItemBtns();

        if (PlayerCtl.E.SelectItem != null &&
            Index + 1 == PlayerCtl.E.SelectItem.equipSite)
        {
            PlayerCtl.E.SelectItem = BagLG.E.SelectItem.ItemData;
        }

        BagLG.E.SelectItem = null;
    }
}
