using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CommonFunction
{
    public static T CreateGlobalObject<T>() where T : Component
    {
        var obj = new GameObject();
        obj.transform.parent = Main.E.transform;
        var entity = obj.AddComponent<T>();
        obj.name = entity.ToString();

        return entity;
    }

    public static T FindChiledByName<T>(Transform parent, string name) where T : Component
    {
        var findObj = FindChiledByName(parent, name);
        if (findObj != null)
            return findObj.GetComponent<T>();

        return null;
    }
    public static GameObject FindChiledByName(Transform parent, string name)
    {
        if (parent == null)
            Debug.LogError(parent.gameObject.name + "---" + name + " is null");

        Transform childTrans = parent.Find(name);
        if (childTrans != null)
        {
            return childTrans.gameObject;
        }
        else
        {
            return FIndAllChiled(parent, name);
        }
    }
    private static GameObject FIndAllChiled(Transform parent, string name)
    {
        GameObject retObj = null;

        foreach (Transform t in parent)
        {
            if (t.name == name)
            {
                retObj = t.gameObject;
                return retObj;
            }
            else
            {
                retObj = FIndAllChiled(t, name);
                if (retObj == null)
                    continue;
                else
                    break;
            }
        }

        return retObj;
    }

    public static GameObject Instantiate(string path, Transform parent, Vector3 pos)
    {
        var resources = ResourcesMng.E.ReadResources(path);
        if (resources == null)
            return null;

        var obj = GameObject.Instantiate(resources, parent) as GameObject;
        obj.transform.position = pos;

        return obj;
    }
    public static T Instantiate<T>(string path, Transform parent, Vector3 pos) where T : Component
    {
        var obj = Instantiate(path, parent, pos);
        if (obj == null)
            return null;

        var componte = obj.GetComponent<T>();

        if (componte == null)
            componte = obj.AddComponent<T>();

        return componte;
    }

    public static void GoToNextScene(int id, string name)
    {
        DataMng.E.NextSceneID = id;
        DataMng.E.NextSceneName = name;

        SceneManager.LoadSceneAsync("NowLoading");

        DataMng.E.CurrentSceneID = id;
    }

    public static Vector3 Vector3Sum(Vector3 v1, Vector3 v2)
    {
        return new Vector3(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
    }
    public static Vector3Int Vector3Sum(Vector3Int v1, Vector3Int v2)
    {
        return new Vector3Int(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
    }

    public static Vector3Int Vector3Minus(Vector3Int v1, Vector3Int v2)
    {
        return new Vector3Int(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);
    }

    public static string ToUTF8Bom(string msg)
    {
        using (var stream = new MemoryStream())
        {
            using (var sw = new StreamWriter(stream, new UTF8Encoding(encoderShouldEmitUTF8Identifier: true)))
            {
                sw.Write(msg);
            }

            return BitConverter.ToString(stream.ToArray());
        }
    }
}
