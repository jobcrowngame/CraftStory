using UnityEngine;

/// <summary>
/// UI ベース
/// </summary>
public class UIBase : MonoBehaviour
{
    public bool IsActive { get => gameObject.activeSelf; } // アクティブ

    public virtual void Init() { }
    public virtual void Init(object data) { }
    public virtual void Init<T>(T t) where T : class { }

    public virtual void Close()
    {
        gameObject.SetActive(false);
    }
    public virtual void Open(object data)
    {
        gameObject.SetActive(true);
    }
    public virtual void Open()
    {
        gameObject.SetActive(true);
    }
    public virtual void Destroy()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// サブGameObjectのComponentを検索
    /// </summary>
    /// <typeparam name="T">Component</typeparam>
    /// <param name="chiledName">サブGameObject名</param>
    /// <param name="parent">親</param>
    /// <returns></returns>
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

    /// <summary>
    /// サブをインスタンス
    /// </summary>
    /// <typeparam name="T">Component</typeparam>
    /// <param name="resourcesPath">アセットのパス</param>
    /// <param name="parent">インスタンス親</param>
    /// <returns></returns>
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

    /// <summary>
    /// サブGameObjectを削除
    /// </summary>
    /// <param name="parent"></param>
    protected void ClearCell(Transform parent)
    {
        foreach (Transform t in parent)
        {
            GameObject.Destroy(t.gameObject);
        }
    }

    /// <summary>
    /// アセットをロード
    /// </summary>
    /// <typeparam name="T">アセットタイプ</typeparam>
    /// <param name="resourcesPath">パス</param>
    /// <returns></returns>
    protected T ReadResources<T>(string resourcesPath) where T : Object
    {
        return ResourcesMng.E.ReadResources<T>(resourcesPath);
    }
}
