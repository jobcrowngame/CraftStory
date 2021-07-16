using UnityEngine;

public class EntityBlock : EntityBase
{
    public override void ClickingEnd()
    {
        base.ClickingEnd();

        NWMng.E.AddItem((rp) =>
        {
            NWMng.E.GetItemList((rp2) =>
            {
                DataMng.GetItems(rp2);
                WorldMng.E.MapCtl.DeleteEntity(this);
            });
        }, EConfig.ItemID, 1);
    }
}