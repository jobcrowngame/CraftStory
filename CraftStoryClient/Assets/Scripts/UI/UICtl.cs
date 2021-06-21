using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class UICtl : Single<UICtl>
{
    private static Transform glubalObjTran;
    private UIBase curentOpenUI;

    private Dictionary<UIType, UIBase> uiDic;
    private List<UIBase> OpenningUI;

    private static Transform uiRootTran;
    public Transform Root
    {
        get
        {
            if (uiRootTran == null)
                uiRootTran = GameObject.Find("Canvas").transform;

            return uiRootTran;
        }
    }

    public IEnumerator InitCoroutine(GameObject glubalObj)
    {
        Debug.Log("èâä˙âª UICtl");

        bool ret = false;

        glubalObjTran = glubalObj.transform;
        uiDic = new Dictionary<UIType, UIBase>();
        OpenningUI = new List<UIBase>();

        ret = CreateGlobalEntity();

        yield return ret;
    }

    private bool CreateGlobalEntity()
    {
        //UserTest.E.Init();
        WorldMng.E.Init();

        return true;
    }

    public T CreateGlobalObject<T>() where T : Component
    {
        var obj = new GameObject();
        obj.transform.parent = glubalObjTran;
        var entity = obj.AddComponent<T>();
        obj.name = entity.ToString();

        return entity;
    }

    private UIBase GetUI(UIType uiType)
    {
        return uiDic[uiType];
    }
    public T GetUI<T>(UIType uiType) where T : UIBase
    {
        if (!uiDic.ContainsKey(uiType))
            return null;
        return uiDic[uiType] as T;
    }
    public void AddUI(UIBase ui, UIType uiType)
    {
        uiDic[uiType] = ui;
    }

    public UIBase OpenUI<T>(UIType uiType, UIOpenType closeType = UIOpenType.None) where T : UIBase
    {
        switch (closeType)
        {
            case UIOpenType.AllClose:
                foreach (var item in OpenningUI)
                {
                    item.Close();
                }
                break;

            case UIOpenType.BeforeClose:
                if (curentOpenUI != null)
                {
                    curentOpenUI.Close();
                }
                break;

            default:
                break;
        }

        UIBase uiClass = null;

        if (!uiDic.ContainsKey(uiType))
        {
            string uiResourcesPath = GetUIResourcesPath(uiType);
            if (uiResourcesPath == "")
            {
                Debug.LogError("bad ui path " + uiType.ToString());
                return null;
            }

            var prefab = Resources.Load(uiResourcesPath) as GameObject;
            if (prefab == null)
                return null;

            var obj = GameObject.Instantiate(prefab, Root);
            if (obj == null)
                return null;

            uiClass = obj.GetComponent<T>();
            if (uiClass == null)
                return null;

            uiClass.Init();

            uiDic[uiType] = uiClass;

        }
        else
        {
            uiClass = uiDic[uiType];
        }

        uiClass.Open();
        OpenningUI.Add(uiClass);

        if (curentOpenUI != null)
        {
            uiClass.SetBeforUI(curentOpenUI);
        }

        curentOpenUI = uiClass;

        return uiClass;
    }

    public void CloseUI(UIType uiType)
    {
        UIBase ui = GetUI(uiType);
        CloseUI(ui);
    }
    public void CloseUI(UIBase ui)
    {
        ui.Close();
    }
    public void Clear()
    {
        uiDic = new Dictionary<UIType, UIBase>();
    }

    private string GetUIResourcesPath(UIType ui)
    {
        switch (ui)
        {
            case UIType.Login: return "Prefabs/UI/Login";
            case UIType.NowLoading: return "Prefabs/UI/NowLoading";
            case UIType.Home: return "Prefabs/UI/Home";
            case UIType.Menu: return "Prefabs/UI/Menu";
            case UIType.Bag: return "Prefabs/UI/Bag";
            case UIType.Lottery: return "Prefabs/UI/Lottery";
            case UIType.GiftBox: return "Prefabs/UI/GiftBox";
            case UIType.Shop: return "Prefabs/UI/Shop";
            case UIType.Terms: return "Prefabs/UI/Terms";
            case UIType.Terms01: return "Prefabs/UI/Terms01";
            case UIType.Terms02: return "Prefabs/UI/Terms02";
            case UIType.Charge: return "Prefabs/UI/Charge";
            case UIType.Craft: return "Prefabs/UI/Craft";
            default: Debug.LogError("not find UIType " + ui); return "";
        }
    }
}

public enum UIOpenType
{
    None,
    AllClose,
    BeforeClose,
}
public enum UIType
{
    Login,
    NowLoading,
    Home,
    Menu,
    Bag,
    Lottery,
    GiftBox,
    Shop,
    Terms,
    Terms01,
    Terms02,
    Charge,
    Craft,
}
