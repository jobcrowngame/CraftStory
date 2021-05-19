
using UnityEngine.UI;

public class Terms02UI : UIBase
{
    Text TitleText;
    Text MsgText;
    Button OKBtn;

    public override void Init()
    {
        base.Init();

        TitleText = FindChiled<Text>("TitleText");
        TitleText.text = "プライバシーポリシー";

        MsgText = FindChiled<Text>("MsgText");
        MsgText.text = @"bbb
            abba
            aaaa
            aaaaa";

        OKBtn = FindChiled<Button>("OKBtn");
        OKBtn.onClick.AddListener(() => { Close(); });
    }
}
