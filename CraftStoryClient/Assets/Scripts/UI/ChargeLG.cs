using Gs2.Weave.Core.CallbackEvent;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weave.Core.Runtime;

public class ChargeLG : UILogicBase<ChargeLG, ChargeUI>
{
    public void Buy()
    {

    }

   public void ShowItems()
    {
         GS2.E.GetChargeList(
            ui.onGetShowcase,
            ui.onError);
    }
}