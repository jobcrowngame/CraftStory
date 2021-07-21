using System;


public class EntityTorch : EntityBase
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

    public override void SetTouchType(DirectionType tType)
    {
        switch (tType)
        {
            case DirectionType.up: CommonFunction.FindChiledByName(transform, "Center").gameObject.SetActive(true); break;
            case DirectionType.foward: CommonFunction.FindChiledByName(transform, "Foword").gameObject.SetActive(true); break;
            case DirectionType.back: CommonFunction.FindChiledByName(transform, "Back").gameObject.SetActive(true); break;
            case DirectionType.right: CommonFunction.FindChiledByName(transform, "Right").gameObject.SetActive(true); break;
            case DirectionType.left: CommonFunction.FindChiledByName(transform, "Left").gameObject.SetActive(true); break;
        }
    }
}