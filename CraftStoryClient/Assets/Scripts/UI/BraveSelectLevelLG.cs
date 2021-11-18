using JsonConfigData;

public class BraveSelectLevelLG : UILogicBase<BraveSelectLevelLG, BraveSelectLevelUI>
{
    public void AddCells()
    {
        NWMng.E.GetMaxBraveLevel((rp) =>
        {
            int maxLv = (int)rp["maxArrivedFloor"];
            int count = maxLv / 5;

            for (int i = 0; i < count; i++)
            {
                var map = GetMapIdByIndex(i);
                if (map != null)
                {
                    UI.AddCell(map);
                }
            }
        });
    }

    private Map GetMapIdByIndex(int index)
    {
        var floor = index * 5;
        if (floor <= 0) floor = 1;

        if (floor == 1)
        {
            return ConfigMng.E.Map[1000];
        }
        else
        {
            foreach (var item in ConfigMng.E.Map.Values)
            {
                if (floor == item.Floor)
                    return item;
            }
        }

        return null;
    }
}