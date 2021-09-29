using System.Collections;
using System.Collections.Generic;

using UnityEngine;

/// <summary>
/// UIコンソール
/// </summary>
public class UICtl : Single<UICtl>
{
    private static Transform glubalObjParent; // グローバルGameObject親
    private UIBase curentOpenUI; // オープンしてるUI
    private UIBase Waiting; // 通信が遅い場合、待つUI

    private Dictionary<UIType, UIBase> uiDic;

    private static Transform uiRootTran; // UIインスタンス親
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
         Logger.Log("初期化 UICtl");

        glubalObjParent = glubalObj.transform;
        uiDic = new Dictionary<UIType, UIBase>();

        //UserTest.E.Init();
        TimeZoneMng.E.Init();
        WorldMng.E.Init();
        AudioMng.Init(glubalObjParent);

        yield return true;
    }

    /// <summary>
    /// グローバルGameObjectのインスタンス
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T CreateGlobalObject<T>() where T : Component
    {
        var obj = new GameObject();
        obj.transform.parent = glubalObjParent;
        var entity = obj.AddComponent<T>();
        obj.name = entity.ToString();

        return entity;
    }

    /// <summary>
    /// UITypeから指定UIをゲット
    /// </summary>
    /// <param name="uiType"></param>
    /// <returns></returns>
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

    /// <summary>
    /// UIをUIコンソールクラスに追加
    /// </summary>
    /// <param name="ui">UI</param>
    /// <param name="uiType">識別タイプ</param>
    public void AddUI(UIBase ui, UIType uiType)
    {
        uiDic[uiType] = ui;
    }

    /// <summary>
    /// UIをオープン
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="uiType"></param>
    /// <param name="closeType"></param>
    /// <returns></returns>
    public T OpenUI<T>(UIType uiType, UIOpenType closeType = UIOpenType.None) where T : UIBase
    {
        return OpenUI<T>(uiType, closeType, null);
    }
    public T OpenUI<T>(UIType uiType, UIOpenType openType, object data) where T : UIBase
    {
        switch (openType)
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

            if (data == null)
                uiClass.Init();
            else
                uiClass.Init(data);

            uiDic[uiType] = uiClass;

        }
        else
        {
            uiClass = uiDic[uiType];
        }

        if (data == null)
            uiClass.Open();
        else
            uiClass.Open(data);

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

    public void DeleteUI(UIType uiType)
    {
        GameObject.Destroy(uiDic[uiType].gameObject);
        uiDic.Remove(uiType);
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
            case UIType.ShopSubscriptionDetails: return "Prefabs/UI/ShopSubscriptionDetails";
            case UIType.Waiting: return "Prefabs/UI/Common/Waiting";
            case UIType.GachaBonus: return "Prefabs/UI/GachaBonus";
            case UIType.Roulette: return "Prefabs/UI/Roulette";
            case UIType.GachaRatio: return "Prefabs/UI/GachaRatio";
            case UIType.DeleteItem: return "Prefabs/UI/DeleteItem";
            case UIType.Friend: return "Prefabs/UI/Friend";
            case UIType.FriendSearch: return "Prefabs/UI/FriendSearch";
            case UIType.FriendDescription: return "Prefabs/UI/FriendDescription";
            case UIType.GachaVerification: return "Prefabs/UI/GachaVerification";
            case UIType.GachaAddBonus: return "Prefabs/UI/GachaAddBonus";
            case UIType.ExchangePoint: return "Prefabs/UI/ExchangePoint";
            case UIType.Mission: return "Prefabs/UI/Mission";
            case UIType.MissionChat: return "Prefabs/UI/MissionChat";
            case UIType.Map: return "Prefabs/UI/Map";
            case UIType.ShopCharge: return "Prefabs/UI/ShopCharge";
            case UIType.ShopGacha: return "Prefabs/UI/ShopGacha";
            case UIType.ShopResource: return "Prefabs/UI/ShopResource";
            case UIType.ShopBlueprint: return "Prefabs/UI/ShopBlueprint";
            case UIType.ShopBlueprintDetails: return "Prefabs/UI/ShopBlueprintDetails";
            case UIType.Chat: return "Prefabs/UI/Chat";



            default: Logger.Error("not find UIType " + ui); return "";
        }
    }

    /// <summary>
    /// UI を操作出来ないようにロック
    /// </summary>
    /// <param name="b">true=ロック　false=アンロック</param>
    public void LockUI(bool b = true)
    {
        if (b)
        {
            if (Waiting == null)
            {
                Waiting = CommonFunction.InstantiateUI<WaitingUI>(GetUIResourcesPath(UIType.Waiting), uiRootTran);
            }
        }
        else
        {
            if (Waiting != null)
            {
                GameObject.Destroy(Waiting.gameObject);
                Waiting = null;
            }
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
    ShopSubscriptionDetails,
    Waiting,
    GachaBonus,
    Roulette,
    GachaRatio,
    DeleteItem,
    Friend,
    FriendSearch,
    FriendDescription,
    GachaVerification,
    GachaAddBonus,
    ExchangePoint,
    Mission,
    MissionChat,
    Map,
    ShopCharge,
    ShopGacha,
    ShopResource,
    ShopBlueprint,
    ShopBlueprintDetails,
    Chat,
    Market,
}
