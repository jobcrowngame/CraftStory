using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public class LocalData
{
    public ItemTable ItemT;
    public LimitedTable LimitedT;
    public StatisticsUserTable StatisticsUserT;
    public UserDataTable UserDataT;

    public LocalData()
    {
        ItemT = new ItemTable();
        LimitedT = new LimitedTable();
        StatisticsUserT = new StatisticsUserTable();
        UserDataT = new UserDataTable();
    }
}
