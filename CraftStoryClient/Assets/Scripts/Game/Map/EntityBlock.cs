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

                var effect = EffectMng.E.AddEffect<EffectBase>(transform.position, EffectType.BlockDestroyEnd);
                effect.Init();
            });
        }, EConfig.ItemID, 1);
    }
}