﻿using System;
using UnityEngine;

public class UIBase : MonoBehaviour
{
    public GameObject obj;
    private UIBase beforeUI;

    public virtual void Init(GameObject obj)
    {
        this.obj = obj;
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

    protected T FindChiled<T>(string chiledName, GameObject parent = null) where T : Component
    {
        if (parent == null)
            parent = obj;
        
        var findObj = CommonFunction.FindChiledByName(parent, chiledName);
        if (findObj != null)
            return findObj.GetComponent<T>();
        else
        {
            MLog.Error("Find chiled fail: " + chiledName);
            MLog.Error("From UI: " + obj.name);
            return null;
        }
    }
    protected Transform FindChiled(string chiledName, GameObject parent = null)
    {
        if (parent == null)
            parent = obj;

        var findObj = CommonFunction.FindChiledByName(parent, chiledName);
        if (findObj != null)
            return findObj.transform;
        else
        {
            MLog.Error("Find chiled fail: " + chiledName);
            MLog.Error("From UI: " + obj.name);
            return null;
        }
    }
}
