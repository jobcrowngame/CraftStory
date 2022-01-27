using UnityEngine.UI;

public class ReviveUI : UIBase
{
    Text Msg { get => FindChiled<Text>("Msg"); }
    Button ReturnBtn { get => FindChiled<Button>("ReturnBtn"); }
    Button ReviveBtn { get => FindChiled<Button>("ReviveBtn"); }

    string MsgAreaMap = @"
この場所で復活しますか？";

    string MsgAreaBrave = @"
この場所で復活しますか？



※ホームに戻る場合、
　この階で獲得したアイテムは手に入りません";

    public override void Init()
    {
        base.Init();

        ReviveLG.E.Init(this);

        ReviveBtn.onClick.AddListener(() =>
        {
            ReviveBtn.gameObject.SetActive(false);
            GoogleMobileAdsMng.E.ShowReawrd(() =>
            {
                Close();
                // 復活
                CharacterCtl.E.getPlayer().Resurrection();
            });
        });

        ReturnBtn.onClick.AddListener(() =>
        {
            ReviveBtn.gameObject.SetActive(false);
            ReturnBtn.gameObject.SetActive(false);

            CommonFunction.GoToNextScene(100);
        });
    }

    public override void Open()
    {
        base.Open();

        Msg.text = DataMng.E.RuntimeData.MapType == MapType.Brave ? MsgAreaBrave : MsgAreaMap;

        ReviveBtn.gameObject.SetActive(true);
        ReturnBtn.gameObject.SetActive(true);
    }
}
