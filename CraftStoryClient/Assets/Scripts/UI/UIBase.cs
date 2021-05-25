using System;
using UnityEngine;

public class UIBase : MonoBehaviour
{
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
        gameObject.SetActive(b);
    }

    protected T FindChiled<T>(string chiledName, GameObject parent = null) where T : Component
    {
        if (parent == null)
            parent = gameObject;
        
        var findObj = CommonFunction.FindChiledByName(parent.transform, chiledName);
        if (findObj != null)
            return findObj.GetComponent<T>();
        else
        {
            Debug.LogError("Find chiled fail: " + chiledName);
            Debug.LogError("From UI: " + gameObject.name);
            return null;
        }
    }
    protected Transform FindChiled(string chiledName, GameObject parent = null)
    {
        if (parent == null)
            parent = gameObject;

        var findObj = CommonFunction.FindChiledByName(parent.transform, chiledName);
        if (findObj != null)
            return findObj.transform;
        else
        {
            Debug.LogError("Find chiled fail: " + chiledName);
            Debug.LogError("From UI: " + gameObject.name);
            return null;
        }
    }

    protected T AddCell<T>(string resourcesPath, Transform parent) where T : UIBase
    {
        var resources = ResourcesMng.E.ReadResources(resourcesPath);
        if (resources == null)
            Debug.LogError("not find resources " + resourcesPath);

        var obj = GameObject.Instantiate(resources, parent);
        if (obj == null)
            return null;

        var cell = obj.GetComponent<T>();
        if (cell == null)
            return null;

        return cell;
    }

    protected void ClearCell(Transform parent)
    {
        foreach (Transform t in parent)
        {
            GameObject.Destroy(t.gameObject);
        }
    }
}
