using UnityEngine;
using UnityEngine.UI;

public class MapUI : UIBase
{
    Button CloseBtn { get => FindChiled<Button>("CloseBtn"); }
    Button HomeBtn { get => FindChiled<Button>("HomeBtn"); }
    Button MarketBtn { get => FindChiled<Button>("MarketBtn"); }
    Button BraveBtn { get => FindChiled<Button>("BraveBtn"); }
    Transform SpriteAnim { get => FindChiled("SpriteAnim"); }

    public override void Init()
    {
        base.Init();

        CloseBtn.onClick.AddListener(Close);

        HomeBtn.onClick.AddListener(() =>
        {
            GuideLG.E.Next();

            if (DataMng.E.RuntimeData.MapType != MapType.Home)
                CommonFunction.GoToNextScene(100);
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
                if (DataMng.E.RuntimeData.MapType != MapType.Market)
                    CommonFunction.GoToNextScene(103);
            }
        });

        BraveBtn.onClick.AddListener(() =>
        {
            // –`Œ¯“ü‚é‚Ìƒ~ƒbƒVƒ‡ƒ“
            NWMng.E.ClearMission(2, 1);

            if (DataMng.E.RuntimeData.MapType != MapType.Brave)
                CommonFunction.GoToNextScene(1000);
        });
    }

    public override void Open()
    {
        base.Open();

        EnActiveBtn(HomeBtn, DataMng.E.RuntimeData.MapType == MapType.Home);
        EnActiveBtn(MarketBtn, DataMng.E.RuntimeData.MapType == MapType.Market);
        EnActiveBtn(BraveBtn, DataMng.E.RuntimeData.MapType == MapType.Brave);

        SpriteAnim.gameObject.SetActive(DataMng.E.RuntimeData.MapType != MapType.Guide && DataMng.E.RuntimeData.GuideEnd2 == 0);
    }

    private void EnActiveBtn(Button btn, bool b = true)
    {
        btn.GetComponent<Image>().color = b ?
            Color.gray :
            Color.white;
        btn.enabled = !b;
    }
}
