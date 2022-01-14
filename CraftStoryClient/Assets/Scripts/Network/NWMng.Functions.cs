

using LitJson;
using System;
using System.Collections.Generic;

public partial class NWMng
{
    /// <summary>
    /// アイテムリストをゲット
    /// </summary>
    /// <param name="callBack"></param>
    public void GetItems(Action callBack = null)
    {
        if (HomeLG.E.UI != null) HomeLG.E.UI.RefreshItemBtns();
        if (callBack != null) callBack();
    }
}
