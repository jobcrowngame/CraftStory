using System;
using UnityEngine;

public class UIBase : MonoBehaviour
{
    public GameObject obj;
    private UIBase beforeUI;

    public virtual void Init()
    {

    }

    public void SetBeforUI(UIBase beforeUI)
    {
        this.beforeUI = beforeUI;
    }

    public virtual void Close()
    {
        Active(false);
    }
    public virtual void Open()
    {
        Active(true);
    }

    private void Active(bool b)
    {
        obj.SetActive(b);
    }
}
