using System;

public class RuntimeData
{
    /// <summary>
    /// ニックネーム
    /// </summary>
    public string NickName { get; set; }

    /// <summary>
    /// 経験値
    /// </summary>
    public float Exp { get; set; }

    /// <summary>
    /// 今のマップタイプ
    /// </summary>
    public MapType MapType { get; set; }
    public int GuideId { get; set; } // 進んでるチュートリアルID

    public bool IsPreviev { get; set; } // 今、プレビュー状態タグ

    /// <summary>
    /// 新規アカウントのフラグ
    /// </summary>
    public bool IsNewUser { get; set; }

    /// <summary>
    /// トータル設置済ブロック数
    /// </summary>
    public int TotalSetBlockCount { get; set; }
}