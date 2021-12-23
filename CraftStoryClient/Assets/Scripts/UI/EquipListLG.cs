﻿using LitJson;
using System.Collections.Generic;
using System.Linq;

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
        get => mTagType;
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
        if (DataMng.E.MapData.Config.MapType != (int)MapType.Guide)
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
        else
        {
            foreach (var item in DataMng.E.GuideItems)
            {
                items.Add(new ItemEquipmentData(item));
            }
            RefreshCells();
            GuideLG.E.Next();
        }
    }

    public void RefreshCells()
    {
        // 何もない場合
        if (items == null || items.Count == 0)
        {
            ui.RefreshCells(null);
            return;
        }

        // ソート
        List<ItemEquipmentData> newItemList = new List<ItemEquipmentData>();
        IOrderedEnumerable<ItemEquipmentData> sortList;
        if (mSortUp)
        {
            sortList = items.OrderBy(rec => rec.equipmentConfig.RareLevel).OrderBy(rec => rec.itemId);
        }
        else
        {
            sortList = items.OrderByDescending(rec => rec.equipmentConfig.RareLevel).ThenByDescending(rec => rec.itemId);
        }

        foreach (var item in sortList)
        {
            // 指定したアイテムタイプではないとスキップ
            if ((ItemType)item.Config.Type != itemType)
                continue;

            // 同じタグじゃないとスキップ
            if (mTagType != TagType.All && (TagType)item.equipmentConfig.TagType != mTagType)
                continue;

            newItemList.Add(item);
        }

        UI.RefreshCells(newItemList);
    }

    public void AppraisalEquipment(EquipListCell cell)
    {
        if (DataMng.E.MapData.Config.MapType != (int)MapType.Guide)
        {
            NWMng.E.AppraisalEquipment((rp) =>
            {
                cell.AppraisalEquipment((string)rp);
            }, cell.Data.id, cell.Data.equipmentConfig.ID);
        }
        else
        {
            cell.AppraisalEquipment("107");
            GuideLG.E.Next();
        }
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