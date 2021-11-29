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

            if (openType == UIOpenType.OnCloseDestroyObj)
            {
                uiClass.OnCloseDestroyObject = true;
            }
            else
            {
                uiDic[uiType] = uiClass;
            }
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
        return "Prefabs/UI/" + ui.ToString();
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

    #region public function

    /// <summary>
    /// アセットをロード
    /// </summary>
    /// <typeparam name="T">アセットタイプ</typeparam>
    /// <param name="resourcesPath">パス</param>
    /// <returns></returns>
    public static T ReadResources<T>(string resourcesPath) where T : Object
    {
        return ResourcesMng.E.ReadResources<T>(resourcesPath);
    }

    #endregion
}

public enum UIOpenType
{
    None,
    AllClose,
    BeforeClose,
    OnCloseDestroyObj,
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
    Terms,
    Terms01,
    Terms02,
    Charge,
    Craft,
    BlueprintReName,
    PlayDescription,
    PersonalMessage,
    BlueprintPreview,
    
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
    ShopBlueprintMyShopSelectItem,
    ShopBlueprintMyShopUpload,
    Chat,
    Market,
    Equip,
    EquipList,
    SkillExplanation,
    BraveSelectLevel,
    LoginBonus,
}
