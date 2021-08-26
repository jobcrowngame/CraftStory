using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 持ち物Window用、サブ
/// </summary>
public class BagItemCell : UIBase
{
    Text itemName { get => FindChiled<Text>("Name"); }
    Text itemCount { get => FindChiled<Text>("Count"); }
    Image Icon { get => FindChiled<Image>("Icon"); }
    Button clickBtn { get => transform.GetComponent<Button>(); }
    Transform selected { get => FindChiled("Select"); }
    Transform Lock { get => FindChiled("Lock"); }

    /// <summary>
    /// アイテムデータ
    /// </summary>
    public ItemData ItemData { get => itemData; }
    private ItemData itemData;

    /// <summary>
    /// データをセット
    /// </summary>
    /// <param name="itemData">アイテムデータ</param>
    public void Set(ItemData itemData)
    {
        this.itemData = itemData;

        clickBtn.onClick.AddListener(() => 
        { 
            BagLG.E.SelectItem = this;
            GuideLG.E.Next();
        });

        Lock.gameObject.SetActive(itemData.islocked == 1);

        if (itemData == null)
            return;

        itemName.text = string.IsNullOrEmpty(ItemData.newName)
            ? itemData.Config().Name
            : itemData.newName;
        Icon.sprite = ReadResources<Sprite>(itemData.Config().IconResourcesPath);
        itemCount.text = "x" + itemData.count;
    }

    /// <summary>
    /// 選択されると選択フラグをアクティブ
    /// </summary>
    /// <param name="b">アクティブ</param>
    public void IsSelected(bool b)
    {
        selected.gameObject.SetActive(b);
    }
}