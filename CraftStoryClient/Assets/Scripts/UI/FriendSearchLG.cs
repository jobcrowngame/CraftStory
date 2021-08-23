
using LitJson;

public class FriendSearchLG : UILogicBase<FriendSearchLG, FriendSearchUI>
{
    public void FriendSearch(string userAcc)
    {
        NWMng.E.SearchFriend((rp) =>
        {
            var info = JsonMapper.ToObject<FriendLG.FriednCell>(rp.ToJson());
            var ui = UICtl.E.OpenUI<FriendDescriptionUI>(UIType.FriendDescription);
            if (ui != null)
            {
                ui.Set(info, FriendDescriptionLG.UIType.Follow);
            }
        }, userAcc);
    }
}