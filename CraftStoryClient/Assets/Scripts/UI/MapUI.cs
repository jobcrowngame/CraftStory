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
            // –`Œ¯“ü‚é‚Ìƒ~ƒbƒVƒ‡ƒ“
            NWMng.E.ClearMission(2, 1);

            if (DataMng.E.RuntimeData.MapType != MapType.Brave)
                CommonFunction.GoToNextScene(1000);
        });
    }
}
