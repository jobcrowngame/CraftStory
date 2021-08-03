using System;


public class DebugLG : UILogicBase<DebugLG, DebugUI>
{
    public void Add(string msg)
    {
        if (UI != null) UI.Add(msg);
    }
    public void Add(int msg)
    {
        if (UI != null) UI.Add(msg.ToString());
    }
    public void Clear()
    {
        if (UI != null) UI.Clear();
    }
}
