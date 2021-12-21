using JsonConfigData;
using UnityEngine;
using UnityEngine.UI;

public class HomeItemBtn : UIBase
{
    private static HomeItemBtn curSelectBtn;
    public static HomeItemBtn CurSelectBtn
    {
        get => curSelectBtn;
        set
        {
            if (curSelectBtn == value)
            {
                if(curSelectBtn != null) curSelectBtn.OnSelected(false);
                curSelectBtn = null;
            }
            else
            {
                if (curSelectBtn != null) curSelectBtn.OnSelected(false);

                curSelectBtn = value;
                if (curSelectBtn != null)
                {
                    curSelectBtn.OnSelected();
                    GuideLG.E.Next();
                }
            }

            PlayerCtl.E.ChangeSelectItem(curSelectBtn == null
                ? null
                : curSelectBtn.itemData);
        }
    }

    Image Icon { get => FindChiled<Image>("Icon"); }
    Text Count { get => FindChiled<Text>("Count"); }
    Transform Selection { get => FindChiled("Selection"); }

    Button btn;

    private ItemData itemData;

    public int Index { get; set; }

    private void Awake()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(OnClick);

        OnSelected(false);
    }
   

    private void OnClick()
    {
        if (itemData == null)
            return;

        // 食べ物の場合、選択しなく直接に食べる
        if (itemData.Type == ItemType.Food)
        {
            PlayerCtl.E.EatFood(itemData.itemId);

            HomeLG.E.UI.RefreshItemBtns();
            return;
        }

        CurSelectBtn = this;
    }
    private void OnSelected(bool b = true)
    {
        Selection.gameObject.SetActive(b);
    }

    public void Refresh()
    {
        var newItemData = DataMng.E.GetItemByEquipedSite(Index + 1);

        if (CurSelectBtn == this && CurSelectBtn.itemData == null)
            CurSelectBtn = null;

        if (newItemData == null)
        {
            Icon.sprite = ReadResources<Sprite>("Textures/icon_noimg");
            Count.text = "";
            OnSelected(false);
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

            Count.text = newItemData.Config.MaxCount == 1 ? "" : "x" + newItemData.count;
        }
    }
}