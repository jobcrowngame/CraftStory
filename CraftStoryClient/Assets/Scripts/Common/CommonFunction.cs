using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CommonFunction
{
    public static GameObject FindChiledByName(GameObject obj, string name)
    {
        if (obj == null)
            MLog.Error(obj.name + "---" + name + " is null");

        Transform trans = obj.transform;
        Transform childTrans = trans.Find(name);
        if (childTrans != null)
        {
            return childTrans.gameObject;
        }
        else
        {
            return FIndAllChiled(obj, name);
        }
    }
    private static GameObject FIndAllChiled(GameObject obj, string name)
    {
        GameObject retObj = null;

        foreach (Transform t in obj.transform)
        {
            if (t.name == name)
            {
                retObj = t.gameObject;
                return retObj;
            }
            else
            {
                retObj = FIndAllChiled(t.gameObject, name);
                if (retObj == null)
                    continue;
                else
                    break;
            }
        }

        return retObj;
    }

    public static GameObject ReadResourcesPrefab(string path)
    {
        var prefab = Resources.Load(path) as GameObject;
        if (prefab == null)
            return null;

        return prefab;
    }
}
