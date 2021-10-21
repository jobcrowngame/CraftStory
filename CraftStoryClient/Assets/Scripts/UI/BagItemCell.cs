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
    Button EquipBtn { get => FindChiled<Button>("EquipBtn"); }
    Transform selected { get => FindChiled("Select"); }
    Transform Lock { get => FindChiled("Lock"); }
    Transform Equiped { get => FindChiled("Equiped"); }

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

        EquipBtn.onClick.AddListener(() =>
        {
            PlayerCtl.E.EquipEquipment(ItemData);
        });

        Lock.gameObject.SetActive(itemData.islocked == 1);

        if (itemData == null)
            return;

        itemName.text = string.IsNullOrEmpty(ItemData.newName)
            ? itemData.Config().Name
            : itemData.newName;
        Icon.sprite = ReadResources<Sprite>(itemData.Config().IconResourcesPath);
        itemCount.text = "x" + itemData.count;

        if (itemData.Config().Type == (int)ItemType.Blueprint && !string.IsNullOrEmpty(itemData.textureName))
        {
            AWSS3Mng.E.DownLoadTexture2D(Icon, itemData.textureName);
        }

        if (CommonFunction.IsEquipment(itemData.itemId))
            Equiped.gameObject.SetActive(itemData.equipSite > 0);
    }

    /// <summary>
    /// 選択されると選択フラグをアクティブ
    /// </summary>
    /// <param name="b">アクティブ</param>
    public void IsSelected(bool b)
    {
        selected.gameObject.SetActive(b);

        if (CommonFunction.IsEquipment(ItemData.itemId))
        {
            EquipBtn.gameObject.SetActive(b);
        }
    }
}