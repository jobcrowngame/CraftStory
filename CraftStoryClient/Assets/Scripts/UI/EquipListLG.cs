using LitJson;
using System.Collections.Generic;

public class EquipListLG : UILogicBase<EquipListLG, EquipListUI>
{
    List<ItemEquipmentData> items = new List<ItemEquipmentData>();

    /// <summary>
    /// ソート
    /// </summary>
    public bool SortUp
    {
        set
        {
            mSortUp = value;
            RefreshCells();
        }
        get => mSortUp;
    }
    private bool mSortUp;

    /// <summary>
    /// タグ
    /// </summary>
    public TagType Tag
    {
        set
        {
            mTagType = value;
            RefreshCells();
        }
    }
    private TagType mTagType = TagType.All;

    /// <summary>
    /// アイテムタイプ
    /// </summary>
    public ItemType itemType;

    /// <summary>
    /// 選択した装備
    /// </summary>
    public EquipListCell SelectedItem
    {
        get => mSelectedItem;
        set => mSelectedItem = value;
    }
    private EquipListCell mSelectedItem;

    public void GetEquipmentList()
    {
        NWMng.E.GetEquipmentInfoList((rp) => 
        {
            if (!string.IsNullOrEmpty(rp.ToString()))
            {
                var result = JsonMapper.ToObject<List<EquipListRP>>(rp.ToJson());
                items.Clear();
                foreach (var item in result)
                {
                    items.Add(new ItemEquipmentData(item));
                }
            }

            RefreshCells();
        });
    }

    public void RefreshCells()
    {
        // 何もない場合
        if (items == null || items.Count == 0)
        {
            ui.RefreshCells(null);
            return;
        }

        List<ItemEquipmentData> newItemList = new List<ItemEquipmentData>();

        items.Sort(delegate (ItemEquipmentData x, ItemEquipmentData y) {
            if (x.itemId == y.itemId) return 0;
            else if (x.itemId > y.itemId) return 1;
            else return -1;
        });

        if (mSortUp)
        {
            items.Sort(delegate (ItemEquipmentData x, ItemEquipmentData y)
            {
                if (x.equipmentConfig.RareLevel == y.equipmentConfig.RareLevel) return 0;
                else if (x.equipmentConfig.RareLevel > y.equipmentConfig.RareLevel) return 1;
                else return -1;
            });
        }
        else
        {
            items.Sort(delegate (ItemEquipmentData x, ItemEquipmentData y)
            {
                if (x.equipmentConfig.RareLevel == y.equipmentConfig.RareLevel) return 0;
                else if (x.equipmentConfig.RareLevel < y.equipmentConfig.RareLevel) return 1;
                else return -1;
            });
        }

        foreach (var item in items)
        {
            // 指定したアイテムタイプではないとスキップ
            if ((ItemType)item.Config().Type != itemType)
                continue;

            // 同じタグちゃないとスキップ
            if (mTagType != TagType.All && (TagType)item.equipmentConfig.TagType != mTagType)
                continue;

            newItemList.Add(item);
        }

        UI.RefreshCells(newItemList);
    }

    public void AppraisalEquipment(EquipListCell cell)
    {
        NWMng.E.AppraisalEquipment((rp) => 
        {
            cell.AppraisalEquipment((string)rp);
        }, cell.Data.id, cell.Data.equipmentConfig.ID);
    }

    public struct EquipListRP
    {
        public int id { get; set; }
        public int itemId { get; set; }
        public int islocked { get; set; }
        public string skills { get; set; }
    }

    public enum TagType
    {
        All,
        Sword,
        Wand,
        Bow,
    }
}