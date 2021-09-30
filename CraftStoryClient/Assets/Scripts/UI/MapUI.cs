using UnityEngine;
using UnityEngine.UI;

public class MapUI : UIBase
{
    Button CloseBtn { get => FindChiled<Button>("CloseBtn"); }
    Button HomeBtn { get => FindChiled<Button>("HomeBtn"); }
    Button MarketBtn { get => FindChiled<Button>("MarketBtn"); }
    Button BraveBtn { get => FindChiled<Button>("BraveBtn"); }

    public override void Init()
    {
        base.Init();

        CloseBtn.onClick.AddListener(Close);

        HomeBtn.onClick.AddListener(() =>
        {
            if (DataMng.E.RuntimeData.MapType != MapType.Home)
                CommonFunction.GoToNextScene(100);
        });

        MarketBtn.onClick.AddListener(() =>
        {
            if (DataMng.E.RuntimeData.MapType != MapType.Market)
                CommonFunction.GoToNextScene(103);
        });

        BraveBtn.onClick.AddListener(() =>
        {
            // 冒険入るのミッション
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
    }

    private void EnActiveBtn(Button btn, bool b = true)
    {
        btn.GetComponent<Image>().color = b ?
            Color.gray :
            Color.white;
        btn.enabled = !b;
    }
}
