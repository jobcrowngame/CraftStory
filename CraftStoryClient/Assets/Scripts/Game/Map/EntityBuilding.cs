

using UnityEngine;

public class EntityBuilding : EntityBase
{
    private bool DoorIsClosed
    {
        get => doorIsClosed;
        set
        {
            doorIsClosed = value;

            var collider = GetComponent<BoxCollider>();
            if (collider != null)
            {
                collider.isTrigger = !value;
            }
               
            var openObj = CommonFunction.FindChiledByName(transform, "Open");
            if (openObj != null)
            {
                openObj.SetActive(!value);
            }

            var closeObj = CommonFunction.FindChiledByName(transform, "Close");
            if (closeObj != null)
            {
                closeObj.SetActive(value);
                collider.isTrigger = !value;
            }
        }
    }
    private bool doorIsClosed = true;

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

    public void OnClickDoor()
    {
        DoorIsClosed = !DoorIsClosed;
    }
}