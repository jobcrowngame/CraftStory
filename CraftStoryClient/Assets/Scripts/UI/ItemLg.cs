using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ItemLg : UILogicBase<ItemLg, ItemCell>
{
    public void Increase(string itemName)
    {
        GS2.E.GetItem(itemName);
    }

    public void Decrease(ItemCell cell, string itemName, long costNum)
    {
        GS2.E.Consume(cell, itemName, costNum);
    }
}