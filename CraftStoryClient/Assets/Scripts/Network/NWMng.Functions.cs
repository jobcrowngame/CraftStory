

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

    /// <summary>
    /// アイテム関連データを読む
    /// </summary>
    /// <param name="itemGuid"></param>
    /// <param name="iData"></param>
    /// <param name="callBack"></param>
    public void GetItemRelationData(int itemGuid, ItemData iData, Action callBack = null)
    {
        GetItemRelationData((rp) => 
        {
            iData.relationData = (string)rp;
            if (callBack != null) callBack();
        }, itemGuid);
    }
}
