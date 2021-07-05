using JsonConfigData;
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
            Logger.Error(parent.gameObject.name + "---" + name + " is null");

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
    public static T InstantiateUI<T>(string resourcesPath, Transform parent) where T : UIBase
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
    public static void ClearCell(Transform parent)
    {
        foreach (Transform t in parent)
        {
            GameObject.Destroy(t.gameObject);
        }
    }

    public static void GoToNextScene()
    {
        UICtl.E.Clear();
        PlayerCtl.E.SelectItem = null;

        SceneManager.LoadSceneAsync("NowLoading");
        Logger.Warning("GoToNextScene TransferGate ID " + DataMng.E.MapData.TransferGate.ID);
    }
    public static void GoToHome()
    {
        UICtl.E.Clear();

        DataMng.E.MapData.TransferGate = new EntityData(100, ItemType.TransferGate);
        SceneManager.LoadSceneAsync("NowLoading");
        Logger.Warning("GotoHome TransferGate ID " + DataMng.E.MapData.TransferGate.ID);
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
        var en = new UTF8Encoding(encoderShouldEmitUTF8Identifier: true);
        return en.GetString(Encoding.Default.GetBytes(msg));
    }

   

    public static Dictionary<int, int> GetItemsByBonus(List<int> bonusIds)
    {
        Dictionary<int, int> items = new Dictionary<int, int>();
        for (int i = 0; i < bonusIds.Count; i++)
        {
            GetItemsByBonus(bonusIds[i], ref items);
        }

        return items;
    }
    public static void GetItemsByBonus(int bonusId, ref Dictionary<int, int> items)
    {
        var config = ConfigMng.E.Bonus[bonusId];
        if (config != null)
        {
            BonusToItems(config.Bonus1, config.BonusCount1, ref items);
            BonusToItems(config.Bonus2, config.BonusCount2, ref items);
            BonusToItems(config.Bonus3, config.BonusCount3, ref items);
            BonusToItems(config.Bonus4, config.BonusCount4, ref items);
            BonusToItems(config.Bonus5, config.BonusCount5, ref items);
            BonusToItems(config.Bonus6, config.BonusCount6, ref items);
        }
    }
    private static void BonusToItems(int itemId, int count, ref Dictionary<int, int> items)
    {
        if (itemId < 0)
            return;
        
        if (items.ContainsKey(itemId))
        {
            items[itemId] += count;
        }
        else
        {
            items[itemId] = count;
        }
    }

    public static void ShowHintBar(int errCode)
    {
        if (!ConfigMng.E.ErrorMsg.ContainsKey(errCode))
        {
            Logger.Error("not find errcode " + errCode);
            return;
        }

        var ui = InstantiateUI<HintBarUI>("Prefabs/UI/Common/HintBar", UICtl.E.Root);
        if (ui != null)
        {
            ui.SetMsg(ConfigMng.E.ErrorMsg[errCode].Message);
        }
    }
    public static void ShowHintBox(string msg, Action okAction)
    {
        var ui = InstantiateUI<HintBoxUI>("Prefabs/UI/Common/HintBox", UICtl.E.Root);
        if (ui != null)
        {
            ui.Init(msg, okAction);
        }
    }
    public static void ShowHintBox(string iconPath, string msg, Action okAction, Action cancelAction = null)
    {
        var ui = InstantiateUI<HintBoxUI>("Prefabs/UI/Common/HintBox", UICtl.E.Root);
        if (ui != null)
        {
            ui.Init(iconPath, msg, okAction, cancelAction);
        }
    }
}
