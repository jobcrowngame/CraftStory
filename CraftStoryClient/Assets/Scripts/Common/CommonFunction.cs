using System;
using System.Collections.Generic;
using System.Text;

using UnityEngine;
using UnityEngine.SceneManagement;

using LitJson;
using System.Linq;

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
    /// グローバルGameObjectのインスタンス
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T CreateGlobalObject<T>() where T : Component
    {
        Transform parent = Main.E == null ? null : Main.E.transform;
        var obj = new GameObject();
        obj.transform.parent = parent;
        var entity = obj.AddComponent<T>();
        obj.name = entity.ToString();

        return entity;
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
        obj.transform.localPosition = pos;

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
        AdventureCtl.E.Clear();
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

        // ローカルデータをセーブ
        if (DataMng.E != null) DataMng.E.Save();

        DataMng.E.RuntimeData.MapType = (MapType)ConfigMng.E.Map[config.NextMap].MapType;

        // Objectをクリア
        WorldMng.E.Clear();

        GC.Collect();

        // Scene遷移
        SceneManager.LoadSceneAsync("NowLoading");

    }
    public static void GotoAreaMap()
    {
        AdventureCtl.E.Clear();
        UICtl.E.Clear();
        if (DataMng.E.MapData != null) DataMng.E.MapData.ClearMapObj();
        PlayerCtl.E.SelectItem = null;

        NowLoadingLG.E.NextSceneName = "AreaMap";
        DataMng.E.RuntimeData.MapType = MapType.AreaMap;

        // Objectをクリア
        WorldMng.E.Clear();

        // ローカルデータをセーブ
        if (DataMng.E != null) DataMng.E.Save();

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
    /// プレイヤーの座標とインスタンスエンティティの座標から向きを取得
    /// </summary>
    /// <param name="createPos">インスタンス座標</param>
    /// <returns></returns>
    public static Direction GetCreateEntityDirection(Vector3 createPos)
    {
        var playerPos = PlayerCtl.E.Character.transform.position;

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
    public static void ShowHintBoxInTitle(string title, string msg, Action okAction, Action cancelAction = null,
        string okBtn = "button_2D_007", string cancelBtn = "button_2D_006")
    {
        ShowHintBox(title, null, msg, okAction, cancelAction, okBtn, cancelBtn);
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

    /// <summary>
    /// 向きをゲット
    /// </summary>
    /// <param name="p1">start</param>
    /// <param name="p2">end</param>
    /// <returns></returns>
    public static Vector2 GetDirection(Vector3 p1, Vector3 p2)
    {
        return new Vector2(p1.x - p2.x, p1.z - p2.z);
    }


    /// <summary>
    /// RandomBonusPondIdからBonusListをゲット
    /// </summary>
    /// <param name="pondId"></param>
    /// <returns></returns>
    public static int GetBonusByPondId(int pondId)
    {
        var config = ConfigMng.E.RandomBonusPond[pondId];
        var percentR = UnityEngine.Random.Range(0, 10000);

        string bonusListStr = "";
        int curPercent = config.Percent01;
        if (percentR <= config.Percent01) bonusListStr = config.BonusList01;
        else if (percentR <= config.Percent01 + config.Percent02) bonusListStr = config.BonusList02;
        else if (percentR <= config.Percent01 + config.Percent02 + config.Percent03) bonusListStr = config.BonusList03;
        else if (percentR <= config.Percent01 + config.Percent02 + config.Percent03 + config.Percent04) 
            bonusListStr = config.BonusList04;
        else if (percentR <= config.Percent01 + config.Percent02 + config.Percent03 + config.Percent04 + 
            config.Percent05) 
            bonusListStr = config.BonusList05;
        else if (percentR <= config.Percent01 + config.Percent02 + config.Percent03 + config.Percent04 + 
            config.Percent05 + config.Percent06) 
            bonusListStr = config.BonusList06;
        else if (percentR <= config.Percent01 + config.Percent02 + config.Percent03 + config.Percent04 + 
            config.Percent05 + config.Percent06 + config.Percent07) 
            bonusListStr = config.BonusList07;
        else bonusListStr = "";

        if (!string.IsNullOrEmpty(bonusListStr))
        {
            var bonusArr = bonusListStr.Split(',');
            var bonusR = UnityEngine.Random.Range(0, bonusArr.Length);
            return int.Parse(bonusArr[bonusR]);
        }
        else
        {
            return -1;
        }
    }

    #region Vector

    /// <summary>
    /// 目標がRect範囲ないにあるかのチェック
    /// </summary>
    /// <param name="selectPos">向き</param>
    /// <param name="targetPos">目標座標</param>
    /// <param name="maxDistance">最大距離</param>
    /// <param name="radius">半径</param>
    public static bool TargetPosInRect(Vector3 selectPos, Vector3 startPos, Vector3 targetPos, float maxDistance, float radius)
    {
        Debug.DrawLine(startPos, targetPos, Color.blue, 5);
        Debug.DrawLine(startPos, selectPos, Color.red, 5);

        var angle = Vector3.Angle(selectPos - startPos, targetPos - startPos);
        var c = Vector3.Distance(startPos, targetPos);
        var b = Math.Sin(angle * (Math.PI / 180)) * c;
        var distance = Vector3.Distance(startPos, targetPos);

        if (b <= radius && angle < 90)
        {
            if (distance < maxDistance)
            {
                Logger.Log("b:{0}, distance:{1}", b, distance);
            }
            else
            {
                Logger.Warning("distance:{0}", distance);
            }
        }

        return angle < 90 && distance < maxDistance && b < radius;
    }

    public static string Vector3ToString(Vector3 pos)
    {
        return string.Format("{0},{1},{2}", pos.x, pos.y, pos.z);
    }
    public static Vector3 StringToVector3(string posStr)
    {
        string[] pos = posStr.Split(',');
        return new Vector3(int.Parse(pos[0]), int.Parse(pos[1]), int.Parse(pos[2]));
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
    /// ベクターの計算
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    /// <returns></returns>
    public static Vector3Int Vector3Sum(Vector3Int v1, Vector3Int v2)
    {
        return new Vector3Int(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
    }

    #endregion


    #region Item 

    /// <summary>
    /// アイテムが装備かをチェック
    /// </summary>
    /// <param name="itemId"></param>
    /// <returns></returns>
    public static bool IsEquipment(int itemId)
    {
        var itemConfig = ConfigMng.E.Item[itemId];

        return (ItemType)itemConfig.Type == ItemType.Weapon
            || (ItemType)itemConfig.Type == ItemType.Armor;
    }

    /// <summary>
    /// Equipmentタイプによって装備位置をゲット
    /// </summary>
    /// <param name="itemType"></param>
    /// <returns></returns>
    public static ItemSite GetEquipSite(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.Weapon: return ItemSite.Weapon;
            case ItemType.Armor: return ItemSite.Armor;

            default: return ItemSite.None;
        }
    }

    public static Dictionary<int,int> GetRandomBonus(int randomBonusId)
    {
        Dictionary<int, int> dic = new Dictionary<int, int>();
        var config = ConfigMng.E.RandomBonus[randomBonusId];

        GetRandomBonusCell(config.Pond01, config.Count01, ref dic);
        GetRandomBonusCell(config.Pond02, config.Count02, ref dic);
        GetRandomBonusCell(config.Pond03, config.Count03, ref dic);

        return dic;
    }
    private static void GetRandomBonusCell(int pondId, int count, ref Dictionary<int, int> dic)
    {
        for (int i = 0; i < count; i++)
        {
            var list = new List<int>();
            GetRandomBonusPond(pondId, ref list);
            for (int j = 0; j < list.Count; j++)
            {
                if (dic.ContainsKey(list[j]))
                {
                    dic[list[j]]++;
                }
                else
                {
                    dic[list[j]] = 1;
                }
            }
        }
    }

    public static void GetRandomBonusPond(int pondId, ref List<int> bonusList)
    {
        var config = ConfigMng.E.RandomBonusPond[pondId];

        GetRandomBonusByPond(config.BonusList01, config.Percent01, ref bonusList);
        GetRandomBonusByPond(config.BonusList02, config.Percent02, ref bonusList);
        GetRandomBonusByPond(config.BonusList03, config.Percent03, ref bonusList);
        GetRandomBonusByPond(config.BonusList04, config.Percent04, ref bonusList);
        GetRandomBonusByPond(config.BonusList05, config.Percent05, ref bonusList);
        GetRandomBonusByPond(config.BonusList06, config.Percent06, ref bonusList);
        GetRandomBonusByPond(config.BonusList07, config.Percent07, ref bonusList);
    }
    private static void GetRandomBonusByPond(string bonusStr, int percent, ref List<int> bonusList)
    {
        var penrcentRange = UnityEngine.Random.Range(0, 10000);
        if (penrcentRange <= percent)
        {
            string[] bonusArr = bonusStr.Split(',');
            var range = UnityEngine.Random.Range(0, bonusArr.Length);
            bonusList.Add(int.Parse(bonusArr[range]));
        }
    }

    public static Dictionary<int, int> GetBonusDic(int bonusId)
    {
        Dictionary<int, int> dic = new Dictionary<int, int>();
        var config = ConfigMng.E.Bonus[bonusId];
        if (config.Bonus1 > 0) dic[config.Bonus1] = config.BonusCount1;
        if (config.Bonus2 > 0) dic[config.Bonus2] = config.BonusCount2;
        if (config.Bonus3 > 0) dic[config.Bonus3] = config.BonusCount3;
        if (config.Bonus4 > 0) dic[config.Bonus4] = config.BonusCount4;
        if (config.Bonus5 > 0) dic[config.Bonus5] = config.BonusCount5;
        if (config.Bonus6 > 0) dic[config.Bonus6] = config.BonusCount6;
        return dic;
    }
    #endregion 
}
