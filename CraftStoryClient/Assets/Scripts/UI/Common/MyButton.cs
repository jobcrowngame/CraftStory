using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

public class MyButton : Button
{
    public bool IsClicking { get => isClicking; }
    bool isClicking;
    Action onClicking;

    void Update()
    {
        if (isClicking && onClicking != null)
        {
            onClicking();
        }
    }

    public void AddClickingListener(Action ac)
    {
        onClicking = ac;
    }

    public void OnClicking()
    {
        isClicking = true;
    }
    public void EndClicking()
    {
        isClicking = false;
    }
}