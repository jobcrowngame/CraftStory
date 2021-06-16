using Gs2.Core.Exception;
using Gs2.Unity.Util;
using Gs2.Weave.Core.CallbackEvent;
using UnityEngine;

public class ChargeUI : UIBase
{
    public GetShowcaseEvent onGetShowcase;
    public ErrorEvent onError;

    Transform gridRoot;

    public override void Init()
    {
        base.Init();

        ChargeLG.E.Init(this);

        InitUI();
    }

    private void InitUI()
    {
        gridRoot = FindChiled("Content");
    }

    public void ShowItems()
    {
        Debug.Log("ShowItems");
    }
    public void OnError()
    {
        Debug.Log("OnError");
        //Debug.Log(error.Message);
    }
}
