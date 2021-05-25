using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class ResourcesMng : Single<ResourcesMng>
{
    Dictionary<string, GameObject> resourcesDic;

    public IEnumerator InitInitCoroutine()
    {
        resourcesDic = new Dictionary<string, GameObject>();

        yield return null;
    }

    public GameObject ReadResources(string path)
    {
        GameObject gameObj = null;
        if (!resourcesDic.TryGetValue(path, out gameObj))
        {
            gameObj = Resources.Load(path) as GameObject;
            if (gameObj == null)
            {
                Debug.LogError("not find resources " + path);
                return null;
            }

            resourcesDic[path] = gameObj;
        }

        return gameObj;
    }
}