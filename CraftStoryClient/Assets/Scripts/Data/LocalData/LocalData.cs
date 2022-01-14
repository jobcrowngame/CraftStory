using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public class LocalData
{
    public ItemTable ItemT;
    public UserDataTable UserDataT;

    public LocalData()
    {
        ItemT = new ItemTable();
        UserDataT = new UserDataTable();
    }
}
