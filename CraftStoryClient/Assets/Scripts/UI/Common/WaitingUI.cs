using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingUI : UIBase
{
    public override void Init()
    {
        base.Init();
        WaitingLG.E.Init(this);
    }
}
