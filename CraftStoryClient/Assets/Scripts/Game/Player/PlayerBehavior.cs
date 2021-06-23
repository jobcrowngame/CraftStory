
using UnityEngine;

public class PlayerBehavior
{
    public PlayerBehaviorType Type
    {
        get => behaviorType;
        set
        {
            if (behaviorType == value)
                return;

            Debug.Log("動作変換 -> " + value);

            behaviorType = value;

            switch (behaviorType)
            {
                case PlayerBehaviorType.Waiting: PlayerCtl.E.PlayerEntity.EntityBehaviorChange(0); break;
                case PlayerBehaviorType.Run: PlayerCtl.E.PlayerEntity.EntityBehaviorChange(1); break;
                case PlayerBehaviorType.Create: PlayerCtl.E.PlayerEntity.EntityBehaviorChange(2); break;
                case PlayerBehaviorType.Breack: PlayerCtl.E.PlayerEntity.EntityBehaviorChange(3); break;
                case PlayerBehaviorType.None: break;

                default: Debug.LogError("Not find behavior type " + value); break;
            }
        }
    }
    private PlayerBehaviorType behaviorType = PlayerBehaviorType.None;

    public ItemType SelectItemType
    {
        get => selectItemType;
        set
        {
            if (selectItemType == value)
                return;

            Debug.Log("選択アイテム変化 -> " + value);

            selectItemType = value;

            switch (selectItemType)
            {
                case ItemType.None:
                case ItemType.Block:
                case ItemType.BuilderPencil:
                case ItemType.NullBlueprint:
                case ItemType.Blueprint:
                case ItemType.Workbench:
                case ItemType.Kamado:
                    break;

                //default: Debug.LogError("Not find ItemType " + value); break;
            }
        }
    }
    private ItemType selectItemType = ItemType.None;
}

public enum PlayerBehaviorType
{
    None,
    Waiting,
    Run,
    Create,
    Breack,
}
