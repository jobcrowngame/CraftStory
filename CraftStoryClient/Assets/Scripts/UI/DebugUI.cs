using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class DebugUI : UIBase
{
    Text Msg { get => FindChiled<Text>("Msg"); }
    Button CloseBtn { get => FindChiled<Button>("CloseBtn"); }

    public override void Init()
    {
        base.Init();
        DebugLG.E.Init(this);

        CloseBtn.onClick.AddListener(Close);
    }

    public void Refresh(string msg)
    {
        Msg.text = msg;
    }
}