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
        var newItemData = DataMng.E.GetItemByEquipedSite(Index + 1);
        if (newItemData == null)
        {
            Icon.sprite = ReadResources<Sprite>("Textures/icon_noimg");
            itemData = null;
        }
        else
        {
            // アイテムが変わった場合、Iconを更新
            if (itemData == null || newItemData.id != itemData.id)
            {

                if (!string.IsNullOrEmpty(newItemData.textureName))
                {
                    AWSS3Mng.E.DownLoadTexture2D(Icon, newItemData.textureName);
                }
                else
                {
                    Icon.sprite = ReadResources<Sprite>(newItemData.Config.IconResourcesPath);
                }

                itemData = newItemData;
            }
        }
    }

    /// <summary>
    /// クリックした場合のイベント
    /// </summary>
    private void OnClickSelectItem()
    {
        try
        {
            // 選択してるアイテムがない場合スキップ
            if (BagLG.E.SelectItem == null)
                return;

            // 装備出来ないアイテムならスキップ
            if (BagLG.E.SelectItem.ItemData.Config.CanEquip != 1)
                return;

            // 同じアイテムならスキップ
            if (itemData != null && BagLG.E.SelectItem.ItemData.id == itemData.id)
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
                    // 装備箇所更新
                    var item = DataMng.E.GetItemByGuid(itemData.id);
                    if (item != null)
                    {
                        item.equipSite = 0;
                    }

                    NWMng.E.EquitItem((rp2) =>
                    {
                        var item = DataMng.E.GetItemByGuid(BagLG.E.SelectItem.ItemData.id);
                        if (item != null)
                        {
                            item.equipSite = Index + 1;
                        }

                        RefreshUIBtns();
                    }, BagLG.E.SelectItem.ItemData.id, Index + 1);
                }, itemData.id, 0);
            }

            GuideLG.E.Next();
        }
        catch (System.Exception ex)
        {
            Logger.Error(ex);
        }
    }

    /// <summary>
    /// UI アイテムボタンのデータを更新
    /// </summary>
    private void RefreshUIBtns()
    {
        // UI　更新
        if (BagLG.E.UI != null) BagLG.E.UI.RefreshSelectItemBtns();
        if (HomeLG.E.UI != null) HomeLG.E.UI.RefreshItemBtns();

        HomeItemBtn.CurSelectBtn = null;
        BagLG.E.SelectItem = null;
    }
}
