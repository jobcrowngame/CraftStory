using JsonConfigData;
using LitJson;
using UnityEngine;

public class BraveSelectLevelLG : UILogicBase<BraveSelectLevelLG, BraveSelectLevelUI>
{
    public void AddCells()
    {
        NWMng.E.GetMaxBraveLevel((rp) =>
        {
            var result = JsonMapper.ToObject<BraveSelectLevelRP>(rp.ToJson());
            int count = Mathf.Abs(result.maxArrivedFloor - 1) / 5;

            UI.SetMaxFloor(result.maxArrivedFloor);

            for (int i = 0; i <= count; i++)
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
        var floor = index * 5 + 1;
        foreach (var item in ConfigMng.E.Map.Values)
        {
            if (floor == item.Floor)
                return item;
        }

        return null;
    }

    struct BraveSelectLevelRP
    {
        public int maxArrivedFloor { get; set; }
    }
}