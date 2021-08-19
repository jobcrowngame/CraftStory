using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class ResourcesMng : Single<ResourcesMng>
{
    Dictionary<string, Object> resourcesDic;

    public IEnumerator InitInitCoroutine()
    {
        resourcesDic = new Dictionary<string, Object>();

        yield return null;
    }

    public Object ReadResources(string path)
    {
        if (string.IsNullOrEmpty(path))
            return null;
        
        Object gameObj = null;
        if (!resourcesDic.TryGetValue(path, out gameObj))
        {
            gameObj = Resources.Load(path);
            if (gameObj == null)
            {
                Logger.Error("not find resources " + path);
                return null;
            }

            resourcesDic[path] = gameObj;
        }

        return gameObj;
    }

    public T ReadResources<T>(string path) where T : Object
    {
        if (string.IsNullOrEmpty(path))
            return null;

        Object gameObj = null;
        if (!resourcesDic.TryGetValue(path, out gameObj))
        {
            gameObj = Resources.Load<T>(path);
            if (gameObj == null)
            {
                Logger.Error("not find resources " + path);
                return null;
            }

            resourcesDic[path] = gameObj;
        }

        return (T)gameObj;
    }
}