

using UnityEngine;

/// <summary>
/// 向きがある建物（ドア、作業台...)
/// </summary>
public class EntityBuilding : EntityBase
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
    /// クリックした場合のロジック
    /// </summary>
    public override void OnClick()
    {
        base.OnClick();


        if (Type == EntityType.Door)
        {
            DoorIsClosed = !DoorIsClosed;
        }
        else if (Type == EntityType.Workbench || Type == EntityType.Kamado)
        {
            var ui = UICtl.E.OpenUI<CraftUI>(UIType.Craft);
            ui.SetType(Type);
        }
    }
}