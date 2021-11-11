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

    Text MyGoodNum { get => FindChiled<Text>("MyGoodNum"); }

    Transform Comment { get => FindChiled("Comment"); }
    InputField CommentInputField { get => FindChiled<InputField>("CommentInputField", Comment); }


    public override void Init()
    {
        base.Init();
        PersonalMessageLG.E.Init(this);

        title = FindChiled<TitleUI>("Title");
        title.SetTitle("プロフィール");
        title.SetOnClose(() => { Close(); GuideLG.E.Next(); });

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

            NWMng.E.UpdateNickName((rp) =>
            {
                DataMng.E.RuntimeData.NickName = InputField.text;
                NickNameText.text = DataMng.E.RuntimeData.NickName;

                Show.gameObject.SetActive(true);
                ChangeNickName.gameObject.SetActive(false);

                CommonFunction.ShowHintBar(11);
                GuideLG.E.Next();
            }, InputField.text);
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

        CommentInputField.onEndEdit.AddListener((text) => 
        {
            if (string.IsNullOrEmpty(text))
            {
                CommonFunction.ShowHintBar(22);
                return;
            }

            NWMng.E.UpdateComment((rp) => 
            {
                CommonFunction.ShowHintBar(23);
                DataMng.E.RuntimeData.Comment = text;
            }, text);
        });
    }
    public override void Open()
    {
        base.Open();
        Account.text = DataMng.E.UserData.Account;
        NickNameText.text = DataMng.E.RuntimeData.NickName;
        InputField.text = DataMng.E.RuntimeData.NickName;
        CommentInputField.text = DataMng.E.RuntimeData.Comment;
        MyGoodNum.text = DataMng.E.RuntimeData.MyGoodNum.ToString();
    }
}
