

public class FriendDescriptionLG : UILogicBase<FriendDescriptionLG, FriendDescriptionUI>
{
    public UIType Type
    {
        get => mType;
        set
        {
            mType = value;
            UI.Refresh(mType);
        }
    }
    private UIType mType = UIType.None;

    public void Follow(int guid)
    {
        NWMng.E.Follow((rp) => 
        {
            CommonFunction.ShowHintBar(24);
            FriendLG.E.Refresh();
            UI.Close();
        }, guid);
    }

    public void DeFollow(int guid)
    {
        NWMng.E.DeFollow((rp) =>
        {
            CommonFunction.ShowHintBar(25);
            FriendLG.E.Refresh();
            UI.Close();
        }, guid);
    }

    public enum UIType
    {
        None = -1,
        Follow,
        DeFollow,
    }
}