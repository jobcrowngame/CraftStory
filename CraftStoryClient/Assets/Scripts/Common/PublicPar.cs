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

    public static string URL = "13.230.170.40/Main.php";
    public static string LocalURL = "http://localhost/Game/Main.php";

    #region Game

    // Dataセーブパース
    public static string SaveRootPath =
#if UNITY_IOS
        Application.persistentDataPath;
#elif UNITY_EDITOR
        Application.persistentDataPath + /SaveData;
#endif

    #endregion
}
