using System;

public class GachaAddBonusLG : UILogicBase<GachaAddBonusLG, GachaAddBonusUI>
{
    bool isFlipping = false;
    public bool IsFlipping { get => isFlipping; }

    public void OnOpen()
    {
        isFlipping = false;
    }

    public void CardFlipping()
    {
        isFlipping = true;
    }
}