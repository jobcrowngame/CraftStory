using UnityEngine;

public class BaseUI
{
    public static GameObject uiRoot;
    private static GameObject obj;

    public static GameObject Obj
    {
        get
        {
            if (obj == null)
            {
                obj = new GameObject();
                obj.transform.parent = uiRoot.transform;
            }

            return obj;
        }
    }
}
