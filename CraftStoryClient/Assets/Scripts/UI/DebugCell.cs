using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class DebugCell : UIBase
{
    Text Text { get => GetComponent<Text>(); }

    public void Set(string msg)
    {
        Text.text = msg;
    }
}
