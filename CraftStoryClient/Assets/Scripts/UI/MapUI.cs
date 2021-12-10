using UnityEngine;
using UnityEngine.UI;

public class MapUI : UIBase
{
    Button CloseBtn { get => FindChiled<Button>("CloseBtn"); }
    Button HomeBtn { get => FindChiled<Button>("HomeBtn"); }
    Button MarketBtn { get => FindChiled<Button>("MarketBtn"); }
    Button BraveBtn { get => FindChiled<Button>("BraveBtn"); }
    Button BraveBtn2 { get => FindChiled<Button>("BraveBtn2"); }
    Button EquipBtn { get => FindChiled<Button>("EquipBtn"); }
    Transform SpriteAnim { get => FindChiled("SpriteAnim"); }
    Transform SpriteAnim4 { get => FindChiled("SpriteAnim4"); }
    Transform SpriteAnim5 { get => FindChiled("SpriteAnim5"); }

    public override void Init()
    {
        base.Init();

        CloseBtn.onClick.AddListener(Close);
        HomeBtn.onClick.AddListener(OnClickHomeBtn);
        MarketBtn.onClick.AddListener(OnClickMarketBtn);
        BraveBtn.onClick.AddListener(OnClickBraveBtn);
        BraveBtn2.onClick.AddListener(OnClickBraveBtn2);
        EquipBtn.onClick.AddListener(OnClickEquipBtn);
    }

    public override void Open()
    {
        base.Open();

        EnActiveBtn(HomeBtn, DataMng.E.RuntimeData.MapType == MapType.Home);
        EnActiveBtn(MarketBtn, DataMng.E.RuntimeData.MapType == MapType.Market);
        EnActiveBtn(BraveBtn, DataMng.E.RuntimeData.MapType == MapType.Brave);
        EnActiveBtn(BraveBtn2, DataMng.E.RuntimeData.MapType == MapType.Event);

        //SpriteAnim.gameObject.SetActive(DataMng.E.RuntimeData.MapType != MapType.Guide && DataMng.E.RuntimeData.GuideEnd2 == 0);
        SpriteAnim.gameObject.SetActive(DataMng.E.RuntimeData.MapType != MapType.Guide);
        SpriteAnim4.gameObject.SetActive(MapLG.E.IsEquipTutorial());
        SpriteAnim5.gameObject.SetActive(DataMng.E.RuntimeData.MapType != MapType.Guide && DataMng.E.RuntimeData.GuideEnd5 == 0);

        // 冒険エリアの場合、時間を停止
        if (DataMng.E.RuntimeData.MapType == MapType.Brave)
        {
            PlayerCtl.E.Pause();
        }
    }
    public override void Close()
    {
        base.Close();

        PlayerCtl.E.Pause(false);
    }

    private void OnClickHomeBtn()
    {
        GuideLG.E.Next();

        // ホームにからホーム遷移できません
        if (DataMng.E.RuntimeData.MapType == MapType.Home)
            return;

        // 冒険途中でホーム戻る場合、ボーナス計算します
        if (DataMng.E.RuntimeData.MapType == MapType.Brave)
        {
            AdventureCtl.E.GetBonus(() =>
            {
                CommonFunction.GoToNextScene(100);
            });
        }
        else
        {
            CommonFunction.GoToNextScene(100);
        }
    }
    private void OnClickMarketBtn()
    {
        if (DataMng.E.RuntimeData.GuideEnd2 == 0)
        {
            DataMng.E.RuntimeData.GuideId = 2;
            CommonFunction.GoToNextScene(104);
        }
        else
        {
            if (DataMng.E.RuntimeData.MapType == MapType.Market)
                return;

            if (DataMng.E.RuntimeData.MapType == MapType.Brave)
            {
                AdventureCtl.E.GetBonus(() =>
                {
                    CommonFunction.GoToNextScene(103);
                });
            }
            else
            {
                CommonFunction.GoToNextScene(103);
            }
        }
    }
    private void OnClickBraveBtn ()
    {
        if (!PlayerCtl.E.Character.IsEquipedEquipment())
        {
            OnClickEquipBtn();
            return;
        }

        if (DataMng.E.RuntimeData.GuideEnd5 == 0)
        {
            DataMng.E.RuntimeData.GuideId = 5;
            CommonFunction.GoToNextScene(107);
        }
        else
        {
            if (DataMng.E.RuntimeData.MapType != MapType.Brave)
            {
                UICtl.E.OpenUI<BraveSelectLevelUI>(UIType.BraveSelectLevel);
            }
        }
    }
    private void OnClickBraveBtn2()
    {
        if (!PlayerCtl.E.Character.IsEquipedEquipment())
        {
            OnClickEquipBtn();
            return;
        }

        if (DataMng.E.RuntimeData.GuideEnd5 == 0)
        {
            DataMng.E.RuntimeData.GuideId = 5;
            CommonFunction.GoToNextScene(107);
        }
        else
        {
            CommonFunction.GoToNextScene(2009);
        }
    }
    private void OnClickEquipBtn()
    {
        if (MapLG.E.IsEquipTutorial())
        {
            DataMng.E.RuntimeData.GuideId = 4;
            CommonFunction.GoToNextScene(106);
        }
        else
        {
            UICtl.E.OpenUI<EquipUI>(UIType.Equip, UIOpenType.BeforeClose);
        }
    }

    private void EnActiveBtn(Button btn, bool b = true)
    {
        btn.GetComponent<Image>().color = b ?
            Color.gray :
            Color.white;
        btn.enabled = !b;
    }
}
