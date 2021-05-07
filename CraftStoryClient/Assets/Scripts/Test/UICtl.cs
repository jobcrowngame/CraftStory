using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICtl : Single<UICtl>
{
    private static Transform rootTf;

    public void Init(GameObject uiRoot)
    {
        rootTf = uiRoot.transform;

        CreateGlobalEntity();
    }

    private void CreateGlobalEntity()
    {
        GS2.E.Init();
    }

    public static T CreateGlobalObject<T>() where T : Component
    {
        var obj = new GameObject();
        obj.transform.parent = rootTf;
        var entity = obj.AddComponent<T>();
        obj.name = entity.ToString();

        return entity;
    }
}
