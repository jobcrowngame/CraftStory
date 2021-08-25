using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class RuntimeData
{
    public int Coin1 { get; set; } // クラフトシード
    public int Coin2 { get; set; } // クリスタル
    public int Coin3 { get; set; } // ポイント
    public string NickName { get; set; } // ニックネーム
    public string Comment { get; set; } // コメント

    public int NewEmailCount { get; set; } // 新しいメール数
    public int SubscriptionLv01 { get; set; } // サブスクリプション 1
    public int SubscriptionLv02 { get; set; } // サブスクリプション 2
    public int SubscriptionLv03 { get; set; } // サブスクリプション 3
    public DateTime SubscriptionUpdateTime01 { get; set; } // サブスクリプション 更新時間 1
    public DateTime SubscriptionUpdateTime02 { get; set; } // サブスクリプション 更新時間 2
    public DateTime SubscriptionUpdateTime03 { get; set; } // サブスクリプション 更新時間 3

    public int GuideEnd { get; set; } // チュートリアル完了

    public MapType MapType { get; set; } // マップタイプ
    public int GuideId { get; set; } // 進んでるチュートリアルID

    public bool IsPreviev { get; set; } // 今、プレビュー状態タグ
}