using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public class LocalData
{
    public ItemTable ItemT;
    //public EquipmentTable EquipmentT;
    public limitedTable limitedT;
    public Statistics_userTable Statistics_userT;
    public UserDataTable UserDataT;

    public LocalData()
    {
        ItemT = new ItemTable();
        //EquipmentT = new EquipmentTable();
        limitedT = new limitedTable();
        Statistics_userT = new Statistics_userTable();
        UserDataT = new UserDataTable();
    }
}
