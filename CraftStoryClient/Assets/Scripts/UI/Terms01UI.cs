
using UnityEngine.UI;

public class Terms01UI : UIBase
{
    Text TitleText;
    Text MsgText;
    Button OKBtn;

    public override void Init()
    {
        base.Init();

        TitleText = FindChiled<Text>("TitleText");
        TitleText.text = "利用規約";

        MsgText = FindChiled<Text>("MsgText");

        OKBtn = FindChiled<Button>("OKBtn");
        OKBtn.onClick.AddListener(() => { Close(); });
    }
}
