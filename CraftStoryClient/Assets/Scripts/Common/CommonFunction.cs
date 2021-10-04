using System;
using System.Collections.Generic;
using System.Text;

using UnityEngine;
using UnityEngine.SceneManagement;

using LitJson;

public class CommonFunction
{
    /// <summary>
    /// ゲーム終了場合の共通メソッド
    /// </summary>
    public static void QuitGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// サブObjectを取得共通メソッド
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="parent">親のObject</param>
    /// <param name="name">Object名</param>
    /// <returns></returns>
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

    /// <summary>
    /// 全子供Objectから検索
    /// </summary>
    /// <param name="parent">親のObject</param>
    /// <param name="name">Object名</param>
    /// <returns></returns>
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

    /// <summary>
    /// 全サブObjectを取得
    /// </summary>
    /// <param name="parent">親のObject</param>
    /// <param name="list">サブリスト</param>
    public static void GetAllChiled(Transform parent, ref List<GameObject> list)
    {
        foreach (Transform item in parent)
        {
            if (item.childCount > 0)
            {
                GetAllChiled(item, ref list);
            }
            else
            {
                list.Add(item.gameObject);
            }
        }
    }

    /// <summary>
    /// GameObjectをインスタンス化
    /// </summary>
    /// <param name="path">ソースのパス</param>
    /// <param name="parent">親のObject</param>
    /// <param name="pos">インスタンス座標</param>
    /// <returns></returns>
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

    /// <summary>
    /// UI  GameObjectをインスタンス化
    /// </summary>
    /// <typeparam name="T">UIスクリプト</typeparam>
    /// <param name="resourcesPath">ソースのパス</param>
    /// <param name="parent">親のObject</param>
    /// <returns></returns>
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

    /// <summary>
    /// 全サブObjectを削除
    /// </summary>
    /// <param name="parent">親のObject</param>
    public static void ClearCell(Transform parent)
    {
        foreach (Transform t in parent)
        {
            GameObject.Destroy(t.gameObject);
        }
    }

    /// <summary>
    /// 次Sceneに遷移
    /// </summary>
    /// <param name="TransferGateID">転送門ID</param>
    public static void GoToNextScene(int TransferGateID)
    {
        UICtl.E.Clear();
        if (DataMng.E.MapData != null) DataMng.E.MapData.ClearMapObj();
        PlayerCtl.E.SelectItem = null;

        var config = ConfigMng.E.TransferGate[TransferGateID];

        // 宝マップに入るかを判断
        var random = UnityEngine.Random.Range(0, 100f);
        if (random < config.TreasureMapPercent)
        {
            NowLoadingLG.E.BeforTransferGateID = ConfigMng.E.Map[config.NextMap].TransferGateID;
            config = ConfigMng.E.TransferGate[config.TreasureMap];
        }

        NowLoadingLG.E.NextMapID = config.NextMap;
        NowLoadingLG.E.NextSceneName = config.NextMapSceneName;

        DataMng.E.RuntimeData.MapType = (MapType)ConfigMng.E.Map[config.NextMap].MapType;

        // Scene遷移
        SceneManager.LoadSceneAsync("NowLoading");
    }

    /// <summary>
    /// フレンドホームに行く
    /// </summary>
    /// <param name="userGuid">ユーザーGUID</param>
    public static void GotoFriendHome(int userGuid)
    {
        NWMng.E.GetFriendHomeData((rp) => 
        {
            if (string.IsNullOrEmpty(rp.ToString()))
                return;

            var homeData = (string)rp["homedata"];
            DataMng.E.SetMapData(new MapData(homeData), MapType.FriendHome);

            GoToNextScene(102);
        }, userGuid);
    }

