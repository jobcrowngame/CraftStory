using System;
using System.Collections.Generic;
using LitJson;

/// <summary>
/// フレンドロジック
/// </summary>
public class FriendLG : UILogicBase<FriendLG, FriendUI>
{
    /// <summary>
    /// Windowタイプ
    /// </summary>
    public UIType Type 
    {
        get => mType;
        set
        {
            mType = value;

            Refresh();
        }
    }
    private UIType mType = UIType.None;

    /// <summary>
    /// ソートタイプ
    /// </summary>
    public SortType Sort 
    {
        get => mSort;
        set
        {
            mSort = value;
            Refresh();
        }
    }
    private SortType mSort = SortType.None;

    public override void Init(FriendUI ui)
    {
        base.Init(ui);
        Sort = SortType.Asc;
        Type = UIType.Follow;
    }

    /// <summary>
    /// Windowを更新
    /// </summary>
    public void Refresh()
    {
        if (mType == UIType.Follow)
        {
            // フォロー
            ReadFollow();
        }
        else
        {
            // フォロワー
            ReadFollower();
        }
    }

    /// <summary>
    /// フォローリストをゲット
    /// </summary>
    public void ReadFollow()
    {
        NWMng.E.ReadFollow((rp) => 
        {
            if (string.IsNullOrEmpty(rp.ToString()))
                return;

            var list = JsonMapper.ToObject<List<FriednCell>>(rp.ToJson());
            UI.RefreshCell(list);
        });
    }

    /// <summary>
    /// フォロワーリストをゲット
    /// </summary>
    public void ReadFollower()
    {
        NWMng.E.ReadFollower((rp) =>
        {
            if (string.IsNullOrEmpty(rp.ToString()))
                return;

            var list = JsonMapper.ToObject<List<FriednCell>>(rp.ToJson());
            UI.RefreshCell(list);
        });
    }

    /// <summary>
    /// 通信用構造体
    /// </summary>
    public struct FriednCell
    {
        public int guid { get; set; }
        public string nickname { get; set; }
        public string comment { get; set; }
        public DateTime loginTime { get; set; }
    }

    /// <summary>
    /// Windowタイプ
    /// </summary>
    public enum UIType
    {
        None = -1,
        /// <summary>
        /// フォロー
        /// </summary>
        Follow,

        /// <summary>
        /// フォロワー
        /// </summary>
        Follower,
    }

    /// <summary>
    /// ソートタイプ
    /// </summary>
    public enum SortType
    {
        None = -1,

        Asc = 1,
        Des = 2
    }
}