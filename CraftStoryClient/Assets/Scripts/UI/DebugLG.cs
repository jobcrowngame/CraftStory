using System;


public class DebugLG : UILogicBase<DebugLG, DebugUI>
{
    string mMsg;

    public void Add(string msg)
    {
        mMsg += msg;
    }
    public void Add(int msg)
    {
        mMsg += msg;
    }


    public void Refresh()
    {
        if (UI != null)
        {
            UI.Refresh(mMsg);
        }
    }
}
