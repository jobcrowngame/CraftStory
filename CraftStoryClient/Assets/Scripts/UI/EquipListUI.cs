using System.Collections.Generic;
using UnityEngine;

public class EquipListUI : UIBase
{
    TitleUI Title { get => FindChiled<TitleUI>("Title"); }
    Transform Parent { get => FindChiled("Parent"); }

    ItemType itemType;

    public override void Init(object data)
    {
        base.Init(data);

        EquipListLG.E.Init(this);
    }

    public override void Open(object data)
    {
        base.Open(data);

        ClearCell(Parent);

        itemType = (ItemType)data;

        Title.SetTitle("装備一覧");
        Title.SetOnClose(Close);

        EquipListLG.E.GetList();
    }

    public void RefreshCell(List<EquipListLG.EquipListRP> list)
    {
        if (list == null)
            return;

        foreach (var item in list)
        {
            var data = new ItemEquipmentData(item);
            if ((ItemType)data.Config().Type != itemType)
                continue;

            var cell = AddCell<EquipListCell>("Prefabs/UI/EquipListCell", Parent);
            cell.Set(data);
        }
    }
}
