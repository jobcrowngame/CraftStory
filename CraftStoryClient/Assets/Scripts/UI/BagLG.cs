using Gs2.Unity.Gs2Inventory.Model;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public class BagLG : UILogicBase<BagLG, BagUI>
{
    public ItemCell SelectItem 
    {
        get => selectItem;
        set
        {
            if (selectItem != null)
            {
                selectItem.IsSelected(false);
            }

            selectItem = value;
            if (selectItem != null) selectItem.IsSelected(true);
        }
    }
    private ItemCell selectItem;
}
