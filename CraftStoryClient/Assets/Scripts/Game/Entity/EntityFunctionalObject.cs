using UnityEngine;

/// <summary>
/// 機能Object
/// </summary>
public class EntityFunctionalObject : EntityBase
{
    // ドアの状態（閉じっている）
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

    /// <summary>
    /// 長い時間クリック終了場合のロジック
    /// </summary>
    public override void ClickingEnd()
    {
        base.ClickingEnd();

        OnDestroyEntity();
    }

    /// <summary>
    /// クリックイベント
    /// </summary>
    public override void OnClick()
    {
        base.OnClick();

        switch (Type)
        {
            case EntityType.Door:
                DoorIsClosed = !DoorIsClosed;
                break;

            case EntityType.Workbench:
            case EntityType.Kamado:
            case EntityType.EquipmentWorkbench:
            case EntityType.CookingTable:
                var ui = UICtl.E.OpenUI<CraftUI>(UIType.Craft);
                ui.SetType(Type);
                break;

            case EntityType.Mission:
                UICtl.E.OpenUI<MissionUI>(UIType.Mission);
                break;
            default: Logger.Error("not find entityType " + Type); break;
        }

        GuideLG.E.NextOnClickEntity(Type);
    }
}