using UnityEngine;

public class UIBase : MonoBehaviour
{
    public bool IsActive { get => gameObject.activeSelf; }

    public virtual void Init()
    {
    }
    public virtual void Init(object data)
    {
    }
    public virtual void Init<T>(T t) where T : class
    {
    }

    public virtual void Close()
    {
        gameObject.SetActive(false);
    }
    public virtual void Open()
    {
        gameObject.SetActive(true);
    }
    public virtual void Destroy()
    {
        Destroy(gameObject);
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
            Logger.Error("Find chiled fail: " + chiledName);
            Logger.Error("From UI: " + gameObject.name);
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
            Logger.Error("Find chiled fail: " + chiledName);
            Logger.Error("From UI: " + gameObject.name);
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
