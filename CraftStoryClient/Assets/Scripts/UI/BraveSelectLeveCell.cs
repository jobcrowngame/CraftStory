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
            if (MapLG.E.BtnIsLocked)
                return;

            MapLG.E.LockBtn();

            if (AdventureCtl.E.BonusList.Count > 0)
            {
                AdventureCtl.E.GetBonus(() =>
                {
                    MoveScene();
                });
            }
            else
            {
                MoveScene();
            }
        });
    }

    private void MoveScene()
    {
        PlayerCtl.E.Lock = false;

        CommonFunction.GoToNextScene(config.ID);
    }

    public void Set(Map config)
    {
        this.config = config;
        Text.text = config.Floor + "F";
    }
}
