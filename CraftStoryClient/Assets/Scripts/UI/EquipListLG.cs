using LitJson;
using System.Collections.Generic;

public class EquipListLG : UILogicBase<EquipListLG, EquipListUI>
{
    public void GetList()
    {
        NWMng.E.GetEquipmentInfoList((rp) => 
        {
            if (string.IsNullOrEmpty(rp.ToString()))
            {
                UI.RefreshCell(null);
                return;
            }
            else
            {
                var result = JsonMapper.ToObject<List<EquipListRP>>(rp.ToJson());
                UI.RefreshCell(result);
            }
        });
    }

    public void AppraisalEquipment(ItemEquipmentData data, EquipListCell cell)
    {
        NWMng.E.AppraisalEquipment((rp) => 
        {
            cell.AppraisalEquipment((string)rp);
        }, data.id, data.equipmentConfig.ID);
    }

    public struct EquipListRP
    {
        public int id { get; set; }
        public int itemId { get; set; }
        public int islocked { get; set; }
        public string skills { get; set; }
    }
}