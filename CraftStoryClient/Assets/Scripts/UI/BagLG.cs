using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public class BagLG : UILogicBase<BagLG, BagUI>
{
    public BagItemCell SelectItem 
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
    private BagItemCell selectItem;
}
