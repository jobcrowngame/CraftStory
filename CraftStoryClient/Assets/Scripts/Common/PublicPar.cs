using UnityEngine;

public class PublicPar
{

    #region GS2

    // GS2-Identifier で発行したクライアントID
    public static string clientId = "GKIuTdVMtvZILPRylMfno2QXWmZIt2QfCPySFXB2DMdq6UF2JeR7cPBkhAr12XLa3JX";

    // GS2-Identifier で発行したクライアントシークレット
    public static string clientSecret = "aFVsUzd2ZzVycldkRTRoRjVMYXFMRUNQblQ2ekl2dmk=";

    // アカウントを作成する GS2-Account のネームスペース名
    public static string accountNamespaceName = "game-0001";

    // アカウントの認証結果に付与する署名を計算するのに使用する暗号鍵
    public static string accountEncryptionKeyId = "grn:gs2:ap-northeast-1:MmqB2NSz-CraftStory:key:account-encryption-key-namespace:key:account-encryption-key";

    // カテゴリー名
    public static string inventoryNamespaceName = "namespace01";

    // インベントリの種類名
    public static string inventoryName = "Inventory01";

    // 課金通貨ネームスペース名
    public static string moneyNS = "MoneyNS";

    // 交換ネームスペース名
    public static string exchangeNS = "ExchangeNS";

    // 商品ネームスペース名
    public static string ShowcaseNS = "ShowcaseNS";

    // 
    public static string DistributorNS = "DistributorNS";

    #endregion

    public static string LocalURL = "http://localhost/UrlLocal.php";
    public static string TestURL = "13.230.170.40/TestUrl.php";
    public static string ProductionURL = "craftstory.awscraftstoryserver.com/1_2_4.php";

    public static string Maintenance = @"ただいま、メンテナンスを実施中です。
完了までしばらくお待ちください。
ご利用のお客様にはご不便をおかけいたしますが、
何卒ご理解とご協力をよろしくお願い申し上げます。";

    #region Game

    // Dataセーブパース
    public static string SaveRootPath =
#if UNITY_IOS
        Application.persistentDataPath;
#elif UNITY_ANDROID
        Application.persistentDataPath;
#elif UNITY_EDITOR
        Application.dataPath + "/SaveData";
#endif

    public static string MapDataName = "/MapData.dat";
    public static string UserDataName = "/UserData.dat";

    public static string AppStoryURL = "https://apps.apple.com/jp/app/id1571438709";

    // 資金決済法に基づく表示
    public static string UrlBtn1 = "https://www.craftstory.jp/funding/";
    // 特定商取引法に基づく表示
    public static string UrlBtn2 = "https://www.craftstory.jp/specified/";

    #endregion
}
