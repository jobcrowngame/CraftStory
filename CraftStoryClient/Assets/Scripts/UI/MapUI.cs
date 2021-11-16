using UnityEngine;
using UnityEngine.UI;

public class MapUI : UIBase
{
    Button CloseBtn { get => FindChiled<Button>("CloseBtn"); }
    Button HomeBtn { get => FindChiled<Button>("HomeBtn"); }
    Button MarketBtn { get => FindChiled<Button>("MarketBtn"); }
    Button BraveBtn { get => FindChiled<Button>("BraveBtn"); }
    Button EquipBtn { get => FindChiled<Button>("EquipBtn"); }
    Transform SpriteAnim { get => FindChiled("SpriteAnim"); }
    Transform SpriteAnim4 { get => FindChiled("SpriteAnim4"); }

    public override void Init()
    {
        base.Init();

        CloseBtn.onClick.AddListener(Close);

        HomeBtn.onClick.AddListener(() =>
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
        });

        MarketBtn.onClick.AddListener(() =>
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
        });

        BraveBtn.onClick.AddListener(() =>
        {
            if (!PlayerCtl.E.Character.IsEquipedEquipment())
            {
                CommonFunction.ShowHintBar(33);
                return;
            }

            // 冒険入るのミッション
            NWMng.E.ClearMission(2, 1);

            if (DataMng.E.RuntimeData.MapType != MapType.Brave)
                CommonFunction.GoToNextScene(1000);
        });

        EquipBtn.onClick.AddListener(() =>
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
        });
    }

    public override void Open()
    {
        base.Open();

        EnActiveBtn(HomeBtn, DataMng.E.RuntimeData.MapType == MapType.Home);
        EnActiveBtn(MarketBtn, DataMng.E.RuntimeData.MapType == MapType.Market);
        EnActiveBtn(BraveBtn, DataMng.E.RuntimeData.MapType == MapType.Brave);

        //SpriteAnim.gameObject.SetActive(DataMng.E.RuntimeData.MapType != MapType.Guide && DataMng.E.RuntimeData.GuideEnd2 == 0);
        SpriteAnim.gameObject.SetActive(DataMng.E.RuntimeData.MapType != MapType.Guide);
        SpriteAnim4.gameObject.SetActive(MapLG.E.IsEquipTutorial());
    }

    private void EnActiveBtn(Button btn, bool b = true)
    {
        btn.GetComponent<Image>().color = b ?
            Color.gray :
            Color.white;
        btn.enabled = !b;
    }
}