    /// <summary>
    /// ベクターの計算
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    /// <returns></returns>
    public static Vector3Int Vector3Sum(Vector3Int v1, Vector3Int v2)
    {
        return new Vector3Int(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
    }

    /// <summary>
    /// プレイヤーの座標とインスタンスエンティティの座標から向きを取得
    /// </summary>
    /// <param name="createPos">インスタンス座標</param>
    /// <returns></returns>
    public static Direction GetCreateEntityDirection(Vector3 createPos)
    {
        var playerPos = PlayerCtl.E.PlayerEntity.transform.position;

        var angle = Vector2ToAngle(new Vector2(createPos.x, createPos.z) - new Vector2(playerPos.x, playerPos.z)) + 180;

        Direction dType = Direction.down;

        if (angle >= 223 && angle < 315)
            dType = Direction.back;
        else if (angle >= 135 && angle < 225)
            dType = Direction.left;
        else if (angle >= 45 && angle < 135)
            dType = Direction.foward;
        else
            dType = Direction.right;

        return dType;
    }

    /// <summary>
    /// 向きからインスタンスエンティティの角度を取得
    /// </summary>
    /// <param name="dType">向き</param>
    /// <returns></returns>
    public static int GetCreateEntityAngleByDirection(Direction dType)
    {
        int angle = 0;
        switch (dType)
        {
            case Direction.back: angle = 180; break;
            case Direction.right: angle = 90; break;
            case Direction.left: angle = 270; break;
        }
        return angle;
    }
    /// <summary>
    /// 角度から単位ベクトルを取得
    /// </summary>
    public static Vector2 AngleToVector2(float angle)
    {
        var radian = angle * (Mathf.PI / 180);
        return new Vector2(Mathf.Sin(radian), Mathf.Cos(radian)).normalized;
    }
    /// <summary>
    /// ベクトルから角度を取得
    /// </summary>
    public static float Vector2ToAngle(Vector2 vector)
    {
        return Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;
    }

    /// <summary>
    /// ボーナスリストからアイテムリストを取得
    /// </summary>
    /// <param name="bonusIds">ボーナスリスト</param>
    /// <returns></returns>
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

    /// <summary>
    /// コードによってメッセージを表しするヒントバー
    /// </summary>
    /// <param name="errCode">コード</param>
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

    /// <summary>
    /// ヒントボックス
    /// 重要なメッセージを確認するメッセージボックス
    /// </summary>
    /// <param name="msg">メッセージ</param>
    /// <param name="okAction">Okボタンをクリック場合のアクション</param>
    /// <param name="cancelAction">Cancelボタンをクリック場合のアクション</param>
    /// <param name="okBtn">他のボタン画像に交換する場合のテクスチャ名</param>
    /// <param name="cancelBtn">他のボタン画像に交換する場合のテクスチャ名</param>
    public static void ShowHintBox(string msg, Action okAction, Action cancelAction = null, 
        string okBtn = "button_2D_007", string cancelBtn = "button_2D_006")
    {
        ShowHintBox(null, msg, okAction, cancelAction, okBtn, cancelBtn);
    }
    // メッセージボックスにアイコンを追加
    public static void ShowHintBox(string iconPath, string msg, Action okAction, Action cancelAction = null, 
        string okBtn = "button_2D_007", string cancelBtn = "button_2D_006")
    {
        ShowHintBox("", iconPath, msg, okAction, cancelAction, okBtn, cancelBtn);
    }
    public static HintBoxUI ShowHintBox(string title, string iconPath, string msg, Action okAction, Action cancelAction = null,
        string okBtn = "button_2D_007", string cancelBtn = "button_2D_006")
    {
        var ui = InstantiateUI<HintBoxUI>("Prefabs/UI/Common/HintBox", UICtl.E.Root);
        if (ui != null)
        {
            ui.Set(iconPath, msg, okAction, cancelAction);
            ui.SetTitle(title);
            ui.SetBtnName(okBtn, cancelBtn);
        }
        return ui;
    }

    /// <summary>
    /// びっくりマックを表しするかの判断
    /// </summary>
    /// <returns></returns>
    public static bool MenuRedPoint()
    {
        bool ret = false;
        if (!ret) ret = NewMessage();
        return ret;
    }
    public static bool NewMessage()
    {
        return DataMng.E.RuntimeData.NewEmailCount > 0
            ? true
            : false;
    }

    /// <summary>
    /// メンテナンス場合の動作
    /// </summary>
    public static void Maintenance()
    {
        ShowHintBox(PublicPar.Maintenance, () => { QuitGame(); });
    }

    /// <summary>
    /// バージョンアップ場合の動作
    /// </summary>
    /// <param name="ver"></param>
    public static void VersionUp(string ver)
    {
        string msg = string.Format(@"アプリバージョンが古いです。
最新のバージョンに更新してください。

今のバージョン: v.{0}
最新のバージョン: v.{1}",
        Application.version, ver);

        ShowHintBox(msg, () =>
        {
#if UNITY_IOS
            Application.OpenURL(PublicPar.AppStoryURL_IOS);
#elif UNITY_ANDROID
            Application.OpenURL(PublicPar.AppStoryURL_Android);
#endif
            QuitGame();
        });
    }


    /// <summary>
    /// 指定された文字列がメールアドレスとして正しい形式か検証する
    /// </summary>
    /// <param name="address">検証する文字列</param>
    /// <returns>正しい時はTrue。正しくない時はFalse。</returns>
    public static bool IsValidMailAddress(string address)
    {
        if (string.IsNullOrEmpty(address))
        {
            return false;
        }

        try
        {
            System.Net.Mail.MailAddress a =
                new System.Net.Mail.MailAddress(address);
        }
        catch (FormatException)
        {
            //FormatExceptionがスローされた時は、正しくない
            return false;
        }

        return true;
    }


    /// <summary>
    /// テクスチャからSpriteに変換
    /// </summary>
    /// <param name="texture">テクスチャ</param>
    public static Sprite Texture2dToSprite(Texture2D texture)
    {
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
    }

    /// <summary>
    /// ファイル名をゲット
    /// </summary>
    /// <returns></returns>
    public static string GetTextureName()
    {
        string fileName = string.Format("Blueprint-{0}-{1}", 
            DataMng.E.UserData.Account,
            DateTime.Now.ToString("yyyyMMddhhmmss"));
        return fileName;
    }

    /// <summary>
    /// 距離をゲット
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <returns></returns>
    public static float GetDistance(Vector3 p1, Vector3 p2)
    {
        return Mathf.Abs(Vector3.Distance(p1, p2));
    }
}
