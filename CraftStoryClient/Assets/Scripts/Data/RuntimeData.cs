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

    /// <summary>
    /// ニックネーム
    /// </summary>
    public string NickName { get; set; }

    /// <summary>
    /// コメント
    /// </summary>
    public string Comment { get; set; }

    public int NewEmailCount { get; set; } // 新しいメール数
    public int SubscriptionLv01 { get; set; } // サブスクリプション 1
    public int SubscriptionLv02 { get; set; } // サブスクリプション 2
    public int SubscriptionLv03 { get; set; } // サブスクリプション 3
    public DateTime SubscriptionUpdateTime01 { get; set; } // サブスクリプション 更新時間 1
    public DateTime SubscriptionUpdateTime02 { get; set; } // サブスクリプション 更新時間 2
    public DateTime SubscriptionUpdateTime03 { get; set; } // サブスクリプション 更新時間 3

    /// <summary>
    /// チュートリアル完了のフラグ 0 = false 1 = true
    /// </summary>
    public int GuideEnd { get; set; }

    /// <summary>
    /// 今のマップタイプ
    /// </summary>
    public MapType MapType { get; set; }
    public int GuideId { get; set; } // 進んでるチュートリアルID

    public bool IsPreviev { get; set; } // 今、プレビュー状態タグ
}