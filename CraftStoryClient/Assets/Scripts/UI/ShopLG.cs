using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ShopLG : UILogicBase<ShopLG, ShopUI>
{
    private string showcaseName;
    public string ShowcaseName
    {
        get { return showcaseName; }
    }

    public void GetShowcase(string showcaseName)
    {
        GS2.E.GetShowcase((r) =>
        {
            this.showcaseName = r.Name;

            ui.AddItems(r);
        }, showcaseName);
    }
}