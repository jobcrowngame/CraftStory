using UnityEngine.UI;

public class FriendDescriptionUI : UIBase
{
    TitleUI Title { get => FindChiled<TitleUI>("Title"); }
    Text NickName { get => FindChiled<Text>("NickName"); }
    Text Commant { get => FindChiled<Text>("Commant"); }
    Button DeFollowBtn { get => FindChiled<Button>("DeFollowBtn"); }
    Button FollowBtn { get => FindChiled<Button>("FollowBtn"); }

    FriendLG.FriednCell info;

    public override void Init()
    {
        base.Init();
        FriendDescriptionLG.E.Init(this);

        Title.SetTitle("ƒtƒŒƒ“ƒh");
        Title.SetOnClose(Close);

        FollowBtn.onClick.AddListener(()=> 
        {
            FriendDescriptionLG.E.Follow(info.guid);
        });
        DeFollowBtn.onClick.AddListener(() =>
        {
            FriendDescriptionLG.E.DeFollow(info.guid);
        });
    }

    public void Set(FriendLG.FriednCell info, FriendDescriptionLG.UIType type)
    {
        this.info = info;

        NickName.text = info.nickname;
        Commant.text = info.comment;

        FriendDescriptionLG.E.Type = type;
    }

    public void Refresh(FriendDescriptionLG.UIType type)
    {
        FollowBtn.gameObject.SetActive(type == FriendDescriptionLG.UIType.Follow);
        DeFollowBtn.gameObject.SetActive(type == FriendDescriptionLG.UIType.DeFollow);
    }
}
