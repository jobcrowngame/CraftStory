using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class UICtl : Single<UICtl>
{
    private static Transform glubalObjTran;
    private UIBase curentOpenUI;

    private Dictionary<UIType, UIBase> uiDic;

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
         Logger.Log("èâä˙âª UICtl");

        glubalObjTran = glubalObj.transform;
        uiDic = new Dictionary<UIType, UIBase>();

        //UserTest.E.Init();
        IAPMng.E.Init();
        TimeZoneMng.E.Init();
        WorldMng.E.Init();

        yield return true;
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

    public T OpenUI<T>(UIType uiType, UIOpenType closeType = UIOpenType.None) where T : UIBase
    {
        switch (closeType)
        {
            case UIOpenType.AllClose:
                foreach (var item in uiDic.Values)
                {
                    if (item != null && item.IsActive)
                        item.Close();
                }
                break;

            case UIOpenType.BeforeClose:
                if (curentOpenUI != null)
                    curentOpenUI.Close();
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
                Logger.Error("bad ui path " + uiType.ToString());
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
                uiClass = obj.AddComponent<T>();

            uiClass.Init();

            uiDic[uiType] = uiClass;

        }
        else
        {
            uiClass = uiDic[uiType];
        }

        uiClass.Open();

        curentOpenUI = uiClass;

        return uiClass as T;
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
        uiDic.Clear();
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
            case UIType.BlueprintReName: return "Prefabs/UI/BlueprintReName";
            case UIType.PlayDescription: return "Prefabs/UI/PlayDescription";
            case UIType.PersonalMessage: return "Prefabs/UI/PersonalMessage";
            case UIType.BlueprintPreview: return "Prefabs/UI/BlueprintPreview";
            case UIType.MyShop: return "Prefabs/UI/MyShop";
            case UIType.MyShopSelectItem: return "Prefabs/UI/MyShopSelectItem";
            case UIType.MyShopUpload: return "Prefabs/UI/MyShopUpload";
            case UIType.Email: return "Prefabs/UI/Email";
            case UIType.EmailDetails: return "Prefabs/UI/EmailDetails";
            case UIType.Notice: return "Prefabs/UI/Notice";
            case UIType.NoticeDetail: return "Prefabs/UI/NoticeDetail";
            case UIType.Debug: return "Prefabs/UI/Common/Debug";
            default: Logger.Error("not find UIType " + ui); return "";
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
    BlueprintReName,
    PlayDescription,
    PersonalMessage,
    BlueprintPreview,
    MyShop,
    MyShopSelectItem,
    MyShopUpload,
    Email,
    EmailDetails,
    Notice,
    NoticeDetail,
    Debug,
}
