using UnityEngine;
using UnityEngine.UI;

public class PersonalMessageUI : UIBase
{
    TitleUI title;
    Text Account { get => FindChiled<Text>("Account"); }

    Transform Show { get => FindChiled("Show"); }
    Text NickNameText { get => FindChiled<Text>("NickNameText", Show); }
    Button ChangeBtn { get => FindChiled<Button>("ChangeBtn", Show); }

    Transform ChangeNickName { get => FindChiled("ChangeNickName"); }
    InputField InputField { get=> FindChiled<InputField>("InputField", ChangeNickName); }
    Button OKBtn { get => FindChiled<Button>("OKBtn", ChangeNickName); }

    public override void Init()
    {
        base.Init();
        PersonalMessageLG.E.Init(this);

        title = FindChiled<TitleUI>("Title");
        title.SetTitle("プロフィール");
        title.SetOnClose(() => { Close(); GuideLG.E.Next(); });
        title.EnActiveCoin(1);
        title.EnActiveCoin(2);
        title.EnActiveCoin(3);

        ChangeBtn.onClick.AddListener(() => 
        {
            Show.gameObject.SetActive(false);
            ChangeNickName.gameObject.SetActive(true);

            GuideLG.E.Next();
        });
        OKBtn.onClick.AddListener(() =>
        {
            if (InputField.text.Length < 1)
            {
                CommonFunction.ShowHintBar(10);
                return;
            }

            if (DataMng.E.RuntimeData.MapType == MapType.Guide)
            {
                DataMng.E.UserData.NickName = InputField.text;
                NickNameText.text = DataMng.E.UserData.NickName;

                Show.gameObject.SetActive(true);
                ChangeNickName.gameObject.SetActive(false);

                CommonFunction.ShowHintBar(11);

                GuideLG.E.Next();
            }
            else
            {
                NWMng.E.UpdateNickName((rp) =>
                {
                    DataMng.E.UserData.NickName = InputField.text;
                    NickNameText.text = DataMng.E.UserData.NickName;

                    Show.gameObject.SetActive(true);
                    ChangeNickName.gameObject.SetActive(false);

                    CommonFunction.ShowHintBar(11);
                }, InputField.text);
            }
        });

        InputField.onEndEdit.AddListener((r) => 
        {
            if (InputField.text.Length < 1)
            {
                CommonFunction.ShowHintBar(10);
                return;
            }

            GuideLG.E.Next();
        });
    }
    public override void Open()
    {
        base.Open();
        Account.text = DataMng.E.UserData.Account;
        NickNameText.text = DataMng.E.UserData.NickName;
    }
}
