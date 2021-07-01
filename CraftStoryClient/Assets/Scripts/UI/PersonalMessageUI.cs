using UnityEngine.UI;

public class PersonalMessageUI : UIBase
{
    TitleUI title;
    Text Account { get => FindChiled<Text>("Account"); }

    public override void Init()
    {
        base.Init();
        PersonalMessageLG.E.Init(this);

        title = FindChiled<TitleUI>("Title");
        title.SetTitle("ŒÂlî•ñ");
        title.SetOnClose(() => { Close(); });
        title.EnActiveCoin(1);
        title.EnActiveCoin(2);
        title.EnActiveCoin(3);

        Account.text = DataMng.E.UserData.Account;
    }
}
