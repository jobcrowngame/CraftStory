using UnityEngine.UI;

public class GachaRatioCell : UIBase
{
    Text Rare { get => FindChiled<Text>("Level"); }
    Text Name { get => FindChiled<Text>("Name"); }
    Text Percent { get => FindChiled<Text>("Percent"); }

    public void Set(int bonus, int percent, int rare)
    {
        Rare.text = GetRareString(rare);
        Percent.text = percent / 10f + "%";

        var config = ConfigMng.E.Bonus[bonus];
        Name.text = ConfigMng.E.Item[config.Bonus1].Name + "X" + config.BonusCount1;
    }

    private string GetRareString(int rare)
    {
        switch (rare)
        {
            case 3: return "★★★";
            case 2: return "★★";
            case 1: return "★";
            default: return "★";
        }
    }
}