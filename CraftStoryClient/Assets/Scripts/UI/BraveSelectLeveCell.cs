using JsonConfigData;
using UnityEngine.UI;

public class BraveSelectLeveCell : UIBase
{
    MyButton btn { get => GetComponent<MyButton>(); }
    Text Text { get => FindChiled<Text>("Text"); }
    private Map config;

    private void Awake()
    {
        btn.AddClickListener((i) =>
        {
            PlayerCtl.E.Lock = false;

            // –`Œ¯“ü‚é‚Ìƒ~ƒbƒVƒ‡ƒ“
            NWMng.E.ClearMission(2, 1);

            CommonFunction.GoToNextScene(config.TransferGateID);
        });
    }

    public void Set(Map config)
    {
        this.config = config;
        Text.text = (config.Floor + 1) + "F";
    }
}
