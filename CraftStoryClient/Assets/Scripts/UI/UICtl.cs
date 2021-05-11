using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICtl : Single<UICtl>
{
    private static Transform glubalObjTran;
    private static Transform uiRootTran;
    private UIBase curentOpenUI;

    private Dictionary<UIType, UIBase> uiDic;
    private List<UIBase> OpenningUI;

    public void Init(GameObject glubalObj, GameObject uiRoot)
    {
        glubalObjTran = glubalObj.transform;
        uiRootTran = uiRoot.transform;
        uiDic = new Dictionary<UIType, UIBase>();
        OpenningUI = new List<UIBase>();

        CreateGlobalEntity();
    }

    private void CreateGlobalEntity()
    {
        MLog.E.Init();
        GS2.E.Init();
        UserTest.E.Init();

        OpenUI<LoginUI>(UIType.Login);
    }

    public T CreateGlobalObject<T>() where T : Component
    {
        var obj = new GameObject();
        obj.transform.parent = glubalObjTran;
        var entity = obj.AddComponent<T>();
        obj.name = entity.ToString();

        return entity;
    }

    public UIBase GetUI(UIType uiType)
    {
        return uiDic[uiType];
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
                MLog.Error("bad ui path " + uiType.ToString());
                return null;
            }

            var prefab = Resources.Load(uiResourcesPath) as GameObject;
            if (prefab == null)
                return null;

            var obj = GameObject.Instantiate(prefab, uiRootTran);
            if (obj == null)
                return null;

            uiClass = obj.GetComponent<T>();
            if (uiClass == null)
                return null;

            uiClass.Init(obj);

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

    private string GetUIResourcesPath(UIType ui)
    {
        switch (ui)
        {
            case UIType.Login: return "Prefabs/UI/Login";
            case UIType.Home: return "Prefabs/UI/Home";
            case UIType.Bag: return "Prefabs/UI/Bag";
            case UIType.Lottery: return "Prefabs/UI/Lottery";
            case UIType.GiftBox: return "Prefabs/UI/GiftBox";
            default: return "";
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
    Home,
    Bag,
    Lottery,
    GiftBox,
}
