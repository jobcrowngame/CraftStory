
using JsonConfigData;

public class CraftLG : UILogicBase<CraftLG, CraftUI>
{
    public int SelectCount
    {
        get => selectCount;
        set
        {
            selectCount = value;

            UI.SetSelectCountText(value.ToString());
        }
    }
    int selectCount = 1;


    public Craft SelectCraft
    {
        get => selectCraft;
        set => selectCraft = value;
    }
    Craft selectCraft;


    public void OnClickAdd()
    {
        if (selectCount < 100)
        {
            SelectCount++;
            UI.RefreshCost();
        }
    }
    public void OnClickAdd10()
    {
        if (selectCount < 100)
        {
            SelectCount += 10;

            if (SelectCount >= 100)
                SelectCount = 99;

            UI.RefreshCost();
        }
    }
    public void OnClickRemove()
    {
        if (selectCount > 1)
        {
            SelectCount--;
            UI.RefreshCost();
        }
    }
    public void OnClickRemove10()
    {
        if (selectCount > 1)
        {
            SelectCount -= 10;

            if (SelectCount < 1)
                SelectCount = 1;

            UI.RefreshCost();
        }
    }
}
