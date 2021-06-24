using UnityEngine;

public class UIBase : MonoBehaviour
{
    private UIBase beforeUI;

    public virtual void Init()
    {
    }
    public virtual void Init<T>(T t) where T : class
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
    public virtual void Destroy()
    {
        Destroy(gameObject);
    }

    private void Active(bool b)
    {
        gameObject.SetActive(b);
    }

    protected T FindChiled<T>(string chiledName, Transform parent = null) where T : Component
    {
        if (parent == null)
            parent = gameObject.transform;
        
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
            return null;

        var obj = GameObject.Instantiate(resources, parent) as GameObject;
        if (obj == null)
            return null;

        var cell = obj.GetComponent<T>();
        if (cell == null)
            cell = obj.AddComponent<T>();

        return cell;
    }

    protected void ClearCell(Transform parent)
    {
        foreach (Transform t in parent)
        {
            GameObject.Destroy(t.gameObject);
        }
    }

    protected T ReadResources<T>(string resourcesPath) where T : Object
    {
        return ResourcesMng.E.ReadResources<T>(resourcesPath);
    }
}
