
/// <summary>
/// 通信を待つ場合、Waiting　UI
/// </summary>
public class WaitingUI : UIBase
{
    public override void Init()
    {
        base.Init();
        WaitingLG.E.Init(this);
    }
}
