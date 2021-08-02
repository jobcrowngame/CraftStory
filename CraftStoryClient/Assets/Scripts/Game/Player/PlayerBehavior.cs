
using UnityEngine;

public class PlayerBehavior
{
    public PlayerBehaviorType Type
    {
        get => behaviorType;
        set
        {
            //if (behaviorType == value)
                //return;

            if (behaviorType != value)
            {
                Logger.Log("動作変換 -> " + value);
            }

            behaviorType = value;

            switch (behaviorType)
            {
                case PlayerBehaviorType.Waiting: 
                    //PlayerCtl.E.PlayerEntity.ShowDestroyEffect(false);
                    break;

                case PlayerBehaviorType.Run:
                    //PlayerCtl.E.PlayerEntity.ShowDestroyEffect(false);
                    break;

                case PlayerBehaviorType.Create:
                    //PlayerCtl.E.PlayerEntity.ShowDestroyEffect(false);
                    break;

                case PlayerBehaviorType.Breack: 
                    PlayerCtl.E.PlayerEntity.ShowDestroyEffect(true);
                    break;

                case PlayerBehaviorType.Jump:
                    //PlayerCtl.E.PlayerEntity.ShowDestroyEffect(false);
                    break;

                case PlayerBehaviorType.None: break;

                default: Logger.Error("Not find behavior type " + value); break;
            }

            PlayerCtl.E.PlayerEntity.EntityBehaviorChange((int)behaviorType - 1);

            // Breack以外ならDestroyEffectを削除する
            if (behaviorType != PlayerBehaviorType.Breack)
            {
                EffectMng.E.RemoveDestroyEffect();
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

             Logger.Log("選択アイテム変化 -> " + value);

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

                //default: Logger.Error("Not find ItemType " + value); break;
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
    Jump,
}
