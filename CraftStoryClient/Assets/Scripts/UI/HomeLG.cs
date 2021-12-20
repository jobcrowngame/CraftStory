
using System.Collections.Generic;

public class HomeLG : UILogicBase<HomeLG, HomeUI>
{
    Stack<BraveCellItem> itemStack = new Stack<BraveCellItem>();

    private bool isInited = false;

    public override void Init(HomeUI ui)
    {
        base.Init(ui);

        if (isInited)
            return;

        isInited = true;

        TimeZoneMng.E.AddTimerEvent01(() =>
        {
            if (itemStack.Count > 0)
            {
                var bonus = itemStack.Pop();
                UI.AddItem(bonus);
            }
        });
    }

    public void AddBonus(int id)
    {
        var config = ConfigMng.E.Bonus[id];

        if (config.Bonus1 > 0)
        {
            itemStack.Push(new BraveCellItem() { itemId = config.Bonus1, count = config.BonusCount1 });
        }
        if (config.Bonus2 > 0)
        {
            itemStack.Push(new BraveCellItem() { itemId = config.Bonus2, count = config.BonusCount2 });
        }
        if (config.Bonus3 > 0)
        {
            itemStack.Push(new BraveCellItem() { itemId = config.Bonus3, count = config.BonusCount3 });
        }
        if (config.Bonus4 > 0)
        {
            itemStack.Push(new BraveCellItem() { itemId = config.Bonus4, count = config.BonusCount4 });
        }
        if (config.Bonus5 > 0)
        {
            itemStack.Push(new BraveCellItem() { itemId = config.Bonus5, count = config.BonusCount5 });
        }
        if (config.Bonus6 > 0)
        {
            itemStack.Push(new BraveCellItem() { itemId = config.Bonus6, count = config.BonusCount6 });
        }
    }

    public void ClearItemStack()
    {
        itemStack.Clear();
    }

    public void DeleteItems()
    {
        UICtl.E.OpenUI<DeleteItemUI>(UIType.DeleteItem);
    }

    public struct BraveCellItem
    {
        public int itemId;
        public int count;
    }
}