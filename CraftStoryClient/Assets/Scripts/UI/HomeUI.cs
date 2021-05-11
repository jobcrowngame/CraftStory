using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomeUI : UIBase
{
    private Button bagBtn;

    public override void Init()
    {
        HomeLG.E.Init(this);

        if (!InitUI())
        {
            MLog.Error("HomeUI Init fail!");
        }
    }

    private bool InitUI()
    {
        var idObj = CommonFunction.FindChiledByName(gameObject, "BagBtn");
        if (idObj != null)
            bagBtn = idObj.GetComponent<Button>();
        else
            return false;

        bagBtn.onClick.AddListener(() => { UICtl.E.OpenUI<BagUI>(UIType.Bag); });

        return true;
    }
}
