using System;
using System.Collections.Generic;
using LitJson;

public class FriendLG : UILogicBase<FriendLG, FriendUI>
{
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
    public void Refresh()
    {
        UI.ClearCell();
        if (mType == UIType.Follow)
        {
            ReadFollow();
        }
        else
        {
            ReadFollower();
        }
    }

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

    public struct FriednCell
    {
        public int guid { get; set; }
        public string nickname { get; set; }
        public string comment { get; set; }
        public DateTime loginTime { get; set; }
    }

    public enum UIType
    {
        None = -1,
        Follow,
        Follower,
    }
    public enum SortType
    {
        None = -1,
        Asc = 1,
        Des = 2
    }
}