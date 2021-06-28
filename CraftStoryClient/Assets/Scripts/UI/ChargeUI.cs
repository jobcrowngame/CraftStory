using UnityEngine;

public class ChargeUI : UIBase
{
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
        Logger.Log("ShowItems");
    }
    public void OnError()
    {
        Logger.Log("OnError");
        //Logger.Log(error.Message);
    }
}
