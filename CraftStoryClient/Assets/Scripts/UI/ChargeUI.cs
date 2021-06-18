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
        Debug.Log("ShowItems");
    }
    public void OnError()
    {
        Debug.Log("OnError");
        //Debug.Log(error.Message);
    }
}
